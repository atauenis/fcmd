/* The File Commander main window
 * Background workers
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using pluginner;
using pluginner.Toolkit;

namespace fcmd
{
	partial class MainWindow
	{
		/* ЗАМЕТКА РАЗРАБОТЧИКУ
		 * 
		 * В данном файле размещаются функции для работы с файлами и каталогами.
		 * Данные функции запускаются в отдельных от UI потоках из кода
		 * файла MainWindow-Actions.cs . Всякая функция должна иметь
		 * префикс Do, означающий её чисто утилитарную принадлежность.
		 * 
		 * Вызовы пользовательского интерфейса (XWT) должны производиться через
		 * вызывалку Xwt.Application.Invoke(new Action(delegate { КОД ПОТОКА ИНТЕРФЕЙСА }));
		 * в противном случае возможны глюки (вылеты WPF, зависания и лаги GTK).
		 */

		/// <summary>
		/// Background file copier
		/// </summary>
		/// <param name='SourceFS'>
		/// Source FS.
		/// </param>
		/// <param name='DestinationFS'>
		/// Destination FS.
		/// </param>
		/// <param name='SourceFile'>
		/// Source file.
		/// </param>
		/// <param name='DestinationURL'>
		/// Destination URL.
		/// </param>
		/// <param name="AC">
		/// Asynchronus copier
		/// </param>
		[Obsolete]
		void DoCp(pluginner.IFSPlugin SourceFS, pluginner.IFSPlugin DestinationFS, pluginner.File SourceFile, string DestinationURL, AsyncCopy AC)
		{
			ReplaceQuestionDialog.ClickedButton Placeholder = ReplaceQuestionDialog.ClickedButton.Cancel;
			DoCp(
				SourceFS,
				DestinationFS,
				SourceFile,
				DestinationURL,
				ref Placeholder,
				AC
			);
		}

		/// <summary>
		/// Background file copier
		/// </summary>
		/// <param name='SourceFS'>
		/// Source FS.
		/// </param>
		/// <param name='DestinationFS'>
		/// Destination FS.
		/// </param>
		/// <param name='SourceFile'>
		/// Source file.
		/// </param>
		/// <param name='DestinationURL'>
		/// Destination URL.
		/// </param>
		/// <param name='SkipAll'>
		/// The referenced variable will be set to TRUE if user chooses "Skip all"
		/// </param>
		/// <param name='ReplaceAll'>
		/// The referenced variable will be set to TRUE if user chooses "Replace all"
		/// </param>
		void DoCp(pluginner.IFSPlugin SourceFS, pluginner.IFSPlugin DestinationFS, pluginner.File SourceFile, string DestinationURL, ref ReplaceQuestionDialog.ClickedButton Feedback, AsyncCopy AC)
		{
			pluginner.File NewFile = SourceFile;
			NewFile.Path = DestinationURL;

			if (SourceFile.Path == DestinationURL){
				string itself = Localizator.GetString("CantCopySelf");
				string toshow = string.Format(Localizator.GetString("CantCopy"), SourceFile.Name, itself);
				
				Xwt.Application.Invoke(delegate { Xwt.MessageDialog.ShowWarning(toshow); });
				//calling the msgbox in non-main threads causes some UI bugs, so push this call into main thread
				return;
			}

			if(DestinationFS.FileExists (DestinationURL)){

                ReplaceQuestionDialog rpd = null;
                bool ready = false;

                Xwt.Application.Invoke(
                    delegate
                    {
	                    rpd = new ReplaceQuestionDialog(DestinationFS.GetFile(DestinationURL, new double()).Name);
	                    ready = true;
                    }
                );
                do { } while (!ready);
                ready = false;
				
                var ClickedButton = ReplaceQuestionDialog.ClickedButton.Skip;
				Xwt.Application.Invoke(
					delegate
					{
						ClickedButton = rpd.Run();
						ready = true;
					}
				);

				do {} while (!ready);

				switch(ClickedButton){
				case ReplaceQuestionDialog.ClickedButton.Replace:
					//continue execution
					Feedback = rpd.ChoosedButton;
					break;
				case ReplaceQuestionDialog.ClickedButton.ReplaceAll:
					//continue execution
					Feedback = rpd.ChoosedButton;
					break;
				case ReplaceQuestionDialog.ClickedButton.ReplaceOld:
					Feedback = rpd.ChoosedButton;
					if(SourceFS.GetMetadata(SourceFile.Path).LastWriteTimeUTC < DestinationFS.GetFile(DestinationURL,new double()).Metadata.LastWriteTimeUTC)
					{/*continue execution*/}
					else
					{return;}
					break;
				case ReplaceQuestionDialog.ClickedButton.Skip:
					Feedback = rpd.ChoosedButton;
					return;
				case ReplaceQuestionDialog.ClickedButton.SkipAll:
					Feedback = rpd.ChoosedButton;
					return;
				}
			}

			try
			{
				pluginner.FSEntryMetadata md = SourceFS.GetMetadata(SourceFile.Path);
				md.FullURL = NewFile.Path;

				System.IO.Stream SrcStream = SourceFS.GetStream(SourceFile.Path);
				DestinationFS.Touch(md);
				System.IO.Stream DestStream = DestinationFS.GetStream(DestinationURL,true);

				if(AC == null) AC = new AsyncCopy();
				bool CpComplete = false;

				AC.OnComplete+=(rezultat)=>{ CpComplete = true; };

				//warning: due to some GTK# bugs, buffer sizes lesser than 128KB may cause
				//an StackOverflowException at UI update code
				AC.CopyFile(SrcStream, DestStream, 131072); //buffer is 1/8 megabyte

				do{ /*nothing*/	}
				while(!CpComplete); //don't stop this thread until the copy is finished
				return;
			}
			catch (Exception ex)
			{
				if (ex.GetType() != typeof(System.Threading.ThreadAbortException)) { 
					Utilities.ShowMessage(string.Format(Localizator.GetString("CantCopy"),SourceFile.Name,ex.Message));
					Console.WriteLine("Cannot copy because of {0}({1}) at \n{2}.", ex.GetType(), ex.Message, ex.StackTrace);
				}
			}
		}

		/// <summary>
		/// Copy the entrie directory
		/// </summary>
		private void DoCpDir(string source, string destination, pluginner.IFSPlugin fsa, pluginner.IFSPlugin fsb)
		{
			if (!fsb.DirectoryExists(destination)) { fsb.CreateDirectory(destination); }
			fsb.CurrentDirectory = destination;

			foreach (DirItem di in fsa.DirectoryContent)
			{
				if (di.TextToShow == "..")
				{ /* don't touch the link to the parent directory */}
				else if (!di.IsDirectory)
				{
					//it is file
					string s1 = di.Path; //source url
					FSEntryMetadata md1 = fsa.GetMetadata(s1);
					string s2 = destination + fsb.DirSeparator + md1.Name; //destination url

					DoCp(fsa, fsb, fsa.GetFile(s1, new double()), s2, new AsyncCopy());
				}
				else if (di.IsDirectory)
				{
					//it is subdirectory
					DoCpDir(di.Path, destination + fsb.DirSeparator + di.TextToShow, fsa,fsb);
				}
			}
		}

		/// <summary>
		/// Background file remove
		/// </summary>
		/// <param name="url">url of the file</param>
		/// <param name="fs">filesystem of the file</param>
		void DoRmFile(string url, pluginner.IFSPlugin fs)
		{
			try
			{
				fs.DeleteFile(url);
			}
			catch (Exception err)
			{
				Utilities.ShowError(err.Message, null);
			}
		}

		/// <summary>
		/// Background directory remove
		/// </summary>
		/// <param name="url">url of the file</param>
		/// <param name="fs">filesystem of the file</param>
		void DoRmDir(string url, pluginner.IFSPlugin fs)
		{
			try
			{
				fs.DeleteDirectory(url, true);
			}
			catch (pluginner.ThisDirCannotBeRemovedException)
			{
				new MsgBox(url, string.Format(Localizator.GetString("DirCantBeRemoved"), url), MsgBox.MsgBoxType.Warning);
			}
			catch (Exception err)
			{
				new MsgBox(err.Message, null, MsgBox.MsgBoxType.Error);
			}
		}

	}
}
