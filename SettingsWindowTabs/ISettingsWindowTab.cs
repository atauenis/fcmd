/* The File Commander Settings window tabs
 * Interface for Settings window tabs
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using Xwt;

namespace fcmd.SettingsWindowTabs
{
	interface ISettingsWindowTab
	{
		Widget Content { get; }
		bool SaveSettings();
	}
}
