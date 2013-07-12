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
#if DEBUG
			this.lplLeft[0].BackColor = System.Drawing.Color.FromName("yellow");
#endif
			this.lplLeft[0].BackColor = SystemColors.Window;
            this.lplLeft[0].TabIndex = 0;
			this.lplLeft[0].DoubleClick += new StringEvent(this.Panel_DblClick);
			this.lplLeft[0].GotFocus += new System.EventHandler(this.Panel_Focus);
			this.lplLeft[0].BorderStyle = BorderStyle.FixedSingle;
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
			this.lplRight[0].BackColor = System.Drawing.Color.FromName("yellow");
#endif
            this.lplRight[0].TabIndex = 0;
            this.lplRight[0].DoubleClick += new StringEvent(this.Panel_DblClick);
			this.lplRight[0].GotFocus += new System.EventHandler(this.Panel_Focus);
			this.lplRight[0].BorderStyle = BorderStyle.FixedSingle;
            this.Controls.Add(this.lplRight[0]); //ввожу панель в форму

			//TODO:подумать над слежением за панелями (активная-пассивная)
			#endregion

			List<pluginner.IPlugin> plugins = new List<pluginner.IPlugin>();
			string file = Application.StartupPath + "/../../base-plugins/localfs/bin/Debug/localfs.dll";
            Assembly assembly = Assembly.LoadFile(file);

            foreach (Type type in assembly.GetTypes()) {
				Type iface = type.GetInterface("pluginner.IPlugin");

				if (iface != null)	{
					pluginner.IPlugin plugin = (pluginner.IPlugin)Activator.CreateInstance(type); //BUG: InvalidCastException: Cannot cast from source type to destination type.
					plugins.Add(plugin); 
				}
			}

			//todo: выкинуть plugins[] и привязать к listpanel'ям
			//todo: написать API плагинов доступа к файловым системам

			#region Изначальный перечень файлов
			string startupDir = Directory.GetLogicalDrives()[0];
			//формирую список
			string[] dirList; string[] fileList;
			dirList = Directory.GetDirectories(startupDir);
			fileList = Directory.GetFiles (startupDir);

            foreach (string curItem in dirList)
            { //директории
				ListPanel.ItemDescription NewItem;
				NewItem = new ListPanel.ItemDescription();
				DirectoryInfo di = new DirectoryInfo(curItem);
				NewItem.Text.Add (di.Name);
				NewItem.Text.Add ("<DIR>");
				NewItem.Text.Add (di.LastWriteTime.ToShortDateString());
				NewItem.Value = curItem + "/";
				lplLeft[0].Items.Add (NewItem);
            }
            foreach (string curItem in fileList)
            { //файлы
				ListPanel.ItemDescription NewItem;
				NewItem = new ListPanel.ItemDescription();
				FileInfo fi = new FileInfo(curItem);
				NewItem.Text.Add (fi.Name);
				NewItem.Text.Add (fi.Length / 1024 + "КБ");
				NewItem.Text.Add (fi.CreationTime.Date.ToShortDateString());
				NewItem.Value = curItem;
				lplLeft[0].Items.Add (NewItem);
            }
			#endregion

			this.OnSizeChanged (new EventArgs()); //hack: обновляю панели
		}

		public void frmMain_Resize(object sender, EventArgs e){ //Деформация формы
			foreach (ListPanel llp in this.lplLeft){
				llp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top - mstMenu.Height - mstKeyboard.Height); 
			}
			foreach(ListPanel rlp in this.lplRight){
				rlp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top - mstKeyboard.Height);
				rlp.Left = this.Width / 2;
			}
			txtURL[0].Location = new Point(0,mstMenu.Height);
			txtURL[0].Width = lplLeft[0].Width;
			txtURL[1].Left = lplRight[0].Left;
			txtURL[1].Width = lplRight[0].Width;
		}

		private void Panel_Focus(object sender, EventArgs e){ //панель получила фокус
			ActivePanel = (ListPanel)sender;
		}

		private void Panel_LostFocus(object sender, EventArgs e){ //панель лишилась фокуса
			//PassivePanel = (ListPanel)sender;
		}

        private void Panel_DblClick(object sender, EventArgs<String> e){ //двойной щелчок по панели
			if (Directory.Exists (e.Data)){
				//это - каталог
				txtURL[0].Text = e.Data;
				KeyEventArgs kea = new KeyEventArgs(Keys.Enter);
				txtURL_KeyUp(txtURL[0],kea);
			}else MessageBox.Show(e.Data,"это файл");
        }

		private void ForceGo(object sender, EventArgs e){ //hack //убрать
			txtURL_KeyUp(txtURL[0],new KeyEventArgs(Keys.Enter));
			this.OnSizeChanged (new EventArgs()); //hack: обновляю панели
		}

		private void txtURL_KeyUp(object sender, KeyEventArgs e){ //отпускание клавиши в поле адреса
			if(e.KeyCode == Keys.Enter){
				TextBox tb = (TextBox) sender;
				if(!Directory.Exists (tb.Text)){return;} //проверка наличия каталога
				try{
				ActivePanel.Items.Clear();

				//todo:вынести в плагин localfs.dll
				string[] dirList; string[] fileList;
				dirList = Directory.GetDirectories(tb.Text);
				fileList = Directory.GetFiles (tb.Text);

	            foreach (string curItem in dirList)
	            { //директории
					ListPanel.ItemDescription NewItem;
					NewItem = new ListPanel.ItemDescription();
					DirectoryInfo di = new DirectoryInfo(curItem);
					NewItem.Text.Add (di.Name);
					NewItem.Text.Add ("<DIR>");
					NewItem.Text.Add (di.LastWriteTime.ToShortDateString());
					NewItem.Value = curItem + "/";
					lplLeft[0].Items.Add (NewItem);
	            }
	            foreach (string curItem in fileList)
	            { //файлы
					ListPanel.ItemDescription NewItem;
					NewItem = new ListPanel.ItemDescription();
					FileInfo fi = new FileInfo(curItem);
					NewItem.Text.Add (fi.Name);
					NewItem.Text.Add (fi.Length / 1024 + "КБ");
					NewItem.Text.Add (fi.CreationTime.Date.ToShortDateString());
					NewItem.Value = curItem;
					lplLeft[0].Items.Add (NewItem);
	            }

					lplLeft[0].Redraw();
				}catch(Exception ex){
				MessageBox.Show(ex.StackTrace,ex.Message,MessageBoxButtons.OK,MessageBoxIcon.Stop);
				}
			}
		}

		private void mstMenu_ItemClicked(object sender, System.Windows.Forms.ToolStripItemClickedEventArgs e){
			
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