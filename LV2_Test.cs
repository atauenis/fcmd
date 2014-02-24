using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fcmd
{
    class LV2_Test : Xwt.Window
    {
        public LV2_Test()
        {
            List<Object> Data = new List<Object>();
            this.Title = "LV2 test form";
            this.Show();
            Data.Add(null);
            Data.Add(null);
            Data[1] = "qwertyuiop";
            pluginner.ListView2 lv2 = new pluginner.ListView2();
            List<pluginner.ListView2.CollumnInfo> ci = new List<pluginner.ListView2.CollumnInfo>();
            ci.Add(new pluginner.ListView2.CollumnInfo { Title = "test1", Width = 50 });
            ci.Add(new pluginner.ListView2.CollumnInfo { Title = "test2", Width = 100  });
            lv2.Collumns = ci.ToArray();

            Data[0] = "raz"; lv2.AddItem(Data);
            Data[0] = "dva"; lv2.AddItem(Data);
            Data[0] = "tri"; lv2.AddItem(Data);
            for (int i = 0; i <= 10; i++)
            {
                Data[0] = i; lv2.AddItem(Data);
            }
            this.Content = lv2;

            lv2.Items[2].State = pluginner.ListView2.ItemStates.Selected;
        }
    }
}
