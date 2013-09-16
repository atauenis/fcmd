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
		TextBox txtBox = new TextBox();
        public System.Windows.Forms.Control DisplayBox(){
            txtBox.Name = "txtBox";
            txtBox.Dock = DockStyle.Fill;
            txtBox.Multiline = true;
            txtBox.Text = Content;
            txtBox.ReadOnly = true;
			txtBox.SelectionStart = 0;  // подавление системного...
			txtBox.SelectionLength = 0; // ...автовыделения всего
			txtBox.ScrollBars = ScrollBars.Both;
			txtBox.BackColor = System.Drawing.Color.White;
			txtBox.BorderStyle = BorderStyle.None; //для отключения влияния тем WinXP+

            return txtBox;
        }

		public void LoadFile(string url, pluginner.IFSPlugin fsplugin){
            Content = Encoding.UTF8.GetString(fsplugin.GetFile (url, new int()).Content);
		}

		public bool CanCopy{get{return true;}}

		public void Copy(){//правка-копировать
			Clipboard.SetText(txtBox.SelectedText);
		}

		public bool CanSelectAll{get{return true;}}

		public void SelectAll(){//Выделить всё
			txtBox.SelectionStart = 0;
			txtBox.SelectionLength = txtBox.Text.Length;
		}

		public bool CanPrint{get{return false;}}

		public void Print(){//печать на принтер
			//todo
		}

		public void PrintSettings(){ //Параметры страницы
			//todo
		}

        public ToolStripMenuItem[] SettingsMenu{//(под)меню настроек
            get{
                List<ToolStripMenuItem> Options = new List<ToolStripMenuItem>();
                Options.Add(new ToolStripMenuItem("У плагина нет настроек",null,this.Test));
                return Options.ToArray();
            }
        }

        public void Test(object sender, EventArgs e){
            MessageBox.Show("У плагина нет настроек...пока что.");
        }
    }
}
