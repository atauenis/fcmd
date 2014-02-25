/* The File Commander - plugin API - ListView2
 * The enhanced ListView widget
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xwt;

namespace pluginner
{
    public class ListView2 : Widget
    {
        private VBox Layout = new VBox();
        private HBox CollumnRow = new HBox();
        private ScrollView ScrollerIn = new ScrollView(); //vertical scroller
        private ScrollView ScrollerOut = new ScrollView(); //horizontal scroller
        private List<Label> CollumnTitles = new List<Label>();
        private Table Grid = new Table();
        private int LastRow = 0;
        private int LastCol = 0;
        private Views _View = Views.Details;
        //todo: int MaxRow (для переноса при режиме Small Icons)
        private List<CollumnInfo> _Collumns = new List<CollumnInfo>();
        private int Through10Counter = 0; //для устранения зависания UI при загрузке длинных списков
        
        /// <summary>List of items. Please do not edit directly! Please use the AddItem and RemoveItem functions.</summary>
        public List<ListView2Item> Items = new List<ListView2Item>();
        public ListView2Item PointedItem;
        public List<ListView2Item> SelectedItems = new List<ListView2Item>();

        public ListView2()
	    {
            base.Content = ScrollerOut;
            Layout.Spacing = 0;
            Grid.DefaultRowSpacing = 0;

            ScrollerOut.Content = Layout;
            ScrollerOut.VerticalScrollPolicy = ScrollPolicy.Never;
            ScrollerIn.Content = Grid;
            ScrollerIn.HorizontalScrollPolicy = ScrollPolicy.Never;
            Layout.PackStart(CollumnRow);
            Layout.PackStart(ScrollerIn,true,true);

            Layout.KeyPressed += Layout_KeyPressed;
            Layout.CanGetFocus = true;
            base.CanGetFocus = true;
            this.KeyPressed += Layout_KeyPressed;

            this.ScrollerIn.BackgroundColor = Xwt.Drawing.Colors.White;
	    }


        //EVENT HANDLERS

        private void Item_ButtonPressed(object sender, ButtonEventArgs e)
        {
            this.SetFocus();
            ListView2Item lvi = Items[(sender as ListView2Item).RowNo];//через жопу? уточнить лучший способ, sender не работает
            //currently, the mouse click policy is same as in Total and Norton Commander
            if (e.Button == PointerButton.Right)//right click - select & do nothing
            {
                _SelectItem(lvi);
                return;
            }
            if (e.Button == PointerButton.Left)//left click - point & don't touch selection
            {
                _SetPoint(lvi);
            }
        }

        void Layout_KeyPressed(object sender, KeyEventArgs e)
        {
            //currently, the keyboard feel is same as in Norton & Total Commanders
            switch (e.Key)
            {
                case Key.Up: //[↑] - move cursor up
                    if(PointedItem.RowNo > 0)
                        _SetPoint(Items[PointedItem.RowNo - 1]);
                    e.Handled = true;
                    return;
                case Key.Down: //[↓] - move cursor bottom
                    if(PointedItem.RowNo < LastRow - 1)
                        _SetPoint(Items[PointedItem.RowNo + 1]);
                    e.Handled = true;
                    return;
                case Key.Insert: //[Ins] - set selection & move pointer bottom
                    _SelectItem(PointedItem);
                    if(PointedItem.RowNo < LastRow - 1)
                        _SetPoint(Items[PointedItem.RowNo + 1]);
                    e.Handled = true;
                    return;
                case Key.Return: //[↵] - same as double click
                    PointedItem.OnDblClick();
                    e.Handled = true;
                    return;
                case Key.NumPadMultiply: //gray [*] - invert selection
                    foreach (ListView2Item lvi in Items)
                    {
                        if ((int)lvi.State >= 2){
                            _UnselectItem(lvi);
                        }
                        else{
                            _SelectItem(lvi);
                        }
                    }
                    e.Handled = true;
                    return;
            }
        }


        //SUB-PROGRAMS

        /// <summary>Inverts selection of an item</summary>
        /// <param name="lvi">The requested ListView2Item</param>
        private void _SelectItem(ListView2Item lvi)
        {
            SelectedItems.Add(lvi);
            switch (lvi.State)
            {
                case ItemStates.Default:
                    lvi.State = ItemStates.Selected;
                    break;
                case ItemStates.Pointed:
                    lvi.State = ItemStates.PointedAndSelected;
                    break;

                case ItemStates.Selected:
                    lvi.State = ItemStates.Default;
                    break;
                case ItemStates.PointedAndSelected:
                    lvi.State = ItemStates.Pointed;
                    PointedItem = lvi;
                    break;
            }
        }

        /// <summary>Removes selection of an item</summary>
        /// <param name="lvi">The requested ListView2Item</param>
        private void _UnselectItem(ListView2Item lvi)
        {
            SelectedItems.Remove(lvi);
            if (lvi.State == ItemStates.PointedAndSelected)
                lvi.State = ItemStates.Pointed;
            else
                lvi.State = ItemStates.Default;
        }

        /// <summary>Sets the pointer to an item</summary>
        /// <param name="lvi">The requested ListView2Item</param>
        private void _SetPoint(ListView2Item lvi)
        {
            //unpoint current
            if (PointedItem != null)
            {
                if ((int)PointedItem.State > 1)
                    PointedItem.State = ItemStates.Selected;
                else
                    PointedItem.State = ItemStates.Default;
            }

            //point new
            if ((int)lvi.State > 1)
                lvi.State = ItemStates.PointedAndSelected;
            else
                lvi.State = ItemStates.Pointed;
            PointedItem = lvi;
        }


        //PUBLIC MEMBERS

        public void AddItem(List<Object> Data, string Tag = null)
        {
            ListView2Item lvi = new ListView2Item(
                LastRow,
                LastCol,
                Tag,
                _Collumns.ToArray(),
                Data);
            lvi.Font = Xwt.Drawing.Font.SystemSansSerifFont.WithWeight(Xwt.Drawing.FontWeight.Heavy);
            lvi.State = ItemStates.Default;
            AddItem(lvi);
        }

        public void AddItem(ListView2Item Item)
        {
            Items.Add(Item);
            Grid.Add(Item, LastCol, LastRow,1,1,true);
            Item.ButtonPressed += new EventHandler<ButtonEventArgs>(Item_ButtonPressed);
            Item.CanGetFocus = true;
            if (LastRow == 0) _SetPoint(Item);
            LastRow++;

            Through10Counter++;
            if (Through10Counter == 250)
            {
                Xwt.Application.MainLoop.DispatchPendingEvents();
                Through10Counter = 0;
            }
        }

        public void RemoveItem(ListView2Item Item)
        {
            //Note that the removing item is simply hided.
            //To remove it completely, call Clear() sub-programm. But all other rows will be also removed.
            Item.Visible = false;
        }

        public ListView2Item GetItem(int Row)
        {
            return Items[Row];
        }

        public void Clear()
        {
            Grid.Clear();
            Items.Clear();
            LastRow = LastCol = 0;
            PointedItem = null;
        }

        /// <summary>Clear selection of row</summary>
        /// <param name="Item">The row or null if need to unselect all</param>
        public void Unselect(ListView2Item Item = null)
        {
            if (Item == null)
            {
                foreach (ListView2Item lvi in SelectedItems)
                {
                    lvi.State = ItemStates.Default;
                }
                SelectedItems.Clear();
            }
            else
            {
                Item.State = ItemStates.Default;
                SelectedItems.Remove(Item);
            }
        }

        public CollumnInfo[] Collumns
        {
            get { return _Collumns.ToArray(); }
            set {
                _Collumns.Clear();
                CollumnTitles.Clear();
                foreach (CollumnInfo ci in value)
                {
                    _Collumns.Add(ci);
                    CollumnTitles.Add(new Xwt.Label(ci.Title) { WidthRequest = ci.Width, Visible = ci.Visible});
                    CollumnRow.PackStart(CollumnTitles[CollumnTitles.Count-1]);
                }
            }
        }

        /// <summary>Defines visiblity of the widget's border</summary>
        public bool BorderVisible
        {
            get { return ScrollerOut.BorderVisible; }
            set { ScrollerOut.BorderVisible = value; }
        }

        public int SelectedRow
        {
            get { return PointedItem.RowNo; }
            set { _SetPoint(Items[value]); }
        }


        //ENUMS & STRUCTS

        /// <summary>
        /// Defines how the items are displayed in the control.
        /// </summary>
        public enum Views
        {
            SmallIcons, LargeIcons, Details
        }


        /// <summary>
        /// Enumeration of items' selection statuses
        /// </summary>
        public enum ItemStates
        {
            /// <summary>Default item state (not selected nor pointed)</summary>
            Default = 0,
            /// <summary>The item is pointed, but not selected</summary>
            Pointed = 1,
            /// <summary>The item is selected</summary>
            Selected = 2,
            /// <summary>The item is pointed and selected</summary>
            PointedAndSelected = 3
        }

        /// <summary>
        /// Structure, that contains information about collumns
        /// </summary>
        public struct CollumnInfo
        {
            public string Title;
            public object Tag;
            public double Width;
            public bool Visible;
        }
    }
}
