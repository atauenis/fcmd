/* The File Commander
 * Entry point for the fcmd.exe
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Windows.Forms;

namespace fcmd
{
    class Startup
    {
        [STAThread] //need because of unfixed wpf elementhost bug
        static void Main(string[] Commands)
        {
#if DEBUG
            Xwt.Application.Initialize(Xwt.ToolkitType.Wpf);
#else
            
            //todo: xwt.application.run together with winforms.application.run

            switch (Environment.OSVersion.Platform)
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
            }
#endif
            new MainWindow().Show();
            Application.Run(new frmMain());

            //note that the "MainWindow" is the modern main window (made using XWT)
            //and the "frmMain" is the old main window (made with Windows Forms)
        }
    }
}
