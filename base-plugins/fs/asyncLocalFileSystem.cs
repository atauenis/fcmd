/* The File Commander base plugins - Local filesystem adapter
 * The main part of the LocalFS FS plugin
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (С) 2014, Zhigunov Andrew (breakneck11@gmail.com)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.AccessControl;
using pluginner;
using System.Diagnostics;

namespace fcmd.base_plugins.fs
{
	class asyncLocalFileSystem : pluginner.IFSPlugin
	{

		public asyncLocalFileSystem() {
			MillisecondsToWait = 1000;
		}

		private AsyncLocalDirProvider _current_dir = null;

		private string _path = null;

		/// <summary>
		/// Content of the directory
		/// </summary>
		public IEnumerable<DirItem> DirectoryContent
		{
			get { return _current_dir.Content; }
		}

		/// <summary>
		/// The directory which content is shown in <value>DirectoryContent</value>
		/// </summary>
		public string CurrentDirectory
		{
			get { return "file://" + _path; }
			set {
				_CheckProtocol(value);
				_path = _getPath(value);
				Reopen();
			}
		}

		private void Reopen()
		{
			if (_current_dir != null) {
				_current_dir.Dispose();
				_current_dir = null;
			}
			_current_dir = AsyncLocalDirProvider.CreateFromDirectory(_path);
		}

		private void _CheckProtocol(string url) {
			if (!url.StartsWith("file://"))
				throw new pluginner.PleaseSwitchPluginException();
		}

		private string _getPath(string url) {
			_CheckProtocol(url);
			return url.Replace("file://", "");
		}

		/// <summary>
		/// Check if the given path leads to the existing file
		/// </summary>
		/// <param name="URL">Path to the file</param>
		/// <returns></returns>
		public bool FileExists(string URL) {
			return File.Exists(_getPath(URL));
		}

		/// <summary>
		/// Checks if the given path leads tj the existing directory
		/// </summary>
		/// <param name="URL">Path to the directory</param>
		/// <returns></returns>
		public bool DirectoryExists(string URL) {
			_CheckProtocol(URL);
			return Directory.Exists(_getPath(URL));
		}

		/// <summary>
		/// Checks if a directory or a file with the given path can be read
		/// </summary>
		/// <param name="URL">Path to the directory or the file</param>
		/// <returns></returns>
		public bool CanBeRead(string URL) {
			string path = _getPath(URL);
			if (DirectoryExists(URL)) {
				// According to MSDN, Directory.Exist returns false, if an user doesn't have
				// rights to read it. We've already passed that case, so we are able to read the directory.
				return true;
			} else if (FileExists(URL)) {
				try {
					using (FileStream fs = new FileStream(path, FileMode.Open)) {
						return true;
					}
				} catch {
					return false;
				}
			} else {
				return false;
			}
		}

		public FSEntryMetadata GetMetadata(string URL) {
			return fcmd.base_plugins.fs.localFileSystem._GetMetadata(URL);
		}

		public byte[] GetFileContent(string URL) {
			_CheckProtocol(URL);
			return File.ReadAllBytes(_getPath(URL));
		}

		public System.IO.Stream GetFileStream(string URL, bool Write = false) {
			return new FileStream(_getPath(URL), FileMode.Open, Write ? FileAccess.ReadWrite : FileAccess.Read);
		}

		public void WriteFileContent(string URL, int Start, byte[] Content)
		{
			using (FileStream fs = new FileStream(_getPath(URL), FileMode.Open)) {
				fs.Seek(Start, SeekOrigin.Begin);
				using (BinaryWriter bw = new BinaryWriter(fs)) {
					bw.Write(Content);
				}
			}
		}

		public void Touch(FSEntryMetadata metadata) {
			localFileSystem._Touch(metadata);
		}

		public void Touch(string URL) {
			localFileSystem._Touch(URL);
		}

		public void DeleteFile(string URL) {
			File.Delete(_getPath(URL));
		}

		public void MoveFile(string oldURL, string newURL) {
			File.Move(_getPath(oldURL), _getPath(newURL));
		}

		// This code is based on Stack Overflow answer.
		// Any better ways to check that are welcome
		private static bool HasWritePermissionOnDir(string path, bool Recursive = false) {
			var writeAllow = false;
			var accessControlList = Directory.GetAccessControl(path);
			if (accessControlList == null)
				return false;
			var accessRules = accessControlList.GetAccessRules(true, true, 
										typeof(System.Security.Principal.SecurityIdentifier));
			if (accessRules ==null)
				return false;

			foreach (FileSystemAccessRule rule in accessRules) {
				if ((FileSystemRights.Write & rule.FileSystemRights) != FileSystemRights.Write) 
					continue;

				if (rule.AccessControlType == AccessControlType.Allow)
					writeAllow = true;
				else if (rule.AccessControlType == AccessControlType.Deny)
					return false;
			}

			return writeAllow && (!Recursive || Directory.EnumerateDirectories(path).All(
				dir => HasWritePermissionOnDir(path + Path.DirectorySeparatorChar + dir, true)));
		}

		public void DeleteDirectory(string URL, bool TrySafe) {
			if (TrySafe && !(DirectoryExists(URL) && HasWritePermissionOnDir(_getPath(URL), true))) {
				throw new pluginner.ThisDirCannotBeRemovedException();
			}
			Directory.Delete(_getPath(URL), true);
		}

		public void CreateDirectory(string URL) {
			Directory.CreateDirectory(_getPath(URL));
		}

		public void MoveDirectory(string OldURL, string NewURL) {
			Directory.Move(_getPath(OldURL), _getPath(NewURL));
		}

		public string DirSeparator { get { return Path.DirectorySeparatorChar.ToString(); } }

		Process current_process = null;

		bool IsProcessActive { get { return current_process != null; } }

		public int MillisecondsToWait { set; get; }

		public void CLIstdinWriteLine(string StdIn) {
			if (IsProcessActive) {
				current_process.StandardInput.WriteLine(StdIn);
				return;
			}
			try {
				ProcessStartInfo psi = null;
				StdIn = StdIn.Trim();
				if (StdIn.Contains(' ')) {
					psi = new ProcessStartInfo(StdIn.Substring(0, StdIn.IndexOf(' ')), StdIn.Substring(StdIn.IndexOf(' ') + 1));
				} else {
					psi = new ProcessStartInfo(StdIn);
				}
				psi.WorkingDirectory = _getPath(CurrentDirectory);
				psi.RedirectStandardOutput = true;
				psi.RedirectStandardInput = true;
				psi.RedirectStandardError = true;
				psi.UseShellExecute = false;
				psi.CreateNoWindow = true;
				current_process = Process.Start(psi);
				current_process.EnableRaisingEvents = true;
				current_process.OutputDataReceived += curent_process_OutputDataReceived;
				current_process.ErrorDataReceived += curent_process_ErrorDataReceived;
				current_process.BeginErrorReadLine();
				current_process.BeginOutputReadLine();

				while (!current_process.HasExited) {
					System.Threading.Thread.Sleep(MillisecondsToWait);
					Xwt.Application.MainLoop.DispatchPendingEvents();
				}
				Console.WriteLine("Process {0} exited.", current_process.ProcessName);
				current_process = null;
				var handler = CLIpromptChanged;
				if (handler != null) {
					handler("FC: " + _getPath(CurrentDirectory) + ">");
				}
			} catch (Exception e) {
				var process = current_process;
				current_process = null;
				Console.WriteLine("FC CLI error:" + e.Message);
			}
		}

		void curent_process_ErrorDataReceived(object sender, DataReceivedEventArgs e) {
			Console.WriteLine("STDERR {0}: {1}", current_process.ProcessName, e.Data);
			var handler = CLIstderrDataReceived;
			if (handler != null) {
				handler(e.Data);
			}
		}

		private void curent_process_OutputDataReceived(object sender, DataReceivedEventArgs e) {
			Console.WriteLine("STDOUT {0}: {1}", current_process.ProcessName, e.Data);
			var handler = CLIstdoutDataReceived;
			if (handler != null) {
				handler(e.Data);
			}
		}

		public event TypedEvent<string> CLIstdoutDataReceived;

		public event TypedEvent<string> CLIstderrDataReceived;

		public event TypedEvent<string> CLIpromptChanged;

		public string Name
		{
			get { return "AsyncLocalFS"; }
		}

		public string Version
		{
			get { return new Version(1, 0).ToString(); }
		}

		public string Author
		{
			get { return "Break-Neck"; }
		}

		public int[] APICompatibility
		{
			get { throw new NotImplementedException(); }
		}

		public object APICallPlugin(string call, params object[] arguments)
		{
			throw new NotImplementedException();
		}

		public event TypedEvent<object[]> APICallHost;

		public System.Configuration.Configuration FCConfig
		{
			set { /* do nothing, just log */ }
		}

		[Obsolete("Due to technical restrictions is never risen.")]
		public event TypedEvent<string> StatusChanged;

		[Obsolete("Due to technical restrictions is never risen.")]
		public event TypedEvent<double> ProgressChanged;
	}
}
