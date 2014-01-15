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
 * Данные функции запускаются в отдельных от UI потоках и вызваются функциями
 * из файлов frmMain.cs и frmMain-CLI.cs . Всякая функция должна иметь
 * префикс Do, означающий её чисто утилитарную принадлежность, без UI.
 * Крайне нежелательно обращение к элементам управления формы, поскольку
 * это требует всяких согласующих операций и прочих делегатов, что ухудшает
 * читаемость кода.
 * Также нежелательно обращение к System.Windows.Forms, поскольку код данного
 * файла должен быть кросс-платформенным.
 */

        /// <summary>
        /// Background directory lister
        /// </summary>
        void DoLs(string URL, pluginner.FileListPanel lp, ref int StatusFeedback)
        {
            //CheckForIllegalCrossThreadCalls = false; //HACK: заменить на долбанные делегации и прочую нетовскую муть
            /*lp.list.UseWaitCursor = true;
            lp.list.Items.Clear();
            lp.list.BeginUpdate();

            //load the directory
            pluginner.IFSPlugin fsp = lp.FSProvider;
            fsp.CurrentDirectory = URL;

            //making the statistic info
            int FileWeight = 0;
            checked { FileWeight = 100 / fsp.DirectoryContent.Count; }

            for (int i = 0; i < fsp.DirectoryContent.Count; i++)
            {
                pluginner.DirItem di = fsp.DirectoryContent[i];

                //parsing all files, that given from the FS provider
                StatusFeedback += FileWeight / 100;
                if (di.Hidden == false || fcmd.Properties.Settings.Default.ShowHidedFiles == true)
                {
                    AddItem(lp, di.TextToShow, fsp.GetMetadata(di.Path), di.Size, di.Date); //todo: add the icon of the file
                }
            }
            lp.list.EndUpdate();
            lp.list.UseWaitCursor = false;*/
            lp.LoadDir(URL, 0);
        }


        /// <summary>
        /// Background file copier
        /// </summary>
        /// <param name="lpa">active listpanel</param>
        /// <param name="lpp">passive listpanel</param>
        void DoCp(FileListPanel lpa, FileListPanel lpp, string DestinationURL, pluginner.File SourceFile, int Progress)
        {
            pluginner.IFSPlugin DestinationFS = lpp.FS;

            pluginner.File NewFile = SourceFile;
            NewFile.Path = DestinationURL;

            DestinationFS.WriteFile(NewFile, Progress);
        }

        /// <summary>
        /// Copy the entrie directory
        /// </summary>
        private void DoCpDir(string source, string destination)
        {
            pluginfinder pf = new pluginfinder();
            pluginner.IFSPlugin fsa = pf.GetFSplugin(source); fsa.CurrentDirectory = source;
            pluginner.IFSPlugin fsb = pf.GetFSplugin(destination);

            if (!Directory.Exists(destination)) { fsb.CreateDirectory(destination); }
            fsb.CurrentDirectory = destination;
            foreach (pluginner.DirItem di in fsa.DirectoryContent)
            {
                if (di.TextToShow == "..")
                { /*не трогать, это кнопка "вверх"*/}
                else if (!di.IsDirectory)
                {
                    //перебор файлов
                    //формирование нового пути
                    string s1 = di.Path; //старый путь
                    pluginner.File CurFile = fsa.GetFile(s1, new int()); //исходный файл
                    string s2 = destination + fsb.DirSeparator + CurFile.Name; //новый путь

                    //запись копии
                    CurFile.Path = s2;
                    fsb.WriteFile(CurFile, new int());
                }
                else
                {
                    //перебор подкаталогов
                    DoCpDir(di.Path, destination + fsb.DirSeparator + di.TextToShow);
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
