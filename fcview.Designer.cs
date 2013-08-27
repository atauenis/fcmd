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
            this.файлToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.mnuMode = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewModeText = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewModeImage = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuViewModePlugin = new System.Windows.Forms.ToolStripMenuItem();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customizeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.optionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contentsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.indexToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.searchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuKeyboard = new System.Windows.Forms.MenuStrip();
            this.pnlContainer = new System.Windows.Forms.Panel();
            this.mnu.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnu
            // 
            this.mnu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.файлToolStripMenuItem,
            this.mnuEdit,
            this.mnuView,
            this.toolsToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.mnu.Location = new System.Drawing.Point(0, 0);
            this.mnu.Name = "mnu";
            this.mnu.Size = new System.Drawing.Size(499, 24);
            this.mnu.TabIndex = 0;
            this.mnu.Text = "menuStrip1";
            // 
            // файлToolStripMenuItem
            // 
            this.файлToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileOpen,
            this.mnuFileReload,
            this.mnuFileS1,
            this.mnuFilePrint,
            this.mnuFilePrintOptions,
            this.mnuFileS2,
            this.mnuFileExit});
            this.файлToolStripMenuItem.Name = "файлToolStripMenuItem";
            this.файлToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.файлToolStripMenuItem.Text = "Файл";
            // 
            // mnuFileOpen
            // 
            this.mnuFileOpen.Name = "mnuFileOpen";
            this.mnuFileOpen.Size = new System.Drawing.Size(204, 22);
            this.mnuFileOpen.Text = "Открыть...";
            // 
            // mnuFileReload
            // 
            this.mnuFileReload.Name = "mnuFileReload";
            this.mnuFileReload.Size = new System.Drawing.Size(204, 22);
            this.mnuFileReload.Text = "Перезагрузить";
            // 
            // mnuFileS1
            // 
            this.mnuFileS1.Name = "mnuFileS1";
            this.mnuFileS1.Size = new System.Drawing.Size(201, 6);
            // 
            // mnuFilePrint
            // 
            this.mnuFilePrint.Name = "mnuFilePrint";
            this.mnuFilePrint.Size = new System.Drawing.Size(204, 22);
            this.mnuFilePrint.Text = "Печать...";
            // 
            // mnuFilePrintOptions
            // 
            this.mnuFilePrintOptions.Name = "mnuFilePrintOptions";
            this.mnuFilePrintOptions.Size = new System.Drawing.Size(204, 22);
            this.mnuFilePrintOptions.Text = "Параметры страницы...";
            // 
            // mnuFileS2
            // 
            this.mnuFileS2.Name = "mnuFileS2";
            this.mnuFileS2.Size = new System.Drawing.Size(201, 6);
            // 
            // mnuFileExit
            // 
            this.mnuFileExit.Name = "mnuFileExit";
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
            this.mnuEdit.Size = new System.Drawing.Size(59, 20);
            this.mnuEdit.Text = "Правка";
            // 
            // mnuEditCopy
            // 
            this.mnuEditCopy.Image = ((System.Drawing.Image)(resources.GetObject("mnuEditCopy.Image")));
            this.mnuEditCopy.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuEditCopy.Name = "mnuEditCopy";
            this.mnuEditCopy.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.mnuEditCopy.Size = new System.Drawing.Size(181, 22);
            this.mnuEditCopy.Text = "Копировать";
            // 
            // mnuEditS1
            // 
            this.mnuEditS1.Name = "mnuEditS1";
            this.mnuEditS1.Size = new System.Drawing.Size(178, 6);
            // 
            // mnuEditSelectAll
            // 
            this.mnuEditSelectAll.Name = "mnuEditSelectAll";
            this.mnuEditSelectAll.Size = new System.Drawing.Size(181, 22);
            this.mnuEditSelectAll.Text = "Выделить всё";
            // 
            // mnuEditS2
            // 
            this.mnuEditS2.Name = "mnuEditS2";
            this.mnuEditS2.Size = new System.Drawing.Size(178, 6);
            // 
            // mnuEditFind
            // 
            this.mnuEditFind.Name = "mnuEditFind";
            this.mnuEditFind.Size = new System.Drawing.Size(181, 22);
            this.mnuEditFind.Text = "Поиск...";
            // 
            // mnuView
            // 
            this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuMode});
            this.mnuView.Name = "mnuView";
            this.mnuView.Size = new System.Drawing.Size(39, 20);
            this.mnuView.Text = "Вид";
            // 
            // mnuMode
            // 
            this.mnuMode.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewModeText,
            this.mnuViewModeImage,
            this.mnuViewModePlugin});
            this.mnuMode.Name = "mnuMode";
            this.mnuMode.Size = new System.Drawing.Size(112, 22);
            this.mnuMode.Text = "Режим";
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
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.customizeToolStripMenuItem,
            this.optionsToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            this.toolsToolStripMenuItem.Text = "&Tools";
            // 
            // customizeToolStripMenuItem
            // 
            this.customizeToolStripMenuItem.Name = "customizeToolStripMenuItem";
            this.customizeToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.customizeToolStripMenuItem.Text = "&Customize";
            // 
            // optionsToolStripMenuItem
            // 
            this.optionsToolStripMenuItem.Name = "optionsToolStripMenuItem";
            this.optionsToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
            this.optionsToolStripMenuItem.Text = "&Options";
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.contentsToolStripMenuItem,
            this.indexToolStripMenuItem,
            this.searchToolStripMenuItem,
            this.toolStripSeparator5,
            this.aboutToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // contentsToolStripMenuItem
            // 
            this.contentsToolStripMenuItem.Name = "contentsToolStripMenuItem";
            this.contentsToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.contentsToolStripMenuItem.Text = "&Contents";
            // 
            // indexToolStripMenuItem
            // 
            this.indexToolStripMenuItem.Name = "indexToolStripMenuItem";
            this.indexToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.indexToolStripMenuItem.Text = "&Index";
            // 
            // searchToolStripMenuItem
            // 
            this.searchToolStripMenuItem.Name = "searchToolStripMenuItem";
            this.searchToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.searchToolStripMenuItem.Text = "&Search";
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(119, 6);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
            this.aboutToolStripMenuItem.Text = "&About...";
            // 
            // mnuKeyboard
            // 
            this.mnuKeyboard.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.mnuKeyboard.Location = new System.Drawing.Point(0, 318);
            this.mnuKeyboard.Name = "mnuKeyboard";
            this.mnuKeyboard.Size = new System.Drawing.Size(499, 24);
            this.mnuKeyboard.TabIndex = 1;
            this.mnuKeyboard.Text = "menuStrip1";
            // 
            // pnlContainer
            // 
            this.pnlContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlContainer.Location = new System.Drawing.Point(0, 24);
            this.pnlContainer.Name = "pnlContainer";
            this.pnlContainer.Size = new System.Drawing.Size(499, 294);
            this.pnlContainer.TabIndex = 2;
            // 
            // fcview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(499, 342);
            this.Controls.Add(this.pnlContainer);
            this.Controls.Add(this.mnu);
            this.Controls.Add(this.mnuKeyboard);
            this.KeyPreview = true;
            this.MainMenuStrip = this.mnu;
            this.Name = "fcview";
            this.Text = "fcview";
            this.mnu.ResumeLayout(false);
            this.mnu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip mnu;
        private System.Windows.Forms.ToolStripMenuItem файлToolStripMenuItem;
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
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem customizeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem optionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem contentsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem indexToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem searchToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator mnuEditS2;
        private System.Windows.Forms.ToolStripMenuItem mnuEditFind;
        private System.Windows.Forms.ToolStripMenuItem mnuView;
        private System.Windows.Forms.ToolStripMenuItem mnuMode;
        private System.Windows.Forms.ToolStripMenuItem mnuViewModeText;
        private System.Windows.Forms.ToolStripMenuItem mnuViewModeImage;
        private System.Windows.Forms.ToolStripMenuItem mnuViewModePlugin;
        private System.Windows.Forms.MenuStrip mnuKeyboard;
        private System.Windows.Forms.Panel pnlContainer;
    }
}