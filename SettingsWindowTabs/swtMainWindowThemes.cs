/* The File Commander Settings window tabs
 * Tab "Themes"
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pluginner.Toolkit;
using Xwt;

namespace fcmd.SettingsWindowTabs
{
	class swtMainWindowThemes : ISettingsWindowTab
	{
		Localizator Locale = new Localizator();
		Stylist s = new Stylist(fcmd.Properties.Settings.Default.UserTheme); //todo: add wysiwyg css editor

		Table layout = new Table();
		CheckBox chkUseThemes = new CheckBox("yuz kastom temez fo zi pogam's intefeys");
		TextEntry txtThemePath = new TextEntry();

		public swtMainWindowThemes()
		{
			chkUseThemes.Label = Locale.GetString("SWTMWTusethemes");
			if (fcmd.Properties.Settings.Default.UserTheme == null)
			{
				txtThemePath.Text = "";
				chkUseThemes.State = CheckBoxState.Off;
			}
			else
			{
				txtThemePath.Text = fcmd.Properties.Settings.Default.UserTheme;
				chkUseThemes.State = CheckBoxState.On;
			}

			txtThemePath.Sensitive = (chkUseThemes.State == CheckBoxState.On ? true : false);

			layout.Add(chkUseThemes,0,0);
			layout.Add(txtThemePath,1,0);

			chkUseThemes.Toggled+=(o,ea)=>{ txtThemePath.Sensitive = (chkUseThemes.State == CheckBoxState.On ? true : false); };
		}

		public Xwt.Widget Content
		{
			get { return layout; }
		}

		public bool SaveSettings()
		{
			if (txtThemePath.Text.Length == 0 || chkUseThemes.State == CheckBoxState.Off)
				fcmd.Properties.Settings.Default.UserTheme = null;

			return true;
		}
	}
}
