/* The File Commander
 * XML node displayer (with support for editing)
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;
using System.Xml;

namespace fcmd
{
	/// <summary>Graphics XML node representer</summary>
	public class XmlDisplay : Widget
	{
		//временный код! переписать с поддержкой редактирования и сворачивания!

		Xwt.VBox layout = new VBox();
		XmlNode node;

		public XmlDisplay(XmlNode node, System.Collections.Hashtable ht = null)
		{
			this.CanGetFocus = true;
			this.node = node;
			if (ht != null) ht.Add(node, this); //register self in the common registry of conformity between XmlNode <--> XmlDisplay

			Xwt.Expander exp = new Expander { Expanded = true, Label = node.Name, Font = Font.WithWeight(Xwt.Drawing.FontWeight.Semibold) };
			exp.Content = layout;
			layout.Font = Font.WithWeight(Xwt.Drawing.FontWeight.Normal); //для уверенности
			this.Content = exp;
						
			if (node.Attributes != null)
			foreach (XmlAttribute a in node.Attributes)
			{
				layout.PackStart(new Label(a.LocalName + " = " + a.Value) { Tag = a, Font = Font.WithWeight(Xwt.Drawing.FontWeight.Normal) } );
			}

			if (node.ChildNodes != null)
			if (node.ChildNodes.Count > 0)
				foreach (XmlNode n in node.ChildNodes)
				{
					XmlDisplay child_xd = new XmlDisplay(n, ht) { Tag = n, MarginLeft=24 };
					layout.PackStart(child_xd); //обеспечивается рекурсивность
				}
			else
				layout.PackStart(new Label(node.InnerText) { Tag = node, Font = Font.WithWeight(Xwt.Drawing.FontWeight.Normal) });
		}
						
		public XmlNode Node
		{
			get { return node; }
		}
	}
}
