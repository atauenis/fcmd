/* The File Commander
 * "Editable label" widget
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */

using Xwt;
using Xwt.Drawing;

namespace pluginner.Widgets
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
			Content = l;
			l.ButtonPressed += l_ButtonPressed;
			t.KeyPressed += t_KeyPressed;
		}

		public EditableLabel ()
		{
			Content = l;
			l.ButtonPressed += l_ButtonPressed;
			t.KeyPressed += t_KeyPressed;
		}

		//todo: doesn't fire every time when Enter (Return) is pressing, thus it's need to be fixed. Possibly, due to XWT peculiarity (input focus doesn't sets as fully as required).
		private void t_KeyPressed(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Escape) {
				SetEditInput(null);
			}
			l.Text = t.Text;
			if (e.Key == Key.Return) {
				SetEditInput(false);
			}
		}

		private void l_ButtonPressed(object sender, ButtonEventArgs e)
		{
			//todo add between-clicks timeout detection, like https://github.com/atauenis/fcmd/pull/16
			if (e.MultiplePress > 1 && IsEditable)
				SetEditInput(true);
		}
		
		/// <summary>
		/// The text's font
		/// </summary>
		public new Font Font
		{
			get { return l.Font; }
			set { l.Font = value; t.Font = value; }
		}

		/// <summary>
		/// The text's color (Due to the XWT limitations, applied only at view mode)
		/// </summary>
		public Color TextColor
		{
			get { return l.TextColor; }
			set { l.TextColor = value; }
		}

		/// <summary>
		/// The text's background color
		/// </summary>
		public new Color BackgroundColor
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
			set { IsEditable = value; t.Sensitive = value; Content = l; }
		}

		/// <summary>
		/// Manually set this EditableLabel to view or edit mode
		/// </summary>
		/// <param name="SetToEdit"><value>true</value> if edit is required, <value>false</value> if view mode is required (if there are user edits, they will be saved) or <value>null</value> if view mode is required, but the last changes from the user should not be saved</param>
		public void SetEditInput(bool? SetToEdit)
		{
			if (SetToEdit == null){
				t.Text = l.Text;
				Content = l;
				return;
			}

			switch (SetToEdit)
			{
				case true:
					Content = t;
					t.SetFocus();
					return;
				case false:
					Content = l;
					var editComplete = EditComplete;
					if (editComplete != null) {
						editComplete (this);
					}
					return;
			}
		}

		public event TypedEvent<EditableLabel> EditComplete;
	}
}
