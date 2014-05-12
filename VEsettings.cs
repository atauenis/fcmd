/* The File Commander - internal Viewer/Editor
 * VE settings window
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fcmd
{
	class VEsettings : Xwt.Dialog
	{
		Localizator Locale = new Localizator();
		pluginner.Stylist s = new pluginner.Stylist();
		Xwt.VBox Layout = new Xwt.VBox();
		Xwt.CheckBox chkShowToolbar = new Xwt.CheckBox();
		Xwt.CheckBox chkShowCmdBar = new Xwt.CheckBox();
		public VEsettings()
		{
			this.Title = Locale.GetString("FCVES_Title");
			this.Content = Layout;
			this.Buttons.Add(new Xwt.Command("Ok"), new Xwt.Command("Cancel"));
			this.Buttons[0].Clicked += (o, ea) => { Save(); };
			this.Buttons[1].Clicked += (o, ea) => { this.Hide(); };

			s.Stylize(chkShowToolbar);
			s.Stylize(chkShowCmdBar);

			chkShowToolbar.Label = Locale.GetString("FCVES_ShowToolbar");
			chkShowToolbar.State = CBSfromBool(Properties.Settings.Default.VE_ShowToolbar);

			chkShowCmdBar.Label = Locale.GetString("FCVES_ShowCmdBar");
			chkShowCmdBar.State = CBSfromBool(Properties.Settings.Default.VE_ShowCmdBar);
			
			Layout.PackStart(chkShowToolbar);
			Layout.PackStart(chkShowCmdBar);
			
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
