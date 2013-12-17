/* The File Commander
 * Entry point for the fcmd.exe
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Windows.Forms;

namespace fcmd
{
    class Startup
    {
        [STAThread] //need because of unfixed wpf elementhost bug
        static void Main()
        {
            Xwt.Application.Initialize(Xwt.ToolkitType.Gtk );
            Application.Run(new frmMain());//BUG: github issue #2
            //todo: xwt.application.run together with winforms.application.run

            /*switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32NT:
                    Xwt.Application.Initialize(Xwt.ToolkitType.Wpf);
                    break;
                case PlatformID.MacOSX: //i don't sure that Mono detect OSX as OSX, not Unix; see http://mono.wikia.com/wiki/Detecting_the_execution_platform
                    Xwt.Application.Initialize(Xwt.ToolkitType.Cocoa);
                    break;
                default:
                case PlatformID.Unix: //gtk fallback for unknown oses
                    Xwt.Application.Initialize(Xwt.ToolkitType.Gtk);
                    break;
            }*/
        }
    }
}
