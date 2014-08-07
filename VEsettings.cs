/* The File Commander - internal Viewer/Editor
 * VE settings window
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;

namespace fcmd
{
	class VEsettings : Xwt.Dialog
	{
		Xwt.VBox Layout = new Xwt.VBox();
		Xwt.CheckBox chkShowToolbar = new Xwt.CheckBox();
		Xwt.CheckBox chkShowCmdBar = new Xwt.CheckBox();
		public VEsettings()
		{
			this.Content = Layout;
			this.Buttons.Add(new Xwt.Command("Ok"), new Xwt.Command("Cancel"));
			this.Buttons[0].Clicked += (o, ea) => { Save(); };
			this.Buttons[1].Clicked += (o, ea) => { this.Hide(); };

			chkShowToolbar.State = CBSfromBool(Properties.Settings.Default.VE_ShowToolbar);

			chkShowCmdBar.State = CBSfromBool(Properties.Settings.Default.VE_ShowCmdBar);
			
			Layout.PackStart(chkShowToolbar);
			Layout.PackStart(chkShowCmdBar);

			Localizator.LocalizationChanged += Localizator_LocalizationChanged;
			Localizator_LocalizationChanged(null,null);
		}

		void Localizator_LocalizationChanged(object sender, EventArgs e)
		{
			this.Title = Localizator.GetString("FCVES_Title");
			chkShowToolbar.Label = Localizator.GetString("FCVES_ShowToolbar");
			chkShowCmdBar.Label = Localizator.GetString("FCVES_ShowCmdBar");
		}

		private void Save()
		{
			Properties.Settings.Default.VE_ShowToolbar = BoolFromCBX(chkShowToolbar);
			Properties.Settings.Default.VE_ShowCmdBar = BoolFromCBX(chkShowCmdBar);

			this.Hide();
		}

		/// <summary>Converts boolean values into Xwt.CheckBoxState</summary>
		private Xwt.CheckBoxState CBSfromBool(bool Bulevo)
		{
			switch (Bulevo)
			{
				case true: return Xwt.CheckBoxState.On;
				case false: return Xwt.CheckBoxState.Off;
			}
			return Xwt.CheckBoxState.Mixed; //fallback
		}

		/// <summary>Converts Xwt.CheckBox selection status into boolean value</summary>
		private bool BoolFromCBX(Xwt.CheckBox CBX)
		{
			switch (CBX.State)
			{
				case Xwt.CheckBoxState.On: return true;
				case Xwt.CheckBoxState.Off: return false;
			}
			return false; //fallback
		}
	}
}
