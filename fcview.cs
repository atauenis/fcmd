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

        private void fcview_Load(object sender, EventArgs e)
        {

        }

        public void LoadFile(byte[] content, string URL){//загрузка бинарных файлов
        }

        public void LoadFile(string content, string URL){//загрузка txt-файлов
            this.Text = "Просмоторщик FC - " + URL;

            pluginfinder pf = new pluginfinder();
            pluginner.IViewerPlugin vp;
            vp = pf.GetFCVplugin(content);
            vp.LoadFile(URL);
            this.pnlContainer.Controls.Add(vp.DisplayBox());
            this.Show();
        }
    }
}
