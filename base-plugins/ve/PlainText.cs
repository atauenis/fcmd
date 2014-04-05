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
		Xwt.Menu mnuFormat = new Xwt.Menu();
		Xwt.Table Layout = new Xwt.Table() { DefaultRowSpacing = 0 };
		Xwt.RichTextView RTV = new Xwt.RichTextView();
		Xwt.ScrollView ScrollBox;
		Xwt.Label lblFileName = new Xwt.Label("file name");
		Xwt.MenuButton mbMode = new Xwt.MenuButton("Text") { Sensitive = false, Type = Xwt.ButtonType.Normal, Style = Xwt.ButtonStyle.Flat };
		Xwt.MenuButton mbCodepage = new Xwt.MenuButton("codepage") { Type = Xwt.ButtonType.DropDown, Style = Xwt.ButtonStyle.Flat };
		
		int Codepage = Encoding.Default.CodePage;
		byte[] fileContent;
		string Txt = "";

		public PlainText() //constructor
		{
			ScrollBox = new Xwt.ScrollView(RTV);
			ScrollBox.HeightRequest = 350;//todo: read from settings
			Layout.Add(ScrollBox, 0, 1, 1, 3, true, true);

			Layout.Add(lblFileName, 0, 0);
			Layout.Add(mbMode, 1, 0);
			Layout.Add(mbCodepage, 2, 0);

			foreach (EncodingInfo cp in Encoding.GetEncodings())
			{
				Xwt.MenuItem mi = new Xwt.MenuItem();
				mi.Tag = cp.CodePage;
				mi.Label = "CP" + cp.CodePage + " - " + cp.DisplayName;
				mi.Clicked += new EventHandler(Codepage_Clicked);
				mnuFormat.Items.Add(mi);
			}
			mbCodepage.Menu = mnuFormat;
		}

		void Codepage_Clicked(object sender, EventArgs e)
		{
			Xwt.MenuItem MI = (Xwt.MenuItem)sender;
			ChangeCodepage(Convert.ToInt32(MI.Tag));
		}

		void ChangeCodepage(int CP)
		{
			Codepage = Convert.ToInt32(CP);
			Txt = Encoding.GetEncoding(Codepage).GetString(fileContent ?? new byte[] { 0, 0 });
			RTV.LoadText(Txt, new Xwt.Formats.PlainTextFormat());
			mbCodepage.Label = Encoding.GetEncoding(Codepage).EncodingName;
		}

		public int[] FlexibleAPIversion {
			get{
				int[] fapiver = {0,1,0, 0,1,0};
				return fapiver;
			}
		}

		public object FlexibleAPIcall(string call, params object[] arguments)
		{
			return null;
		}

		public void OpenFile(string url, pluginner.IFSPlugin fsplugin)
		{
			lblFileName.Text = url;
			fileContent = fsplugin.GetFileContent(url);
			ChangeCodepage(Codepage);
		}

		public void SaveFile(bool SaveAs = false) { }

		public void ExecuteCommand(string Command, string[] Arguments)
		{
			switch(Command){
				case "codepage":
					ChangeCodepage(Convert.ToInt32(Arguments[1]));
					break;
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
			get { return Layout; }
		}

		public bool ReadOnly { get { return false; } set { } } //todo

		public bool CanEdit { get { return false; } }//todo (needs to edit xwt rtv or te)

		public Xwt.Menu FormatMenu { get { return mnuFormat;} }

		public bool ShowToolbar {
			set
			{
				lblFileName.Visible = value;
				mbMode.Visible = value;
				mbCodepage.Visible = value;
			}
		}
	}
}
