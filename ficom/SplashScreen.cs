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
using System.Reflection;
using System.Resources;

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
            backgroundWorker.WorkerReportsProgress = true;
            pgbShow.Maximum = 1000;
            pgbShow.Minimum = 0;

            detectorLabel1.Text = "";
            detectorLabel2.Text = "";  
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            GlobalFunc.logManager.WriteLog("Open LCMS");
            int countTime = 0;

            #region loading basic.xml
            try
            {
                
                XmlSerializer deserializer1 = new XmlSerializer(typeof(BasicSetting));
                TextReader textReader = new StreamReader(@Directory.GetCurrentDirectory() + @"\xml\Basic.xml");
                GlobalFunc.basicSetting = (BasicSetting)deserializer1.Deserialize(textReader);
                textReader.Close();
                if (GlobalFunc.basicSetting.Lang == "Zn")
                {
                    GlobalFunc.rm = new ResourceManager("LCMS.Lang.LangZn", GlobalFunc.assembly);
                }
                else
                {
                    GlobalFunc.rm = new ResourceManager("LCMS.Lang.LangEn", GlobalFunc.assembly);
                }

                noticeLabel.Invoke(new MethodInvoker(delegate { noticeLabel.Text = GlobalFunc.rm.GetString("loadBasicXML"); }));



                countTime += 50;
                if (GlobalFunc.basicSetting.GetTemp.ToLower() == "on")
                {
                    GlobalFunc.getTemp = true;
                }
                else if (GlobalFunc.basicSetting.GetTemp.ToLower() == "off")
                {
                    GlobalFunc.getTemp = false;
                }
                countTime += 50;
                GlobalFunc.intIOAddress = Convert.ToInt32(GlobalFunc.basicSetting.IoAddress, 16);
                countTime += 50;
                XmlSerializer deserializer2 = new XmlSerializer(typeof(ScriptSet));
                TextReader textReader2 = new StreamReader(@Directory.GetCurrentDirectory() + @"\xml\DualScript.xml");
                GlobalFunc.dualScriptSet = (ScriptSet)deserializer2.Deserialize(textReader2);
                textReader2.Close();
                countTime += 50;

                XmlSerializer deserializer3 = new XmlSerializer(typeof(ScriptSet));
                TextReader textReader3 = new StreamReader(@Directory.GetCurrentDirectory() + @"\xml\TopScript.xml");
                GlobalFunc.topScriptSet = (ScriptSet)deserializer3.Deserialize(textReader3);
                textReader3.Close();
                countTime += 50;

                XmlSerializer deserializer4 = new XmlSerializer(typeof(ScriptSet));
                TextReader textReader4 = new StreamReader(@Directory.GetCurrentDirectory() + @"\xml\BottomScript.xml");
                GlobalFunc.bottomScriptSet = (ScriptSet)deserializer4.Deserialize(textReader4);
                textReader4.Close();

                GlobalFunc.LoadIsotopXML();

                if (GlobalFunc.basicSetting.InsalledIO.ToLower() == "true")
                {
                    try
                    {
                        //reset to only led #1 on
                        PortAccess.Output(GlobalFunc.intIOAddress, 0);
                    }
                    catch
                    {
                        MessageBox.Show("IO Address not found");
                    }
                }
                countTime += 50;
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            #endregion

            #region check inpot32.dll is existed
            noticeLabel.Invoke(new MethodInvoker(delegate { noticeLabel.Text = GlobalFunc.rm.GetString("checkIO"); }));
            if (!File.Exists(@"C:\Windows\System32\inpout32.dll"))
            {
                if (File.Exists(@Directory.GetCurrentDirectory() + @"\inpout32.dll"))
                {
                    noticeLabel.Invoke(new MethodInvoker(delegate
                    {
                        MessageBox.Show(@"Please copy inpout32.dll from C:\LCMS to C:\Windows\System32");
                    }));
                }
                else
                {
                    noticeLabel.Invoke(new MethodInvoker(delegate
                    {
                        MessageBox.Show("Can't find inpout32.dll in program directory");
                    }));
                    
                }
            }
            countTime += 50;
            Thread.Sleep(1000);
            #endregion

            #region check exe path is correct
            //no more use mca32
            //backgroundWorker.ReportProgress(30);          
            //if (!File.Exists(GlobalFunc.basicSetting.ExePath))
            //{
            //    for (int k = 0; k < 10; k++)
            //    {
            //        noticeLabel.Invoke(new MethodInvoker(delegate { noticeLabel.Text = GlobalFunc.rm.GetString("exeNotFound"); }));
            //        Thread.Sleep(1000);
            //    }
            //}       
            //Thread.Sleep(3000); //wait 5 second to start connect
            #endregion

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
                    detectorLabel2.Invoke(new MethodInvoker(delegate
                    {
                        detectorLabel2.Text += GlobalFunc.rm.GetString("successDetector2");
                        GlobalFunc.tc.GetDetector2ICR();
                    }));
                    detectorLabel2.ForeColor = Color.Blue;
                }
                #endregion
            }

            for (int i = 0; i < 1000 - countTime; i++)
            {
                Thread.Sleep(10);
                backgroundWorker.ReportProgress(i);
            }
        }

        private void backgroundWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            pgbShow.Value = e.ProgressPercentage;
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
        }
    }
}
