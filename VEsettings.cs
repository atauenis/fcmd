/* The File Commander - internal Viewer/Editor
 * VE settings window
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fcmd
{
    class VEsettings : Xwt.Dialog
    {
        Localizator Locale = new Localizator();
        public VEsettings()
        {
            this.Title = "Under construction";
            this.Content = new Xwt.Label("Не включать, работают люди!");
            //todo
        }
    }
}
