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
		//Внутренние переменные
		private List<ListPanel> lplLeft = new List<ListPanel>();
		private List<ListPanel> lplRight = new List<ListPanel>();
		private ListPanel ActivePanel; //текущая активная панель (на которой стоит фокус)
		private ListPanel PassivePanel; //текущая пассивная панель (панель-получатель)
		private TextBox[] txtURL = new TextBox[2];

		//Подпрограммы
		static void Main(){ //Иницализация программы
			Application.Run(new frmMain());//BUG: github issue #2
		}

		public frmMain() { //Инициализация элементов управления
			InitializeComponent();
		}
		
		public void frmMain_Load(object sender, EventArgs e){ //функция Form_Load()
			#if DEBUG
				MessageBox.Show ("File commander, версия " + Application.ProductVersion);
			#endif

			this.KeyDown += frmMain_KeyDown;

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
			//Формирую панели
			//Левая
			this.lplLeft.Add (new ListPanel()); //добавление в коллекцию левых панелей
			this.lplLeft[0].Location = new System.Drawing.Point(0, mstMenu.Height+txtURL[0].Height);
            this.lplLeft[0].Name = "lplLeft";
			this.lplLeft[0].BackColor = SystemColors.Window;
            this.lplLeft[0].TabIndex = 0;
			this.lplLeft[0].DoubleClick += new StringEvent(this.Panel_DblClick);
			this.lplLeft[0].GotFocus += this.Panel_Focus;
			this.lplLeft[0].BorderStyle = BorderStyle.FixedSingle;
			this.lplLeft[0].FSProvider = new localFileSystem();
			ListPanel.CollumnOptions colopt = new ListPanel.CollumnOptions();
			colopt.Caption = "Имя";
			colopt.Size=new Size(200,0);
			colopt.Tag = "Name";
			this.lplLeft[0].Collumns.Add (colopt);
			colopt.Caption = "Размер";
			colopt.Tag = "Size";
			colopt.Size = new Size(50,0);
			this.lplLeft[0].Collumns.Add (colopt);
			colopt.Caption = "Дата";
			colopt.Tag = "Date";
			colopt.Size = new Size(100,0);
			this.lplLeft[0].Collumns.Add(colopt);
			this.lplLeft[0].ShowCollumnTitles = true;
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
			this.lplRight[0].BorderStyle = BorderStyle.FixedSingle;
			this.lplRight[0].FSProvider = new localFileSystem();
			colopt.Caption = "Имя";
			colopt.Size=new Size(200,0);
			colopt.Tag = "Name";
			this.lplRight[0].Collumns.Add (colopt);
			colopt.Caption = "Размер";
			colopt.Tag = "Size";
			colopt.Size = new Size(50,0);
			this.lplRight[0].Collumns.Add (colopt);
			colopt.Caption = "Дата";
			colopt.Tag = "Date";
			colopt.Size = new Size(100,0);
			this.lplRight[0].Collumns.Add(colopt);
			this.lplRight[0].ShowCollumnTitles = true;
            this.Controls.Add(this.lplRight[0]); //ввожу панель в форму
			PassivePanel = this.lplRight[0];
			#endregion

//			List<pluginner.IPlugin> plugins = new List<pluginner.IPlugin>();
//			string file = Application.StartupPath + "/../../base-plugins/localfs/bin/Debug/localfs.dll";
//            Assembly assembly = Assembly.LoadFile(file);
//
//            foreach (Type type in assembly.GetTypes()) {
//				Type iface = type.GetInterface("pluginner.IPlugin");
//
//				if (iface != null)	{
//					pluginner.IPlugin plugin = (pluginner.IPlugin)Activator.CreateInstance(type); //BUG: InvalidCastException: Cannot cast from source type to destination type.
//					plugins.Add(plugin); 
//				}
//			}
//
//			//todo: выкинуть plugins[] и привязать к listpanel'ям
//			//todo: написать API плагинов доступа к файловым системам

			#region Изначальный перечень файлов
			string startupDir = "file://" + Directory.GetLogicalDrives()[0];
			LoadDir(startupDir,lplLeft[0]);
			LoadDir(Application.StartupPath + "/../../" ,lplRight[0]);
			#endregion

			this.OnSizeChanged (new EventArgs()); //hack: обновляю панели
		}

		public void frmMain_Resize(object sender, EventArgs e){ //Деформация формы
			foreach (ListPanel llp in this.lplLeft){
				llp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top - mstMenu.Height - mstKeyboard.Height); 
			}
			foreach(ListPanel rlp in this.lplRight){
				rlp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top - mstMenu.Height - mstKeyboard.Height); 
				//rlp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top - mstKeyboard.Height);
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
			string Modificator = "None"; //на будущее
			if(e.Alt) Modificator = "Alt";
			if(e.Control) Modificator = "Ctrl";
			if(e.Shift) Modificator = "Shift";
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
					LoadDir(tb.Text,ActivePanel);
				}else{
					if(!PassivePanel.FSProvider.IsDirPresent(tb.Text)) return; //проверка наличия каталога
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
				lp.Items.Clear();

				//гружу директорию
				pluginner.IFSPlugin fsp = lp.FSProvider;
				fsp.CurrentDirectory = url;
				foreach(pluginner.DirItem di in fsp.DirectoryContent){ //перебираю файлы, найденные провайдером ФС
					if(di.Hidden == false){
						ListPanel.ItemDescription NewItem;
						NewItem = new ListPanel.ItemDescription();
						NewItem.Text.Add (di.TextToShow);
						NewItem.Text.Add (Convert.ToString (di.Size / 1024) + "KB");
						NewItem.Text.Add (di.Date.ToShortDateString());
						NewItem.Value = di.Path;
						lp.Items.Add (NewItem);
					}
				}
				lp.Redraw();
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