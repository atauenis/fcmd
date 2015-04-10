/* The File Commander - plugin API - VirtualListView
 * VLV data source
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2015, Alexander Tauenis (atauenis@yandex.ru)
* Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pluginner.Widgets;
using Xwt;
using Xwt.Drawing;

namespace pluginner.Toolkit
{
	/// <summary>
	/// VirtualListView data source
	/// </summary>
	public class VLVDataSource //UNDONE
	{
		private List<Widget> db = new List<Widget>();

		/// <summary>
		/// Provides "tangible" "items" for Virtual List View (the "items" are stored in the internal database)
		/// </summary>
		/// <param name="from">The first item that should be returned</param>
		/// <param name="to">The last item that should be returned</param>
		/// <returns>Enumeration of Xwt widgets that should be displayed in the VLV</returns>
		public IEnumerable<Widget> Flange(int from, int to)
		{
			List<Widget> output = new List<Widget>();
			for (int i = from; i < to; i++)
			{
				output.Add(db[i]);
			}
			return output;
		}

		/// <summary>Add a new item</summary>
		/// <param name="Data">The item's content</param>
		/// <param name="EditableFields">List of editable fields</param>
		/// <param name="ItemTag">The tag for the new item (optional)</param>
		public void AddItem(List<Object> Data, List<Boolean> EditableFields, string ItemTag = null)
		{
			/*ListView2Item lvi = new ListView2Item(
				LastRow,
				LastCol,
				ItemTag,
				_columns.ToArray(),
				Data,
				FontForFileNames)
			{
				Font = Font.SystemSansSerifFont.WithWeight(FontWeight.Heavy),
				PointerBgColor = PointedBgColor,
				PointerFgColor = PointedFgColor,
				SelectionBgColor = SelectedBgColor,
				SelectionFgColor = SelectedFgColor,
				State = ListView2.ItemStates.Default
			};
			AddItem(lvi);*/
		}

		/// <summary>Add a new ListView2Item into this ListView2</summary>
		/// <param name="Item">The new ListView2Item</param>
		private void AddItem(ListView2Item Item)
		{
			/*if (Color2)
			{
				Item.NormalBgColor = NormalBgColor2;
				Item.NormalFgColor = NormalFgColor1;
			}
			else
			{
				Item.NormalBgColor = NormalBgColor1;
				Item.NormalFgColor = NormalFgColor1;
			}

			Color2 = !Color2;
			db.Add(Item);
			Item.ButtonPressed += Item_ButtonPressed;
			Item.EditComplete += sender =>
			{
				var handler = EditComplete;
				if (handler != null)
				{
					handler(sender, this);
				}
			};
			Item.CanGetFocus = true;
			if (LastRow == 0) _SetPoint(Item);
			LastRow++;*/
		}


		/// <summary>
		/// Add some new widgets into database
		/// </summary>
		public void AddItems(IEnumerable<Widget> w)
		{
			db.AddRange(w);
		}
		
		/// <summary>
		/// Clear the database
		/// </summary>
		public void Clear()
		{
			db.Clear();
		}

		/// <summary>
		/// The count of the "items" in the memory
		/// </summary>
		public Int64 Count
		{
			get { return db.Count; }
		}
	}
}
