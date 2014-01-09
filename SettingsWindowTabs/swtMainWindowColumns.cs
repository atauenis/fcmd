/* The File Commander Settings window tabs
 * Tab "Collumns in panels"
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
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

        Xwt.Frame fraTabs = new Xwt.Frame();
        Xwt.Table fraTabsBox = new Xwt.Table();
        Xwt.TextEntry txtTabExtension = new Xwt.TextEntry();
        Xwt.TextEntry txtTabSize = new Xwt.TextEntry();
        Xwt.TextEntry txtTabDate = new Xwt.TextEntry();
        Xwt.TextEntry txtTabFilemode = new Xwt.TextEntry();

        Xwt.Frame fraOther = new Xwt.Frame() {Sensitive = false};
        Xwt.Table fraOtherBox = new Xwt.Table();
        Xwt.CheckBox chkExpandName = new Xwt.CheckBox();
        Xwt.CheckBox chkShowCentury = new Xwt.CheckBox();
        Xwt.CheckBox chkShowTimeAs12h = new Xwt.CheckBox();
        Xwt.CheckBox chkShowDirsInStatus = new Xwt.CheckBox();
        Xwt.TextEntry txtMaxHumanySizeInPanels = new Xwt.TextEntry();
        Xwt.TextEntry txtMaxHumanySizeInStatus = new Xwt.TextEntry();

        public swtMainWindowColumns()
        {
            Xwt.MarkdownView wrn = new Xwt.MarkdownView();
            wrn.LoadText("#В связи с ограничениями Xwt.ListView тут настроек пока нет\n\nНеобходимо доработать сей виджет или написать аналог", new Xwt.Formats.MarkdownTextFormat());
            wrn.BackgroundColor = Xwt.Drawing.Colors.Transparent;
            box.PackStart(wrn);
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

            fraOtherBox.Add(new Xwt.Label(Locale.GetString("SWTMWCMaxHumanSizePan")), 0, 4);
            fraOtherBox.Add(txtMaxHumanySizeInPanels, 1, 4);
            fraOtherBox.Add(new Xwt.Label(Locale.GetString("SWTMWCMaxHumanSizeStatus")), 0, 5);
            fraOtherBox.Add(txtMaxHumanySizeInStatus, 1, 5);
        }

        public Xwt.Widget Content
        {
            get { return box; }
        }

        public bool SaveSettings()
        {
            return true;//undone
        }
    }
}
