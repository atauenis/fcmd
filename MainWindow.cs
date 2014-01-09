/* The File Commander main window
 * The main file manager window
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
    class MainWindow : Xwt.Window
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
            PanelLayout.Panel1.Content = new pluginner.FileListPanel(); //Левая, правая где сторона? Улица, улица, ты, брат, пьяна!
            PanelLayout.Panel2.Content = new pluginner.FileListPanel();
            PanelLayout.KeyReleased += new EventHandler<Xwt.KeyEventArgs>(PanelLayout_KeyReleased);

            Layout.PackStart(PanelLayout, true, Xwt.WidgetPlacement.Fill, Xwt.WidgetPlacement.Fill, -12, -12, -12,12);
            Layout.PackStart(CommandBox,false,Xwt.WidgetPlacement.End,Xwt.WidgetPlacement.Fill,-12,-12,-12);
            Layout.PackStart(KeyBoardHelp, false,Xwt.WidgetPlacement.End,Xwt.WidgetPlacement.Start,-12,-6,-12,-12);
            
            this.Content = Layout;
            
            //build panels
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

            p1.GotFocus += (o, ea) => { Console.WriteLine("DEBUG: LEFT panel got focus"); SwitchPanel(p1); };
            p2.GotFocus += (o, ea) => {Console.WriteLine("DEBUG: RIGHT panel got focus"); SwitchPanel(p2); };

            //build keyboard help bar
            for (int i = 1; i < 11; i++)
            {
                KeybHelpButtons[i] = new Xwt.Button(Locale.GetString("FCF" + i));
                KeybHelpButtons[i].Style = Xwt.ButtonStyle.Flat;
                KeybHelpButtons[i].CanGetFocus = false;
                KeyBoardHelp.PackStart(KeybHelpButtons[i]);
            }

            //apply user's settings
            //default directories
            if (Properties.Settings.Default.Panel1URL.Length != 0)
                p1.LoadDir(Properties.Settings.Default.Panel1URL, 0);
            else
                p1.LoadDir("file://"+System.Windows.Forms.Application.StartupPath,0);

            if (Properties.Settings.Default.Panel2URL.Length != 0)
                p2.LoadDir(Properties.Settings.Default.Panel2URL, 0);
            else
                p2.LoadDir("file://"+System.Windows.Forms.Application.StartupPath,0);

            //default panel
            switch (fcmd.Properties.Settings.Default.LastActivePanel)
            {
                case 1:
                    p1.ListingView.SetFocus();
                    break;
                case 2:
                    p2.ListingView.SetFocus();
                    break;
                default:
                    p1.ListingView.SetFocus();
                    break;
            }

        }

        void MainWindow_CloseRequested(object sender, Xwt.CloseRequestedEventArgs args)
        {
            Properties.Settings.Default.Panel1URL = p1.FS.CurrentDirectory;
            Properties.Settings.Default.Panel2URL = p2.FS.CurrentDirectory;
            fcmd.Properties.Settings.Default.LastActivePanel = (ActivePanel == p1) ? (byte)1 : (byte)2;
            Xwt.Application.Exit();
        }

        /// <summary>The entry form's keyboard keypress handler (except commandbar keypresses)</summary>
        void PanelLayout_KeyReleased(object sender, Xwt.KeyEventArgs e)
        {
            pluginner.FileListPanel p1 = (PanelLayout.Panel1.Content as pluginner.FileListPanel);
            pluginner.FileListPanel p2 = (PanelLayout.Panel2.Content as pluginner.FileListPanel);

            //panel focus (active/passive detection) debug
            //Console.WriteLine((ActivePanel == p1) + " | " + (ActivePanel == p2));
        }

        /// <summary>Switches the active panel</summary>
        /// <param name="NewPanel">The new active panel</param>
        private void SwitchPanel(pluginner.FileListPanel NewPanel)
        {
            PassivePanel = ActivePanel;
            ActivePanel = NewPanel;
            CommandBox.PlaceholderText = ActivePanel.FS.CurrentDirectory + ">";
        }
    }
}
