/* The File Commander
 * The main window's panel widget
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-15, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using pluginner.Widgets.FListView;
using Xwt;

namespace fcmd
{
	/// <summary>
	/// Main window panel
	/// </summary>
	class MWPanel : Widget
	{
		private Table Layout = new Table();
		private Notebook Tabview = new Notebook();
		private List<FileListPanel2> Panels = new List<FileListPanel2>();

		public MWPanel()
		{
			Content = Layout;
			Margin = 0;
			Layout.Add(new Label("Панель дисков и закладок"), 0, 0, 1, 3, true);
			Layout.Add(new Label("Панель информации о диске"), 0, 1, 1, 1, true);
			Layout.Add(new Button("/"){CanGetFocus = false}, 1, 1);
			Layout.Add(new Button(".."){CanGetFocus = false}, 2, 1);
			Layout.Add(Tabview, 0, 2, 1, 3, true, true);
			Layout.Add(new Label("Эмулятор командной строки"), 0, 3, 1, 3, true);

			Panels.Add(new FileListPanel2());
			FileListPanel2 Pan1 = Panels[0];
			Tabview.Add(Pan1,@"NOT READY");
			Pan1.Navigating += (ea) => { Tabview.Tabs[0].Label = ea  + "*";};
			Pan1.Navigated += (ea) => { Tabview.Tabs[0].Label = ea;};
		}
		
		public void LoadDir(string url)
		{
			Panels[0].LoadDir(url);
		}
	}
}
