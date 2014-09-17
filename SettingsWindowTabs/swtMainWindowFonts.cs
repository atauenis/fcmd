/* The File Commander Settings window tabs
 * Tab "Main window layout"
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2014, Zhigunov Andrew (breakneck11@gmail.com)
 * Contributors should place own signs here.
 */
using System;
using System.Linq;
using Xwt;
using Xwt.Drawing;
using fcmd.Properties;

namespace fcmd.SettingsWindowTabs
{
	public class swtMainWindowFonts : ISettingsWindowTab
	{

		VBox box = new VBox();
		VBox fraMainBox = new VBox();
		Frame fraMain = new Frame();
		Label lblFileNameFont = new Label() { Text = fcmd.Localizator.GetString("swtFileNamesFont") };
		ComboBox cmbRegisteredFonts = new ComboBox();

		public swtMainWindowFonts () {
			box.PackStart(fraMain);
			fraMain.Content = fraMainBox;
			fraMainBox.PackStart(lblFileNameFont);
			fraMainBox.PackStart(cmbRegisteredFonts);
			foreach (string ffamily in Font.AvailableFontFamilies.OrderBy(x => x)) {
				cmbRegisteredFonts.Items.Add(ffamily);
			}
			cmbRegisteredFonts.SelectionChanged += cmbRegisteredFonts_SelectionChanged;
			cmbRegisteredFonts.SelectedItem = String.IsNullOrWhiteSpace(Settings.Default.UserFileListFontFamily) ?
				Font.SystemFont.Family : Font.FromName(Settings.Default.UserFileListFontFamily).Family;
		}

		void cmbRegisteredFonts_SelectionChanged(object sender, EventArgs e) {
			cmbRegisteredFonts.Font = Font.FromName(cmbRegisteredFonts.SelectedItem.ToString());
		}

#region ISettingsWindowTab implementation

		public bool SaveSettings () {
			try {
				Settings.Default.UserFileListFontFamily = cmbRegisteredFonts.SelectedItem.ToString();
				return true;
			} catch (Exception ex) {
				MessageDialog.ShowError(ex.Message);
				return false;
			}
		}

		public Widget Content {
			get {
				return box;
			}
		}

#endregion ISettingsWindowTab implementation
	}
}

