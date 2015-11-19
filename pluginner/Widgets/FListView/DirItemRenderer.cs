/* The File Commander - plugin API - FListView
 * Filesystem DirItem renderer for FListView widgets
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2015, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using Xwt;
using Xwt.Drawing;

namespace pluginner.Widgets.FListView
{
	/// <summary>
	/// DirItem renderer for FListView. Can be used to render directory entries, received from an Filesystem Plugin.
	/// </summary>
	public class DirItemRenderer : IItemRenderer
	{
		public Widget RenderWidget(TileTag tag)
		{
			if (tag.Raw is DirItem){
				HBox w = new HBox();
				DirItem di = (DirItem) Convert.ChangeType(tag.Raw, typeof(DirItem));

				string name = di.TextToShow;
				string date = di.Date.ToShortDateString();
				string size = di.Size + " байтъ";

				if (DetailsMode && Columns != null) //details mode is enabled and columns are visible
					foreach (FLVColumn col in Columns)
					{
						switch (col.Id)
						{
							case "FName":
								w.PackStart(col.Size > -1 ? new Label(name) {WidthRequest = col.Size} : new Label(name));
								break;
							case "FDate":
								w.PackStart(col.Size > -1 ? new Label(date) {WidthRequest = col.Size} : new Label(date));
								break;
							case "FSize":
								w.PackStart(col.Size > -1 ? new Label(size) {WidthRequest = col.Size} : new Label(size));
								break;
						}
					}
				else w.PackStart(new Label(name)); //other modes - list, big icons, thumbanials...
				w.Tag = tag;

				tag.SelectionChanged -= w_SelectionChanged;
				tag.SelectionChanged += w_SelectionChanged;
				PaintSelection(w,tag.SelectionStatus);

				return w;
				//todo: rewrite from HBox.PackStart to manual drawing based on Canvas (to allow drawing of new labels over non fitted ends of old labels)
			}
			else throw new InvalidOperationException("DirItemRenderer can be used only with tiles with raw data of type \"pluginner.DirItem\".");
		}


		public IconSize IconSize { get; set; }
		public bool DetailsMode { get; set; }
		public ScrollMode ListLayout { get; set; }
		public FLVColumn[] Columns { get; set; }

		public event EventHandler<SelectionChangedEventArgs> TileSelectionChanged;

		void w_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			Widget w = (Widget) sender;
			PaintSelection(w, e.NewStatus);
			if(TileSelectionChanged!=null) TileSelectionChanged(w, e);
		}

		private void PaintSelection(Widget w, SelectionStatus ss)
		{
			switch (ss)
			{
				case SelectionStatus.None:
					w.BackgroundColor = Colors.White;
					break;
				case SelectionStatus.Selected:
					w.BackgroundColor = Colors.MediumVioletRed;
					break;
				case SelectionStatus.PointedAndSelected:
					w.BackgroundColor = Colors.Violet;
					break;
				case SelectionStatus.Pointed:
					w.BackgroundColor = Colors.Aqua;
					break;
			}
		}

	}
}
