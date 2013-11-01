/* The File Commander - API для плагинов
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
		/// Gets the list of the directory's subitems.
		/// </summary>
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
		bool FileExists(string URL);

		/// <summary>
		/// Determines whether at the specified URL exists a directory
		/// </summary>
		/// <returns>
		/// <c>true</c> if the directory really present; else, retruns <c>false</c>.
		/// </returns>
		/// <param name='URL'>
		/// The directory location (URL)
		/// </param>
		bool DirectoryExists(string URL);

        /// <summary>
        /// Tries to read the file or directory <paramref name="URL"/> and determines whether it can be read without access errors.
        /// </summary>
        /// <param name="URL">The uniform resource locator for the requested file</param>
        /// <returns></returns>
        bool CanBeRead(string URL);

		/// <summary>
		/// Reads the file and returns both file content and it's metadata.
		/// </summary>
		/// <returns>
		/// The file.
		/// </returns>
		/// <param name='URL'>
		/// URL of the file.
		/// </param>
		File GetFile(string URL, int Progress);

        /// <summary>
        /// Gets the file's attribbutes
        /// </summary>
        /// <param name="URL"></param>
        FSEntryMetadata GetMetadata(string URL);

		/// <summary>Writes the file.</summary>
		/// <param name='NewFile'>New file's content.</param>
		void WriteFile(File NewFile, int Progress);
        //todo:void WriteFileMetadata(FileMetadata md);
        
		/// <summary>
		/// Delete the file <paramref name="URL"/>.
		/// </summary>
		/// <param name='URL'>
		/// URL of the file.
		/// </param>
		void DeleteFile(string URL);
        
        /// <summary>Move the file</summary>
        void MoveFile(string oldURL, string newURL);

	    /// <summary>
		/// Delete the directory <paramref name="URL"/>.
		/// </summary>
		/// <param name='URL'>
		/// URL of the dir.
		/// </param>
        /// <param name="TrySafe">
        /// When set to true, the plugin should firstly check the possibility of deleting the directory. In case of impossibility it should throw ThisDirCannotBeRemovedException
        /// </param>       
		void DeleteDirectory(string URL, bool TrySafe);
        /* Если булево значение = true, плагин должен сначала проверить, выйдет ли
         * каменный цветок, если он вдруг окажется не уверен в этом, должен кинуть
         * сразу исключение pluginner.ThisDirCannotBeRemovedException .
         */

        /// <summary>
        /// Creates a new directory
        /// </summary>
        /// <param name="URL"></param>
        void CreateDirectory(string URL);

        /// <summary>
        /// Move or rename the directory
        /// </summary>
        void MoveDirectory(string OldURL, string NewURL);

        /// <summary>
        /// Separator of the directories in path
        /// </summary>
        string DirSeparator { get; }
	}
	//todo: IEditorPlugin, IUIPlugin (плагины к интерфейсу File Commander)

    /// <summary>
    /// Filesystem entry's metadata (like system.io.fileinfo/directoryinfo)
    /// </summary>
    public class FSEntryMetadata
    {
        /// <summary>The file's short name with extension</summary>
        public string Name;
        /// <summary>The file's containing directory</summary>
        public string UpperDirectory;
        /// <summary>The file's full path</summary>
        public string FullURL;

        /// <summary>The file's attribbutes</summary>
        public System.IO.FileAttributes Attrubutes;
        /// <summary>The file's GMT-time of creation</summary>
        public DateTime CreationTimeUTC;
        /// <summary>The file's GMT-time of last modification</summary>
        public DateTime LastWriteTimeUTC;
        /// <summary>The file's GTM-time of last reading</summary>
        public DateTime LastAccessTimeUTC;

        /// <summary>The file's size (in bytes)</summary>
        public long Lenght;

        /// <summary>Is the file configured for read only</summary>
        public bool IsReadOnly;

        /// <summary>Returns uniform resource locator of the file</summary>
        public override string ToString()
        { return this.FullURL; }
    }

	/// <summary>
	/// File info
	/// </summary>
	public struct File{
		/// <summary>
		/// The file's or the directory's metadata (date, size, etc).
		/// </summary>
		public FSEntryMetadata Metadata;

        /// <summary>
        /// The file's MIME type.
        /// NOTE: if the file is a directory, it's type will be "x-fcmd/directory"
        /// </summary>
        public string MIMEType;

		/// <summary>
		/// The file's full path.
		/// </summary>
		public string Path;

		/// <summary>
		/// The file's content.
        /// NOTE: if the 'file' is a directory, it's content will be null
		/// </summary>
		public byte[] Content;

        /// <summary>
        /// Returns the file's name
        /// </summary>
        public string Name;
	}

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

    /// <summary>
    /// Interface for FCView plugins
    /// </summary>
    public interface IViewerPlugin : IPlugin{
        /// <summary>
        /// The control to be displayed in FCView window
        /// </summary>
        System.Windows.Forms.Control DisplayBox();

        /// <summary>
        /// Loads & shows a file into the File Commander Viewer
        /// </summary>
        /// <param name="url"></param>
		void LoadFile(string url, pluginner.IFSPlugin fsplugin);

		/// <summary>
		/// Gets a value indicating whether this plugin can copy content into system clipboard.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance can use clipboard; otherwise, <c>false</c>.
		/// </value>
		bool CanCopy{get;}

		/// <summary>
		/// Copy selected content into the system clipboard.
		/// </summary>
		void Copy();

		/// <summary>
		/// Gets a value indicating whether this plugin can select all content.
		/// </summary>
		/// <value>
		/// <c>true</c> if this instance can select all content; otherwise, <c>false</c>.
		/// </value>
		bool CanSelectAll{get;}

		/// <summary>
		/// Selects all content inside this plugin.
		/// </summary>
		void SelectAll();

        /// <summary>
        /// Shows plugin's content search dialog box
        /// </summary>
        void Search();

        /// <summary>
        /// Search the document with last parameters next
        /// </summary>
        void SearchNext();

		/// <summary>
		/// Gets a value indicating whether this plugin can print content.
		/// </summary>
		/// <value>
		/// <c>true</c> if this plugin can print; otherwise, <c>false</c>.
		/// </value>
		bool CanPrint{get;}

		/// <summary>
		/// Print content in this instance.
		/// </summary>
		void Print();

		/// <summary>
		/// Shows print settngs dialog window.
		/// </summary>
		void PrintSettings();

        /// <summary>
        /// An array of ToolStripMenuItems for FCView's menu of plugin's settings
        /// </summary>
        System.Windows.Forms.ToolStripMenuItem[] SettingsMenu { get; }
    }

    #region PleaseSwitchPluginException
    /// <summary>
	/// This exception fires when the plugin module needs to be changed to an other plugin module.
	/// For example, when a filesystem plugin tried to be used with uncompatible filesystem or a image viewer plugin tried to show a text file.
    /// The File Commander should catch this exception and find a new plugin (see pluginfinder.cs file)
	/// </summary>
	[System.Serializable]
	public class PleaseSwitchPluginException : System.Exception
	{
		/// <summary>
		/// Informs the File Commander that the plugin cannot be used now and must be changed
		/// </summary>
		public PleaseSwitchPluginException ()
		{
		}
		
		/// <summary>
		/// Informs the File Commander that the plugin cannot be used now and must be changed
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception reason. </param>
		public PleaseSwitchPluginException (string message) : base (message)
		{
		}
		
		/// <summary>
		/// Informs the File Commander that the plugin cannot be used now and must be changed. The reason should be showed in the <see cref="inner"/>.
		/// </summary>
		/// <param name="message">A <see cref="T:System.String"/> that describes the exception. </param>
		/// <param name="inner">The exception that is the cause of the current exception. </param>
		public PleaseSwitchPluginException (string message, System.Exception inner) : base (message, inner)
		{
		}
		
		/// <summary>
		/// Initializes a new instance of the <see cref="T:PleaseSwitchPluginException"/> class
		/// </summary>
		/// <param name="context">The contextual information about the source or destination.</param>
		/// <param name="info">The object that holds the serialized object data.</param>
		protected PleaseSwitchPluginException (System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base (info, context)
		{
        }
    #endregion
    }

    /* Исключение выскакивает в случае установления невозможности удаления каталога
     * (это проверяется ДО реального удаления путём переименования или чтения прав!).
     * Плагин должен кидать это исключение только после отмены всех действий,
     * использовавшихся для проверки каталога на удаляемость.
     */
    #region ThisDirCannotBeRemovedException
    /// <summary>
    /// This exception fires when the FS plugin is unable to remove the requested directory.
    /// The File Commander should catch this exception and abort the directory removal procedure.
    /// The plugin should not throw this exception until all "rule checking" changes was undoned.
    /// </summary>
    [global::System.Serializable]
    public class ThisDirCannotBeRemovedException : Exception
    {
        //
        // For guidelines regarding the creation of new exception types, see
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
        // and
        //    http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
        //

        public ThisDirCannotBeRemovedException() { }
        public ThisDirCannotBeRemovedException(string message) : base(message) { }
        public ThisDirCannotBeRemovedException(string message, Exception inner) : base(message, inner) { }
        protected ThisDirCannotBeRemovedException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
    #endregion
}

