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
		public struct CollumnOptions{//параметры столбца
			/// <summary>
			/// Заголовок столбца ///
			/// The collumn's caption.
			/// </summary>
			public string Caption;

			/// <summary>
			/// Метка слобца (для определения что это такое) ///
			/// The collumn's tag.
			/// </summary>
			public string Tag;

			/// <summary>
			/// Ширина и высота столбца ///
			/// The collumn's size.
			/// </summary>
			public System.Drawing.Size Size;
		}

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
		List<CollumnOptions> _collumns = new List<CollumnOptions>();//заголовки столбцов //TODO: столбцы
		List<ItemDescription> _items = new List<ItemDescription>(); //элементы списка
		List<Label> lblNadpis = new List<Label>(); //элемент списка (пункт)
		List<Label> lblCaption = new List<Label>(); //заголовки столбцов

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
			_collumns = Collumns; //ибо коллекции в свойствах работают иногда через анус
			Label Lbl;
			Lbl = new Label();
			Lbl.Left = Offset;
			if (lblNadpis.Count > 0){ //если есть какие-то пункты
				Lbl.Top = lblNadpis[lblNadpis.Count-1].Top + lblNadpis[lblNadpis.Count-1].Height;
			}else{ //если пунктов нет вообще
				if(_collumns.Count != 0){ //если есть столбцы
					Lbl.Top = lblCaption[0].Height;
				}else{
					Lbl.Top = 0;
				}
			}
			if(Offset!=0) Lbl.AutoSize = true; else Lbl.AutoSize = false; Lbl.Width = this.Width;
			Lbl.Text = Txt;
			Lbl.Tag = Context;

			lblNadpis.Add (Lbl);
			this.Controls.Add (lblNadpis[lblNadpis.Count -1]);
			lblNadpis[lblNadpis.Count -1].DoubleClick += _DblClick;
			lblNadpis[lblNadpis.Count -1].Click += _OneClick;
			lblNadpis[lblNadpis.Count -1].KeyDown += _KeyDown;
		}
		private void _Clear(){//очистка формы
			lblNadpis.Clear();
			this.Controls.Clear();
		}
		private void _Repaint(){ //перерисовка экрана
			_Clear();

			if(_collumns.Count != 0){ //если есть столбцы
				//Отрисовка заголовков столбцов
				int ColNo = 0; //номер текущего столбца
				foreach (CollumnOptions Col in _collumns) {

					string Cap = Col.Caption;//Cap=the CAPtion of the collumn
					Label Collbl = new Label();
					Collbl.Text = Cap;
					Collbl.BackColor = SystemColors.ButtonFace;
					Collbl.BorderStyle = BorderStyle.Fixed3D;

					//Вычисляю отступ
					int ColOffset = 0;
					if(ColNo != 0){
						for (int ccic = 0; ccic < ColNo; ccic++) { ///ccic=current collumn in cycle
							ColOffset+=_collumns[ccic].Size.Width;
						}
					}else ColOffset = 0;

					Collbl.Left = ColOffset;
					lblCaption.Add (Collbl);
					this.Controls.Add (lblCaption[lblCaption.Count -1]);

					ColNo++;
				}

			}

			//отрисовка элементов
			int i = 0; //номер текущего элемента в БД
			foreach (ItemDescription x in _items){ //цикл по массиву элементов (_items[])

				if(_collumns.Count != 0){
					int ColNo2 = 0; //лучше ничего не придумал
					foreach (CollumnOptions ThisCollumn in _collumns) {
						//Вычисляю отступ
						int ItOffset = 0;
						if(ColNo2 != 0){
							for (int ccic = 0; ccic < ColNo2; ccic++) { ///ccic=current collumn in cycle
								ItOffset+=_collumns[ccic].Size.Width;
							}
						}else ItOffset = 0;
						_AddItem(x.Text[ColNo2],ItOffset,i);
						ColNo2++;
					}
				}else{
					_AddItem (x.Text[0],0, i);
				}

				//Обработка выделения
				switch (x.Selection) { //TODO:нормальные цвета
				case SelectionStatuses.NotSelected:
					lblNadpis[lblNadpis.Count -1].BackColor = SystemColors.Window;
					lblNadpis[lblNadpis.Count -1].ForeColor = SystemColors.ControlText;
					break;
				case SelectionStatuses.Selected:
					lblNadpis[lblNadpis.Count -1].BackColor = SystemColors.Window;
					lblNadpis[lblNadpis.Count -1].ForeColor = SystemColors.Highlight;
					break;
				case SelectionStatuses.Highlighted:
					lblNadpis[lblNadpis.Count -1].BackColor = SystemColors.HotTrack;
					lblNadpis[lblNadpis.Count -1].ForeColor = SystemColors.HighlightText;
					break;
				}

				i++;
			}
		}

		private void _DblClick(object sender, EventArgs e){//обработчик двойного щелчка
			Label l = (Label)sender;
			EventArgs<string> ea = new EventArgs<string>(_items[Convert.ToInt32 (l.Tag)].Value);
			DoubleClick(sender,ea);
		}

		private void _OneClick(object sender, EventArgs e){//обработчик одинарного щелчка
			foreach (ItemDescription Item in _items) {
				Item.Selection = SelectionStatuses.NotSelected;
			}
			Label l = (Label)sender;
			//MessageBox.Show(l.Tag.ToString());
			_items[(int)l.Tag].Selection = SelectionStatuses.Highlighted;
			_Repaint();
		}

		private void _KeyDown(object sender, KeyEventArgs e){//обработчик нажатия клавиши
			//UNDONE (под моно не фурычит, а под дотнетом не тестировал)
			MessageBox.Show (e.KeyData.ToString());
			switch (e.KeyCode) {
			case Keys.Up:
				//стрелка вверх
				foreach (ItemDescription id in _items){
					id.Selection = SelectionStatuses.NotSelected;
				}
				Label lup = (Label)sender;
				_items[(int)lup.Tag - 1].Selection = SelectionStatuses.Highlighted;
				_Repaint();
				break;
			case Keys.Down:
				//стрелка вниз
				foreach (ItemDescription id in _items){
					id.Selection = SelectionStatuses.NotSelected;
				}
				Label ldn = (Label)sender;
				_items[(int)ldn.Tag + 1].Selection = SelectionStatuses.Highlighted;
				_Repaint();
				break;
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
				_Clear();
				_Repaint();
			}
		}

		public List<CollumnOptions> Collumns{
			get{return _collumns;}
			set{_collumns = value;}
		}
    }
}
