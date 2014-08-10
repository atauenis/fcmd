/* The File Commander Settings window tabs
 * Tab "Main window layout"
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Zhigunov Andrew (breakneck11@gmail.com)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using fcmd.Properties;
using Xwt;

namespace fcmd.SettingsWindowTabs
{
	/// <summary>
	/// "Main window" tab
	/// </summary>
	class swtMainWindow : ISettingsWindowTab
	{
		VBox box = new VBox();
		
		Frame fraMain = new Frame();
		VBox fraMainBox = new VBox();
		CheckBox chkShowToolBar = new CheckBox {Sensitive = false};
		CheckBox chkDiskButtons = new CheckBox(); //ok
		CheckBox chkDiskListBox = new CheckBox { Sensitive = false };
		CheckBox chkPanelTitle = new CheckBox(); //ok
		CheckBox chkTableColumns = new CheckBox();//ok
		CheckBox chkInfoBar = new CheckBox();//ok
		CheckBox chkCmdLine = new CheckBox { Sensitive = false };
		CheckBox chkKeybHelp = new CheckBox();//ok
		Label lblBookmarks = new Label();
		TextEntry txtBookmarks = new TextEntry();
		Label lblLanguage = new Label();
		ComboBox cbxLanguage = new ComboBox();

		public swtMainWindow()
		{
			Localizator.LocalizationChanged += Localizator_LocalizationChanged;
			box.PackStart(fraMain);
			fraMain.Content = fraMainBox;
			Localizator_LocalizationChanged(null,null);

			chkDiskButtons.State = CBSfromBool(Settings.Default.ShowDiskList);
			chkPanelTitle.State = CBSfromBool(Settings.Default.ShowPanelUrlbox);
			chkTableColumns.State = CBSfromBool(Settings.Default.ShowPanelTableCaptions);
			chkInfoBar.State = CBSfromBool(Settings.Default.ShowFileInfo);
			chkKeybHelp.State = CBSfromBool(Settings.Default.ShowKeybrdHelp);
			txtBookmarks.Text = Settings.Default.BookmarksFile ?? "";

			cbxLanguage.Items.Add("(internal)ru_RU", @"Русский");
			cbxLanguage.Items.Add("(internal)en_US", @"English");
			//Here may be some code loading external languages
			cbxLanguage.SelectedItem = Settings.Default.Language;

			fraMainBox.PackStart(chkShowToolBar);
			fraMainBox.PackStart(chkDiskButtons);
			fraMainBox.PackStart(chkDiskListBox);
			fraMainBox.PackStart(chkPanelTitle);
			fraMainBox.PackStart(chkTableColumns);
			fraMainBox.PackStart(chkInfoBar);
			fraMainBox.PackStart(chkCmdLine);
			fraMainBox.PackStart(chkKeybHelp);
			fraMainBox.PackStart(lblBookmarks);
			fraMainBox.PackStart(txtBookmarks);
			fraMainBox.PackStart(lblLanguage);
			fraMainBox.PackStart(cbxLanguage);
		}

		void Localizator_LocalizationChanged(object sender, EventArgs e)
		{
			fraMain.Label = Localizator.GetString("swtMainWindow");

			chkShowToolBar.Label = Localizator.GetString("SWTMWtoolbar");
			chkDiskButtons.Label = Localizator.GetString("SWTMWdiskbuttons");
			chkDiskListBox.Label = Localizator.GetString("SWTMWdisklistbox");
			chkPanelTitle.Label = Localizator.GetString("SWTMWpaneltitle");
			chkTableColumns.Label = Localizator.GetString("SWTMWtablecolumns");
			chkInfoBar.Label = Localizator.GetString("SWTMWinfobar");
			chkCmdLine.Label = Localizator.GetString("SWTMWcmdline");
			chkKeybHelp.Label = Localizator.GetString("SWTMWkeybhelp");
			lblBookmarks.Text = Localizator.GetString("SWTMWbookmars");
			lblLanguage.Text = Localizator.GetString("SWTMWlanguage");
		}

		public bool SaveSettings() {
			try
			{
				Settings.Default.ShowDiskList = BoolFromCBX(chkDiskButtons);
				Settings.Default.ShowPanelUrlbox = BoolFromCBX(chkPanelTitle);
				Settings.Default.ShowPanelTableCaptions = BoolFromCBX(chkTableColumns);
				Settings.Default.ShowFileInfo = BoolFromCBX(chkInfoBar);
				Settings.Default.ShowKeybrdHelp = BoolFromCBX(chkKeybHelp);
				Settings.Default.BookmarksFile = txtBookmarks.Text;
				string old_language = Settings.Default.Language, new_language = (string)(cbxLanguage.SelectedItem);
				Settings.Default.Language = new_language;
				if (old_language != new_language)
				{
					Localizator.LoadLanguage(new_language, false);
				}
				return true;
			}
			catch(Exception ex) { MessageDialog.ShowError(ex.Message) ;  return false; }
		}

		public Widget Content
		{
			get { return box; }
		}

		/// <summary>Converts boolean values into Xwt.CheckBoxState</summary>
		private CheckBoxState CBSfromBool (bool Bulevo)
		{
			switch (Bulevo){
				case true: return CheckBoxState.On;
				case false: return CheckBoxState.Off;
			}
			return CheckBoxState.Mixed; //fallback
		}

		/// <summary>Converts Xwt.CheckBox selection status into boolean value</summary>
		private bool BoolFromCBX(CheckBox CBX)
		{
			switch (CBX.State){
				case CheckBoxState.On: return true;
				case CheckBoxState.Off: return false;
			}
			return false; //fallback
		}
	}
}
