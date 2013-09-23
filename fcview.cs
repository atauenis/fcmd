/* The File Commander - просмоторщик файлов (FCView)
 * Главное окно просмоторщика
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace fcmd
{
    public partial class fcview : Form
    {
        pluginner.IViewerPlugin vp;
        pluginner.IFSPlugin fs;
        pluginfinder pf = new pluginfinder();
        string Path, PluginLink;
        Localizator locale = new Localizator();

        public fcview()
        {
            InitializeComponent();
        }

        public void LoadFile(string content, string URL){//загрузка txt-файлов
            pluginfinder pf = new pluginfinder();
            Path = URL;

			try {
                LoadFile(URL,new localFileSystem(),pf.GetFCVplugin(content));
			} catch (Exception ex) {
				MessageBox.Show(ex.Message + ex.StackTrace,URL,MessageBoxButtons.OK,MessageBoxIcon.Error);
				return;
			}
        }

        /// <summary>
        /// Loads the file with the custom FS and viewer plugins
        /// </summary>
        /// <param name="URL">The URL of the file</param>
        /// <param name="fs">The filesystem plugin for this file read</param>
        /// <param name="ViewWith">The viewer plugin</param>
        public void LoadFile(string URL, pluginner.IFSPlugin FS, pluginner.IViewerPlugin ViewWith)
        {
            this.Text = string.Format(locale.GetString("FCVTitle"), URL);
            this.UseWaitCursor = true;
            Path = URL;
            vp = ViewWith;
            vp.LoadFile(URL, pf.GetFSplugin(URL));

            if (vp.DisplayBox().BackColor != SystemColors.Window) pnlContainer.BorderStyle = BorderStyle.Fixed3D;
            // ^- если цвет фона плагина не оконный, рисовать рамку. Это выверт для подавления XP-стилей
            this.pnlContainer.Controls.Add(vp.DisplayBox());

            //"доработка" интерфейса fcview под текущий плагин
            mnuFilePrint.Enabled = vp.CanPrint;
            mnuFilePrintOptions.Enabled = vp.CanPrint;
            mnuEditCopy.Enabled = vp.CanCopy;
            mnuEditSelectAll.Enabled = vp.CanSelectAll;

            mnuFormat.DropDownItems.Clear();
            if (vp.SettingsMenu.Length > 0)
            {
                mnuFormat.DropDownItems.AddRange(vp.SettingsMenu);
            }

            //создание меню "вид" (список доступных плагинов)
            mnuView.DropDownItems.Clear();
            foreach (string Plugin4List in pf.ViewPlugins)
            {
                ToolStripMenuItem NewMenuItem = new ToolStripMenuItem(null, null,(object s, EventArgs ea) => SwitchPlugin((ToolStripMenuItem)s));
                NewMenuItem.Tag = Plugin4List.Split(";".ToCharArray())[1];
                NewMenuItem.Text = Plugin4List.Split(";".ToCharArray())[2];
                if (NewMenuItem.Tag.ToString() == PluginLink) NewMenuItem.Checked = true;
                mnuView.DropDownItems.Add(NewMenuItem);
            }

            this.Show();
            this.UseWaitCursor = false;
        }

        private void SwitchPlugin(ToolStripMenuItem SelectedByUser){
            PluginLink = SelectedByUser.Tag.ToString();
            foreach (ToolStripMenuItem tsmi in mnuView.DropDownItems){
                tsmi.Checked = false;
            }
            LoadFile(Path, fs, pf.LoadFCVPlugin(SelectedByUser.Tag.ToString()));
            SelectedByUser.Checked = true;
        }

        private void mnuFilePrint_Click(object sender, EventArgs e)
        {
            vp.Print();
        }

        private void mnuFilePrintOptions_Click(object sender, EventArgs e)
        {
            vp.PrintSettings();
        }

        private void mnuEditCopy_Click(object sender, EventArgs e)
        {
            vp.Copy();
        }

        private void mnuEditSelectAll_Click(object sender, EventArgs e)
        {
            vp.SelectAll();
        }

        private void mnuEditFind_Click(object sender, EventArgs e)
        {
            //vp.Search()
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
            //string AboutString = "FCView " + Application.ProductVersion + "\n" + "(C) 2013, Alexander Tauenis\n\n";
            string AboutString = String.Format(locale.GetString("FCViewVer"),Application.ProductVersion) + "\n" + "(C) 2013, Alexander Tauenis\n\n";
            AboutString += vp.Name + " " + vp.Version + "\n" + vp.Author;
            MessageBox.Show(AboutString, "File Commander Viewer", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void fcview_KeyDown(object sender, KeyEventArgs e){
#if DEBUG
            string DebugText = "Отладка клавиатуры FCView: нажата и отпущена клавиша " + e.KeyCode.ToString();
            DebugText += ", модификаторы: Ctrl=" + e.Control.ToString();
            DebugText += " Alt=" + e.Alt.ToString();
            DebugText += " Shift=" + e.Shift.ToString();
            Console.WriteLine(DebugText);
#endif
            switch (e.KeyData){
                case Keys.F4://вид
                    mnuView.ShowDropDown();
                    break;
                case Keys.F7: //поиск
                    mnuEditFind_Click(new object(),new EventArgs());
                    break;
                case Keys.F8: //формат (параметры плагина)
                    new ToolStripMenuItem(null, null, vp.SettingsMenu).ShowDropDown();
                    break;
                case Keys.F10:
                    this.Close();
                    break;
                case Keys.Q:
                    this.Close();
                    break;
            }
        }

        private void mnuFileReload_Click(object sender, EventArgs e){
            LoadFile(Path,fs,vp);
        }

        private void fcview_Load(object sender, EventArgs e)
        {
            this.OnResize(new EventArgs());
            Localize();
        }

        private void tsKeyboard_ItemClicked(object sender, ToolStripItemClickedEventArgs e){
            switch (e.ClickedItem.Name){
                case "tsbHelpF1": this.OnKeyDown(new KeyEventArgs(Keys.F1)); break;
                case "tsbHelpF2": this.OnKeyDown(new KeyEventArgs(Keys.F2)); break;
                case "tsbHelpF3": this.OnKeyDown(new KeyEventArgs(Keys.F3)); break;
                case "tsbHelpF4": this.OnKeyDown(new KeyEventArgs(Keys.F4)); break;
                case "tsbHelpF5": this.OnKeyDown(new KeyEventArgs(Keys.F5)); break;
                case "tsbHelpF6": this.OnKeyDown(new KeyEventArgs(Keys.F6)); break;
                case "tsbHelpF7": this.OnKeyDown(new KeyEventArgs(Keys.F7)); break;
                case "tsbHelpF8": this.OnKeyDown(new KeyEventArgs(Keys.F8)); break;
                case "tsbHelpF9": this.OnKeyDown(new KeyEventArgs(Keys.F9)); break;
                case "tsbHelpF10": this.OnKeyDown(new KeyEventArgs(Keys.F10)); break;
            }
        }

        private void fcview_Resize(object sender, EventArgs e)
        {
            tsKeyboard.Width = this.Width;
        }

        /// <summary>
        /// Перевести весь интерфейс на текущий язык
        /// </summary>
        public void Localize(){
            tsbHelpF1.Text = locale.GetString("FCVF1");
            tsbHelpF2.Text = locale.GetString("FCVF2");
            tsbHelpF3.Text = locale.GetString("FCVF3");
            tsbHelpF4.Text = locale.GetString("FCVF4");
            tsbHelpF5.Text = locale.GetString("FCVF5");
            tsbHelpF6.Text = locale.GetString("FCVF6");
            tsbHelpF7.Text = locale.GetString("FCVF7");
            tsbHelpF8.Text = locale.GetString("FCVF8");
            tsbHelpF9.Text = locale.GetString("FCVF9");
            tsbHelpF10.Text = locale.GetString("FCVF10");

            mnuFile.Text = locale.GetString("FCVFile");
            mnuFileOpen.Text = locale.GetString("FCVFileOpen");
            mnuFileReload.Text = locale.GetString("FCVFileReload");
            mnuFilePrint.Text = locale.GetString("FCVFilePrint");
            mnuFilePrintOptions.Text = locale.GetString("FCVFilePrintOptions");
            mnuFileExit.Text = locale.GetString("FCVFileExit");

            mnuEdit.Text = locale.GetString("FCVEdit");
            mnuEditCopy.Text = locale.GetString("FCVEditCopy");
            mnuEditSelectAll.Text = locale.GetString("FCVEditSelAll");
            mnuEditFind.Text = locale.GetString("FCVEditSearch");

            mnuView.Text = locale.GetString("FCVView");

            mnuFormat.Text = locale.GetString("FCVFormat");
            mnuHelp.Text = locale.GetString("FCVHelpMenu");
            mnuHelpAbout.Text = locale.GetString("FCVHelpAbout");
        }

        private void mnuFormat_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //обновление спика параметров плагина (меню Формат)
            if (vp.SettingsMenu.Length > 0){
                mnuFormat.DropDownItems.Clear();
                mnuFormat.DropDownItems.AddRange(vp.SettingsMenu);
            }
        }
    }
}
