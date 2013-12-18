/* The File Commander - simple Viewer/Editor
 * The main window of the viewer/editor (VE)
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
    /// <summary>Viewer-Editor</summary>
    class VEd : Xwt.Window
    {
        Localizator Locale = new Localizator();
        pluginner.IVEPlugin Plugin;
        pluginner.IFSPlugin FSPlugin;

        //Xwt.Menu MainMenu = new Xwt.Menu();
        Xwt.MenuItem mnuFile = new Xwt.MenuItem() { Tag="mnuFile"};
        Xwt.MenuItem mnuFileNew = new Xwt.MenuItem() { Tag="mnuFileNew"};
        Xwt.MenuItem mnuFileOpen = new Xwt.MenuItem() { Tag="mnuFileOpen"};
        Xwt.MenuItem mnuFileReload = new Xwt.MenuItem() { Tag="mnuFileReload"};
        Xwt.MenuItem mnuFileSave = new Xwt.MenuItem() { Tag="mnuFileSave"};
        Xwt.MenuItem mnuFilePrint = new Xwt.MenuItem() { Tag="mnuFilePrint"};
        Xwt.MenuItem mnuFilePrintSettings = new Xwt.MenuItem() { Tag="mnuFilePrintSettings"};
        Xwt.MenuItem mnuFilePrintPreview = new Xwt.MenuItem() { Tag="mnuFilePrintPreview"};
        Xwt.MenuItem mnuFileClose = new Xwt.MenuItem() { Tag="mnuFileClose"};

        Xwt.MenuItem mnuEdit = new Xwt.MenuItem() { Tag = "mnuEdit",UseMnemonic = true };
        Xwt.MenuItem mnuEditCut = new Xwt.MenuItem() { Tag = "mnuEditCut" };
        Xwt.MenuItem mnuEditCopy = new Xwt.MenuItem() { Tag = "mnuEditCopy" };
        Xwt.MenuItem mnuEditPaste = new Xwt.MenuItem() { Tag = "mnuEditPaste" };
        Xwt.MenuItem mnuEditSelectAll = new Xwt.MenuItem() { Tag = "mnuEditSelAll" };
        Xwt.MenuItem mnuEditFindReplace = new Xwt.MenuItem() { Tag = "mnuEditSearch" };
        Xwt.MenuItem mnuEditFindNext = new Xwt.MenuItem() { Tag = "mnuEditSearchNext" };

        Xwt.MenuItem mnuView = new Xwt.MenuItem(); //auto filled
        Xwt.MenuItem mnuFormat = new Xwt.MenuItem(); // ---//---

        Xwt.MenuItem mnuHelp = new Xwt.MenuItem();
        Xwt.MenuItem mnuHelpHelpme = new Xwt.MenuItem() { Tag = "mnuHelpHelpme" };
        Xwt.MenuItem mnuHelpAbout = new Xwt.MenuItem() { Tag = "mnuHelpAbout" };

        Xwt.VBox Layout = new Xwt.VBox();
        Xwt.Widget PluginBody;
        Xwt.TextEntry CommandBox = new Xwt.TextEntry();
        Xwt.HBox KeyBoardHelp = new Xwt.HBox();
        Xwt.Button[] KeybHelpButtons = new Xwt.Button[11]; //одна лишняя, которая [0]

        public VEd()
        {
            for (int i = 1; i < 11; i++){
                KeybHelpButtons[i] = new Xwt.Button(Locale.GetString("FCVE_F"+i));
                KeybHelpButtons[i].Style = Xwt.ButtonStyle.Flat;
                KeybHelpButtons[i].CanGetFocus = false;
                KeyBoardHelp.PackStart(KeybHelpButtons[i]);
            }

            this.Title = Locale.GetString("File Commander VE");
            this.Content = Layout;

            CommandBox.KeyReleased += new EventHandler<Xwt.KeyEventArgs>(CommandBox_KeyReleased);

            mnuFile.Label = Locale.GetString("FCVE_mnuFile");
            mnuFile.SubMenu = new Xwt.Menu();
            mnuFile.SubMenu.Items.Add(mnuFileNew);
            mnuFile.SubMenu.Items.Add(mnuFileOpen);
            mnuFile.SubMenu.Items.Add(mnuFileReload);
            mnuFile.SubMenu.Items.Add(mnuFileSave);
            mnuFile.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
            mnuFile.SubMenu.Items.Add(mnuFilePrint);
            mnuFile.SubMenu.Items.Add(mnuFilePrintPreview);
            mnuFile.SubMenu.Items.Add(mnuFilePrintSettings);
            mnuFile.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
            mnuFile.SubMenu.Items.Add(mnuFileClose);
            TranslateMenu(mnuFile.SubMenu);

            mnuEdit.Label = Locale.GetString("FCVE_mnuEdit");
            mnuEdit.SubMenu = new Xwt.Menu();
            mnuEdit.SubMenu.Items.Add(mnuEditCut);
            mnuEdit.SubMenu.Items.Add(mnuEditCopy);
            mnuEdit.SubMenu.Items.Add(mnuEditPaste);
            mnuEdit.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
            mnuEdit.SubMenu.Items.Add(mnuEditSelectAll);
            mnuEdit.SubMenu.Items.Add(new Xwt.SeparatorMenuItem());
            mnuEdit.SubMenu.Items.Add(mnuEditFindReplace);
            mnuEdit.SubMenu.Items.Add(mnuEditFindNext);
            TranslateMenu(mnuEdit.SubMenu);

            mnuView.Label = Locale.GetString("FCVE_mnuView");
            mnuFormat.Label = Locale.GetString("FCVE_mnuFormat");

            mnuHelp.Label = Locale.GetString("FCVE_mnuHelp");
            mnuHelp.SubMenu = new Xwt.Menu();
            mnuHelp.SubMenu.Items.Add(mnuHelpHelpme);
            mnuHelp.SubMenu.Items.Add(mnuHelpAbout);
            TranslateMenu(mnuHelp.SubMenu);

            this.MainMenu = new Xwt.Menu();
            this.MainMenu.Items.Add(mnuFile);
            this.MainMenu.Items.Add(mnuEdit);
            this.MainMenu.Items.Add(mnuView);
            this.MainMenu.Items.Add(mnuFormat);
            this.MainMenu.Items.Add(mnuHelp);

            mnuFileOpen.Clicked += (o, ea) => { OpenFile(); };
            mnuFilePrint.Clicked += (o, ea) => { SendCommand("print"); };
            mnuFilePrintSettings.Clicked += (o, ea) => { SendCommand("pagesetup"); };
            mnuFilePrintPreview.Clicked += (o, ea) => { SendCommand("print preview"); };
            mnuFileClose.Clicked += (o, ea) => { Exit(); };
            mnuEditCut.Clicked += (o, ea) => { SendCommand("cut selected"); };
            mnuEditCopy.Clicked += (o, ea) => { SendCommand("copy selected"); };
            mnuEditPaste.Clicked += (o, ea) => { SendCommand("paste clipboard"); };
            mnuEditSelectAll.Clicked += (o, ea) => { SendCommand("select *"); };
            mnuEditFindReplace.Clicked += (o, ea) => { SendCommand("findreplace"); };
            mnuEditFindNext.Clicked += (o, ea) => { SendCommand("findreplace last"); };
            mnuHelpAbout.Clicked += new EventHandler(mnuHelpAbout_Clicked);
            this.CloseRequested += (o, ea) => { SendCommand("unload"); };

            PluginBody = new Xwt.Spinner() { Animate = true };

            BuildLayout();
        }

        void mnuHelpAbout_Clicked(object sender, EventArgs e)
        {
            string AboutString1 = String.Format(Locale.GetString("FCVEVer1"), System.Windows.Forms.Application.ProductVersion); ;
            string AboutString2 = string.Format(Locale.GetString("FCVEVer2"), Plugin.Name, Plugin.Version,"\n", Plugin.Author);
            Xwt.MessageDialog.ShowMessage(AboutString1,AboutString2);
        }
        
        void CommandBox_KeyReleased(object sender, Xwt.KeyEventArgs e)
        {
            if (e.Key == Xwt.Key.Return)
            {
                SendCommand(CommandBox.Text);
            }
        }

        /// <summary>
        /// Sends a command to the plugin
        /// </summary>
        /// <param name="Cmd">the command in DOS/*NIX-style: NAME ARG1 ARG2 ARG3</param>
        void SendCommand(string Cmd)
        {
            Plugin.ExecuteCommand(Cmd.Split(" ".ToCharArray())[0], Cmd.Split(" ".ToCharArray(),StringSplitOptions.RemoveEmptyEntries));
            CommandBox.Text = "";
        }

        /// <summary>Sets VE mode (view or edit)</summary>
        /// <param name="AllowEdit">Set to true if edit controls should be showed</param>
        void SetVEMode(bool AllowEdit)
        {
            mnuFileSave.Visible = AllowEdit;
            mnuFileNew.Visible = AllowEdit;
            mnuEditCut.Visible = AllowEdit;
            mnuEditPaste.Visible = AllowEdit;
        }

        /// <summary>Load the file in the VE (with plugin autodetection)</summary>
        /// <param name="URL">The file's URL</param>
        /// <param name="FS">The file's filesystem</param>
        /// <param name="AllowEdit">Mode of VE: true=editor, false=viewer</param>
        public void LoadFile(string URL, pluginner.IFSPlugin FS, bool AllowEdit)
        {
            string content = Encoding.UTF8.GetString(FS.GetFile(URL, new int()).Content);
            pluginfinder pf = new pluginfinder();

            try
            {
                string GottenHeaders;
                if (content.Length >= 20) GottenHeaders = content.Substring(0, 20);
                else GottenHeaders = content;
                LoadFile(URL, FS, pf.GetFCVEplugin("NAME=" + URL + "HEADERS=" + GottenHeaders), AllowEdit);
            }
            catch (pluginfinder.PluginNotFoundException ex)
            {
                Console.WriteLine("ERROR: VE plugin is not loaded: " + ex.Message + "\n" + ex.StackTrace);
                Xwt.MessageDialog.ShowError(Locale.GetString("FCVE_PluginNotFound"));
                LoadFile(URL, FS, new base_plugins.ve.PlainText(), AllowEdit);
            }
            catch (Exception ex)
            {

                Console.WriteLine("VE can't load file: " + ex.Message + "\n" + ex.StackTrace);
                return;
            }
        }

        /// <summary>Load the file in the VE</summary>
        /// <param name="URL">The URL of the file</param>
        /// <param name="FS">The filesystem of the file</param>
        /// <param name="plugin">The VE plugin, which will be used to load this file</param>
        /// <param name="AllowEdit">Allow editing the file</param>
        public void LoadFile(string URL, pluginner.IFSPlugin FS, pluginner.IVEPlugin plugin, bool AllowEdit)
        {
            if(AllowEdit)
                this.Title = string.Format(Locale.GetString("FCETitle"), URL);
            else
                this.Title = string.Format(Locale.GetString("FCVTitle"), URL);

            Plugin = plugin;
            Plugin.ReadOnly = !AllowEdit;
            Plugin.OpenFile(URL, FS);
            mnuFormat.SubMenu = Plugin.FormatMenu;

            bool Mode = AllowEdit;

            if (!Plugin.CanEdit && AllowEdit)
            {
                Xwt.MessageDialog.ShowWarning(String.Format(Locale.GetString("FCVEpluginro1"), Plugin.Name + " " + Plugin.Version), Locale.GetString("FCVEpluginro2"));
                Mode = false;
            }

            FSPlugin = FS;
            PluginBody = Plugin.Body;
            SetVEMode(Mode);
            BuildLayout();
        }

        private void OpenFile()
        {
            FileChooser OpenFileDialog = new FileChooser(FSPlugin);
            if (OpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.Cancel || OpenFileDialog.SelectedFile == null) return;
            SendCommand("unload");
            LoadFile(OpenFileDialog.SelectedFile, OpenFileDialog.listPanel1.FSProvider, false);//undone: добавить запись тек. состояния VE.
        }

        /// <summary>(Re)builds the "Layout" vbox</summary>
        private void BuildLayout()
        {
            Layout.Clear();
            Layout.PackStart(PluginBody, true, Xwt.WidgetPlacement.Fill, Xwt.WidgetPlacement.Fill, -12, -12, -12);
            Layout.PackStart(CommandBox, false, Xwt.WidgetPlacement.Fill, Xwt.WidgetPlacement.Fill, -12, -6, -12, -12);
            if(fcmd.Properties.Settings.Default.ShowKeybrdHelp) Layout.PackStart(KeyBoardHelp, false, Xwt.WidgetPlacement.Fill, Xwt.WidgetPlacement.Fill, -12, 6, -12, -12);
            //the values -12, -6 and 6 are need for 0px margins, and found experimentally.
            //as those experiments showed, these values is measured in pixels. Live and learn!
        }


        /// <summary>Translates the <paramref name="mnu"/> into the current UI language</summary>
        private void TranslateMenu(Xwt.Menu mnu)
        {
            try
            { //dirty hack...i don't know why, but "if(mnu.Items == null) return;" raises NullReferenceException...
                foreach (Xwt.MenuItem currentMenuItem in mnu.Items)
                {
                    if (currentMenuItem.GetType() != typeof(Xwt.SeparatorMenuItem))
                    { //skip separators
                        currentMenuItem.Label = Locale.GetString("FCVE_" + currentMenuItem.Tag.ToString());
                        TranslateMenu(currentMenuItem.SubMenu);
                    }
                }
            }
            catch { }
        }

        private void Exit()
        {
            SendCommand("unload");
            this.Hide();
        }

        
    }
}
