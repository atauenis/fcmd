/* The File Commander - просмоторщик файлов (FCView)
 * Главное окно просмоторщика
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace fcmd
{
    public partial class fcview : Form
    {
        public fcview()
        {
            InitializeComponent();
        }

        public void LoadFile(string content, string URL){//загрузка txt-файлов
            this.Text = "Просмоторщик FC - " + URL;

            pluginfinder pf = new pluginfinder();
            pluginner.IViewerPlugin vp;
            vp = pf.GetFCVplugin(content); //определяю плагин просмоторщика по заголовкам (MZ, RAR, <?xml, <!doctype и т.п.)
            vp.LoadFile(URL);
			if(vp.DisplayBox().BackColor != SystemColors.Window) pnlContainer.BorderStyle = BorderStyle.Fixed3D;
			// ^- если цвет фона плагина не оконный, рисовать рамку. Это выверт для подавления XP-стилей
            this.pnlContainer.Controls.Add(vp.DisplayBox());

			//"доработка" интерфейса fcview под текущий плагин
			mnuFilePrint.Enabled = vp.CanPrint;
			mnuFilePrintOptions.Enabled = vp.CanPrint;
			mnuEditCopy.Enabled = vp.CanCopy;
			mnuEditSelectAll.Enabled = vp.CanSelectAll;

            this.Show();
        }
    }
}
