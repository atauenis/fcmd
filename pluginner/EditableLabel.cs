/* The File Commander
 * "Editable label" widget
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
	/// <summary>
	/// Editable label (like the file name label in the wide-used file managers)
	/// </summary>
	public class EditableLabel : Widget
	{
		//todo: to be used in FileListPanel, XML editor, bookmark editor, skin editor

		Label l = new Label();
		TextEntry t = new TextEntry();
		bool IsEditable = true;

		public EditableLabel(string text, bool editable = true)
		{
			Text = text;
			Editable = editable;
			this.Content = l;
			l.ButtonPressed += l_ButtonPressed;
			t.KeyPressed += t_KeyPressed;
		}
		public EditableLabel ()
		{
			this.Content = l;
			l.ButtonPressed += l_ButtonPressed;
			t.KeyPressed += t_KeyPressed;
		}

		void t_KeyPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape)
				SetEditInput(null);
			l.Text = t.Text;
			if(e.Key == Key.Return)
				SetEditInput(false);
		}

		void l_ButtonPressed(object sender, ButtonEventArgs e)
		{
			//todo add between-clicks timeout detection, like https://github.com/atauenis/fcmd/pull/16
			if (e.MultiplePress > 1 && IsEditable)
				SetEditInput(true);
		}
		
		/// <summary>
		/// The text's font
		/// </summary>
		public new Xwt.Drawing.Font Font
		{
			get { return l.Font; }
			set { l.Font = value; t.Font = value; }
		}

		/// <summary>
		/// The text's color (Due to the XWT limitations, applied only at view mode)
		/// </summary>
		public Xwt.Drawing.Color TextColor
		{
			get { return l.TextColor; }
			set { l.TextColor = value; }
		}

		public new Xwt.Drawing.Color BackgroundColor
		{
			get { return base.BackgroundColor; }
			set
			{
				base.BackgroundColor = value;
				l.BackgroundColor = value;
				t.BackgroundColor = value;
			}
		}

		/// <summary>
		/// The text-like content of the widget
		/// </summary>
		public string Text
		{
			get { return l.Text; }
			set { l.Text = value; t.Text = value; }
		}

		/// <summary>
		/// Gets or sets edit-lock for this widget. Note that this does not controls the SetEditInput() function.
		/// </summary>
		public bool Editable
		{
			get { return IsEditable; }
			set { IsEditable = value; t.Sensitive = value; this.Content = l; }
		}

		/// <summary>
		/// Manually set this EditableLabel to view or edit mode
		/// </summary>
		/// <param name="SetToEdit"><value>true</value> if edit is required, <value>false</value> if view mode is required (if there are user edits, they will be saved) or <value>null</value> if view mode is required, but the last changes from the user should not be saved</param>
		public void SetEditInput(bool? SetToEdit)
		{
			if (SetToEdit == null){
				t.Text = l.Text;
				this.Content = l;
				return;
			}

			switch (SetToEdit)
			{
				case true:
					this.Content = t;
					return;
				case false:
					this.Content = l;
					return;
			}
		}
	}
}
