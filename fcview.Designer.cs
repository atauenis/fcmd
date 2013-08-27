namespace fcmd
{
    partial class fcview
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(fcview));
            this.mnu = new System.Windows.Forms.MenuStrip();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileReload = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileS1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFilePrint = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFilePrintOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFileS2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEdit = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditS1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditSelectAll = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEditS2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuEditFind = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewMode = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewModeText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewModeImage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewModePlugin = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTools = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuToolsPluginSettings = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptions = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuHelpAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.tsKeyboard = new System.Windows.Forms.ToolStrip();
            this.tsbHelpF1 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF2 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF3 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF4 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF5 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF6 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF7 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF8 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF9 = new System.Windows.Forms.ToolStripButton();
            this.tsbHelpF10 = new System.Windows.Forms.ToolStripButton();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.mnu.SuspendLayout();
            this.toolStripContainer1.BottomToolStripPanel.SuspendLayout();
            this.toolStripContainer1.ContentPanel.SuspendLayout();
            this.toolStripContainer1.SuspendLayout();
            this.tsKeyboard.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnu
            // 
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuEdit,
            this.mnuView,
            this.mnuTools,
            this.mnuHelp});
            this.mnu.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.Flow;
            this.mnu.Location = new System.Drawing.Point(0, 0);
            this.mnu.Name = "mnu";
            this.mnu.Size = new System.Drawing.Size(571, 23);
            this.mnu.TabIndex = 0;
            this.mnu.Text = "menuStrip1";
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileOpen,
            this.mnuFileReload,
            this.mnuFileS1,
            this.mnuFilePrint,
            this.mnuFilePrintOptions,
            this.mnuFileS2,
            this.mnuFileExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(48, 19);
            this.mnuFile.Text = "Файл";
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileOpen.Image")));
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuFileOpen.Size = new System.Drawing.Size(204, 22);
            this.mnuFileOpen.Text = "Открыть...";
            // 
            // mnuFileReload
            // 
            this.mnuFileReload.Image = ((System.Drawing.Image)(resources.GetObject("mnuFileReload.Image")));
            this.mnuFileReload.Name = "mnuFileReload";
            this.mnuFileReload.ShortcutKeys = System.Windows.Forms.Keys.F5;
            this.mnuFileReload.Size = new System.Drawing.Size(204, 22);
            this.mnuFileReload.Text = "Перезагрузить";
            this.mnuFileReload.Click += new System.EventHandler(this.mnuFileReload_Click);
            // 
            // mnuFileS1
            // 
            this.mnuFileS1.Name = "mnuFileS1";
            this.mnuFileS1.Size = new System.Drawing.Size(201, 6);
            // 
            // mnuFilePrint
            // 
            this.mnuFilePrint.Image = ((System.Drawing.Image)(resources.GetObject("mnuFilePrint.Image")));
            this.mnuFilePrint.Name = "mnuFilePrint";
            this.mnuFilePrint.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
            this.mnuFilePrint.Size = new System.Drawing.Size(204, 22);
            this.mnuFilePrint.Text = "Печать...";
            this.mnuFilePrint.Click += new System.EventHandler(this.mnuFilePrint_Click);
            // 
            // mnuFilePrintOptions
            // 
            this.mnuFilePrintOptions.Image = ((System.Drawing.Image)(resources.GetObject("mnuFilePrintOptions.Image")));
            this.mnuFilePrintOptions.Name = "mnuFilePrintOptions";
            this.mnuFilePrintOptions.Size = new System.Drawing.Size(204, 22);
            this.mnuFilePrintOptions.Text = "Параметры страницы...";
            this.mnuFilePrintOptions.Click += new System.EventHandler(this.mnuFilePrintOptions_Click);
            // 
            // mnuFileS2
            // 
            this.mnuFileS2.Name = "mnuFileS2";
            this.mnuFileS2.Size = new System.Drawing.Size(201, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
            this.mnuFileExit.ShortcutKeys = System.Windows.Forms.Keys.F10;
            this.mnuFileExit.Size = new System.Drawing.Size(204, 22);
            this.mnuFileExit.Text = "Закрыть просмотр";
            // 
            // mnuEdit
            // 
            this.mnuEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuEditCopy,
            this.mnuEditS1,
            this.mnuEditSelectAll,
            this.mnuEditS2,
            this.mnuEditFind});
            this.mnuEdit.Name = "mnuEdit";
            this.mnuEdit.Size = new System.Drawing.Size(59, 19);
            this.mnuEdit.Text = "Правка";
            // 
            // mnuEditCopy
            // 
            this.mnuEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditCopy.Image")));
            this.mnuEditCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuEditCopy.Name = "mnuEditCopy";
            this.mnuEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mnuEditCopy.Size = new System.Drawing.Size(190, 22);
            this.mnuEditCopy.Text = "Копировать";
            this.mnuEditCopy.Click += new System.EventHandler(this.mnuEditCopy_Click);
            // 
            // mnuEditS1
            // 
            this.mnuEditS1.Name = "mnuEditS1";
            this.mnuEditS1.Size = new System.Drawing.Size(187, 6);
            // 
            // mnuEditSelectAll
            // 
            this.mnuEditSelectAll.Name = "mnuEditSelectAll";
            this.mnuEditSelectAll.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.A)));
            this.mnuEditSelectAll.Size = new System.Drawing.Size(190, 22);
            this.mnuEditSelectAll.Text = "Выделить всё";
            this.mnuEditSelectAll.Click += new System.EventHandler(this.mnuEditSelectAll_Click);
            // 
            // mnuEditS2
            // 
            this.mnuEditS2.Name = "mnuEditS2";
            this.mnuEditS2.Size = new System.Drawing.Size(187, 6);
            // 
            // mnuEditFind
            // 
            this.mnuEditFind.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditFind.Image")));
            this.mnuEditFind.Name = "mnuEditFind";
            this.mnuEditFind.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
            this.mnuEditFind.Size = new System.Drawing.Size(190, 22);
            this.mnuEditFind.Text = "Поиск...";
            this.mnuEditFind.Click += new System.EventHandler(this.mnuEditFind_Click);
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewMode});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(39, 19);
            this.mnuView.Text = "Вид";
            // 
            // mnuViewMode
            // 
            this.mnuViewMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewModeText,
            this.mnuViewModeImage,
            this.mnuViewModePlugin});
            this.mnuViewMode.Name = "mnuViewMode";
            this.mnuViewMode.Size = new System.Drawing.Size(112, 22);
            this.mnuViewMode.Text = "Режим";
            // 
            // mnuViewModeText
            // 
            this.mnuViewModeText.Name = "mnuViewModeText";
            this.mnuViewModeText.Size = new System.Drawing.Size(150, 22);
            this.mnuViewModeText.Text = "Текст";
            // 
            // mnuViewModeImage
            // 
            this.mnuViewModeImage.Name = "mnuViewModeImage";
            this.mnuViewModeImage.Size = new System.Drawing.Size(150, 22);
            this.mnuViewModeImage.Text = "Изображения";
            // 
            // mnuViewModePlugin
            // 
            this.mnuViewModePlugin.Name = "mnuViewModePlugin";
            this.mnuViewModePlugin.Size = new System.Drawing.Size(150, 22);
            this.mnuViewModePlugin.Text = "Плагин...";
            // 
            // mnuTools
            // 
            this.mnuTools.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuToolsPluginSettings,
            this.mnuOptions});
            this.mnuTools.Name = "mnuTools";
            this.mnuTools.Size = new System.Drawing.Size(83, 19);
            this.mnuTools.Text = "Параметры";
            // 
            // mnuToolsPluginSettings
            // 
            this.mnuToolsPluginSettings.Image = ((System.Drawing.Image)(resources.GetObject("mnuToolsPluginSettings.Image")));
            this.mnuToolsPluginSettings.Name = "mnuToolsPluginSettings";
            this.mnuToolsPluginSettings.ShortcutKeys = System.Windows.Forms.Keys.F4;
            this.mnuToolsPluginSettings.Size = new System.Drawing.Size(252, 22);
            this.mnuToolsPluginSettings.Text = "Настройка плагина вывода...";
            this.mnuToolsPluginSettings.Click += new System.EventHandler(this.mnuToolsPluginSettings_Click);
            // 
            // mnuOptions
            // 
            this.mnuOptions.Name = "mnuOptions";
            this.mnuOptions.Size = new System.Drawing.Size(252, 22);
            this.mnuOptions.Text = "Параметры...";
            // 
            // mnuHelp
            // 
            this.mnuHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripSeparator5,
            this.mnuHelpAbout});
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(65, 19);
            this.mnuHelp.Text = "Справка";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(155, 6);
            // 
            // mnuHelpAbout
            // 
            this.mnuHelpAbout.Name = "mnuHelpAbout";
            this.mnuHelpAbout.Size = new System.Drawing.Size(158, 22);
            this.mnuHelpAbout.Text = "О программе...";
            this.mnuHelpAbout.Click += new System.EventHandler(this.mnuHelpAbout_Click);
            // 
            // toolStripContainer1
            // 
            // 
            // toolStripContainer1.BottomToolStripPanel
            // 
            this.toolStripContainer1.BottomToolStripPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.toolStripContainer1.BottomToolStripPanel.Controls.Add(this.tsKeyboard);
            // 
            // toolStripContainer1.ContentPanel
            // 
            this.toolStripContainer1.ContentPanel.Controls.Add(this.pnlContainer);
            this.toolStripContainer1.ContentPanel.Size = new System.Drawing.Size(571, 292);
            this.toolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStripContainer1.Location = new System.Drawing.Point(0, 23);
            this.toolStripContainer1.Name = "toolStripContainer1";
            this.toolStripContainer1.Size = new System.Drawing.Size(571, 342);
            this.toolStripContainer1.TabIndex = 4;
            this.toolStripContainer1.Text = "toolStripContainer1";
            // 
            // tsKeyboard
            // 
            this.tsKeyboard.AllowMerge = false;
            this.tsKeyboard.AutoSize = false;
            this.tsKeyboard.Dock = System.Windows.Forms.DockStyle.None;
            this.tsKeyboard.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsKeyboard.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbHelpF1,
            this.tsbHelpF2,
            this.tsbHelpF3,
            this.tsbHelpF4,
            this.tsbHelpF5,
            this.tsbHelpF6,
            this.tsbHelpF7,
            this.tsbHelpF8,
            this.tsbHelpF9,
            this.tsbHelpF10});
            this.tsKeyboard.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.tsKeyboard.Location = new System.Drawing.Point(3, 0);
            this.tsKeyboard.Name = "tsKeyboard";
            this.tsKeyboard.ShowItemToolTips = false;
            this.tsKeyboard.Size = new System.Drawing.Size(546, 25);
            this.tsKeyboard.TabIndex = 4;
            this.tsKeyboard.Text = "toolStrip1";
            this.tsKeyboard.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsKeyboard_ItemClicked);
            // 
            // tsbHelpF1
            // 
            this.tsbHelpF1.AutoToolTip = false;
            this.tsbHelpF1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF1.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF1.Image")));
            this.tsbHelpF1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF1.Name = "tsbHelpF1";
            this.tsbHelpF1.Size = new System.Drawing.Size(72, 22);
            this.tsbHelpF1.Text = "F1 Справка";
            // 
            // tsbHelpF2
            // 
            this.tsbHelpF2.AutoToolTip = false;
            this.tsbHelpF2.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF2.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF2.Image")));
            this.tsbHelpF2.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF2.Name = "tsbHelpF2";
            this.tsbHelpF2.Size = new System.Drawing.Size(23, 22);
            this.tsbHelpF2.Text = "F2";
            // 
            // tsbHelpF3
            // 
            this.tsbHelpF3.AutoToolTip = false;
            this.tsbHelpF3.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF3.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF3.Image")));
            this.tsbHelpF3.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF3.Name = "tsbHelpF3";
            this.tsbHelpF3.Size = new System.Drawing.Size(23, 22);
            this.tsbHelpF3.Text = "F3";
            // 
            // tsbHelpF4
            // 
            this.tsbHelpF4.AutoToolTip = false;
            this.tsbHelpF4.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF4.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF4.Image")));
            this.tsbHelpF4.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF4.Name = "tsbHelpF4";
            this.tsbHelpF4.Size = new System.Drawing.Size(46, 22);
            this.tsbHelpF4.Text = "F4 Вид";
            // 
            // tsbHelpF5
            // 
            this.tsbHelpF5.AutoToolTip = false;
            this.tsbHelpF5.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF5.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF5.Image")));
            this.tsbHelpF5.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF5.Name = "tsbHelpF5";
            this.tsbHelpF5.Size = new System.Drawing.Size(80, 22);
            this.tsbHelpF5.Text = "F5 Обновить";
            // 
            // tsbHelpF6
            // 
            this.tsbHelpF6.AutoToolTip = false;
            this.tsbHelpF6.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF6.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF6.Image")));
            this.tsbHelpF6.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF6.Name = "tsbHelpF6";
            this.tsbHelpF6.Size = new System.Drawing.Size(23, 22);
            this.tsbHelpF6.Text = "F6";
            // 
            // tsbHelpF7
            // 
            this.tsbHelpF7.AutoToolTip = false;
            this.tsbHelpF7.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF7.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF7.Image")));
            this.tsbHelpF7.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF7.Name = "tsbHelpF7";
            this.tsbHelpF7.Size = new System.Drawing.Size(61, 22);
            this.tsbHelpF7.Text = "F7 Поиск";
            // 
            // tsbHelpF8
            // 
            this.tsbHelpF8.AutoToolTip = false;
            this.tsbHelpF8.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF8.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF8.Image")));
            this.tsbHelpF8.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF8.Name = "tsbHelpF8";
            this.tsbHelpF8.Size = new System.Drawing.Size(69, 22);
            this.tsbHelpF8.Text = "F8 Формат";
            // 
            // tsbHelpF9
            // 
            this.tsbHelpF9.AutoToolTip = false;
            this.tsbHelpF9.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF9.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF9.Image")));
            this.tsbHelpF9.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF9.Name = "tsbHelpF9";
            this.tsbHelpF9.Size = new System.Drawing.Size(23, 22);
            this.tsbHelpF9.Text = "F9";
            // 
            // tsbHelpF10
            // 
            this.tsbHelpF10.AutoToolTip = false;
            this.tsbHelpF10.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsbHelpF10.Image = ((System.Drawing.Image)(resources.GetObject("tsbHelpF10.Image")));
            this.tsbHelpF10.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbHelpF10.Name = "tsbHelpF10";
            this.tsbHelpF10.Size = new System.Drawing.Size(66, 22);
            this.tsbHelpF10.Text = "F10 Выход";
            // 
            // pnlContainer
            // 
            this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContainer.Location = new System.Drawing.Point(0, 0);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(571, 292);
            this.pnlContainer.TabIndex = 3;
            // 
            // fcview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(571, 365);
            this.Controls.Add(this.toolStripContainer1);
            this.Controls.Add(this.mnu);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnu;
            this.Name = "fcview";
            this.Text = "fcview";
            this.Load += new System.EventHandler(this.fcview_Load);
            this.Resize += new System.EventHandler(this.fcview_Resize);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.fcview_KeyDown);
            this.mnu.ResumeLayout(false);
            this.mnu.PerformLayout();
            this.toolStripContainer1.BottomToolStripPanel.ResumeLayout(false);
            this.toolStripContainer1.ContentPanel.ResumeLayout(false);
            this.toolStripContainer1.ResumeLayout(false);
            this.toolStripContainer1.PerformLayout();
            this.tsKeyboard.ResumeLayout(false);
            this.tsKeyboard.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnu;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripMenuItem mnuFileOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuFileReload;
        private System.Windows.Forms.ToolStripSeparator mnuFileS1;
        private System.Windows.Forms.ToolStripMenuItem mnuFilePrint;
        private System.Windows.Forms.ToolStripMenuItem mnuFilePrintOptions;
        private System.Windows.Forms.ToolStripSeparator mnuFileS2;
        private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
        private System.Windows.Forms.ToolStripMenuItem mnuEdit;
        private System.Windows.Forms.ToolStripMenuItem mnuEditCopy;
        private System.Windows.Forms.ToolStripSeparator mnuEditS1;
        private System.Windows.Forms.ToolStripMenuItem mnuEditSelectAll;
        private System.Windows.Forms.ToolStripMenuItem mnuTools;
        private System.Windows.Forms.ToolStripMenuItem mnuToolsPluginSettings;
        private System.Windows.Forms.ToolStripMenuItem mnuOptions;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem mnuHelpAbout;
        private System.Windows.Forms.ToolStripSeparator mnuEditS2;
        private System.Windows.Forms.ToolStripMenuItem mnuEditFind;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuViewMode;
        private System.Windows.Forms.ToolStripMenuItem mnuViewModeText;
        private System.Windows.Forms.ToolStripMenuItem mnuViewModeImage;
        private System.Windows.Forms.ToolStripMenuItem mnuViewModePlugin;
        private System.Windows.Forms.ToolStripContainer toolStripContainer1;
        private System.Windows.Forms.Panel pnlContainer;
        private System.Windows.Forms.ToolStrip tsKeyboard;
        private System.Windows.Forms.ToolStripButton tsbHelpF1;
        private System.Windows.Forms.ToolStripButton tsbHelpF2;
        private System.Windows.Forms.ToolStripButton tsbHelpF3;
        private System.Windows.Forms.ToolStripButton tsbHelpF4;
        private System.Windows.Forms.ToolStripButton tsbHelpF5;
        private System.Windows.Forms.ToolStripButton tsbHelpF6;
        private System.Windows.Forms.ToolStripButton tsbHelpF7;
        private System.Windows.Forms.ToolStripButton tsbHelpF8;
        private System.Windows.Forms.ToolStripButton tsbHelpF9;
        private System.Windows.Forms.ToolStripButton tsbHelpF10;
    }
}