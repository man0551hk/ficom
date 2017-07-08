namespace LCMS
{
    partial class ScriptEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditor));
            this.editScriptBtn = new System.Windows.Forms.Button();
            this.scriptArea = new System.Windows.Forms.TextBox();
            this.saveFileDialog1 = new System.Windows.Forms.SaveFileDialog();
            this.openScriptDialog = new System.Windows.Forms.OpenFileDialog();
            this.openScriptBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // editScriptBtn
            // 
            this.editScriptBtn.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.editScriptBtn.Location = new System.Drawing.Point(12, 12);
            this.editScriptBtn.Name = "editScriptBtn";
            this.editScriptBtn.Size = new System.Drawing.Size(123, 35);
            this.editScriptBtn.TabIndex = 7;
            this.editScriptBtn.Text = "Save Script";
            this.editScriptBtn.UseVisualStyleBackColor = true;
            this.editScriptBtn.Click += new System.EventHandler(this.editScriptBtn_Click);
            // 
            // scriptArea
            // 
            this.scriptArea.Location = new System.Drawing.Point(12, 54);
            this.scriptArea.Multiline = true;
            this.scriptArea.Name = "scriptArea";
            this.scriptArea.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.scriptArea.Size = new System.Drawing.Size(786, 578);
            this.scriptArea.TabIndex = 8;
            // 
            // saveFileDialog1
            // 
            this.saveFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.saveFileDialog1_FileOk);
            // 
            // openScriptDialog
            // 
            this.openScriptDialog.FileName = "openFileDialog1";
            this.openScriptDialog.FileOk += new System.ComponentModel.CancelEventHandler(this.openScriptDialog_FileOk);
            // 
            // openScriptBtn
            // 
            this.openScriptBtn.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.openScriptBtn.Location = new System.Drawing.Point(141, 12);
            this.openScriptBtn.Name = "openScriptBtn";
            this.openScriptBtn.Size = new System.Drawing.Size(123, 35);
            this.openScriptBtn.TabIndex = 9;
            this.openScriptBtn.Text = "Open Script";
            this.openScriptBtn.UseVisualStyleBackColor = true;
            this.openScriptBtn.Click += new System.EventHandler(this.openScriptBtn_Click);
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(810, 644);
            this.Controls.Add(this.openScriptBtn);
            this.Controls.Add(this.scriptArea);
            this.Controls.Add(this.editScriptBtn);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScriptEditor";
            this.Text = "ScriptEditor";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button editScriptBtn;
        private System.Windows.Forms.TextBox scriptArea;
        private System.Windows.Forms.SaveFileDialog saveFileDialog1;
        private System.Windows.Forms.OpenFileDialog openScriptDialog;
        private System.Windows.Forms.Button openScriptBtn;
    }
}