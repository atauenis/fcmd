/* The File Commander - plugin API
 * Filesystem plugins' interface
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;

namespace pluginner
{
	/// <summary>Interface for filesystem & archive plugins.</summary>
	public interface IFSPlugin : IPlugin
	{
		/// <summary>
		/// Gets the list of the directory's subitems.
		/// </summary>
		List<DirItem> DirectoryContent { get; }

		/// <summary>
		/// Gets or sets the current directory.
		/// </summary>
		/// <value>
		/// The current directory URL.
		/// </value>
		string CurrentDirectory { get; set; }

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
		/// <param name="Progress">OBDOLETE, WILL BE REMOVED SOON</param>
		File GetFile(string URL, double Progress);

		/// <summary>
		/// Gets the file's attribbutes
		/// </summary>
		/// <param name="URL"></param>
		FSEntryMetadata GetMetadata(string URL);

		/// <summary>
		/// Get the file's full content
		/// </summary>
		/// <param name="URL">The URL of the file</param>
		byte[] GetFileContent(string URL);

		/// <summary>
		/// Get the file's full content
		/// </summary>
		/// <param name="URL">The URL of the file</param>
		/// <param name="Start">The starting point in the file at which to begin reading</param>
		/// <param name="Length">The number of bytes to read</param>
		byte[] GetFileContent(string URL, Int32 Start, Int32 Length);
		//in future versions, the Int32 will be replaced with a longer type
		//(currently, the temporary system.io call is limited to 4GB files)

		/// <summary>Writes a file.</summary>
		/// <param name='NewFile'>The file</param>
		/// <param name="Content">The file's content</param>
		[Obsolete("Replacing to Touch+WriteFileContent (но ПОКА ЧТО много где используется старый вызов)")]
		void WriteFile(File NewFile, int Progress, byte[] Content);
		//todo:void WriteFileMetadata(FileMetadata md);
		//todo: переписать всё, убрать инт прогресс и ускорить работу путём секционирования файлов

		/// <summary>Writes bytes into a file</summary>
		/// <param name="URL">The URL of the file</param>
		/// <param name="Start">The starting point at which to begin writing</param>
		/// <param name="Content">A byte array containing the data to write</param>
		void WriteFileContent(string URL, Int32 Start, byte[] Content);

		/// <summary>Writes bytes into a file (the file will be fully overwrited)</summary>
		/// <param name="URL">The URL of the file</param>
		/// <param name="Content">A byte array containing the data to write</param>
		void WriteFileContent(string URL, byte[] Content);

		/// <summary>Writes metadata of the specifed filesystem item (it's URL is written in the <paramref name="metadata"/>).</summary>
		/// <param name="metadata">The metadata and URL of the fs item</param>
		void Touch(FSEntryMetadata metadata);

		/// <summary>Creates a new file with default metadata</summary>
		/// <param name="URL">The URL of the file</param>
		void Touch(string URL);

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
		 * каменный цветок, ну а если не выйдет, плагин должен кинуть
		 * сразу же исключение pluginner.ThisDirCannotBeRemovedException .
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

		/// <summary>
		/// Raises when the plugin want to change text in the panel's status bar
		/// </summary>
		event pluginner.TypedEvent<string> StatusChanged;

		/// <summary>
		/// Raises when the plugin want to show a progressbar in the panel status bar; the argument is a number in range of 0...1; if the eventarg is 0, the progress bar will hide
		/// </summary>
		event pluginner.TypedEvent<double> ProgressChanged;

		/// <summary>
		/// Start a program <paramref name="CommandLine"/> in the environment or write some data into the current program's stdin stream.
		/// </summary>
		/// <param name="StdIn">The stdin text or the new program name</param>
		void CLIstdinWriteLine(string StdIn);

		/// <summary>
		/// The plugin raises this event when the STDOUT of a program in CLI sends a data;
		/// </summary>
		event TypedEvent<string> CLIstdoutDataReceived;

		/// <summary>
		/// The plugin raises this event when the STDERR of a program in CLI sends a data;
		/// </summary>
		event TypedEvent<string> CLIstderrDataReceived;

		/// <summary>
		/// The plugin raises this event when the commandbar prompt should be changed
		/// </summary>
		event TypedEvent<string> CLIpromptChanged;
	}

	/// <summary>File info</summary>
	public struct File
	{
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
		/// Returns the file's name
		/// </summary>
		public string Name;
	}

	/// <summary>Common directory item info.</summary>
	public struct DirItem
	{
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
		/// The file's date&time in GMT
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
		//	http://msdn.microsoft.com/library/default.asp?url=/library/en-us/cpgenref/html/cpconerrorraisinghandlingguidelines.asp
		// and
		//	http://msdn.microsoft.com/library/default.asp?url=/library/en-us/dncscol/html/csharp07192001.asp
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
