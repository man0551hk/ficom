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
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace LCMS
{
    public partial class SplashScreen : Form
    {
        //good -> separate two thread to run check Detector and update progress bar
        BackgroundWorker backgroundWorker = new BackgroundWorker();
        public SplashScreen()
        {           
            InitializeComponent();

            backgroundWorker.DoWork += backgroundWorker_DoWork;
            backgroundWorker.ProgressChanged += backgroundWorker_ProgressChanged;
            backgroundWorker.RunWorkerCompleted += backgroundWorker_RunWorkerCompleted;

            backgroundWorker.RunWorkerAsync();
            backgroundWorker.WorkerReportsProgress = true;//啟動回報進度
            pgbShow.Maximum = 1000;//ProgressBar上限
            pgbShow.Minimum = 0;//ProgressBar下限

            detectorLabel1.Text = "";
            detectorLabel2.Text = "";  
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            GlobalFunc.logManager.WriteLog("Open LCMS");
            int countTime = 0;

            #region check exe path is correct
            backgroundWorker.ReportProgress(30);          
            if (!File.Exists(GlobalFunc.basicSetting.ExePath))
            {
                for (int k = 0; k < 10; k++)
                {
                    noticeLabel.Invoke(new MethodInvoker(delegate { noticeLabel.Text = GlobalFunc.rm.GetString("exeNotFound"); }));
                    Thread.Sleep(1000);
                }
            }           
            #endregion

            Thread.Sleep(3000); //wait 5 second to start connect

            if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "top" || GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
            {
                #region Detector 1
                noticeLabel.Invoke(new MethodInvoker(delegate { noticeLabel.Text = GlobalFunc.rm.GetString("connectDetector1"); }));
                backgroundWorker.ReportProgress(50);
                countTime += 50;
                noticeLabel.Invoke(new MethodInvoker(delegate
                {
                    GlobalFunc.tc.checkDetector1Connection();
                }));
                if (!GlobalFunc.connectedToDetector1)
                {
                    detectorLabel1.Invoke(new MethodInvoker(delegate { detectorLabel1.Text = GlobalFunc.rm.GetString("failConnectDetector1"); }));
                }
                else
                {
                    detectorLabel1.Invoke(new MethodInvoker(delegate
                    {
                        detectorLabel1.Text = GlobalFunc.rm.GetString("successDetector1");
                        GlobalFunc.tc.GetDetector1ICR();
                    }));
                    detectorLabel1.ForeColor = Color.Blue;
                }
                #endregion
                Thread.Sleep(3000);
            }

            if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "bottom" || GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
            {
                #region Detector 2
                noticeLabel.Invoke(new MethodInvoker(delegate { noticeLabel.Text = GlobalFunc.rm.GetString("connectDetector2"); }));
                backgroundWorker.ReportProgress(50);
                countTime += 50;
                noticeLabel.Invoke(new MethodInvoker(delegate
                {
                    GlobalFunc.tc.checkDetector2Connection();
                }));
                if (!GlobalFunc.connectedToDetector2)
                {
                    detectorLabel2.Invoke(new MethodInvoker(delegate { detectorLabel2.Text += GlobalFunc.rm.GetString("failConnectDetector2"); }));
                }
                else
                {
                    //GlobalFunc.GetIcrTemp2();
                    detectorLabel2.Invoke(new MethodInvoker(delegate
                    {
                        detectorLabel2.Text += GlobalFunc.rm.GetString("successDetector2");
                        GlobalFunc.tc.GetDetector2ICR();
                    }));
                    detectorLabel2.ForeColor = Color.Blue;
                }
                #endregion
            }


            /*for (int i = 0; i < 300; i++)
            {
                Thread.Sleep(10);
                noticeLabel.Invoke(new MethodInvoker(delegate { noticeLabel.Text = GlobalFunc.rm.GetString("findPreviousJob"); }));
                backgroundWorker.ReportProgress(50);
            }*/
            
            for (int i = 0; i < 1000 - countTime; i++)
            {
                Thread.Sleep(10);
                backgroundWorker.ReportProgress(i);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbShow.Value = e.ProgressPercentage;
            //percentageText.Text = pgbShow.Value + "%";
            //當backgroundWorker的i改變時就會觸發，進而更改pgbShow.Value
        }

        private void backgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            Thread.Sleep(500);
            this.Hide();
            if (GlobalFunc.mainForm == null)
            {
                GlobalFunc.mainForm = new MainForm();
                GlobalFunc.mainForm.BringToFront();
            }
            else
            {
                GlobalFunc.mainForm.Dispose();
                GlobalFunc.mainForm = new MainForm();
                GlobalFunc.mainForm.BringToFront();
            }
            GlobalFunc.mainForm.Show();                
            //MessageBox.Show("Processing was completed");
            //當backgroundWorker工作完成時顯示

        }
    }
}
