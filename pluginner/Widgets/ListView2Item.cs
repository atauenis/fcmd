/* The File Commander - plugin API - ListView2
 * The ListView2 item widget
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using Xwt;
using Xwt.Drawing;

namespace pluginner.Widgets
{
	public class ListView2Item : Canvas
	{
		/// <summary>Data store</summary>
		private Object[] _Values;
		/// <summary>"Is the Field Editable" data store</summary>
		private bool[] _Editables;
		/// <summary>Column info store</summary>
		private ListView2.ColumnInfo[] _Cols;
		/// <summary>Selection state</summary>
		private ListView2.ItemStates _State;

		private Color DefBgColor;
		private Color DefFgColor;
		private Color PointBgColor;
		private Color PointFgColor;
		private Color SelBgColor;
		private Color SelFgColor;

		private Color CurFgColor;

		protected override void OnDraw(Context ctx, Rectangle dirtyRect)
		{
			base.OnDraw(ctx, dirtyRect);
			if (_Values.Length > _Cols.Length) return; //if the column count is less than the count of columns in the data, НАХУЙ ТАКУЮ РАБОТУ

			double PosByX = 0;
			for (int i = 0; i < _Values.Length; i++)
			{
				var Value = _Values[i];
				if (_Cols[i].Visible)
					Draw(Value, PosByX, ctx, _Cols[i].Width, CurFgColor);

				if (_Cols.Length > i && i != _Cols.Length - 1)
				{
					PosByX += _Cols[i].Width;
				}
			}
		}

		/// <summary>
		/// Draw a information on the ListView2Item
		/// </summary>
		/// <param name="What">What should be drawed</param>
		/// <param name="Where">Where (position by X) should be drawed</param>
		/// <param name="On">On what Drawing.Context the information should be drawed</param>
		/// <param name="MaxWidth">The limit of the picture's width</param>
		/// <param name="TextColor">The text foreground color</param>
		private void Draw(object What, double Where, Context On, double MaxWidth, Color TextColor)
		{
			if (What.GetType() != typeof (Image)
				&&
				What.GetType() != typeof (DirItem))
			{
				TextLayout tl = new TextLayout(this)
				{
					Text = What.ToString(),
					Font = Font,
					Width = MaxWidth,
					Trimming = TextTrimming.WordElipsis
				};
				On.SetColor (TextColor);
				On.DrawTextLayout(tl, Where + 4, 0);
			}
			if (What is Image)
			{
				On.DrawImage(What as Image, Where + 2, 0);
			}
		}

		public void OnDblClick()
		{
			OnButtonPressed(new ButtonEventArgs { MultiplePress = 2 });
		}

		/// <summary>Creates a new ListView2Item</summary>
		/// <param name="RowNumber">Number of owning row</param>
		/// <param name="ColNumber">Number of owning column</param>
		/// <param name="RowTag">The item's tag</param>
		/// <param name="Columns">Array of column information</param>
		/// <param name="Data">The data that should be shown in this LV2I</param>
		public ListView2Item(int rowNumber, int colNumber, string rowTag, ListView2.ColumnInfo[] columns, List<Object> data)
		{
			MinHeight = 16;
			HeightRequest = 16;
			MinWidth = 500;
			ExpandHorizontal = true;
			ExpandVertical = true;

			_Values = data.ToArray();
			_Cols = columns;
			Tag = rowTag;
			RowNo = rowNumber;
			ColNo = colNumber;

			QueueDraw();
		}

		public Color NormalBgColor
		{
			get { return DefBgColor; }
			set {
				DefBgColor = value;
				if (State == ListView2.ItemStates.Default)
					BackgroundColor = DefBgColor;
			}
		}

		public Color NormalFgColor
		{
			get { return DefFgColor; }
			set
			{
				DefFgColor = value;
				if (State == ListView2.ItemStates.Default) {
					CurFgColor = value;
					QueueDraw ();
				}
			}
		}

		public Color PointerBgColor
		{
			get { return PointBgColor; }
			set
			{
				PointBgColor = value;
				if ((int)State == 1){
					BackgroundColor = value;
				}
			}
		}

		public Color PointerFgColor
		{
			get { return PointFgColor; }
			set
			{
				PointFgColor = value;
				if ((int)State == 1){
//undone
				}
			}
		}

		public Color SelectionBgColor
		{
			get { return SelBgColor; }
			set
			{
				SelBgColor = value;
				if ((int)State >= 2){
					BackgroundColor = value;
				}
			}
		}

		public Color SelectionFgColor
		{
			get { return SelFgColor; }
			set
			{
				SelFgColor = value;
				if ((int)State >= 2){
//undone
				}
			}
		}

		/// <summary>Set the font of the row</summary>
		public new Font Font
		{
			get; set;
		}

		/// <summary>Set column list</summary>
		public ListView2.ColumnInfo[] Columns
		{
			set { _Cols = value; QueueDraw(); }
		}

		/// <summary>Status of the item selection</summary>
		public ListView2.ItemStates State
		{
			get { return _State; }
			set
			{
				_State = value;
				switch (value)
				{
					case ListView2.ItemStates.Pointed:
						BackgroundColor = PointerBgColor;
						CurFgColor = PointFgColor;
						break;
					case ListView2.ItemStates.Selected:
						BackgroundColor = SelBgColor;
						CurFgColor = SelFgColor;
						break;
					case ListView2.ItemStates.PointedAndSelected:
						//todo: replace this buggy algorythm with better one
						//дело в том, что xwt немного путает одинаковые цвета,
						//на минимальные доли, но этого достаточно для color1!=color2
						if (PointBgColor == NormalBgColor){
							BackgroundColor = SelectionBgColor;
							CurFgColor = SelectionFgColor;
						}
						else{
							BackgroundColor =
								SelBgColor.BlendWith(
								PointBgColor, 0.5
								);
							CurFgColor =
								SelFgColor.BlendWith(
								PointFgColor, 0.5
								);

						}
						break;
					default:
						BackgroundColor = DefBgColor;
						CurFgColor = DefFgColor;
						break;
				}
				QueueDraw();
			}
		}

		public int RowNo;
		public int ColNo;
		//public string Tag; //don't forgetting that the lv2 is used only for file list, so the tag can be only a string

		/// <summary>
		/// Get or set the data. Note that the data should be written fully.
		/// </summary>
		public List<Object> Data
		{
			get { return _Values.ToList(); }
			set {
				if (_Cols == null) {
					throw new Exception("Please set columns first!");
				}
				_Values = value.ToArray();
				QueueDraw();
			}
		}

		[Obsolete("Not obsolete, but not implemented yet, do not use at now!")]
		public event TypedEvent<EditableLabel> EditComplete;
	}
}
