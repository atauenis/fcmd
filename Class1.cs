using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using fcmd.base_plugins.fs;
using pluginner;
using pluginner.Toolkit;
using pluginner.Widgets.FListView;
using Xwt;
using Xwt.Drawing;
using ThreadState = System.Threading.ThreadState;

namespace fcmd
{
	class Class1 : Window
	{
		//FListView<DirItem> flv = new FListView<DirItem>(new DirItemRenderer());
		public Class1()
		{
			Title = "F L I S T V I E W   DEMO";
			//Title = "F L I S T V I E W   DEMO  :  LOADING FS...";
			Height = 480;
			Width = 640;
			Padding = 0;
			//Content = flv;
			localFileSystem lfs = new localFileSystem();
			Show();
			/*if(OSVersionEx.Platform == PlatformID.Win32NT)
			lfs.CurrentDirectory = @"D:\сашины\";
			else
			lfs.CurrentDirectory = @"/etc/";
			List<DirItem> dil = new List<DirItem>();
			FileSystemOperationStatus fsos = new FileSystemOperationStatus();
			Thread thr = new Thread(() => lfs.GetDirectoryContent(ref dil, fsos));
			thr.Start();
			flv.Values = dil;
			flv.DetailsMode = true;
			FLVColumn[] cols = new FLVColumn[3]{
				new FLVColumn(){Id="FName",Title="Имя"},
				new FLVColumn(){Id="FDate",Title="Дата"},
				new FLVColumn(){Id="FSize",Title="Размер"}
			};
			flv.Columns = cols;
			flv.TileSelectionChanged += (o, ea) => { Console.WriteLine("{0} Статус: {1}\n{0} Выбрано элементов: {2}\n{0} Под курсором то же, о чём сообщили: {3}", DateTime.Now, ea.NewStatus, flv.SelectedTiles.Count, flv.PointedTile == o); };

			int counter = 0;
			do
			{
				counter ++;
				if (counter == 100)
				{
					Title = "F L I S T V I E W   DEMO  : LOADING " + lfs.CurrentDirectory + " (" + dil.Count + " items loaded) ";
					counter = 0;
					var cap = dil.Count;
					flv.Capacity = Convert.ToUInt64(cap);
				}
				Application.MainLoop.DispatchPendingEvents();
			} while (thr.ThreadState == ThreadState.Running);

			Title = "F L I S T V I E W   DEMO  :  RENDERING THE WIDGET...";
			flv.Capacity = Convert.ToUInt64(dil.LongCount());
			Title = "F L I S T V I E W   DEMO  :  " + lfs.CurrentDirectory + " (" + flv.Capacity + " items)";*/

			//FileListPanel2 flp = new FileListPanel2();
			MWPanel flp = new MWPanel();
			Content = flp;
			if(OSVersionEx.Platform == PlatformID.Win32NT)
				flp.LoadDir(@"C:\windows\system32");
			else
				flp.LoadDir(@"/etc/");


			/*Height = 480;
			Width = 640;*/
			Closed += (o, ea) => Application.Exit();
		}
	}
}
