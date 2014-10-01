/* The File Commander main window
 * File managing operations
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using pluginner.Widgets;
using Xwt;

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
				Xwt.MessageDialog.ShowError(string.Format(Localizator.GetString("FileNotFound"),ActivePanel.GetValue(ActivePanel.dfDisplayName)));
				return;
			}
			string FileContent = Encoding.ASCII.GetString(fs.GetFileContent(url));
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

			return Encoding.ASCII.GetString(fs.GetFileContent(url));
		}

		/// <summary>
		/// Makes a new directory at the specifed <paramref name="url"/>
		/// </summary>
		/// <param name="url"></param>
		public void MkDir(string url)
		{
			FileProcessDialog fpd = new FileProcessDialog();
			fpd.lblStatus.Text = string.Format(Localizator.GetString("DoingMkdir"), "\n" + url, null);
			fpd.Show();

			Thread MkDirThread = new Thread(delegate() { ActivePanel.FS.CreateDirectory(url); });
			MkDirThread.Start();

			do { /*Application.DoEvents();*/ Xwt.Application.MainLoop.DispatchPendingEvents(); }
			while (MkDirThread.ThreadState == ThreadState.Running);

			fpd.pbrProgress.Fraction = 1;
			ActivePanel.LoadDir(ActivePanel.FS.CurrentDirectory);
			fpd.Hide();
		}

        /// <summary>Removes the current selected files</summary>
        public void Rm()
        {
            if (ActivePanel.GetValue(ActivePanel.dfDisplayName) == "..") { return; }
            ListView2Item[] chdrws = ActivePanel.ListingView.ChoosedRows.ToArray();//because the List may change due the process, we getting the copy of the list (as array, but how else?)
            foreach (ListView2Item selitem in chdrws)
            {
                string URL = selitem.Data[ActivePanel.dfURL].ToString();
                Rm(URL);
            }
            ActivePanel.LoadDir();
        }

		/// <summary>
		/// Removes the specifed file
		/// </summary>
		public string Rm(string url)
		{
			if (ActivePanel.GetValue<string>(ActivePanel.dfDisplayName) == "..") { return "Cannot remove .."; }

			if (!Xwt.MessageDialog.Confirm(
				String.Format(Localizator.GetString("FCDelAsk"), url, null),
				Xwt.Command.Remove,
				true))
			{ return Localizator.GetString("Canceled"); };

			FileProcessDialog fpd = new FileProcessDialog();
			fpd.lblStatus.Text = String.Format(Localizator.GetString("DoingRemove"), "\n" + url, null);
			fpd.cmdCancel.Sensitive = false;
			fpd.Show();

			string curItemDel = ActivePanel.GetValue<string>(ActivePanel.dfURL);
			pluginner.IFSPlugin fsdel = ActivePanel.FS;
			if (fsdel.FileExists(curItemDel))
			{
				fpd.pbrProgress.Fraction = 0.5;
				Thread RmFileThread = new Thread(delegate() { DoRmFile(curItemDel, fsdel); });
				RmFileThread.Start();

				do { Xwt.Application.MainLoop.DispatchPendingEvents(); }
				while (RmFileThread.ThreadState == ThreadState.Running);

				fpd.pbrProgress.Fraction = 1;
				fpd.Hide();
				return "Файл удалён.\n";
			}
			if (fsdel.DirectoryExists(curItemDel))
			{
				fpd.lblStatus.Text = String.Format(Localizator.GetString("DoingRemove"), "\n" + url, "\n[" + Localizator.GetString("Directory").ToUpper() + "]");
				fpd.pbrProgress.Fraction = 0.5;
				Thread RmDirThread = new Thread(delegate() { DoRmDir(curItemDel, fsdel); });
				RmDirThread.Start();

				do { Xwt.Application.MainLoop.DispatchPendingEvents(); }
				while (RmDirThread.ThreadState == ThreadState.Running);

				fpd.pbrProgress.Fraction = 1;

				fpd.Hide();
				return "Каталог удалён.\n";
			}
			return "Файл не найден";
		}

		/// <summary>
		/// Copy the highlighted file to the passive panel. To be used in FC UI. 
		/// Includes asking of the destination path.
		/// </summary>
        [STAThread]
		public void Cp()
		{
			if (ActivePanel.GetValue<string>(ActivePanel.dfDisplayName) == "..") { return; }

            foreach (ListView2Item selitem in ActivePanel.ListingView.ChoosedRows)
            {
                string SourceURL = selitem.Data[ActivePanel.dfURL].ToString();
                pluginner.IFSPlugin SourceFS = ActivePanel.FS;

                //check for file existing
                if (SourceFS.FileExists(SourceURL))
                {
	                string SourceName = SourceFS.GetMetadata(SourceURL).Name;
                    InputBox ibx = new InputBox(String.Format(Localizator.GetString("CopyTo"), SourceName), PassivePanel.FS.CurrentDirectory + PassivePanel.FS.DirSeparator + SourceName);
                    if (ibx.ShowDialog(this))
                    {
                        String DestinationFilePath = ibx.Result;
                        string StatusMask = Localizator.GetString("DoingCopy");

                        ReplaceQuestionDialog.ClickedButton dummy = ReplaceQuestionDialog.ClickedButton.Cancel;
                        AsyncCopy AC = new AsyncCopy();

                        Thread CpThread = new Thread(delegate() { DoCp(ActivePanel.FS, PassivePanel.FS, SourceURL, DestinationFilePath, ref dummy, AC); });
                        CpThread.TrySetApartmentState(ApartmentState.STA);
                        FileProcessDialog fpd = new FileProcessDialog();
                        fpd.InitialLocation = Xwt.WindowLocation.CenterParent;
                        fpd.lblStatus.Text = String.Format(StatusMask, ActivePanel.GetValue<string>(ActivePanel.dfURL), ibx.Result, null);
						fpd.cmdCancel.Clicked += (object s, EventArgs e) => { CpThread.Abort(); MessageDialog.ShowWarning(Localizator.GetString("Canceled"), ActivePanel.GetValue<string>(ActivePanel.dfURL)); };

                        AC.ReportMessage = Localizator.GetString("CopyStatus");
						AC.OnProgress += (message, percent) =>
                        {
							Xwt.Application.Invoke(() =>
                                {
                                    try
                                    {
                                        fpd.pbrProgress.Fraction = (double)((double)percent / (double)100);
                                        fpd.lblStatus.Text = String.Format(StatusMask, ActivePanel.GetValue<string>(ActivePanel.dfURL), ibx.Result, message);
                                    }
                                    catch { }
                                }
                            );
                        };

                        fpd.Show();
                        CpThread.Start();

                        do
                        {
                            Xwt.Application.MainLoop.DispatchPendingEvents();
                        }
                        while (CpThread.ThreadState == ThreadState.Running);
                        //todo: замер и показ скорости, пауза, запрос отмены, вывод в фоновый поток (кнопка "в фоне").

                        fpd.Hide();
                    }
                    continue;
                }
                //not a file...maybe directory?
                if (SourceFS.DirectoryExists(SourceURL))//а вдруг есть такой каталог?
                {
                    InputBox ibxd = new InputBox(String.Format(Localizator.GetString("CopyTo"), SourceFS.GetMetadata(SourceURL).Name), PassivePanel.FS.CurrentDirectory + "/" + SourceFS.GetMetadata(SourceURL).Name);

                    if (ibxd.ShowDialog())
                    {
                        String DestinationDirPath = ibxd.Result;
                        //копирование каталога
                        Thread CpDirThread = new Thread(delegate() { DoCpDir(SourceURL, DestinationDirPath, ActivePanel.FS, PassivePanel.FS); });
                        CpDirThread.TrySetApartmentState(ApartmentState.STA); 

                        FileProcessDialog CpDirProgressDialog = new FileProcessDialog();
                        CpDirProgressDialog.InitialLocation = Xwt.WindowLocation.CenterParent;
                        CpDirProgressDialog.lblStatus.Text = String.Format(Localizator.GetString("DoingCopy"), "\n" + ActivePanel.GetValue<string>(ActivePanel.dfURL) + " [" + Localizator.GetString("Directory") + "]\n", ibxd.Result, null);
						CpDirProgressDialog.cmdCancel.Clicked += (object s, EventArgs e) => { CpDirThread.Abort(); MessageDialog.ShowWarning(Localizator.GetString("Canceled"), ActivePanel.GetValue<string>(ActivePanel.dfURL)); };

                        CpDirProgressDialog.Show();
                        CpDirThread.Start();

                        do { Xwt.Application.MainLoop.DispatchPendingEvents(); }
                        while (CpDirThread.ThreadState == ThreadState.Running);

                        //LoadDir(PassivePanel.FSProvider.CurrentDirectory, PassivePanel); //обновление пассивной панели
                        PassivePanel.LoadDir();
                        CpDirProgressDialog.Hide();
                    }
                    continue;
                }
                //and, if none of those IF blocks has been entered, say that this isn't a real file nor a directory
                Xwt.MessageDialog.ShowWarning(
                    Localizator.GetString("FileNotFound"),
                    ActivePanel.GetValue<string>(
                        ActivePanel.dfURL
                    )
                );
                continue;

            }
		}

		/// <summary>
		/// Move the selected file or directory
		/// </summary>
		void Mv()
		{
			if (ActivePanel.GetValue<string>(ActivePanel.dfDisplayName) == "..") { return; }

			pluginner.IFSPlugin SourceFS = ActivePanel.FS;
			pluginner.IFSPlugin DestinationFS = PassivePanel.FS;

            foreach (ListView2Item selitem in ActivePanel.ListingView.ChoosedRows)
            {
                //Getting useful URL parts
                string SourceName = selitem.Data[ActivePanel.dfDisplayName].ToString(); //ActivePanel.GetValue<string>(ActivePanel.dfDisplayName);
                string SourcePath = selitem.Data[ActivePanel.dfURL].ToString(); //ActivePanel.GetValue<string>(ActivePanel.dfURL);
                string DestinationPath = DestinationFS.CurrentDirectory + DestinationFS.DirSeparator + SourceName;

                InputBox ibx = new InputBox
                    (
                    string.Format(Localizator.GetString("MoveTo"), SourceName),
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
                    //todo
                }

                //Now, assuming that the src & dest fs is same and supports
                //cross-disk file moving

                if (SourcePath == DestinationPath)
                {
                    string itself = Localizator.GetString("CantCopySelf");
                    string toshow = string.Format(Localizator.GetString("CantMove"), SourcePath, itself);

                    Xwt.Application.Invoke(delegate { Xwt.MessageDialog.ShowWarning(toshow); });
                    //calling the msgbox in non-main threads causes some UI bugs, thus pushing this call into main thread
                    return;
                }

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
}
