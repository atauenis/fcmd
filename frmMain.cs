/* The File Commander
 * Главное окно
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
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

namespace fcmd
{
	public partial class frmMain : Form 
	{
        //todo: поддержка локализации и замена временных фраз на серьёзные из файла
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
        private ToolStripMenuItem менюВПроцессеРазработкиToolStripMenuItem;
        private ToolStripMenuItem даНичегоТутНетToolStripMenuItem;
		private TextBox[] txtURL = new TextBox[2];
        Localizator locale = new Localizator(); //объект для работы с локализациями интерфейса

		//Подпрограммы
		static void Main(){ //Иницализация программы
			Application.Run(new frmMain());//BUG: github issue #2
		}

		public frmMain() { //Инициализация элементов управления
			InitializeComponent();
		}
		
		private void frmMain_Load(object sender, EventArgs e){ //функция Form_Load()
            Application.EnableVisualStyles();
            Localize();

			#if DEBUG
				MessageBox.Show ("File commander, версия " + Application.ProductVersion);
			#endif

			#region txtURL[x]
			//формирую поля ввода пути
			txtURL[0] = new TextBox();
			txtURL[1] = new TextBox();
			txtURL[0].Tag = 0;
			txtURL[1].Tag = 1;
			txtURL[0].Top = mstMenu.Height;
			txtURL[1].Top = mstMenu.Height;
			txtURL[0].KeyUp += txtURL_KeyUp;
			txtURL[0].DoubleClick += ForceGo;
			txtURL[1].KeyUp += txtURL_KeyUp;
			txtURL[1].DoubleClick += ForceGo;
			this.Controls.Add (txtURL[0]);
			this.Controls.Add (txtURL[1]);
			this.txtURL[0].Text = Directory.GetLogicalDrives()[0];
			#endregion
            
			#region Панели
            //TODO: переписать на ListBox
			//Формирую панели
			//Левая
			this.lplLeft.Add (new ListPanel()); //добавление в коллекцию левых панелей
			this.lplLeft[0].Location = new System.Drawing.Point(0, mstMenu.Height+txtURL[0].Height);
            this.lplLeft[0].Name = "lplLeft";
			this.lplLeft[0].BackColor = SystemColors.Window;
            this.lplLeft[0].TabIndex = 0;
			this.lplLeft[0].DoubleClick += new StringEvent(this.Panel_DblClick);
			this.lplLeft[0].GotFocus += this.Panel_Focus;
			this.lplLeft[0].BorderStyle = BorderStyle.Fixed3D;
			this.lplLeft[0].FSProvider = new localFileSystem();
            lplLeft[0].list.Columns.Add("Name", locale.GetString("FName"));
            lplLeft[0].list.Columns.Add("Size", locale.GetString("FSize"));
            lplLeft[0].list.Columns.Add("Date", locale.GetString("FDate"));
            lplLeft[0].list.DoubleClick += (object s, EventArgs ea) => { this.frmMain_KeyDown(s, new KeyEventArgs(Keys.Enter)); };
            this.Controls.Add(this.lplLeft[0]); //ввожу панель в форму
			ActivePanel = this.lplLeft[0]; //и делаю её активной
			//Правая
			this.lplRight.Add (new ListPanel()); //добавление в коллекцию правых панелей
            this.lplRight[0].Location = new System.Drawing.Point(0, mstMenu.Height+txtURL[0].Height);
            this.lplRight[0].Name = "lplRight";
#if DEBUG
			this.lplRight[0].BackColor = SystemColors.Window;
#endif
            this.lplRight[0].TabIndex = 0;
            this.lplRight[0].DoubleClick += new StringEvent(this.Panel_DblClick);
			this.lplRight[0].GotFocus += this.Panel_Focus;
			this.lplRight[0].BorderStyle = BorderStyle.Fixed3D;
			this.lplRight[0].FSProvider = new localFileSystem();
            lplRight[0].list.Columns.Add("Name", locale.GetString("FName"));
            lplRight[0].list.Columns.Add("Size", locale.GetString("FSize"));
            lplRight[0].list.Columns.Add("Date", locale.GetString("FDate"));
            this.Controls.Add(this.lplRight[0]); //ввожу панель в форму
			PassivePanel = this.lplRight[0];
			#endregion

			#region Изначальный перечень файлов
			string startupDir = "file://" + Directory.GetLogicalDrives()[0];
            ActivePanel = lplRight[0];
            Ls(startupDir);
            ActivePanel = lplLeft[0];
            Ls("file://" + Application.StartupPath + "/../../");
			#endregion

			this.OnSizeChanged (new EventArgs()); //hack: обновляю панели
		}

		private void frmMain_Resize(object sender, EventArgs e){ //Деформация формы
			foreach (ListPanel llp in this.lplLeft){
				llp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top - mstMenu.Height - tsKeyboard.Height); 
			}
			foreach(ListPanel rlp in this.lplRight){
				rlp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top - mstMenu.Height - tsKeyboard.Height); 
				rlp.Left = this.Width / 2;
			}
			txtURL[0].Location = new Point(0,mstMenu.Height);
			txtURL[0].Width = lplLeft[0].Width;
			txtURL[1].Left = lplRight[0].Left;
			txtURL[1].Width = lplRight[0].Width;
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
                        if (ibx.ShowDialog() == DialogResult.OK)
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
                    if (!ActivePanel.FSProvider.IsDirPresent(ActivePanel.list.SelectedItems[0].Tag.ToString())) return; //todo: выругаться
                    Ls(ActivePanel.list.SelectedItems[0].Tag.ToString());
                    break;
                case Keys.F3: //просмотр
                    //todo:принуд. вызов fcview с плагином TxtViewer по Shift+F3

                    fcview fcv = new fcview();
					pluginner.IFSPlugin fs = ActivePanel.FSProvider;
					if(!fs.IsFilePresent(ActivePanel.list.SelectedItems[0].Tag.ToString())) return; //todo: выругаться

                    FCView(ActivePanel.list.SelectedItems[0].Tag.ToString());
                    break;
                case Keys.F5: //копировать
                    Cp();
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
			if (lp.FSProvider.IsDirPresent (e.Data)){
				//это - каталог

				int NeededTextbox = 0; //нужный текстбокс //remove govnokod!!!
				if(lp.Name == "lplLeft") NeededTextbox = 0;
				else NeededTextbox = 1;

				txtURL[NeededTextbox].Text = e.Data;
				KeyEventArgs kea = new KeyEventArgs(Keys.Enter);
				txtURL_KeyUp(txtURL[NeededTextbox],kea);
			}else MessageBox.Show(e.Data,"это файл");
			//todo:добавить вызов lister'а (FCView)
        }

		private void ForceGo(object sender, EventArgs e){ //hack //убрать и заменить на нормальное решение!!!
			txtURL_KeyUp(txtURL[0],new KeyEventArgs(Keys.Enter));
			this.OnSizeChanged (new EventArgs()); //hack: обновляю панели
		}

		private void txtURL_KeyUp(object sender, KeyEventArgs e){ //отпускание клавиши в поле адреса
			if(e.KeyCode == Keys.Enter){
				TextBox tb = (TextBox) sender;
				string txt = "";

				switch((int)tb.Tag){
				case 0://левый
					txt = "lplLeft";
					break;
				case 1://правый
					txt = "lplRight";
					break;
				}

				if(ActivePanel.Name == txt){
					if(!ActivePanel.FSProvider.IsDirPresent(tb.Text)) return; //проверка наличия каталога
                    if (!ActivePanel.FSProvider.CanBeRead(tb.Text)) { MessageBox.Show("Нет доступа"); return; } //проверка наличия доступа
					Ls(tb.Text);
				}else{
					if(!PassivePanel.FSProvider.IsDirPresent(tb.Text)) return; //проверка наличия каталога
                    if (!PassivePanel.FSProvider.CanBeRead(tb.Text)) { MessageBox.Show("Нет доступа"); return; } //проверка наличия доступа
					LoadDir(tb.Text,PassivePanel);
				}

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
		private void LoadDir(string url, ListPanel lp){
            lp.list.Items.Clear();

				//гружу директорию
				pluginner.IFSPlugin fsp = lp.FSProvider;
				fsp.CurrentDirectory = url;
				foreach(pluginner.DirItem di in fsp.DirectoryContent){ //перебираю файлы, найденные провайдером ФС
					if(di.Hidden == false){
                        ListViewItem NewItem = new ListViewItem(di.TextToShow);
                        NewItem.Tag = di.Path; //путь будет тегом
                        NewItem.SubItems.Add(Convert.ToString(di.Size / 1024) + "KB");
                        NewItem.SubItems.Add(di.Date.ToShortDateString());
                        lp.list.Items.Add(NewItem);
					}
				}
		}

        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMain));
            this.mstMenu = new System.Windows.Forms.MenuStrip();
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
            this.менюВПроцессеРазработкиToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.даНичегоТутНетToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mstMenu.SuspendLayout();
            this.tsKeyboard.SuspendLayout();
            this.SuspendLayout();
            // 
            // mstMenu
            // 
            this.mstMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.менюВПроцессеРазработкиToolStripMenuItem});
            this.mstMenu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mstMenu.Location = new System.Drawing.Point(0, 0);
            this.mstMenu.Name = "mstMenu";
            this.mstMenu.Size = new System.Drawing.Size(618, 23);
            this.mstMenu.TabIndex = 0;
            this.mstMenu.Text = "menuStrip1";
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
            this.tsKeyboard.Location = new System.Drawing.Point(0, 332);
            this.tsKeyboard.Name = "tsKeyboard";
            this.tsKeyboard.Size = new System.Drawing.Size(618, 25);
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
            this.tsbHelpF1.Size = new System.Drawing.Size(72, 22);
            this.tsbHelpF1.Text = "F1 Справка";
            // 
            // tsbHelpF2
            // 
            this.tsbHelpF2.AutoToolTip = false;
            this.tsbHelpF2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF2.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF2.Image")));
            this.tsbHelpF2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF2.Name = "tsbHelpF2";
            this.tsbHelpF2.Size = new System.Drawing.Size(60, 22);
            this.tsbHelpF2.Text = "F2 Вызов";
            // 
            // tsbHelpF3
            // 
            this.tsbHelpF3.AutoToolTip = false;
            this.tsbHelpF3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF3.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF3.Image")));
            this.tsbHelpF3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF3.Name = "tsbHelpF3";
            this.tsbHelpF3.Size = new System.Drawing.Size(65, 22);
            this.tsbHelpF3.Text = "F3 Чтение";
            // 
            // tsbHelpF4
            // 
            this.tsbHelpF4.AutoToolTip = false;
            this.tsbHelpF4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF4.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF4.Image")));
            this.tsbHelpF4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF4.Name = "tsbHelpF4";
            this.tsbHelpF4.Size = new System.Drawing.Size(66, 22);
            this.tsbHelpF4.Text = "F4 Правка";
            // 
            // tsbHelpF5
            // 
            this.tsbHelpF5.AutoToolTip = false;
            this.tsbHelpF5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF5.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF5.Image")));
            this.tsbHelpF5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF5.Name = "tsbHelpF5";
            this.tsbHelpF5.Size = new System.Drawing.Size(60, 22);
            this.tsbHelpF5.Text = "F5 Копия";
            // 
            // tsbHelpF6
            // 
            this.tsbHelpF6.AutoToolTip = false;
            this.tsbHelpF6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF6.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF6.Image")));
            this.tsbHelpF6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF6.Name = "tsbHelpF6";
            this.tsbHelpF6.Size = new System.Drawing.Size(50, 22);
            this.tsbHelpF6.Text = "F6 Имя";
            // 
            // tsbHelpF7
            // 
            this.tsbHelpF7.AutoToolTip = false;
            this.tsbHelpF7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF7.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF7.Image")));
            this.tsbHelpF7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF7.Name = "tsbHelpF7";
            this.tsbHelpF7.Size = new System.Drawing.Size(64, 22);
            this.tsbHelpF7.Text = "F7 Новый";
            // 
            // tsbHelpF8
            // 
            this.tsbHelpF8.AutoToolTip = false;
            this.tsbHelpF8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF8.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF8.Image")));
            this.tsbHelpF8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF8.Name = "tsbHelpF8";
            this.tsbHelpF8.Size = new System.Drawing.Size(63, 22);
            this.tsbHelpF8.Text = "F8 Удал-е";
            // 
            // tsbHelpF9
            // 
            this.tsbHelpF9.AutoToolTip = false;
            this.tsbHelpF9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF9.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF9.Image")));
            this.tsbHelpF9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF9.Name = "tsbHelpF9";
            this.tsbHelpF9.Size = new System.Drawing.Size(60, 22);
            this.tsbHelpF9.Text = "F9 Меню";
            // 
            // tsbHelpF10
            // 
            this.tsbHelpF10.AutoToolTip = false;
            this.tsbHelpF10.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF10.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF10.Image")));
            this.tsbHelpF10.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF10.Name = "tsbHelpF10";
            this.tsbHelpF10.Size = new System.Drawing.Size(66, 19);
            this.tsbHelpF10.Text = "F10 Выход";
            // 
            // менюВПроцессеРазработкиToolStripMenuItem
            // 
            this.менюВПроцессеРазработкиToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.даНичегоТутНетToolStripMenuItem});
            this.менюВПроцессеРазработкиToolStripMenuItem.Name = "менюВПроцессеРазработкиToolStripMenuItem";
            this.менюВПроцессеРазработкиToolStripMenuItem.Size = new System.Drawing.Size(186, 19);
            this.менюВПроцессеРазработкиToolStripMenuItem.Text = "Меню в процессе разработки!";
            // 
            // даНичегоТутНетToolStripMenuItem
            // 
            this.даНичегоТутНетToolStripMenuItem.Name = "даНичегоТутНетToolStripMenuItem";
            this.даНичегоТутНетToolStripMenuItem.Size = new System.Drawing.Size(176, 22);
            this.даНичегоТутНетToolStripMenuItem.Text = "Да, ничего тут нет.";
            // 
            // frmMain
            // 
            this.ClientSize = new System.Drawing.Size(618, 357);
            this.Controls.Add(this.tsKeyboard);
            this.Controls.Add(this.mstMenu);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mstMenu;
            this.Name = "frmMain";
            this.Text = "File Commander";
            this.Load += new System.EventHandler(this.frmMain_Load);
            this.Resize += new System.EventHandler(this.frmMain_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmMain_KeyDown);
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
        /// Перевести интерфейс на язык локали
        /// </summary>
        private void Localize(){
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
        }

        private void AddItem(ListPanel lp, ListViewItem NewItem){
            CheckForIllegalCrossThreadCalls = false; //HACK: заменить на долбанные делегации и прочую нетовскую муть
            lp.list.Items.Add(NewItem);
        }

	}

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