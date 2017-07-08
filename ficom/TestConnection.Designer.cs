namespace LCMS
{
    partial class TestConnection
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TestConnection));
            this.axUCONN21 = new AxUMCBILib.AxUCONN2();
            this.axUDROP1 = new AxUMCBILib.AxUDROP();
            this.axULIST1 = new AxUMCBILib.AxULIST();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.axUCONN21)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axUDROP1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axULIST1)).BeginInit();
            this.SuspendLayout();
            // 
            // axUCONN21
            // 
            this.axUCONN21.Enabled = true;
            this.axUCONN21.Location = new System.Drawing.Point(599, 12);
            this.axUCONN21.Name = "axUCONN21";
            this.axUCONN21.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axUCONN21.OcxState")));
            this.axUCONN21.Size = new System.Drawing.Size(100, 50);
            this.axUCONN21.TabIndex = 0;
            // 
            // axUDROP1
            // 
            this.axUDROP1.Enabled = true;
            this.axUDROP1.Location = new System.Drawing.Point(12, 12);
            this.axUDROP1.Name = "axUDROP1";
            this.axUDROP1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axUDROP1.OcxState")));
            this.axUDROP1.Size = new System.Drawing.Size(556, 24);
            this.axUDROP1.TabIndex = 1;
            this.axUDROP1.NewSelection += new System.EventHandler(this.axUDROP1_NewSelection);
            // 
            // axULIST1
            // 
            this.axULIST1.Enabled = true;
            this.axULIST1.Location = new System.Drawing.Point(599, 68);
            this.axULIST1.Name = "axULIST1";
            this.axULIST1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axULIST1.OcxState")));
            this.axULIST1.Size = new System.Drawing.Size(100, 50);
            this.axULIST1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 80);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "label1";
            // 
            // TestConnection
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(711, 267);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.axULIST1);
            this.Controls.Add(this.axUDROP1);
            this.Controls.Add(this.axUCONN21);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "TestConnection";
            this.Text = "TestConnection";
            ((System.ComponentModel.ISupportInitialize)(this.axUCONN21)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axUDROP1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axULIST1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        public AxUMCBILib.AxUCONN2 axUCONN21;
        public AxUMCBILib.AxUDROP axUDROP1;
        public AxUMCBILib.AxULIST axULIST1;
        public System.Windows.Forms.Label label1;

    }
}