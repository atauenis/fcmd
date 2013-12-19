/* The File Commander Settings window tabs
 * Tab "Viewing/editing"
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
    class swtViewerEditor : ISettingsWindowTab
    {
        Localizator Locale = new Localizator();
        Xwt.VBox box = new Xwt.VBox();
        Xwt.VBox fraViewBox = new Xwt.VBox();
        Xwt.Frame fraView = new Xwt.Frame();
        Xwt.RadioButtonGroup ViewerSelection = new Xwt.RadioButtonGroup();
        Xwt.RadioButton optInternalViewer = new Xwt.RadioButton();
        Xwt.RadioButton optExternalViewer = new Xwt.RadioButton();
        Xwt.TextEntry txtExternalViewer = new Xwt.TextEntry();
        Xwt.Button cmdVEsetup = new Xwt.Button();

        Xwt.Frame fraEdit = new Xwt.Frame();
        Xwt.VBox fraEditBox = new Xwt.VBox();
        Xwt.RadioButtonGroup EditorSelection = new Xwt.RadioButtonGroup();
        Xwt.RadioButton optInternalEditor = new Xwt.RadioButton();
        Xwt.RadioButton optExternalEditor = new Xwt.RadioButton();
        Xwt.TextEntry txtExternalEditor = new Xwt.TextEntry();

        public swtViewerEditor()
        {
            //prepare UI
            fraView.Content = fraViewBox;
            fraView.Label = Locale.GetString("SWTVEviewer");
            optInternalViewer.Group = ViewerSelection;
            optInternalViewer.Label = Locale.GetString("SWTVEinternalv");
            optExternalViewer.Group = ViewerSelection;
            optExternalViewer.Label = Locale.GetString("SWTVEexternalv");
            cmdVEsetup.Label = Locale.GetString("SWTVEvesetup");
            cmdVEsetup.Clicked += (o, ea) => { new VEsettings().Run(); };

            fraViewBox.PackStart(optInternalViewer);
            fraViewBox.PackStart(optExternalViewer);
            fraViewBox.PackStart(txtExternalViewer, true, Xwt.WidgetPlacement.Start, Xwt.WidgetPlacement.Start, 24);

            fraEdit.Content = fraEditBox;
            fraEdit.Label = Locale.GetString("SWTVEeditor");
            optInternalEditor.Group = EditorSelection;
            optInternalEditor.Label = Locale.GetString("SWTVEinternaleditor");
            optExternalEditor.Group = EditorSelection;
            optExternalEditor.Label = Locale.GetString("SWTVEexternaleditor");
            
            txtExternalViewer.Sensitive = false; //set default value...
            txtExternalEditor.Sensitive = false; //...later, it will be confirmed

            ViewerSelection.ActiveRadioButtonChanged += (o, ea) => { txtExternalViewer.Sensitive = optExternalViewer.Active; };
            EditorSelection.ActiveRadioButtonChanged += (o, ea) => { txtExternalEditor.Sensitive = optExternalEditor.Active; };


            fraEditBox.PackStart(optInternalEditor);
            fraEditBox.PackStart(optExternalEditor);
            fraEditBox.PackStart(txtExternalEditor, true, Xwt.WidgetPlacement.Start, Xwt.WidgetPlacement.Start, 24);

            box.PackStart(fraView);
            box.PackStart(fraEdit);
            box.PackStart(cmdVEsetup);

            //write settings on UI
            optExternalViewer.Active = fcmd.Properties.Settings.Default.UseExternalViewer;
            optExternalEditor.Active = fcmd.Properties.Settings.Default.UseExternalEditor;
            txtExternalEditor.Text = fcmd.Properties.Settings.Default.ExternalEditor;
            txtExternalViewer.Text = fcmd.Properties.Settings.Default.ExternalViewer;
            txtExternalEditor.PlaceholderText = "vim $";
            txtExternalViewer.PlaceholderText = "cat $ | less";
        }

        public Xwt.Widget Content { get { return box; } }
        public bool SaveSettings()
        {
            fcmd.Properties.Settings.Default.UseExternalViewer = optExternalViewer.Active;
            fcmd.Properties.Settings.Default.UseExternalEditor = optExternalEditor.Active;
            fcmd.Properties.Settings.Default.ExternalViewer = txtExternalViewer.Text;
            fcmd.Properties.Settings.Default.ExternalEditor = txtExternalEditor.Text;
            return true;
        }
    }
}
