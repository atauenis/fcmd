namespace fcmd
{
    partial class FileProcessDialog
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
            this.lblStatus = new System.Windows.Forms.Label();
            this.pbrProgress = new System.Windows.Forms.ProgressBar();
            this.cmdCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblStatus
            // 
            this.lblStatus.Location = new System.Drawing.Point(12, 9);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(260, 58);
            this.lblStatus.TabIndex = 0;
            this.lblStatus.Text = "ТЕСТ ЗАПОЛНЕНИЯ 1\r\nТЕСТ ЗАПОЛНЕНИЯ 2\r\nТЕСТ ЗАПОЛНЕНИЯ 3\r\nТЕСТ ЗАПОЛНЕНИЯ 4";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // pbrProgress
            // 
            this.pbrProgress.Location = new System.Drawing.Point(15, 80);
            this.pbrProgress.Name = "pbrProgress";
            this.pbrProgress.Size = new System.Drawing.Size(257, 23);
            this.pbrProgress.TabIndex = 1;
            // 
            // cmdCancel
            // 
            this.cmdCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cmdCancel.Location = new System.Drawing.Point(105, 119);
            this.cmdCancel.Name = "cmdCancel";
            this.cmdCancel.Size = new System.Drawing.Size(75, 23);
            this.cmdCancel.TabIndex = 2;
            this.cmdCancel.Text = "&Отмена";
            this.cmdCancel.UseVisualStyleBackColor = true;
            // 
            // FileProcessDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cmdCancel;
            this.ClientSize = new System.Drawing.Size(284, 155);
            this.Controls.Add(this.cmdCancel);
            this.Controls.Add(this.pbrProgress);
            this.Controls.Add(this.lblStatus);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FileProcessDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FileProcessDialog";
            this.Load += new System.EventHandler(this.FileProcessDialog_Load);
            this.Shown += new System.EventHandler(this.FileProcessDialog_Shown);
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.Label lblStatus;
        public System.Windows.Forms.ProgressBar pbrProgress;
        public System.Windows.Forms.Button cmdCancel;
    }
}