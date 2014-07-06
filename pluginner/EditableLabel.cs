using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace pluginner
{
	public class EditableLabel : Widget
	{
		//todo: to be used in FileListPanel, XML editor, bookmark editor, skin editor

		Label l = new Label();
		TextEntry t = new TextEntry();
		public EditableLabel ()
		{
			this.Content = l;
			l.ButtonPressed += l_ButtonPressed;
			t.KeyPressed += t_KeyPressed;
		}

		void t_KeyPressed(object sender, KeyEventArgs e)
		{
			l.Text = t.Text;
			if(e.Key == Key.Return)
			this.Content = l;
		}

		void l_ButtonPressed(object sender, ButtonEventArgs e)
		{
			if (e.MultiplePress > 1)
			{
				this.Content = t;
			}
		}

		public new Xwt.Drawing.Font Font
		{
			get { return l.Font; }
			set { l.Font = value; t.Font = value; }
		}

		public string Text
		{
			get { return l.Text; }
			set { l.Text = value; t.Text = value; }
		}
	}
}
