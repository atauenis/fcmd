/* The File Commander backend   Ядро File Commander
 * Local filesystem adapter     Модуль доступа к локальным ФС
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace fcmd.base_plugins.fs
{
	public class localFileSystem : pluginner.IFSPlugin
	{
        /* ЗАМЕТКА РАЗРАБОТЧИКУ             DEVELOPER NOTES
         * В данном файле содержится код    This file contanis the local filesystem
         * плагина доступа к локальным ФС.  adapter for the File Commander kernel.
         * Данный код используется как в    This code should be cross-platform and
         * версии для Win (.Net), так и     should be tested on both .NET Win. Forms
         * в версии для *nix/MacOS (Mono)   and Linux/BSD (Mono/GTK#) envirroments.
         */
        public string Name { get { return new Localizator().GetString("LocalFSVer"); } }
		public string Version { get{return "1.0";} }
		public string Author { get{return "A.T.";} }
		public List<pluginner.DirItem> DirectoryContent {get{return DirContent;}} //возврат директории в FC
        public event pluginner.TypedEvent<String> StatusChanged;
        public event pluginner.TypedEvent<double> ProgressChanged;

		List<pluginner.DirItem> DirContent = new List<pluginner.DirItem>();
		string CurDir;

		public string CurrentDirectory {get{return CurDir;} set{CurDir = value; ReadDirectory(value);}}

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
            if (StatusChanged != null) StatusChanged(string.Format(new Localizator().GetString("DoingListdir"), "", InternalURL));

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
                DirContent.Add(tmpVar);
            }

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

				DirContent.Add(tmpVar);
                Progress += FileWeight;
                if (ProgressChanged != null) { ProgressChanged(Progress); }
                Xwt.Application.MainLoop.DispatchPendingEvents();
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

				DirContent.Add(tmpVar);
                Progress += FileWeight;
                if (ProgressChanged != null && Progress <= 1) { ProgressChanged(Progress); }
                Xwt.Application.MainLoop.DispatchPendingEvents();
			}
            if (ProgressChanged != null) { ProgressChanged(2); }
            if (StatusChanged != null) { StatusChanged(""); };
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

		public pluginner.File GetFile(string url, double Progress){ //чтение файла
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://","");

			pluginner.File fsf = new pluginner.File(); //fsf=filesystem file
            Progress = 50;
			fsf.Path = InternalURL;
			fsf.Metadata = GetMetadata(url);
            try { fsf.Content = File.ReadAllBytes(InternalURL); } catch {fsf.Content = null;}
            fsf.Name = new FileInfo(InternalURL).Name;
			return fsf;
		}

        public void WriteFile(pluginner.File NewFile, int Progress)
        { //запись файла
            _CheckProtocol(NewFile.Path);
            string InternalURL = NewFile.Path.Replace("file://", "");

            try{
                Progress = 10;
                pluginner.File f = NewFile;
                if(!Directory.Exists(InternalURL)) File.WriteAllBytes(InternalURL, f.Content);
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
                Console.Write(ex.Message + "\n" + ex.StackTrace + "\n" + "Catched in local fs provider while loading " + InternalURL + "\n");
            }
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

            lego.Attrubutes = metadatasource.Attributes;
            lego.CreationTimeUTC = metadatasource.CreationTimeUtc;
            lego.FullURL = url;
            lego.IsReadOnly = metadatasource.IsReadOnly;
            lego.LastAccessTimeUTC = metadatasource.LastAccessTimeUtc;
            lego.LastWriteTimeUTC = metadatasource.LastWriteTimeUtc;
            if(!Directory.Exists(InternalURL)) lego.Lenght = metadatasource.Length;
            lego.Name = metadatasource.Name;
            lego.UpperDirectory = metadatasource.DirectoryName;

            return lego;
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

