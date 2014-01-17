/* The File Commander main window
 * File managing operations
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace fcmd
{
    partial class MainWindow
    {
        /* ЗАМЕТКА РАЗРАБОТЧИКУ
         * 
         * В данном файле размещаются подпрограммы для управления файлами, которые
         * вызываются из MainWindow.cs. Также планируется использование этих подпрограмм
         * после реализации текстовой коммандной строки FC (которая внизу окна).
         * Все комманды работают с активной и пассивой панелью - Active/PassivePanel.
         * FC всегда их определяет сам. Пассивая панель - всегда получатель файлов.
         * Названия комманд - UNIX в верблюжьем регистре (Ls, Rm, MkDir, Touch и т.п.).
         * Всем коммандам параметры передаются строкой, но допускаются исключения, напр.,
         * если базовая функция "перегружена" функцией для нужд графического интерфейса.
         * Sorry for my bad english.
         */

        /// <summary>
        /// Reads the <paramref name="url"/>'s file/subdir list and pushes it into the active panel.
        /// </summary>
        /// <param name="url"></param>
        public void Ls(string url)
        {
            int Status = 0;
            Thread LsThread = new Thread(delegate() { DoLs(url, ActivePanel, ref Status); });
            FileProcessDialog fpd = new FileProcessDialog();
            fpd.Location = new Xwt.Point(this.Location.Y, this.Location.X);
            string FPDtext = String.Format(Locale.GetString("DoingListdir"), "\n" + url, "");
            FPDtext = FPDtext.Replace("{1}", "");
            fpd.lblStatus.Text = FPDtext;

            fpd.Show();
            LsThread.Start();

            do
            {
                Xwt.Application.MainLoop.DispatchPendingEvents();//Application.DoEvents();
                fpd.pbrProgress.Fraction = Status;
            }
            while (LsThread.ThreadState == ThreadState.Running);
            fpd.Hide();
        }

        /// <summary>
        /// Reads the file <paramref name="url"/> and shows in FC Viewer
        /// </summary>
        /// <param name="url"></param>
        public void FCView(string url)
        {
            VEd fcv = new VEd();
            pluginner.IFSPlugin fs = ActivePanel.FS;
            if (!fs.FileExists(url))
            {
                //MessageBox.Show(string.Format(locale.GetString("FileNotFound"), ActivePanel.list.SelectedItems[0].Text), "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                Xwt.MessageDialog.ShowError(string.Format(Locale.GetString("FileNotFound"),ActivePanel.GetValue(ActivePanel.dfDisplayName)));
                return;
            }

            pluginner.File SelectedFile = fs.GetFile(url, new int());
            string FileContent = Encoding.ASCII.GetString(SelectedFile.Content);
            fcv.LoadFile(url, ActivePanel.FS, false);
            fcv.Show();
        }

        /// <summary>
        /// Reads the file <paramref name="url"/> and returns it as string 
        /// (for File Commander's CLI panel)
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public string Cat(string url)
        {
            pluginner.IFSPlugin fs = ActivePanel.FS;
            if (!fs.FileExists(url)) return "Файл не найден\n";

            pluginner.File SelectedFile = fs.GetFile(url, new int());
            return Encoding.ASCII.GetString(SelectedFile.Content);
        }

        /// <summary>
        /// Makes a new directory at the specifed <paramref name="url"/>
        /// </summary>
        /// <param name="url"></param>
        public void MkDir(string url)
        {
            FileProcessDialog fpd = new FileProcessDialog();
            fpd.lblStatus.Text = string.Format(Locale.GetString("DoingMkdir"), "\n" + url, null);
            fpd.Show();

            Thread MkDirThread = new Thread(delegate() { ActivePanel.FS.CreateDirectory(url); });
            MkDirThread.Start();

            do { /*Application.DoEvents();*/ Xwt.Application.MainLoop.DispatchPendingEvents(); }
            while (MkDirThread.ThreadState == ThreadState.Running);

            fpd.pbrProgress.Fraction = 1;
            ActivePanel.LoadDir(ActivePanel.FS.CurrentDirectory,0);
            fpd.Hide();
        }

        /// <summary>
        /// Removes the specifed file
        /// </summary>
        public string Rm(string url)
        {
            if (!Xwt.MessageDialog.Confirm(
                String.Format(Locale.GetString("FCDelAsk"), url, null),
                Xwt.Command.Remove,
                false))
            { return Locale.GetString("Canceled"); };

            FileProcessDialog fpd = new FileProcessDialog();
            fpd.lblStatus.Text = String.Format(Locale.GetString("DoingRemove"), "\n" + url, null);
            fpd.cmdCancel.Sensitive = false;
            fpd.Show();

            string curItemDel = ActivePanel.FLStore.GetValue<string>(ActivePanel.ListingView.SelectedRow, ActivePanel.dfURL);
            pluginner.IFSPlugin fsdel = ActivePanel.FS;
            if (fsdel.FileExists(curItemDel))
            {
                fpd.pbrProgress.Fraction = 0.5;
                Thread RmFileThread = new Thread(delegate() { DoRmFile(curItemDel, fsdel); });
                RmFileThread.Start();

                do { Xwt.Application.MainLoop.DispatchPendingEvents(); }
                while (RmFileThread.ThreadState == ThreadState.Running);

                fpd.pbrProgress.Fraction = 1;
                ActivePanel.LoadDir(ActivePanel.FS.CurrentDirectory,0);
                fpd.Hide();
                return "Файл удалён.\n";
            }
            if (fsdel.DirectoryExists(curItemDel))
            {
                fpd.lblStatus.Text = String.Format(Locale.GetString("DoingRemove"), "\n" + url, "\n[" + Locale.GetString("Directory").ToUpper() + "]");
                fpd.pbrProgress.Fraction = 0.5;
                Thread RmDirThread = new Thread(delegate() { DoRmDir(curItemDel, fsdel); });
                RmDirThread.Start();

                do { Xwt.Application.MainLoop.DispatchPendingEvents(); }
                while (RmDirThread.ThreadState == ThreadState.Running);

                fpd.pbrProgress.Fraction = 1;
                ActivePanel.LoadDir(ActivePanel.FS.CurrentDirectory,0);

                fpd.Hide();
                return "Каталог удалён.\n";
            }
            return "Файл не найден";
        }

        /// <summary>
        /// Copy the highlighted file to the passive panel. To be used in FC UI. 
        /// Includes asking of the destination path.
        /// </summary>
        public void Cp()
        {
            int Progress = 0;
            string SourceURL = ActivePanel.FLStore.GetValue<string>(ActivePanel.ListingView.SelectedRow, ActivePanel.dfURL);
            pluginner.IFSPlugin SourceFS = ActivePanel.FS;
            pluginner.File SourceFile = SourceFS.GetFile(SourceURL, Progress);

            //check for file existing
            if(SourceFS.FileExists(SourceURL)){
                InputBox ibx = new InputBox(String.Format(Locale.GetString("CopyTo"), SourceFile.Name), PassivePanel.FS.CurrentDirectory + "/" + SourceFile.Name);
                if (ibx.ShowDialog())
                {
                    String DestinationFilePath = ibx.Result;
                    Thread CpThread = new Thread(delegate() { DoCp(ActivePanel, PassivePanel, DestinationFilePath, SourceFile, Progress); });
                    FileProcessDialog fpd = new FileProcessDialog();
                    //UNDONE: place the FPD over the middle of the main window
                    fpd.lblStatus.Text = String.Format(Locale.GetString("DoingCopy"), "\n" + ActivePanel.FLStore.GetValue<string>(ActivePanel.ListingView.SelectedRow, ActivePanel.dfURL) + "\n", ibx.Result, null);
                    fpd.cmdCancel.Clicked += (object s, EventArgs e) => { CpThread.Abort(); new MsgBox(Locale.GetString("Canceled"), ActivePanel.FLStore.GetValue<string>(ActivePanel.ListingView.SelectedRow, ActivePanel.dfURL), MsgBox.MsgBoxType.Warning); };

                    CpThread.Start();
                    fpd.Show();

                    do {Xwt.Application.MainLoop.DispatchPendingEvents(); fpd.pbrProgress.Fraction = Progress; }
                    while (CpThread.ThreadState == ThreadState.Running);

                    PassivePanel.LoadDir();
                    fpd.Hide();
                }
                return;
            }
            //not a file...maybe directory?
            if (SourceFS.DirectoryExists(SourceURL))//а вдруг есть такой каталог?
            {
                InputBox ibxd = new InputBox(String.Format(Locale.GetString("CopyTo"), SourceFile.Name), PassivePanel.FS.CurrentDirectory + "/" + SourceFile.Name);
                String DestinationDirPath = ibxd.Result;
                if (ibxd.ShowDialog())
                {
                    //копирование каталога
                    Thread CpDirThread = new Thread(delegate() { DoCpDir(SourceURL, DestinationDirPath); });

                    FileProcessDialog CpDirProgressDialog = new FileProcessDialog();
                    /*CpDirProgressDialog.Y = this.Top + ActivePanel.Top;
                    CpDirProgressDialog.X = this.Left + ActivePanel.Left;*/
                    //UNDONE: позиция окна статуса!!!
                    CpDirProgressDialog.lblStatus.Text = String.Format(Locale.GetString("DoingCopy"), "\n" + ActivePanel.FLStore.GetValue<string>(ActivePanel.ListingView.SelectedRow, ActivePanel.dfURL) + " [" + Locale.GetString("Directory") + "]\n", ibxd.Result, null);
                    CpDirProgressDialog.cmdCancel.Clicked += (object s, EventArgs e) => { CpDirThread.Abort(); new MsgBox(Locale.GetString("Canceled"), ActivePanel.GetValue<string>(ActivePanel.dfURL), MsgBox.MsgBoxType.Warning); };

                    CpDirProgressDialog.Show();
                    CpDirThread.Start();

                    do { Xwt.Application.MainLoop.DispatchPendingEvents(); }
                    while (CpDirThread.ThreadState == ThreadState.Running);

                    //LoadDir(PassivePanel.FSProvider.CurrentDirectory, PassivePanel); //обновление пассивной панели
                    PassivePanel.LoadDir();
                    CpDirProgressDialog.Hide();
                }
                return;
            }
            //and if no of those IF blocks are open, say that this isn't a real file nor directory
            Xwt.MessageDialog.ShowWarning(
                Locale.GetString("FileNotFound"),
                ActivePanel.FLStore.GetValue<string>(
                    ActivePanel.ListingView.SelectedRow, ActivePanel.dfURL
                )
            );
            return;
        }

        /// <summary>
        /// Move the selected file or directory
        /// </summary>
        void Mv()
        {
            pluginner.IFSPlugin SourceFS = ActivePanel.FS;
            pluginner.IFSPlugin DestinationFS = PassivePanel.FS;

            //Getting useful URL parts
            string SourceName = ActivePanel.FLStore.GetValue<string>(ActivePanel.ListingView.SelectedRow, ActivePanel.dfDisplayName);
            string SourcePath = ActivePanel.FLStore.GetValue<string>(ActivePanel.ListingView.SelectedRow, ActivePanel.dfURL);
            string DestinationPath = DestinationFS.CurrentDirectory + DestinationFS.DirSeparator + SourceName;

            InputBox ibx = new InputBox
                (
                string.Format(Locale.GetString("MoveTo"), SourceName),
                DestinationPath
                );
            if (ibx.ShowDialog())
                DestinationPath = ibx.Result;
            else return;

            // Comparing the filesystems; if they is diffrent, use copy&delete
            // instead of direct move (if anyone knows, how a file can be moved
            // from an FTP to an ext2fs on Windows or MacOS please tell me :-) )
            if (SourceFS.GetType() != DestinationFS.GetType())
            {
                Xwt.MessageDialog.ShowError("Cannot move between diffrent filesystems!\nНе сделана поддержка перемещения между разными ФС.");
                Cp();
                return;
            }

            //Now, assuming that the src & dest fs is same and supports
            //cross-disk file moving
            if (SourceFS.DirectoryExists(SourcePath))
            {//this is a directory
                SourceFS.MoveDirectory(SourcePath, DestinationPath);
            }
            if (SourceFS.FileExists(SourcePath))
            {//this is a file
                SourceFS.MoveFile(SourcePath, DestinationPath);
            }

            ActivePanel.LoadDir();
            PassivePanel.LoadDir();
        }
    }
}
