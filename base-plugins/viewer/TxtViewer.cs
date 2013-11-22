/* The File Commander file VIEWer   Просмоторщик файлов FCView
 * Default/txt file viewer.         Модуль вывода текстовых файлов
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
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
        
        List<ToolStripMenuItem> Options = new List<ToolStripMenuItem>();
        string Content = "";
        string URL = "";
        pluginner.IFSPlugin FS;
        Xwt.TextEntry txtBox = new Xwt.TextEntry();
        PrintDocument Doc = new PrintDocument();
        string LastSearch = "";

        public Xwt.Widget DisplayBox{
            get
            {
                //initialize xwt
                switch (Environment.OSVersion.Platform)
                {
                    case PlatformID.Win32NT:
                        Xwt.Application.InitializeAsGuest(Xwt.ToolkitType.Wpf);
                        break;
                    case PlatformID.MacOSX: //i don't sure that Mono detect OSX as OSX, not Unix; see http://mono.wikia.com/wiki/Detecting_the_execution_platform
                        Xwt.Application.InitializeAsGuest(Xwt.ToolkitType.Cocoa);
                        break;
                    default:
                    case PlatformID.Unix: //gtk fallback for unknown oses
                        Xwt.Application.InitializeAsGuest(Xwt.ToolkitType.Gtk);
                        break;
                }

                txtBox.Name = "txtBox";
                txtBox.MultiLine = true;
                txtBox.Text = Content;
                txtBox.ReadOnly = true;
                txtBox.ShowFrame = false;
                txtBox.ShowScrollBars = true;
                return txtBox;
            }
        }

		public void LoadFile(string url, pluginner.IFSPlugin fsplugin){
            URL = url; FS = fsplugin;
            Content = Encoding.UTF8.GetString(fsplugin.GetFile (url, new int()).Content);
		}

		public bool CanCopy{get{return true;}}

		public void Copy(){//edit-copy
            string SelText = txtBox.Text.Substring(txtBox.SelectionStart, txtBox.SelectionLength);
            Xwt.Clipboard.SetText(SelText);
		}

		public bool CanSelectAll{get{return true;}}

		public void SelectAll(){//edit-select all
			txtBox.SelectionStart = 0;
			txtBox.SelectionLength = txtBox.Text.Length;
		}

		public bool CanPrint{get{return true;}}

		public void Print(){//печать на принтер
			//initialize printer API
            Doc.PrintPage += DrawTextOnPrn;
            Doc.DocumentName = URL;

            //request printing settings
            //currently not cross-platform
            System.Windows.Forms.PrintDialog PrnSelector = new System.Windows.Forms.PrintDialog();
            PrnSelector.Document = Doc;
            if (PrnSelector.ShowDialog() == System.Windows.Forms.DialogResult.Cancel) return;

            //go!
            Doc.Print();
		}

        /// <summary>
        /// Draws the text (funny words :-) ) on the printer
        /// </summary>
        private void DrawTextOnPrn(object sender, PrintPageEventArgs e) //отрисовка текста, Printer.Print(string) больше нету :-(
        {
            e.Graphics.DrawString(txtBox.Text, ConvertXwtFont(txtBox.Font), System.Drawing.Brushes.Black, 10, 25);
        }

		public void PrintSettings(){ //file-page setup
            PageSetupDialog psd = new PageSetupDialog();
            psd.Document = Doc;
            psd.ShowDialog();
		}

        //public ToolStripMenuItem[] SettingsMenu{//(под)меню настроек
        //    get{
        //        Options.Clear();
        //        Options.Add(new ToolStripMenuItem("У плагина нет настроек",null,this.Test));
        //        Options.Add(new ToolStripMenuItem("Шрифт...", null, this.SetFont));
        //        //Options.Add(new ToolStripSeparator());//Аргумент '1': преобразование типа из 'System.Windows.Forms.ToolStripSeparator' в 'System.Windows.Forms.ToolStripMenuItem' невозможно



        //        EncodingInfo[] Kodirovki = Encoding.GetEncodings();
        //        foreach (EncodingInfo cp in Kodirovki)
        //        {//looping all available codepages and adding it into menu
        //            ToolStripMenuItem NewItem = new ToolStripMenuItem(cp.DisplayName + " (" + cp.Name + ")", null, this.ChCp);
        //            NewItem.Tag = cp.CodePage;
        //            Options.Add(NewItem);
        //        }

        //        return Options.ToArray();
        //    }
        //}

        public List<Xwt.MenuItem> SettingsMenu //the plugin's settings menu (in windows fcview - 'format')
        {
            get
            {
                List<Xwt.MenuItem> mnuFormat = new List<Xwt.MenuItem>();
                Xwt.MenuItem mnuFormatTest = new Xwt.MenuItem("test");
                mnuFormatTest.Clicked += this.Test;
                mnuFormat.Add(mnuFormatTest);

                EncodingInfo[] Kodirovki = Encoding.GetEncodings();
                foreach (EncodingInfo cp in Kodirovki)
                {//reading all available codepages and adding it into menu
                    Xwt.CheckBoxMenuItem NewItem = new Xwt.CheckBoxMenuItem();
                    NewItem.Label = cp.DisplayName;
                    NewItem.Clicked += this.ChCp;
                    mnuFormat.Add(NewItem);
                }

                return mnuFormat;
            }
        }

        public void ChCp(object sender, EventArgs e) //change codepage
        {
            //ToolStripMenuItem SelItem = (ToolStripMenuItem)sender;
            //Content = Encoding.GetEncoding(Convert.ToInt32(SelItem.Tag)).GetString(FS.GetFile(URL, new int()).Content);
            //txtBox.Text = Content;

            ////set a tick
            //foreach (ToolStripMenuItem stroka in Options)
            //{
            //    if (Convert.ToInt32(SelItem.Tag) == Convert.ToInt32(stroka.Tag))
            //    {//this is that!
            //        stroka.Checked = true;
            //    }
            //    else
            //    {//no, removing possibly mark
            //        stroka.Checked = false;
            //    }
            //}

            Xwt.CheckBoxMenuItem SelItem = (Xwt.CheckBoxMenuItem)sender;
            Content = Encoding.GetEncoding(SelItem.Label).GetString(FS.GetFile(URL, new int()).Content);
            txtBox.Text = Content;
            //undone: отладить, скорее всего, будет падать из-за несоответствия типов
            foreach (Xwt.CheckBoxMenuItem CurItem in SettingsMenu)
            {
                if (Convert.ToInt32(SelItem.Label) == Convert.ToInt32(CurItem.Label))
                {//this is that!
                    SelItem.Checked = true;
                }
                else
                {//no, removing possibly mark
                    CurItem.Checked = false;
                }
            }
        }

        public void Test(object sender, EventArgs e){ //this plugin have no settings (placeholder)
            Xwt.MessageDialog.ShowMessage("У плагина нет настроек...пока что.");
        }

        public void SetFont(object sender, EventArgs e){//format-font
            //currently not cross platform!
            System.Windows.Forms.FontDialog fd = new System.Windows.Forms.FontDialog();
            fd.AllowScriptChange = false; //.net strings are unicode, need to disable changing the cp of the textbox
            fd.ShowColor = true;
            //fd.Font = txtBox.Font;
            //fd.Color = txtBox.ForeColor;

            fd.ShowDialog();

            //txtBox.Font = fd.Font;
            //txtBox.ForeColor = fd.Color;
            
            //undone: the default xwt is too simple
            //нужно найти способ изменения шрифта текстэнтри или внести правки в иксвэтэ :-)
        }

        public void Search(){//правка-поиск
            //todo: rewrite with xwt
            InputBox ibx = new InputBox(new Localizator().GetString("FCVWhatFind"));
            if (ibx.ShowDialog() == DialogResult.Cancel) return; //если нажали отмену
            int startPos = txtBox.Text.IndexOf(ibx.Result,txtBox.SelectionStart + 1/*чтобы искать дальнейшие вхождения*/);

            if (startPos == -1) { MessageBox.Show(new Localizator().GetString("FCVNothingFound"), null, MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            txtBox.SelectionStart = startPos;
            txtBox.SelectionLength = ibx.Result.Length;
            LastSearch = ibx.Result;
        }

        public void SearchNext(){//искать дальше
            //todo: partially rewrite with use of xwt
            if (LastSearch.Length == 0) { Search(); return; }

            int startPos = txtBox.Text.IndexOf(LastSearch, txtBox.SelectionStart + 1);
            if (startPos == -1) { MessageBox.Show(new Localizator().GetString("FCVNothingFound"), null, MessageBoxButtons.OK, MessageBoxIcon.Information); return; }
            txtBox.SelectionStart = startPos;
            txtBox.SelectionLength = LastSearch.Length;
        }

        /// <summary>
        /// Converts XWT font to WinForms (GDI+) font
        /// </summary>
        /// <param name="originalfont">The original XWT font</param>
        /// <returns>The resultating GDI+ font</returns>
        private System.Drawing.Font ConvertXwtFont(Xwt.Drawing.Font originalfont)
        {//todo: обработка font.style'в 
            System.Drawing.Font newfont = new System.Drawing.Font(originalfont.Family, (float)originalfont.Size);
            return newfont;
        }

        /* TODO-list плагина TxtViewer
         * Вывод непечатаемых символов
         * Настройки шрифта, и прочего
         */
    }
}
