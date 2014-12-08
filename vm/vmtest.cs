using System.Linq;
// ReSharper disable once RedundantUsingDirective
using System;
using System.Collections.Generic;
using Xwt;

namespace fcmd.vm
{
	/// <summary>
	/// VMList test window (playground)
	/// </summary>
	class vmtest : Window
	{
// ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
		private pluginner.Widgets.VirtualListView vml;
		private Label[] values = new Label[65535];

		public vmtest()
		{
			vml = new pluginner.Widgets.VirtualListView(GetWidgets) { ItemCount = values.Length };
			for (int i = 0; i < values.Length; i++)
			{
				values[i] = new Label("Label " + i);
				values[i].ExpandHorizontal = true;
				values[i].ExpandVertical = true;
			}

			Content = vml;
			Title = "Полигон для тестирования виртуального list view";
			Height = Width = 256;
		}

		public IEnumerable<Widget> GetWidgets(int start, int stop)
		{
			List<Widget> l = new List<Widget>();
			
			for (int i = start; i < stop; i++)
			{
				if (values.Length > i)
					l.Add(values[i]);
				#if DEBUG
					//else Console.WriteLine(@"Out of range; not a bug.");
				#endif
			}

			return l;
		}
	}
}
