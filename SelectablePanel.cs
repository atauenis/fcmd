/* The File Commander
 * Аналог Panel с возможностью установки фокуса
 * (C) 2010, Hans Passant, http://stackoverflow.com/questions/3562235
 * Доработка: 2013, Александр Тауенис (atauenis@yandex.ru)
 */
using System;
using System.Drawing;
using System.Windows.Forms;

class SelectablePanel : Panel {
	bool showBorder = false;

	public SelectablePanel() {
		this.SetStyle(ControlStyles.Selectable, true);
		this.TabStop = true;
	}
	protected override void OnMouseDown(MouseEventArgs e) {
		this.Focus();
		base.OnMouseDown(e);
	}
	protected override bool IsInputKey(Keys keyData) {
		if (keyData == Keys.Up || keyData == Keys.Down) return true;
		if (keyData == Keys.Left || keyData == Keys.Right) return true;
		return base.IsInputKey(keyData);
	}
	protected override void OnEnter(EventArgs e) {
		this.Invalidate();
		base.OnEnter(e);
	}
	protected override void OnLeave(EventArgs e) {
		this.Invalidate();
		base.OnLeave(e);
	}
	protected override void OnPaint(PaintEventArgs pe) {
		base.OnPaint(pe);
		if (this.Focused & showBorder) {
			var rc = this.ClientRectangle;
			rc.Inflate(-2, -2);
			ControlPaint.DrawFocusRectangle(pe.Graphics, rc);
		}
	}

	/// <summary>
	/// Gets or sets a value indicating whether this <see cref="SelectablePanel"/> have a visible border.
	/// </summary>
	/// <value>
	/// <c>true</c> if border visible; otherwise, <c>false</c>.
	/// </value>
	public bool ShowBorder {
		get {
			return showBorder;
		}
		set {
			showBorder = value;
		}
	}

}