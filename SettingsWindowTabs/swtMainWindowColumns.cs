/* The File Commander Settings window tabs
 * Tab "Columns in panels"
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using fcmd.Properties;
using Xwt;

namespace fcmd.SettingsWindowTabs
{
	class swtMainWindowColumns : ISettingsWindowTab
	{
		VBox box = new VBox();

		Frame fraExtensions = new Frame {Sensitive = false};
		VBox fraExtensionsBox = new VBox();
		RadioButtonGroup rbgExtensionDisplayStyle = new RadioButtonGroup();
		RadioButton optDisplayExtTogether = new RadioButton();
		RadioButton optDisplayExtFar = new RadioButton();

		Frame fraTabs = new Frame {Sensitive = false};
		Table fraTabsBox = new Table();
		Label lblTabExt = new Label();
		Label lblTabSize = new Label();
		Label lblTabDate = new Label();
		Label lblTabPermission = new Label();
		TextEntry txtTabExtension = new TextEntry();
		TextEntry txtTabSize = new TextEntry();
		TextEntry txtTabDate = new TextEntry();
		TextEntry txtTabFilemode = new TextEntry();

		Frame fraOther = new Frame();
		Table fraOtherBox = new Table();
		CheckBox chkExpandName = new CheckBox {Sensitive = false};
		CheckBox chkShowCentury = new CheckBox {Sensitive = false};
		CheckBox chkShowTimeAs12h = new CheckBox {Sensitive = false};
		CheckBox chkShowDirsInStatus = new CheckBox {Sensitive = false};
		Label lblSizeDisplayStyle = new Label();
		ComboBox cmbPanelSizeDisplay = new ComboBox();
		TextEntry txtMaxHumanySizeInStatus = new TextEntry {Sensitive = false};

		public swtMainWindowColumns()
		{
			box.PackStart(fraExtensions);
			box.PackStart(fraTabs);
			box.PackStart(fraOther);
			fraExtensions.Content = fraExtensionsBox;
			fraTabs.Content = fraTabsBox;
			fraOther.Content = fraOtherBox;

			optDisplayExtTogether.Group = rbgExtensionDisplayStyle;
			optDisplayExtFar.Group = rbgExtensionDisplayStyle;
			fraExtensionsBox.PackStart(optDisplayExtTogether);
			fraExtensionsBox.PackStart(optDisplayExtFar);

			fraTabsBox.Add(lblTabExt, 0, 0);
			fraTabsBox.Add(txtTabExtension, 1, 0);

			fraTabsBox.Add(lblTabSize, 0, 1);
			fraTabsBox.Add(txtTabSize, 1, 1);

			fraTabsBox.Add(lblTabDate, 0, 2);
			fraTabsBox.Add(txtTabDate, 1, 2);

			fraTabsBox.Add(lblTabPermission, 0, 3);
			fraTabsBox.Add(txtTabFilemode, 1, 3);

			fraOtherBox.Add(chkExpandName, 0, 0);
			fraOtherBox.Add(chkShowCentury, 0, 1);
			fraOtherBox.Add(chkShowTimeAs12h, 0, 2);
			fraOtherBox.Add(chkShowDirsInStatus, 0, 3);

			fraOtherBox.Add(lblSizeDisplayStyle, 0, 4);
			fraOtherBox.Add(cmbPanelSizeDisplay, 1, 4);
			fraOtherBox.Add(new Label(Localizator.GetString("SWTMWCMaxHumanSizeStatus")), 0, 5);
			fraOtherBox.Add(txtMaxHumanySizeInStatus, 1, 5); //устаревшая реализация сокращения размера в инфобаре, todo: заменить на аналогичную вышестоящей настройке


			//load settings
			//todo

			Localizator.LocalizationChanged += Localizator_LocalizationChanged;
			Localizator_LocalizationChanged(null,null);
		}

		void Localizator_LocalizationChanged(object sender, EventArgs e)
		{
			fraExtensions.Label = Localizator.GetString("SWTMWCFileExtView");
			fraTabs.Label = Localizator.GetString("SWTMWCColumns");
			optDisplayExtTogether.Label = Localizator.GetString("SWTMWCExtTogether");
			optDisplayExtFar.Label = Localizator.GetString("SWTMWCExtFar");
			lblSizeDisplayStyle.Text = Localizator.GetString("SWTMWCSizeDisplay");
			lblTabPermission.Text = Localizator.GetString("SWTMWCFileMode");
			lblTabDate.Text = Localizator.GetString("SWTMWCDate");
			lblTabSize.Text = Localizator.GetString("SWTMWCSize");
			lblTabExt.Text = Localizator.GetString("SWTMWCExt");
			chkExpandName.Label = Localizator.GetString("SWTMWCExpandName");
			chkShowCentury.Label = Localizator.GetString("SWTMWCShowCentury");
			chkShowTimeAs12h.Label = Localizator.GetString("SWTMWCShowTimeAs12h");
			chkShowDirsInStatus.Label = Localizator.GetString("SWTMWCShowDirsInStatus");

			cmbPanelSizeDisplay.Items.Clear();
			cmbPanelSizeDisplay.Items.Add("000", Localizator.GetString("SizeDisplayPolicy000"));
			cmbPanelSizeDisplay.Items.Add("100", Localizator.GetString("SizeDisplayPolicy100"));
			cmbPanelSizeDisplay.Items.Add("200", Localizator.GetString("SizeDisplayPolicy200"));
			cmbPanelSizeDisplay.Items.Add("111", Localizator.GetString("SizeDisplayPolicy111"));
			cmbPanelSizeDisplay.Items.Add("222", Localizator.GetString("SizeDisplayPolicy222"));
			cmbPanelSizeDisplay.Items.Add("110", Localizator.GetString("SizeDisplayPolicy110"));
			cmbPanelSizeDisplay.Items.Add("220", Localizator.GetString("SizeDisplayPolicy220"));

			/*
			SizeDisplayPolicy000=bytes
			SizeDisplayPolicy100=kbytes
			SizeDisplayPolicy200=kbytes (x.xx KB)
			SizeDisplayPolicy111=dynamic (x.x К/М/Г)
			SizeDisplayPolicy222=dynamic (x.xx К/М/Г)
			SizeDisplayPolicy110=dynamic (x.x К/М)
			SizeDisplayPolicy220=dynamic (x.xx K/M)
			*/

			switch (Settings.Default.SizeShorteningPolicy)
			{
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

		public Widget Content
		{
			get { return box; }
		}

		public bool SaveSettings()
		{
			Settings.Default.SizeShorteningPolicy = cmbPanelSizeDisplay.SelectedItem.ToString();
			return true;//undone
		}
	}
}
