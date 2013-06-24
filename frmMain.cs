using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace fcmd
{
	public partial class frmMain : Form
	{
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

			//формирую список
			string[] dirList; string[] fileList;
			dirList = Directory.GetDirectories ("/home/atauenis/");
			fileList = Directory.GetFiles ("/home/atauenis/");

			foreach(string curItem in dirList){ //директории
				lstFiles.Items.Add (curItem + "/");
			}
			foreach(string curItem in fileList){ //файлы
				lstFiles.Items.Add (curItem);
			}
		}


		private void lstFiles_DblClick(object sender, EventArgs e){
			int RowId = lstFiles.SelectedIndex;
			string RowText = lstFiles.Items[RowId].ToString();
			MessageBox.Show (RowText,"В выделенной строке...");
		}
	}
}
