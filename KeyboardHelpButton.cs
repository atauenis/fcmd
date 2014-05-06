/* The File Commander - Keyboard Help Button
 * Button that to be used to suggest keyboard shortcut in main window
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace fcmd
{
	class KeyboardHelpButton : Widget
	{
		HBox hb = new HBox();
		Label lblF = new Label();
		Label lblT = new Label();

		string fkey = "", text = "";

		public KeyboardHelpButton()
		{
			this.Content = hb;
			hb.PackStart(lblF);
			hb.PackStart(lblT);

			lblF.Text = FKey;
			lblT.Text = Text;

			this.BackgroundColor = Xwt.Drawing.Colors.Black;

			lblF.BackgroundColor = Xwt.Drawing.Colors.Black;
			lblF.TextColor = Xwt.Drawing.Colors.LightGray;
			lblF.ButtonPressed += (o, ea) => { if (this.Clicked != null) this.Clicked(o, ea); };

			lblT.BackgroundColor = Xwt.Drawing.Colors.LightGray;
			lblT.TextColor = Xwt.Drawing.Colors.Blue;
			lblT.ButtonPressed += (o,ea)=>{ if(this.Clicked!=null) this.Clicked(o,ea);};

			//todo: add support for theming. Possibly, it's need to replace stupid XML-style themes with CSS-based ones (and embed a CSS parser)
		}

		/// <summary>The F-key that is associated with this KHB</summary>
		public string FKey {
			get { return fkey; }
			set{
				fkey = value;
				lblF.Text = fkey;
			}
		}

		/// <summary>The text to be showed on this KHB</summary>
		public string Text {
			get { return text; }
			set{
				text = value;
				lblT.Text = text;
			}
		}

		public event EventHandler Clicked;
	}
}
