/* The File Commander
 * File choosion dialog (temporary)
 * (C) The File Commander Team - https://github.com/atauenis/fcmd
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Contributors should place own signs here.
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;

namespace fcmd
{
    public partial class FileChooser : Form
    {
        Localizator locale = new Localizator();
        public FileChooser(pluginner.IFSPlugin FS)
        {
            InitializeComponent();
            listPanel1.FSProvider = FS;
            LoadDir(FS.CurrentDirectory, listPanel1);
            listPanel1.list.View = View.Details;

            listPanel1.list.Columns.Add("Name", locale.GetString("FName"));
            listPanel1.list.Columns.Add("Size", locale.GetString("FSize"));
            listPanel1.list.Columns.Add("Date", locale.GetString("FDate"));
            listPanel1.list.DoubleClick += cmdOk_Click;//(o,ea) {};
        }

        private void FileChooser_Load(object sender, EventArgs e)
        {

        }

        /// <summary>Defines, what file is selected</summary>
        public string SelectedFile;

        private void cmdOk_Click(object sender, EventArgs e)
        {
            if (listPanel1.list.SelectedItems.Count > 0)
                SelectedFile = listPanel1.list.SelectedItems[0].Tag.ToString();
            else
                SelectedFile = null;
        }

        public void LoadDir(string url, ListPanel lp)
        {
            lp.lblPath.Text = url;
            int Status = 0;
            Thread LsThread = new Thread(delegate() { DoLs(url, lp, ref Status); });
            FileProcessDialog fpd = new FileProcessDialog();
            fpd.Y = lp.Top * 1.5;
            fpd.X = lp.Left * 1.5;
            string FPDtext = String.Format(locale.GetString("DoingListdir"), "\n" + url, "");
            FPDtext = FPDtext.Replace("{1}", "");
            fpd.lblStatus.Text = FPDtext;

            fpd.Show();
            LsThread.Start();

            do { Application.DoEvents(); fpd.pbrProgress.Fraction = Status; }
            while (LsThread.ThreadState == System.Threading.ThreadState.Running);
            fpd.Hide();
        }

        /// <summary>
        /// Background directory lister
        /// </summary>
        void DoLs(string URL, ListPanel lp, ref int StatusFeedback)
        {
            CheckForIllegalCrossThreadCalls = false; //HACK: заменить на долбанные делегации и прочую нетовскую муть
            lp.list.UseWaitCursor = true;
            lp.list.Items.Clear();
            lp.list.BeginUpdate();

            //load the directory
            pluginner.IFSPlugin fsp = lp.FSProvider;
            fsp.CurrentDirectory = URL;

            //making the statistic info
            int FileWeight = 0;
            checked { FileWeight = 100 / fsp.DirectoryContent.Count; }

            for (int i = 0; i < fsp.DirectoryContent.Count; i++)
            {
                pluginner.DirItem di = fsp.DirectoryContent[i];

                //parsing all files, that given from the FS provider
                StatusFeedback += FileWeight / 100;
                if (di.Hidden == false || fcmd.Properties.Settings.Default.ShowHidedFiles == true)
                {
                    AddItem(lp, di.TextToShow, fsp.GetMetadata(di.Path), di.Size, di.Date); //todo: add the icon of the file
                }
            }
            lp.list.EndUpdate();
            lp.list.UseWaitCursor = false;
        }

        /// <summary>Add an item into the ListPanel's ListView</summary>
        void AddItem(ListPanel lp, string ItemDisplayText, pluginner.FSEntryMetadata ItemMetadata, long ItemSize, DateTime ItemDate)
        {
            //creating a new listpanel's listview item
            ListViewItem NewItem = new ListViewItem(ItemDisplayText);
            NewItem.Tag = ItemMetadata; //each list item is "tagged" with the file's metadata
            NewItem.SubItems.Add(KiloMegaGigabyteConvert(ItemSize));
            NewItem.SubItems.Add(ItemDate.ToShortDateString());
            AddItem(lp, NewItem);
            //todo: add file icons
        }

        /// <summary>
        /// Adds a new item <paramref name="NewItem"/> into the ListPanel <paramref name="lp"/>
        /// </summary>
        /// <param name="lp"></param>
        /// <param name="NewItem"></param>
        private void AddItem(ListPanel lp, ListViewItem NewItem)
        {
            CheckForIllegalCrossThreadCalls = false; //HACK: заменить на долбанные делегации и прочую нетовскую муть
            lp.list.Items.Add(NewItem);
        }

        /// <summary>
        /// Converts the file size (in bytes) to human-readable string
        /// </summary>
        private string KiloMegaGigabyteConvert(long Input)
        {
            const int Kibi = 1024; //to be used when a maths-knowledged contributor changed these "magic numbers" to equations of 1024
            //необходимо заменить магические числа на формулы с применением константы 1024 (Kibi)

            if (Input > 1099511627776) return (Input / 1099511627776).ToString() + " TB"; //terabyte
            if (Input > 1073741824) return (Input / 1073741824).ToString() + " GB"; //gigabyte
            if (Input > 1048576) return (Input / 1048576).ToString() + " MB"; //megabyte
            if (Input > 1024) return (Input / Kibi).ToString() + " KB"; //kilobyte

            return Input.ToString() + " B"; //byte (if Input less than 1024)
        }


    }
}
