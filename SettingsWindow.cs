/* The File Commander Settings window
 * FC settings window
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using fcmd.SettingsWindowTabs;
using Xwt;

namespace fcmd
{
	/// <summary>The settings window (window, where user switches the program's settings)</summary>
	class SettingsWindow : Dialog
	{
		HPaned Layout = new HPaned();
		ListBox TabList = new ListBox();

		public SettingsWindow()
		{
			Build();
		}

		private void Build()
		{
			Title = Localizator.GetString("FCS_Title");

			Layout.Panel1.Content = TabList;
			Content = Layout;
			ShowInTaskbar = false;

			Buttons.Clear();
			Buttons.Add(Command.Save, Command.Cancel);
			Buttons[0].Clicked += cmdOk_Clicked;
			Buttons[1].Clicked += (o, e) => Hide();

			TabList.Items.Clear();
			TabList.SelectionChanged -= TabList_SelectionChanged;
			TabList.MinHeight = 388; TabList.MinWidth = 128;
			TabList.Items.Add(new swtMainWindow(),Localizator.GetString("swtMainWindow"));
			TabList.Items.Add(new swtMainWindowColumns(), Localizator.GetString("swtMainWindowColumns"));
			TabList.Items.Add(new swtMainWindowInfobar(), Localizator.GetString("SWTMWinfobar"));
			TabList.Items.Add(new swtMainWindowThemes(), Localizator.GetString("swtMainWindowThemes"));
			TabList.Items.Add(new swtViewerEditor(), Localizator.GetString("swtViewerEditor"));
			TabList.Items.Add(new swtMainWindowFonts() , Localizator.GetString("swtFonts"));

			TabList.SelectionChanged += TabList_SelectionChanged;
			TabList.SelectRow(0); //wpf hack (row №0 isn't automatical selected)
		}

		void cmdOk_Clicked(object sender, EventArgs e)
		{
			foreach (ISettingsWindowTab swt in TabList.Items)
			{
				if (swt.SaveSettings()) continue;

				//if someone is unable to save settings...
				MessageDialog.ShowError(Localizator.GetString("FCS_CantSaveSettings"));
				return;
			}
			this.Hide();
		}

		void TabList_SelectionChanged(object sender, EventArgs e)
		{
			if(TabList.SelectedRow > -1)
				Layout.Panel2.Content = ((ISettingsWindowTab) TabList.SelectedItem).Content;
		}
	}
}
