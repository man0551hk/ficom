using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
namespace LCMS
{
    public partial class DummyScreen : Form
    {
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        public DummyScreen()
        {           
            InitializeComponent();
            this.FormClosing += DummyScreen_FormClosing;
        }

        private void DummyScreen_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                GlobalFunc.tc.DisableDetector1();
            }
            catch { }
            try
            {
                GlobalFunc.tc.DisableDetector2();
            }
            catch { }
            GlobalFunc.tc.axUCONN21.Close();

            GlobalFunc.logManager.WriteLog("Close LCMS");
            Process.GetCurrentProcess().Kill();
            Application.Exit();

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (GlobalFunc.loginForm == null)
            {
                GlobalFunc.loginForm = new LoginForm();
            }
            GlobalFunc.loginForm.Show();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalFunc.tc.DisableDetector1();
            }
            catch { }
            try
            {
                GlobalFunc.tc.DisableDetector2();
            }
            catch { }
            GlobalFunc.tc.axUCONN21.Close();
            GlobalFunc.logManager.WriteLog("Close LCMS");
            Process.GetCurrentProcess().Kill();
            Application.Exit();
        }          
    }
}
