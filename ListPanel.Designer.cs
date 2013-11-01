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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.list = new System.Windows.Forms.ListView();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblPath = new System.Windows.Forms.Label();
            this.tsDisks = new System.Windows.Forms.ToolStrip();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.Outset;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.list, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblStatus, 0, 3);
            this.tableLayoutPanel1.Controls.Add(this.lblPath, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.tsDisks, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 4;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(283, 327);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // list
            // 
            this.list.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.list.Dock = System.Windows.Forms.DockStyle.Fill;
            this.list.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.list.FullRowSelect = true;
            this.list.Location = new System.Drawing.Point(5, 47);
            this.list.Name = "list";
            this.list.Size = new System.Drawing.Size(273, 260);
            this.list.TabIndex = 1;
            this.list.UseCompatibleStateImageBehavior = false;
            this.list.View = System.Windows.Forms.View.Details;
            this.list.SelectedIndexChanged += new System.EventHandler(this.list_SelectedIndexChanged);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblStatus.Location = new System.Drawing.Point(5, 312);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(273, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = " ";
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.BackColor = System.Drawing.SystemColors.ActiveCaption;
            this.lblPath.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lblPath.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.lblPath.Location = new System.Drawing.Point(5, 29);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(273, 13);
            this.lblPath.TabIndex = 3;
            this.lblPath.Text = "lblPath";
            this.lblPath.DoubleClick += new System.EventHandler(this.lblPath_DoubleClick);
            // 
            // tsDisks
            // 
            this.tsDisks.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.tsDisks.Location = new System.Drawing.Point(2, 2);
            this.tsDisks.Name = "tsDisks";
            this.tsDisks.RenderMode = System.Windows.Forms.ToolStripRenderMode.System;
            this.tsDisks.Size = new System.Drawing.Size(279, 25);
            this.tsDisks.TabIndex = 4;
            this.tsDisks.Text = "toolStrip1";
            // 
            // ListPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ListPanel";
            this.Size = new System.Drawing.Size(283, 327);
            this.Load += new System.EventHandler(this.ListPanel_Load);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        public System.Windows.Forms.ListView list;
        public System.Windows.Forms.Label lblStatus;
        public System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.ToolStrip tsDisks;


    }
}
