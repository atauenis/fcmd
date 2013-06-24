/* The File Commander
 * Элемент управления ListPanel (draft)
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace fcmd
{
    public partial class ListPanel : UserControl
    {
        public ListPanel()
        {//Ну, за инициализацию!
            InitializeComponent();
        }

        private void ListPanel_Load(object sender, EventArgs e)
        {//Ну, за загрузку!
            lbx.Location = new Point(0, 0);
        }

        private void ListPanel_Resize(object sender, EventArgs e)
        {//Ну, за деформацию!
            lbx.Size = this.Size;
        }

        private void lbx_DblClick(object sender, EventArgs e){//Доублклик по списку
            OnDoubleClick(e);
        }


        //Методы
        /// <summary>
        /// Добавить пункт в список
        /// </summary>
        /// <param name="item">Аналогично listbox.Items.Add(item)</param>
        public void AddItem(object item)
        {
            lbx.Items.Add(item);
        }
        public object GetCurrentItem()
        {
            int RowId = lbx.SelectedIndex;
            string RowText = lbx.Items[RowId].ToString();
            return RowText;
        }



        //Свойства
        //TODO: цветовые качества листпанели (после обратной разработки listbox)
    }
}
