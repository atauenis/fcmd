﻿/* The File Commander main window
 * The main file manager window
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.IO;
using pluginner.Toolkit;
using pluginner.Widgets;

namespace fcmd
{
	partial class MainWindow : Xwt.Window
	{
		static string ProductVersion {
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		Stylist stylist;
		Xwt.Menu WindowMenu = new Xwt.Menu();

		Xwt.MenuItem mnuFile = new Xwt.MenuItem { Tag="mnuFile" };
		Xwt.MenuItem mnuFileUserMenu = new Xwt.MenuItem { Tag = "mnuFileUserMenu" };
		Xwt.MenuItem mnuFileView = new Xwt.MenuItem { Tag = "mnuFileView" };
		Xwt.MenuItem mnuFileEdit = new Xwt.MenuItem { Tag = "mnuFileEdit" };
		Xwt.MenuItem mnuFileCompare = new Xwt.MenuItem { Tag = "mnuFileCompare" };
		Xwt.MenuItem mnuFileCopy = new Xwt.MenuItem { Tag = "mnuFileCopy" };
		Xwt.MenuItem mnuFileMove = new Xwt.MenuItem { Tag = "mnuFileMove" };
		Xwt.MenuItem mnuFileNewDir = new Xwt.MenuItem { Tag = "mnuFileNewDir" };
		Xwt.MenuItem mnuFileRemove = new Xwt.MenuItem { Tag = "mnuFileRemove" };
		Xwt.MenuItem mnuFileAtributes = new Xwt.MenuItem { Tag = "mnuFileAtributes" };
		Xwt.MenuItem mnuFileQuickSelect = new Xwt.MenuItem { Tag = "mnuFileQuickSelect" };
		Xwt.MenuItem mnuFileQuickUnselect = new Xwt.MenuItem { Tag = "mnuFileQuickUnselect" };
		Xwt.MenuItem mnuFileSelectAll = new Xwt.MenuItem { Tag = "mnuFileSelectAll" };
		Xwt.MenuItem mnuFileUnselect = new Xwt.MenuItem { Tag = "mnuFileUnselect" };
		Xwt.MenuItem mnuFileInvertSelection = new Xwt.MenuItem { Tag = "mnuFileInvertSelection" };
		Xwt.MenuItem mnuFileExit = new Xwt.MenuItem { Tag = "mnuFileExit" };

		Xwt.MenuItem mnuView = new Xwt.MenuItem { Tag="mnuView" };
		Xwt.MenuItem mnuViewShort = new Xwt.MenuItem { Tag="mnuViewShort" };
		Xwt.MenuItem mnuViewDetails = new Xwt.MenuItem { Tag="mnuViewDetails" };
		Xwt.MenuItem mnuViewIcons = new Xwt.MenuItem { Tag="mnuViewIcons" };
		Xwt.MenuItem mnuViewThumbs = new Xwt.MenuItem { Tag="mnuViewThumbs" };
		Xwt.MenuItem mnuViewQuickView = new Xwt.MenuItem { Tag="mnuViewQuickView" };
		Xwt.MenuItem mnuViewTree = new Xwt.MenuItem { Tag="mnuViewTree" };
		Xwt.MenuItem mnuViewPCPCconnect = new Xwt.MenuItem { Tag = "mnuViewPCPCconnect" };
		Xwt.MenuItem mnuViewPCNETPCconnect = new Xwt.MenuItem { Tag = "mnuViewPCNETPCconnect" };
		Xwt.MenuItem mnuViewByName = new Xwt.MenuItem { Tag="mnuViewByName" };
		Xwt.MenuItem mnuViewByType = new Xwt.MenuItem { Tag="mnuViewByType" };
		Xwt.MenuItem mnuViewByDate = new Xwt.MenuItem { Tag="mnuViewByDate" };
		Xwt.MenuItem mnuViewBySize = new Xwt.MenuItem { Tag="mnuViewBySize" };
		Xwt.MenuItem mnuViewNoFilter = new Xwt.MenuItem { Tag="mnuViewNoFilter" };
		Xwt.MenuItem mnuViewWithFilter = new Xwt.MenuItem { Tag="mnuViewWithFilter" };

		Xwt.MenuItem mnuNavigate = new Xwt.MenuItem { Tag="mnuNav" };
		Xwt.MenuItem mnuNavigateTree = new Xwt.MenuItem { Tag="mnuNavigateTree" };
		Xwt.MenuItem mnuNavigateFind = new Xwt.MenuItem { Tag="mnuNavigateFind" };
		Xwt.MenuItem mnuNavigateHistory = new Xwt.MenuItem { Tag="mnuNavigateHistory" };
		Xwt.MenuItem mnuNavigateReload = new Xwt.MenuItem { Tag="mnuNavigateReload" };

		Xwt.MenuItem mnuTools = new Xwt.MenuItem { Tag="mnuTools" };
		Xwt.MenuItem mnuToolsOptions = new Xwt.MenuItem { Tag="mnuToolsOptions" };
		Xwt.MenuItem mnuToolsPluginManager = new Xwt.MenuItem { Tag="mnuToolsPluginManager" };
		Xwt.MenuItem mnuToolsEditUserMenu = new Xwt.MenuItem { Tag = "mnuToolsEditUserMenu" };
		Xwt.MenuItem mnuToolsKeychains = new Xwt.MenuItem { Tag="mnuToolsKeychains" };
		Xwt.MenuItem mnuToolsConfigEdit = new Xwt.MenuItem { Tag = "mnuToolsConfigEdit" };
		Xwt.CheckBoxMenuItem mnuViewKeybrdHelp = new Xwt.CheckBoxMenuItem { Tag = "mnuViewKeybrdHelp" };
		Xwt.CheckBoxMenuItem mnuViewInfobar = new Xwt.CheckBoxMenuItem { Tag = "mnuViewInfobar" };
		Xwt.CheckBoxMenuItem mnuViewDiskButtons = new Xwt.CheckBoxMenuItem { Tag = "mnuViewDiskButtons" };
		Xwt.MenuItem mnuToolsDiskLabel = new Xwt.MenuItem { Tag = "mnuToolsDiskLabel" };
		Xwt.MenuItem mnuToolsFormat = new Xwt.MenuItem { Tag = "mnuToolsFormat" };
		Xwt.MenuItem mnuToolsSysInfo = new Xwt.MenuItem { Tag = "mnuToolsSysInfo" };

		Xwt.MenuItem mnuHelp = new Xwt.MenuItem { Tag = "mnuHelp" };
		Xwt.MenuItem mnuHelpHelpMe = new Xwt.MenuItem { Tag = "mnuHelpHelpMe" };
		Xwt.MenuItem mnuHelpDebug = new Xwt.MenuItem { Tag="mnuHelpDebug" };
		Xwt.MenuItem mnuHelpAbout = new Xwt.MenuItem { Tag = "mnuHelpAbout" };

		Xwt.VBox Layout = new Xwt.VBox();
		Xwt.HPaned PanelLayout = new Xwt.HPaned();

		FileListPanel p1;
		FileListPanel p2;

		List<ListView2.ColumnInfo> LVCols = new List<ListView2.ColumnInfo>();

		/// <summary>The current active panel</summary>
		FileListPanel ActivePanel;
		/// <summary>The current inactive panel</summary>
		FileListPanel PassivePanel;

		Xwt.HBox KeyBoardHelp = new Xwt.HBox();
		KeyboardHelpButton[] KeybHelpButtons = new KeyboardHelpButton[11];//одна лишняя, которая нумбер [0]

		public MainWindow(string[] argv)
		{
			this.Title = "File Commander";
			this.MainMenu = WindowMenu;
			this.PaddingLeft = PaddingRight = PaddingTop = 0;
			PaddingBottom = PaddingBottom/3;

			//build user interface
			MainMenu.Items.Add(mnuFile);
			MainMenu.Items.Add(mnuView);
			MainMenu.Items.Add(mnuNavigate);
			MainMenu.Items.Add(mnuTools);
			MainMenu.Items.Add(mnuHelp);

			mnuFile.SubMenu = new Xwt.Menu();
			mnuFile.SubMenu.Items.Add(mnuFileUserMenu);
			mnuFile.SubMenu.Items.Add(mnuFileView);
			mnuFile.SubMenu.Items.Add(mnuFileEdit);
			mnuFile.SubMenu.Items.Add(mnuFileCompare);
			mnuFile.SubMenu.Items.Add(mnuFileCopy);
			mnuFile.SubMenu.Items.Add(mnuFileMove);
			mnuFile.SubMenu.Items.Add(mnuFileNewDir);
			mnuFile.SubMenu.Items.Add(mnuFileRemove);
			mnuFile.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
			mnuFile.SubMenu.Items.Add(mnuFileAtributes);
			mnuFile.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
			mnuFile.SubMenu.Items.Add(mnuFileQuickSelect);
			mnuFile.SubMenu.Items.Add(mnuFileQuickUnselect);
			mnuFile.SubMenu.Items.Add(mnuFileSelectAll);
			mnuFile.SubMenu.Items.Add(mnuFileUnselect);
			mnuFile.SubMenu.Items.Add(mnuFileInvertSelection);
			mnuFile.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
			mnuFile.SubMenu.Items.Add(mnuFileExit);

			mnuView.SubMenu = new Xwt.Menu();
			mnuView.SubMenu.Items.Add(mnuViewShort);
			mnuView.SubMenu.Items.Add(mnuViewDetails);
			mnuView.SubMenu.Items.Add(mnuViewIcons);
			mnuView.SubMenu.Items.Add(mnuViewThumbs);
			mnuView.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
			mnuView.SubMenu.Items.Add(mnuViewQuickView);
			mnuView.SubMenu.Items.Add(mnuViewTree);
			mnuView.SubMenu.Items.Add(mnuViewPCPCconnect);
			mnuView.SubMenu.Items.Add(mnuViewPCNETPCconnect);
			mnuView.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
			mnuView.SubMenu.Items.Add(mnuViewByName);
			mnuView.SubMenu.Items.Add(mnuViewByType);
			mnuView.SubMenu.Items.Add(mnuViewByDate);
			mnuView.SubMenu.Items.Add(mnuViewBySize);
			mnuView.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
			mnuView.SubMenu.Items.Add(mnuViewNoFilter);
			mnuView.SubMenu.Items.Add(mnuViewWithFilter);
			mnuView.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
			mnuView.SubMenu.Items.Add(mnuViewKeybrdHelp); //these checkboxes don't work, because no code was written
			mnuView.SubMenu.Items.Add(mnuViewInfobar);
			mnuView.SubMenu.Items.Add(mnuViewDiskButtons);

			mnuNavigate.SubMenu = new Xwt.Menu();
			mnuNavigate.SubMenu.Items.Add(mnuNavigateTree);
			mnuNavigate.SubMenu.Items.Add(mnuNavigateHistory);
			mnuNavigate.SubMenu.Items.Add(mnuNavigateFind);
			mnuNavigate.SubMenu.Items.Add(mnuNavigateReload);

			mnuTools.SubMenu = new Xwt.Menu();
			mnuTools.SubMenu.Items.Add(mnuToolsOptions);
			mnuTools.SubMenu.Items.Add(mnuToolsPluginManager);
			mnuTools.SubMenu.Items.Add(mnuToolsEditUserMenu);
			mnuTools.SubMenu.Items.Add(mnuToolsKeychains);
			mnuTools.SubMenu.Items.Add(mnuToolsConfigEdit);
			mnuTools.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
			mnuTools.SubMenu.Items.Add(mnuToolsDiskLabel);
			mnuTools.SubMenu.Items.Add(mnuToolsFormat);
			mnuTools.SubMenu.Items.Add(mnuToolsSysInfo);

			mnuHelp.SubMenu = new Xwt.Menu();
			mnuHelp.SubMenu.Items.Add(mnuHelpHelpMe);
			mnuHelp.SubMenu.Items.Add(mnuHelpDebug);
			mnuHelp.SubMenu.Items.Add(mnuHelpAbout);

			TranslateMenu(MainMenu);

			this.CloseRequested += MainWindow_CloseRequested;
			PanelLayout.KeyReleased += PanelLayout_KeyReleased;
			mnuFileView.Clicked += (o, ea) => { PanelLayout_KeyReleased(o, new Xwt.KeyEventArgs(Xwt.Key.F3, Xwt.ModifierKeys.None, false, 0)); };
			mnuFileEdit.Clicked += (o, ea) => { PanelLayout_KeyReleased(o, new Xwt.KeyEventArgs(Xwt.Key.F4, Xwt.ModifierKeys.None, false, 0)); };
			mnuFileCopy.Clicked += (o, ea) => { PanelLayout_KeyReleased(o, new Xwt.KeyEventArgs(Xwt.Key.F5, Xwt.ModifierKeys.None, false, 0)); };
			mnuFileMove.Clicked += (o, ea) => { PanelLayout_KeyReleased(o, new Xwt.KeyEventArgs(Xwt.Key.F6, Xwt.ModifierKeys.None, false, 0)); };
			mnuFileNewDir.Clicked += (o, ea) => { PanelLayout_KeyReleased(o, new Xwt.KeyEventArgs(Xwt.Key.F7, Xwt.ModifierKeys.None, false, 0)); };
			mnuFileRemove.Clicked += (o, ea) => { PanelLayout_KeyReleased(o, new Xwt.KeyEventArgs(Xwt.Key.F8, Xwt.ModifierKeys.None, false, 0)); };
			mnuFileSelectAll.Clicked += (o, ea) => { ActivePanel.ListingView.Select(null); };
			mnuFileUnselect.Clicked += (o, ea) => { ActivePanel.ListingView.Unselect(); };
			mnuFileInvertSelection.Clicked += (o, ea) => { ActivePanel.ListingView.InvertSelection(); };
			mnuFileQuickSelect.Clicked += (o, ea) => { PanelLayout_KeyReleased(o, new Xwt.KeyEventArgs(Xwt.Key.NumPadAdd, Xwt.ModifierKeys.None, false, 0)); };
			mnuFileQuickUnselect.Clicked += (o, ea) => { PanelLayout_KeyReleased(o, new Xwt.KeyEventArgs(Xwt.Key.NumPadSubtract, Xwt.ModifierKeys.None, false, 0)); };
			mnuFileExit.Clicked += (o, ea) => { this.Close(); };
			mnuViewNoFilter.Clicked += (o, ea) => { ActivePanel.LoadDir(); };
			mnuViewWithFilter.Clicked += mnuViewWithFilter_Clicked;
			mnuNavigateReload.Clicked += mnuNavigateReload_Clicked;
			mnuToolsOptions.Clicked += mnuToolsOptions_Clicked;
			mnuHelpDebug.Clicked += ShowDebugInfo;
			mnuHelpAbout.Clicked += mnuHelpAbout_Clicked;

			Layout.PackStart(PanelLayout, true, Xwt.WidgetPlacement.Fill, Xwt.WidgetPlacement.Fill,0,0,0,0);
			Layout.PackStart(KeyBoardHelp, false,Xwt.WidgetPlacement.End,Xwt.WidgetPlacement.Fill,1,3,1,2);

			this.Content = Layout;

			//check settings
			if (fcmd.Properties.Settings.Default.UserTheme != null)
			{
				if (fcmd.Properties.Settings.Default.UserTheme != "")
				{
					if (File.Exists(fcmd.Properties.Settings.Default.UserTheme))
						stylist = new Stylist(fcmd.Properties.Settings.Default.UserTheme);
					else
					{
						Xwt.MessageDialog.ShowError(Localizator.GetString("ThemeNotFound"), fcmd.Properties.Settings.Default.UserTheme);
						Xwt.Application.Exit();
					}
				}
			}

			//load bookmarks
			string BookmarksStore = null;
			if (fcmd.Properties.Settings.Default.BookmarksFile != null && fcmd.Properties.Settings.Default.BookmarksFile.Length > 0)
			{
				BookmarksStore = File.ReadAllText(fcmd.Properties.Settings.Default.BookmarksFile, Encoding.UTF8);
			}

			//build panels
			PanelLayout.Panel1.Content = new FileListPanel(BookmarksStore, fcmd.Properties.Settings.Default.UserTheme, Properties.Settings.Default.InfoBarContent1, Properties.Settings.Default.InfoBarContent2); //Левая, правая где сторона? Улица, улица, ты, брат, пьяна!
			PanelLayout.Panel2.Content = new FileListPanel(BookmarksStore, fcmd.Properties.Settings.Default.UserTheme, Properties.Settings.Default.InfoBarContent1, Properties.Settings.Default.InfoBarContent2);

			p1 = PanelLayout.Panel1.Content as FileListPanel;
			p2 = PanelLayout.Panel2.Content as FileListPanel;
			var openFileHandler = new pluginner.TypedEvent<string> (Panel_OpenFile);
			p1.OpenFile += openFileHandler;
			p2.OpenFile += openFileHandler;

			/*LVCols.Add(new ListView2.ColumnInfo { Title = "", Tag = "Icon", Width = 16, Visible = true });
			LVCols.Add(new ListView2.ColumnInfo { Title = "URL", Tag = "Path", Width = 0, Visible = false });
			LVCols.Add(new ListView2.ColumnInfo { Title = Localizator.GetString("FName"), Tag = "FName", Width = 100, Visible = true });
			LVCols.Add(new ListView2.ColumnInfo { Title = Localizator.GetString("FSize"), Tag = "FSize", Width = 50, Visible = true });
			LVCols.Add(new ListView2.ColumnInfo { Title = Localizator.GetString("FDate"), Tag = "FDate", Width = 50, Visible = true });
			LVCols.Add(new ListView2.ColumnInfo { Title = "Directory item info", Tag = "DirItem", Width = 0, Visible = false });
			*/
			if (Properties.Settings.Default.AsyncLocalFileLoading) {
				p1.FS = new base_plugins.fs.asyncLocalFileSystem();
				p2.FS = new base_plugins.fs.asyncLocalFileSystem();
			} else {
				p1.FS = new base_plugins.fs.localFileSystem();
				p2.FS = new base_plugins.fs.localFileSystem();
			}

			p1.GotFocus += (o, ea) => SwitchPanel(p1);
			p2.GotFocus += (o, ea) => SwitchPanel(p2);

			//build keyboard help bar
			for (int i = 1; i < 11; i++)
			{
				KeybHelpButtons[i] = new KeyboardHelpButton {CanGetFocus = false};
				KeyBoardHelp.PackStart(KeybHelpButtons[i],true,Xwt.WidgetPlacement.Fill,Xwt.WidgetPlacement.Fill,0,-6,0,-3);
			}
			KeybHelpButtons[1].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F1, Xwt.ModifierKeys.None,false,0)); };
			KeybHelpButtons[2].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F2, Xwt.ModifierKeys.None, false, 0)); };
			KeybHelpButtons[3].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F3, Xwt.ModifierKeys.None, false, 0)); };
			KeybHelpButtons[4].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F4, Xwt.ModifierKeys.None, false, 0)); };
			KeybHelpButtons[5].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F5, Xwt.ModifierKeys.None, false, 0)); };
			KeybHelpButtons[6].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F6, Xwt.ModifierKeys.None, false, 0)); };
			KeybHelpButtons[7].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F7, Xwt.ModifierKeys.None, false, 0)); };
			KeybHelpButtons[8].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F8, Xwt.ModifierKeys.None, false, 0)); };
			KeybHelpButtons[9].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F9, Xwt.ModifierKeys.None, false, 0)); };
			KeybHelpButtons[10].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F10, Xwt.ModifierKeys.None, false, 0)); };
			//todo: replace this shit-code with huge using of KeybHelpButtons[n].Tag property (note that it's difficult to be realized due to c# restrictions)

			Localizator.LocalizationChanged += (o, ea) => Localize();
			Localize();

			//apply user's settings
			//window size
			this.Width = fcmd.Properties.Settings.Default.WinWidth;
			this.Height = fcmd.Properties.Settings.Default.WinHeight;
			//file size display policy
			char[] Policies = fcmd.Properties.Settings.Default.SizeShorteningPolicy.ToCharArray();

			//load last directory or the current directory if the last directory hasn't remembered
			if (Properties.Settings.Default.Panel1URL.Length != 0) {
				p1.LoadDir (Properties.Settings.Default.Panel1URL, ConvertSDP (Policies [0]), ConvertSDP (Policies [1]), ConvertSDP (Policies [2]));
			} else {
				p1.LoadDir ("file://" + System.IO.Directory.GetCurrentDirectory (), ConvertSDP (Policies [0]), ConvertSDP (Policies [1]), ConvertSDP (Policies [2]));
			}

			if (Properties.Settings.Default.Panel2URL.Length != 0) {
				p2.LoadDir (Properties.Settings.Default.Panel2URL, ConvertSDP (Policies [0]), ConvertSDP (Policies [1]), ConvertSDP (Policies [2]));
			} else {
				p2.LoadDir ("file://" + System.IO.Directory.GetCurrentDirectory (), ConvertSDP (Policies [0]), ConvertSDP (Policies [1]), ConvertSDP (Policies [2]));
			}

			//default panel
			switch (fcmd.Properties.Settings.Default.LastActivePanel)
			{
				case 1:
					p1.ListingView.SetFocus();
					ActivePanel = p1; PassivePanel = p2;
					if(argv.Length == 1) p1.LoadDir(argv[0]);
					break;
				case 2:
					p2.ListingView.SetFocus();
					ActivePanel = p2; PassivePanel = p1;
					if (argv.Length == 1) p2.LoadDir(argv[0]);
					break;
				default:
					p1.ListingView.SetFocus();
					ActivePanel = p1; PassivePanel = p2;
				if (argv.Length == 1) p1.LoadDir(argv[0]);
					break;
			}
#if DEBUG
			Console.WriteLine(@"DEBUG: MainWindow initialization has been completed.");
#endif
		}

		private void Localize()
		{
			TranslateMenu(this.MainMenu);

			for (int i = 1; i < 11; i++)
			{
				KeybHelpButtons[i].FKey = "F" + i;
				KeybHelpButtons[i].Text = Localizator.GetString("FCF" + i);
			}

			LVCols.Clear();
			LVCols.Add(new ListView2.ColumnInfo { Title = "", Tag = "Icon", Width = 16, Visible = true });
			LVCols.Add(new ListView2.ColumnInfo { Title = "URL", Tag = "Path", Width = 0, Visible = false });
			LVCols.Add(new ListView2.ColumnInfo { Title = Localizator.GetString("FName"), Tag = "FName", Width = 100, Visible = true });
			LVCols.Add(new ListView2.ColumnInfo { Title = Localizator.GetString("FSize"), Tag = "FSize", Width = 50, Visible = true });
			LVCols.Add(new ListView2.ColumnInfo { Title = Localizator.GetString("FDate"), Tag = "FDate", Width = 50, Visible = true });
			LVCols.Add(new ListView2.ColumnInfo { Title = "Directory item info", Tag = "DirItem", Width = 0, Visible = false });
			p1.ListingView.SetColumns(LVCols);
			p2.ListingView.SetColumns(LVCols);
		}

		private void mnuViewWithFilter_Clicked(object sender, EventArgs e)
		{
			string Filter = @"*.*";

			InputBox ibx = new InputBox(Localizator.GetString("NameFilterQuestion"), Filter);
			Xwt.CheckBox chkRegExp = new Xwt.CheckBox(Localizator.GetString("NameFilterUseRegExp"));
			ibx.OtherWidgets.Add(chkRegExp, 0, 0);
			if (!ibx.ShowDialog()) return;
			Filter = ibx.Result;
			if (chkRegExp.State == Xwt.CheckBoxState.Off)
			{
				Filter = Filter.Replace(".", @"\.");
				Filter = Filter.Replace("*", ".*");
				Filter = Filter.Replace("?", ".");
			}
			try
			{
				System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(Filter);

				List<pluginner.DirItem> GoodItems = new List<pluginner.DirItem>();
				foreach (pluginner.DirItem di in ActivePanel.FS.DirectoryContent)
				{
					if (re.IsMatch(di.TextToShow))
						GoodItems.Add(di);
				}

				/*ActivePanel.LoadDir(
					ActivePanel.FS.CurrentDirectory,
					GoodItems,
					ActivePanel.CurShortenKB,
					ActivePanel.CurShortenMB,
					ActivePanel.CurShortenGB
					);*/
				ActivePanel.LoadDir(
					ActivePanel.FS.CurrentDirectory,
					ActivePanel.CurShortenKB,
					ActivePanel.CurShortenMB,
					ActivePanel.CurShortenGB
					);	//undone!

				ActivePanel.StatusBar.Text = string.Format(Localizator.GetString("NameFilterFound"), Filter, GoodItems.Count);
			}
			catch (Exception ex)
			{
				Xwt.MessageDialog.ShowError(Localizator.GetString("NameFilterError"), ex.Message);
			}
		}

		private void mnuNavigateReload_Clicked(object sender, EventArgs e)
		{
			ActivePanel.LoadDir();
		}

		private void Panel_OpenFile(string data)
		{
			if (data.StartsWith("file://") && System.IO.File.Exists(data.Replace("file://","")))
			{
				try
				{
					System.Diagnostics.Process proc = new System.Diagnostics.Process();
					proc.StartInfo.FileName = data.Replace("file://", "");
					proc.StartInfo.UseShellExecute = true;
					proc.Start();
				}
				catch (Exception ex)
				{
					Xwt.MessageDialog.ShowMessage(ex.Message);
				}
			}//todo: else {download to HDD and open locally, if modified, upload back after closing the editor}
		}

		private void ShowDebugInfo (object sender, EventArgs e)
		{
			System.Configuration.Configuration confLR = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal);
			System.Configuration.Configuration confR = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoaming);
			System.Configuration.Configuration confEXE = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.None);

			Xwt.Dialog Fcdbg = new Xwt.Dialog();
			Fcdbg.Buttons.Add(Xwt.Command.Close);
			Fcdbg.Buttons[0].Clicked += (o, ea) => {Fcdbg.Hide();};
			Fcdbg.Title="FC debug output";
			string txt = "" +
				"===THE FILE COMMANDER, VERSION " + ProductVersion + (Environment.Is64BitProcess ? " 64-BIT" : " 32-BIT") + "===\n" +
				Environment.CommandLine + " @ .NET fw " + Environment.Version + (Environment.Is64BitOperatingSystem ? " 64-bit" : " 32-bit") + " on " + Environment.MachineName + "-" + Environment.OSVersion + " (" + OSVersionEx.Platform + " v" + Environment.OSVersion.Version.Major + "." + Environment.OSVersion.Version.Minor + ")\n" +
				"The current drawing toolkit is " + Xwt.Toolkit.CurrentEngine.GetSafeBackend(this) + "\n" +
				"\nCONFIGuration files:\n---------\n" +
				"Local: " + confLR.FilePath + " (exists? " + b2s(confLR.HasFile) + ")\n" +
				"Roaming: " + confR.FilePath + " (exists? " + b2s(confR.HasFile) + ")\n" +
				"Overall: " + confEXE.FilePath + " (exists? " + b2s(confEXE.HasFile) + ")\n" +
				"\nPanel debug:\n---------\n" +
				"The active panel is: " + ((ActivePanel == p1) ? "LEFT\n" : "RIGHT\n") +
				"The passive panel is: " + ((ActivePanel == p2) ? "LEFT\n" : "RIGHT\n") +
				"They are different? " + b2s(ActivePanel != PassivePanel) + " (should be yes)\n" +
				"The LEFT filesystem: " + p1.FS + " at \"" + p1.FS.CurrentDirectory + "\"\n" +
				"The RIGHT filesystem: " + p2.FS + " at \"" + p2.FS.CurrentDirectory + "\"\n" +
				"Filesystems are same by type? " + b2s(p1.FS.GetType() == p2.FS.GetType()) + ".\n" +
				"Filesystems are identically? " + b2s(p1.FS == p2.FS) + " (should be no).\n" +
				"\nTheme debug:\n---------\n" +
				"Using external theme? " + b2s(!string.IsNullOrEmpty(fcmd.Properties.Settings.Default.UserTheme)) + "\n" +
				"Theme's cascade style sheet file: \"" + fcmd.Properties.Settings.Default.UserTheme + "\"\n\nIf you having some troubles, please report this to https://github.com/atauenis/fcmd bug tracker or http://atauenis.ru/phpBB3/viewtopic.php?f=4&t=211 topic. \nThe End.";
			Xwt.RichTextView rtv = new Xwt.RichTextView();
			rtv.LoadText(txt, new Xwt.Formats.PlainTextFormat());
			Xwt.ScrollView sv = new Xwt.ScrollView(rtv);
			Fcdbg.Content = sv;
			Fcdbg.Width = 500;
			Fcdbg.Run();
		}

		// for ShowDebugInfo bool-value displaing purposes
		private string b2s(bool b)
		{
			return (b == true) ? "YES" : "NO";
		}

		private void mnuToolsOptions_Clicked(object sender, EventArgs e)
		{
			new SettingsWindow().Run();
			ActivePanel.LoadDir();
			PassivePanel.LoadDir();
		}

		private void mnuHelpAbout_Clicked(object sender, EventArgs e)
		{
			System.Configuration.Configuration conf = System.Configuration.ConfigurationManager.OpenExeConfiguration(System.Configuration.ConfigurationUserLevel.PerUserRoamingAndLocal);
			string AboutString = string.Format(
				Localizator.GetString("FileCommanderVer"),
				"File Commander",
				ProductVersion,
				"\nhttps://github.com/atauenis/fcmd",
				conf.FilePath,
				Environment.OSVersion,
				Environment.Version + (Environment.Is64BitProcess ? " x86-64" : " x86")
				);
			Xwt.MessageDialog.ShowMessage(AboutString);
		}

		private void MainWindow_CloseRequested(object sender, Xwt.CloseRequestedEventArgs args)
		{
			//save settings bcos zi form is closing
			Properties.Settings.Default.WinHeight = this.Height;
			Properties.Settings.Default.WinWidth = this.Width;
			Properties.Settings.Default.Panel1URL = p1.FS.CurrentDirectory;
			Properties.Settings.Default.Panel2URL = p2.FS.CurrentDirectory;
			Properties.Settings.Default.LastActivePanel = (ActivePanel == p1) ? (byte)1 : (byte)2;
			Properties.Settings.Default.Save();
			Xwt.Application.Exit();
		}

		/// <summary>The entry form's keyboard keypress handler (except commandbar keypresses)</summary>
		private void PanelLayout_KeyReleased(object sender, Xwt.KeyEventArgs e)
		{
#if DEBUG
			FileListPanel p1 = (PanelLayout.Panel1.Content as FileListPanel);
			FileListPanel p2 = (PanelLayout.Panel2.Content as FileListPanel);
			Console.WriteLine("KEYBOARD DEBUG: " + e.Modifiers + "+" + e.Key + " was pressed. Panels focuses: " + (ActivePanel == p1) + " | " + (ActivePanel == p2));
#endif
			if (e.Key == Xwt.Key.Return) return;//ENTER presses are handled by other event

			string URL1;
			if (ActivePanel.ListingView.SelectedRow > -1)
				{ URL1 = ActivePanel.GetValue(ActivePanel.dfURL); }
			else
				{ URL1 = null; }
			pluginner.IFSPlugin FS1 = ActivePanel.FS;

			string URL2;
			if (PassivePanel.ListingView.SelectedRow > -1)
				{ URL2 = PassivePanel.GetValue(PassivePanel.dfURL); }
			else
				{ URL2 = null; }
			pluginner.IFSPlugin FS2 = PassivePanel.FS;

			switch (e.Key)
			{
				case Xwt.Key.NumPadAdd: //[+] gray - add selection
					string Filter = @"*.*";

					InputBox ibx_qs = new InputBox(Localizator.GetString("QuickSelect"), Filter);
					Xwt.CheckBox chkRegExp = new Xwt.CheckBox(Localizator.GetString("NameFilterUseRegExp"));
					ibx_qs.OtherWidgets.Add(chkRegExp, 0, 0);
					if (!ibx_qs.ShowDialog()) return;
					Filter = ibx_qs.Result;
					if (chkRegExp.State == Xwt.CheckBoxState.Off)
					{
						Filter = Filter.Replace(".", @"\.");
						Filter = Filter.Replace("*", ".*");
						Filter = Filter.Replace("?", ".");
					}
					try
					{
						System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(Filter);

						int Count = 0;
						foreach (ListView2Item lvi in ActivePanel.ListingView.Items)
						{
							if (re.IsMatch(lvi.Data[1].ToString())){
								ActivePanel.ListingView.Select(lvi);
								Count++;
							}
						}

						ActivePanel.StatusBar.Text = string.Format(Localizator.GetString("NameFilterFound"), Filter, Count);
					}
					catch (Exception ex)
					{
						Xwt.MessageDialog.ShowError(Localizator.GetString("NameFilterError"), ex.Message);
					}
					return;

				case Xwt.Key.NumPadSubtract: //[-] gray - add selection
					string Filter_qus = @"*.*";

					InputBox ibx_qus = new InputBox(Localizator.GetString("QuickUnselect"), Filter_qus);
					Xwt.CheckBox chkRegExp_qus = new Xwt.CheckBox(Localizator.GetString("NameFilterUseRegExp"));
					ibx_qus.OtherWidgets.Add(chkRegExp_qus, 0, 0);
					if (!ibx_qus.ShowDialog()) return;
					Filter_qus = ibx_qus.Result;
					if (chkRegExp_qus.State == Xwt.CheckBoxState.Off)
					{
						Filter_qus = Filter_qus.Replace(".", @"\.");
						Filter_qus = Filter_qus.Replace("*", ".*");
						Filter_qus = Filter_qus.Replace("?", ".");
					}
					try
					{
						System.Text.RegularExpressions.Regex re = new System.Text.RegularExpressions.Regex(Filter_qus);

						int Count_qus = 0;
						foreach (ListView2Item lvi in ActivePanel.ListingView.Items)
						{
							if (re.IsMatch(lvi.Data[1].ToString()))
							{
								ActivePanel.ListingView.Unselect(lvi);
								Count_qus++;
							}
						}
					}
					catch (Exception ex)
					{
						Xwt.MessageDialog.ShowError(Localizator.GetString("NameFilterError"), ex.Message);
					}
					return;

				//F KEYS
				case Xwt.Key.F3: //F3: View. Shift+F3: View as text.
					if (URL1 == null)
						return;

					if (!FS1.FileExists(URL1))
					{
						Xwt.MessageDialog.ShowWarning(string.Format(Localizator.GetString("FileNotFound"), ActivePanel.GetValue(ActivePanel.dfDisplayName)));
						return;
					}

					VEd V = new VEd();
					if (e.Modifiers == Xwt.ModifierKeys.None)
					{ V.LoadFile(URL1, FS1, false); V.Show(); }
					else if(e.Modifiers == Xwt.ModifierKeys.Shift)
					{ V.LoadFile(URL1, FS1, new base_plugins.ve.PlainText(), false); V.Show(); }
					//todo: handle Ctrl+F3 (Sort by name).
					return;
				case Xwt.Key.F4: //F4: Edit. Shift+F4: Edit as txt.
					if (URL1 == null)
						return;

					if (!FS1.FileExists(URL1))
					{
						Xwt.MessageDialog.ShowWarning(string.Format(Localizator.GetString("FileNotFound"), ActivePanel.GetValue(ActivePanel.dfDisplayName)));
						return;
					}

					VEd E = new VEd();
					if (e.Modifiers == Xwt.ModifierKeys.None)
					{ E.LoadFile(URL1, FS1, true); E.Show(); }
					else if(e.Modifiers == Xwt.ModifierKeys.Shift)
					{ E.LoadFile(URL1, FS1, new base_plugins.ve.PlainText(), true); E.Show(); }
					//todo: handle Ctrl+F4 (Sort by extension).
					return;
				case Xwt.Key.F5: //F5: Copy.
					if (URL1 == null)
						return;
					Cp();
					//todo: handle Ctrl+F5 (Sort by timestamp).
					return;
				case Xwt.Key.F6: //F6: Move/Rename.
					if (URL1 == null)
						return;
					Mv();
					//todo: handle Ctrl+F6 (Sort by size).
					return;
				case Xwt.Key.F7: //F7: New directory.
					InputBox ibx = new InputBox(Localizator.GetString("NewDirURL"), ActivePanel.FS.CurrentDirectory + Localizator.GetString("NewDirTemplate"));
					if (ibx.ShowDialog()) MkDir(ibx.Result);
					return;
				case Xwt.Key.F8: //F8: delete
					if (URL1 == null)
						return;
					Rm();
					//todo: move to trash can/recycle bin & handle Shit+F8 (remove completely)
					return;
				case Xwt.Key.F10: //F10: Exit
					//todo: ask user, are it really want to close FC?
					Xwt.Application.Exit();
					//todo: handle Alt+F10 (directory tree)
					return;
			}
#if DEBUG
			Console.WriteLine("KEYBOARD DEBUG: the key wasn't handled");
#endif
			e.Handled = true;
		}

		/// <summary>Switches the active panel</summary>
		/// <param name="NewPanel">The new active panel</param>
		private void SwitchPanel(FileListPanel NewPanel)
		{
			if (NewPanel == ActivePanel) return;
			PassivePanel = ActivePanel;
			ActivePanel = NewPanel;
#if DEBUG
			string PanelName = (NewPanel == p1) ? "LEFT" : "RIGHT";
			Console.WriteLine("FOCUS DEBUG: The " + PanelName + " panel (" + NewPanel.FS.CurrentDirectory + ") got focus");
#endif
			AssemblyName an = Assembly.GetExecutingAssembly().GetName();
			this.Title = string.Format(
				"{0} - {1}",
				"FC",//System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, //todo: add the ProductName w/o WinForms usage
				ActivePanel.FS.CurrentDirectory
			);

			PassivePanel.UrlBox.BackgroundColor = Xwt.Drawing.Colors.LightBlue;
			ActivePanel.UrlBox.BackgroundColor = Xwt.Drawing.Colors.DodgerBlue;
		}

		/// <summary>Converts size display policy (as string) to FLP.SizeDisplayPolicy</summary>
		private FileListPanel.SizeDisplayPolicy ConvertSDP(char SizeDisplayPolicy)
		{
			switch (SizeDisplayPolicy.ToString())
			{
				case "0":
					return FileListPanel.SizeDisplayPolicy.DontShorten;
				case "1":
					return FileListPanel.SizeDisplayPolicy.OneNumeral;
				case "2":
					return FileListPanel.SizeDisplayPolicy.TwoNumeral;
				default:
					return FileListPanel.SizeDisplayPolicy.OneNumeral;
			}
		}

		/// <summary>Translates the <paramref name="mnu"/> into the current UI language</summary>
		private void TranslateMenu(Xwt.Menu mnu)
		{
			try
			{ //dirty hack...i don't know why, but "if(mnu.Items == null) return;" raises NullReferenceException...
				foreach (Xwt.MenuItem currentMenuItem in mnu.Items)
				{
					if (currentMenuItem.GetType() != typeof(Xwt.SeparatorMenuItem))
					{ //skip separators
						currentMenuItem.Label = Localizator.GetString("FC" + currentMenuItem.Tag);
						TranslateMenu(currentMenuItem.SubMenu);
					}
				}
			}
			catch { }
		}
	}
}
