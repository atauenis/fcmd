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

        Xwt.TextEntry CommandBox = new Xwt.TextEntry();
        Xwt.HBox KeyBoardHelp = new Xwt.HBox();
        Xwt.Button[] KeybHelpButtons = new Xwt.Button[11]; //одна лишняя, которая [0]


        public MainWindow()
        {
            this.Title = "File Commander";
            PanelLayout.Panel1.Content = new pluginner.FileListPanel(); //Левая, правая где сторона? Улица, улица, ты, брат, пьяна! HNY
            PanelLayout.Panel2.Content = new pluginner.FileListPanel();
            PanelLayout.KeyReleased += new EventHandler<Xwt.KeyEventArgs>(PanelLayout_KeyReleased);

            Layout.PackStart(PanelLayout, true, Xwt.WidgetPlacement.Fill, Xwt.WidgetPlacement.Fill, -12, -12, -12,6);
            Layout.PackStart(CommandBox,true,Xwt.WidgetPlacement.End,Xwt.WidgetPlacement.Fill,-12,-12,-12);
            Layout.PackStart(KeyBoardHelp, false,Xwt.WidgetPlacement.End,Xwt.WidgetPlacement.Start,-12,-6,-12,-12);
            this.Content = Layout;
            
            //build panels
            pluginner.FileListPanel p = (PanelLayout.Panel1.Content as pluginner.FileListPanel);
            p.FS = new base_plugins.fs.localFileSystem();
            p.ListingView.Columns.Add("Вымя", p.dfDisplayName);
            p.ListingView.Columns.Add("Объём", p.dfSize);
            p.LoadDir("file://C:\\", 1024);

            //build keyboard help bar
            for (int i = 1; i < 11; i++)
            {
                KeybHelpButtons[i] = new Xwt.Button(Locale.GetString("FCF" + i));
                KeybHelpButtons[i].Style = Xwt.ButtonStyle.Flat;
                KeybHelpButtons[i].CanGetFocus = false;
                KeyBoardHelp.PackStart(KeybHelpButtons[i]);
            }   
        }

        /// <summary>The entry form's keyboard keypress handler (except commandbar keypresses)</summary>
        void PanelLayout_KeyReleased(object sender, Xwt.KeyEventArgs e)
        {
            //throw new NotImplementedException();
        }
    }
}
