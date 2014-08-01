/* The File Commander
 * The entry point for the fcmd.exe
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using System.Windows.Forms;
using pluginner.Toolkit;

namespace fcmd
{
	class Startup
	{
		[STAThread] //it's required due to WPF restrictions (without this, the Xwt.Wpf.dll backend is unable to start)
		static void Main(string[] Commands)
		{
			Console.WriteLine("The File Commander, version " + Application.ProductVersion + "\n(C) 2013-14, Alexander Tauenis and the FC development team (https://github.com/atauenis/fcmd).\nThe FC is licensed \"as is,\" with  no  warranties regarding product performance or non-infringement of third party intellectual property rights; the software may be modified without restrictions");
#if DEBUG
			try 
			{ 
				if (OSVersionEx.Platform == PlatformID.Win32NT)
				{
					Xwt.Application.Initialize(Xwt.ToolkitType.Wpf); //on Windows, you may set WPF or GTK as toolkit type for debugging purposes
				}
				else if (OSVersionEx.Platform == PlatformID.MacOSX)
				{
					Xwt.Application.Initialize(Xwt.ToolkitType.Cocoa);
				}
				else
				{
					Xwt.Application.Initialize(Xwt.ToolkitType.Gtk);
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
					"The File Commander " + Application.ProductVersion + " (" + (Environment.Is64BitProcess ? "x64" : "x86") + "-DEBUG) Startup Failure"
				);
				return;
			}
#else
			try
			{
				Xwt.Application.Initialize(OSVersionEx.GetToolkitType());
			}
			catch (Exception ex) {
				System.Windows.Forms.MessageBox.Show(
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
			Xwt.Application.Run();
#if !DEBUG
			}
			catch (Exception ex)
			{
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
