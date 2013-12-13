/* The File Commander - plugin API
 * Base plugins' interface (common)
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;

namespace pluginner{
    public delegate string TypedEvent<T>(T data);

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
    /// Interface for FCView plugins
    /// </summary>
    [Obsolete]
    public interface IViewerPlugin : IPlugin{
        /// <summary>
        /// The control to be displayed in FCView window
        /// </summary>
        Xwt.Widget DisplayBox { get; }

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
        //System.Windows.Forms.ToolStripMenuItem[] SettingsMenu { get; }
        List<Xwt.MenuItem> SettingsMenu { get; }

        /*/// <summary>
        /// Command-mode call handler
        /// </summary>
        /// <param name="Command">The user's command with parameters</param>
        void CommandCall(string Command);*///to be used when the FC Viewer and FC Editor will be merged into an combined suite with vim-like commands interface
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
}

