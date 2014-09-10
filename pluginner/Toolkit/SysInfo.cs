/* The File Commander - plugin API
 * Cross-platform system information provider (finder)
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WinForms = System.Windows.Forms;

namespace pluginner.Toolkit
{
	/// <summary>
	/// Provides information about the current system environment.
	/// </summary>
	public static class SysInfo
	{
		/// <summary>
		/// Gets the maximum amount of time, in milliseconds, that can elapse between a first click and a second click for the OS to consider the mouse action a double-click.
		/// </summary>
		public static double DoubleClickTime
		{
			get
			{
				switch (OSVersionEx.Platform)
				{
					case PlatformID.Win32Windows:
					case PlatformID.Win32S:
					case PlatformID.Win32NT:
					case PlatformID.WinCE:
					case PlatformID.Xbox:
						return WinForms.SystemInformation.DoubleClickTime;
					case PlatformID.Unix:
						//in the future, here should be returned a value from GTK+ settings
					case PlatformID.MacOSX:
						return 1000; //"workaround" for https://github.com/atauenis/fcmd/issues/8#issuecomment-51187241
					default:
						return 1000;
				}
			}
		}
	}
}
