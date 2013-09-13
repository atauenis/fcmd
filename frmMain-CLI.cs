/* The File Commander - главное окно
 * Комманды коммандной строки frmMain (command line interface)
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Text;

namespace fcmd
{
    public partial class frmMain{
        /* ЗАМЕТКА РАЗРАБОТЧИКУ
         * 
         * В данном файле размещаются комманды "коммандной строки" FC, которые также
         * используются в штатных функциях файлового менеджера (навигация по каталогам).
         * Все комманды работают с активной и пассивой панелью - Active/PassivePanel.
         * FC всегда их определяет сам. Пассивая панель - всегда получатель файлов.
         * Названия комманд - POSIX в верблюжьем регистре (Ls, Rm, MkDir, Touch и т.п.).
         * Всем коммандам параметры передаются строкой, но допускаются исключения, напр.,
         * если базовая функция "перегружена" функцией для нужд графического интерфейса.
         */

        /// <summary>
        /// Reads the <paramref name="url"/>'s file/subdir list and pushes it into the active panel.
        /// </summary>
        /// <param name="url"></param>
        public void Ls(string url){
            ListPanel lp = ActivePanel;
            lp.Items.Clear();

            //гружу директорию
            pluginner.IFSPlugin fsp = lp.FSProvider;
            fsp.CurrentDirectory = url;
            foreach (pluginner.DirItem di in fsp.DirectoryContent)
            { //перебираю файлы, найденные провайдером ФС
                if (di.Hidden == false)
                {
                    ListPanel.ItemDescription NewItem;
                    NewItem = new ListPanel.ItemDescription();
                    NewItem.Text.Add(di.TextToShow);
                    NewItem.Text.Add(Convert.ToString(di.Size / 1024) + "KB");
                    NewItem.Text.Add(di.Date.ToShortDateString());
                    NewItem.Value = di.Path;
                    NewItem.Selection = 0;
                    NewItem.Selected = false;
                    lp.Items.Add(NewItem);
                }
            }
            lp.Redraw();
        }

        /// <summary>
        /// Reads the file <paramref name="url"/> and shows in FCView
        /// </summary>
        /// <param name="url"></param>
        public void FCView(string url){
            fcview fcv = new fcview();
            pluginner.IFSPlugin fs = ActivePanel.FSProvider;
            if (!fs.IsFilePresent(url)) return; //todo: выругаться

            pluginner.File SelectedFile = fs.GetFile(url);
            string FileContent = Encoding.ASCII.GetString(SelectedFile.Content);
            fcv.LoadFile(FileContent, url);
        }

        /// <summary>
        /// Reads the file <paramref name="url"/> and returns it as string 
        /// (for File Commander's CLI panel)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string Cat(string url){
            fcview fcv = new fcview();
            pluginner.IFSPlugin fs = ActivePanel.FSProvider;
            if (!fs.IsFilePresent(url)) return "Файл не найден\n";

            pluginner.File SelectedFile = fs.GetFile(url);
            return Encoding.ASCII.GetString(SelectedFile.Content);
        }

        /// <summary>
        /// Makes a new directory at the specifed <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        public void MkDir(string url){
            ActivePanel.FSProvider.MakeDir(url);
        }

        /// <summary>
        /// Removes the specifed file
        /// </summary>
        /// <param name="url"></param>
        public string Rm(string url){
            DialogResult DoYouWantToDo = MessageBox.Show("Вы правда хотите сделать это? :-(", "", MessageBoxButtons.YesNo);
            if (DoYouWantToDo == DialogResult.No) return "Отменено пользователем\n";

            ListPanel.ItemDescription curItemDel = ActivePanel.HighlightedItem;
            pluginner.IFSPlugin fsdel = ActivePanel.FSProvider;
            if (!fsdel.IsFilePresent(curItemDel.Value)) return "Файл не найден\n"; //todo: выругаться
            fsdel.RemoveFile(curItemDel.Value);

            return ""; //успех
        }

        /// <summary>
        /// Copy the highlighted file to the passive panel. To be used in FC UI. 
        /// Includes asking of the destination path.
        /// </summary>
        public void Cp(){
            string SourceURL = ActivePanel.HighlightedItem.Value;
            pluginner.IFSPlugin SourceFS = ActivePanel.FSProvider;
            pluginner.File SourceFile = SourceFS.GetFile(SourceURL);

            if (!SourceFS.IsFilePresent(SourceURL)) return; //todo: выругаться

            InputBox ibx = new InputBox("Куда копировать?", PassivePanel.FSProvider.CurrentDirectory + "/" + SourceFile.Name);
            if (ibx.ShowDialog() == DialogResult.OK){
                string DestinationURL = ibx.Result;
                pluginner.IFSPlugin DestinationFS = PassivePanel.FSProvider;

                pluginner.File NewFile = SourceFile;
                NewFile.Path = DestinationURL;

                DestinationFS.WriteFile(NewFile);
            } else return;
        }
    }
}
