/* The File Commander Settings window
 * FC settings window
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fcmd
{
    /// <summary>The settings window (window, where user switches the program's settings)</summary>
    class SettingsWindow : Xwt.Dialog
    {
        Localizator Locale = new Localizator();
        Xwt.HPaned Layout = new Xwt.HPaned();
        Xwt.ListBox TabList = new Xwt.ListBox();

        public SettingsWindow()
        {
            this.Title = Locale.GetString("FCS_Title");

            Layout.Panel1.Content = TabList;
            this.Content = Layout;

            this.Buttons.Add(Xwt.Command.Save, Xwt.Command.Cancel);
            this.Buttons[0].Clicked += new EventHandler(cmdOk_Clicked);
            this.Buttons[1].Clicked += (o, e) => { this.Hide(); };

            TabList.MinHeight = 388; TabList.MinWidth = 128;
            TabList.Items.Add(new SettingsWindowTabs.swtMainWindow(),Locale.GetString("swtMainWindow"));
            TabList.SelectionChanged += new EventHandler(TabList_SelectionChanged);
            TabList.SelectRow(0); //wpf hack (row №0 isn't automatical selected)
        }

        void cmdOk_Clicked(object sender, EventArgs e)
        {
            foreach (SettingsWindowTabs.ISettingsWindowTab swt in TabList.Items)
            {
                if (!swt.SaveSettings())
                {
                    //if someone is unable to save settings...
                    Xwt.MessageDialog.ShowError(Locale.GetString("FCS_CantSaveSettings"));
                    return;
                }
            }
            this.Hide();
        }

        void TabList_SelectionChanged(object sender, EventArgs e)
        {
            Layout.Panel2.Content = (TabList.SelectedItem as SettingsWindowTabs.ISettingsWindowTab).Content;
        }
    }
}
