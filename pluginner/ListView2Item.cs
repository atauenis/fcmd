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

        private void Rebuild()
        {
            Layout.Clear();
            Layout.BackgroundColor = Xwt.Drawing.Colors.Bisque;
            for (int i = 0; i < _Values.Length; i++)
            {
                Object Item = _Values[i];

                Label lbl = new Label();
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

        /// <summary>Set the font of the row</summary>
        public Xwt.Drawing.Font Font
        {
            set
            {
                foreach (Xwt.Widget w in Layout.Children)
                {
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
                        Layout.BackgroundColor = Xwt.Drawing.Colors.AntiqueWhite;
                        break;
                    case ListView2.ItemStates.Selected:
                        Layout.BackgroundColor = Xwt.Drawing.Colors.DeepSkyBlue;
                        break;
                    case ListView2.ItemStates.PointedAndSelected:
                        Layout.BackgroundColor =
                            Xwt.Drawing.Colors.DeepSkyBlue.BlendWith(
                            Xwt.Drawing.Colors.WhiteSmoke, 0.5
                            );
                        break;
                    default:
                        Layout.BackgroundColor = Xwt.Drawing.Colors.White;
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
