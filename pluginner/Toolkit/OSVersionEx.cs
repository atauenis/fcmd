/* The File Commander
 * OSVersionEx, decorator for Environment.OSVersion
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using System.ComponentModel;
using System.Diagnostics;
using Xwt;

namespace pluginner.Toolkit
{
	// ReSharper disable once InconsistentNaming
	public static class OSVersionEx
	{
		public static ToolkitType GetToolkitType()
		{
			var platform = Platform;
			switch (platform)
			{
				case PlatformID.Win32NT:
					return ToolkitType.Wpf;
				case PlatformID.Unix:
					return ToolkitType.Gtk3;
				case PlatformID.MacOSX:
					return ToolkitType.Cocoa;
				default:
					throw new NotSupportedException(string.Format("Not supported value {0} for {1} type", platform.ToString(), typeof(PlatformID)));
			}
		}

		public static PlatformID Platform
		{
			get
			{
				var platformId = Environment.OSVersion.Platform;
				if (platformId != PlatformID.Unix)
				{
					return platformId;
				}

				var platformHasDarwinKernel = GetPlatformHasDarwinKernel();
				return platformHasDarwinKernel
					? PlatformID.MacOSX
					: PlatformID.Unix;
			}
		}

		public static string ServicePack
		{
			get { return Environment.OSVersion.ServicePack; }
		}

		public static Version Version
		{
			get { return Environment.OSVersion.Version; }
		}

		public static string VersionString
		{
			get { return Environment.OSVersion.VersionString; }
		}

		private static bool GetPlatformHasDarwinKernel()
		{
			if (_platformHasDarwinKernel != null)
			{
				return _platformHasDarwinKernel.Value;
			}

			var process = new Process {StartInfo = {FileName = "uname", UseShellExecute = false, RedirectStandardOutput = true}};
			try
			{
				process.Start();
				var processOutput = process.StandardOutput.ReadToEnd();
				_platformHasDarwinKernel = processOutput.StartsWith("Darwin", StringComparison.InvariantCultureIgnoreCase);
				process.WaitForExit();
			}
			catch (Win32Exception)
			{
				//when "uname" not found
				_platformHasDarwinKernel = false;
			}
			finally
			{
				process.Dispose();
			}

			return _platformHasDarwinKernel.Value;
		}

		private static bool? _platformHasDarwinKernel;
	}
}
