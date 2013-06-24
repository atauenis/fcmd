namespace fcmd
{
    partial class ListPanel
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lbx = new System.Windows.Forms.ListBox();
            this.SuspendLayout();
            // 
            // lbx
            // 
            this.lbx.FormattingEnabled = true;
            this.lbx.Location = new System.Drawing.Point(18, 29);
            this.lbx.Name = "lbx";
            this.lbx.ScrollAlwaysVisible = true;
            this.lbx.Size = new System.Drawing.Size(64, 69);
            this.lbx.TabIndex = 0;
            this.lbx.DoubleClick += new System.EventHandler(this.lbx_DblClick);
            // 
            // ListPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lbx);
            this.Name = "ListPanel";
            this.Load += new System.EventHandler(this.ListPanel_Load);
            this.Resize += new System.EventHandler(this.ListPanel_Resize);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListBox lbx;
    }
}
