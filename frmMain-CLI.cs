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
using System.Threading;

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
            Thread LsThread = new Thread(delegate() { DoLs(url, ActivePanel); });
            FileProcessDialog fpd = new FileProcessDialog();
            fpd.Top = this.Top + ActivePanel.Top;
            fpd.Left = this.Left + ActivePanel.Left;
            fpd.lblStatus.Text = "Загружаю содержимое каталога " + url;
            
            fpd.Show();
            LsThread.Start();

            do{Application.DoEvents();}
            while (LsThread.ThreadState == ThreadState.Running);
            ActivePanel.Redraw();
            fpd.Hide();
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
            if (ibx.ShowDialog() == DialogResult.OK)
            {

                Thread CpThread = new Thread(delegate() { DoCp(ActivePanel, PassivePanel, ibx.Result, SourceFile); });
                FileProcessDialog fpd = new FileProcessDialog();
                fpd.Top = this.Top + ActivePanel.Top;
                fpd.Left = this.Left + ActivePanel.Left;
                fpd.lblStatus.Text = "Выполняется копирование:\n" + ActivePanel.HighlightedItem.Value + "\nВ " + ibx.Result;
                fpd.cmdCancel.Click += (object s, EventArgs e) => { CpThread.Abort(); MessageBox.Show("Отменено"); };

                CpThread.Start();
                fpd.Show();

                do{Application.DoEvents();}
                while (CpThread.ThreadState == ThreadState.Running);

                LoadDir(PassivePanel.FSProvider.CurrentDirectory, PassivePanel); //обновление пассивной панели
                fpd.Hide();
            }
            else return;
        }
    }
}
