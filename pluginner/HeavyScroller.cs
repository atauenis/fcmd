/* The File Commander - plugin API
 * A scrollable panel
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace pluginner
{
    /// <summary>A scrollable panel</summary>
    class HeavyScroller : Xwt.Widget
    {
        Table Layout = new Table();
        Canvas Locator = new Canvas() ;
        Widget Child = new Label("No child is inserted");
        HScrollbar HScroll = new HScrollbar();
        VScrollbar VScroll = new VScrollbar();
        double OffsetX = 0;
        double OffsetY = 0;

        public HeavyScroller()
        {
            Locator.AddChild(Child);
            Layout.Add(Locator, 0, 0, 1, 1, true, true);
            Layout.Add(VScroll, 1, 0);
            Layout.Add(HScroll, 0, 1);
            base.Content = Layout;

            BoundsChanged += new EventHandler(HeavyScroller_BoundsChanged);

            VScroll.ValueChanged += new EventHandler(VScroll_ValueChanged);
        }

        void HeavyScroller_BoundsChanged(object sender, EventArgs e)
        {
            VScroll.LowerValue = HScroll.LowerValue = 0;
            VScroll.StepIncrement = 1;
            VScroll.UpperValue = Child.Surface.GetPreferredSize().Height;
            HScroll.UpperValue = Child.Surface.GetPreferredSize().Width;

            Scroll();
        }

        void VScroll_ValueChanged(object sender, EventArgs e)
        {
            OffsetY = -VScroll.Value;
            Scroll();
        }

        void Scroll()
        {
            Locator.SetChildBounds(Child, new Rectangle(OffsetX,OffsetY,Child.Surface.GetPreferredSize().Width,Child.Surface.GetPreferredSize().Height));
        }

        public void ScrollToY(double y)
        {
            OffsetY = y;
            VScroll.Value = y;
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
                ScrollToY(0);
            }
        }

        /// <summary>Allows/denies scrolling the content on the horizontal axis</summary>
        public bool CanScrollByX
        {
            set { }
        }

        /// <summary>Allows/denies scrolling the content on the vertical axis</summary>
        public bool CanScrollByY
        {
            set { }
        }
    }
}
