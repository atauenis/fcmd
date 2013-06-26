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

		//Подпрограммы
		static void Main(){ //Иницализация программы
			Application.Run(new frmMain());
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
            this.lplLeft[0].BackColor = System.Drawing.Color.FromName("yellow");
            this.lplLeft[0].TabIndex = 0;
            this.lplLeft[0].DoubleClick += new System.EventHandler(this.Panel_DblClick);
			this.lplLeft[0].GotFocus += new System.EventHandler(this.Panel_Focus);
            this.Controls.Add(this.lplLeft[0]); //ввожу панель в форму
			ActivePanel = this.lplLeft[0]; //и делаю её активной
			//Правая
			this.lplRight.Add (new ListPanel()); //добавление в коллекцию правых панелей
            this.lplRight[0].Location = new System.Drawing.Point(0, 30);
            this.lplRight[0].Name = "lplRight";
            this.lplRight[0].Size = new System.Drawing.Size(300, 300);
            this.lplRight[0].BackColor = System.Drawing.Color.FromName("yellow");
            this.lplRight[0].TabIndex = 0;
            this.lplRight[0].DoubleClick += new System.EventHandler(this.Panel_DblClick);
			this.lplRight[0].GotFocus += new System.EventHandler(this.Panel_Focus);
            this.Controls.Add(this.lplRight[0]); //ввожу панель в форму
			ActivePanel = this.lplRight[0]; //и делаю её активной


			string startupDir = Directory.GetLogicalDrives()[0];
			//формирую список
			string[] dirList; string[] fileList;
			dirList = Directory.GetDirectories(startupDir);
			fileList = Directory.GetFiles (startupDir);

            foreach (string curItem in dirList)
            { //директории
                lplLeft[0].AddItem(curItem + "/");
            }
            foreach (string curItem in fileList)
            { //файлы
                lplLeft[0].AddItem(curItem);
            }

			lplRight[0].AddItem("Test");
		}

		public void frmMain_Resize(object sender, EventArgs e){ //Деформация формы
			foreach (ListPanel llp in this.lplLeft){
				llp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top);
			}
			foreach(ListPanel rlp in this.lplRight){
				rlp.Size = new Size(this.Width / 2,this.Height - ActivePanel.Top);
				rlp.Left = this.Width / 2;
			}
		}
		private void Panel_Focus(object sender, EventArgs e){ //панель получила фокус
			ActivePanel = (ListPanel)sender;
		}

		private void Panel_LostFocus(object sender, EventArgs e){ //панель лишилась фокуса
			//PassivePanel = (ListPanel)sender;
		}

        private void Panel_DblClick(object sender, EventArgs e){
			ListPanel lp = (ListPanel)sender;
            MessageBox.Show(lp.GetSelectedItem().ToString() );
        }

	}
}