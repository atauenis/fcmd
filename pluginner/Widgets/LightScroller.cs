/* The File Commander - plugin API
 * A lightweight scrollable panel
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using Xwt;

namespace pluginner.Widgets
{
	/// <summary>A lightweight scrollable panel</summary>
	public class LightScroller : Canvas
	{
		Widget _content;
		Rectangle _rectal;

		public LightScroller ()
		{
			MouseScrolled += (sender, e) => {
				switch(e.Direction){
				case ScrollDirection.Down:
					if(CanScrollByY) { ScrollBottom(); return;}
					if(CanScrollByX) { ScrollRight(); return; }
					return;
				case ScrollDirection.Up:
					if(CanScrollByY) { ScrollUp(); return;}
					if(CanScrollByX) { ScrollLeft(); return; }
					return;
				case ScrollDirection.Right:
					if(CanScrollByX) { ScrollRight(); return; }
					return;
				case ScrollDirection.Left:
					if(CanScrollByX) { ScrollLeft(); return; }
					return;
				}
			};
		}

		/// <summary>Gets or sets the content of the panel</summary>
		public new Widget Content{
			get{ return _content; }
			set{
				_content = value;
				AddChild(_content,10,10);
				_rectal = GetChildBounds(_content);
				_rectal.Top = 0;
				HeightRequest = _rectal.Height;
				SetChildBounds(_content,_rectal);
			}
		}

		/// <summary>Allows/denies scrolling the content on the horizontal axis</summary>
		public bool CanScrollByX = true;

		/// <summary>Allows/denies scrolling the content on the vertical axis</summary>
		public bool CanScrollByY = true;

		protected void ScrollUp(){
			if(_rectal.Top > Bounds.Top) return;
			_rectal.Top += 5;
			SetChildBounds(_content, _rectal);
		}

		protected void ScrollBottom(){//может глючить, разобраться!
			if(_rectal.Bottom > Bounds.Bottom) return;
			_rectal.Top -= 5;
			SetChildBounds(_content, _rectal);
		}

		protected void ScrollRight(){
			if(_rectal.Right < Bounds.Right) return;
			_rectal.Left -= 5;
			SetChildBounds(_content, _rectal);
		}

		protected void ScrollLeft(){
			if(_rectal.Left > Bounds.Left) return;
			_rectal.Left += 5;
			SetChildBounds(_content, _rectal);
		}

	}
}

