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
using System.Drawing.Printing;

namespace fcmd.base_plugins.viewer
{
    class TxtViewer : pluginner.IViewerPlugin
    {
        #region metadata
        public string Name { get { return new Localizator().GetString("TxtViewerVer"); } }
        public string Version { get { return "1.0.0"; } }
        public string Author { get{ return "A.T."; } }
        #endregion

        string Content = "";
        string URL = "";
        pluginner.IFSPlugin FS;
		TextBox txtBox = new TextBox();
        PrintDocument Doc = new PrintDocument();
        string LastSearch = "";

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
            URL = url; FS = fsplugin;
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

		public bool CanPrint{get{return true;}}

		public void Print(){//печать на принтер
			//инициализация печати
            Doc.PrintPage += DrawTextOnPrn;
            Doc.DocumentName = URL;

            //запрашиваю параметры печати
            PrintDialog PrnSelector = new PrintDialog();
            PrnSelector.Document = Doc;
            if (PrnSelector.ShowDialog() == DialogResult.Cancel) return;

            //понеслась!
            Doc.Print();
		}

        /// <summary>
        /// Отрисовывает текст (хорошо сказал!) на принтере
        /// </summary>
        private void DrawTextOnPrn(object sender, PrintPageEventArgs e) //отрисовка текста, Printer.Print(string) больше нету :-(
        {
            e.Graphics.DrawString(txtBox.Text, txtBox.Font, System.Drawing.Brushes.Black, 10, 25);
        }

		public void PrintSettings(){ //Параметры страницы
            PageSetupDialog psd = new PageSetupDialog();
            psd.Document = Doc;
            psd.ShowDialog();
		}

        public ToolStripMenuItem[] SettingsMenu{//(под)меню настроек
            get{
                List<ToolStripMenuItem> Options = new List<ToolStripMenuItem>();
                Options.Add(new ToolStripMenuItem("У плагина нет настроек",null,this.Test));
                Options.Add(new ToolStripMenuItem("Шрифт...", null, this.SetFont));
                //Options.Add(new ToolStripSeparator());//Аргумент '1': преобразование типа из 'System.Windows.Forms.ToolStripSeparator' в 'System.Windows.Forms.ToolStripMenuItem' невозможно



                EncodingInfo[] Kodirovki = Encoding.GetEncodings();
                foreach (EncodingInfo cp in Kodirovki)
                {//looping all available codepages and adding it into menu
                    ToolStripMenuItem NewItem = new ToolStripMenuItem(cp.DisplayName + " (" + cp.Name + ")", null, this.ChCp);
                    NewItem.Tag = cp.CodePage;
                    Options.Add(NewItem);
                }

                return Options.ToArray();
            }
        }

        public void ChCp(object sender, EventArgs e)
        {
            ToolStripMenuItem SelItem = (ToolStripMenuItem)sender;
            Content = Encoding.GetEncoding(Convert.ToInt32(SelItem.Tag)).GetString(FS.GetFile(URL, new int()).Content);
            txtBox.Text = Content;
            //todo: поставить галочку
        }

        public void Test(object sender, EventArgs e){
            MessageBox.Show("У плагина нет настроек...пока что.");
        }

        public void SetFont(object sender, EventArgs e){//формат-выбор шрифта
            FontDialog fd = new FontDialog();
            fd.AllowScriptChange = false; //.net у нас юникодовый, кодировки менять нельзя
            fd.ShowColor = true;

            fd.Font = txtBox.Font;
            fd.Color = txtBox.ForeColor;

            fd.ShowDialog();

            txtBox.Font = fd.Font;
            txtBox.ForeColor = fd.Color;
        }

        public void Search(){//правка-поиск
            InputBox ibx = new InputBox(new Localizator().GetString("FCVWhatFind"));
            if (ibx.ShowDialog() == DialogResult.Cancel) return; //если нажали отмену
            int startPos = txtBox.Text.IndexOf(ibx.Result,txtBox.SelectionStart + 1/*чтобы искать дальнейшие вхождения*/);

            if (startPos == -1) { MessageBox.Show(new Localizator().GetString("FCVNothingFound"), null, MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            txtBox.SelectionStart = startPos;
            txtBox.SelectionLength = ibx.Result.Length;
            LastSearch = ibx.Result;
        }

        public void SearchNext(){//искать дальше
            if (LastSearch.Length == 0) { Search(); return; }

            int startPos = txtBox.Text.IndexOf(LastSearch, txtBox.SelectionStart + 1);
            if (startPos == -1) { MessageBox.Show(new Localizator().GetString("FCVNothingFound"), null, MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            txtBox.SelectionStart = startPos;
            txtBox.SelectionLength = LastSearch.Length;
        }

        /* TODO-list плагина TxtViewer
         * Вывод непечатаемых символов
         * Настройки шрифта, и прочего
         */
    }
}
