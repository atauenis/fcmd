/* The File Commander - plugin API
 * A scrollable panel
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using System;
using Xwt;

namespace pluginner.Widgets
{
	/// <summary>A scrollable panel, that can be 100% controlled by the host</summary>
	public class HeavyScroller : Widget
	{
		Table Layout = new Table();
		Canvas Locator = new Canvas() ;
		Widget Child = new Label("No child is inserted");
		HScrollbar HScroll = new HScrollbar();
		VScrollbar VScroll = new VScrollbar();
		double OffsetX;
		double OffsetY;

		public HeavyScroller()
		{
			Locator.AddChild(Child);
			Layout.Add(Locator, 0, 0, 1, 1, true, true);
			Layout.Add(VScroll, 1, 0);
			Layout.Add(HScroll, 0, 1);
			base.Content = Layout;

			BoundsChanged += HeavyScroller_BoundsChanged;
			VScroll.ValueChanged += VScroll_ValueChanged;
			HScroll.ValueChanged += HScroll_ValueChanged;
			MouseScrolled += HeavyScroller_MouseScrolled;
		}


		void HeavyScroller_MouseScrolled(object sender, MouseScrolledEventArgs e)
		{
			switch (e.Direction)
			{
				case ScrollDirection.Down:
					ScrollTo(OffsetY - 10);
					return;
				case ScrollDirection.Up:
					ScrollTo(OffsetY + 10);
					return;
				case ScrollDirection.Right:
					ScrollTo(null, OffsetX + 10);
					return;
				case ScrollDirection.Left:
					ScrollTo(null, OffsetX - 10);
					return;
			}
		}

		void HeavyScroller_BoundsChanged(object sender, EventArgs e)
		{
			VScroll.LowerValue = HScroll.LowerValue = 0;
			VScroll.StepIncrement = 1;
			VScroll.UpperValue = Child.Surface.GetPreferredSize().Height;
			HScroll.UpperValue = Child.Surface.GetPreferredSize().Width;

			Scroll();
		}

		void HScroll_ValueChanged(object sender, EventArgs e)
		{
			ScrollTo(null, -HScroll.Value, false);
		}

		void VScroll_ValueChanged(object sender, EventArgs e)
		{
			ScrollTo(-VScroll.Value, null, false);
		}

		/// <summary>Materializes the current position of content in this scrollable view.</summary>
		void Scroll()
		{
			//To simply scroll the content, please call ScrollTo()!
			Locator.SetChildBounds(Child, new Rectangle(OffsetX,OffsetY,Child.Surface.GetPreferredSize().Width,Child.Surface.GetPreferredSize().Height));
		}

		/// <summary>Scrolls this scroller to the specifed coordinates</summary>
		/// <param name="y">The new coordinate by vertical axis or null if do not change</param>
		/// <param name="x">The new coordinate by horizontal axis or null if do not change</param>
		/// <param name="TouchScrollbars">It is need to update scroll bars values? Set this to False to prevent endless loops.</param>
		public void ScrollTo(double? y = null, double? x = null, bool TouchScrollbars = true)
		{
			if (y != null){
				OffsetY = (double)y;
				if(TouchScrollbars)
				VScroll.Value = (double)-y;
			}

			if (x != null){
				OffsetX = (double)x;
				if(TouchScrollbars)
				HScroll.Value = (double)-x;
			}

			Scroll();
		}

		public new Widget Content
		{
			get { return Child; }
			set{
				Child = value;
				Locator.Clear();
				Locator.AddChild(Child);

				VScroll.LowerValue = HScroll.LowerValue = 0;
				VScroll.StepIncrement = 1;
				VScroll.UpperValue = Child.Surface.GetPreferredSize().Height;
				HScroll.UpperValue = Child.Surface.GetPreferredSize().Width;
				ScrollTo(0);
			}
		}

		/// <summary>Allows/denies scrolling the content on the horizontal axis</summary>
		public bool CanScrollByX
		{ get; set; }

		/// <summary>Allows/denies scrolling the content on the vertical axis</summary>
		public bool CanScrollByY
		{ get; set; }

		public double PosX
		{
			get { return OffsetX; }
		}

		public double PosY
		{
			get { return OffsetY; }
		}
	}
}
