/* The File Commander - plugin API
 * The file list widget
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013-14, Alexander Tauenis (atauenis@yandex.ru)
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

        /// <summary>User navigates into another directory</summary>
        public event TypedEvent<string> Navigate;
        /// <summary>User tried to open the highlighted file</summary>
        public event TypedEvent<string> OpenFile;



        public FileListPanel()
        {
            FLStore = new Xwt.ListStore(dfURL, dfDisplayName,dfSize,dfMetadata,dfChanged);
            ListingView.DataSource = FLStore;
            ListingView.ButtonPressed += new EventHandler<Xwt.ButtonEventArgs>(ListingView_ButtonPressed);
            ListingView.KeyReleased += new EventHandler<Xwt.KeyEventArgs>(ListingView_KeyReleased);
            UrlBox.KeyReleased += new EventHandler<Xwt.KeyEventArgs>(UrlBox_KeyReleased);

            this.PackStart(UrlBox,false, true);
            this.PackStart(ListingView, true, true);
            this.PackStart(StatusBar, false,true);

            UrlBox.ShowFrame = false;
            UrlBox.Text = @"file://C:\NC";
            ListingView.BorderVisible = false;
            StatusBar.Text = "0 bytes";
        }

        void UrlBox_KeyReleased(object sender, Xwt.KeyEventArgs e)
        {
            if (e.Key == Xwt.Key.Return)
            {
                LoadDir(UrlBox.Text,0);
            }
        }

        void ListingView_KeyReleased(object sender, Xwt.KeyEventArgs e)
        {
            if (e.Key == Xwt.Key.Return && ListingView.SelectedRow > -1)
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

                if (FS.DirectoryExists(url))
                {//it's directory
                    if (Navigate != null) Navigate(url); //raise event
                    else Console.WriteLine("WARNING: the event FLP.Navigate was not handled by the host");

                    LoadDir(url, 0);
                    return;
                }
                else
                {//it's file
                    if (OpenFile != null) OpenFile(url); //raise event
                    else Console.WriteLine("WARNING: the event FLP.OpenFile was not handled by the host");
                }

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
            //TODO: rewrite size humanization code
            //тодо: сделать аналог настройки TC "Конфигурация-Табуляторы-Размеры в панелях"
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
                if(di.TextToShow == "..")//parent dir
                    FLStore.SetValue<string>(FLStore.RowCount - 1, dfSize, "<↑ UP>");
                else if (di.IsDirectory){//dir
                    FLStore.SetValue<string>(FLStore.RowCount - 1, dfSize, "<DIR>");
                    FLStore.SetValue<DateTime>(FLStore.RowCount - 1, dfChanged, di.Date);
                }
                else{//file
                    FLStore.SetValue<string>(FLStore.RowCount - 1, dfSize, KiloMegaGigabyteConvert(di.Size, MinHumanize));//todo: сократить/очеловечить до КБ/МБ/ГБ.
                    FLStore.SetValue<DateTime>(FLStore.RowCount - 1, dfChanged, di.Date);
                }
            }
            ListingView.SelectRow(0);
        }

        /// <summary>
        /// Reloads the current directory
        /// </summary>
        public void LoadDir()
        {
            LoadDir(FS.CurrentDirectory, 0);
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
        
        /// <summary>
        /// Gets the selected row's value from the <paramref name="Datafield"/>
        /// </summary>
        /// <typeparam name="T">The type of the datafield</typeparam>
        /// <param name="Datafield">The datafield</param>
        /// <returns>The value</returns>
        public T GetValue<T>(Xwt.IDataField<T> Datafield){
            return FLStore.GetValue<T>(ListingView.SelectedRow, Datafield);
        }
    }
}
