/* The File Commander Settings window tabs
 * Tab "Themes"
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using fcmd.Properties;
using pluginner.Toolkit;
using Xwt;

namespace fcmd.SettingsWindowTabs
{
	class swtMainWindowThemes : ISettingsWindowTab
	{
		Stylist s = new Stylist(Settings.Default.UserTheme); //todo: add wysiwyg css editor

		Table layout = new Table();
		CheckBox chkUseThemes = new CheckBox("yuz kastom temez fo zi pogam's intefeys");
		TextEntry txtThemePath = new TextEntry();

		public swtMainWindowThemes()
		{
			if (Settings.Default.UserTheme == null)
			{
				txtThemePath.Text = "";
				chkUseThemes.State = CheckBoxState.Off;
			}
			else
			{
				txtThemePath.Text = Settings.Default.UserTheme;
				chkUseThemes.State = CheckBoxState.On;
			}

			txtThemePath.Sensitive = (chkUseThemes.State == CheckBoxState.On ? true : false);

			layout.Add(chkUseThemes,0,0);
			layout.Add(txtThemePath,1,0);

			chkUseThemes.Toggled+=(o,ea)=>{ txtThemePath.Sensitive = (chkUseThemes.State == CheckBoxState.On ? true : false); };

			Localizator.LocalizationChanged += Localizator_LocalizationChanged;
			Localizator_LocalizationChanged(null, null);
		}

		void Localizator_LocalizationChanged(object sender, System.EventArgs e)
		{
			chkUseThemes.Label = Localizator.GetString("SWTMWTusethemes");
		}

		public Widget Content
		{
			get { return layout; }
		}

		public bool SaveSettings()
		{
			if (txtThemePath.Text.Length == 0 || chkUseThemes.State == CheckBoxState.Off)
				Settings.Default.UserTheme = null;

			return true;
		}
	}
}
