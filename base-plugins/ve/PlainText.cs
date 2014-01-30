using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace fcmd.base_plugins.ve
{
    class PlainText : pluginner.IVEPlugin
    {
        #region Metadata
        public string Name { get { return new Localizator().GetString("VEptxtVer"); } }
        public string Version { get { return "1.0"; } }
        public string Author { get { return "Alexander Tauenis"; } }
        #endregion

        Xwt.RichTextView RTV = new Xwt.RichTextView();
        Xwt.ScrollView ScrollBox;
        Xwt.Menu mnuFormat = new Xwt.Menu();
        int Codepage = Encoding.UTF8.CodePage;
        byte[] fileContent;
        string Txt = "";

        public PlainText() //constructor
        {
            ScrollBox = new Xwt.ScrollView(RTV);
            ScrollBox.HeightRequest = 350;//todo: read from settings
        }

        public void OpenFile(string url, pluginner.IFSPlugin fsplugin)
        {
            fileContent = fsplugin.GetFileContent(url);
			Txt = Encoding.GetEncoding(Codepage).GetString(fileContent ?? new byte[]{0,0}); //фаллбак взят с потолка, чем бы дитя не тешилось, лишь бы не...ругалось

            RTV.LoadText(Txt, new Xwt.Formats.PlainTextFormat());
        }

        public void SaveFile(bool SaveAs = false) { }

        public void ExecuteCommand(string Command, string[] Arguments)
        {
            switch(Command){
                case "findreplace": break;
                case "cut": break;
                case "copy": break;
                case "paste": break;
                case "select": break;
                case "print": break;
                case "pagesetup": break;
                default: Xwt.MessageDialog.ShowWarning("Unknown command:",Command); break;
            }
        }

        public Xwt.Widget Body {
            get { return ScrollBox; }
        }

        public bool ReadOnly { get { return false; } set { } } //todo

        public bool CanEdit { get { return false; } }//todo (needs to edit xwt rtv or te)

        public Xwt.Menu FormatMenu { get { return mnuFormat;} }
    }
}
