/* The File Commander - plugin API - FListView
 * Listview-like widget with asynchronous data filling
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2015, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xwt;
using XwtColors = Xwt.Drawing.Colors;

namespace pluginner.Widgets.FListView
{
	/// <summary>
	/// File Commander virtual ListView widget
	/// </summary>
	/// <typeparam name="T">Type of information to be displayed</typeparam>
	public class FListView<T> : Widget
	{
		private Table Layout = new Table();
		private HBox ColumnRow = new HBox();
		private AutoCanvas TileCanvas = new AutoCanvas();
		private VScrollbar VScroll = new VScrollbar();
		private HScrollbar HScroll = new HScrollbar();

		private FLVColumn[] ColumnStorage; //columns' tags
		private bool DetailsModeEnabled;

		private UInt64 TileCount; //count of tiles
		private IEnumerable<T> ValueStorage; //raw data
		private TileTag[,] TileValues; //tiles' data (2D array)
		private Widget[,] VisibleWidgets;
		private bool beskonech_lock; //locking of infinity loops

		private ScrollMode sm = ScrollMode.Vertical;
		private IconSize icsz = IconSize.SmallIcons;

		public FListView(IItemRenderer ItemRenderer)
		{
			Renderer = ItemRenderer;
			Content = Layout;
			CanGetFocus = true;
			
			VisibleSize = new XY();
			VisibleRange = new Range {X0 = 0, X1 = 0, Y0 = 0, Y1 = 0};

			Layout.Add(ColumnRow,1,1,1,1,true);
			Layout.Add(TileCanvas,1,2,1,1,true,true);
			Layout.Add(VScroll,2,2);
			Layout.Add(HScroll,1,3);
			Layout.ExpandVertical = true;
			Layout.DefaultRowSpacing = Layout.DefaultColumnSpacing = 0;

			BoundsChanged += FListView_BoundsChanged;
			KeyPressed += FListView_KeyPressed;
			ButtonPressed += (o, ea) => SetFocus();
			MouseScrolled += FListView_MouseScrolled;

			VScroll.StepIncrement = HScroll.StepIncrement = 1;
			VScroll.ValueChanged += VScroll_ValueChanged;
			HScroll.ValueChanged += HScroll_ValueChanged;

			Renderer.TileSelectionChanged += Renderer_TileSelectionChanged;
		}

		/// <summary>
		/// Range of visible tiles
		/// </summary>
		public Range VisibleRange { get; set; }

		/// <summary>
		/// Size of the visible area (in tiles)
		/// </summary>
		public XY VisibleSize { get; set; }

		/// <summary>
		/// Tiles' values store
		/// </summary>
		public IEnumerable<T> Values { set { ValueStorage = value; } }

		/// <summary>
		/// Count of tiles in the FListView. This can be used to display the FListView when the array of values is not fully filled.
		/// </summary>
		[DefaultValue(0)]
		public UInt64 Capacity { get { return TileCount; } set { TileCount = value; Accommodate(); FListView_BoundsChanged(null,null);} }

		/// <summary>
		/// Item renderer. This property can be set only from the class's constructor, at any later time only getting is possible
		/// </summary>
		public IItemRenderer Renderer { get; private set; }

		/// <summary>
		/// Scrolling mode (by X or by Y)
		/// </summary>
		[DefaultValue(ScrollMode.Vertical)]
		public ScrollMode ScrollMode {
			get { return sm; }
			set
			{
				sm = value;
				Renderer.ListLayout = (sm == ScrollMode.Horizontal ? ScrollMode.Vertical : ScrollMode.Horizontal);
			}
		}

		/// <summary>
		/// Enable or disable "details view" (show/hide column headers).
		/// </summary>
		[DefaultValue(false)]
		public bool DetailsMode {
			get { return DetailsModeEnabled; }
			set
			{
				DetailsModeEnabled = value;
				Renderer.DetailsMode = value;
			}
		}

		/// <summary>
		/// Size of icons
		/// </summary>
		[DefaultValue(IconSize.SmallIcons)]
		public IconSize IconSize {
			get { return icsz; }
			set
			{
				icsz = value;
				Renderer.IconSize = icsz;
			}
		}

		/// <summary>
		/// Columns for details view
		/// </summary>
		public FLVColumn[] Columns {
			set
			{
				//draw column headers
				//todo: made a widget for column headers with support for resizing, icon for sort order, etc
				for (int i = value.Length; i > 0; i--)
				{
					if(i!=1)
						ColumnRow.PackEnd(new Label(value[i-1].Title));
					else
						ColumnRow.PackEnd(new Label(value[i-1].Title), true, WidgetPlacement.Fill);
				}
				ColumnStorage = value;
				Renderer.Columns = ColumnStorage;
			}
			get { return ColumnStorage; }
		}

		/// <summary>
		/// List of tiles selected by user
		/// </summary>
		public List<TileTag> SelectedTiles = new List<TileTag>();

		/// <summary>
		/// The value of tile pointed by user
		/// </summary>
		public T PointedValue { get; private set; }

		/// <summary>
		/// The current pointed tile;
		/// </summary>
		public Widget PointedTile { get; private set; }

		/// <summary>
		/// Is the widget ready for interaction with user
		/// </summary>
		//public new bool Sensitive { get; set; }

		/// <summary>
		/// Event that fire when tile's selection state has been changed
		/// </summary>
		public event EventHandler<SelectionChangedEventArgs> TileSelectionChanged;

		/// <summary>
		/// Accommodate tiles in the grid (fill TileValues[x,y] array)
		/// </summary>
		private void Accommodate()
		{
			if(ScrollMode == ScrollMode.Horizontal) throw new NotImplementedException("Горизонтальной прокрутки пока нет");
			if(IconSize > IconSize.SmallIcons) throw new NotImplementedException("Пока поддерживаются только иконки 16х16");

			//fill as "small icons"/"details"
			int vsc = ValueStorage.Count(); //cache for the Count, added to prevent overflows if the Count will change suddenly
			TileValues = new TileTag[1, vsc] ;
			for (int i = 0; i < vsc; i++)
			{
				TileValues[0, i] = new TileTag {Raw = ValueStorage.ElementAt(i)};
			}

			VScroll.UpperValue = TileValues.GetLength(1) - VisibleSize.Y;
			HScroll.UpperValue = TileValues.GetLength(0) - VisibleSize.X;
		}

		/// <summary>
		/// Render & draw visible tiles
		/// </summary>
		private void DrawVisible()
		{
			TileCanvas.Clear();
			VisibleWidgets = new Widget[VisibleSize.X+1,VisibleSize.Y+1];
			double xoffset = 0; //horizontal offset in pixels (for multi-collumn display)

			for (int ix = VisibleRange.X0; ix <= VisibleRange.X1; ix++)
			{
				if (ix >= TileValues.GetLength(0)) break;
				for (int iy = VisibleRange.Y0; iy <= VisibleRange.Y1; iy++)
				{
					if (iy >= TileValues.GetLength(1)) break;

					TileTag tt = TileValues[ix, iy];
					tt.Y = iy - VisibleRange.Y0;
					tt.X = ix - VisibleRange.X0;
					Widget w = Renderer.RenderWidget(tt);
					w.ButtonPressed += Tile_ButtonPressed;
					if (tt.SelectionStatus == SelectionStatus.Pointed || tt.SelectionStatus == SelectionStatus.PointedAndSelected) PointedTile = w;

					TileCanvas.AddChildY(w, (int) xoffset * ix); //todo: проработать высчитывание позиции по Х
					VisibleWidgets[ix - VisibleRange.X0, iy - VisibleRange.Y0] = w;
				}
				if (VisibleWidgets[0, 0] != null) xoffset += VisibleWidgets[0, 0].Size.Width + TileCanvas.HorizontalSpacing;
			}
		}

		/// <summary>
		/// Calculates visible space and displaying ranges
		/// </summary>
		private void CalculateVisibleSpace()
		{
			//fallback if any errors occured
			VisibleSize.X = 0;
			VisibleSize.Y = 0;
			VisibleRange.X1 = VisibleRange.X0 + 0;
			VisibleRange.Y1 = VisibleRange.Y0 + 0;

			//calculate the capacity of the visible area when it is possible (at least one tile is drawed) and only 1 time
			if (VisibleWidgets != null && !beskonech_lock)
			{
				if (VisibleWidgets.GetLength(1) != 0)
				{
					beskonech_lock = true;
					if (VisibleWidgets.GetValue(0, 0) != null)
					{
						double YSizePx = TileCanvas.Size.Height - HScroll.Size.Height;
						double YSizeOne = VisibleWidgets[0, 0].Size.Height + TileCanvas.VerticalSpacing;
						double YCapacity = Math.Round(YSizePx / YSizeOne - 0.5);
						VisibleSize.Y = Convert.ToInt32(YCapacity) + 1;
						VisibleRange.Y1 = VisibleRange.Y0 + VisibleSize.Y;

						double XSizePx = TileCanvas.Size.Width - VScroll.Size.Height ;
						double XSizeOne = VisibleWidgets[0, 0].Size.Width + TileCanvas.HorizontalSpacing;
						double XCapacity = Math.Round(XSizePx / XSizeOne - 0.5);
						VisibleSize.X = Convert.ToInt32(XCapacity) + 1 ;
						VisibleRange.X1 = VisibleRange.X0 + VisibleSize.X;
					}
				}
			}

			//calculate 1st column width if details mode is enabled
			if (DetailsMode && Columns != null)
			{
				if (Columns.Length > 1)
				{
					int firstcolx = (int)ColumnRow.Children.ElementAt(Columns.Length - 1).Size.Width;
					ColumnStorage[0].Size = firstcolx;
				}
			}

		}

		/// <summary>
		/// Fires when a mouse button has been pressed, while mouse pointer is over a tile
		/// </summary>
		void Tile_ButtonPressed(object sender, ButtonEventArgs e)
		{
			if (e.Handled) return;
			if(e.Button == PointerButton.Left)
				PointTile(sender as Widget);
			else if(e.Button == PointerButton.Right)
				SelectTile(sender as Widget);
		}

		/// <summary>
		/// Fires when a keyboard button was pressed
		/// </summary>
		void FListView_KeyPressed(object sender, KeyEventArgs e)
		{
			int Ycur, Ynew, Xcur, Xnew;
			Ycur = ((TileTag) PointedTile.Tag).Y;
			Xcur = ((TileTag) PointedTile.Tag).X;
			Ynew = Ycur;
			Xnew = Xcur;

			switch (e.Key)
			{
				case Key.NumPadDown:
				case Key.Down:
					Ynew = Ycur + 1;
					break;
				case Key.NumPadUp:
				case Key.Up:
					Ynew = Ycur - 1;
					break;
				case Key.NumPadRight:
				case Key.Right:
					Xnew = Xcur + 1;
					break;
				case Key.NumPadLeft:
				case Key.Left:
					Xnew = Xcur - 1;
					break;
				default:
					#if DEBUG
					Console.WriteLine("DEBUG: (FListView) It isn't need to move pointer");
					#endif
					return;
			}
			#if DEBUG
			Console.WriteLine("DEBUG: (FListView) The pointer is at ({0};{1}), now will move to ({2};{3})", Xcur, Ycur, Xnew, Ynew);
			if(!PointTile(Xnew, Ynew)) Console.WriteLine("DEBUG: (FListView) The pointer didn't moved.");
			#endif
		}

		/// <summary>
		/// Fires when a mouse wheel has been rolled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void FListView_MouseScrolled(object sender, MouseScrolledEventArgs e)
		{
			//todo: allow to select whether the pointer should be moved or the current view should be scrolled on mouse wheel rolling
			switch (e.Direction)
			{
				case ScrollDirection.Down:
					VScroll.Value += VScroll.StepIncrement*3;
					break;
				case ScrollDirection.Up:
					VScroll.Value -= VScroll.StepIncrement*3;
					break;
				case ScrollDirection.Left:
					HScroll.Value -= HScroll.StepIncrement*3;
					break;
				case ScrollDirection.Right:
					HScroll.Value += HScroll.StepIncrement*3;
					break;
			}
				VScroll_ValueChanged(null,EventArgs.Empty); //hack due to XWT error! нужно протестировать со свежим XWT и в случае чего завести баг в трекере xwt
		}

		/// <summary>
		/// Used when FLV bounds are changed or the FLV should accept it current size
		/// </summary>
		void FListView_BoundsChanged(object sender, EventArgs e)
		{
			//calculate visible space size (in widgets)
			CalculateVisibleSpace();

			//and draw the current view
			DrawVisible();
			beskonech_lock = false;

			//update scroll bars max. values
			VScroll.UpperValue = TileValues.GetLength(1) - VisibleSize.Y;
			HScroll.UpperValue = TileValues.GetLength(0) - VisibleSize.X;
		}

		void VScroll_ValueChanged(object sender, EventArgs e)
		{
			VisibleRange.Y0 = (int)Math.Round(VScroll.Value);
			VisibleRange.Y1 = VisibleRange.Y0 + VisibleSize.Y;
			DrawVisible();
		}

		void HScroll_ValueChanged(object sender, EventArgs e)
		{
			VisibleRange.X0 = (int)Math.Round(HScroll.Value);
			VisibleRange.X1 = VisibleRange.X0 + VisibleSize.X;
			DrawVisible();
		}

		void Renderer_TileSelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			TileTag tt = ((Widget) sender).Tag as TileTag;
			if (e.NewStatus == SelectionStatus.Selected || e.NewStatus == SelectionStatus.PointedAndSelected)
			{
				if(!SelectedTiles.Contains(tt)) SelectedTiles.Add(tt);
			}
			else
			{
				if (SelectedTiles.Contains(tt)) SelectedTiles.Remove(tt);
			}

			if (TileSelectionChanged != null) TileSelectionChanged(sender, e);
		}

		/// <summary>
		/// Sets selection mode on a tile
		/// </summary>
		/// <param name="Tile">The tile</param>
		/// <param name="Selected"><value>true</value>=select; <value>false</value>=deselect; <value>null</value>=inverse selection mode</param>
		void SelectTile(Widget Tile, bool? Selected = null)
		{
			TileTag ptt = (TileTag)Tile.Tag;
			switch (ptt.SelectionStatus)
			{
				case SelectionStatus.None:
					if(Selected == null || Selected == true) ptt.ChangeSelectionState(Tile,SelectionStatus.Selected);
					return;
				case SelectionStatus.Pointed:
					if (Selected == null || Selected == true) ptt.ChangeSelectionState(Tile, SelectionStatus.PointedAndSelected);
					return;
				case SelectionStatus.PointedAndSelected:
					if(Selected == null || Selected == false) ptt.ChangeSelectionState(Tile, SelectionStatus.Pointed);
					return;
				case SelectionStatus.Selected:
					if(Selected == null || Selected == false) ptt.ChangeSelectionState(Tile, SelectionStatus.None);
					return;
			}
		}

		/// <summary>
		/// Set the pointer to a tile with specifed coordinates
		/// </summary>
		/// <returns><value>true</value> if the pointer is successfully moved, or <value>false</value> if the moving cannot be performed</returns>
		bool PointTile(int x, int y)
		{
			if (TileValues.GetLength(0) >= x && TileValues.GetLength(1) >= y) //is present
			{
				if (x < 0 || y < 0) return false; //cannot move pointer to negative coordinates
				if (VisibleWidgets.GetLength(0) > x && VisibleWidgets.GetLength(1) > y) //if visible
				{
					if (VisibleWidgets[x, y] != null) //if the widget by (X;Y) is on the screen and is really presents
						PointTile(VisibleWidgets[x, y]);
					else
						return false;
				}
				else
				{
					//undone: scroll and set pointer...
					//PointTile(VisibleWidgets[x, y]);
					Console.WriteLine("Cant scroll outside current screen!");
				}
				return true;
			}
			
			return false;
		}

		/// <summary>
		/// Set the pointer to a tile
		/// </summary>
		/// <param name="Tile">The tile that should receive the pointing</param>
		void PointTile(Widget Tile)
		{
			if (PointedTile == Tile) return; //don't reselect current tile
			UnpointTile();
			PointedTile = Tile;
			TileTag ptt = (TileTag) PointedTile.Tag;
			PointedValue = (T)Convert.ChangeType(ptt.Raw, typeof(T));
			switch (ptt.SelectionStatus)
			{
				case SelectionStatus.None:
				case SelectionStatus.Pointed:
					ptt.ChangeSelectionState(PointedTile,SelectionStatus.Pointed);
					break;
				case SelectionStatus.Selected:
				case SelectionStatus.PointedAndSelected:
					ptt.ChangeSelectionState(PointedTile,SelectionStatus.PointedAndSelected);
					break;
			}
		}

		/// <summary>
		/// Removes pointing from current tile
		/// </summary>
		void UnpointTile()
		{
			Widget lasttile = PointedTile;
			PointedTile = null;
			if (lasttile == null) return;
			TileTag ptt = (TileTag) lasttile.Tag;
			switch (ptt.SelectionStatus)
			{
				case SelectionStatus.Pointed:
					ptt.ChangeSelectionState(lasttile, SelectionStatus.None);
					break;
				case SelectionStatus.PointedAndSelected:
					ptt.ChangeSelectionState(lasttile, SelectionStatus.Selected);
					break;
			}
		}
	}
}
