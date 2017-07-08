using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LCMS
{
    public partial class CustomMessageBox : Form
    {
        static CustomMessageBox newMessageBox;
        static string Button_id;
        static int disposeFormTimer;
        static Timer msgTimer = new Timer();

        public CustomMessageBox()
        {
            InitializeComponent();
            this.TopMost = true;
            disposeFormTimer = 5;
        }

        public static string Show(string txtMessage, string txtTitle, string yesBtnTxt, string noBtnTxt, bool timer, int interval)
        {
            newMessageBox = new CustomMessageBox();
            newMessageBox.labelTxt.Text = txtMessage;
            newMessageBox.Text = txtTitle;
            if (yesBtnTxt != "")
            {
                newMessageBox.yesBtn.Visible = true;
                newMessageBox.yesBtn.Text = yesBtnTxt;
            }
            else
            {
                newMessageBox.yesBtn.Visible = false;
            }
            if (noBtnTxt != "")
            {
                newMessageBox.noBtn.Visible = true;
                newMessageBox.noBtn.Text = noBtnTxt;
            }
            else
            {
                newMessageBox.noBtn.Visible = false;
            }
            if (timer)
            {  
                msgTimer.Interval = interval;
                msgTimer.Enabled = true;
                msgTimer.Start();
                msgTimer.Tick += new EventHandler(msgTimer_Tick);
                Button_id = "1";
            }
            int locationY = newMessageBox.labelTxt.Height + 30;
            int x = newMessageBox.yesBtn.Location.X;
            newMessageBox.yesBtn.Location = new Point(x, locationY);

            x = newMessageBox.noBtn.Location.X;
            newMessageBox.noBtn.Location = new Point(x, locationY);
            newMessageBox.ShowDialog();
            return Button_id;
        }
        
        public static void UpdateLabel(string text)
        {
            newMessageBox.labelTxt.Text = text;
        }

        private static void msgTimer_Tick(object sender, EventArgs e)
        {
            disposeFormTimer--;

            if (disposeFormTimer >= 0)
            {
                //newMessageBox.timerLabel.Text = disposeFormTimer.ToString();
            }
            else
            {
                //CommFunc.sdc.UpdateStatusLabel("Stand By", 255, 255, 0);
                newMessageBox.Dispose();
            }
        }

        private void yesBtn_Click(object sender, EventArgs e)
        {
            newMessageBox.Dispose();
            Button_id = "1";
        }

        private void noBtn_Click(object sender, EventArgs e)
        {
            newMessageBox.Dispose();
            Button_id = "2";
        }

        public string txtMessage = "";
        public string txtTitle = "";
        public void showManualClose()
        {  
            labelTxt.Text = txtMessage;
            Text = txtTitle;
            yesBtn.Visible = false;           
            noBtn.Visible = false;           
            ShowDialog();           
        }
        
    }
}
