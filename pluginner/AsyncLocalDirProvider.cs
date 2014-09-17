/* The File Commander base plugins - Local filesystem adapter
 * The main part of the LocalFS FS plugin
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (С) 2014, Zhigunov Andrew (breakneck11@gmail.com)
 * Contributors should place own signs here.
 */
using pluginner.Toolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace pluginner
{
	public class AsyncLocalDirProvider : IAsyncLoadingCollection < DirItem >, IDisposable {

		private List<DirItem> loaded_content = new List<DirItem>();

		private Task loading = null;

		private CancellationTokenSource cancel_maker = new CancellationTokenSource();

		private const int milliseconds_to_wait = 100;

		private bool _isStillLoading;

		protected AsyncLocalDirProvider(string path) {
			Path = path;
			IsStillLoading = true;
			loading = Task.Factory.StartNew(Load, cancel_maker.Token);
		}

		/// <summary>
		/// Create async directory loader from path to the directory
		/// </summary>
		/// <param name="path">Path to the directory</param>
		/// <returns>Object providing async access to the directory</returns>
		public static AsyncLocalDirProvider CreateFromDirectory(string path) { return new AsyncLocalDirProvider(path); }

		/// <summary>
		/// Wait until the element at the given possition will be accessible or load will be finished
		/// </summary>
		/// <param name="position">Position of the requested element</param>
		private void WaitUntilElement(int possition) {
			while (possition >= loaded_content.Count && IsStillLoading) {
				Thread.Sleep(milliseconds_to_wait);
			}
		}

		private void Load() {
			DirectoryInfo curdir = new DirectoryInfo(Path);
			if (curdir.Parent != null) {
				loaded_content.Add(new DirItem() {
					URL = "file://" + curdir.Parent.FullName,
					TextToShow = "..",
					MIMEType = "x-fcmd/up",
					IconSmall = Utilities.GetIconForMIME("x-fcmd/up")
				});
			}
			uint counter = 0;
			const uint every_100 = ~(((~(uint)0) >> 7) << 7); // == 128 ~= 100
			foreach (string dir_name in Directory.EnumerateDirectories(Path)) {
				DirectoryInfo dir_info = new DirectoryInfo(dir_name);
				if ((++counter & every_100) == 0 && cancel_maker.Token.IsCancellationRequested)
				{
					IsStillLoading = false;
					cancel_maker.Token.ThrowIfCancellationRequested();
				}
				DirItem next = new DirItem()
				{
					IsDirectory = true,
					URL = "file://" + dir_name,
					TextToShow = dir_info.Name,
					Date = dir_info.CreationTime,
					Hidden = dir_info.Name.StartsWith("."),
					MIMEType = "x-fcmd/directory",
					IconSmall = Utilities.GetIconForMIME("x-fcmd/directory")
				};
				//Must be added one by one, not with AddRange, as this can interfere with Count property.
				loaded_content.Add(next);
			}
			foreach (string file_name in Directory.EnumerateFiles(Path)) {
				FileInfo file_inf = new FileInfo(file_name);
				string mime_value = file_name.LastIndexOf('.') > 0 ? Utilities.GetContentType(file_name.Substring(file_name.LastIndexOf('.') + 1)) : "application/octet-stream";
				if ((++counter & every_100) == 0 && cancel_maker.Token.IsCancellationRequested)
				{
					IsStillLoading = false;
					cancel_maker.Token.ThrowIfCancellationRequested();
				}
				DirItem next = new DirItem()
				{
					IsDirectory = false,
					URL = "file://" + file_name,
					TextToShow = file_inf.Name,
					Date = file_inf.LastWriteTime,
					Size = file_inf.Length,
					Hidden = file_inf.Name.StartsWith("."),
					MIMEType = mime_value,
					IconSmall = Utilities.GetIconForMIME(mime_value)
				};
				loaded_content.Add(next);
			}
			IsStillLoading = false;
		}

		/// <summary>
		/// Path to the local operating directory
		/// </summary>
		public string Path { private set; get; }

		/// <summary>
		/// Content of the operating directory
		/// </summary>
		public IEnumerable<DirItem> Content {
			get {
				for (int i = 0; i < loaded_content.Count || IsStillLoading; ++i) {
					WaitUntilElement(i);
					if (loaded_content.Count > i) {
						yield return loaded_content[i];
					}
				}
			}
		}

		/// <summary>
		/// Get from the directory item with given index
		/// </summary>
		/// <param name="index">Index of the requested item</param>
		/// <returns>Element in the given position</returns>
		public DirItem GetByIndex(int index) {
			if (index < 0) {
				throw new ArgumentOutOfRangeException("Index in the array must be non-negative.");
			}
			WaitUntilElement(index);
			if (index < loaded_content.Count) {
				return loaded_content[index];
			}
			throw new ArgumentOutOfRangeException("Given index overpasses boundary of the array.");
		}

		/// <summary>
		/// Is risen when loading is finished
		/// </summary>
		public event EventHandler LoadingFinished;

		/// <summary>
		/// Show if loading is complete
		/// </summary>
		public bool IsStillLoading {
			protected set {
				if (!_isStillLoading && value) {
					_isStillLoading = value;
					var handler = LoadingFinished;
					if (handler != null) {
						handler(this, EventArgs.Empty);
					}
				}
				_isStillLoading = value;
			}
			get { return _isStillLoading; }
		}

		/// <summary>
		/// Stop loading
		/// </summary>
		public void Stop() {
			cancel_maker.Cancel();
		}

		/// <summary>
		/// Close and free some internal resources
		/// </summary>
		public void Dispose() {
			if (IsStillLoading) {
				Stop();
			}
			loaded_content.Clear();
			cancel_maker.Dispose();
		}

		public override string ToString() {
			return Path;
		}
	}
}
