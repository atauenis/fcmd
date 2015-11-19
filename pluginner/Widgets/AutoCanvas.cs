/* The File Commander - plugin API
 * A canvas that can align childs automatically
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2015, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using System.ComponentModel;
using System.Linq;
using Xwt;

namespace pluginner.Widgets
{
	/// <summary>
	/// A canvas that can align children automatically
	/// </summary>
	class AutoCanvas : Canvas
	{
		private double LastY;
		private double LastX;
		
		//todo: обеспечить добавку виджетов в другую плоскость и по координате в пикслеях, и тупо в первую пустую позицию в этой строке/столбце

		/// <summary>
		/// Add a widget below
		/// </summary>
		/// <param name="W">The widget</param>
		/// <param name="PosX">Horizontal position in pixels</param>
		public void AddChildY(Widget W, int PosX = 0)
		{
			AddChild(W,PosX,LastY);
			LastY += Children.ElementAt(Children.Count() - 1).Size.Height + VerticalSpacing;
		}

		/// <summary>
		/// Add a widget to the right edge
		/// </summary>
		/// <param name="W"></param>
		/// <param name="PosY"></param>
		public void AddChildX(Widget W, int PosY = 0)
		{
			AddChild(W,LastX,PosY);
			LastY += Children.ElementAt(Children.Count() - 1).Size.Width + HorizontalSpacing;
		}

		/// <summary>
		/// Clear the AutoCanvas
		/// </summary>
		public new void Clear()
		{
			base.Clear();
			LastX = LastY = 0;
		}

		/// <summary>
		/// Vertical widget spacing
		/// </summary>
		[DefaultValue(0)]
		public double VerticalSpacing { get; set; }

		/// <summary>
		/// Horizontal widget spacing
		/// </summary>
		[DefaultValue(0)]
		public double HorizontalSpacing { get; set; }
	}
}
