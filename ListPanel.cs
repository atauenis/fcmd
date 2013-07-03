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
			public Image Icon;
		}

		///<summary>
		/// Прокачанный контрол Label
		/// </summary>
		public class Label2 : Label{
			public int Context;
		}

		//Внутренние переменные
		string[] _collumns;//заголовки столбцов
		List<ItemDescription> _items = new List<ItemDescription>(); //элементы списка
		List<Label2> Nadpis = new List<Label2>(); //элемент списка (пункт)

		//Подпрограммы
        /// <summary>
        /// Конструктор лист-панели
        /// </summary>
        public ListPanel(){//Ну, за инициализацию!
            InitializeComponent();
        }

        private void ListPanel_Load(object sender, EventArgs e){//Ну, за загрузку!
            //TODO
			_Repaint();
        }

        private void ListPanel_Resize(object sender, EventArgs e){//Ну, за деформацию!
            //TODO
			_Repaint ();
        }

		//Отрисовка
		private void _AddItem(string Txt, int Offset, int Context){//добавление пункта
			Label2 Lbl;
			Lbl = new Label2();
#if DEBUG
			Lbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
#endif
			Lbl.Left = Offset;
			if (Nadpis.Count > 0){ //если есть пункты
				Lbl.Top = Nadpis[Nadpis.Count-1].Top + Nadpis[Nadpis.Count-1].Height;
			}else{ //если их нет
				Lbl.Top = 0;
			}
			if(Offset!=0) Lbl.AutoSize = true; else Lbl.AutoSize = false; Lbl.Width = this.Width;
			Lbl.Text = Txt;
			Lbl.Context = Context;

			Nadpis.Add (Lbl);
			this.Controls.Add (Nadpis[Nadpis.Count -1]);
			Nadpis[Nadpis.Count -1].DoubleClick += _DblClick;
		}
		private void _Clear(){//очистка формы
			Nadpis.Clear();
			this.Controls.Clear();
		}
		private void _Repaint(){ //перерисовка экрана
			_Clear();
			int i = 0; //номер текущего элемента в БД
			foreach (ItemDescription x in _items){
				_AddItem (x.Text[0],0, i);
				i++;
			}
		}

		private void _DblClick(object sender, EventArgs e){
			Label2 l = (Label2)sender;
			MessageBox.Show (_items[l.Context].Text[0]);
			OnDoubleClick(e);
		}

        //Свойства
		public void DblClick(object sender, EventArgs e, string selected){
		}

		//Методы

        //Свойства
        public List<ItemDescription> Items{
			get{
			return _items;
			
			}
			set{
				_items = value;
				_Clear();
				_Repaint();
			}
		}
    }
}
