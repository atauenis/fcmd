/* The File Commander
 * PlatformHelper
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using fcmd.Enums;

namespace fcmd.Helpers
{
	public static class PlatformHelper
	{
		public static PlatformEnum GetPlatform()
		{
			switch (Environment.OSVersion.Platform)
			{
				case PlatformID.Win32NT:
					return PlatformEnum.Windows;

				case PlatformID.Unix:
				default:
					var platformHasDarwinKernel = GetPlatformHasDarwinKernel();
					return platformHasDarwinKernel
						? PlatformEnum.OSX
						: PlatformEnum.Unix;
			}
		}

		private static bool GetPlatformHasDarwinKernel()
		{
			if (_platformHasDarwinKernel == null)
			{
				var process = new Process();
				process.StartInfo.UseShellExecute = false;
				process.StartInfo.RedirectStandardOutput = true;
				process.StartInfo.FileName = "uname";

				try
				{
					process.Start();
					var output = process.StandardOutput.ReadToEnd();
					_platformHasDarwinKernel = output.StartsWith("Darwin", StringComparison.InvariantCultureIgnoreCase);
					process.WaitForExit();
				}
				catch (Win32Exception ex)
				{
					_platformHasDarwinKernel = false;
				}
				finally
				{
					process.Dispose();
				}
			}

			return _platformHasDarwinKernel.Value;
		}

		private static bool? _platformHasDarwinKernel;
	}
}
