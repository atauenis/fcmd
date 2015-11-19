/* The File Commander - plugin API - FListView
 * FListView item renderer interface
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2015, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */

using System;
using Xwt;

namespace pluginner.Widgets.FListView
{
	/// <summary>
	/// Interface for FListView item renderers
	/// </summary>
	public interface IItemRenderer
	{
		Widget RenderWidget(TileTag tag);

		IconSize IconSize { get; set; }
		bool DetailsMode { get; set; }
		FLVColumn[] Columns { get; set; }
		ScrollMode ListLayout { get; set; }

		event EventHandler<SelectionChangedEventArgs> TileSelectionChanged;
	}
}
