namespace ficom
{
    partial class WarmUp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WarmUp));
            this.warmupStatusTxt = new System.Windows.Forms.Label();
            this.pb_Process = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // warmupStatusTxt
            // 
            this.warmupStatusTxt.AutoSize = true;
            this.warmupStatusTxt.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.warmupStatusTxt.Location = new System.Drawing.Point(12, 9);
            this.warmupStatusTxt.Name = "warmupStatusTxt";
            this.warmupStatusTxt.Size = new System.Drawing.Size(0, 26);
            this.warmupStatusTxt.TabIndex = 10;
            // 
            // pb_Process
            // 
            this.pb_Process.Location = new System.Drawing.Point(1, 70);
            this.pb_Process.Name = "pb_Process";
            this.pb_Process.Size = new System.Drawing.Size(601, 23);
            this.pb_Process.TabIndex = 11;
            // 
            // WarmUp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(602, 94);
            this.Controls.Add(this.pb_Process);
            this.Controls.Add(this.warmupStatusTxt);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "WarmUp";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.WarmUp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label warmupStatusTxt;
        private System.Windows.Forms.ProgressBar pb_Process;
    }
}