using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fcmd.SettingsWindowTabs
{
    class swtMainWindowInfobar : ISettingsWindowTab
    {
        Localizator Locale = new Localizator();
        Xwt.Frame fraInfobar = new Xwt.Frame();
        Xwt.VBox Layout = new Xwt.VBox();
        Xwt.TextEntry txtText1 = new Xwt.TextEntry();
        Xwt.TextEntry txtText2 = new Xwt.TextEntry();

        public swtMainWindowInfobar()
        {
            fraInfobar.Label = Locale.GetString("SWTMWinfobar").Replace("    ", "");
            fraInfobar.Content = Layout;
            Layout.PackStart(new Xwt.Label(
                Locale.GetString("SWTMWItext1")
                ));
            Layout.PackStart(txtText1);
            Layout.PackStart(new Xwt.Label(
                Locale.GetString("SWTMWItext2")
                ));
            Layout.PackStart(txtText2);
            Layout.PackStart(new Xwt.Label(
                Locale.GetString("SWTMWIhelp")
                ){ Wrap = Xwt.WrapMode.Word }
                );

            txtText1.Text = fcmd.Properties.Settings.Default.InfoBarContent1;
            txtText2.Text = fcmd.Properties.Settings.Default.InfoBarContent2;
        }

        public Xwt.Widget Content
        {
            get { return fraInfobar; }
        }

        public bool SaveSettings()
        {
            fcmd.Properties.Settings.Default.InfoBarContent1 = txtText1.Text;
            fcmd.Properties.Settings.Default.InfoBarContent2 = txtText2.Text;
            return true;
        }
    }
}
