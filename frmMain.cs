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


			//Формирую панели
			//Левая
			this.lplLeft.Add (new ListPanel()); //добавление в коллекцию левых панелей
            this.lplLeft[0].Location = new System.Drawing.Point(0, 30);
            this.lplLeft[0].Name = "lplLeft";
            this.lplLeft[0].Size = new System.Drawing.Size(300, 300);
#if DEBUG
			this.lplLeft[0].BackColor = System.Drawing.Color.FromName("yellow");
#endif
            this.lplLeft[0].TabIndex = 0;
			this.lplLeft[0].DoubleClick += new StringEvent(this.Panel_DblClick);
			this.lplLeft[0].GotFocus += new System.EventHandler(this.Panel_Focus);
			this.lplLeft[0].BorderStyle = BorderStyle.Fixed3D;
			ListPanel.CollumnOptions colopt = new ListPanel.CollumnOptions();
			colopt.Caption = "Имя";
			colopt.Size=new Size(300,0);
			colopt.Tag = "Name";
			this.lplLeft[0].Collumns.Add (colopt);
			colopt.Caption = "Размер";
			colopt.Tag = "Size";
			this.lplLeft[0].Collumns.Add (colopt);
            this.Controls.Add(this.lplLeft[0]); //ввожу панель в форму
			ActivePanel = this.lplLeft[0]; //и делаю её активной
			//Правая
			this.lplRight.Add (new ListPanel()); //добавление в коллекцию правых панелей
            this.lplRight[0].Location = new System.Drawing.Point(0, 30);
            this.lplRight[0].Name = "lplRight";
            this.lplRight[0].Size = new System.Drawing.Size(300, 300);
#if DEBUG
			this.lplRight[0].BackColor = System.Drawing.Color.FromName("yellow");
#endif
            this.lplRight[0].TabIndex = 0;
            this.lplRight[0].DoubleClick += new StringEvent(this.Panel_DblClick);
			this.lplRight[0].GotFocus += new System.EventHandler(this.Panel_Focus);
			this.lplRight[0].BorderStyle = BorderStyle.Fixed3D;
            this.Controls.Add(this.lplRight[0]); //ввожу панель в форму

			//TODO:подумать над слежением за панелями (активная-пассивная)

			//формирую поля ввода пути
			txtURL[0] = new TextBox();
			txtURL[1] = new TextBox();
			txtURL[0].Tag = 0;
			txtURL[1].Tag = 1;
			this.Controls.Add (txtURL[0]);
			this.Controls.Add (txtURL[1]);
			this.txtURL[0].Text = Directory.GetLogicalDrives()[0];


			string startupDir = Directory.GetLogicalDrives()[0];
			//формирую список
			string[] dirList; string[] fileList;
			dirList = Directory.GetDirectories(startupDir);
			fileList = Directory.GetFiles (startupDir);

            foreach (string curItem in dirList)
            { //директории
                //lplLeft[0].AddItem(curItem + "/");
				ListPanel.ItemDescription NewItem;
				NewItem = new ListPanel.ItemDescription();
				NewItem.Text.Add (curItem + "/");
				NewItem.Text.Add ("<DIR>");
				NewItem.Value = curItem + "/";
				lplLeft[0].Items.Add (NewItem);
            }
            foreach (string curItem in fileList)
            { //файлы
				ListPanel.ItemDescription NewItem;
				NewItem = new ListPanel.ItemDescription();
				NewItem.Text.Add (curItem);
				FileInfo fi = new FileInfo(curItem);
				NewItem.Text.Add (fi.Length.ToString());
				NewItem.Value = curItem;
				lplLeft[0].Items.Add (NewItem);
            }

			this.OnSizeChanged (new EventArgs()); //обновляю панели
		}

		public void frmMain_Resize(object sender, EventArgs e){ //Деформация формы
			foreach (ListPanel llp in this.lplLeft){
				llp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top);
			}
			foreach(ListPanel rlp in this.lplRight){
				rlp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top);
				rlp.Left = this.Width / 2;
			}
			txtURL[0].Location = new Point(0,0);
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
	                //lplLeft[0].AddItem(curItem + "/");
					ListPanel.ItemDescription NewItem;
					NewItem = new ListPanel.ItemDescription();
					NewItem.Text.Add (curItem + "/");
					NewItem.Value = curItem + "/";
					ActivePanel.Items.Add (NewItem);
	            }
	            foreach (string curItem in fileList)
	            { //файлы
					ListPanel.ItemDescription NewItem;
					NewItem = new ListPanel.ItemDescription();
					NewItem.Text.Add (curItem);
					NewItem.Value = curItem;
					ActivePanel.Items.Add (NewItem);
	            }

				this.OnSizeChanged (new EventArgs()); //обновляю панели
				}catch(Exception ex){
				MessageBox.Show(ex.StackTrace,ex.Message,MessageBoxButtons.OK,MessageBoxIcon.Stop);
				}
			}
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