/* The File Commander - plugin API - VirtualListView
 * The enhanced ListView widget with virtual mode inside
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
* Contributors should place own signs here.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using pluginner;
using pluginner.Toolkit;
using pluginner.Widgets;
using Xwt;
using Xwt.Drawing;

namespace pluginner.Widgets
{
	/// <summary>
	/// Virtual Mode List (playground)
	/// </summary>
	public class VirtualListView : Widget
	{
		private Table layout = new Table();
		private HBox ColumnRow = new HBox();//проверить надобность!!!
		private VScrollbar vscroll = new VScrollbar();
		private HScrollbar hscroll = new HScrollbar();
		private Canvas canvasina = new Canvas();

		private List<Label> ColumnTitles = new List<Label>();
		private int LastRow;
		private int LastCol;
		private ListView2.Views _View = ListView2.Views.Details;
		//todo: int MaxRow (для переноса при режиме Small Icons)
		private List<ListView2.ColumnInfo> _columns = new List<ListView2.ColumnInfo>();
		private bool Color2; //для обеспечения чередования цветов строк
		private DateTime PointedItemLastClickTime = DateTime.Now.AddDays(-1); //for double click detecting

		public static double MillisecondsForDoubleClick = SysInfo.DoubleClickTime; //Depends on user settings

		public event TypedEvent<ListView2Item> PointerMoved;
		public event TypedEvent<List<ListView2Item>> SelectionChanged;
		public event TypedEvent<ListView2Item> PointedItemDoubleClicked;
		public event TypedEvent<EditableLabel, ListView2> EditComplete;

		//Color sheme
		public Color NormalBgColor1 = Colors.White;
		public Color NormalBgColor2 = Colors.WhiteSmoke;
		public Color NormalFgColor1 = Colors.Black;
		public Color NormalFgColor2 = Colors.Black;
		public Color PointedBgColor = Colors.LightGray;
		public Color PointedFgColor = Colors.Black;
		public Color SelectedBgColor = Colors.White;
		public Color SelectedFgColor = Colors.Red;

		public Font FontForFileNames = Font.SystemFont;

		//For virtual mode
		int VisibleItemsByY = -1;
		int VisibleItemsByX = -1;

		/// <summary>List of items. Please do not edit directly! Please use the AddItem and RemoveItem functions.</summary>
		public List<ListView2Item> Items = new List<ListView2Item>();
		/// <summary>The pointed item</summary>
		public ListView2Item PointedItem;
		/// <summary>The list of selected items</summary>
		public List<ListView2Item> SelectedItems = new List<ListView2Item>();
		/// <summary>The rows that are allowed to be pointed by keyboard OR null if all rows are allowed</summary>
		public List<int> AllowedToPoint = new List<int>();

		private Func<int, int, IEnumerable<Widget>> source;
		private long itemCount;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Source">A pointer to a function that provides a IEnumerable of ListView2Item that should be shown; the integers are the begin and the end of the array of Items</param>
		public VirtualListView(Func<int, int, IEnumerable<Widget>> Source)
		{
			source = Source;
			Content = layout;
			BoundsChanged += (o, ea) => FillIn();

			layout.DefaultColumnSpacing = layout.DefaultRowSpacing = 0;
			layout.Add(canvasina, 1, 1);
			layout.Add(vscroll,   2, 1);
			layout.Add(hscroll,   1, 2);

			canvasina.ExpandHorizontal = true;
			canvasina.ExpandVertical = true;
			canvasina.BackgroundColor = Colors.Bisque;

			hscroll.ExpandHorizontal = true;

			vscroll.ExpandVertical = true;
			vscroll.LowerValue = 0;
			vscroll.UpperValue = 0;
			vscroll.StepIncrement = 1;
			vscroll.ValueChanged += vscroll_ValueChanged;

			hscroll.LowerValue = 0;
			hscroll.UpperValue = 0;
			hscroll.StepIncrement = 1;
			hscroll.ValueChanged += hscroll_ValueChanged;
		}

		/// <summary>
		/// Count of items in the widget
		/// </summary>
		public long ItemCount
		{
			get { return itemCount; }
			set 
			{
				itemCount = value;
				vscroll.UpperValue = value;
			}
		}

		public void FillIn()
		{
			if (!source(0, 1).Any()) return; //if there is no items, go outside

			//get the etalon sizes
			Size mySize = canvasina.Size;
			Size oneItemSize = source(0, 1).ElementAt(0).Surface.GetPreferredSize();

			if (_View == ListView2.Views.Details)//details mode (rus: Вид->Таблица)
			{
				double visibleHeight = mySize.Height;
				double itemHeight = oneItemSize.Height;
				double visibleWidth = mySize.Width;
				double itemWidth = oneItemSize.Width;

				#if DEBUG
				if (oneItemSize.IsZero){ throw new Exception("Something is wrong! Possibly, FillIn() has been called when GUI isn't ready.");}
				#endif

				VisibleItemsByY = VisibleItemsByX = -1;

				for (double i = 0; i < visibleHeight; i += itemHeight)
				{
					VisibleItemsByY++;
				}

				for (double i = 0; i < visibleWidth; i += itemWidth)
				{
					VisibleItemsByX++;
				}

				vscroll.UpperValue = itemCount - VisibleItemsByY;
				hscroll.UpperValue = visibleWidth - itemWidth; //buggy
			}

			if (VisibleItemsByY != -1)
				vscroll_ValueChanged(null, null);
		}

		//SUB-PROGRAMS

		/// <summary>
		/// Sets the pointer to an item by defined condition.
		/// </summary>
		/// <param name='Condition'>
		/// Условие (на сколько строк переместиться)
		/// </param>
		private void _SetPointerByCondition(int Condition)
		{
			/*ОПИСАНИЕ: Перенос курсора выше или ниже.
			  ПРИНЦИП: При наличии списка допущенных к выбору строк (массив номеров строк AllowedToPoint),
			  курсор прыгает в ближайшую допущенную строку в прямом направлении. При выходе из сего списка,
			  курсор может идти в том же направлении дальше без ограничений.
			  */
			int NewRow;
			if (Condition > 0)
			{
				//move bottom
				NewRow = PointedItem.RowNo + Condition;
				foreach (int r in AllowedToPoint)
				{
					if (r > NewRow - 1)
					{
						NewRow = r; break;
					}
				}

				if (NewRow < LastRow)
					_SetPoint(Items[NewRow]);
			}
			else if (Condition < 0)
			{
				//move up
				NewRow = PointedItem.RowNo - -Condition;
				for (int i = AllowedToPoint.Count - 1; i > 0; i--)
				{
					int r = AllowedToPoint[i];
					if (r < NewRow)
					{
						NewRow = r; break;
					}
				}
				if (NewRow >= 0)
					_SetPoint(Items[NewRow]);
			}
		}

		/// <summary>Inverts selection of an item</summary>
		/// <param name="lvi">The requested ListView2Item</param>
		private void _SelectItem(ListView2Item lvi)
		{
			switch (lvi.State)
			{
				case ListView2.ItemStates.Default:
					lvi.State = ListView2.ItemStates.Selected;
					SelectedItems.Add(lvi);
					break;
				case ListView2.ItemStates.Pointed:
					lvi.State = ListView2.ItemStates.PointedAndSelected;
					SelectedItems.Add(lvi);
					break;
				case ListView2.ItemStates.Selected:
				case ListView2.ItemStates.PointedAndSelected:
					_UnselectItem(lvi);
					break;
			}
			if (SelectionChanged != null) SelectionChanged(SelectedItems);
		}

		/// <summary>Removes selection of an item</summary>
		/// <param name="lvi">The requested ListView2Item</param>
		private void _UnselectItem(ListView2Item lvi)
		{
			SelectedItems.Remove(lvi);
			if (lvi.State == ListView2.ItemStates.PointedAndSelected)
				lvi.State = ListView2.ItemStates.Pointed;
			else
				lvi.State = ListView2.ItemStates.Default;
				if (SelectionChanged != null) SelectionChanged(SelectedItems);
		}

		/// <summary>Sets the pointer to an item</summary>
		/// <param name="lvi">The requested ListView2Item</param>
		private void _SetPoint(ListView2Item lvi)
		{
			//unpoint current
			if (PointedItem != null)
			{
				if ((int)PointedItem.State > 1)
					PointedItem.State = ListView2.ItemStates.Selected;
				else
					PointedItem.State = ListView2.ItemStates.Default;
			}

			//point new
			if ((int)lvi.State > 1)
				lvi.State = ListView2.ItemStates.PointedAndSelected;
			else
				lvi.State = ListView2.ItemStates.Pointed;
			PointedItem = lvi;

			var pointerMoved = PointerMoved;
			if (pointerMoved != null)
			{
				pointerMoved(lvi);
			}

			/*//if need, scroll the view
			double top = -ScrollerIn.PosY;
			double down = ScrollerIn.Size.Height;
			double newpos = lvi.Size.Height * lvi.RowNo;

			if (top > down)
			{
				//если прокручено далее первой страницы
				down = top + ScrollerIn.Size.Height;
			}

			if (newpos > down || newpos < top)
			{
				ScrollerIn.ScrollTo(-(lvi.Size.Height * lvi.RowNo));
			}*/
			//UNDONE: VirtualListView: прокрутка!!!!!!!!

			//todo: add smooth scrolling
		}


		void vscroll_ValueChanged(object sender, EventArgs e)
		{
			if (VisibleItemsByY == -1) FillIn();

			int top = (int)vscroll.Value, bottom = (int)vscroll.Value + VisibleItemsByY;
			IEnumerable<Widget> visiblew = source(top, bottom);

			canvasina.Clear();

			double YOffset = 0;
			foreach (Widget w in visiblew)
			{
				canvasina.AddChild(w, 0, YOffset);
				YOffset += w.Size.Height;
				if (YOffset >= Size.Height) break;
			}

			foreach (Widget w in canvasina.Children)
			{
				Rectangle r = canvasina.GetChildBounds(w);
				r.Left = -hscroll.Value;
				canvasina.SetChildBounds(w, r);
			}
		}

		void hscroll_ValueChanged(object sender, EventArgs e)
		{
			if (_View == ListView2.Views.Details)
			{
				foreach (Widget w in canvasina.Children)
				{
					Rectangle r = canvasina.GetChildBounds(w);
					r.Left = -hscroll.Value;
					canvasina.SetChildBounds(w, r);
				}
			}
		}

		//PUBLIC PROPERTIES

		/// <summary>Sets column configuration</summary>
		public void SetColumns(IEnumerable<ListView2.ColumnInfo> columns)
		{
			_columns.Clear();
			ColumnTitles.Clear();
			ColumnRow.Clear();
			foreach (ListView2.ColumnInfo ci in columns)
			{
				_columns.Add(ci);
				ColumnTitles.Add(new Label(ci.Title) { WidthRequest = ci.Width, Visible = ci.Visible });
				ColumnRow.PackStart(ColumnTitles[ColumnTitles.Count - 1]);
			}
		}

		/// <summary>Selected row's number</summary>
		public int SelectedRow
		{
			get { return PointedItem.RowNo; }
			set { _SetPoint(Items[value]); }
		}

		/// <summary>Gets the list of the rows that currently are choosed by the user</summary>
		public List<ListView2Item> ChoosedRows
		{
			get
			{
				if (SelectedItems.Count == 0)
				{
					List<ListView2Item> list_one = new List<ListView2Item> { PointedItem };
					return list_one;
				}
				// ReSharper disable once RedundantIfElseBlock //to ease readability
				else
				{
					return SelectedItems;
				}
			}
		}
	}
}
