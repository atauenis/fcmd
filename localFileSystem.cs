/* The File Commander
 * Модуль доступа к локальным ФС
 * В будущем должен быть выделен в отдельный плагин (хотя, а надо ли?)
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.IO;
using pluginner;

namespace fcmd
{
	public class localFileSystem : pluginner.IFSPlugin
	{

		public string Name { get{return "Local filesystem plugin [internal]";} }
		public string Version { get{return "1.0";} }
		public string Author { get{return "A.T.";} }
		public List<DirItem> DirectoryContent {get{return DirContent;}} //возврат директории в FC

		List<pluginner.DirItem> DirContent = new List<DirItem>();


		public void ReadDirectory(string url){//прочитать каталог и загнать в DirectoryContent
			DirContent.Clear();
			string InternalURL = url.Replace("file://","");

			pluginner.DirItem tmpVar = new pluginner.DirItem();

			string[] files = System.IO.Directory.GetFiles(InternalURL);
			string[] dirs = System.IO.Directory.GetDirectories (InternalURL);

			foreach(string curDir in dirs){
				//перебираю каталоги
				DirectoryInfo di = new DirectoryInfo(curDir);
				tmpVar.IsDirectory = true;
				tmpVar.Path = curDir;
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
				tmpVar.Path = curFile;
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

		public string ReadFile(string url){ //чтение файла
			return "work in progress :-)";
			//todo
		}

		public int WriteFile(string url, string content){//запись файла
			string InternalURL = url.Replace("file://","");
			return 0;
			//todo
		}
	}
}

