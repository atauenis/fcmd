/* The File Commander - plugin API
 * The file list widget
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace pluginner
{
    /// <summary>Filelist panel</summary>
    public class FileListPanel : Xwt.VBox
    {
        public Xwt.ListStore FLStore; //file list storage
        public Xwt.DataField<string> dfURL = new Xwt.DataField<string>();
        public Xwt.DataField<string> dfDisplayName = new Xwt.DataField<string>();
        public Xwt.DataField<string> dfSize = new Xwt.DataField<string>();
        public Xwt.DataField<DateTime> dfChanged = new Xwt.DataField<DateTime>();
        public Xwt.DataField<pluginner.FSEntryMetadata> dfMetadata = new Xwt.DataField<FSEntryMetadata>();
        
        public Xwt.TextEntry UrlBox = new Xwt.TextEntry();
        public Xwt.ListView ListingView = new Xwt.ListView();
        public Xwt.Label StatusBar = new Xwt.Label();
        public pluginner.IFSPlugin FS;
        public event TypedEvent<string> Navigate;



        public FileListPanel()
        {
            FLStore = new Xwt.ListStore(dfURL, dfDisplayName,dfSize,dfMetadata,dfChanged);
            ListingView.DataSource = FLStore;
            ListingView.ButtonPressed += new EventHandler<Xwt.ButtonEventArgs>(ListingView_ButtonPressed);
            ListingView.KeyReleased += new EventHandler<Xwt.KeyEventArgs>(ListingView_KeyReleased);

            this.PackStart(UrlBox, true, Xwt.WidgetPlacement.Start, Xwt.WidgetPlacement.Fill);
            this.PackStart(ListingView, true, Xwt.WidgetPlacement.Fill, Xwt.WidgetPlacement.Fill);
            this.PackStart(StatusBar, true, Xwt.WidgetPlacement.End, Xwt.WidgetPlacement.Fill);

            UrlBox.ShowFrame = false;
            UrlBox.Text = "C:\\NC";
            StatusBar.Text = "0 bytes";
        }

        void ListingView_KeyReleased(object sender, Xwt.KeyEventArgs e)
        {
            if (e.Key == Xwt.Key.Return)
            {
                NavigateTo(FLStore.GetValue<string>(ListingView.SelectedRow, dfURL));
            }
        }

        void ListingView_ButtonPressed(object sender, Xwt.ButtonEventArgs e)
        {
            if (e.MultiplePress == 2)//double click
                NavigateTo(FLStore.GetValue<string>(ListingView.SelectedRow, dfURL));
        }

        /// <summary>
        /// Open the FS item at <paramref name="url"/> (if it's file, load; if it's directory, go to)
        /// </summary>
        private void NavigateTo(string url)
        {
            try
            {
                if(Navigate!= null) Navigate(url); //raise event

                if (FS.DirectoryExists(url)){
                    LoadDir(url, 0);
                }
                //else the host should handle this
            }
            catch (Exception ex)
            {
                Xwt.MessageDialog.ShowError(ex.Message);
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        /// <summary>
        /// Load the directory into the panel
        /// </summary>
        /// <param name="URL">Full path of the directory</param>
        /// <param name="MinHumanize">Minimal file size, that should be humanized</param>
        public void LoadDir(string URL, int MinHumanize)
        {
            if (FS == null) throw new InvalidOperationException("No filesystem is binded to this FileListPanel");
            FS.CurrentDirectory = URL;
            
            FLStore.Clear();
            UrlBox.Text = URL;
            foreach (DirItem di in FS.DirectoryContent)
            {
                FLStore.AddRow();
                FLStore.SetValue<string>(FLStore.RowCount-1, dfURL, di.Path);
                FLStore.SetValue<pluginner.FSEntryMetadata>(FLStore.RowCount - 1, dfMetadata, FS.GetMetadata(di.Path));
                FLStore.SetValue<string>(FLStore.RowCount - 1, dfDisplayName, di.TextToShow);
                if(di.IsDirectory)
                    FLStore.SetValue<string>(FLStore.RowCount-1, dfSize, "<DIR>");
                else
                    FLStore.SetValue<string>(FLStore.RowCount-1, dfSize, KiloMegaGigabyteConvert(di.Size,MinHumanize));//todo: сократить/очеловечить до КБ/МБ/ГБ.
                FLStore.SetValue<DateTime>(FLStore.RowCount-1, dfChanged, di.Date);
            }
        }

        /// <summary>Converts the file size (in bytes) to human-readable string</summary>
        /// <param name="Input">The input value</param>
        /// <param name="ShortestNonhumanity">The miminal file size that should be shortened</param>
        /// <returns>Human-readable string (xxx yB)</returns>
        private string KiloMegaGigabyteConvert(long Input, int ShortestNonhumanity)
        {
            if (Input < ShortestNonhumanity) return Input.ToString() + " B"; //if Input shouldn't be shortened, return it as is

            if (Input > 1099511627776) return (Input / 1099511627776).ToString() + " TB";
            if (Input > 1073741824) return (Input / 1073741824).ToString() + " GB";
            if (Input > 1048576) return (Input / 1048576).ToString() + " MB";
            if (Input > 1024) return (Input / 1024).ToString() + " KB";

            return Input.ToString() + " B"; //if Input is less than 1024
        }
    }
}
