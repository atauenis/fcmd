/* The File Commander - plugin API
 * File Commander Stylist.
 * CSS declaration parser and Xwt theme enabler
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using mucss;

namespace pluginner
{
	/// <summary>Simplifies CSS-based theming of some Xwt widgets</summary>
	public class Stylist
	{
		/// <summary>The μCSS parser that is the kernel of the Stylist</summary>
		public mucss.Parser Parser;

		bool semaphore = false;

		/// <summary>Initialize Stylist</summary>
		/// <param name="CSS_File">The path to the CSS file (or null if only internal styles are need to be used)</param>
		public Stylist(string CSS_File = null)
		{
			DefaultStyle = Utilities.GetEmbeddedResource("Resources.Default.css");

			if (DefaultStyle == null) Environment.FailFast("File Commander has been crashed: the default theme's stylesheet is unable to load. Possibly there is a failure of the pluginner.dll body or RAM banks. Try to reinstall FC.", new InvalidProgramException("Default style isn't loading"));
			if(CSS_File != null && CSS_File != "")
				{ Parser = new Parser(System.IO.File.ReadAllText(CSS_File) + DefaultStyle); }
			else
			 Parser = new Parser(DefaultStyle);
		}

		/// <summary>Enable theming of the widget</summary>
		/// <param name="Widget">The widget that needs to be themized</param>
		/// <param name="Selector">The selector</param>
		public void Stylize(Xwt.Widget Widget, string Selector = "Widget"){
			if (!semaphore) {
				semaphore = true;
				Stylize(Widget, "Widget"); //apply default style for all widgets
				try { 
				Stylize(Widget,Widget.GetType().ToString().Substring(4)); //apply default style for the widget type
				}
				catch { Console.WriteLine("NOTICE: No style is set for " + Widget.GetType().ToString().Substring(4) + "s"); }
				semaphore = false;
			}

			ApplyStyle(Widget, Selector);

			Widget.MouseEntered+=(o,ea)=>
			{ ApplyStyle(Widget, Selector + ":hover"); };

			Widget.MouseExited+=(o,ea)=>
			{
				ApplyStyle(Widget, Selector);
				if (Widget.HasFocus)
					ApplyStyle(Widget, Selector + ":focus");
			};

			Widget.ButtonPressed+=(o,ea)=>
			{ ApplyStyle(Widget, Selector + ":active"); };

			Widget.GotFocus+=(o,ea)=>
			{ ApplyStyle(Widget, Selector + ":focus"); };

			Widget.LostFocus += (o, ea) =>
			{ ApplyStyle(Widget, Selector); };

			Widget.ButtonReleased+=(o,ea)=>
			{
				ApplyStyle(Widget, Selector);
				if (Widget.HasFocus)
					ApplyStyle(Widget, Selector + ":focus");
			};
		}

		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Style">The specified selector with the desired style</param>
		public void ApplyStyle(Xwt.Widget Widget, string Pattern)
		{
			if(Widget.GetType() == typeof(Xwt.Label)) { ApplyStyle((Xwt.Label)Widget, Pattern); return; }

			Selector Selector = Parser.Get(Pattern);
			Widget.BackgroundColor =
			Utilities.GetXwtColor(
				Selector.Declarations["background-color"].Value
			);

			Widget.Font = Xwt.Drawing.Font.FromName(
				Selector.Declarations["font-family"].Value
			);

			Widget.Visible = Selector.Declarations["display"].Value == "none" ? false : true;

		}

		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Style">The specified selector with the desired style</param>
		public void ApplyStyle(Xwt.Label Widget, string Pattern)
		{
			Selector Selector = Parser.Get(Pattern);
			Widget.BackgroundColor =
			Utilities.GetXwtColor(
				Selector.Declarations["background-color"].Value
			);

			Widget.TextColor =
			Utilities.GetXwtColor(
				Selector.Declarations["color"].Value
			);

			Widget.Font = Xwt.Drawing.Font.FromName(
				Selector.Declarations["font-family"].Value
			);

			Widget.Visible = Selector.Declarations["display"].Value == "none" ? false : true;
		}

		private string DefaultStyle;
	}
}
