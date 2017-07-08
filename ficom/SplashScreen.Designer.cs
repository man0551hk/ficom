namespace LCMS
{
    partial class SplashScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SplashScreen));
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.pgbShow = new System.Windows.Forms.ProgressBar();
            this.noticeLabel = new System.Windows.Forms.Label();
            this.detectorLabel1 = new System.Windows.Forms.Label();
            this.detectorLabel2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(-3, 0);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(800, 362);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // pgbShow
            // 
            this.pgbShow.Location = new System.Drawing.Point(2, 363);
            this.pgbShow.Name = "pgbShow";
            this.pgbShow.Size = new System.Drawing.Size(795, 23);
            this.pgbShow.TabIndex = 1;
            // 
            // noticeLabel
            // 
            this.noticeLabel.AutoSize = true;
            this.noticeLabel.BackColor = System.Drawing.Color.White;
            this.noticeLabel.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.noticeLabel.ForeColor = System.Drawing.Color.Red;
            this.noticeLabel.Location = new System.Drawing.Point(5, 328);
            this.noticeLabel.Name = "noticeLabel";
            this.noticeLabel.Size = new System.Drawing.Size(0, 26);
            this.noticeLabel.TabIndex = 2;
            // 
            // detectorLabel1
            // 
            this.detectorLabel1.AutoSize = true;
            this.detectorLabel1.BackColor = System.Drawing.Color.White;
            this.detectorLabel1.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detectorLabel1.ForeColor = System.Drawing.Color.Red;
            this.detectorLabel1.Location = new System.Drawing.Point(5, 9);
            this.detectorLabel1.Name = "detectorLabel1";
            this.detectorLabel1.Size = new System.Drawing.Size(156, 25);
            this.detectorLabel1.TabIndex = 3;
            this.detectorLabel1.Text = "detectorLabel";
            // 
            // detectorLabel2
            // 
            this.detectorLabel2.AutoSize = true;
            this.detectorLabel2.BackColor = System.Drawing.Color.White;
            this.detectorLabel2.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.detectorLabel2.ForeColor = System.Drawing.Color.Red;
            this.detectorLabel2.Location = new System.Drawing.Point(5, 34);
            this.detectorLabel2.Name = "detectorLabel2";
            this.detectorLabel2.Size = new System.Drawing.Size(156, 25);
            this.detectorLabel2.TabIndex = 5;
            this.detectorLabel2.Text = "detectorLabel";
            // 
            // SplashScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(799, 388);
            this.ControlBox = false;
            this.Controls.Add(this.detectorLabel2);
            this.Controls.Add(this.detectorLabel1);
            this.Controls.Add(this.noticeLabel);
            this.Controls.Add(this.pgbShow);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SplashScreen";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "SplashScreen";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ProgressBar pgbShow;
        private System.Windows.Forms.Label noticeLabel;
        private System.Windows.Forms.Label detectorLabel1;
        private System.Windows.Forms.Label detectorLabel2;
    }
}