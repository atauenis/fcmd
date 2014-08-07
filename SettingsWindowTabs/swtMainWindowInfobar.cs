using fcmd.Properties;
using Xwt;

namespace fcmd.SettingsWindowTabs
{
	class swtMainWindowInfobar : ISettingsWindowTab
	{
		private Frame fraInfobar = new Frame();
		private VBox Layout = new VBox();

		private Label lblInfobarTextNoSel = new Label();
		private Label lblInfobarTextWithSel = new Label();
		private Label lblInfobarPatterns = new Label(){ Wrap = WrapMode.Word };

		private TextEntry txtText1 = new TextEntry();
		private TextEntry txtText2 = new TextEntry();

		public swtMainWindowInfobar()
		{
			
			fraInfobar.Content = Layout;
			Layout.PackStart(lblInfobarTextNoSel);
			Layout.PackStart(txtText1);
			Layout.PackStart(lblInfobarTextWithSel);
			Layout.PackStart(txtText2);
			Layout.PackStart(lblInfobarPatterns);

			txtText1.Text = Settings.Default.InfoBarContent1;
			txtText2.Text = Settings.Default.InfoBarContent2;

			Localizator.LocalizationChanged += Localizator_LocalizationChanged;
			Localizator_LocalizationChanged(null, null);
		}

		void Localizator_LocalizationChanged(object sender, System.EventArgs e)
		{
			fraInfobar.Label = Localizator.GetString("SWTMWinfobar");
			lblInfobarTextNoSel.Text = Localizator.GetString("SWTMWItext1");
			lblInfobarTextWithSel.Text = Localizator.GetString("SWTMWItext2");
			lblInfobarPatterns.Text = Localizator.GetString("SWTMWIhelp");
		}

		public Widget Content
		{
			get { return fraInfobar; }
		}

		public bool SaveSettings()
		{
			Settings.Default.InfoBarContent1 = txtText1.Text;
			Settings.Default.InfoBarContent2 = txtText2.Text;
			return true;
		}
	}
}
