/* The File Commander base plugins - Local filesystem adapter
 * The FTP filesystem plugin
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using pluginner;
using pluginner.Toolkit;

namespace fcmd.base_plugins.fs
{
	class FTPFileSystem : IFSPlugin
	{
		private FTPClient ftp;
		private string currentDirectory = "";
		private List<DirItem> directoryContent = new List<DirItem>();

		public static Regex FtpListDirectoryDetailsRegex = new Regex(@".*(?<month>(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec))\s*(?<day>[0-9]*)\s*(?<yearTime>([0-9]|:)*)\s*(?<fileName>.*)", RegexOptions.Compiled | RegexOptions.IgnoreCase); //undone: add style switching (windows, unix, etc)
		private byte[] Buffer = new byte[512];


		private void _CheckProtocol(string url)
		{ //проверка на то, чтобы нечаянно через ftpfs не попытались зайти в локальную ФС, webdav, реестр и т.п. :-)
			if (!url.ToLowerInvariant().StartsWith("ftp:")) throw new pluginner.PleaseSwitchPluginException();
		}

		private void LoadDir(string url)
		{
			currentDirectory = url;
			_CheckProtocol(url);

			Uri URI = new Uri(url);

			if (ftp == null)
				Connect(url);

// ReSharper disable once PossibleNullReferenceException, because Connect(url) would initialize it or would crash this constructor
			Socket sck = ftp.GetDataSocket();//possible ftpexception, todo add try...catch
			string ListResult;
			string dummy = "";
			ftp.SendCommand("CWD " + URI.PathAndQuery,out dummy,250);
			ftp.SendCommand("TYPE A");
			ftp.SendCommand("LIST", out ListResult);

			string directoryListing = "";//убрать после разборки с форматами ответов на "LIST"

			StreamReader sr = new StreamReader(new NetworkStream(sck));
			directoryContent.Clear();
			while (!sr.EndOfStream)
			{
				string CurItem = sr.ReadLine();
				directoryListing += CurItem+"\n";

// ReSharper disable PossibleNullReferenceException
				DirItem di = new DirItem();
				Match m = FtpListDirectoryDetailsRegex.Match(CurItem);

				di.IsDirectory = CurItem.StartsWith("d");
				if (di.IsDirectory) di.IconSmall = Utilities.GetIconForMIME("x-fcmd/directory");
				di.TextToShow = m.Groups["fileName"].Value;
				di.Date = DateTime.Now;
				di.URL = "ftp://" + URI.Host + ":" + URI.Port + URI.PathAndQuery + m.Groups["fileName"].Value + "/";
				directoryContent.Add(di);
// ReSharper restore PossibleNullReferenceException
			}
			ftp.ReadResponse();
		}

		private void Connect(string url)
		{
			Uri adr = new Uri(url);
			ftp = new FTPClient(
				adr.Host,
				adr.Port,
				"anonymous",
				@"test@test.ru"
			);
		}

		public IEnumerable<DirItem> DirectoryContent
		{
			get { return directoryContent; }
		}

		public string CurrentDirectory
		{
			get { return currentDirectory; }
			set { LoadDir(value); }
		}

		public bool FileExists(string URL)
		{
			throw new NotImplementedException();
		}

		public bool DirectoryExists(string URL)
		{
			return directoryContent.Any(di => di.URL == URL);
		}

		public bool CanBeRead(string URL)
		{
			throw new NotImplementedException();
		}

		public FSEntryMetadata GetMetadata(string URL)
		{
			//throw new NotImplementedException();
			return new FSEntryMetadata();
		}

		public byte[] GetFileContent(string URL)
		{
			throw new NotImplementedException();
		}

		public System.IO.Stream GetFileStream(string URL, bool Lock = false)
		{
			throw new NotImplementedException();
		}

		public void WriteFileContent(string URL, int Start, byte[] Content)
		{
			throw new NotImplementedException();
		}

		public void Touch(FSEntryMetadata metadata)
		{
			throw new NotImplementedException();
		}

		public void Touch(string URL)
		{
			throw new NotImplementedException();
		}

		public void DeleteFile(string URL)
		{
			throw new NotImplementedException();
		}

		public void MoveFile(string oldURL, string newURL)
		{
			throw new NotImplementedException();
		}

		public void DeleteDirectory(string URL, bool TrySafe)
		{
			throw new NotImplementedException();
		}

		public void CreateDirectory(string URL)
		{
			throw new NotImplementedException();
		}

		public void MoveDirectory(string OldURL, string NewURL)
		{
			throw new NotImplementedException();
		}

		public string DirSeparator
		{
			get { return "/"; }
		}

		public event TypedEvent<string> StatusChanged;

		public event TypedEvent<double> ProgressChanged;

		public void CLIstdinWriteLine(string StdIn)
		{
			throw new NotImplementedException();
		}

		public event TypedEvent<string> CLIstdoutDataReceived;

		public event TypedEvent<string> CLIstderrDataReceived;

		public event TypedEvent<string> CLIpromptChanged;

		public string Name { get { return "File Transfer Protocol"; } }
		public string Version { get { return System.Windows.Forms.Application.ProductVersion; } }
		public string Author { get { return "A.T."; } }

		public int[] APICompatibility
		{
			get
			{
				int[] fapiver = { 0, 1, 0, 0, 1, 0 };
				return fapiver;
			}
		}

		public object APICallPlugin(string call, params object[] arguments)
		{
			return null;
		}

		public event TypedEvent<object[]> APICallHost;

		public System.Configuration.Configuration FCConfig
		{
			set { /*not used because the plugin is internal*/ }
		}
	}
}
