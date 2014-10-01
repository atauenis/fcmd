using System;
using System.Collections.Generic;
using System.Linq;
using pluginner.Widgets;
using Xwt;
using Xwt.Drawing;

namespace fcmd.vm
{
	/// <summary>
	/// Virtual Mode List (playground)
	/// </summary>
	class vmlist : Widget
	{
		private Table layout = new Table();
		private VScrollbar vscroll = new VScrollbar();
		private HScrollbar hscroll = new HScrollbar();
		private Canvas canvasina = new Canvas();

		private ListView2.Views _View = ListView2.Views.Details;
		private int VisibleItemsByY = -1, VisibleItemsByX = -1;

		private Func<int, int, IEnumerable<Widget>> source;
		private long itemCount;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Source">A pointer to a function that provides a IEnumerable of ListView2Item that should be shown; the integers are the begin and the end of the array of Items</param>
		public vmlist(Func<int, int, IEnumerable<Widget>> Source)
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
	}
}
