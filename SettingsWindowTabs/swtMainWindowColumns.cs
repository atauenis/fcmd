/* The File Commander Settings window tabs
 * Tab "Collumns in panels"
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fcmd.SettingsWindowTabs
{
    class swtMainWindowColumns : ISettingsWindowTab
    {
        Xwt.VBox box = new Xwt.VBox();
        Localizator Locale = new Localizator();

        Xwt.Frame fraExtensions = new Xwt.Frame() {Sensitive = false};
        Xwt.VBox fraExtensionsBox = new Xwt.VBox();
        Xwt.RadioButtonGroup rbgExtensionDisplayStyle = new Xwt.RadioButtonGroup();
        Xwt.RadioButton optDisplayExtTogether = new Xwt.RadioButton();
        Xwt.RadioButton optDisplayExtFar = new Xwt.RadioButton();

        Xwt.Frame fraTabs = new Xwt.Frame(){Sensitive = false};
        Xwt.Table fraTabsBox = new Xwt.Table();
        Xwt.TextEntry txtTabExtension = new Xwt.TextEntry();
        Xwt.TextEntry txtTabSize = new Xwt.TextEntry();
        Xwt.TextEntry txtTabDate = new Xwt.TextEntry();
        Xwt.TextEntry txtTabFilemode = new Xwt.TextEntry();

        Xwt.Frame fraOther = new Xwt.Frame();
        Xwt.Table fraOtherBox = new Xwt.Table();
        Xwt.CheckBox chkExpandName = new Xwt.CheckBox() {Sensitive = false};
        Xwt.CheckBox chkShowCentury = new Xwt.CheckBox() {Sensitive = false};
        Xwt.CheckBox chkShowTimeAs12h = new Xwt.CheckBox() {Sensitive = false};
        Xwt.CheckBox chkShowDirsInStatus = new Xwt.CheckBox() {Sensitive = false};
        Xwt.ComboBox cmbPanelSizeDisplay = new Xwt.ComboBox();
        Xwt.TextEntry txtMaxHumanySizeInStatus = new Xwt.TextEntry() {Sensitive = false};

        public swtMainWindowColumns()
        {
            box.PackStart(fraExtensions);
            box.PackStart(fraTabs);
            box.PackStart(fraOther);
            fraExtensions.Content = fraExtensionsBox;
            fraExtensions.Label = Locale.GetString("SWTMWCFileExtView");
            fraTabs.Content = fraTabsBox;
            fraTabs.Label = Locale.GetString("SWTMWCCollumns");
            fraOther.Content = fraOtherBox;

            optDisplayExtTogether.Group = rbgExtensionDisplayStyle;
            optDisplayExtFar.Group = rbgExtensionDisplayStyle;
            optDisplayExtTogether.Label = Locale.GetString("SWTMWCExtTogether");
            optDisplayExtFar.Label = Locale.GetString("SWTMWCExtFar");
            fraExtensionsBox.PackStart(optDisplayExtTogether);
            fraExtensionsBox.PackStart(optDisplayExtFar);

            fraTabsBox.Add(new Xwt.Label(Locale.GetString("SWTMWCExt")), 0, 0);
            fraTabsBox.Add(txtTabExtension, 1, 0);

            fraTabsBox.Add(new Xwt.Label(Locale.GetString("SWTMWCSize")), 0, 1);
            fraTabsBox.Add(txtTabSize, 1, 1);

            fraTabsBox.Add(new Xwt.Label(Locale.GetString("SWTMWCDate")), 0, 2);
            fraTabsBox.Add(txtTabDate, 1, 2);

            fraTabsBox.Add(new Xwt.Label(Locale.GetString("SWTMWCFileMode")), 0, 3);
            fraTabsBox.Add(txtTabFilemode, 1, 3);


            chkExpandName.Label = Locale.GetString("SWTMWCExpandName");
            chkShowCentury.Label = Locale.GetString("SWTMWCShowCentury");
            chkShowTimeAs12h.Label = Locale.GetString("SWTMWCShowTimeAs12h");
            chkShowDirsInStatus.Label = Locale.GetString("SWTMWCShowDirsInStatus");

            fraOtherBox.Add(chkExpandName, 0, 0);
            fraOtherBox.Add(chkShowCentury, 0, 1);
            fraOtherBox.Add(chkShowTimeAs12h, 0, 2);
            fraOtherBox.Add(chkShowDirsInStatus, 0, 3);

            cmbPanelSizeDisplay.Items.Add("000", Locale.GetString("SizeDisplayPolicy000"));
            cmbPanelSizeDisplay.Items.Add("100", Locale.GetString("SizeDisplayPolicy100"));
            cmbPanelSizeDisplay.Items.Add("200", Locale.GetString("SizeDisplayPolicy200"));
            cmbPanelSizeDisplay.Items.Add("111", Locale.GetString("SizeDisplayPolicy111"));
            cmbPanelSizeDisplay.Items.Add("222", Locale.GetString("SizeDisplayPolicy222"));
            cmbPanelSizeDisplay.Items.Add("110", Locale.GetString("SizeDisplayPolicy110"));
            cmbPanelSizeDisplay.Items.Add("220", Locale.GetString("SizeDisplayPolicy220"));

            /*
            SizeDisplayPolicy000=bytes
            SizeDisplayPolicy100=kbytes
            SizeDisplayPolicy200=kbytes (x.xx KB)
            SizeDisplayPolicy111=dynamic (x.x К/М/Г)
            SizeDisplayPolicy222=dynamic (x.xx К/М/Г)
            SizeDisplayPolicy110=dynamic (x.x К/М)
            SizeDisplayPolicy220=dynamic (x.xx K/M)
            */

            fraOtherBox.Add(new Xwt.Label(Locale.GetString("SWTMWCSizeDisplay")), 0, 4);
            fraOtherBox.Add(cmbPanelSizeDisplay, 1, 4);
            fraOtherBox.Add(new Xwt.Label(Locale.GetString("SWTMWCMaxHumanSizeStatus")), 0, 5);
            fraOtherBox.Add(txtMaxHumanySizeInStatus, 1, 5);


            //load settings

            switch (fcmd.Properties.Settings.Default.SizeShorteningPolicy){
                case "000":
                    cmbPanelSizeDisplay.SelectedIndex = 0;
                    break;
                case "100":
                    cmbPanelSizeDisplay.SelectedIndex = 1;
                    break;
                case "200":
                    cmbPanelSizeDisplay.SelectedIndex = 2;
                    break;
                case "111":
                    cmbPanelSizeDisplay.SelectedIndex = 3;
                    break;
                case "222":
                    cmbPanelSizeDisplay.SelectedIndex = 4;
                    break;
                case "110":
                    cmbPanelSizeDisplay.SelectedIndex = 5;
                    break;
                case "220":
                    cmbPanelSizeDisplay.SelectedIndex = 6;
                    break;
            }
        }

        public Xwt.Widget Content
        {
            get { return box; }
        }

        public bool SaveSettings()
        {
            fcmd.Properties.Settings.Default.SizeShorteningPolicy = cmbPanelSizeDisplay.SelectedItem.ToString();
            return true;//undone
        }
    }
}
