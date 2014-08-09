/* The File Commander - Keyboard Help Button
 * Button that to be used to suggest keyboard shortcut in main window
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * (C) 2014, Evgeny Akhtimirov (wilbit@me.com)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pluginner.Toolkit;
using Xwt;
using mucss;

namespace fcmd
{
/// <summary>Keyboard help "button" (which are shown in the bottom of the main & VE window)</summary>
	class KeyboardHelpButton : Widget
	{
		/*/// <summary>Extended Label widget</summary>
		private class Label2 : Label
		{
			//To be used instead of Label (to fix Xwt design errors sometimes later due to a Xwt bug:
			//System.Reflection.AmbiguousMatchException was unhandled
			//	HResult=-2147475171
			//	Message=Ambiguous match found.
			//	Source=mscorlib
			//	StackTrace:
			//		at System.RuntimeType.GetMethodImpl(String name, BindingFlags bindingAttr, Binder binder, CallingConventions callConv, Type[] types, ParameterModifier[] modifiers)
			//		at System.Type.GetMethod(String name, BindingFlags bindingAttr)
			//		at Xwt.Backends.EventHost.IsOverriden(EventMap emap, Type thisType, Type t) in d:\сашины\filecommander\xwt\Xwt\Xwt.Backends\EventHost.cs:line 78

			public void OnMouseEntered(object o, MouseMovedEventArgs ea)
			{ base.OnMouseEntered(ea); }
			public void OnMouseExited(object o, MouseMovedEventArgs ea)
			{ base.OnMouseExited(ea); }
			public void OnButtonPressed(object o, ButtonEventArgs ea)
			{ base.OnButtonPressed(ea); }
			public void OnButtonReleased(object o, ButtonEventArgs ea)
			{ base.OnButtonReleased(ea); }
			public void OnGotFocus(object o, ButtonEventArgs ea)
			{ base.OnGotFocus(ea); }
			public void OnLostFocus(object o, ButtonEventArgs ea)
			{ base.OnLostFocus(ea); }
		}*/

		HBox hb = new HBox();
		Label lblF = new Label(); //F key
		Label lblD = new Label(); //Description
		Stylist s = new Stylist(fcmd.Properties.Settings.Default.UserTheme);

		string fkey = "", text = "";

		public KeyboardHelpButton(string Style = "KeyboardHelp")
		{
			this.Content = hb;
			hb.PackStart(lblF);
			hb.PackStart(lblD,true);

			lblF.Text = FKey;
			lblD.Text = Text;

			s.Stylize(this,Style);
			s.Stylize(lblF,Style+"F");
			s.Stylize(lblD,Style+"Descr");

			var eventHandler = new EventHandler<ButtonEventArgs> ((o, ea) => {
				var handler = this.Clicked;
				if (handler != null) {
					handler (this, ea);
				}
			});
			lblF.ButtonPressed += eventHandler;
			lblD.ButtonPressed += eventHandler;
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
				lblD.Text = text;
				lblD.Visible = (Text.Length) > 0 ? true : false;
			}
		}

		public event EventHandler Clicked;
	}
}
