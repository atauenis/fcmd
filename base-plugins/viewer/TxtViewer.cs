/* The File Commander - просмоторщик файлов (FCView)
 * Модуль вывода текстовых файлов (аналог F3 в Norton/Volkov/Midnight/Total Commander)
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace fcmd.base_plugins.viewer
{
    class TxtViewer : pluginner.IViewerPlugin
    {
        #region metadata
        public string Name { get { return "Модуль вывода текстовых файлов (аналог ncview до 4 версии)"; } }
        public string Version { get { return "1.0.0"; } }
        public string Author { get{ return "A.T."; } }
        #endregion

        string Content = "";
        public System.Windows.Forms.Control DisplayBox(){
            TextBox txtBox = new TextBox();
            txtBox.Name = "txtBox";
            txtBox.Dock = DockStyle.Fill;
            txtBox.Multiline = true;
            txtBox.Text = Content;
            txtBox.ReadOnly = true;

            return txtBox;
        }

        public void LoadFile(string url)
        {
            pluginfinder pf = new pluginfinder();
            pluginner.IFSPlugin fsp = pf.GetFSplugin(url);
            Content = fsp.ReadFile(url);
        }
    }
}
