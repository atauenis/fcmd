/* The File Commander
 * Модуль доступа к локальным ФС
 * Единственный плагин, не вынесенный во внешнюю dll (из соображений компактности)
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.IO;

namespace fcmd
{
	public class localFileSystem : pluginner.IFSPlugin
	{

		public string Name { get{return "Local filesystem plugin [internal]";} }
		public string Version { get{return "1.0";} }
		public string Author { get{return "A.T.";} }
		public List<pluginner.DirItem> DirectoryContent {get{return DirContent;}} //возврат директории в FC

		List<pluginner.DirItem> DirContent = new List<pluginner.DirItem>();
		string CurDir;

		public string CurrentDirectory {get{return CurDir;} set{CurDir = value; ReadDirectory(value);}}

		private void _CheckProtocol(string url){ //проверка на то, чтобы нечаянно через localfs не попытались зайти в ftp, webdav, реестр и т.п. :-)
			if(!url.StartsWith("file:")) throw new pluginner.PleaseSwitchPluginException();
		}

		public bool IsFilePresent(string URL){//проверить наличие файла
			_CheckProtocol(URL);
			string InternalURL = URL.Replace("file://","");
			if(File.Exists(InternalURL)) return true; //файл е?
			return false; //та ничого нэма! [не забываем, что return xxx прекращает выполнение подпрограммы]
		}

		public bool IsDirPresent(string URL){//проверить наличие папки
			_CheckProtocol(URL);
			string InternalURL = URL.Replace("file://","");
			if(Directory.Exists(InternalURL)) return true; //каталох е?
			return false; //та ничого нэма! [не забываем, что return xxx прекращает выполнение подпрограммы]
		}

		public void ReadDirectory(string url){//прочитать каталог и загнать в DirectoryContent
			_CheckProtocol(url);
			DirContent.Clear();		
			string InternalURL = url.Replace("file://","");

			pluginner.DirItem tmpVar = new pluginner.DirItem();

			string[] files = System.IO.Directory.GetFiles(InternalURL);
			string[] dirs = System.IO.Directory.GetDirectories (InternalURL);

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
                Console.WriteLine("Can't get access to " + InternalURL + "\nThe blocking reason is: " + ex.Message);
                return false;
            }
        }

		public pluginner.File GetFile(string url){ //чтение файла
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://","");

			pluginner.File fsf = new pluginner.File(); //fsf=filesystem file
			fsf.Path = InternalURL;
			fsf.Metadata = new FileInfo(InternalURL);
			fsf.Content = File.ReadAllBytes(InternalURL);
            fsf.Name = new FileInfo(InternalURL).Name;
			return fsf;
		}

		public void WriteFile(pluginner.File NewFile){ //запись файла
            _CheckProtocol(NewFile.Path);
            string InternalURL = NewFile.Path.Replace("file://", "");

            try{
                pluginner.File f = NewFile;
                File.WriteAllBytes(InternalURL, f.Content);
                File.SetAttributes(InternalURL, f.Metadata.Attributes);
                File.SetCreationTime(InternalURL, f.Metadata.CreationTime);
                File.SetLastWriteTime(InternalURL, DateTime.Now);
            }
            catch (Exception ex){
                System.Windows.Forms.MessageBox.Show(ex.Message,"Ошибка",System.Windows.Forms.MessageBoxButtons.OK,System.Windows.Forms.MessageBoxIcon.Stop);
            }
		}

		public void RemoveFile(string url){//удалить файл
			_CheckProtocol(url);
			string InternalURL = url.Replace("file://","");

            File.Delete(InternalURL);
		}

        public void MakeDir(string url){//создать каталог
            _CheckProtocol(url);
            string InternalURL = url.Replace("file://", "");

            Directory.CreateDirectory(InternalURL);
        }

	}
}

