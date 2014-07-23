/* The File Commander - plugin API
 * File Commander Stylist.
 * CSS declaration parser and Xwt theme enabler
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using System;
using mucss;
using pluginner.Widgets;
using Xwt;
using Xwt.Drawing;

namespace pluginner.Toolkit
{
	/// <summary>Simplifies CSS-based theming of some Xwt widgets</summary>
	public class Stylist
	{
		/// <summary>The μCSS parser that is the kernel of the Stylist</summary>
		public Stylesheet CSS;

		bool semaphore;

		/// <summary>Initialize Stylist</summary>
		/// <param name="CSS_File">The path to the CSS file (or null if only internal styles are need to be used)</param>
		public Stylist(string CSS_File = null)
		{
			string DefaultStyle = Utilities.GetEmbeddedResource("Resources.Default.css");

			if (DefaultStyle == null) Environment.FailFast("File Commander has been crashed: the default theme's stylesheet is unable to load. Possibly there is a failure of the pluginner.dll body or RAM banks. Try to reinstall FC.", new InvalidProgramException("Default style isn't loading"));
			if(CSS_File != null && CSS_File != "")
				{ CSS = new Stylesheet(System.IO.File.ReadAllText(CSS_File) + DefaultStyle); }
			else
				CSS = new Stylesheet(DefaultStyle);
		}

		/// <summary>Enable theming of the widget</summary>
		/// <param name="Widget">The widget that needs to be themized</param>
		/// <param name="Selector">The selector pattern</param>
		public void Stylize(Widget Widget, string Selector = "Widget"){
			if (!semaphore) {
				semaphore = true;
				Stylize(Widget, Selector); //apply default style for all widgets
				try {
					Stylize(Widget, Widget.GetType().ToString().Substring(Widget.GetType().ToString().IndexOf('.') + 1)); //apply default style for the widget type
				}
				catch { Console.WriteLine("NOTICE: No style is set for widgets of type " + Widget.GetType().ToString().Substring(Widget.GetType().ToString().IndexOf('.') + 1)); }
				semaphore = false;
			}

			ApplyStyle(Widget, Selector);

			Widget.MouseEntered+=(o,ea)=> ApplyStyle(Widget, Selector + ":hover");

			Widget.MouseExited+=(o,ea)=>
			{
				ApplyStyle(Widget, Selector);
				if (Widget.HasFocus)
					ApplyStyle(Widget, Selector + ":focus");
			};

			Widget.ButtonPressed+=(o,ea)=> ApplyStyle(Widget, Selector + ":active");

			Widget.GotFocus+=(o,ea)=> ApplyStyle(Widget, Selector + ":focus");

			Widget.LostFocus += (o, ea) => ApplyStyle(Widget, Selector);

			Widget.ButtonReleased+=(o,ea)=>
			{
				ApplyStyle(Widget, Selector);
				if (Widget.HasFocus)
					ApplyStyle(Widget, Selector + ":focus");
			};
		}

		/// <summary>Enable theming of the ListView2</summary>
		/// <param name="Widget">The ListView2 that needs to be themized</param>
		/// <param name="Selector">The selector pattern</param>
		public void Stylize(ListView2 Widget, string Selector = "FileList")
		{
			Stylize(Widget as Widget,Selector);
			Selector all = CSS[Selector];
			Selector row1 = CSS[Selector+"RowA"];
			Selector row2 = CSS[Selector+"RowB"];
			Selector sel = CSS[Selector+"Row:checked"];
			Selector point = CSS[Selector+"Row:active"];
			if (all.Declarations["font-family"].Value != "inherit")
				Widget.Font = Font.FromName(
					all.Declarations["font-family"].Value
				);
			if (all.Declarations["background-color"].Value != "inherit")
				Widget.BackgroundColor = Utilities.GetXwtColor(all.Declarations["background-color"].Value);
			if (row1.Declarations["background-color"].Value != "inherit")
				Widget.NormalBgColor1 = Utilities.GetXwtColor(row1.Declarations["background-color"].Value);
			if (row2.Declarations["background-color"].Value != "inherit")
				Widget.NormalBgColor2 = Utilities.GetXwtColor(row2.Declarations["background-color"].Value);
			if (row1.Declarations["color"].Value != "inherit")
				Widget.NormalFgColor1 = Utilities.GetXwtColor(row1.Declarations["color"].Value);
			if (row2.Declarations["color"].Value != "inherit") 
				Widget.NormalFgColor2 = Utilities.GetXwtColor(row2.Declarations["color"].Value);
			if (point.Declarations["background-color"].Value != "inherit")
				Widget.PointedBgColor = Utilities.GetXwtColor(point.Declarations["background-color"].Value);
			if (point.Declarations["color"].Value != "inherit") 
				Widget.PointedFgColor = Utilities.GetXwtColor(point.Declarations["color"].Value);
			if (sel.Declarations["background-color"].Value != "inherit") 
				Widget.SelectedBgColor = Utilities.GetXwtColor(sel.Declarations["background-color"].Value);
			if (sel.Declarations["color"].Value != "inherit")
				Widget.SelectedFgColor = Utilities.GetXwtColor(sel.Declarations["color"].Value);

			Widget.BorderVisible = GetBorder(all.Declarations["border-style"].Value);
		}


		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Pattern">The specified selector with the desired style</param>
		public void ApplyStyle(Widget Widget, string Pattern)
		{
			if (Widget.GetType() == typeof(Label)) { ApplyStyle((Label)Widget, Pattern); return; }
			if (Widget.GetType() == typeof(Box)) { ApplyStyle((Box)Widget, Pattern); return; }

			Selector Selector = CSS[Pattern];

			if (Selector.Declarations["background-color"].Value != "inherit")
			Widget.BackgroundColor =
			Utilities.GetXwtColor(
				Selector.Declarations["background-color"].Value
			);

			if (Selector.Declarations["font-family"].Value != "inherit")
			Widget.Font = Font.FromName(
				Selector.Declarations["font-family"].Value
			);

			Widget.Visible = Selector.Declarations["display"].Value != "none";

		}

		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Pattern">The specified selector with the desired style</param>
		public void ApplyStyle(Label Widget, string Pattern)
		{
			Selector Selector = CSS[Pattern];

			if (Selector.Declarations["background-color"].Value != "inherit")
			Widget.BackgroundColor =
			Utilities.GetXwtColor(
				Selector.Declarations["background-color"].Value
			);

			if (Selector.Declarations["color"].Value != "inherit")
			Widget.TextColor =
			Utilities.GetXwtColor(
				Selector.Declarations["color"].Value
			);

			if (Selector.Declarations["font-family"].Value != "inherit")
			Widget.Font = Font.FromName(
				Selector.Declarations["font-family"].Value
			);

			Widget.Visible = Selector.Declarations["display"].Value != "none";
		}

		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Pattern">The specified selector with the desired style</param>
		public void ApplyStyle(Box Widget, string Pattern)
		{
			Selector Selector = CSS[Pattern];

			if (Selector.Declarations["background-color"].Value != "inherit")
			Widget.BackgroundColor =
			Utilities.GetXwtColor(
				Selector.Declarations["background-color"].Value
			);

			foreach (Widget Child in Widget.Children)
			{
				ApplyStyle(Child,Pattern);
			}


			Widget.Visible = Selector.Declarations["display"].Value != "none";
		}

		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Pattern">The specified selector with the desired style</param>
		public void ApplyStyle(Button Widget, string Pattern)
		{
			Selector Selector = CSS[Pattern];

			if (Selector.Declarations["background-color"].Value != "inherit")
			Widget.BackgroundColor =
			Utilities.GetXwtColor(
				Selector.Declarations["background-color"].Value
			);

			if (Selector.Declarations["border-style"].Value != "inherit")
			if (GetBorder(Selector.Declarations["border-style"].Value))
				Widget.Style = ButtonStyle.Normal;
			else
				Widget.Style = ButtonStyle.Borderless;


			Widget.Visible = Selector.Declarations["display"].Value != "none";
		}

		/// <summary>Apply the specified selector (style) to the specified widget</summary>
		/// <param name="Widget">The widget that should "got" the style</param>
		/// <param name="Selector">The specified selector with the desired style</param>
		public void ApplyStyle(Label Widget, Selector Selector)
		{
			if (Selector.Declarations["background-color"].Value != "inherit")
				Widget.BackgroundColor =
				Utilities.GetXwtColor(
					Selector.Declarations["background-color"].Value
				);

			if (Selector.Declarations["color"].Value != "inherit")
				Widget.TextColor =
				Utilities.GetXwtColor(
					Selector.Declarations["color"].Value
				);

			if (Selector.Declarations["font-family"].Value != "inherit")
				Widget.Font = Font.FromName(
					Selector.Declarations["font-family"].Value
				);

			Widget.Visible = Selector.Declarations["display"].Value != "none";
		}


		private bool GetBorder(string borderStyle)
		{
			if (borderStyle != "none")  return true; else return false;
			//it's need to understand all css borderstyles
		}
	}
}
