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
		//TODO:убрать ListBox и заменить на более кустомизируемый аналог с колонками и иконками
		//Типы
		public struct OptSel{ //тип для панели optionbox'ов ввверху listpanel'и
			int Count;
			string[] Name;
			string[] Value;

		}

		//Внутренние переменные
		OptSel _os;


		//Подпрограммы
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

        private void lbx_DblClick(object sender, EventArgs e)
		{//Доублклик по списку
            OnDoubleClick(e);
        }


        //Методы
        /// <summary>
        /// Добавить пункт в список.
		/// Add a item into the list.
        /// </summary>
        /// <para
		/// m name="item">Аналогично listbox.Items.Add(item)</param>
        public void AddItem(object item){
            lbx.Items.Add(item);
        }

		/// <summary>
		/// Возвращает выбранную строку.
		/// Gets the selected item.
		/// </summary>
		/// <returns>
		/// Выбранная строка.
		/// The selected item.
		/// </returns>
		public object GetSelectedItem(){
				int RowId;
				RowId = (int)lbx.SelectedIndex;
				string RowText;
				RowText = (string)lbx.Items[RowId].ToString();
            	return RowText;
        }
		//TODO: RemoveItem(id), GetItem(id), EditItem(id), а также перечень из checkbox'ов вверху



        //Свойства
        //TODO: цветовые качества листпанели (после обратной разработки listbox)
		public OptSel OptionSelector{
			get{return _os;}
			set{_os = value;}
		}
    }
}
