/* The File Commander Settings window tabs
 * Tab "Viewing/editing"
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using fcmd.Properties;
using Xwt;

namespace fcmd.SettingsWindowTabs
{
	class swtViewerEditor : ISettingsWindowTab
	{
		VBox box = new VBox();
		VBox fraViewBox = new VBox();
		Frame fraView = new Frame();
		RadioButtonGroup ViewerSelection = new RadioButtonGroup();
		RadioButton optInternalViewer = new RadioButton();
		RadioButton optExternalViewer = new RadioButton();
		TextEntry txtExternalViewer = new TextEntry();
		Button cmdVEsetup = new Button();

		Frame fraEdit = new Frame();
		VBox fraEditBox = new VBox();
		RadioButtonGroup EditorSelection = new RadioButtonGroup();
		RadioButton optInternalEditor = new RadioButton();
		RadioButton optExternalEditor = new RadioButton();
		TextEntry txtExternalEditor = new TextEntry();

		public swtViewerEditor()
		{
			//prepare UI
			fraView.Content = fraViewBox;
			optInternalViewer.Group = ViewerSelection;
			optExternalViewer.Group = ViewerSelection;
			cmdVEsetup.Clicked += (o, ea) => { new VEsettings().Run(); };

			fraViewBox.PackStart(optInternalViewer);
			fraViewBox.PackStart(optExternalViewer);
			fraViewBox.PackStart(txtExternalViewer, true, WidgetPlacement.Start, WidgetPlacement.Start, 24);

			fraEdit.Content = fraEditBox;
			optInternalEditor.Group = EditorSelection;
			optExternalEditor.Group = EditorSelection;
			
			txtExternalViewer.Sensitive = false; //set default value...
			txtExternalEditor.Sensitive = false; //...later, it will be confirmed

			ViewerSelection.ActiveRadioButtonChanged += (o, ea) => { txtExternalViewer.Sensitive = optExternalViewer.Active; };
			EditorSelection.ActiveRadioButtonChanged += (o, ea) => { txtExternalEditor.Sensitive = optExternalEditor.Active; };


			fraEditBox.PackStart(optInternalEditor);
			fraEditBox.PackStart(optExternalEditor);
			fraEditBox.PackStart(txtExternalEditor, true, WidgetPlacement.Start, WidgetPlacement.Start, 24);

			box.PackStart(fraView);
			box.PackStart(fraEdit);
			box.PackStart(cmdVEsetup);

			//write settings on UI
			optExternalViewer.Active = Settings.Default.UseExternalViewer;
			optExternalEditor.Active = Settings.Default.UseExternalEditor;
			txtExternalEditor.Text = Settings.Default.ExternalEditor;
			txtExternalViewer.Text = Settings.Default.ExternalViewer;
			txtExternalEditor.PlaceholderText = "vim $";
			txtExternalViewer.PlaceholderText = "cat $ | less";

			Localizator.LocalizationChanged += Localizator_LocalizationChanged;
			Localizator_LocalizationChanged(null, null);
		}

		void Localizator_LocalizationChanged(object sender, System.EventArgs e)
		{
			fraView.Label = Localizator.GetString("SWTVEviewer");
			optInternalViewer.Label = Localizator.GetString("SWTVEinternalv");
			optExternalViewer.Label = Localizator.GetString("SWTVEexternalv");
			cmdVEsetup.Label = Localizator.GetString("SWTVEvesetup");
			fraEdit.Label = Localizator.GetString("SWTVEeditor");
			optInternalEditor.Label = Localizator.GetString("SWTVEinternaleditor");
			optExternalEditor.Label = Localizator.GetString("SWTVEexternaleditor");
		}

		public Widget Content { get { return box; } }
		public bool SaveSettings()
		{
			Settings.Default.UseExternalViewer = optExternalViewer.Active;
			Settings.Default.UseExternalEditor = optExternalEditor.Active;
			Settings.Default.ExternalViewer = txtExternalViewer.Text;
			Settings.Default.ExternalEditor = txtExternalEditor.Text;
			return true;
		}
	}
}
