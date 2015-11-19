/* The File Commander - plugin API - FListView
 * FListView STRUCTs
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2015, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;

namespace pluginner.Widgets.FListView
{
	/// <summary>
	/// Defines a point with some coordinates
	/// </summary>
	public class XY
	{
		public int X;
		public int Y;
	}

	/// <summary>
	/// Диапазон чего-то в плокости
	/// </summary>
	public class Range
	{
		public int X0;
		public int X1;
		public int Y0;
		public int Y1;

		public bool XEmpty { get { return X0 < 1 && X1 < 1; } }
		public bool YEmpty { get { return Y0 < 1 && Y1 < 1; } }
	}

	/// <summary>
	/// FListView tile data
	/// </summary>
	public class TileTag
	{
		private SelectionStatus selstatus = SelectionStatus.None;
		/// <summary>
		/// Tile's raw data
		/// </summary>
		public object Raw;

		/// <summary>
		/// Horizontal coordinate on the screen
		/// </summary>
		public int X;

		/// <summary>
		/// Vertical coordinate on the screen
		/// </summary>
		public int Y;

		/// <summary>
		/// Tile status (selected, not selected, etc)
		/// </summary>
		public SelectionStatus SelectionStatus {
			get { return selstatus; }
		}

		/// <summary>
		/// Set the selection status
		/// </summary>
		/// <param name="Where">The owner of this Tag</param>
		/// <param name="NewStatus">The new status of selection</param>
		public void ChangeSelectionState(object Where, SelectionStatus NewStatus)
		{
				selstatus = NewStatus;
				if(SelectionChanged != null) SelectionChanged(Where, new SelectionChangedEventArgs(NewStatus));
		}

		/// <summary>
		/// Fires when tile's selection state has been changed
		/// </summary>
		public event EventHandler<SelectionChangedEventArgs> SelectionChanged;
	}

	public class SelectionChangedEventArgs : EventArgs
	{
		public SelectionStatus NewStatus;

		public SelectionChangedEventArgs(SelectionStatus ss)
		{
			NewStatus = ss;
		}
	}

	/// <summary>
	/// Possible modes of scrolling area
	/// </summary>
	public enum ScrollMode
	{
		Vertical, Horizontal
	}

	/// <summary>
	/// Standart sizes of titles (but not only available!)
	/// </summary>
	public enum IconSize
	{
		/// <summary>
		/// Small icons (details, small icons)
		/// </summary>
		SmallIcons = 16,
		/// <summary>
		/// Large icons (standart icons)
		/// </summary>
		LargeIcons = 32,
		/// <summary>
		/// Extra large icons (information tiles)
		/// </summary>
		ExtraLargeIcons = 48,
		/// <summary>
		/// File content previews (thumbanials)
		/// </summary>
		Thumbanials = 64
	}

	/// <summary>
	/// Tile statuses: selected, not selected, etc
	/// </summary>
	public enum SelectionStatus
	{
		/// <summary>
		/// The tile is only visible
		/// </summary>
		None = 0,
		/// <summary>
		/// The tile is under cursor
		/// </summary>
		Pointed = 1,
		/// <summary>
		/// The tile is selected, and actions may be applied to it
		/// </summary>
		Selected = 2,
		/// <summary>
		/// The tile is selected and pointed
		/// </summary>
		PointedAndSelected = 3
	}

	public class FLVColumn
	{
		public string Id="";
		public string Title = "COLUMN";
		public int Size = -1;
		public bool? SortIcon = null;
		public bool AlignmentRight = false;
	}
}
