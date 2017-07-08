using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
namespace LCMS
{
    public partial class ScriptEditor : Form
    {
        public ScriptEditor()
        {
            InitializeComponent();
        }

        public void SetScript(string path)
        {
            scriptArea.Text = File.ReadAllText(path).Replace("\r\n", System.Environment.NewLine).Replace("\n", System.Environment.NewLine).Replace("\t", " ");
        }

        private void editScriptBtn_Click(object sender, EventArgs e)
        {
            saveFileDialog1.ShowDialog();
        }

        private void saveFileDialog1_FileOk(object sender, CancelEventArgs e)
        {
            if (saveFileDialog1.FileName != "")
            {
                using (StreamWriter outfile = new StreamWriter(saveFileDialog1.FileName))
                {
                    outfile.Write(scriptArea.Text.ToString());
                }
            }
        }

        private void openScriptBtn_Click(object sender, EventArgs e)
        {
            openScriptDialog.AddExtension = true;
            openScriptDialog.Filter = "(*.*)|*.*";
            openScriptDialog.ShowDialog();
        }

        private void openScriptDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (openScriptDialog.FileName != "")
            {
                SetScript(openScriptDialog.FileName);
            }
        }
    }
}
