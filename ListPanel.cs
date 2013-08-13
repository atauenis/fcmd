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
			public const short CannotBeSelected = -1;
		}

		//Внутренние переменные
		List<CollumnOptions> _collumns = new List<CollumnOptions>();//заголовки столбцов
		List<ItemDescription> _items = new List<ItemDescription>(); //элементы списка
		List<Label> lblCaption = new List<Label>(); //заголовки столбцов
		List<List<Object>> Stroki = new List<List<Object>>(); //список строк
		Panel ScrollPanel = new Panel();
		bool showColTitles = false; //отображать ли заголовки столбцов?
		pluginner.IFSPlugin FsPlugin;

		//Подпрограммы
        public ListPanel(){//Ну, за инициализацию!
            InitializeComponent();
        }

		public new event StringEvent DoubleClick;

        private void ListPanel_Load(object sender, EventArgs e){//Ну, за загрузку!
			this.GotFocus += ListPanel_GotFocus;
			this.LostFocus += ListPanel_LostFocus;
			this.MouseClick += (snd, ea) => this.Focus();

			_Repaint();
			ScrollPanel.Width = this.Width;
			ScrollPanel.Height = this.Height;
			ScrollPanel.Location = new Point(0,0);
			ScrollPanel.AutoScroll = true;
			this.Controls.Add (ScrollPanel);
        }

        private void ListPanel_Resize(object sender, EventArgs e){//Ну, за деформацию!
			_Repaint ();
			if(showColTitles){
				ScrollPanel.Top = lblCaption[0].Height;
				ScrollPanel.Height = this.Height - lblCaption[0].Height;
			}else{
				ScrollPanel.Top = 0;
				ScrollPanel.Height = this.Height;
			}
			ScrollPanel.Width = this.Width;
        }

		private int GetPercents(int Input, int Percents){//высчитать Percents % от Input
			int OnePercent = Input / 100;
			return OnePercent * Percents;
		}

		//Отрисовка
		private void _AddItem(string Txt, int Row, int Collumn, short Selection){//добавление пункта в Stroki[][] и Controls[]
			_collumns = Collumns; //ибо коллекции в свойствах работают иногда через анус
			Label Lbl;
			Lbl = new Label();
			Lbl.Text = Txt;
			Lbl.Tag = Row;
			int Offset = 0; //отступ по горизонтали
			for (int i = 0; i < Collumn; i++) {//перебираю столбцы пока не достигну текущего
				Offset += _collumns[i].Size.Width;
			}
			Lbl.Left = Offset;

			Lbl.Top = Lbl.Height * Row + 1; //todo:заменить на цикл, суммирующий высоту всех строк
			Lbl.Width = _collumns[Collumn].Size.Width;

			//Обрабатываю выделение
			if(_items[Row].Selection > 0){
				Lbl.BackColor = SystemColors.HotTrack;
				Lbl.ForeColor = SystemColors.HighlightText;
			}else{
				Lbl.BackColor = this.BackColor;
				Lbl.ForeColor = this.ForeColor;
			}

			//Вношу в форму
			Lbl.Click += _OneClick;
			Lbl.DoubleClick += _DblClick;
			Lbl.KeyDown += _KeyDown;
			ScrollPanel.Controls.Add(Lbl);

			//undone
//			lblNadpis.Add (Lbl);
//			this.Controls.Add (lblNadpis[lblNadpis.Count -1]);
//			lblNadpis[lblNadpis.Count -1].DoubleClick += _DblClick;
//			lblNadpis[lblNadpis.Count -1].Click += _OneClick;
//			lblNadpis[lblNadpis.Count -1].KeyDown += _KeyDown;
		}
		private void _Clear(){//очистка формы
			//undone
			Stroki.Clear ();
			ScrollPanel.Controls.Clear();
		}
		private void _Repaint(){ //перерисовка экрана
			_Clear();

			if(showColTitles){ //если столбцы рисовать надо
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
					Collbl.Width = Col.Size.Width;
					lblCaption.Add (Collbl);
					this.Controls.Add (lblCaption[lblCaption.Count -1]);

					ColNo++;
				}
			}

			//отрисовка элемнтов
			int CurRowNo = 0;
			foreach(ItemDescription CurItem in _items){//паршу массив значений строк

				for (int CurColNo = 0; CurColNo < CurItem.Text.Count; CurColNo++) {//паршу перечень столбцов
					_AddItem(CurItem.Text[CurColNo],CurRowNo,CurColNo,_items[CurRowNo].Selection);
				}

				CurRowNo ++;
			}
//Обработка выделения
//				switch (x.Selection) { //TODO:нормальные цвета
//					//undone
//				case SelectionStatuses.NotSelected:
//					lblNadpis[lblNadpis.Count -1].BackColor = SystemColors.Window;
//					lblNadpis[lblNadpis.Count -1].ForeColor = SystemColors.ControlText;
//					break;
//				case SelectionStatuses.Selected:
//					lblNadpis[lblNadpis.Count -1].BackColor = SystemColors.Window;
//					lblNadpis[lblNadpis.Count -1].ForeColor = SystemColors.Highlight;
//					break;
//				case SelectionStatuses.Highlighted:
//					lblNadpis[lblNadpis.Count -1].BackColor = SystemColors.HotTrack;
//					lblNadpis[lblNadpis.Count -1].ForeColor = SystemColors.HighlightText;
//					break;
//				}
//				i++;
//			}

		}

		private void _Unhighlight(int Row){ //снять подсветку со строки
			_items[Row].Selected = false;
			_items[Row].Selection = SelectionStatuses.NotSelected;
		}

		private void _Unhighlight(){//снять подсветку со всех строк
			foreach(ItemDescription id in _items){
				id.Selected = false;
				id.Selection = SelectionStatuses.NotSelected;
			}
		}

		private void _Highlight(int Row){//выделить строку
			_Unhighlight();
			_items[Row].Selected = true;
			_items[Row].Selection = SelectionStatuses.Highlighted;
		}

		private void _DblClick(object sender, EventArgs e){//обработчик двойного щелчка по надписи
			Label l = (Label)sender;
			EventArgs<string> ea = new EventArgs<string>(_items[Convert.ToInt32 (l.Tag)].Value);
			DoubleClick(this,ea);
		}

		private void _OneClick(object sender, EventArgs e){//обработчик одинарного щелчка по надписи
			_Unhighlight(); //снимаю выделение со всех.
			Label l = (Label)sender;
			_Highlight((int)l.Tag);
			_Repaint();
		}

		private void _KeyDown(object sender, KeyEventArgs e){//обработчик нажатия клавиши
			//UNDONE (под моно не вызывается, а под дотнетом не тестировал)
			MessageBox.Show (e.KeyData.ToString()); //убрать!!!
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

		//для обеспечения фокусировки
		void ListPanel_LostFocus(object sender, EventArgs e){ //потеря фокуса
			Invalidate();
		}
		void ListPanel_GotFocus(object sender, EventArgs e){
			//do nothing
		}

		//Методы
		/// <summary>
		/// Перерисовать панель ///
		/// Update this panel
		/// </summary>
		public void Redraw(){ //HACK: должно само обновляться
			_Repaint();
		}

        //Свойства
		/// <summary>
		/// Gets or sets the file system driver.
		/// </summary>
		/// <value>
		/// The file system driver.
		/// </value>
		public pluginner.IFSPlugin FSProvider{
			get{return FsPlugin;}
			set{FsPlugin = value;}
		}

		/// <summary>
		/// Список элементов лист-панели ///
		/// Gets or sets the items.
		/// </summary>
		/// <value>
		/// The items.
		/// </value>
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

		/// <summary>
		/// Столбцы лист-панли ///
		/// Gets or sets the collumns.
		/// </summary>
		/// <value>
		/// The collumns.
		/// </value>
		public List<CollumnOptions> Collumns{
			get{return _collumns;}
			set{_collumns = value;}
		}

		public bool ShowCollumnTitles{
			get{return showColTitles;}
			set{showColTitles = value;}
		}
    }
}
