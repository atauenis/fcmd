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
using fcmd.base_plugins.fs;

namespace fcmd
{
    public partial class fcview : Form
    {
        /* TODO-list for FCView
         * Вызов справки
         * Окно с прогрессом загрузки
         */


        pluginner.IViewerPlugin vp;
        pluginner.IFSPlugin fs;
        pluginfinder pf = new pluginfinder();
        string Path, PluginLink;
        Localizator locale = new Localizator();

        public fcview()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Loads the file <paramref name="URL"/> with automatically selected plugin
        /// </summary>
        /// <param name="URL">The file's URL</param>
        /// <param name="FS">The file's filesystem plugin</param>

        public void LoadFile(string URL, pluginner.IFSPlugin FS){// "text/plain" loading
            string content = Encoding.UTF8.GetString(FS.GetFile(URL,new int()).Content);
            pluginfinder pf = new pluginfinder();
            Path = URL;

			try {
                string GottenHeaders;
                if(content.Length >= 20) GottenHeaders = content.Substring(0,20);
                else GottenHeaders = content;
                LoadFile(URL, new localFileSystem(), pf.GetFCVplugin("NAME=" + URL + "HEADERS=" + GottenHeaders));
			} catch (Exception ex) {
				MessageBox.Show(ex.Message + "\n" + ex.StackTrace,URL,MessageBoxButtons.OK,MessageBoxIcon.Error);
                Console.WriteLine("fcview can't load file: " + ex.Message + "\n" + ex.StackTrace);
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
            fs = FS;
            vp = ViewWith;

#if DEBUG
            Console.WriteLine("load fcv plugin: " + vp.Name + " displaybox type:" + vp.DisplayBox.GetType().ToString());
#endif
            vp.LoadFile(URL, pf.GetFSplugin(URL));

            //initialize xwt
            switch (Environment.OSVersion.Platform){
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
            //prepare the xwt-fcmd bridge and return to the reality the plugin's DISPLAYBOX
            Xwt.Toolkit t = Xwt.Toolkit.CurrentEngine;
            ElHo.Child = (System.Windows.UIElement)t.GetNativeWidget(vp.DisplayBox);

            //tuning the fcview ui for the current plugin
            mnuFilePrint.Enabled = vp.CanPrint;
            mnuFilePrintOptions.Enabled = vp.CanPrint;
            mnuEditCopy.Enabled = vp.CanCopy;
            mnuEditSelectAll.Enabled = vp.CanSelectAll;

            PopulateMenuFormat();
            

            //building the "view" menu (the list of available plugins)
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

        private void PopulateMenuFormat(){
            mnuFormat.DropDownItems.Clear();
            foreach (Xwt.MenuItem CurMenuItem in vp.SettingsMenu){
                mnuFormat.DropDownItems.Add(
                    CurMenuItem.Label,
                    null,
                    (object s, EventArgs ea) =>
                    {
                        //undone: OnClick code here
                        /*какого мужского полового органа M$'овцам до сих пор так и
                          не допёрло сделать нормальный RaiseEvent в шарпе...
                          коммерческий продукт ёпте...накипело*/
                    });
                //todo: submenus, images
            }
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
            vp.Search();
        }

        private void mnuHelpAbout_Click(object sender, EventArgs e)
        {
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
                    mnuFormat.ShowDropDown();
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

        [STAThread]
        private void fcview_Load(object sender, EventArgs e)
        {
            this.OnResize(new EventArgs());
            ElHo.Dock = DockStyle.Fill;
            toolStripContainer1.ContentPanel.Controls.Add(ElHo);
            Localize();
         }

        System.Windows.Forms.Integration.ElementHost ElHo = new System.Windows.Forms.Integration.ElementHost();
        //http://atsoftware.gb7.ru/blog/2013/11/elementhost-invalidoperationexception/
        
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
        /// Translate entrie UI into the locale
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
            mnuEditFindNext.Text = locale.GetString("FCVEditSearchNext");

            mnuView.Text = locale.GetString("FCVView");

            mnuFormat.Text = locale.GetString("FCVFormat");
            mnuHelp.Text = locale.GetString("FCVHelpMenu");
            mnuHelpAbout.Text = locale.GetString("FCVHelpAbout");
        }

        private void mnuFormat_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            //обновление спика параметров плагина (меню Формат)
            //update plugin options list (menu "format")
            /*if (vp.SettingsMenu.Length > 0){
                mnuFormat.DropDownItems.Clear();
                mnuFormat.DropDownItems.AddRange(vp.SettingsMenu);
            }*///UNDONE
        }

        private void mnuEditFindNext_Click(object sender, EventArgs e)
        {
            vp.SearchNext();
        }

        private void mnuFileExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void mnuFileOpen_Click(object sender, EventArgs e)
        {
            //todo
        }

    }
}
