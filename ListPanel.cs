﻿/* The File Commander
 * Элемент управления ListPanel (улучшенный аналог ListView)
 * (C) 2013, Alexander Tauenis (atauenis@yandex.ru)
 * Копирование кода разрешается только с письменного согласия
 * разработчика (А.Т.).
 */
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;

namespace fcmd
{
    public partial class ListPanel : System.Windows.Forms.UserControl
    {
		//Типы
		public class ItemDescription{//тип для пункта в списке
			public List<string> Text = new List<string>();
			public string Value;
			public bool Selected = false;
			public short Selection = 0;
			public Image Icon;
		}
		public struct CollumnOptions{//параметры столбца
			/// <summary>
			/// Заголовок столбца ///
			/// The collumn's caption.
			/// </summary>
			public string Caption;

			/// <summary>
			/// Метка слобца (для определения что это такое) ///
			/// The collumn's tag.
			/// </summary>
			public string Tag;

			/// <summary>
			/// Ширина и высота столбца ///
			/// The collumn's size.
			/// </summary>
			public System.Drawing.Size Size;
		}

		//Внутренние переменные
		pluginner.IFSPlugin FsPlugin;
        TextBox txtPath = new TextBox();

		//Подпрограммы
        public ListPanel(){//Ну, за инициализацию!
			InitializeComponent();
            list.GotFocus += (sender, e) => OnGotFocus(e);
			list.LostFocus += (sender, e) => OnLostFocus(e);
        }

        //События
		public new event StringEvent DoubleClick; //todo: old hack for mono, needs to be confirmed and possibly removed
        public new event StringEvent OnURLBoxNavigate; //event for urlbox-initiated navigation

        private void ListPanel_Load(object sender, EventArgs e){//Ну, за загрузку!
            Repaint();
			this.MouseClick += (snd, ea) => this.Focus();
        }

        //Свойства
		/// <summary>
		/// Gets or sets the file system driver.
		/// </summary>
		/// <value>
		/// The file system driver.
		/// </value>
		public pluginner.IFSPlugin FSProvider{
			get{return FsPlugin;}
			set{FsPlugin = value;}
		}

        private void list_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                pluginner.FSEntryMetadata Finfo = list.SelectedItems[0].Tag as pluginner.FSEntryMetadata;
                string Text = fcmd.Properties.Settings.Default.InfoBarContent;

                Text = Text.Replace("{Name}", Finfo.Name);
                Text = Text.Replace("{Size}", Finfo.Lenght.ToString()); //todo: auto-translation between byte-kbyte-mbyte...
                Text = Text.Replace("{ShortDate}", Finfo.LastWriteTimeUTC.ToShortDateString());
                Text = Text.Replace("{LongDate}", Finfo.LastWriteTimeUTC.ToLongDateString());
                //undone

                lblStatus.Text = Text;
            }
            catch (System.ArgumentOutOfRangeException aoore)
            {
                //всё нормально, пользователь не выбрал никакого пункта
                lblStatus.Text = " ";//пустая строка чтобы не было неприятностей с работой AutoSize
            }
            catch (Exception ex) { lblStatus.Text = ex.Message; }
        }

        /// <summary>
        /// Repaint the ListPanel
        /// </summary>
        public void Repaint()
        {
            lblStatus.Visible = fcmd.Properties.Settings.Default.ShowFileInfo;
            foreach (System.IO.DriveInfo di in System.IO.DriveInfo.GetDrives()){
                string d = di.Name;
                ToolStripButton tsb = new ToolStripButton(d,null,(object s, EventArgs ea) => {ToolStripButton tsbn = (ToolStripButton)s; LP_goToDir("file://" + tsbn.Text);});
                
                //Painting drive icons
                switch(di.DriveType){
                    case System.IO.DriveType.Fixed:
                        tsb.Image = fcmd.Properties.Resources.DiskHDD;
                        break;
                    case System.IO.DriveType.CDRom:
                        tsb.Image = fcmd.Properties.Resources.DiskCD;
                        break;
                    case System.IO.DriveType.Removable:
                        tsb.Image = fcmd.Properties.Resources.DiskRemovable;
                        break;
                    case System.IO.DriveType.Network:
                        tsb.Image = fcmd.Properties.Resources.DiskNetwork;
                        break;
                    case System.IO.DriveType.Ram:
                        tsb.Image = fcmd.Properties.Resources.DiskRAM;
                        break;
                    case System.IO.DriveType.Unknown:
                        tsb.Image = fcmd.Properties.Resources.DiskUnknown;
                        break;
                }

                //OS-specific icons (mono mistakes)
                if (d.StartsWith("A:")) tsb.Image = fcmd.Properties.Resources.DiskFDD;
                if (d.StartsWith("/dev")) tsb.Image = fcmd.Properties.Resources.DiskDevfs;
                if (d.StartsWith("/proc")) tsb.Image = fcmd.Properties.Resources.DiskProcfs;
                if (d == "/") tsb.Image = fcmd.Properties.Resources.DiskRootFs;

                tsDisks.Items.Add(tsb);
            }
        }

        /// <summary>
        /// Load the directory <paramref name="url"/>. To be used only inside listpanel.cs
        /// </summary>
        /// <param name="url"></param>
        private void LP_goToDir(string url)
        {
            //todo: переписать: убрать самодеятельность, обращаться к хозяину листпанели
            lblPath.Text = url;
            try
            {
                FsPlugin.CurrentDirectory = url;
                list.Items.Clear();
                foreach (pluginner.DirItem di in FsPlugin.DirectoryContent)
                {
                    if (di.Hidden == false)
                    {
                        ListViewItem NewItem = new ListViewItem(di.TextToShow);
                        NewItem.Tag = di.Path; //путь будет тегом
                        NewItem.SubItems.Add(Convert.ToString(di.Size / 1024) + "KB");
                        NewItem.SubItems.Add(di.Date.ToShortDateString());
                        list.Items.Add(NewItem);
                    }
                }
            }
            catch (pluginner.PleaseSwitchPluginException bezsilnost)
            {
                //todo: послать запрос хозяину, т.е. frmmain
            }
        }

        // Processing URL-box clicks and other stuff about it
        private void lblPath_DoubleClick(object sender, EventArgs e)
        {
            //preparing txtpath (url typing box)
            txtPath.Text = lblPath.Text;
            txtPath.Dock = DockStyle.Fill;
            txtPath.BackColor = lblPath.BackColor;
            txtPath.ForeColor = lblPath.ForeColor;
            txtPath.Font = new Font(txtPath.Font, FontStyle.Bold);
            txtPath.DoubleClick += this.txtPath_DoubleClick;
            txtPath.KeyUp += this.txtPath_KeyUp;
            txtPath.Focus();

            //switching controls
            tableLayoutPanel1.Controls.Remove(lblPath);
            tableLayoutPanel1.Controls.Add(txtPath,0,1);
        }

        private void txtPath_DoubleClick(object sender, EventArgs e)
        {
            //switching controls, don't forgetting that the
            //url viewing box is already ready
            tableLayoutPanel1.Controls.Remove(txtPath);
            tableLayoutPanel1.Controls.Add(lblPath, 0, 1);
        }

        private void txtPath_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                this.OnGotFocus(new EventArgs()); //extractly here because user should not switch focus until the panel is not used for file operations

                TextBox UrlBox = sender as TextBox;
                if(OnURLBoxNavigate != null) OnURLBoxNavigate(sender, new EventArgs<string>(UrlBox.Text));
                tableLayoutPanel1.Controls.Remove(txtPath);
                tableLayoutPanel1.Controls.Add(lblPath, 0, 1);
            }
        }
    }
}
