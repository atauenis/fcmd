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
using System.IO;

namespace fcmd
{
    public partial class frmMain{
        /* ЗАМЕТКА РАЗРАБОТЧИКУ
         * 
         * В данном файле размещаются комманды "коммандной строки" FC, которые также
         * используются в штатных функциях файлового менеджера (навигация по каталогам).
         * Все комманды работают с активной и пассивой панелью - Active/PassivePanel.
         * FC всегда их определяет сам. Пассивая панель - всегда получатель файлов.
         * Названия комманд - UNIX в верблюжьем регистре (Ls, Rm, MkDir, Touch и т.п.).
         * Всем коммандам параметры передаются строкой, но допускаются исключения, напр.,
         * если базовая функция "перегружена" функцией для нужд графического интерфейса.
         */

        /// <summary>
        /// Reads the <paramref name="url"/>'s file/subdir list and pushes it into the active panel.
        /// </summary>
        /// <param name="url"></param>
        public void Ls(string url){
            int Status = 0;
            ActivePanel.lblPath.Text = url;
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
            fpd.Hide();
        }

        /// <summary>
        /// Reads the file <paramref name="url"/> and shows in FCView
        /// </summary>
        /// <param name="url"></param>
        public void FCView(string url){
            fcview fcv = new fcview();
            pluginner.IFSPlugin fs = ActivePanel.FSProvider;
            if (!fs.FileExists(url))
            {
                MessageBox.Show(string.Format(locale.GetString("FileNotFound"), ActivePanel.list.SelectedItems[0].Text), "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            pluginner.File SelectedFile = fs.GetFile(url, new int());
            string FileContent = Encoding.ASCII.GetString(SelectedFile.Content);
            fcv.LoadFile(url,ActivePanel.FSProvider);
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
            if (!fs.FileExists(url)) return "Файл не найден\n";

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

            Thread MkDirThread = new Thread(delegate() { ActivePanel.FSProvider.CreateDirectory(url); });
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

            string curItemDel = ActivePanel.list.SelectedItems[0].Tag.ToString();
            pluginner.IFSPlugin fsdel = ActivePanel.FSProvider;
            if (fsdel.FileExists(curItemDel))
            {
                fpd.pbrProgress.Value = 50;
                Thread RmFileThread = new Thread(delegate() { DoRmFile(curItemDel, fsdel); });
                RmFileThread.Start();

                do { Application.DoEvents();}
                while (RmFileThread.ThreadState == ThreadState.Running);

                fpd.pbrProgress.Value = 100;
                LoadDir(ActivePanel.FSProvider.CurrentDirectory, ActivePanel);
                fpd.Hide();
                return "Файл удалён.\n";
            }
            if (fsdel.DirectoryExists(curItemDel))
            {
                fpd.lblStatus.Text = String.Format(locale.GetString("DoingRemove"), "\n" + url, "\n[" + locale.GetString("Directory").ToUpper() + "]");
                fpd.pbrProgress.Value = 50;
                Thread RmDirThread = new Thread(delegate() { DoRmDir(curItemDel, fsdel); });
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
            string SourceURL = ActivePanel.list.SelectedItems[0].Tag.ToString();
            pluginner.IFSPlugin SourceFS = ActivePanel.FSProvider;
            pluginner.File SourceFile = SourceFS.GetFile(SourceURL, Progress);

            if (!SourceFS.FileExists(SourceURL))
            {//такого файла (уже?) нет
                if (SourceFS.DirectoryExists(SourceURL))//а вдруг есть такой каталог?
                {
                    InputBox ibxd = new InputBox(String.Format(locale.GetString("CopyTo"), SourceFile.Name), PassivePanel.FSProvider.CurrentDirectory + "/" + SourceFile.Name);
                    if (ibxd.ShowDialog() == DialogResult.OK) { CpDir(SourceURL, ibxd.Result); }
                    LoadDir(PassivePanel.FSProvider.CurrentDirectory, PassivePanel); //обновление пассивной панели
                    return;
                }
                MessageBox.Show(string.Format(locale.GetString("FileNotFound"), ActivePanel.list.SelectedItems[0].Text), "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            InputBox ibx = new InputBox(String.Format(locale.GetString("CopyTo"), SourceFile.Name), PassivePanel.FSProvider.CurrentDirectory + "/" + SourceFile.Name);
            if (ibx.ShowDialog() == DialogResult.OK)
            {

                Thread CpThread = new Thread(delegate() { DoCp(ActivePanel, PassivePanel, ibx.Result, SourceFile, Progress); });
                FileProcessDialog fpd = new FileProcessDialog();
                fpd.Top = this.Top + ActivePanel.Top;
                fpd.Left = this.Left + ActivePanel.Left;
                fpd.lblStatus.Text = String.Format(locale.GetString("DoingCopy"), "\n" + ActivePanel.list.SelectedItems[0].Tag.ToString() + "\n", ibx.Result,null);
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

        /// <summary>
        /// Copy the entrie directory
        /// </summary>
        private void CpDir(string source, string destination){
            //todo: вынести в workers.cs и отдельный поток
            pluginfinder pf = new pluginfinder();
            pluginner.IFSPlugin fsa = pf.GetFSplugin(source); fsa.CurrentDirectory = source;
            pluginner.IFSPlugin fsb = pf.GetFSplugin(destination);

            if(!Directory.Exists(destination)){ fsb.CreateDirectory(destination);}
            fsb.CurrentDirectory = destination; 
            foreach (pluginner.DirItem di in fsa.DirectoryContent){
                if (di.TextToShow == "..")
                {/*не трогать, это кнопка "вверх"*/}
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
                    CpDir(di.Path, destination + fsb.DirSeparator + di.TextToShow);
                }
            }
        }

    }
}
