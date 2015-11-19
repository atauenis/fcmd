/* The File Commander
 * The file list widget
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-15, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using fcmd.base_plugins.fs;
using pluginner;
using pluginner.Toolkit;
using pluginner.Widgets.FListView;
using Xwt;
using XwtColors = Xwt.Drawing.Colors;

namespace fcmd
{
	/// <summary>
	/// File Commander main window's panel
	/// </summary>
	class FileListPanel2 : Widget //todo: убрать цифру "два" после удаления старого FLP
	{
		Table Layout = new Table();
		internal TextEntry UrlBox = new TextEntry();
		internal FListView<DirItem> FLV = new FListView<DirItem>(new DirItemRenderer());
		internal Label StatusLabel = new Label("Статусная строка");
		public FileListPanel2()
		{
			Content = Layout;
			Layout.Margin = 0;
			Layout.DefaultColumnSpacing = Layout.DefaultRowSpacing = 0;

			Layout.Add(UrlBox,0,0);
			Layout.Add(new Button("*") { CanGetFocus = false}, 1, 0);
			Layout.Add(new Button("↓") { CanGetFocus = false}, 2, 0);
			Layout.Add(FLV,0,1,1,3,true,true);
			Layout.Add(StatusLabel,0,2);

			UrlBox.BackgroundColor = XwtColors.DeepSkyBlue;
			UrlBox.ShowFrame = false;
			UrlBox.Cursor = CursorType.Arrow;
			UrlBox.GotFocus += (o, e) => { UrlBox.BackgroundColor = XwtColors.White; UrlBox.Cursor = CursorType.IBeam;};
			UrlBox.LostFocus += (o, e) => { UrlBox.BackgroundColor = XwtColors.DeepSkyBlue; UrlBox.Cursor = CursorType.Arrow; };
			UrlBox.KeyPressed += (o, e) => { if(e.Key == Key.Return) {FLV.SetFocus(); LoadDir(UrlBox.Text);}};

			FLV.DetailsMode = true;
			FLVColumn[] cols = new FLVColumn[3]{
				new FLVColumn(){Id="FName",Title="Имя"},
				new FLVColumn(){Id="FDate",Title="Дата"},
				new FLVColumn(){Id="FSize",Title="Размер"}
			};
			FLV.Columns = cols;
		}

		public void LoadDir(string url)
		{
			if (Navigating != null) Navigating(url);
			UrlBox.Text = url;
			StatusLabel.Text = url;
			localFileSystem lfs = new localFileSystem();
			lfs.CurrentDirectory = url;
			List<DirItem> dil = new List<DirItem>();
			FileSystemOperationStatus fsos = new FileSystemOperationStatus();
			Thread thr = new Thread(() => lfs.GetDirectoryContent(ref dil, fsos));
			StatusLabel.Text = "Гружу каталог...";
			thr.Start();
			FLV.Values = dil;

			int counter = 0;
			do
			{
				counter++;
				if (counter == 10000)
				{
					counter = 0;
					var cap = dil.Count;
					FLV.Capacity = Convert.ToUInt64(cap);
					StatusLabel.Text = url + " - " + cap + " (и будут ещё)";
				}
				Application.MainLoop.DispatchPendingEvents();
			} while (thr.ThreadState == ThreadState.Running);
			FLV.Capacity = Convert.ToUInt64(dil.Count);
			StatusLabel.Text = url + " - " + FLV.Capacity;
			if (Navigated != null) Navigated(url);
		}

		/// <summary>
		/// FileListPanel begins a directory changing
		/// </summary>
		public event TypedEvent<string> Navigating;
		/// <summary>
		/// FileListPanel completed a directory changing
		/// </summary>
		public event TypedEvent<string> Navigated;
	}
}
