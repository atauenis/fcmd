/* The File Commander - API для плагинов
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;

namespace pluginner{
	/// <summary>
	/// Default plugin interface.
	/// </summary>
	public interface IPlugin{
		/// <summary>
		/// Gives the plugin's name
		/// </summary>
		string Name { get; }

		/// <summary>
		/// Gives the plugin's version.
		/// </summary>
		string Version { get; }

		/// <summary>
		/// Gives the plugin's author.
		/// </summary>
		string Author { get; }
	}

	/// <summary>
	/// Interface for filesystem & archive plugins.
	/// </summary>
	public interface IFSPlugin : IPlugin{
		/// <summary>
		/// Gets or sets the content of the directory.
		/// </summary>
		/// <value>
		/// The content of the directory.
		/// </value>
		List<DirItem> DirectoryContent {get;}

		/// <summary>
		/// Gets or sets the current directory.
		/// </summary>
		/// <value>
		/// The current directory URL.
		/// </value>
		string CurrentDirectory {get; set;}

		/// <summary>
		/// Determines whether at the specified URL exists a file
		/// </summary>
		/// <returns>
		/// <c>true</c> if the file really present; else, retruns <c>false</c>.
		/// </returns>
		/// <param name='URL'>
		/// The file location (URL)
		/// </param>
		bool IsFilePresent(string URL);

		/// <summary>
		/// Determines whether at the specified URL exists a directory
		/// </summary>
		/// <returns>
		/// <c>true</c> if the directory really present; else, retruns <c>false</c>.
		/// </returns>
		/// <param name='URL'>
		/// The directory location (URL)
		/// </param>
		bool IsDirPresent(string URL);


//		/// <summary>
//		/// Reads a file.
//		/// </summary>
//		/// <returns>
//		/// The file content.
//		/// </returns>
//		/// <param name='url'>
//		/// URL of the file (with plugin prefix)
//		/// </param>
//		string ReadFile(string url);
//
//		//todo: byte[] ReadFileHex(string url);
//
//		/// <summary>
//		/// Writes the file.
//		/// </summary>
//		/// <returns>
//		/// The return code (0=ok, 1=no permission, TODO)
//		/// </returns>
//		/// <param name='url'>
//		/// The file's URL
//		/// </param>
//		/// <param name='content'>
//		/// The file's new content.
//		/// </param>
//		int WriteFile(string url, string content);
//
//		//todo:работа с аттрибутами файлов и правами доступа
	}
	//todo: IViewerPlugin, IEditorPlugin, IUIPlugin (плагины к интерфейсу File Commander)

	/// <summary>
	/// Directory item info (structure).
	/// </summary>
	public struct DirItem{
		/// <summary>
		/// The path of the file.
		/// </summary>
		public string Path;

		/// <summary>
		/// The text to show in the list.
		/// </summary>
		public string TextToShow;

		/// <summary>
		/// The file's size.
		/// </summary>
		public long Size;

		/// <summary>
		/// The file's date.
		/// </summary>
		public DateTime Date;

		/// <summary>
		/// The file's accessrules.
		/// </summary>
		public string Rules;

		/// <summary>
		/// Is the item a directory? 1=dir, 0=file
		/// </summary>
		public bool IsDirectory;

		/// <summary>
		/// Is the file/directory hidden. 0=maybe showed, 1=dont show
		/// </summary>
		public bool Hidden;
	}
}

