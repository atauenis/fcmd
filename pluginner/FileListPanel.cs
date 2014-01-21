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

        private SizeDisplayPolicy CurShortenKB, CurShortenMB, CurShortenGB;

        public FileListPanel()
        {
            FLStore = new Xwt.ListStore(dfURL, dfDisplayName,dfSize,dfMetadata,dfChanged);
            ListingView.DataSource = FLStore;
            ListingView.ButtonPressed += new EventHandler<Xwt.ButtonEventArgs>(ListingView_ButtonPressed);
            ListingView.KeyReleased += new EventHandler<Xwt.KeyEventArgs>(ListingView_KeyReleased);
            ListingView.GotFocus += (o, ea) => { this.OnGotFocus(ea); };
            ListingView.SelectionChanged += (o, ea) => { this.OnGotFocus(ea); }; //hack for incomplete Xwt.Gtk ListView (19/01/2014)
            
            UrlBox.KeyReleased += new EventHandler<Xwt.KeyEventArgs>(UrlBox_KeyReleased);

            this.PackStart(UrlBox,false, true);
            this.PackStart(ListingView, true, true);
            this.PackStart(StatusBar, false,true);

            UrlBox.ShowFrame = false;
            UrlBox.Text = @"file://C:\NC";
            UrlBox.GotFocus += (o, ea) => { this.OnGotFocus(ea); };
            ListingView.BorderVisible = false;
            StatusBar.Text = "0 bytes";
        }

        void UrlBox_KeyReleased(object sender, Xwt.KeyEventArgs e)
        {
            if (e.Key == Xwt.Key.Return)
            {
                LoadDir(UrlBox.Text);
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

                    LoadDir(url);
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
        /// Load the directory into the panel and set view options
        /// </summary>
        /// <param name="URL">Full path of the directory</param>
        /// <param name="ShortenKB">How kilobyte sizes should be humanized</param>
        /// <param name="ShortenMB">How megabyte sizes should be humanized</param>
        /// <param name="ShortenGB">How gigabyte sizes should be humanized</param> //плохой перевод? "так nбайтные размеры должны очеловечиваться"
        public void LoadDir(string URL, SizeDisplayPolicy ShortenKB, SizeDisplayPolicy ShortenMB, SizeDisplayPolicy ShortenGB)
        {
            CurShortenKB = ShortenKB; CurShortenMB = ShortenMB; CurShortenGB = ShortenGB;

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
                    FLStore.SetValue<string>(FLStore.RowCount - 1, dfSize, KiloMegaGigabyteConvert(di.Size, ShortenKB, ShortenMB, ShortenGB));
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
            LoadDir(FS.CurrentDirectory);
        }

        /// <summary>
        /// Load the directory into the panel
        /// </summary>
        /// <param name="URL">Full path of the directory</param>
        public void LoadDir(string URL)
        {
            LoadDir(URL, CurShortenKB, CurShortenMB, CurShortenGB);
        }

        /// <summary>Converts the file size (in bytes) to human-readable string</summary>
        /// <param name="Input">The input value</param>
        /// <param name="ShortestNonhumanity">The miminal file size that should be shortened</param>
        /// <returns>Human-readable string (xxx yB)</returns>
        private string KiloMegaGigabyteConvert(long Input, SizeDisplayPolicy ShortenKB, SizeDisplayPolicy ShortenMB, SizeDisplayPolicy ShortenGB)
        {
            double ShortenedSize; //here will be writed the decimal value of the hum. readable size

            //TeraByte (will be shortened everywhen)
            if (Input > 1099511627776) return (Input / 1099511627776).ToString() + " TB";

            //GigaByte
            if (Input > 1073741824)
            {
                ShortenedSize = Input / 1073741824;
                switch (ShortenGB)
                {
                    case SizeDisplayPolicy.OneNumeral:
                        return string.Format("{0:0.#} GB", ShortenedSize);
                    case SizeDisplayPolicy.TwoNumeral:
                        return string.Format("{0:0.##} GB", ShortenedSize);
                }
            }

            //MegaByte
            if (Input > 1048576)
            {
                ShortenedSize = Input / 1048576;
                switch (ShortenMB)
                {
                    case SizeDisplayPolicy.OneNumeral:
                        return string.Format("{0:0.#} MB", ShortenedSize);
                    case SizeDisplayPolicy.TwoNumeral:
                        return string.Format("{0:0.##} MB", ShortenedSize);
                }
            }

            //KiloByte
            if (Input > 1024)
            {
                ShortenedSize = Input / 1024;
                switch (ShortenKB)
                {
                    case SizeDisplayPolicy.OneNumeral:
                        return string.Format("{0:0.#} KB", ShortenedSize);
                    case SizeDisplayPolicy.TwoNumeral:
                        return string.Format("{0:0.##} KB", ShortenedSize);
                }
            }

            return Input.ToString() + " B"; //if Input is less than 1k or shortening is disallowed
        }

        /// <summary>Defines the size shortening policy</summary>
        public enum SizeDisplayPolicy
        {
            DontShorten=0, OneNumeral=1, TwoNumeral=2
            //2048 B, 2 KB, 2.0 KB
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
