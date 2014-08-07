/* The File Commander base plugins - Local filesystem adapter
 * The main part of the LocalFS FS plugin
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * (С) 2014, Zhigunov Andrew (breakneck11@gmail.com)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.IO;
using pluginner.Toolkit;

namespace fcmd.base_plugins.fs
{
	public partial class localFileSystem : pluginner.IFSPlugin
	{
		/* ЗАМЕТКА РАЗРАБОТЧИКУ				DEVELOPER NOTES
		 * В данном файле содержится код	This file contanis the local FS
		 * плагина доступа к локальным ФС.	adapter for the File Commander.
		 * Не забывайте про отличия сред	Please don't forget about the differences
		 * *nix и MacOS X от Windows.		between OSX, *nix and Win32.
		 * Код должен работать везде!		THE CODE MUST WORK EVERYWHERE!
		 */
		public string Name { get { return Localizator.GetString("LocalFSVer"); } }
		public string Version { get{return "1.0";} }
		public string Author { get{return "A.T.";} }
		public System.Configuration.Configuration FCConfig { set {} } //it can be a placeholder because the LFS can use the fcmd.Properties.Settings...
		public List<pluginner.DirItem> DirectoryContent {get{return DirContent;}} //возврат директории в FC
		public event pluginner.TypedEvent<String> StatusChanged;
		public event pluginner.TypedEvent<double> ProgressChanged;
		public event pluginner.TypedEvent<object[]> APICallHost;

		List<pluginner.DirItem> DirContent = new List<pluginner.DirItem>();
		string CurDir;

		public string CurrentDirectory
		{
			get { return CurDir; }
			set {
				CurDir = value;
				ReadDirectory(value);
			}
		}

		private void _CheckProtocol(string url){ //проверка на то, чтобы нечаянно через localfs не попытались зайти в ftp, webdav, реестр и т.п. :-)
			if(!url.StartsWith("file:")) throw new pluginner.PleaseSwitchPluginException();
		}

		public string DirSeparator{get{return Path.DirectorySeparatorChar.ToString();}}

		public bool FileExists(string URL){//проверить наличие файла
			_CheckProtocol(URL);
			string InternalURL = URL.Replace("file://","");
			if(File.Exists(InternalURL)) return true; //файл е?
			return false; //та ничого нэма! [не забываем, что return xxx прекращает выполнение подпрограммы]
		}

		public bool DirectoryExists(string URL){//проверить наличие папки
			_CheckProtocol(URL);
			string InternalURL = URL.Replace("file://","");
			if(Directory.Exists(InternalURL)) return true; //каталох е?
			return false; //та ничого нэма! [не забываем, что return xxx прекращает выполнение подпрограммы]
		}

		public void ReadDirectory(string url){//прочитать каталог и загнать в DirectoryContent
			_CheckProtocol(url);
			DirContent.Clear();
			string InternalURL = url.Replace("file://", "");
			if (StatusChanged != null) StatusChanged(string.Format(Localizator.GetString("DoingListdir"), "", InternalURL));

			pluginner.DirItem tmpVar = new pluginner.DirItem();

			string[] files = System.IO.Directory.GetFiles(InternalURL);
			string[] dirs = System.IO.Directory.GetDirectories (InternalURL);
			float FileWeight = 1 / ((float)files.Length + (float)dirs.Length);
			float Progress = 0;

			//элемент "вверх по древу"
			DirectoryInfo curdir = new DirectoryInfo(InternalURL);
			if (curdir.Parent != null){
				tmpVar.Path = "file://" + curdir.Parent.FullName;
				tmpVar.TextToShow = "..";
				tmpVar.MIMEType = "x-fcmd/up";
				tmpVar.IconSmall = Utilities.GetIconForMIME("x-fcmd/up");
				DirContent.Add(tmpVar);
			}

			uint counter = 0;
			// 2 ** 10 ~= 1000 (is about 1000)
			// so dispatching will be done every time 1000 files will have been looked throught
			// update_every == 00...0011...11 in binary format and count of '1' is 10
			// so (++counter & update_every) == 0 will be true after every 2 ** 10 ~= 1000
			// passed files
			const uint update_every = ~(((~(uint)0) >> 10) << 10);

			foreach(string curDir in dirs){
				//перебираю каталоги
				DirectoryInfo di = new DirectoryInfo(curDir);
				tmpVar.IsDirectory = true;
				tmpVar.Path = "file://" + curDir;
				tmpVar.TextToShow = di.Name;
				tmpVar.Date = di.CreationTime;
				if (di.Name.StartsWith(".")) {
					tmpVar.Hidden = true;
				}else{
					tmpVar.Hidden = false;
				}
				tmpVar.MIMEType = "x-fcmd/directory";
				tmpVar.IconSmall = Utilities.GetIconForMIME("x-fcmd/directory");

				DirContent.Add(tmpVar);
				Progress += FileWeight;
				if (ProgressChanged != null) { ProgressChanged(Progress); }
				if ((++counter & update_every) == 0) {
					Xwt.Application.MainLoop.DispatchPendingEvents(); 
				}
			}

			foreach(string curFile in files){
				FileInfo fi = new FileInfo(curFile);
				tmpVar.IsDirectory = false;
				tmpVar.Path = "file://" + curFile;
				tmpVar.TextToShow = fi.Name;
				tmpVar.Date = fi.LastWriteTime;
				tmpVar.Size = fi.Length;
				if (fi.Name.StartsWith(".")) {
					tmpVar.Hidden = true;
				}else{
					tmpVar.Hidden = false;
				}

				if (curFile.LastIndexOf('.') > 0)
					tmpVar.MIMEType = Utilities.GetContentType(curFile.Substring(curFile.LastIndexOf('.')+1));
				else
					tmpVar.MIMEType = "application/octet-stream";

				tmpVar.IconSmall = Utilities.GetIconForMIME(tmpVar.MIMEType);

				DirContent.Add(tmpVar);
				Progress += FileWeight;
				if (ProgressChanged != null && Progress <= 1) { ProgressChanged(Progress); }
				if ((++counter & update_every) == 0) {
					Xwt.Application.MainLoop.DispatchPendingEvents();
				}
			}
			if (ProgressChanged != null) { ProgressChanged(2); }
			if (StatusChanged != null) { StatusChanged(""); };


			if (CLIpromptChanged != null)
			{
				CLIpromptChanged("FC: " + InternalURL + ">");
			}
		}

		public bool CanBeRead(string url){ //проверить файл/папку "URL" на читаемость
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://","");

			try{
				bool IsDir = Directory.Exists(InternalURL);
				if (IsDir)
				{//проверка читаемости каталога
					System.IO.Directory.GetFiles(InternalURL);
				}
				else
				{//проверка читаемости файла
					File.ReadAllBytes(InternalURL);
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine("LocalFS: Can't get access to " + InternalURL + "\nThe blocking reason is: " + ex.Message);
				return false;
			}
		}

		public void Touch(pluginner.FSEntryMetadata Metadata)
		{
			string url = Metadata.FullURL;
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://", "");

			if (!Directory.Exists(InternalURL) && !File.Exists(InternalURL))
			{
				StreamWriter sw = File.CreateText(InternalURL);
				sw.Close();
				sw.Dispose();
			}

			try
			{
				File.SetAttributes(InternalURL, Metadata.Attrubutes);
				File.SetCreationTime(InternalURL, Metadata.CreationTimeUTC);
				File.SetLastWriteTime(InternalURL, DateTime.Now);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public void Touch(string URL)
		{
			_CheckProtocol(URL);
			string InternalURL = URL.Replace("file://", "");

			pluginner.FSEntryMetadata newmd = new pluginner.FSEntryMetadata();
			newmd.FullURL = InternalURL;
			newmd.CreationTimeUTC = DateTime.UtcNow;
			newmd.LastWriteTimeUTC = DateTime.UtcNow;
			Touch(newmd);
		}

		public System.IO.Stream GetStream(string url, bool Lock = false)
		{ //запрос потока для файла
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://", "");

			FileAccess fa = (Lock ? FileAccess.ReadWrite : FileAccess.Read);

			return new FileStream(InternalURL, FileMode.Open, fa);
		}

		public pluginner.File GetFile(string url, double Progress)
		{ //чтение файла
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://", "");

			pluginner.File fsf = new pluginner.File(); //fsf=filesystem file
			Progress = 50;
			fsf.Path = "file://" + InternalURL; //this long method have a correction for possibly damages of letters' cases or changes of slashes
			fsf.Metadata = GetMetadata(url);
			fsf.Name = new FileInfo(InternalURL).Name;
			return fsf;
		}

		public byte[] GetFileContent(string url)
		{
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://", "");
			FileStream fistr = new FileStream(InternalURL, FileMode.Open,FileAccess.Read,FileShare.Read);
			BinaryReader bire = new BinaryReader(fistr);
			int Length = 0;
			try
			{ Length = (int)fistr.Length; }
			catch
			{ Length = int.MaxValue; Console.WriteLine("LOCALFS: the file is too long, reading only first " + int.MaxValue + " bytes.\nCall fs.GetFileContent() with length definition."); }
			return bire.ReadBytes(Length);
		}

		public byte[] GetFileContent(string url, Int32 start, Int32 length)
		{
			//код не проверялся!!!
			//this code wasn't debugged!!!
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://", "");
			FileStream fistr = new FileStream(InternalURL, FileMode.Open,FileAccess.Read,FileShare.Read);
			BinaryReader bire = new BinaryReader(fistr);
			byte[] rezultat = new byte[length];
			bire.Read(rezultat, start, length);
			return rezultat;
		}

		public void WriteFile(pluginner.File NewFile, int Progress, byte[] Content)
		{ //запись файла
			_CheckProtocol(NewFile.Path);
			string InternalURL = NewFile.Path.Replace("file://", "");

			try{
				Progress = 10;
				pluginner.File f = NewFile;
				if(!Directory.Exists(InternalURL)) File.WriteAllBytes(InternalURL, Content);
				Progress = 25;
				if (!Directory.Exists(InternalURL)) File.SetAttributes(InternalURL, f.Metadata.Attrubutes);
				Progress = 50;
				File.SetCreationTime(InternalURL, f.Metadata.CreationTimeUTC);
				Progress = 75;
				File.SetLastWriteTime(InternalURL, DateTime.Now);
				Progress = 100;
			}
			catch (Exception ex){
				//System.Windows.Forms.MessageBox.Show(ex.Message,"LocalFS error",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Stop);
				new MsgBox(ex.Message, null, MsgBox.MsgBoxType.Error);
				Console.Write(ex.Message + "\n" + ex.StackTrace + "\n" + "Catched in local fs provider while writing " + InternalURL + "\n");
			}
		}

		public void WriteFileContent(string url, Int32 Start, byte[] Content)
		{
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://", "");

			FileStream fistr = new FileStream(InternalURL, FileMode.OpenOrCreate, FileAccess.ReadWrite);
			BinaryWriter biwr = new BinaryWriter(fistr);
			biwr.Write(Content, Start, Content.Length);
		}

		public void WriteFileContent(string URL, byte[] Content)
		{
			WriteFileContent(URL, 0, Content);
		}

		public void DeleteFile(string url){//удалить файл
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://","");

			File.Delete(InternalURL);
		}

		public void DeleteDirectory(string url, bool TryFirst){//удалить папку
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://","");
			if (TryFirst) {
				if (!CheckForDeletePossiblity(InternalURL)) throw new pluginner.ThisDirCannotBeRemovedException();
			}
			Directory.Delete(InternalURL,true);//рекурсивное удаление
		}

		public void CreateDirectory(string url){//создать каталог
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://", "");

			Directory.CreateDirectory(InternalURL);
		}

		/// <summary>
		/// Check the directory "url", it is may be purged&deleted
		/// </summary>
		/// <param name="url"></param>
		private bool CheckForDeletePossiblity(string url){
			try{
				DirectoryInfo d = new DirectoryInfo(url);
				foreach (FileInfo file in d.GetFiles())
				{
					//перебираю все файлы в каталоге
					string newName = file.FullName + ".fcdeltest";
					string oldName = file.FullName;
					try
					{
						file.MoveTo(newName);
						new FileInfo(newName).MoveTo(oldName);
					}
					catch (Exception nesudba)
					{
#if DEBUG
						Console.WriteLine("Check for deleteability was breaked by " + oldName + ": " + nesudba.Message);
#endif
						return false;
					}
				}

				foreach (DirectoryInfo dir in d.GetDirectories())
				{
					//рекурсивно перебираю все подкаталоги в каталоге (папки хранятся в фейле, фейлы в подкаталогах, подкаталог в каталоге. Марь Иванна, правильно?)
					return CheckForDeletePossiblity(dir.FullName);
				}
				return true;
			}
			catch (Exception ex) { Console.WriteLine("ERROR: CheckForDeletePossiblity failed: " + ex.Message + ex.StackTrace + "\nThe FC's crash was prevented. Please inform the program authors."); return false; }
		}

		public void MoveFile(string source, string destination){
			_CheckProtocol(source);
			string internalSource = source.Replace("file://", "");
			string internalDestination = destination.Replace("file://", "");

			File.Move(internalSource, internalDestination);
		}

		public void MoveDirectory(string source, string destination)
		{
			_CheckProtocol(source);
			string internalSource = source.Replace("file://", "");
			string internalDestination = destination.Replace("file://", "");

			Directory.Move(internalSource, internalDestination);
		}

		public pluginner.FSEntryMetadata GetMetadata(string url)
		{
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://", "");
			pluginner.FSEntryMetadata lego = new pluginner.FSEntryMetadata();
			FileInfo metadatasource = new FileInfo(InternalURL);

			lego.Name = metadatasource.Name;
			lego.FullURL = url;
			try{
			lego.UpperDirectory = "file://" + metadatasource.DirectoryName;
			lego.RootDirectory = "file://" + metadatasource.Directory.Root.FullName;
			lego.Attrubutes = metadatasource.Attributes;
			lego.CreationTimeUTC = metadatasource.CreationTimeUtc;
			lego.IsReadOnly = metadatasource.IsReadOnly;
			lego.LastAccessTimeUTC = metadatasource.LastAccessTimeUtc;
			lego.LastWriteTimeUTC = metadatasource.LastWriteTimeUtc;
			if(!Directory.Exists(InternalURL)) lego.Lenght = metadatasource.Length;
			}catch(Exception ex){Console.WriteLine("WARNING: can't build metadata lego for " + url + ": " + ex.Message + ex.StackTrace);}

			return lego;
		}

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

		/// <summary> Send new feedback data to UI</summary>
		/// <param name="Progress">The new progress value (or -1.79769e+308 if it should stay w/o changes): from 0.0 to 1.0 (or > 1.0 to hide the bar)</param>
		/// <param name="Status">The new status text (or null if it should stay w/o changes)</param>
		private void SetFeedback(double Progress = double.MinValue, string Status = null)
		{
			if (Progress != double.MinValue){
				if (ProgressChanged != null) ProgressChanged(Progress);
			}

			if (Status != null){
				if (StatusChanged != null) StatusChanged(Status);
			}
		}

	}
}

