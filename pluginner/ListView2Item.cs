/* The File Commander - plugin API - ListView2
 * The ListView2 item widget
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
    public class ListView2Item : Xwt.Widget
    {
        /// <summary>Data store</summary>
        private Object[] _Values;
        /// <summary>Collumn info store</summary>
        private ListView2.CollumnInfo[] _Cols;
        /// <summary>Selection state</summary>
        private ListView2.ItemStates _State;
        /// <summary>Widget layout host</summary>
        private HBox Layout = new HBox();

        private Xwt.Drawing.Color DefBgColor;
        private Xwt.Drawing.Color DefFgColor;
        private Xwt.Drawing.Color PointBgColor;
        private Xwt.Drawing.Color PointFgColor;
        private Xwt.Drawing.Color SelBgColor;
        private Xwt.Drawing.Color SelFgColor;

        private void Rebuild()
        {
            Layout.Clear();
            //Layout.BackgroundColor = Xwt.Drawing.Colors.Bisque;
            for (int i = 0; i < _Values.Length; i++)
            {
                Object Item = _Values[i];

                Label lbl = new Label();
                if (Item.GetType() != typeof(DirItem))
                lbl.Text = Item.ToString(); //todo: добавить поддержку сложных типов, таких как рисунки и контролы
                //lbl.BackgroundColor = Xwt.Drawing.Colors.Chocolate;
                if (_Cols.Count() > i && i != _Cols.Count() - 1)
                {
                    lbl.WidthRequest = _Cols[i].Width;
                    lbl.Visible = _Cols[i].Visible;
                }
                Layout.PackStart(lbl);
            }
        }

        public void OnDblClick()
        {
            this.OnButtonPressed(new ButtonEventArgs() { MultiplePress = 2 });
        }

        /// <summary>Creates a new ListView2Item</summary>
        /// <param name="RowNumber">Number of owning row</param>
        /// <param name="ColNumber">Number of owning collumn</param>
        /// <param name="RowTag">The item's tag</param>
        /// <param name="Collumns">Array of collumn information</param>
        /// <param name="Data">The data that should be shown in this LV2I</param>
        public ListView2Item(int RowNumber, int ColNumber, string RowTag, ListView2.CollumnInfo[] Collumns, List<Object> Data)
        {
            //this.BackgroundColor = Xwt.Drawing.Colors.GreenYellow;
            this.Content = Layout;
            this.ExpandHorizontal = true;

            _Values = Data.ToArray();
            _Cols = Collumns;
            Tag = RowTag;
            RowNo = RowNumber;
            ColNo = ColNumber;
            Rebuild();
        }

        public Xwt.Drawing.Color NormalBgColor
        {
            get { return DefBgColor; }
            set {
                DefBgColor = value;
                if (State == ListView2.ItemStates.Default)
                    Layout.BackgroundColor = DefBgColor;
            }
        }

        public Xwt.Drawing.Color NormalFgColor
        {
            get { return DefFgColor; }
            set
            {
                DefFgColor = value;
                if (State == ListView2.ItemStates.Default) {
                    foreach (object w in Layout.Children){
                        if (w.GetType() == typeof(Xwt.Label)){
                            (w as Label).TextColor = value;
                        }
                    }
                }
            }
        }

        public Xwt.Drawing.Color PointerBgColor
        {
            get { return PointBgColor; }
            set
            {
                PointBgColor = value;
                if ((int)State == 1){
                    Layout.BackgroundColor = value;
                }
            }
        }

        public Xwt.Drawing.Color PointerFgColor
        {
            get { return PointFgColor; }
            set
            {
                PointFgColor = value;
                if ((int)State == 1){
                    foreach (object w in Layout.Children)
                    {
                        if (w.GetType() == typeof(Xwt.Label)){
                            (w as Label).TextColor = value;
                        }
                    }
                }
            }
        }

        public Xwt.Drawing.Color SelectionBgColor
        {
            get { return SelBgColor; }
            set
            {
                SelBgColor = value;
                if ((int)State >= 2){
                    Layout.BackgroundColor = value;
                }
            }
        }

        public Xwt.Drawing.Color SelectionFgColor
        {
            get { return SelFgColor; }
            set
            {
                SelFgColor = value;
                if ((int)State >= 2){
                    foreach (object w in Layout.Children)
                    {
                        if (w.GetType() == typeof(Xwt.Label)){
                            (w as Label).TextColor = value;
                        }
                    }
                }
            }
        }

        /// <summary>Set the font of the row</summary>
        public new Xwt.Drawing.Font Font
        {
            set
            {
                foreach (Xwt.Widget w in Layout.Children){
                    w.Font = value;
                }
            }
        }

        /// <summary>Set collumn list</summary>
        public ListView2.CollumnInfo[] Collumns
        {
            set { _Cols = value; Rebuild(); }
        }

        /// <summary>Status of the item selection</summary>
        public ListView2.ItemStates State
        {
            get { return _State; }
            set
            {
                _State = value;
                switch (value)
                {
                    case ListView2.ItemStates.Pointed:
                        Layout.BackgroundColor = PointerBgColor;
                        foreach (object w in Layout.Children)
                        {
                            if (w.GetType() == typeof(Xwt.Label)){
                                (w as Label).TextColor = PointerFgColor;
                            }
                        }
                        break;
                    case ListView2.ItemStates.Selected:
                        Layout.BackgroundColor = SelBgColor;
                        foreach (object w in Layout.Children)
                        {
                            if (w.GetType() == typeof(Xwt.Label)){
                                (w as Label).TextColor = SelFgColor;
                            }
                        }
                        break;
                    case ListView2.ItemStates.PointedAndSelected:
                        //todo: replace this buggy algorythm with better one
                        //дело в том, что xwt немного путает одинаковые цвета,
                        //на минимальные доли, но этого достаточно для color1!=color2
                        if (PointBgColor == NormalBgColor){
                            Layout.BackgroundColor = SelectionBgColor;
                        }
                        else{
                            Layout.BackgroundColor =
                                SelBgColor.BlendWith(
                                PointBgColor, 0.5
                                );
                        }
                        break;
                    default:
                        Layout.BackgroundColor = DefBgColor;
                        foreach (object w in Layout.Children)
                        {
                            if (w.GetType() == typeof(Xwt.Label))
                            {
                                (w as Label).TextColor = NormalFgColor;
                            }
                        }
                        break;
                }
            }
        }

        public int RowNo;
        public int ColNo;
        public string Tag; //don't forgetting that the lv2 is used only for file list, so the tag can be only a string

        /// <summary>
        /// Get or set the data. Note that the data should be written fully.
        /// </summary>
        public List<Object> Data
        {
            get { return _Values.ToList<Object>(); }
            set {
                if (_Cols == null) {
                    throw new Exception("Please set collumns first!");
                }
                _Values = value.ToArray();
                Rebuild();
            }
        }
    }
}
