/* The File Commander main window
 * The main file manager window
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace fcmd
{
    partial class MainWindow : Xwt.Window
    {
        Localizator Locale = new Localizator();
        Xwt.Menu MainMenu = new Xwt.Menu();
        Xwt.VBox Layout = new Xwt.VBox();
        Xwt.HPaned PanelLayout = new Xwt.HPaned();

        pluginner.FileListPanel p1;
        pluginner.FileListPanel p2;
        
        /// <summary>The current active panel</summary>
        pluginner.FileListPanel ActivePanel;
        /// <summary>The current inactive panel</summary>
        pluginner.FileListPanel PassivePanel;

        Xwt.TextEntry CommandBox = new Xwt.TextEntry();
        Xwt.HBox KeyBoardHelp = new Xwt.HBox();
        Xwt.Button[] KeybHelpButtons = new Xwt.Button[11]; //одна лишняя, которая нумбер [0]


        public MainWindow()
        {
            this.Title = "File Commander";
            this.CloseRequested += new Xwt.CloseRequestedHandler(MainWindow_CloseRequested);
            PanelLayout.KeyReleased += new EventHandler<Xwt.KeyEventArgs>(PanelLayout_KeyReleased);

            Layout.PackStart(PanelLayout, true, Xwt.WidgetPlacement.Fill, Xwt.WidgetPlacement.Fill, -12, -12, -12,12);
            Layout.PackStart(CommandBox,false,Xwt.WidgetPlacement.End,Xwt.WidgetPlacement.Fill,-12,-12,-12);
            Layout.PackStart(KeyBoardHelp, false,Xwt.WidgetPlacement.End,Xwt.WidgetPlacement.Start,-12,-6,-12,-12);
            
            this.Content = Layout;
            
            //load bookmarks
            string BookmarksStore = null;
            if (fcmd.Properties.Settings.Default.BookmarksFile != null && fcmd.Properties.Settings.Default.BookmarksFile.Length > 0)
            {
                BookmarksStore = File.ReadAllText(fcmd.Properties.Settings.Default.BookmarksFile, Encoding.UTF8);

            }

            //build panels
            PanelLayout.Panel1.Content = new pluginner.FileListPanel(BookmarksStore); //Левая, правая где сторона? Улица, улица, ты, брат, пьяна!
            PanelLayout.Panel2.Content = new pluginner.FileListPanel(BookmarksStore);

            p1 = (PanelLayout.Panel1.Content as pluginner.FileListPanel);
            p2 = (PanelLayout.Panel2.Content as pluginner.FileListPanel);
            
            p1.FS = new base_plugins.fs.localFileSystem();
            p1.ListingView.Columns.Add(Locale.GetString("FName"), p1.dfDisplayName);
            p1.ListingView.Columns.Add(Locale.GetString("FSize"), p1.dfSize);
            p1.ListingView.Columns.Add(Locale.GetString("FDate"), p1.dfChanged);

            p2.FS = new base_plugins.fs.localFileSystem();
            p2.ListingView.Columns.Add(Locale.GetString("FName"), p1.dfDisplayName);
            p2.ListingView.Columns.Add(Locale.GetString("FSize"), p1.dfSize);
            p2.ListingView.Columns.Add(Locale.GetString("FDate"), p1.dfChanged);

            p1.GotFocus += (o, ea) => { SwitchPanel(p1); };
            p2.GotFocus += (o, ea) => { SwitchPanel(p2); };

            //build keyboard help bar
            for (int i = 1; i < 11; i++)
            {
                KeybHelpButtons[i] = new Xwt.Button(Locale.GetString("FCF" + i));
                KeybHelpButtons[i].Style = Xwt.ButtonStyle.Flat;
                KeybHelpButtons[i].CanGetFocus = false;
                //KeybHelpButtons[i].Clicked += (o, ea) => { Xwt.MessageDialog.ShowMessage("Button " + i); };//anywhere shows "Button 11"...what the fucks?
                KeyBoardHelp.PackStart(KeybHelpButtons[i]);
            }
            KeybHelpButtons[1].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this,new Xwt.KeyEventArgs(Xwt.Key.F1,Xwt.ModifierKeys.None,false,0)); };
            KeybHelpButtons[2].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F2, Xwt.ModifierKeys.None, false, 0)); };
            KeybHelpButtons[3].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F3, Xwt.ModifierKeys.None, false, 0)); };
            KeybHelpButtons[4].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F4, Xwt.ModifierKeys.None, false, 0)); };
            KeybHelpButtons[5].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F5, Xwt.ModifierKeys.None, false, 0)); };
            KeybHelpButtons[6].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F6, Xwt.ModifierKeys.None, false, 0)); };
            KeybHelpButtons[7].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F7, Xwt.ModifierKeys.None, false, 0)); };
            KeybHelpButtons[8].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F8, Xwt.ModifierKeys.None, false, 0)); };
            KeybHelpButtons[9].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F9, Xwt.ModifierKeys.None, false, 0)); };
            KeybHelpButtons[10].Clicked += (o, ea) => { this.PanelLayout_KeyReleased(this, new Xwt.KeyEventArgs(Xwt.Key.F10, Xwt.ModifierKeys.None, false, 0)); };
            //todo: implement "object Xwt.Widget.Tag" in modXWT (like Winforms's any Control.Tag)

            //apply user's settings
            //file size display policy
            char[] Policies = fcmd.Properties.Settings.Default.SizeShorteningPolicy.ToCharArray();

            //default directories
            if (Properties.Settings.Default.Panel1URL.Length != 0)
                p1.LoadDir(Properties.Settings.Default.Panel1URL, ConvertSDP(Policies[0]), ConvertSDP(Policies[1]), ConvertSDP(Policies[2]));
            else
                p1.LoadDir("file://" + System.Windows.Forms.Application.StartupPath, ConvertSDP(Policies[0]), ConvertSDP(Policies[1]), ConvertSDP(Policies[2]));

            if (Properties.Settings.Default.Panel2URL.Length != 0)
                p2.LoadDir(Properties.Settings.Default.Panel2URL, ConvertSDP(Policies[0]), ConvertSDP(Policies[1]), ConvertSDP(Policies[2]));
            else
                p2.LoadDir("file://"+System.Windows.Forms.Application.StartupPath,ConvertSDP(Policies[0]), ConvertSDP(Policies[1]), ConvertSDP(Policies[2]));

            //default panel
            switch (fcmd.Properties.Settings.Default.LastActivePanel)
            {
                case 1:
                    p1.ListingView.SetFocus();
                    ActivePanel = p1; PassivePanel = p2;
                    break;
                case 2:
                    p2.ListingView.SetFocus();
                    ActivePanel = p2; PassivePanel = p1;
                    break;
                default:
                    p1.ListingView.SetFocus();
                    ActivePanel = p1; PassivePanel = p2;
                    break;
            }
        }

        void MainWindow_CloseRequested(object sender, Xwt.CloseRequestedEventArgs args)
        {
            //save settings bcos zi form is closing
            Properties.Settings.Default.Panel1URL = p1.FS.CurrentDirectory;
            Properties.Settings.Default.Panel2URL = p2.FS.CurrentDirectory;
            fcmd.Properties.Settings.Default.LastActivePanel = (ActivePanel == p1) ? (byte)1 : (byte)2;
            Xwt.Application.Exit();
        }

        /// <summary>The entry form's keyboard keypress handler (except commandbar keypresses)</summary>
        void PanelLayout_KeyReleased(object sender, Xwt.KeyEventArgs e)
        {
#if DEBUG
            pluginner.FileListPanel p1 = (PanelLayout.Panel1.Content as pluginner.FileListPanel);
            pluginner.FileListPanel p2 = (PanelLayout.Panel2.Content as pluginner.FileListPanel);
            Console.WriteLine("KEYBOARD DEBUG: " + e.Modifiers.ToString() + "+" + e.Key.ToString() + " was pressed. Panels focuses: " + (ActivePanel == p1) + " | " + (ActivePanel == p2));
#endif
            if (e.Key == Xwt.Key.Return) return;//ENTER presses are handled by other event

            string URL1;
            if (ActivePanel.ListingView.SelectedRow > -1)
                { URL1 = ActivePanel.GetValue(ActivePanel.dfURL); }
            else
                { URL1 = null; }
            pluginner.IFSPlugin FS1 = ActivePanel.FS;

            string URL2;
            if (PassivePanel.ListingView.SelectedRow > -1)
                { URL2 = PassivePanel.GetValue(PassivePanel.dfURL); }
            else
                { URL2 = null; }
            pluginner.IFSPlugin FS2 = PassivePanel.FS;

            switch (e.Key)
            {
                case Xwt.Key.F3: //F3: View. Shift+F3: View as text.
                    if (URL1 == null)
                        return;

                    if (!FS1.FileExists(URL1))
                    {
                        Xwt.MessageDialog.ShowWarning(string.Format(Locale.GetString("FileNotFound"), ActivePanel.GetValue(ActivePanel.dfDisplayName)));
                        return;
                    }

                    VEd V = new VEd();
                    if (e.Modifiers == Xwt.ModifierKeys.None)
                    { V.LoadFile(URL1, FS1, false); V.Show(); }
                    else if(e.Modifiers == Xwt.ModifierKeys.Shift)
                    { V.LoadFile(URL1, FS1, new base_plugins.ve.PlainText(), false); V.Show(); }
                    //todo: handle Ctrl+F3 (Sort by name).
                    return;
                case Xwt.Key.F4: //F4: Edit. Shift+F4: Edit as txt.
                    if (URL1 == null)
                        return;

                    if (!FS1.FileExists(URL1))
                    {
                        Xwt.MessageDialog.ShowWarning(string.Format(Locale.GetString("FileNotFound"), ActivePanel.GetValue(ActivePanel.dfDisplayName)));
                        return;
                    }

                    VEd E = new VEd();
                    if (e.Modifiers == Xwt.ModifierKeys.None)
                    { E.LoadFile(URL1, FS1, true); E.Show(); }
                    else if(e.Modifiers == Xwt.ModifierKeys.Shift)
                    { E.LoadFile(URL1, FS1, new base_plugins.ve.PlainText(), true); E.Show(); }
                    //todo: handle Ctrl+F4 (Sort by extension).
                    return;
                case Xwt.Key.F5: //F5: Copy.
                    if (URL1 == null)
                        return;
                    Cp();
                    //todo: handle Ctrl+F5 (Sort by timestamp).
                    return;
                case Xwt.Key.F6: //F6: Move/Rename.
                    if (URL1 == null)
                        return;
                    Mv();
                    //todo: handle Ctrl+F6 (Sort by size).
                    return;
                case Xwt.Key.F7: //F7: New directory.
                    InputBox ibx = new InputBox(Locale.GetString("NewDirURL"), ActivePanel.FS.CurrentDirectory + Locale.GetString("NewDirTemplate"));
                    if (ibx.ShowDialog()) MkDir(ibx.Result);
                    return;
                case Xwt.Key.F8: //F8: delete
                    if (URL1 == null)
                        return;
                    Rm(ActivePanel.GetValue(ActivePanel.dfURL));
                    //todo: handle Shit+F8 (Move to trash can/recycle bin)
                    return;
                case Xwt.Key.F10: //F10: Exit
                    //todo: ask user, are it really want to close FC?
                    Xwt.Application.Exit();
                    //todo: handle Alt+F10 (directory tree)
                    return;
                //todo: case Xwt.Key.q: quick search
            }
#if DEBUG
            Console.WriteLine("KEYBOARD DEBUG: the key isn't handled");
#endif
        }

        /// <summary>Switches the active panel</summary>
        /// <param name="NewPanel">The new active panel</param>
        private void SwitchPanel(pluginner.FileListPanel NewPanel)
        {
            PassivePanel = ActivePanel;
            ActivePanel = NewPanel;
            CommandBox.PlaceholderText = ActivePanel.FS.CurrentDirectory + ">";
#if DEBUG
            string PanelName = (NewPanel == p1) ? "LEFT" : "RIGHT";
            Console.WriteLine("FOCUS DEBUG: The " + PanelName + " panel (" + NewPanel.FS.CurrentDirectory + ") got focus");
#endif
        }

        /// <summary>Converts size display policy (as string) to FLP.SizeDisplayPolicy</summary>
        private pluginner.FileListPanel.SizeDisplayPolicy ConvertSDP(char SizeDisplayPolicy)
        {
            switch (SizeDisplayPolicy.ToString())
            {
                case "0":
                    return pluginner.FileListPanel.SizeDisplayPolicy.DontShorten;
                case "1":
                    return pluginner.FileListPanel.SizeDisplayPolicy.OneNumeral;
                case "2":
                    return pluginner.FileListPanel.SizeDisplayPolicy.TwoNumeral;
                default:
                    return pluginner.FileListPanel.SizeDisplayPolicy.OneNumeral;
            }
        }
    }
}
