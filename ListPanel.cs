/* The File Commander
 * Элемент управления ListPanel (улучшенный аналог ListView)
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
    public partial class ListPanel : System.Windows.Forms.UserControl
    {
		//Типы
		public class ItemDescription{//тип для пункта в списке
			public List<string> Text = new List<string>();
			public string Value;
			public bool Selected;
		}

		//Внутренние переменные
		string[] _collumns;//заголовки столбцов
		List<ItemDescription> _items = new List<ItemDescription>(); //элементы списка
		List<Label> Nadpis = new List<Label>(); //элемент списка (пункт)

		//Подпрограммы
        /// <summary>
        /// Конструктор лист-панели
        /// </summary>
        public ListPanel(){//Ну, за инициализацию!
            InitializeComponent();
			Label Lbl;
			Lbl = new Label();
			Lbl.Top = 0;
			Lbl.Left = 0;
			Lbl.AutoSize = true;
			Lbl.Text = "Test";
			Nadpis.Add (Lbl);
			this.Controls.Add (Nadpis[0]);
        }

        private void ListPanel_Load(object sender, EventArgs e){//Ну, за загрузку!
            //TODO
        }

        private void ListPanel_Resize(object sender, EventArgs e){//Ну, за деформацию!
            //TODO
			_Repaint ();
        }

		//Отрисовка
		private void _AddItem(string Txt, int Offset){//добавление пункта
			Label Lbl;
			int LastItem;
			if (Nadpis.Count > 0){
				LastItem = Nadpis.Count - 1;
			}else{
				LastItem = -1;
			}
			Lbl = new Label();
			Lbl.Left = 0;
			Lbl.Top = Nadpis[LastItem].Top + Nadpis[LastItem].Height;
			Lbl.AutoSize = true;
			Lbl.Text = Txt;
			Nadpis.Add (Lbl);
			this.Controls.Add (Nadpis[LastItem + 1]);
		}
		private void _Repaint(){ //перерисовка экрана
			foreach (ItemDescription x in _items){
				_AddItem (x.Text[0],0);
			}
		}

        //Методы

        //Свойства
        public List<ItemDescription> Items{
			get{
			return _items;
			
			}
			set{
				_items = value;
				//TODO:ввести очистку списка перед перезаписью
				foreach (ItemDescription x in _items){
					_AddItem (x.Text[0],0);
				}
			}
		}
    }
}
