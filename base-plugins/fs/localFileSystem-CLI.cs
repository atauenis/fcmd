/* The File Commander base plugins - Local filesystem adapter
 * The command line helper
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace fcmd.base_plugins.fs
{
	partial class localFileSystem
	{
		public event pluginner.TypedEvent<string> CLIstdoutDataReceived;
		public event pluginner.TypedEvent<string> CLIstderrDataReceived;
		public event pluginner.TypedEvent<string> CLIpromptChanged;

		Process CLIproc = new Process();
		Boolean CLIsomethingIsRunning = false;

		public void CLIstdinWriteLine(string StdIn)
		{
			if (!CLIsomethingIsRunning) {
				try
				{
					int ParamStart = StdIn.IndexOf(" ");

					ProcessStartInfo psi;
					if (ParamStart >= 0)
						psi = new ProcessStartInfo(StdIn.Substring(0, ParamStart), StdIn.Substring(ParamStart + 1));
					else
						psi = new ProcessStartInfo(StdIn);
					psi.WorkingDirectory = CurrentDirectory.Replace("file://", "");
					psi.RedirectStandardOutput = true;
					psi.RedirectStandardInput = true;
					psi.RedirectStandardError = true;
					psi.UseShellExecute = false;
					psi.CreateNoWindow = true;
					CLIsomethingIsRunning = true;

					CLIproc = Process.Start(psi);
					string procname = CLIproc.ProcessName;
					Console.WriteLine("Started: " + procname);
					if (CLIpromptChanged != null)
						CLIpromptChanged("FC: " + procname + ">");
					CLIproc.EnableRaisingEvents = true;
					CLIproc.OutputDataReceived += new DataReceivedEventHandler(CLIproc_OutputDataReceived);
					CLIproc.ErrorDataReceived += new DataReceivedEventHandler(CLIproc_ErrorDataReceived);
					CLIproc.BeginOutputReadLine();
					CLIproc.BeginErrorReadLine();

					while (!CLIproc.HasExited)
					{
						try
						{ Xwt.Application.MainLoop.DispatchPendingEvents(); }
						catch { } //for incresing stability on systems with bad RAM
					}
					CLIsomethingIsRunning = false;
					Console.WriteLine("Stopped: " + procname);
					if (CLIpromptChanged != null)
						CLIpromptChanged("FC: " + CurrentDirectory.Replace("file://","") + ">");
				}
				catch (Exception ex)
				{
					CLIsomethingIsRunning = false;
					if (CLIstderrDataReceived != null)
					CLIstderrDataReceived(new Localizator().GetString("CantRunEXE") + StdIn + "\n" + ex.Message);
					//todo: add at win32 systems calling "cmd.exe /C" if the program can't start
				}
			}
			else{
				CLIproc.StandardInput.WriteLine(StdIn);
			}
		}

		void CLIproc_Exited(object sender, EventArgs e)
		{
			CLIsomethingIsRunning = false;
			CLIproc.EnableRaisingEvents = false;//на всякий случай :-)
		}
		
		void CLIproc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (CLIproc.HasExited || e.Data == null) return;
			if (CLIstderrDataReceived != null) CLIstderrDataReceived(e.Data);
			else
				Xwt.Application.Invoke(new Action(delegate { Xwt.MessageDialog.ShowWarning(CLIproc.ProcessName, e.Data); }));
		}

		void CLIproc_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (CLIstdoutDataReceived != null) CLIstdoutDataReceived(e.Data);
			else
				try
				{
					Console.WriteLine("STDOUT of " + CLIproc.ProcessName + ": " + e.Data + "\n");
				}
				catch (InvalidOperationException)
				{ /*do nothing, perhaps the data came later than the process was ended*/
				}
		}
	}
}
