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
		//Типы

		//Внутренние переменные
        private object Vnutr;

		//Подпрограммы
        /// <summary>
        /// Конструктор лист-панели
        /// </summary>
        /// <param name="Tip">Тип объекта, который будет внутри</param>
        public ListPanel(object Tip){//Ну, за инициализацию!
            InitializeComponent();
            this.Vnutr = new Tip();
            this.Controls.Add(this.Vnutr);
        }

        private void ListPanel_Load(object sender, EventArgs e){//Ну, за загрузку!
            Vnutr.Location = new Point(0, 0);
        }

        private void ListPanel_Resize(object sender, EventArgs e){//Ну, за деформацию!
            Vnutr.Size = this.Size;
        }

        private void lbx_DblClick(object sender, EventArgs e){//Доублклик по списку
            OnDoubleClick(e);
        }


        //Методы

        //Свойства
        //TODO: контекст, вложенный контрол
        string[] Context;
    }
}
