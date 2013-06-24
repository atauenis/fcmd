/* The File Commander
 * Главное окно
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
//using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace fcmd
{
	public partial class frmMain : Form 
	{
        private ListPanel listPanel1;
    
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
			dirList = Directory.GetDirectories("C:\\WINDOWS\\");
			fileList = Directory.GetFiles ("C:\\WINDOWS\\");

            foreach (string curItem in dirList)
            { //директории
                listPanel1.AddItem(curItem + "/");
            }
            foreach (string curItem in fileList)
            { //файлы
                listPanel1.AddItem(curItem);
            }
		}

        //private void lstFiles_DblClick(object sender, EventArgs e){
        //    int RowId = lstFiles.SelectedIndex;
        //    string RowText = lstFiles.Items[RowId].ToString();
        //    MessageBox.Show (RowText,"В выделенной строке...");
        //}

        private void Panel_DblClick(object sender, EventArgs e)
        {
            MessageBox.Show((string)listPanel1.GetCurrentItem().ToString() );
        }

	}
}
