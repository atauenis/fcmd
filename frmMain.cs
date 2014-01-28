/* The File Commander
 * Главное окно (временная версия)    The main window (temporary)
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.Threading;
using fcmd.base_plugins.fs;

namespace fcmd
{
	public partial class frmMain : Form 
	{
        //TODO переписать на XWT

        //Внутренние переменные
		private List<ListPanel> lplLeft = new List<ListPanel>();
		private List<ListPanel> lplRight = new List<ListPanel>();
		private ListPanel ActivePanel; //текущая активная панель (на которой стоит фокус)
		private ListPanel PassivePanel; //текущая пассивная панель (панель-получатель)
        private MenuStrip mstMenu;
        private ToolStrip tsKeyboard;
        private ToolStripButton tsbHelpF1;
        private ToolStripButton tsbHelpF2;
        private ToolStripButton tsbHelpF3;
        private ToolStripButton tsbHelpF4;
        private ToolStripButton tsbHelpF5;
        private ToolStripButton tsbHelpF6;
        private ToolStripButton tsbHelpF7;
        private ToolStripButton tsbHelpF8;
        private ToolStripButton tsbHelpF9;
        private ToolStripButton tsbHelpF10;
        private ToolStripMenuItem mnuFile;
        private ToolStripMenuItem mnuFileUserMenu;
        private ToolStripMenuItem mnuFileView;
        private ToolStripMenuItem mnuFileEdit;
        private ToolStripMenuItem mnuFileCopy;
        private ToolStripMenuItem mnuFileMove;
        private ToolStripMenuItem mnuFileNewDir;
        private ToolStripMenuItem mnuFileRemove;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem mnuFileAtributes;
        private ToolStripSeparator toolStripMenuItem2;
        private ToolStripMenuItem mnuView;
        private ToolStripMenuItem mnuViewShort;
        private ToolStripMenuItem mnuViewDetails;
        private ToolStripMenuItem mnuViewIcons;
        private ToolStripMenuItem mnuViewThumbs;
        private ToolStripMenuItem mnuViewQuickView;
        private ToolStripMenuItem mnuViewTree;
        private ToolStripMenuItem mnuViewPCPCconnect;
        private ToolStripMenuItem mnuViewPCNETPCconnect;
        private ToolStripSeparator toolStripMenuItem3;
        private ToolStripMenuItem mnuViewByName;
        private ToolStripMenuItem mnuViewByType;
        private ToolStripMenuItem mnuViewByDate;
        private ToolStripMenuItem mnuViewBySize;
        private ToolStripSeparator toolStripMenuItem4;
        private ToolStripMenuItem mnuViewNoFilter;
        private ToolStripMenuItem mnuViewWithFilter;
        private ToolStripMenuItem mnuNavigate;
        private ToolStripMenuItem mnuNavigateTree;
        private ToolStripMenuItem mnuNavigateFind;
        private ToolStripMenuItem mnuNavigateHistory;
        private ToolStripMenuItem mnuNavigateReload;
        private ToolStripMenuItem mnuNavigateFlyTo;
        private ToolStripMenuItem mnuTools;
        private ToolStripMenuItem mnuToolsOptions;
        private ToolStripMenuItem mnuToolsPluginManager;
        private ToolStripMenuItem mnuToolsEditUserMenu;
        private ToolStripSeparator toolStripMenuItem5;
        private ToolStripMenuItem mnuToolsKeybrdHelp;
        private ToolStripMenuItem mnuToolsInfobar;
        private ToolStripMenuItem mnuToolsDiskButtons;
        private ToolStripSeparator toolStripMenuItem6;
        private ToolStripMenuItem mnuToolsConfigEdit;
        private ToolStripMenuItem mnuToolsKeychains;
        private ToolStripMenuItem mnuFileQuickSelect;
        private ToolStripMenuItem mnuFileUnselect;
        private ToolStripMenuItem mnuFileInvertSelection;
        private ToolStripMenuItem mnuFileRevertSelection;
        private ToolStripSeparator toolStripMenuItem7;
        private ToolStripMenuItem mnuFileExit;
        private ToolStripMenuItem mnuHelp;
        private ToolStripMenuItem mnuHelpHelpMe;
        private ToolStripMenuItem mnuHelpAbout;
        private ToolStripMenuItem mnuViewToolbar;
        private ToolStripSeparator toolStripMenuItem8;
        private ToolStripMenuItem mnuToolsDiskLabel;
        private ToolStripMenuItem mnuToolsSysInfo;
        private ToolStripMenuItem mnuToolsFormat;
        private ToolStripMenuItem mnuFileCompare;
        Localizator locale = new Localizator(); //объект для работы с локализациями интерфейса

		//Подпрограммы

		public frmMain() { //Инициализация элементов управления
			InitializeComponent();
		}
		
		private void frmMain_Load(object sender, EventArgs e){ //функция Form_Load()
            Application.EnableVisualStyles();
            Localize();

			#if DEBUG
                new MsgBox("File commander, версия " + Application.ProductVersion);//,"Отладочная версия",MsgBox.MsgBoxType.Information);
			#endif
            
			#region Панели
			//Формирую панели
			//Левая
			this.lplLeft.Add (new ListPanel()); //добавление в коллекцию левых панелей
			this.lplLeft[0].Location = new System.Drawing.Point(0, mstMenu.Height);
            this.lplLeft[0].Name = "lplLeft";
            this.lplLeft[0].TabIndex = 0;
			this.lplLeft[0].DoubleClick += new StringEvent(this.Panel_DblClick);
			this.lplLeft[0].GotFocus += this.Panel_Focus;
			this.lplLeft[0].BorderStyle = BorderStyle.Fixed3D;
			this.lplLeft[0].FSProvider = new localFileSystem();
            lplLeft[0].list.Columns.Add("Name", locale.GetString("FName"));
            lplLeft[0].list.Columns.Add("Size", locale.GetString("FSize"));
            lplLeft[0].list.Columns.Add("Date", locale.GetString("FDate"));
            lplLeft[0].list.DoubleClick += (object s, EventArgs ea) => { this.frmMain_KeyDown(s, new KeyEventArgs(Keys.Enter)); };
            lplLeft[0].OnURLBoxNavigate += (object s, EventArgs<string> ea) => {LoadDir(ea.Data,lplLeft[0]) ;};
            this.Controls.Add(this.lplLeft[0]); //ввожу панель в форму
			ActivePanel = this.lplLeft[0]; //и делаю её активной
			//Правая
			this.lplRight.Add (new ListPanel()); //добавление в коллекцию правых панелей
            this.lplRight[0].Location = new System.Drawing.Point(0, mstMenu.Height);
            this.lplRight[0].Name = "lplRight";
            this.lplRight[0].TabIndex = 0;
            this.lplRight[0].list.DoubleClick += (object s, EventArgs ea) => { this.frmMain_KeyDown(s, new KeyEventArgs(Keys.Enter)); };
			this.lplRight[0].GotFocus += this.Panel_Focus;
			this.lplRight[0].BorderStyle = BorderStyle.Fixed3D;
			this.lplRight[0].FSProvider = new localFileSystem();
            lplRight[0].list.Columns.Add("Name", locale.GetString("FName"));
            lplRight[0].list.Columns.Add("Size", locale.GetString("FSize"));
            lplRight[0].list.Columns.Add("Date", locale.GetString("FDate"));
            lplRight[0].OnURLBoxNavigate += (object s, EventArgs<string> ea) => { LoadDir(ea.Data, lplRight[0]); };
            this.Controls.Add(this.lplRight[0]); //ввожу панель в форму
			PassivePanel = this.lplRight[0];
			#endregion

            tsKeyboard.Visible = fcmd.Properties.Settings.Default.ShowKeybrdHelp;

            mnuToolsKeybrdHelp.Checked = fcmd.Properties.Settings.Default.ShowKeybrdHelp;
            mnuToolsDiskButtons.Checked = fcmd.Properties.Settings.Default.ShowDiskList;
            mnuToolsInfobar.Checked = fcmd.Properties.Settings.Default.ShowFileInfo;

			#region Изначальный перечень файлов
			string startupDir = "file://" + Directory.GetLogicalDrives()[0];
            ActivePanel = lplRight[0];
            Ls(startupDir);
            ActivePanel = lplLeft[0];
            Ls("file://" + Application.StartupPath + "/../../");
			#endregion

			this.OnSizeChanged (new EventArgs()); //расстановка панелей по местам

            //initialize both XWT and WinForms toolkits (через жопу)
            this.Show(); this.Select(); //h a c k
            Xwt.Application.Run();
		}

		private void frmMain_Resize(object sender, EventArgs e){ //Деформация формы
            //SEE GITHUB BUG #3 - https://github.com/atauenis/fcmd/issues/3
            //don't forget to make code for more goodly filling the window with panel
            //when keyboard help bar is hided
            foreach (ListPanel llp in this.lplLeft)
            {
                int height = this.Height - ActivePanel.Top;
                height = height - mstMenu.Height;
                height = height - tsKeyboard.Height * 5 / 3; //hack
                llp.Size = new Size(this.Width / 2, height);
            }
            foreach (ListPanel rlp in this.lplRight)
            {
                int height = this.Height - ActivePanel.Top;
                height = height - mstMenu.Height;
                height = height - tsKeyboard.Height * 5 / 3; //hack
                rlp.Size = new Size(this.Width / 2, height);
                rlp.Left = this.Width / 2;
            }
            //tsKeyboard.Visible = false; //debug
		}

		private void frmMain_KeyDown(object sender, KeyEventArgs e){//нажатие клавиши клавиатуры
#if DEBUG
			string DebugText = "Отладка клавиатуры: нажата и отпущена клавиша " + e.KeyCode.ToString();
			DebugText += ", модификаторы: Ctrl=" + e.Control.ToString();
			DebugText += " Alt=" + e.Alt.ToString();
			DebugText += " Shift=" + e.Shift.ToString();
			Console.WriteLine(DebugText);
#endif
            if (ActivePanel.list.SelectedItems.Count == 0 || e.KeyData == Keys.F10 || e.KeyData == Keys.F1 || e.KeyData == Keys.F2)
            {//выполняю операции, не связанные с файлами
                switch (e.KeyData)
                {
                    case Keys.F7: //новый каталог
                        InputBox ibx = new InputBox(locale.GetString("NewDirURL"), ActivePanel.FSProvider.CurrentDirectory + locale.GetString("NewDirTemplate"));
                        if (ibx.ShowDialog())
                        {
                            MkDir(ibx.Result);
                        }
                        break;
                    case Keys.F10: //выход
                        Application.Exit();
                        break;
                }
                return; //далее не выполнять код
            }

            switch (e.KeyData){
                case Keys.Enter: //переход
                    if (ActivePanel.FSProvider.FileExists(ActivePanel.list.SelectedItems[0].Tag.ToString()))
                    {//файл
                        Process proc = new Process();
                        proc.StartInfo.FileName = ActivePanel.list.SelectedItems[0].Tag.ToString();
                        proc.StartInfo.UseShellExecute = true;
                        proc.Start();
                        break;
                    }
                    
                    if (ActivePanel.FSProvider.DirectoryExists(ActivePanel.list.SelectedItems[0].Tag.ToString()))
                    {//каталог
                        Ls(ActivePanel.list.SelectedItems[0].Tag.ToString());
                        return;
                    }
                    
                    break;
                case Keys.F3: //просмотр
                    VEd fcv = new VEd();
					pluginner.IFSPlugin fs = ActivePanel.FSProvider;
                    if (fs.DirectoryExists(ActivePanel.list.SelectedItems[0].Tag.ToString()))
                    {
                        MessageBox.Show(string.Format(locale.GetString("ItsDir"), ActivePanel.list.SelectedItems[0].Text), "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }

                    if (e.Shift == true) //не вызывается. почему? разобраться!
                    { fcv.LoadFile(ActivePanel.list.SelectedItems[0].Tag.ToString(),ActivePanel.FSProvider,new base_plugins.ve.PlainText(),false);
                    }
                    else FCView(ActivePanel.list.SelectedItems[0].Tag.ToString());
                    break;
                case Keys.F4: //правка
                    VEd fce = new VEd();
                    if (ActivePanel.FSProvider.DirectoryExists(ActivePanel.list.SelectedItems[0].Tag.ToString()))
                    {
                        MessageBox.Show(string.Format(locale.GetString("ItsDir"), ActivePanel.list.SelectedItems[0].Text), "", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return;
                    }
                    fce.LoadFile(ActivePanel.list.SelectedItems[0].Tag.ToString(), ActivePanel.FSProvider, true);
                    fce.Show();
                    break;
                case Keys.F5: //копировать
                    Cp();
                    break;
                case Keys.F6: //перенести/переименовать
                    Mv();
                    break;
                case Keys.F8: //удалить
                    Rm(ActivePanel.list.SelectedItems[0].Tag.ToString());
                    break;
            }
		}

		private void Panel_Focus(object sender, EventArgs e){ //панель получила фокус
			ListPanel lp = (ListPanel) sender;
			if(lp.Name != ActivePanel.Name){
				PassivePanel = ActivePanel;
				ActivePanel = lp;
			}
			ActivePanel = (ListPanel)sender;
#if DEBUG
			Console.WriteLine("Отладка слежения за панелями: " + ActivePanel.Name + " <активная~~~пассивная> " + PassivePanel.Name);
#endif
		}

        private void Panel_DblClick(object sender, EventArgs<String> e){ //двойной щелчок по панели
			ListPanel lp = (ListPanel)sender;
            if (lp.FSProvider.DirectoryExists(e.Data))
            {
                //это - каталог, грузить можно
                LoadDir(e.Data, (ListPanel)sender);
            }
            else
            {
                Process proc = new Process();
                proc.StartInfo.FileName = e.Data;
                proc.StartInfo.UseShellExecute = true;
                proc.Start();
            }
        }

		private void mstMenu_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e){
		
		}

        /// <summary>
        /// Loads the directory.
        /// </summary>
        /// <param name='url'>
        /// URL.
        /// </param>
        /// <param name='lp'>
        /// ListPanel, в которую надобно захуячить директорию
        /// </param>
        public void LoadDir(string url, ListPanel lp)
        {
            lp.lblPath.Text = url;
            int Status = 0;
            Thread LsThread = new Thread(delegate() { DoLs(url, lp, ref Status); });
            FileProcessDialog fpd = new FileProcessDialog();
            fpd.Y = this.Top + ActivePanel.Top;
            fpd.X = this.Left + ActivePanel.Left;
            string FPDtext = String.Format(locale.GetString("DoingListdir"), "\n" + url, "");
            FPDtext = FPDtext.Replace("{1}", "");
            fpd.lblStatus.Text = FPDtext;

            fpd.Show();
            LsThread.Start();

            do { Application.DoEvents(); fpd.pbrProgress.Fraction = Status; }
            while (LsThread.ThreadState == System.Threading.ThreadState.Running);
            fpd.Hide();
        }

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.mstMenu = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileUserMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileCompare = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileMove = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileNewDir = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileAtributes = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileQuickSelect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileUnselect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileInvertSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileRevertSelection = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem7 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewShort = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewDetails = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewIcons = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewThumbs = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewQuickView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewTree = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewPCPCconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewPCNETPCconnect = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewByName = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewByType = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewByDate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewBySize = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuViewNoFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewWithFilter = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewToolbar = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNavigate = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNavigateTree = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNavigateFind = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNavigateHistory = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNavigateReload = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNavigateFlyTo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsPluginManager = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsEditUserMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsKeychains = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsKeybrdHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsInfobar = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsDiskButtons = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem6 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsConfigEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem8 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuToolsDiskLabel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsFormat = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsSysInfo = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpHelpMe = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.tsKeyboard = new System.Windows.Forms.ToolStrip();
            this.tsbHelpF1 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF2 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF3 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF4 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF5 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF6 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF7 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF8 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF9 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF10 = new System.Windows.Forms.ToolStripButton();
            this.mstMenu.SuspendLayout();
            this.tsKeyboard.SuspendLayout();
            this.SuspendLayout();
            // 
            // mstMenu
            // 
            this.mstMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuView,
            this.mnuNavigate,
            this.mnuTools,
            this.mnuHelp});
            this.mstMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mstMenu.Location = new System.Drawing.Point(0, 0);
            this.mstMenu.Name = "mstMenu";
            this.mstMenu.Size = new System.Drawing.Size(667, 23);
            this.mstMenu.TabIndex = 0;
            this.mstMenu.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileUserMenu,
            this.mnuFileView,
            this.mnuFileEdit,
            this.mnuFileCompare,
            this.mnuFileCopy,
            this.mnuFileMove,
            this.mnuFileNewDir,
            this.mnuFileRemove,
            this.toolStripMenuItem1,
            this.mnuFileAtributes,
            this.toolStripMenuItem2,
            this.mnuFileQuickSelect,
            this.mnuFileUnselect,
            this.mnuFileInvertSelection,
            this.mnuFileRevertSelection,
            this.toolStripMenuItem7,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(37, 19);
            this.mnuFile.Text = "File";
            // 
            // mnuFileUserMenu
            // 
            this.mnuFileUserMenu.Enabled = false;
            this.mnuFileUserMenu.Name = "mnuFileUserMenu";
            this.mnuFileUserMenu.Size = new System.Drawing.Size(169, 22);
            this.mnuFileUserMenu.Text = "User menu";
            this.mnuFileUserMenu.Click += new System.EventHandler(this.mnuFileUserMenu_Click);
            // 
            // mnuFileView
            // 
            this.mnuFileView.Name = "mnuFileView";
            this.mnuFileView.Size = new System.Drawing.Size(169, 22);
            this.mnuFileView.Text = "View the file";
            this.mnuFileView.Click += new System.EventHandler(this.mnuFileView_Click);
            // 
            // mnuFileEdit
            // 
            this.mnuFileEdit.Enabled = false;
            this.mnuFileEdit.Name = "mnuFileEdit";
            this.mnuFileEdit.Size = new System.Drawing.Size(169, 22);
            this.mnuFileEdit.Text = "Edit the file";
            this.mnuFileEdit.Click += new System.EventHandler(this.mnuFileEdit_Click);
            // 
            // mnuFileCompare
            // 
            this.mnuFileCompare.Enabled = false;
            this.mnuFileCompare.Name = "mnuFileCompare";
            this.mnuFileCompare.Size = new System.Drawing.Size(169, 22);
            this.mnuFileCompare.Text = "Compare";
            // 
            // mnuFileCopy
            // 
            this.mnuFileCopy.Name = "mnuFileCopy";
            this.mnuFileCopy.Size = new System.Drawing.Size(169, 22);
            this.mnuFileCopy.Text = "Copy";
            this.mnuFileCopy.Click += new System.EventHandler(this.mnuFileCopy_Click);
            // 
            // mnuFileMove
            // 
            this.mnuFileMove.Name = "mnuFileMove";
            this.mnuFileMove.Size = new System.Drawing.Size(169, 22);
            this.mnuFileMove.Text = "Move or rename";
            this.mnuFileMove.Click += new System.EventHandler(this.mnuFileMove_Click);
            // 
            // mnuFileNewDir
            // 
            this.mnuFileNewDir.Name = "mnuFileNewDir";
            this.mnuFileNewDir.Size = new System.Drawing.Size(169, 22);
            this.mnuFileNewDir.Text = "New directory";
            this.mnuFileNewDir.Click += new System.EventHandler(this.mnuFileNewDir_Click);
            // 
            // mnuFileRemove
            // 
            this.mnuFileRemove.Name = "mnuFileRemove";
            this.mnuFileRemove.Size = new System.Drawing.Size(169, 22);
            this.mnuFileRemove.Text = "Delete";
            this.mnuFileRemove.Click += new System.EventHandler(this.mnuFileRemove_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(166, 6);
            // 
            // mnuFileAtributes
            // 
            this.mnuFileAtributes.Enabled = false;
            this.mnuFileAtributes.Name = "mnuFileAtributes";
            this.mnuFileAtributes.Size = new System.Drawing.Size(169, 22);
            this.mnuFileAtributes.Text = "Set attribbuttes";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(166, 6);
            // 
            // mnuFileQuickSelect
            // 
            this.mnuFileQuickSelect.Name = "mnuFileQuickSelect";
            this.mnuFileQuickSelect.Size = new System.Drawing.Size(169, 22);
            this.mnuFileQuickSelect.Text = "Select file...";
            this.mnuFileQuickSelect.Click += new System.EventHandler(this.mnuFileQuickSelect_Click);
            // 
            // mnuFileUnselect
            // 
            this.mnuFileUnselect.Name = "mnuFileUnselect";
            this.mnuFileUnselect.Size = new System.Drawing.Size(169, 22);
            this.mnuFileUnselect.Text = "Cancel selection";
            this.mnuFileUnselect.Click += new System.EventHandler(this.mnuFileUnselect_Click);
            // 
            // mnuFileInvertSelection
            // 
            this.mnuFileInvertSelection.Name = "mnuFileInvertSelection";
            this.mnuFileInvertSelection.Size = new System.Drawing.Size(169, 22);
            this.mnuFileInvertSelection.Text = "Invert selection";
            this.mnuFileInvertSelection.Click += new System.EventHandler(this.mnuFileInvertSelection_Click);
            // 
            // mnuFileRevertSelection
            // 
            this.mnuFileRevertSelection.Enabled = false;
            this.mnuFileRevertSelection.Name = "mnuFileRevertSelection";
            this.mnuFileRevertSelection.Size = new System.Drawing.Size(169, 22);
            this.mnuFileRevertSelection.Text = "Rollback selection";
            // 
            // toolStripMenuItem7
            // 
            this.toolStripMenuItem7.Name = "toolStripMenuItem7";
            this.toolStripMenuItem7.Size = new System.Drawing.Size(166, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.Size = new System.Drawing.Size(169, 22);
            this.mnuFileExit.Text = "Exit";
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewShort,
            this.mnuViewDetails,
            this.mnuViewIcons,
            this.mnuViewThumbs,
            this.mnuViewQuickView,
            this.mnuViewTree,
            this.mnuViewPCPCconnect,
            this.mnuViewPCNETPCconnect,
            this.toolStripMenuItem3,
            this.mnuViewByName,
            this.mnuViewByType,
            this.mnuViewByDate,
            this.mnuViewBySize,
            this.toolStripMenuItem4,
            this.mnuViewNoFilter,
            this.mnuViewWithFilter,
            this.mnuViewToolbar});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(44, 19);
            this.mnuView.Text = "View";
            // 
            // mnuViewShort
            // 
            this.mnuViewShort.Name = "mnuViewShort";
            this.mnuViewShort.Size = new System.Drawing.Size(208, 22);
            this.mnuViewShort.Text = "Short";
            this.mnuViewShort.Click += new System.EventHandler(this.mnuViewShort_Click);
            // 
            // mnuViewDetails
            // 
            this.mnuViewDetails.Name = "mnuViewDetails";
            this.mnuViewDetails.Size = new System.Drawing.Size(208, 22);
            this.mnuViewDetails.Text = "Details";
            this.mnuViewDetails.Click += new System.EventHandler(this.mnuViewDetails_Click);
            // 
            // mnuViewIcons
            // 
            this.mnuViewIcons.Name = "mnuViewIcons";
            this.mnuViewIcons.Size = new System.Drawing.Size(208, 22);
            this.mnuViewIcons.Text = "Icons";
            this.mnuViewIcons.Click += new System.EventHandler(this.mnuViewIcons_Click);
            // 
            // mnuViewThumbs
            // 
            this.mnuViewThumbs.Enabled = false;
            this.mnuViewThumbs.Name = "mnuViewThumbs";
            this.mnuViewThumbs.Size = new System.Drawing.Size(208, 22);
            this.mnuViewThumbs.Text = "Thumbanials";
            // 
            // mnuViewQuickView
            // 
            this.mnuViewQuickView.Enabled = false;
            this.mnuViewQuickView.Name = "mnuViewQuickView";
            this.mnuViewQuickView.Size = new System.Drawing.Size(208, 22);
            this.mnuViewQuickView.Text = "Quick view";
            // 
            // mnuViewTree
            // 
            this.mnuViewTree.Enabled = false;
            this.mnuViewTree.Name = "mnuViewTree";
            this.mnuViewTree.Size = new System.Drawing.Size(208, 22);
            this.mnuViewTree.Text = "Directory tree";
            // 
            // mnuViewPCPCconnect
            // 
            this.mnuViewPCPCconnect.Enabled = false;
            this.mnuViewPCPCconnect.Name = "mnuViewPCPCconnect";
            this.mnuViewPCPCconnect.Size = new System.Drawing.Size(208, 22);
            this.mnuViewPCPCconnect.Text = "PC-PC file exchange";
            // 
            // mnuViewPCNETPCconnect
            // 
            this.mnuViewPCNETPCconnect.Enabled = false;
            this.mnuViewPCNETPCconnect.Name = "mnuViewPCNETPCconnect";
            this.mnuViewPCNETPCconnect.Size = new System.Drawing.Size(208, 22);
            this.mnuViewPCNETPCconnect.Text = "PC-NET-PC file exchange";
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(205, 6);
            // 
            // mnuViewByName
            // 
            this.mnuViewByName.Enabled = false;
            this.mnuViewByName.Name = "mnuViewByName";
            this.mnuViewByName.Size = new System.Drawing.Size(208, 22);
            this.mnuViewByName.Text = "By name";
            // 
            // mnuViewByType
            // 
            this.mnuViewByType.Enabled = false;
            this.mnuViewByType.Name = "mnuViewByType";
            this.mnuViewByType.Size = new System.Drawing.Size(208, 22);
            this.mnuViewByType.Text = "By .extension";
            // 
            // mnuViewByDate
            // 
            this.mnuViewByDate.Enabled = false;
            this.mnuViewByDate.Name = "mnuViewByDate";
            this.mnuViewByDate.Size = new System.Drawing.Size(208, 22);
            this.mnuViewByDate.Text = "By date";
            // 
            // mnuViewBySize
            // 
            this.mnuViewBySize.Enabled = false;
            this.mnuViewBySize.Name = "mnuViewBySize";
            this.mnuViewBySize.Size = new System.Drawing.Size(208, 22);
            this.mnuViewBySize.Text = "By size";
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(205, 6);
            // 
            // mnuViewNoFilter
            // 
            this.mnuViewNoFilter.Enabled = false;
            this.mnuViewNoFilter.Name = "mnuViewNoFilter";
            this.mnuViewNoFilter.Size = new System.Drawing.Size(208, 22);
            this.mnuViewNoFilter.Text = "Without filter";
            // 
            // mnuViewWithFilter
            // 
            this.mnuViewWithFilter.Enabled = false;
            this.mnuViewWithFilter.Name = "mnuViewWithFilter";
            this.mnuViewWithFilter.Size = new System.Drawing.Size(208, 22);
            this.mnuViewWithFilter.Text = "Apply a filter";
            // 
            // mnuViewToolbar
            // 
            this.mnuViewToolbar.Enabled = false;
            this.mnuViewToolbar.Name = "mnuViewToolbar";
            this.mnuViewToolbar.Size = new System.Drawing.Size(208, 22);
            this.mnuViewToolbar.Text = "Toolbar";
            // 
            // mnuNavigate
            // 
            this.mnuNavigate.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNavigateTree,
            this.mnuNavigateFind,
            this.mnuNavigateHistory,
            this.mnuNavigateReload,
            this.mnuNavigateFlyTo});
            this.mnuNavigate.Name = "mnuNavigate";
            this.mnuNavigate.Size = new System.Drawing.Size(77, 19);
            this.mnuNavigate.Text = "Navigation";
            // 
            // mnuNavigateTree
            // 
            this.mnuNavigateTree.Enabled = false;
            this.mnuNavigateTree.Name = "mnuNavigateTree";
            this.mnuNavigateTree.Size = new System.Drawing.Size(232, 22);
            this.mnuNavigateTree.Text = "Directory tree";
            // 
            // mnuNavigateFind
            // 
            this.mnuNavigateFind.Enabled = false;
            this.mnuNavigateFind.Name = "mnuNavigateFind";
            this.mnuNavigateFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F7)));
            this.mnuNavigateFind.Size = new System.Drawing.Size(232, 22);
            this.mnuNavigateFind.Text = "Find...";
            // 
            // mnuNavigateHistory
            // 
            this.mnuNavigateHistory.Enabled = false;
            this.mnuNavigateHistory.Name = "mnuNavigateHistory";
            this.mnuNavigateHistory.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.Down)));
            this.mnuNavigateHistory.Size = new System.Drawing.Size(232, 22);
            this.mnuNavigateHistory.Text = "Navigation history";
            // 
            // mnuNavigateReload
            // 
            this.mnuNavigateReload.Name = "mnuNavigateReload";
            this.mnuNavigateReload.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.mnuNavigateReload.Size = new System.Drawing.Size(232, 22);
            this.mnuNavigateReload.Text = "Reload";
            this.mnuNavigateReload.Click += new System.EventHandler(this.mnuNavigateReload_Click);
            // 
            // mnuNavigateFlyTo
            // 
            this.mnuNavigateFlyTo.Enabled = false;
            this.mnuNavigateFlyTo.Name = "mnuNavigateFlyTo";
            this.mnuNavigateFlyTo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Space)));
            this.mnuNavigateFlyTo.Size = new System.Drawing.Size(232, 22);
            this.mnuNavigateFlyTo.Text = "Quickly go to...";
            // 
            // mnuTools
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsOptions,
            this.mnuToolsPluginManager,
            this.mnuToolsEditUserMenu,
            this.mnuToolsKeychains,
            this.toolStripMenuItem5,
            this.mnuToolsKeybrdHelp,
            this.mnuToolsInfobar,
            this.mnuToolsDiskButtons,
            this.toolStripMenuItem6,
            this.mnuToolsConfigEdit,
            this.toolStripMenuItem8,
            this.mnuToolsDiskLabel,
            this.mnuToolsFormat,
            this.mnuToolsSysInfo});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(48, 19);
            this.mnuTools.Text = "Tools";
            // 
            // mnuToolsOptions
            // 
            this.mnuToolsOptions.Name = "mnuToolsOptions";
            this.mnuToolsOptions.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsOptions.Text = "Settings...";
            this.mnuToolsOptions.Click += new System.EventHandler(this.mnuToolsOptions_Click);
            // 
            // mnuToolsPluginManager
            // 
            this.mnuToolsPluginManager.Enabled = false;
            this.mnuToolsPluginManager.Name = "mnuToolsPluginManager";
            this.mnuToolsPluginManager.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsPluginManager.Text = "Add-on manager...";
            // 
            // mnuToolsEditUserMenu
            // 
            this.mnuToolsEditUserMenu.Enabled = false;
            this.mnuToolsEditUserMenu.Name = "mnuToolsEditUserMenu";
            this.mnuToolsEditUserMenu.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsEditUserMenu.Text = "Edit user menu...";
            // 
            // mnuToolsKeychains
            // 
            this.mnuToolsKeychains.Enabled = false;
            this.mnuToolsKeychains.Name = "mnuToolsKeychains";
            this.mnuToolsKeychains.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsKeychains.Text = "Keychains";
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(205, 6);
            // 
            // mnuToolsKeybrdHelp
            // 
            this.mnuToolsKeybrdHelp.CheckOnClick = true;
            this.mnuToolsKeybrdHelp.Name = "mnuToolsKeybrdHelp";
            this.mnuToolsKeybrdHelp.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsKeybrdHelp.Text = "Show keyboard help";
            this.mnuToolsKeybrdHelp.Click += new System.EventHandler(this.mnuToolsKeybrdHelp_Click);
            // 
            // mnuToolsInfobar
            // 
            this.mnuToolsInfobar.CheckOnClick = true;
            this.mnuToolsInfobar.Name = "mnuToolsInfobar";
            this.mnuToolsInfobar.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsInfobar.Text = "Show info bars";
            this.mnuToolsInfobar.Click += new System.EventHandler(this.mnuToolsInfobar_Click);
            // 
            // mnuToolsDiskButtons
            // 
            this.mnuToolsDiskButtons.CheckOnClick = true;
            this.mnuToolsDiskButtons.Name = "mnuToolsDiskButtons";
            this.mnuToolsDiskButtons.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsDiskButtons.Text = "Show disk switch buttons";
            this.mnuToolsDiskButtons.Click += new System.EventHandler(this.mnuToolsDiskButtons_Click);
            // 
            // toolStripMenuItem6
            // 
            this.toolStripMenuItem6.Name = "toolStripMenuItem6";
            this.toolStripMenuItem6.Size = new System.Drawing.Size(205, 6);
            // 
            // mnuToolsConfigEdit
            // 
            this.mnuToolsConfigEdit.Enabled = false;
            this.mnuToolsConfigEdit.Name = "mnuToolsConfigEdit";
            this.mnuToolsConfigEdit.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsConfigEdit.Text = "Edit the FC\'s config";
            // 
            // toolStripMenuItem8
            // 
            this.toolStripMenuItem8.Name = "toolStripMenuItem8";
            this.toolStripMenuItem8.Size = new System.Drawing.Size(205, 6);
            // 
            // mnuToolsDiskLabel
            // 
            this.mnuToolsDiskLabel.Enabled = false;
            this.mnuToolsDiskLabel.Name = "mnuToolsDiskLabel";
            this.mnuToolsDiskLabel.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsDiskLabel.Text = "Disk label";
            // 
            // mnuToolsFormat
            // 
            this.mnuToolsFormat.Enabled = false;
            this.mnuToolsFormat.Name = "mnuToolsFormat";
            this.mnuToolsFormat.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsFormat.Text = "Format disk...";
            // 
            // mnuToolsSysInfo
            // 
            this.mnuToolsSysInfo.Enabled = false;
            this.mnuToolsSysInfo.Name = "mnuToolsSysInfo";
            this.mnuToolsSysInfo.Size = new System.Drawing.Size(208, 22);
            this.mnuToolsSysInfo.Text = "System info";
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuHelpHelpMe,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(44, 19);
            this.mnuHelp.Text = "Help";
            // 
            // mnuHelpHelpMe
            // 
            this.mnuHelpHelpMe.Enabled = false;
            this.mnuHelpHelpMe.Name = "mnuHelpHelpMe";
            this.mnuHelpHelpMe.Size = new System.Drawing.Size(116, 22);
            this.mnuHelpHelpMe.Text = "FC help";
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(116, 22);
            this.mnuHelpAbout.Text = "About...";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // tsKeyboard
            // 
            this.tsKeyboard.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.tsKeyboard.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsKeyboard.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbHelpF1,
            this.tsbHelpF2,
            this.tsbHelpF3,
            this.tsbHelpF4,
            this.tsbHelpF5,
            this.tsbHelpF6,
            this.tsbHelpF7,
            this.tsbHelpF8,
            this.tsbHelpF9,
            this.tsbHelpF10});
            this.tsKeyboard.Location = new System.Drawing.Point(0, 385);
            this.tsKeyboard.Name = "tsKeyboard";
            this.tsKeyboard.Size = new System.Drawing.Size(667, 25);
            this.tsKeyboard.TabIndex = 1;
            this.tsKeyboard.Text = "toolStrip1";
            this.tsKeyboard.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsKeyboard_ItemClicked);
            // 
            // tsbHelpF1
            // 
            this.tsbHelpF1.AutoToolTip = false;
            this.tsbHelpF1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF1.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF1.Image")));
            this.tsbHelpF1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF1.Name = "tsbHelpF1";
            this.tsbHelpF1.Size = new System.Drawing.Size(51, 22);
            this.tsbHelpF1.Text = "F1 Help";
            // 
            // tsbHelpF2
            // 
            this.tsbHelpF2.AutoToolTip = false;
            this.tsbHelpF2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF2.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF2.Image")));
            this.tsbHelpF2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF2.Name = "tsbHelpF2";
            this.tsbHelpF2.Size = new System.Drawing.Size(57, 22);
            this.tsbHelpF2.Text = "F2 Menu";
            // 
            // tsbHelpF3
            // 
            this.tsbHelpF3.AutoToolTip = false;
            this.tsbHelpF3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF3.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF3.Image")));
            this.tsbHelpF3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF3.Name = "tsbHelpF3";
            this.tsbHelpF3.Size = new System.Drawing.Size(51, 22);
            this.tsbHelpF3.Text = "F3 View";
            // 
            // tsbHelpF4
            // 
            this.tsbHelpF4.AutoToolTip = false;
            this.tsbHelpF4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF4.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF4.Image")));
            this.tsbHelpF4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF4.Name = "tsbHelpF4";
            this.tsbHelpF4.Size = new System.Drawing.Size(46, 22);
            this.tsbHelpF4.Text = "F4 Edit";
            // 
            // tsbHelpF5
            // 
            this.tsbHelpF5.AutoToolTip = false;
            this.tsbHelpF5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF5.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF5.Image")));
            this.tsbHelpF5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF5.Name = "tsbHelpF5";
            this.tsbHelpF5.Size = new System.Drawing.Size(54, 22);
            this.tsbHelpF5.Text = "F5 Copy";
            // 
            // tsbHelpF6
            // 
            this.tsbHelpF6.AutoToolTip = false;
            this.tsbHelpF6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF6.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF6.Image")));
            this.tsbHelpF6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF6.Name = "tsbHelpF6";
            this.tsbHelpF6.Size = new System.Drawing.Size(56, 22);
            this.tsbHelpF6.Text = "F6 Move";
            // 
            // tsbHelpF7
            // 
            this.tsbHelpF7.AutoToolTip = false;
            this.tsbHelpF7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF7.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF7.Image")));
            this.tsbHelpF7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF7.Name = "tsbHelpF7";
            this.tsbHelpF7.Size = new System.Drawing.Size(67, 22);
            this.tsbHelpF7.Text = "F7 New dir";
            // 
            // tsbHelpF8
            // 
            this.tsbHelpF8.AutoToolTip = false;
            this.tsbHelpF8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF8.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF8.Image")));
            this.tsbHelpF8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF8.Name = "tsbHelpF8";
            this.tsbHelpF8.Size = new System.Drawing.Size(59, 22);
            this.tsbHelpF8.Text = "F8 Delete";
            // 
            // tsbHelpF9
            // 
            this.tsbHelpF9.AutoToolTip = false;
            this.tsbHelpF9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF9.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF9.Image")));
            this.tsbHelpF9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF9.Name = "tsbHelpF9";
            this.tsbHelpF9.Size = new System.Drawing.Size(57, 22);
            this.tsbHelpF9.Text = "F9 Menu";
            // 
            // tsbHelpF10
            // 
            this.tsbHelpF10.AutoToolTip = false;
            this.tsbHelpF10.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF10.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF10.Image")));
            this.tsbHelpF10.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF10.Name = "tsbHelpF10";
            this.tsbHelpF10.Size = new System.Drawing.Size(50, 22);
            this.tsbHelpF10.Text = "F10 Exit";
            // 
            // frmMain
            // 
            this.ClientSize = new System.Drawing.Size(667, 410);
            this.Controls.Add(this.tsKeyboard);
            this.Controls.Add(this.mstMenu);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mstMenu;
            this.Name = "frmMain";
            this.Text = "File Commander";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmMain_FormClosed);
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.mstMenu.ResumeLayout(false);
            this.mstMenu.PerformLayout();
            this.tsKeyboard.ResumeLayout(false);
            this.tsKeyboard.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void tsKeyboard_ItemClicked(object sender, ToolStripItemClickedEventArgs e){
            switch (e.ClickedItem.Name){
                case "tsbHelpF1": this.OnKeyDown(new KeyEventArgs(Keys.F1)); break;
                case "tsbHelpF2": this.OnKeyDown(new KeyEventArgs(Keys.F2)); break;
                case "tsbHelpF3": this.OnKeyDown(new KeyEventArgs(Keys.F3)); break;
                case "tsbHelpF4": this.OnKeyDown(new KeyEventArgs(Keys.F4)); break;
                case "tsbHelpF5": this.OnKeyDown(new KeyEventArgs(Keys.F5)); break;
                case "tsbHelpF6": this.OnKeyDown(new KeyEventArgs(Keys.F6)); break;
                case "tsbHelpF7": this.OnKeyDown(new KeyEventArgs(Keys.F7)); break;
                case "tsbHelpF8": this.OnKeyDown(new KeyEventArgs(Keys.F8)); break;
                case "tsbHelpF9": this.OnKeyDown(new KeyEventArgs(Keys.F9)); break;
                case "tsbHelpF10": this.OnKeyDown(new KeyEventArgs(Keys.F10)); break;
            }
        }

        /// <summary>
        /// Translate the form's labels to the current language
        /// </summary>
        private void Localize(){
            //keyboard help
            tsbHelpF1.Text = locale.GetString("FCF1");
            tsbHelpF2.Text = locale.GetString("FCF2");
            tsbHelpF3.Text = locale.GetString("FCF3");
            tsbHelpF4.Text = locale.GetString("FCF4");
            tsbHelpF5.Text = locale.GetString("FCF5");
            tsbHelpF6.Text = locale.GetString("FCF6");
            tsbHelpF7.Text = locale.GetString("FCF7");
            tsbHelpF8.Text = locale.GetString("FCF8");
            tsbHelpF9.Text = locale.GetString("FCF9");
            tsbHelpF10.Text = locale.GetString("FCF10");

            //menus. first level menuitems will be translated automatically
            //       other levels must be translated manually.

            mnuFile.Text = locale.GetString("FCmnuFile");
            foreach (ToolStripItem mnuFileItem in mnuFile.DropDownItems){
                if(mnuFileItem.GetType() != (new ToolStripSeparator()).GetType())//required to filter separators
                    mnuFileItem.Text = locale.GetString("FC" + mnuFileItem.Name);
            }

            mnuView.Text = locale.GetString("FCmnuView");
            foreach (ToolStripItem mnuViewItem in mnuView.DropDownItems){
                if(mnuViewItem.GetType() != (new ToolStripSeparator()).GetType())//required to filter separators
                    mnuViewItem.Text = locale.GetString("FC" + mnuViewItem.Name);
            }

            mnuNavigate.Text = locale.GetString("FCmnuNav");
            foreach (ToolStripItem mnuNavItem in mnuNavigate.DropDownItems)
            {
                if(mnuNavItem.GetType() != (new ToolStripSeparator()).GetType())//required to filter separators
                    mnuNavItem.Text = locale.GetString("FC" + mnuNavItem.Name);
            }

            mnuTools.Text = locale.GetString("FCmnuTools");
            foreach (ToolStripItem mnuTItem in mnuTools.DropDownItems)
            {
                if (mnuTItem.GetType() != (new ToolStripSeparator()).GetType())//required to filter separators
                    mnuTItem.Text = locale.GetString("FC" + mnuTItem.Name);
            }

            mnuHelp.Text = locale.GetString("FCmnuHelp");
            mnuHelpHelpMe.Text = locale.GetString("FCmnuHelpHelpMe");
            mnuHelpAbout.Text = locale.GetString("FCmnuHelpAbout");
        }

        /// <summary>
        /// Adds a new item <paramref name="NewItem"/> into the ListPanel <paramref name="lp"/>
        /// </summary>
        /// <param name="lp"></param>
        /// <param name="NewItem"></param>
        private void AddItem(ListPanel lp, ListViewItem NewItem){
            CheckForIllegalCrossThreadCalls = false; //HACK: заменить на долбанные делегации и прочую нетовскую муть
            lp.list.Items.Add(NewItem);
        }

        /// <summary>
        /// Converts the file size (in bytes) to human-readable string
        /// </summary>
        private string KiloMegaGigabyteConvert(long Input){
            const int Kibi = 1024; //to be used when a maths-knowledged contributor changed these "magic numbers" to equations of 1024
            //необходимо заменить магические числа на формулы с применением константы 1024 (Kibi)

            if (Input > 1099511627776) return (Input / 1099511627776).ToString() + " TB"; //terabyte
            if (Input > 1073741824) return (Input / 1073741824).ToString() + " GB"; //gigabyte
            if (Input > 1048576) return (Input / 1048576).ToString() + " MB"; //megabyte
            if (Input > 1024) return (Input / Kibi).ToString() + " KB"; //kilobyte
            
            return Input.ToString() + " B"; //byte (if Input less than 1024)
        }

        private void mnuFileUserMenu_Click(object sender, EventArgs e)
        {
            OnKeyDown(new KeyEventArgs(Keys.F2));
        }

        private void mnuFileView_Click(object sender, EventArgs e)
        {
            OnKeyDown(new KeyEventArgs(Keys.F3));
        }

        private void mnuFileEdit_Click(object sender, EventArgs e)
        {
            OnKeyDown(new KeyEventArgs(Keys.F4));
        }

        private void mnuFileCopy_Click(object sender, EventArgs e)
        {
            OnKeyDown(new KeyEventArgs(Keys.F5));
        }

        private void mnuFileMove_Click(object sender, EventArgs e)
        {
            OnKeyDown(new KeyEventArgs(Keys.F6));
        }

        private void mnuFileNewDir_Click(object sender, EventArgs e)
        {
            OnKeyDown(new KeyEventArgs(Keys.F7));
        }

        private void mnuFileRemove_Click(object sender, EventArgs e)
        {
            OnKeyDown(new KeyEventArgs(Keys.F8));
        }

        private void mnuViewShort_Click(object sender, EventArgs e){//view-small icons
            ActivePanel.list.View = View.List;
        }

        private void mnuViewDetails_Click(object sender, EventArgs e){//view-table
            ActivePanel.list.View = View.Details;
        }

        private void mnuViewIcons_Click(object sender, EventArgs e){//view-large icons
            ActivePanel.list.View = View.LargeIcon;
        }

        private void mnuFileUnselect_Click(object sender, EventArgs e){ //file-unselect all
            foreach (ListViewItem Tobeunselected in ActivePanel.list.SelectedItems){
                Tobeunselected.Selected = false;
            }
        }

        private void mnuFileQuickSelect_Click(object sender, EventArgs e){//file-quick select
            InputBox ibx = new InputBox(locale.GetString("FileQuickSelectFileAsk"));
            if (ibx.ShowDialog() == false) return;
            
            foreach (ListViewItem Tobeselected in ActivePanel.list.SelectedItems)
            {
                //todo: add filename mask checking, not strong name check
                if (Tobeselected.Text == ibx.Result) Tobeselected.Selected = true;
            }
        }

        private void mnuFileInvertSelection_Click(object sender, EventArgs e){//file-invert selection
            foreach (ListViewItem Inverter in ActivePanel.list.SelectedItems)
            {//undone: this shit does not work, check why!!!
                Inverter.Selected = !Inverter.Selected;
            }
        }

        private void mnuNavigateReload_Click(object sender, EventArgs e){//navigation-reload
            LoadDir(ActivePanel.FSProvider.CurrentDirectory, ActivePanel);
        }

        private void mnuToolsInfobar_Click(object sender, EventArgs e){//options-infobar
            fcmd.Properties.Settings.Default.ShowFileInfo = mnuToolsInfobar.Checked;
            fcmd.Properties.Settings.Default.Save();
            ActivePanel.Repaint();
            PassivePanel.Repaint();
        }

        private void mnuToolsDiskButtons_Click(object sender, EventArgs e){//options-disk buttons
            fcmd.Properties.Settings.Default.ShowDiskList = mnuToolsDiskButtons.Checked;
            fcmd.Properties.Settings.Default.Save();
            ActivePanel.Repaint();
            PassivePanel.Repaint();
        }

        private void mnuToolsKeybrdHelp_Click(object sender, EventArgs e)
        {
            fcmd.Properties.Settings.Default.ShowKeybrdHelp = mnuToolsKeybrdHelp.Checked;
            fcmd.Properties.Settings.Default.Save();

            tsKeyboard.Visible = fcmd.Properties.Settings.Default.ShowKeybrdHelp;
            this.OnResize(new EventArgs());
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            string AboutString = string.Format(locale.GetString("FileCommanderVer"),"File Commander", Application.ProductVersion)+
                                               "\n(C) 2013-14, FC team (Alexander Tauenis & comrades)\nhttps://github.com/atauenis/fcmd\n"+
                                               "Uses Xamarin Window Toolkit (Xwt) with A.T.'s patches\nhttps://github.com/atauenis/xwt/tree/wpf-patches\n\n" + Environment.OSVersion + "\nFramework version: " + Environment.Version;
            MessageBox.Show(AboutString,Application.ProductName);
        }

        //CROSS-PLATFORM-BACKEND
        /// <summary>Add an item into the ListPanel's ListView</summary>
        void AddItem(ListPanel lp, string ItemDisplayText, pluginner.FSEntryMetadata ItemMetadata, long ItemSize, DateTime ItemDate){
            //creating a new listpanel's listview item
            ListViewItem NewItem = new ListViewItem(ItemDisplayText);
            NewItem.Tag = ItemMetadata; //each list item is "tagged" with the file's metadata
            NewItem.SubItems.Add(KiloMegaGigabyteConvert(ItemSize));
            NewItem.SubItems.Add(ItemDate.ToShortDateString());
            AddItem(lp, NewItem);
            //todo: add file icons
        }

        private void frmMain_FormClosed(object sender, FormClosedEventArgs e)
        {
            Xwt.Application.Exit();
        }

        private void mnuToolsOptions_Click(object sender, EventArgs e)
        {
            new SettingsWindow().Run();
            ActivePanel.Repaint();
            PassivePanel.Repaint();
        }

	}

    //CROSS-PLATFORM-BACKEND
	public delegate void StringEvent(object sender, EventArgs<String> e);
	public class EventArgs<T> : EventArgs
		//http://www.gotdotnet.ru/forums/2/51331/
	{
		private T _data;
		public EventArgs(T data)
		{
		_data = data;
		}
		public T Data
		{
		get { return _data; }
		}
	}
}