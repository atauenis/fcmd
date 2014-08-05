/* The File Commander
 * The entry point for the fcmd.exe
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using Xwt;
using WinForms = System.Windows.Forms;
using pluginner.Toolkit;

namespace fcmd
{
	static class Startup
	{
		[STAThread] //it's required due to WPF restrictions (without this, the Xwt.Wpf.dll backend is unable to start)
		static void Main(string[] Commands)
		{
// ReSharper disable LocalizableElement
			Console.WriteLine("The File Commander, version " + WinForms.Application.ProductVersion + "\n(C) 2013-14, the File Commander development team (https://github.com/atauenis/fcmd).\nThe FC is licensed \"as is,\" with  no  warranties regarding product performance or non-infringement of third party intellectual property rights; the software may be modified without restrictions");
#if DEBUG
			try 
			{
				//for debugging purposes you may set any ToolkitType as you need
				switch (OSVersionEx.Platform)
				{
					case PlatformID.Win32NT:
						Application.Initialize(ToolkitType.Wpf);
						break;
					case PlatformID.MacOSX:
						Application.Initialize(ToolkitType.Cocoa);
						break;
					default:
						Application.Initialize(ToolkitType.Gtk);
						break;
				}
			}
			catch (Exception ex)
			{
				string errmsg = "The XWT could not be loaded:\n" + ex.InnerException.Message;

				if(ex.InnerException.InnerException != null){
					errmsg+="\n"+ex.InnerException.InnerException.Message;
				}

				System.Windows.Forms.MessageBox.Show(
						errmsg,
					"The File Commander " + WinForms.Application.ProductVersion + " (" + (Environment.Is64BitProcess ? "x64" : "x86") + "-DEBUG) Startup Failure"
				);
				return;
			}
#else
			try
			{
				Application.Initialize(OSVersionEx.GetToolkitType());
			}
			catch (Exception ex) {
				WinForms.MessageBox.Show(
				"The XWT could not be loaded:\n" + ex.InnerException.Message,
				"The File Commander " + Application.ProductVersion + " (" + (Environment.Is64BitProcess ? "x64" : "x86") + ") Startup Failure"
				);
				return;
			}
#endif
#if !DEBUG
			try{
#endif
			new MainWindow(Commands).Show();
			//todo: add splash screen
			Application.Run();
#if !DEBUG
			}
			catch (Exception ex)
			{
				//startup crash handler
				string msg = "The File Commander has been crashed:\n" + ex.Message + "\n" + ex.StackTrace;
				string inex = "";
				if(ex.InnerException != null) inex = "\n Inner exception" + ex.InnerException.Message + "\n" + ex.StackTrace;
				msg+=inex;

				System.Windows.Forms.MessageBox.Show(
					msg,
					"The File Commander " + Application.ProductVersion + " (" + (Environment.Is64BitProcess ? "x64" : "x86") + ") Crash"
				);
				return;
			}
#endif
		}
	}
}
