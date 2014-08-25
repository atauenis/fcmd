/* The File Commander - plugin API
 * Filesystem plugins' interface
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using Xwt.Drawing;

namespace pluginner
{
	/// <summary>Interface for filesystem & archive plugins.</summary>
	public interface IFSPlugin : IPlugin
	{
		/// <summary>
		/// Gets list of the directory's items.
		/// </summary>
		IEnumerable<DirItem> DirectoryContent { get; }

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
		/// Gets the file's attribbutes
		/// </summary>
		/// <param name="URL">The file's URL</param>
		FSEntryMetadata GetMetadata(string URL);

		/// <summary>
		/// Get the file's full content
		/// </summary>
		/// <param name="URL">The URL of the file</param>
		byte[] GetFileContent(string URL);

		/// <summary>
		/// Gets a Stream that can be used to read and write the file
		/// </summary>
		/// <param name="URL">The URL of the file</param>
		/// <param name="Write">Due to limitations on some filesystems, the usage mode (read-only or write-only) must be set before any file operations</param>
		/// <returns>The .NET Framework I/O stream</returns>
		Stream GetFileStream(string URL, bool Write = false);

		/// <summary>Writes bytes into a file</summary>
		/// <param name="URL">The URL of the file</param>
		/// <param name="Start">The starting point at which to begin writing</param>
		/// <param name="Content">A byte array containing the data to write</param>
		void WriteFileContent   (string URL, Int32 Start, byte[] Content);
		//possible overflow; need get around ^^^^^^^^^^^

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
		 //todo: переписать, выкинуть исключение (A.T., забудь On Error Resume Next как старый сон)

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
		event TypedEvent<string> StatusChanged;

		/// <summary>
		/// Raises when the plugin want to show a progressbar in the panel status bar; the argument is a number in range of 0...1; if the eventarg is 0, the progress bar will hide
		/// </summary>
		event TypedEvent<double> ProgressChanged;

		/// <summary>
		/// Start a external process in the Command Line or write some data into the current program's STDIN stream.
		/// </summary>
		/// <param name="StdIn">The STDIN text or the new program name</param>
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

	/// <summary>Common directory item info.</summary>
	public struct DirItem
	{
		/* TODO: pluginner.DirItem sometimes duplicates pluginner.File, it's need to do something to DRY.
		 * The one of ways is to remove most of the Size,Date,Rules(Permissions),Mimetype and other obsolete fields
		 * and replace with a one FSEntryMetadata field.
		 */
		/// <summary>
		/// The uniform resource locator of the file.
		/// </summary>
		public string URL;

		/// <summary>
		/// The text to show in the list.
		/// </summary>
		public string TextToShow;

		/// <summary>
		/// The file's size.
		/// </summary>
		public long Size;

		/// <summary>
		/// The file's date & time in GMT
		/// </summary>
		public DateTime Date;

		/// <summary>
		/// The file's access rights.
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

		/// <summary>
		/// The file's MIME type.
		/// </summary>
		public string MIMEType;
		/* Allowed MIME types:
		 * any registered or wide-used MIME types
		 * application/octet-stream: a unknown file
		 * x-fcmd/directory: a directory
		 * x-fcmd/up: ".." link
		 * x-fcmd/hardlink: a hard link
		 * x-fcmd/link: link to a file or directory
		 * x-fcmd-win32-shortcut-XXX/YYY: a shortcut to file with XXX/YYY type
		 */

		 /// <summary>
		 /// The file's small icon
		 /// </summary>
		 public Image IconSmall;

		 /// <summary>
		 /// The file's big icon
		 /// </summary>
		 public Image IconBig;
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
		/// <summary>The filesystem's root directory</summary>
		public string RootDirectory;
		/// <summary>The file's full path</summary>
		public string FullURL;

		/// <summary>The file's attribbutes</summary>
		public FileAttributes Attrubutes;
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

		//todo: implement Permissions enum and property (to work samely on *nix, win32, ftp, and permission-less systems)

		/// <summary>Returns uniform resource locator of the file</summary>
		public override string ToString()
		{ return FullURL; }
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
	[Serializable]
	public class ThisDirCannotBeRemovedException : Exception
	{
		public ThisDirCannotBeRemovedException() { }
		public ThisDirCannotBeRemovedException(string message) : base(message) { }
		public ThisDirCannotBeRemovedException(string message, Exception inner) : base(message, inner) { }
		protected ThisDirCannotBeRemovedException(
		  SerializationInfo info,
		  StreamingContext context)
			: base(info, context) { }
	}
	#endregion

}
