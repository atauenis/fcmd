/* The File Commander - окно вывода статуса действия
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
    public partial class FileProcessDialog : Form
    {
        public FileProcessDialog()
        {
            InitializeComponent();
        }


        private void FileProcessDialog_Load(object sender, EventArgs e)
        {

        }

        private void FileProcessDialog_Shown(object sender, EventArgs e)
        {
            this.Refresh();
        }

    }
}
