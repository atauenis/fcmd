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
        /// Background directory lister
        /// </summary>
        [Obsolete("Артефакт из ранних версий на базе winforms, не актуален в связи с вводом Filelistpanel.LoadDir")]
        void DoLs(string URL, pluginner.FileListPanel lp, ref int StatusFeedback)
        {
            //remove as possibly
            Xwt.Application.Invoke(new Action(delegate
            {
                lp.LoadDir(URL);
            }
            ));
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
        void DoCp(pluginner.IFSPlugin SourceFS, pluginner.IFSPlugin DestinationFS, pluginner.File SourceFile, string DestinationURL)
		{
			ReplaceQuestionDialog.ClickedButton Placeholder = ReplaceQuestionDialog.ClickedButton.Cancel;
			DoCp(SourceFS,DestinationFS,SourceFile,DestinationURL,ref Placeholder);
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
        void DoCp(pluginner.IFSPlugin SourceFS, pluginner.IFSPlugin DestinationFS, pluginner.File SourceFile, string DestinationURL, ref ReplaceQuestionDialog.ClickedButton Feedback)
        {
            pluginner.File NewFile = SourceFile;
            NewFile.Path = DestinationURL;

            if (SourceFile.Path == DestinationURL){
                string itself = Locale.GetString("CantCopySelf");
                string toshow = string.Format(Locale.GetString("CantCopy"), SourceFile.Name, itself);
                
                Xwt.Application.Invoke(new Action(delegate { Xwt.MessageDialog.ShowWarning(toshow); }));
                //calling the msgbox in non-main threads causes some UI bugs, thus push this call into main thread
                return;
            }

			if(DestinationFS.FileExists (DestinationURL)){
				ReplaceQuestionDialog rpd = new ReplaceQuestionDialog(DestinationFS.GetFile(DestinationURL,new double()).Name);
				switch(rpd.Run()){
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
                byte[] content = SourceFS.GetFileContent(SourceFile.Path);//todo: разобрать на копирование по частям, дабы не забивать файлы сразу целиком в ОЗУ
                pluginner.FSEntryMetadata md = SourceFS.GetMetadata(SourceFile.Path);
                md.FullURL = NewFile.Path;

                DestinationFS.Touch(md);
                DestinationFS.WriteFileContent(DestinationURL, content);
            }
            catch (pluginner.PleaseSwitchPluginException)
            {
                Xwt.MessageDialog.ShowError("Не совпадают файловые системы. Копирование из ФС в ФС в разработке...типа как.");
                //todo: different-filesystem copying
            }
            catch (Exception ex)
            {
                Xwt.MessageDialog.ShowMessage(string.Format(Locale.GetString("CantCopy"),SourceFile.Name,ex.Message));
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

                    DoCp(fsa, fsb, fsa.GetFile(s1, new double()), s2);
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
                new MsgBox(err.Message, null, MsgBox.MsgBoxType.Error);
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
                new MsgBox(url, string.Format(Locale.GetString("DirCantBeRemoved"), url), MsgBox.MsgBoxType.Warning);
            }
            catch (Exception err)
            {
                new MsgBox(err.Message, null, MsgBox.MsgBoxType.Error);
            }
        }

    }
}
