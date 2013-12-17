/* The File Commander Settings window tabs
 * Interface for Settings window tabs
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
    interface ISettingsWindowTab
    {
        Xwt.Widget Content { get; }
        bool SaveSettings();
    }
}
