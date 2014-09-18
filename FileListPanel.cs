/* The File Commander - plugin API
 * The file list widget
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Zhigunov Andrew (breakneck11@gmail.com)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using pluginner;
using pluginner.Toolkit;
using pluginner.Widgets;
using Xwt;
using Xwt.Drawing;

namespace fcmd
{
	/// <summary>Filelist panel</summary>
	public class FileListPanel : Table
	{
		//Data Field Numbers
		//they aren't const because they may change when the columns are reordered
		public int dfIcon = 0;
		public int dfURL = 1;
		public int dfDisplayName = 2;
		public int dfSize = 3;
		public int dfChanged = 4;
		public int dfDirItem = 5;

		public IFSPlugin FS;
		public LightScroller DiskBox = new LightScroller();
		public HBox DiskList = new HBox();
		public List<Button> DiskButtons = new List<Button>();
		public Button GoRoot = new Button("/");
		EventHandler goRootDelegate = null;
		public Button GoUp = new Button("..");
		EventHandler goUpDelegate = null;
		public TextEntry UrlBox = new TextEntry();
		public MenuButton BookmarksButton = new MenuButton(Image.FromResource("fcmd.Resources.bookmarks.png"));
		public MenuButton HistoryButton = new MenuButton(Image.FromResource("fcmd.Resources.history.png"));
		public ListView2 ListingView = new ListView2();
		public HBox QuickSearchBox = new HBox();
		public TextEntry QuickSearchText = new TextEntry();//по возможность заменить на SearchTextEntry (не раб. на wpf, see xwt bug 330)
		public Label StatusBar = new Label("Information bar");
		public Table StatusTable = new Table();
		public ProgressBar StatusProgressbar = new ProgressBar();
		TextEntry CLIoutput = new TextEntry { MultiLine = true, ShowFrame = true, Visible = false, HeightRequest = 50 };
		TextEntry CLIprompt = new TextEntry();

		/// <summary>User navigates into another directory</summary>
		public event TypedEvent<string> Navigate;
		/// <summary>User tried to open the highlighted file</summary>
		public event TypedEvent<string> OpenFile;

		public SizeDisplayPolicy CurShortenKB, CurShortenMB, CurShortenGB;
		private string SBtext1, SBtext2;
		private Stylist s;

		/// <summary>Initialize the FLP</summary>
		/// <param name="BookmarkXML">The bookmark database</param>
		/// <param name="CSS">The user theme (or null if it's need to use internal theme)</param>
		/// <param name="InfobarText1">The mask for infobar text when a file is selected</param>
		/// <param name="InfobarText2">The mask for infobar text when no files are selected</param>
		public FileListPanel(string BookmarkXML = null, string CSS=null, string InfobarText1 = "{Name}", string InfobarText2 = "F: {FileS}, D: {DirS}")
		{
			s = new Stylist(CSS);
			SBtext1 = InfobarText1;
			SBtext2 = InfobarText2;
			BuildUI(BookmarkXML);
			DiskBox.Content = DiskList;
			DiskBox.CanScrollByY = false;

			GoRoot.ExpandHorizontal = GoUp.ExpandHorizontal = BookmarksButton.ExpandHorizontal = HistoryButton.ExpandHorizontal = false;
			GoRoot.Style = GoUp.Style = BookmarksButton.Style = HistoryButton.Style = ButtonStyle.Flat;
			GoRoot.CanGetFocus = GoUp.CanGetFocus = BookmarksButton.CanGetFocus = HistoryButton.CanGetFocus = false;

			HistoryButton.Menu = new Menu();

			DefaultColumnSpacing = 0;
			DefaultRowSpacing = 0;

			string fontFamily = fcmd.Properties.Settings.Default.UserFileListFontFamily;
			ListingView.FontForFileNames = String.IsNullOrWhiteSpace(fontFamily) ? Font.SystemFont : Font.FromName(fontFamily);

			Add(DiskBox,0,0, 1,1,true,false,WidgetPlacement.Fill);
			Add(GoRoot,1,0 ,1,1,false,false,WidgetPlacement.Fill);
			Add(GoUp,2,0 ,1,1,false,false,WidgetPlacement.Fill);
			Add(UrlBox,0,1, 1,1,true,false,WidgetPlacement.Fill);
			Add(BookmarksButton,1,1 ,1,1,false,false,WidgetPlacement.Start);
			Add(HistoryButton,2,1 ,1,1,false,false,WidgetPlacement.Start);
			Add(ListingView,0,2 ,1,3,false,true); //hexpand will be = 'true' without seeing to this 'false'
			Add(QuickSearchBox,0,3 ,1,3);
			Add(StatusBar,0,4,1,3);
			Add(StatusProgressbar,0,5,1,3);
			Add(CLIoutput,0,6 ,1,3);
			Add(CLIprompt,0,7 ,1,3);

			WriteDefaultStatusLabel();

			CLIprompt.KeyReleased += CLIprompt_KeyReleased;

			QuickSearchText.GotFocus += (o, ea) => { OnGotFocus(ea); };
			QuickSearchText.KeyPressed += QuickSearchText_KeyPressed;
			QuickSearchBox.PackStart(QuickSearchText, true, true);
			QuickSearchBox.Visible = false;
		}

		void QuickSearchText_KeyPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
			{
				QuickSearchText.Text = "";
				QuickSearchBox.Visible = false;
				ListingView.AllowedToPoint.Clear();
				return;
			}

			//search for good items
			ListingView.Sensitive = false;
			ListingView.AllowedToPoint.Clear();
			foreach (ListView2Item lvi in ListingView.Items)
			{
				if(lvi.Data[1].ToString().StartsWith(QuickSearchText.Text)){
					ListingView.AllowedToPoint.Add(lvi.RowNo);
				}
			}
			ListingView.Sensitive = true;

			//set pointer to the first good item (if need)
			if (ListingView.AllowedToPoint.Count > 0){
				if (ListingView.SelectedRow < ListingView.AllowedToPoint[0]
					||
					ListingView.SelectedRow > ListingView.AllowedToPoint[ListingView.AllowedToPoint.Count-1]
					)
				{
					ListingView.SelectedRow = ListingView.AllowedToPoint[0];
					ListingView.ScrollToRow(ListingView.AllowedToPoint[0]);
				}
			}
		}

		void CLIprompt_KeyReleased(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return){
				if (Regex.Match(CLIprompt.Text, "cd|chdir|md|rd|del|deltree|move|copy|cls").Success)
				{
					CLIprompt.Text = "";
					//todo: обработка встроенных комманд
					return;
				}

				CLIoutput.Visible = true;
				string stdin = CLIprompt.Text;
				CLIprompt.Text = "";
				CLIoutput.Text += stdin;
				FS.CLIstdinWriteLine(stdin);
			}
		}

		void FS_CLIpromptChanged(string data)
		{
			CLIprompt.PlaceholderText = data;
		}

		void FS_CLIstdoutDataReceived(string data)
		{
			Application.Invoke(delegate
			{
				CLIoutput.Text += "\n" + data;
			});
		}

		/// <summary>Make the panel's widgets</summary>
		/// <param name="BookmarkXML">Bookmark list XML data</param>
		public void BuildUI(string BookmarkXML = null)
		{
			//URL BOX
			UrlBox.ShowFrame = false;
			UrlBox.GotFocus += (o, ea) => { OnGotFocus(ea); };
			UrlBox.KeyReleased += UrlBox_KeyReleased;

			BookmarkTools bmt = new BookmarkTools(BookmarkXML,"QuickAccessBar");
			bmt.DisplayBookmarks(
				DiskList,
				(url => NavigateTo(url)),
				s
			);

			bmt = new BookmarkTools(BookmarkXML);
			BookmarksButton.Menu = new Menu();
			bmt.DisplayBookmarks(
				BookmarksButton.Menu,
				(url => NavigateTo(url))
			);

			foreach (Button b in DiskButtons)
			{
				s.Stylize(b);
			}
			s.Stylize(DiskBox);
			s.Stylize(UrlBox);
			s.Stylize(ListingView);
			s.Stylize(QuickSearchBox);
			s.Stylize(CLIoutput,"TerminalOutput");
			s.Stylize(CLIprompt,"TerminalPrompt");
			s.Stylize(StatusTable);

			ListingView.KeyReleased += ListingView_KeyReleased;
			ListingView.GotFocus += (o, ea) =>{ OnGotFocus(ea); };
			ListingView.PointerMoved += ListingView_PointerMoved;
			ListingView.SelectionChanged += ListingView_SelectionChanged;
			ListingView.PointedItemDoubleClicked += pointed_item => { OpenPointedItem(); };
			ListingView.EditComplete += ListingView_EditComplete;
			StatusBar.Wrap = WrapMode.Word;
		}

		void ListingView_EditComplete(EditableLabel el, ListView2 lv)
		{
			string Url1 = FS.CurrentDirectory + FS.DirSeparator + ListingView.PointedItem.Data[dfDisplayName];
			string Url2 = FS.CurrentDirectory + FS.DirSeparator + el.Text;
			try { 
				if(FS.DirectoryExists(Url1))
					FS.MoveDirectory(Url1, Url2);
				else
					FS.MoveFile(Url1, Url2);
				StatusBar.Text = ListingView.PointedItem.Data[dfDisplayName] + " → " + el.Text;
			}
			catch(Exception ex) {
				MessageDialog.ShowWarning(ex.Message);
				el.Text = ListingView.PointedItem.Data[dfDisplayName].ToString();
			}
		}

		void ListingView_SelectionChanged(List<ListView2Item> data)
		{
			WriteDefaultStatusLabel();
		}

		void ListingView_PointerMoved(ListView2Item data)
		{
			WriteDefaultStatusLabel();
		}

		void UrlBox_KeyReleased(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				LoadDir(UrlBox.Text);
			}
		}

		void ListingView_KeyReleased(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Return && ListingView.SelectedRow > -1)
			{
				OpenPointedItem();
				return;
			}
			if ((int)e.Key < 65000) //keys before 65000th are characters, numbers & other human stuff
			{
				QuickSearchText.Text += e.Key.ToString();
				QuickSearchBox.Visible = true;
				QuickSearchText.SetFocus();
				return;
			}
			if(Utilities.GetXwtBackendName() == "WPF")
			ListingView.OnKeyPressed(e);
		}

		void OpenPointedItem()
		{
			NavigateTo(ListingView.PointedItem.Data[dfURL].ToString());
		}
		
		/// <summary>Open the FS item at <paramref name="url"/> (if it's file, load; if it's directory, go to)</summary>
		/// <param name="url">The URL of the filesystem entry</param>
		/// <param name="ClearHistory">The number of the history entrie after that all entries should be removed</param>
		private void NavigateTo(string url, int? ClearHistory = null)
		{
			if (!url.Contains("://")){
				//the path is relative
				NavigateTo(FS.CurrentDirectory + FS.DirSeparator + url);
			}

			Menu hm = HistoryButton.Menu;

			if (ClearHistory == null){
				//register current directory in history
				MenuItem hmi = new MenuItem(url);
				hmi.Clicked+=(o,ea)=>{ NavigateTo(url,(int)hmi.Tag); };
				hmi.Tag = hm.Items.Count;
				hm.Items.Add(hmi);
			}
			if (ClearHistory != null){
				//loading from history menu, thus don't making duplicates.
			}


			try
			{
				if (FS.DirectoryExists(url))
				{//it's directory
					var navigate = Navigate;
					if (navigate != null) {
						navigate(url); //raise event
					} else {
						Console.WriteLine("WARNING: the event FLP.Navigate was not handled by the host");
					}

					LoadDir(url);
					return;
				}
				else
				{//it's file
					var openFile = OpenFile;
					if (openFile != null) {
						openFile(url); //raise event
					} else {
						Console.WriteLine("WARNING: the event FLP.OpenFile was not handled by the host");
					}
				}
			}
			catch (PleaseSwitchPluginException)
			{
				throw; //delegate authority to the mainwindow (it is it's jurisdiction).
			}
			catch (Exception ex)
			{
				ListingView.Sensitive = true;
				ListingView.Cursor = CursorType.Arrow;

				MessageDialog.ShowError(ex.Message);
				Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
				WriteDefaultStatusLabel();
			}
		}

		/// <summary>
		/// Load the specifed directory with specifed content into the panel and set view options
		/// </summary>
		/// <param name="URL">The full URL of the directory (for reference needs)</param>
		/// <param name="ShortenKB">How kilobyte sizes should be humanized</param>
		/// <param name="ShortenMB">How megabyte sizes should be humanized</param>
		/// <param name="ShortenGB">How gigabyte sizes should be humanized</param> //плохой перевод? "так nбайтные размеры должны очеловечиваться"
		public void LoadDir(string URL, SizeDisplayPolicy ShortenKB, SizeDisplayPolicy ShortenMB, SizeDisplayPolicy ShortenGB)
		{
			CurShortenKB = ShortenKB; CurShortenMB = ShortenMB; CurShortenGB = ShortenGB;

			if (FS == null) throw new InvalidOperationException("No filesystem is binded to this FileListPanel");

			//неспешное TODO:придумать, куда лучше закорячить; не забываем, что во время работы FS может меняться полностью
			FS.CLIstdoutDataReceived += FS_CLIstdoutDataReceived;
			FS.CLIstderrDataReceived += (stderr) => { CLIoutput.Text += "\n" + stderr; Utilities.ShowWarning(stderr); };
			FS.CLIpromptChanged += FS_CLIpromptChanged;

			if (FS.CurrentDirectory == null){
				//if this is first call in the session (the FLP is just initialized)
				using (Menu hm = HistoryButton.Menu) { 
					MenuItem hmi = new MenuItem(URL);
					hmi.Clicked += (o, ea) => { NavigateTo(URL, (int)hmi.Tag); };
					hmi.Tag = hm.Items.Count;
					hm.Items.Add(hmi);
				}
				FS.StatusChanged += FS_StatusChanged;
				FS.ProgressChanged += FS_ProgressChanged;
			}

			if (URL == "." && FS.CurrentDirectory == null){
				LoadDir(
					"file://"+Directory.GetCurrentDirectory(),
					ShortenKB,
					ShortenMB,
					ShortenGB
				);
				return;
			}

			ListingView.Cursor = CursorType.Wait;
			ListingView.Sensitive = false;

			string oldCurDir = FS.CurrentDirectory;

			try
			{
				FS.CurrentDirectory = URL;
				UrlBox.Text = URL;
				ListingView.Clear();
				UrlBox.Text = URL;
				string updir = URL + FS.DirSeparator+"..";
				string rootdir = FS.GetMetadata(URL).RootDirectory;
				uint counter = 0;
				const uint per_number = ~(((~(uint)0) >> 10) << 10);
				IEnumerable<DirItem> dis = FS.DirectoryContent;
				foreach (DirItem di in dis.Select(dc => { dc.IconSmall = Utilities.GetIconForMIME(dc.MIMEType); return dc; }))
				{
					List<Object> Data = new List<Object>();
					List<Boolean> EditableFileds = new List<bool>();

					Data.Add(di.IconSmall ?? Image.FromResource("fcmd.Resources.image-missing.png")); EditableFileds.Add(false);
					Data.Add(di.URL); EditableFileds.Add(false);
					Data.Add(di.TextToShow); EditableFileds.Add(true);
					if (di.TextToShow == "..")
					{//parent dir
						Data.Add("<↑ UP>"); EditableFileds.Add(false); EditableFileds[2] = false;
						Data.Add(FS.GetMetadata(di.URL).LastWriteTimeUTC.ToLocalTime()); EditableFileds.Add(false);
						updir = di.URL;
					}
					else if (di.IsDirectory)
					{//dir
						Data.Add("<DIR>"); EditableFileds.Add(false);
						Data.Add(di.Date); EditableFileds.Add(false);
					}
					else
					{//file
						Data.Add(KiloMegaGigabyteConvert(di.Size, ShortenKB, ShortenMB, ShortenGB)); EditableFileds.Add(false);
						Data.Add(di.Date); EditableFileds.Add(false);
					}
					Data.Add(di);
					ListingView.AddItem(Data, EditableFileds, di.URL);
					if ((++counter & per_number) == 0) {
						Application.MainLoop.DispatchPendingEvents();
					}
				}
				if (goUpDelegate != null) {
					GoUp.Clicked -= goUpDelegate;
				}
				goUpDelegate = (o,ea)=>{ LoadDir(updir); };
				GoUp.Clicked += goUpDelegate;
				if (goRootDelegate != null) {
					GoRoot.Clicked -= goRootDelegate;
				}
				goRootDelegate = (o,ea)=>{ LoadDir(rootdir); };
				GoRoot.Clicked += goRootDelegate;
			}
			catch (Exception ex)
			{
				if (ex is pluginner.PleaseSwitchPluginException)
				{
					pluginfinder pf = new pluginfinder();
					FS = pf.GetFSplugin(URL);
					LoadDir(URL,ShortenKB,ShortenMB,ShortenGB);
				}
				else if (ex is NullReferenceException)
				{
					MessageDialog.ShowWarning(ex.Message, ex.StackTrace + "\nInner exception: " + ex.InnerException.Message ?? "none");
					LoadDir(oldCurDir, ShortenKB, ShortenMB, ShortenGB);
				}
				else
				{
					MessageDialog.ShowWarning(ex.Message);
					LoadDir(oldCurDir, ShortenKB, ShortenMB, ShortenGB);
				}
			}
			if (ListingView.Items.Count > 0)
			{ ListingView.SelectedRow = 0; ListingView.ScrollerIn.ScrollTo(0, 0); }
			ListingView.SetFocus();//one fixed bug may make many other bugs...уточнить необходимость!
			ListingView.Sensitive = true;
			ListingView.Cursor = CursorType.Arrow;
		}

		private void FS_StatusChanged(string data)
		{
			if (data.Length == 0)
				WriteDefaultStatusLabel();
			else
				StatusBar.Text = data;
		}

		private void FS_ProgressChanged(double data)
		{
			if (data > 0 && data <= 1){
				StatusProgressbar.Visible = true;
				StatusProgressbar.Fraction = data;
			}
			else
			{
				StatusProgressbar.Visible = false;
			}
		}

		/// <summary>
		/// Reloads the current directory
		/// </summary>
		public void LoadDir()
		{
			LoadDir(FS.CurrentDirectory);
		}

		/// <summary>
		/// Load the directory into the panel
		/// </summary>
		/// <param name="URL">Full path of the directory</param>
		public void LoadDir(string URL)
		{
			LoadDir(URL, CurShortenKB, CurShortenMB, CurShortenGB);
		}

		/// <summary>Converts the file size (in bytes) to human-readable string</summary>
		/// <param name="Input">The input value</param>
		/// <returns>Human-readable string (xxx yB)</returns>
		private string KiloMegaGigabyteConvert(long Input, SizeDisplayPolicy ShortenKB, SizeDisplayPolicy ShortenMB, SizeDisplayPolicy ShortenGB)
		{
			double ShortenedSize; //here will be writed the decimal value of the hum. readable size

			//TeraByte (will be shortened everywhen)
			if (Input > 1099511627776) return (Input / 1099511627776) + " TB";

			//GigaByte
			if (Input > 1073741824)
			{
				ShortenedSize = Input / 1073741824;
				switch (ShortenGB)
				{
					case SizeDisplayPolicy.OneNumeral:
						return string.Format("{0:0.#} GB", ShortenedSize);
					case SizeDisplayPolicy.TwoNumeral:
						return string.Format("{0:0.##} GB", ShortenedSize);
				}
			}

			//MegaByte
			if (Input > 1048576)
			{
				ShortenedSize = Input / 1048576;
				switch (ShortenMB)
				{
					case SizeDisplayPolicy.OneNumeral:
						return string.Format("{0:0.#} MB", ShortenedSize);
					case SizeDisplayPolicy.TwoNumeral:
						return string.Format("{0:0.##} MB", ShortenedSize);
				}
			}

			//KiloByte
			if (Input > 1024)
			{
				ShortenedSize = Input / 1024;
				switch (ShortenKB)
				{
					case SizeDisplayPolicy.OneNumeral:
						return string.Format("{0:0.#} KB", ShortenedSize);
					case SizeDisplayPolicy.TwoNumeral:
						return string.Format("{0:0.##} KB", ShortenedSize);
				}
			}

			return Input + " B"; //if Input is less than 1k or shortening is disallowed
		}

		/// <summary>Defines the size shortening policy</summary>
		public enum SizeDisplayPolicy
		{
			DontShorten=0, OneNumeral=1, TwoNumeral=2
			//2048 B, 2 KB, 2.0 KB
		}
		
		/// <summary>
		/// Gets the selected row's value from the column №<paramref name="Field"/>
		/// </summary>
		/// <typeparam name="T">The type of the data</typeparam>
		/// <param name="Field">The field number</param>
		/// <returns>The value</returns>
		public T GetValue<T>(int Field){
			return (T)ListingView.PointedItem.Data[Field];
		}

		public string GetValue(int Field){
			return (string)ListingView.PointedItem.Data[Field];
		}

		/// <summary>Add autobookmark "system disks" onto disk toolbar</summary>
		private void AddSysDrives()
		{
			foreach (DriveInfo di in DriveInfo.GetDrives())
			{
				string d = di.Name;
				Button NewBtn = new Button(null, d);
				NewBtn.Clicked += (o, ea) => { NavigateTo("file://" + d); };
				NewBtn.CanGetFocus = false;
				NewBtn.Style = ButtonStyle.Flat;
				NewBtn.Margin = -3;
				NewBtn.Cursor = CursorType.Hand;
				NewBtn.Sensitive = di.IsReady;
				if (di.IsReady)
				{
					NewBtn.TooltipText = di.VolumeLabel + " (" + di.DriveFormat + ")";
				}
				/* todo: rewrite the code; possibly change the XWT to allow
				 * change the internal padding of the button.
				 */
				switch (di.DriveType)
				{
					case DriveType.Fixed:
						NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.drive-harddisk.png");
						break;
					case DriveType.CDRom:
						NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.drive-optical.png");
						break;
					case DriveType.Removable:
						NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.drive-removable-media.png");
						break;
					case DriveType.Network:
						NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.network-server.png");
						break;
					case DriveType.Ram:
						NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.emblem-system.png");
						break;
					case DriveType.Unknown:
						NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.image-missing.png");
						break;
				}

				//OS-specific icons
				if (d.StartsWith("A:")) NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.media-floppy.png");
				if (d.StartsWith("B:")) NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.media-floppy.png");
				if (d.StartsWith("/dev")) NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.preferences-desktop-peripherals.png");
				if (d.StartsWith("/proc")) NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.emblem-system.png");
				if (d == "/") NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.root-folder.png");

				s.Stylize(NewBtn);
				DiskList.PackStart(NewBtn);
			}
		}

		/// <summary>Add buttons of mounted medias (*nix)</summary>
		private void AddLinuxMounts()
		{
			if (Directory.Exists(@"/mnt"))
			{
				foreach (string dir in Directory.GetDirectories(@"/mnt/"))
				{
					Button NewBtn = new Button(null, dir.Replace("/mnt/",""));
					NewBtn.Clicked += (o, ea) => { NavigateTo("file://" + dir); };
					NewBtn.CanGetFocus = false;
					NewBtn.Style = ButtonStyle.Flat;
					NewBtn.Margin = -3;
					NewBtn.Cursor = CursorType.Hand;
					NewBtn.Image = Image.FromResource(GetType(), "fcmd.Resources.drive-removable-media.png");

					s.Stylize(NewBtn);
					DiskList.PackStart(NewBtn);
				}
			}
			else AddSysDrives(); //fallback for Windows
		}

		/// <summary>
		/// Writes to statusbar the default text
		/// </summary>
		private void WriteDefaultStatusLabel()
		{
			StatusProgressbar.Visible = false;
			if(ListingView.SelectedItems.Count<1)
			StatusBar.Text = MakeStatusbarText(SBtext1);
			else
			StatusBar.Text = MakeStatusbarText(SBtext2);
		}

		private string MakeStatusbarText(string Template)
		{
			string txt = Template;
			if (ListingView.PointedItem != null) { 
				DirItem di = (DirItem)ListingView.PointedItem.Data[dfDirItem];
				txt = txt.Replace("{FullName}", di.TextToShow);
				txt = txt.Replace("{AutoSize}", KiloMegaGigabyteConvert(di.Size,CurShortenKB,CurShortenMB,CurShortenMB));
				txt = txt.Replace("{Date}", di.Date.ToShortDateString());
				txt = txt.Replace("{Time}", di.Date.ToLocalTime().ToShortTimeString());
				txt = txt.Replace("{SelectedItems}", ListingView.SelectedItems.Count.ToString());
				//todo: add masks SizeB, SizeKB, SizeMB, TimeUTC, Name, Extension
			}
			return txt;
		}
	}
}
