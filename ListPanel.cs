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
			public bool Selected = false;
			public short Selection = 0;
			public Image Icon;
		}

		///<summary>
		/// Возможные состояния выделения строки
		/// Row selection statuses
		/// </summary>
		public struct SelectionStatuses{//перечень состояний выделений строки
			/// <summary>
			/// Строка никак не выделена ///
			/// The row is not selected.
			/// </summary>
			public const short NotSelected = 0;

			/// <summary>
			/// Строка подсвечена ///
			/// The row is highlighted.
			/// </summary>
			public const short Highlighted = 1;

			/// <summary>
			/// Строка выделена ///
			/// The row is selected.
			/// </summary>
			public const short Selected = 2;

			/// <summary>
			/// Строка выделена и подсвечена ///
			/// The row is selected and highlighted.
			/// </summary>
			public const short SelectedAndHighlighted = 3;

			/// <summary>
			/// Запретить выделение строки ///
			/// Disable row selection.
			/// </summary>
			public const short CannotBeSelected = 10;
		}

		//Внутренние переменные
		string[] _collumns;//заголовки столбцов //TODO: столбцы
		List<ItemDescription> _items = new List<ItemDescription>(); //элементы списка
		List<Label> Nadpis = new List<Label>(); //элемент списка (пункт)

		//Подпрограммы
        public ListPanel(){//Ну, за инициализацию!
            InitializeComponent();
        }

		public new event StringEvent DoubleClick;
        private void ListPanel_Load(object sender, EventArgs e){//Ну, за загрузку!
			_Repaint();
        }

        private void ListPanel_Resize(object sender, EventArgs e){//Ну, за деформацию!
			_Repaint ();
        }

		//Отрисовка
		private void _AddItem(string Txt, int Offset, int Context){//добавление пункта
			Label Lbl;
			Lbl = new Label();
//#if DEBUG
//			Lbl.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
//#endif
			Lbl.Left = Offset;
			if (Nadpis.Count > 0){ //если есть пункты
				Lbl.Top = Nadpis[Nadpis.Count-1].Top + Nadpis[Nadpis.Count-1].Height;
			}else{ //если их нет
				Lbl.Top = 0;
			}
			if(Offset!=0) Lbl.AutoSize = true; else Lbl.AutoSize = false; Lbl.Width = this.Width;
			Lbl.Text = Txt;
			Lbl.Tag = Context;

			Nadpis.Add (Lbl);
			this.Controls.Add (Nadpis[Nadpis.Count -1]);
			Nadpis[Nadpis.Count -1].DoubleClick += _DblClick;
			Nadpis[Nadpis.Count -1].Click += _OneClick;
			Nadpis[Nadpis.Count -1].KeyDown += _KeyDown;
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

				//Обработка выделения
				switch (x.Selection) { //todo:нормальные цвета
				case SelectionStatuses.NotSelected:
					Nadpis[Nadpis.Count -1].BackColor = System.Drawing.SystemColors.Window;
					break;
				case SelectionStatuses.Selected:
					Nadpis[Nadpis.Count -1].BackColor = System.Drawing.SystemColors.Window;
					break;
					//undone: дописать forecolor
				case SelectionStatuses.Highlighted:
					Nadpis[Nadpis.Count -1].BackColor = System.Drawing.SystemColors.MenuHighlight;
					break;
				}

				i++;
			}
		}

		private void _DblClick(object sender, EventArgs e){//обработчик двойного щелчка
			Label l = (Label)sender;
			EventArgs<string> ea = new EventArgs<string>(_items[Convert.ToInt32 (l.Tag)].Text[0]);
			DoubleClick(sender,ea);
		}

		private void _OneClick(object sender, EventArgs e){//обработчик одинарного щелчка
			foreach (ItemDescription Item in _items) {
				Item.Selection = SelectionStatuses.NotSelected;
			}
			Label l = (Label)sender;
			_items[(int)l.Tag].Selection = SelectionStatuses.Highlighted;
			_Repaint();
		}

		private void _KeyDown(object sender, KeyEventArgs e){//обработчик нажатия клавиши
			//undone
			MessageBox.Show (e.KeyData.ToString());
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
