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
            int Status = 0;
            Thread LsThread = new Thread(delegate() { DoLs(url, ActivePanel, ref Status); });
            FileProcessDialog fpd = new FileProcessDialog();
            fpd.Top = this.Top + ActivePanel.Top;
            fpd.Left = this.Left + ActivePanel.Left;
            string FPDtext = String.Format(locale.GetString("DoingListdir"), "\n" + url, "");
            FPDtext = FPDtext.Replace("{1}", "");
            fpd.lblStatus.Text = FPDtext;
            
            fpd.Show();
            LsThread.Start();

            do { Application.DoEvents(); fpd.pbrProgress.Value = Status; fpd.Refresh(); }
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

            pluginner.File SelectedFile = fs.GetFile(url, new int());
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

            pluginner.File SelectedFile = fs.GetFile(url, new int());
            return Encoding.ASCII.GetString(SelectedFile.Content);
        }

        /// <summary>
        /// Makes a new directory at the specifed <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        public void MkDir(string url){
            FileProcessDialog fpd = new FileProcessDialog();
            fpd.lblStatus.Text = string.Format(locale.GetString("DoingMkdir"),"\n" + url,null);
            fpd.Show();

            Thread MkDirThread = new Thread(delegate() { ActivePanel.FSProvider.MakeDir(url); });
            MkDirThread.Start();

            LoadDir(ActivePanel.FSProvider.CurrentDirectory, ActivePanel);

            do { Application.DoEvents();}
            while (MkDirThread.ThreadState == ThreadState.Running);

            fpd.pbrProgress.Value = 100;
            LoadDir(ActivePanel.FSProvider.CurrentDirectory, ActivePanel);
            fpd.Hide();
        }

        /// <summary>
        /// Removes the specifed file
        /// </summary>
        /// <param name="url"></param>
        public string Rm(string url){
            DialogResult DoYouWantToDo = MessageBox.Show(String.Format(locale.GetString("FCDelAsk"),url, null), "", MessageBoxButtons.YesNo);
            if (DoYouWantToDo == DialogResult.No) return locale.GetString("Canceled");

            FileProcessDialog fpd = new FileProcessDialog();
            fpd.lblStatus.Text = String.Format(locale.GetString("DoingRemove"),"\n" + url,null);
            fpd.cmdCancel.Enabled = false;
            fpd.Show();

            ListPanel.ItemDescription curItemDel = ActivePanel.HighlightedItem;
            pluginner.IFSPlugin fsdel = ActivePanel.FSProvider;
            if (fsdel.IsFilePresent(curItemDel.Value))
            {
                fpd.pbrProgress.Value = 50;
                Thread RmFileThread = new Thread(delegate() { DoRmFile(curItemDel.Value, fsdel); });
                RmFileThread.Start();

                do { Application.DoEvents();}
                while (RmFileThread.ThreadState == ThreadState.Running);

                fpd.pbrProgress.Value = 100;
                LoadDir(ActivePanel.FSProvider.CurrentDirectory, ActivePanel);
                fpd.Hide();
                return "Файл удалён.\n";
            }
            if (fsdel.IsDirPresent(curItemDel.Value))
            {
                fpd.lblStatus.Text = String.Format(locale.GetString("DoingRemove"), "\n" + url, "\n[" + locale.GetString("Directory").ToUpper() + "]");
                fpd.pbrProgress.Value = 50;
                Thread RmDirThread = new Thread(delegate() { DoRmDir(curItemDel.Value, fsdel); });
                RmDirThread.Start();

                do { Application.DoEvents(); }
                while (RmDirThread.ThreadState == ThreadState.Running);

                fpd.pbrProgress.Value = 100;
                LoadDir(ActivePanel.FSProvider.CurrentDirectory, ActivePanel);
                fpd.Hide();
                return "Каталог удалён.\n";
            }
            return "Файл не найден";
        }

        /// <summary>
        /// Copy the highlighted file to the passive panel. To be used in FC UI. 
        /// Includes asking of the destination path.
        /// </summary>
        public void Cp(){
            int Progress = 0;
            string SourceURL = ActivePanel.HighlightedItem.Value;
            pluginner.IFSPlugin SourceFS = ActivePanel.FSProvider;
            pluginner.File SourceFile = SourceFS.GetFile(SourceURL, Progress);

            if (!SourceFS.IsFilePresent(SourceURL)) return; //todo: выругаться

            InputBox ibx = new InputBox(String.Format(locale.GetString("CopyTo"), SourceFile.Name), PassivePanel.FSProvider.CurrentDirectory + "/" + SourceFile.Name);
            if (ibx.ShowDialog() == DialogResult.OK)
            {

                Thread CpThread = new Thread(delegate() { DoCp(ActivePanel, PassivePanel, ibx.Result, SourceFile, Progress); });
                FileProcessDialog fpd = new FileProcessDialog();
                fpd.Top = this.Top + ActivePanel.Top;
                fpd.Left = this.Left + ActivePanel.Left;
                //fpd.lblStatus.Text = "Выполняется копирование:\n" + ActivePanel.HighlightedItem.Value + "\nВ " + ibx.Result;
                fpd.lblStatus.Text = String.Format(locale.GetString("DoingCopy"), "\n" + ActivePanel.HighlightedItem.Value + "\n", ibx.Result,null);
                fpd.cmdCancel.Click += (object s, EventArgs e) => { CpThread.Abort(); MessageBox.Show(locale.GetString("Canceled")); };

                CpThread.Start();
                fpd.Show();

                do { Application.DoEvents(); fpd.pbrProgress.Value = Progress; }
                while (CpThread.ThreadState == ThreadState.Running);

                LoadDir(PassivePanel.FSProvider.CurrentDirectory, PassivePanel); //обновление пассивной панели
                fpd.Hide();
            }
            else return;
        }
    }
}
