/* The File Commander base plugins - Local filesystem adapter
 * The command line helper
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Xwt;
using pluginner.Toolkit;

namespace fcmd.base_plugins.fs
{
	partial class localFileSystem
	{
		public event pluginner.TypedEvent<string> CLIstdoutDataReceived;
		public event pluginner.TypedEvent<string> CLIstderrDataReceived;
		public event pluginner.TypedEvent<string> CLIpromptChanged;

		protected void RaiseCLIpromptChanged(string data)
		{
			Application.Invoke(delegate()
			{
				var handler = CLIpromptChanged;
				if (handler != null) {
					handler(data);
				}
			});
		}

		protected void RaiseCLIstderrDataReceived(string data)
		{
			Application.Invoke(delegate()
			{
				var handler = CLIstderrDataReceived;
				if (handler != null) {
					handler(data);
				}
			});
		}

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
					RaiseCLIpromptChanged("FC: " + procname + ">");
					CLIproc.EnableRaisingEvents = true;
					CLIproc.OutputDataReceived += CLIproc_OutputDataReceived;
					CLIproc.ErrorDataReceived += CLIproc_ErrorDataReceived;
					CLIproc.BeginOutputReadLine();
					CLIproc.BeginErrorReadLine();

					while (!CLIproc.HasExited)
					{
						try
						{
							Xwt.Application.MainLoop.DispatchPendingEvents();
						}
						catch {
							//for incresing stability on systems with bad RAM
						}
					}
					CLIsomethingIsRunning = false;
					Console.WriteLine("Stopped: " + procname);
					RaiseCLIpromptChanged("FC: " + CurrentDirectory.Replace("file://","") + ">");
				}
				catch (Exception ex)
				{
					CLIsomethingIsRunning = false;
					bool inCmd = false;
					try
					{
						//if the OS is Windows NT, try to run the command in CMD.EXE
						if (OSVersionEx.Platform == PlatformID.Win32NT){
							new Process {StartInfo = new ProcessStartInfo("cmd.exe", "/C \"" + StdIn + " && pause\"")}.Start();
							inCmd = true;
						}
					}
					catch
					{ RaiseCLIstderrDataReceived(Localizator.GetString("CantRunEXE") + "cmd.exe /C \"" + StdIn + " && pause\"" + "\n" + ex.Message); }
					if(!inCmd)
					RaiseCLIstderrDataReceived(Localizator.GetString ("CantRunEXE") + StdIn + "\n" + ex.Message);
				}
			}
			else {
				CLIproc.StandardInput.WriteLine(StdIn);
			}
		}

		private void CLIproc_Exited(object sender, EventArgs e)
		{
			CLIsomethingIsRunning = false;
			CLIproc.EnableRaisingEvents = false;//на всякий случай :-)
		}

		private void CLIproc_ErrorDataReceived(object sender, DataReceivedEventArgs e)
		{
			if (CLIproc.HasExited || e.Data == null) {
				return;
			}

			var _CLIstderrDataReceived = CLIstderrDataReceived;
			if (_CLIstderrDataReceived != null) {
				_CLIstderrDataReceived(e.Data);
				return;
			}

			Xwt.Application.Invoke(() => Xwt.MessageDialog.ShowWarning(CLIproc.ProcessName, e.Data));
		}

		private void CLIproc_OutputDataReceived(object sender, DataReceivedEventArgs e)
		{
			var _CLIstdoutDataReceived = CLIstdoutDataReceived;
			if (_CLIstdoutDataReceived != null) {
				_CLIstdoutDataReceived(e.Data);
				return;
			}

			try {
				Console.WriteLine ("STDOUT of " + CLIproc.ProcessName + ": " + e.Data + "\n");
			}
			catch (InvalidOperationException) {
				/*do nothing, perhaps the data came later than the process was ended*/
			}
		}
	}
}
