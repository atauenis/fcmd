﻿/* The File Commander - главное окно
 * Таджики для работ с файлами в фоновых потоках
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms; //я тебе! убрать при первой возможности!

namespace fcmd{
    public partial class frmMain{
        /* ЗАМЕТКА РАЗРАБОТЧИКУ
         * 
         * В данном файле размещаются функции для работы с файлами и каталогами.
         * Данные функции запускаются в отдельных от UI потоках и вызваются функциями
         * из файлов frmMain.cs и frmMain-CLI.cs .
         * Крайне нежелательно обращение к элементам управления формы, поскольку
         * это требует всяких согласующих операций и прочих делегатов, что ухудшает
         * читаемость кода.
         * Также нежелательно обращение к System.Windows.Forms, поскольку код данного
         * файла должен быть кросс-платформенным.
         */

        /// <summary>
        /// Background directory lister
        /// </summary>
        void DoLs(string URL, ListPanel lp, ref int StatusFeedback){
            CheckForIllegalCrossThreadCalls = false; //HACK: заменить на долбанные делегации и прочую нетовскую муть
            lp.list.UseWaitCursor = true;
            lp.list.Items.Clear();
            lp.list.BeginUpdate();
            
            //гружу директорию
            pluginner.IFSPlugin fsp = lp.FSProvider;
            fsp.CurrentDirectory = URL;

            //готовлю статистическую информацию
            int FileWeight = 0;
            checked { FileWeight = 100 / fsp.DirectoryContent.Count; }

            foreach (pluginner.DirItem di in fsp.DirectoryContent)
            { //перебираю файлы, найденные провайдером ФС
                StatusFeedback += FileWeight;
                if (di.Hidden == false)
                {
                    ListViewItem NewItem = new ListViewItem(di.TextToShow);
                    NewItem.Tag = di.Path; //путь будет тегом
                    NewItem.SubItems.Add(Convert.ToString(di.Size / 1024) + "KB");
                    NewItem.SubItems.Add(di.Date.ToShortDateString());
                    AddItem(lp, NewItem);
                }
            }
            lp.list.EndUpdate();
            lp.list.UseWaitCursor = false;
        }


        /// <summary>
        /// Background file copier
        /// </summary>
        /// <param name="lpa">active listpanel</param>
        /// <param name="lpp">passive listpanel</param>
        void DoCp(ListPanel lpa, ListPanel lpp, string DestinationURL, pluginner.File SourceFile, int Progress){
            pluginner.IFSPlugin DestinationFS = lpp.FSProvider;

            pluginner.File NewFile = SourceFile;
            NewFile.Path = DestinationURL;

            DestinationFS.WriteFile(NewFile, Progress);
        }

        /// <summary>
        /// Background file remove
        /// </summary>
        /// <param name="url">url of the file</param>
        /// <param name="fs">filesystem of the file</param>
        void DoRmFile(string url, pluginner.IFSPlugin fs){
            try{
                fs.DeleteFile(url);
            }
            catch (Exception err){
                MessageBox.Show(err.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }

        /// <summary>
        /// Background directory remove
        /// </summary>
        /// <param name="url">url of the file</param>
        /// <param name="fs">filesystem of the file</param>
        void DoRmDir(string url, pluginner.IFSPlugin fs){
            try
            {
                fs.DeleteDirectory(url, true);
            }
            catch (pluginner.ThisDirCannotBeRemovedException nesudba)
            {
                MessageBox.Show(string.Format(locale.GetString("DirCantBeRemoved"),url),null,MessageBoxButtons.OK,MessageBoxIcon.Stop);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, null, MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
        }
    }
}
