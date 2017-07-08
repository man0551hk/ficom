using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Sql;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Diagnostics;
using System.Data.SqlServerCe;
using System.Reflection;
using System.Resources;
using System.IO;
namespace LCMS
{
    public partial class MainForm : Form
    {
        SqlCeConnection con = new SqlCeConnection(Properties.Settings.Default.lcmsConnectionString);
        List<string> roiList = new List<string>();
        string detectorName = "";

        int interval = 0;
        System.Windows.Forms.Timer temperCallBgTimer;
        BackgroundWorker temperbackgroundWorker;
        System.Threading.Timer temperEnsureWorkerGetsCalled;
        object lockObject = new object();
        public string currPanel = "";
        bool sampleSizeDot = false;
        
        public MainForm()
        {
            InitializeComponent();            
            this.FormClosing += MainForm_FormClosing;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            //this.MinimizeBox = false;
            //this.TopMost = true;
            selectProfileBtn.Visible = false;
            backgroundPanel.Visible = false;
            runBkOpBtn.Enabled = false;
            Mea_RunBtn.Enabled = false;
            versionLabel.Text = "beta ver. 4.0";
            this.Activated += MainForm_Activated;
            this.Deactivate += MainForm_Deactivate;

            if (GlobalFunc.getTemp == true)
            {
                temperCallBgTimer = new System.Windows.Forms.Timer();
                temperCallBgTimer.Tick += new EventHandler(tmrCallBgWorker_Tick);
                temperCallBgTimer.Interval = 10000;

                temperbackgroundWorker = new BackgroundWorker();

                temperbackgroundWorker.DoWork += new DoWorkEventHandler(temperbackgroundWorker_DoWork);
                temperbackgroundWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(temperbackgroundWorker_RunWorkerCompleted);
                temperbackgroundWorker.WorkerReportsProgress = true;

                temperCallBgTimer.Start();
            }
        }

        private void tmrCallBgWorker_Tick(object sender, EventArgs e)
        {
            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    if (!temperbackgroundWorker.IsBusy)
                    {
                        temperbackgroundWorker.RunWorkerAsync();
                    }
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
            }
            else
            {
                temperEnsureWorkerGetsCalled = new System.Threading.Timer(new TimerCallback(tmrEnsureWorkerGetsCalled_Callback), null, 0, 15000);
            }
        }

        public void temperbackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "top" || GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
            {
                GlobalFunc.tc.GetDetector1Temp();
                topTemp.Invoke(new MethodInvoker(delegate { topTemp.Text = GlobalFunc.det1_temp + "°C"; }));
            }
            if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "bottom" || GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
            {
                GlobalFunc.tc.GetDetector2Temp();
                bottomTemp.Invoke(new MethodInvoker(delegate { bottomTemp.Text = GlobalFunc.det2_temp + "°C"; }));
            }
            GC.Collect();
        }

        public void temperbackgroundWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            
        }

        public void tmrEnsureWorkerGetsCalled_Callback(object obj)
        {
            if (Monitor.TryEnter(lockObject))
            {
                try
                {
                    if (!temperbackgroundWorker.IsBusy)
                    {
                        temperbackgroundWorker.RunWorkerAsync();
                    }
                }
                finally
                {
                    Monitor.Exit(lockObject);
                }
                temperEnsureWorkerGetsCalled = null;
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            #region set text
            topTemp.Text = "";
            bottomTemp.Text = "";
            label18.Visible = false;
            label20.Visible = false;

            if (GlobalFunc.getTemp == true)
            {
                if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "top" || GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
                {
                    label18.Visible = true;
                    if (GlobalFunc.connectedToDetector1)
                    {
                        if (GlobalFunc.det1_temp == 0.0)
                        {
                            GlobalFunc.tc.GetDetector1Temp();
                            topTemp.Text = GlobalFunc.det1_temp + "°C";
                        }
                    }
                    else
                    {
                        if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
                        {
                            UpdateStatusLabel("Alarm", 255, 0, 0);
                        }
                    }
                }
                else
                {
                    label18.Visible = false;
                    topTemp.Text = "";
                }

                if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "bottom" || GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
                {
                    label20.Visible = true;
                    if (GlobalFunc.connectedToDetector2)
                    {
                        if (GlobalFunc.det2_temp == 0.0)
                        {
                            GlobalFunc.tc.GetDetector2Temp();
                            bottomTemp.Text = GlobalFunc.det2_temp + "°C";
                        }
                    }
                    else
                    {
                        if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
                        {
                            UpdateStatusLabel("Alarm", 255, 0, 0);
                        }
                    }
                }
                else
                {
                    label20.Visible = false;
                    bottomTemp.Text = "";
                }
            }
            else
            {
                label18.Visible = false;
                label20.Visible = false;
                topTemp.Text = "";
                bottomTemp.Text = "";
            }
            //loginAsTxt.Text = GlobalFunc.rm.GetString("loginAsTxt");
            mainProfileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            //statusTxt.Text = GlobalFunc.rm.GetString("statusTxt");
            settingBtn.Text = GlobalFunc.rm.GetString("settingBtnTxt");
            loginBtn.Text = GlobalFunc.rm.GetString("loginBtnTxt");
            if (GlobalFunc.loginStatus != 0)
            {
                loginBtn.Text = GlobalFunc.rm.GetString("logoutText");
            }

            runBkOpBtn.Text = GlobalFunc.rm.GetString("runBk");
            updateBkStatus.Text = GlobalFunc.rm.GetString("updBk");
            DetectorLabel.Text = "";
            currBgStatusPanel.Text = GlobalFunc.rm.GetString("currBgStatusPanel");
            preDateLabel.Text = GlobalFunc.rm.GetString("preDateLabel");
            preLocationLabel.Text = GlobalFunc.rm.GetString("preLocationLabel");
            preCountLabel.Text = GlobalFunc.rm.GetString("preCountLabel");
            preRateLabel.Text = GlobalFunc.rm.GetString("preRateLabel");


            newBkStatus.Text = GlobalFunc.rm.GetString("newBkStatus");
            newDateLabel.Text = GlobalFunc.rm.GetString("preDateLabel");
            newLocationLabel.Text = GlobalFunc.rm.GetString("preLocationLabel");
            newCountLabel.Text = GlobalFunc.rm.GetString("preCountLabel");
            newRateLabel.Text = GlobalFunc.rm.GetString("preRateLabel");

            SetBKLabelEmpty();

            if (newAcquiredTxt.Text == "")
            {
                updateBkStatus.Enabled = false;
            }
            warningMsg.Text = "";
            clockLabel.Text = "";
            backgroundPanel.Visible = false;

            getBKPrevDataBtn.Text = GlobalFunc.rm.GetString("getBKPrevDataBtn");
            if (GlobalFunc.showGetBKDataBtn)
            {
                getBKPrevDataBtn.Visible = true;
            }

            selProfileLabel.Text = "";
            selDetectorLabel.Text = "";
            selQtyIsotope.Text = "";
            #endregion

            #region Wicked
            Mea_Location.DropDownStyle = ComboBoxStyle.DropDownList;
            Mea_FarmCode.DropDownStyle = ComboBoxStyle.DropDownList;
            Mea_PlaceOfOrigin.DropDownStyle = ComboBoxStyle.DropDownList;
            Mea_Species.DropDownStyle = ComboBoxStyle.DropDownList;
            Mea_Destination.DropDownStyle = ComboBoxStyle.DropDownList;
            Mea_SampleSize.KeyPress += acTb_KeyPress;
            Mea_Quantity.KeyPress += acTb2_KeyPress;           
            Mea_RunBtn.Text = GlobalFunc.rm.GetString("run");
            
            #endregion

            selectProfileBtn.Visible = false;
            LoadProfile();
        }

        protected void acTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            //if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && (e.KeyChar != '.'))
            //{
            //    if (e.KeyChar == '.' && !sampleSizeDot)
            //    {
            //        sampleSizeDot = true;
            //        e.Handled = true;
            //    }
            //    else
            //    {
            //        e.Handled = true;
            //    }
            //}
            //else
            //{
            //    if (e.KeyChar == '.' && !sampleSizeDot)
            //    {
            //        sampleSizeDot = true;
            //        e.Handled = true;
            //    }
            //}
            if (!Mea_SampleSize.Text.Contains("."))
            {
                sampleSizeDot = false;
            }

            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !sampleSizeDot)
            {
                //Allows only one Dot Char
                sampleSizeDot = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        protected void acTb2_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) )
            {
                e.Handled = true;
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            timer1.Stop();
            timer1.Enabled = false;
        }

        private void MainForm_Deactivate(object sender, EventArgs e)
        {
            timer1.Enabled = true;
            timer1.Tick += new EventHandler(timer1_Tick);
            timer1.Interval = 10000; // interval is in miliseconds .. current interval is for 10 sec
            timer1.Start();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("confirmExit"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), GlobalFunc.rm.GetString("noBtnTxt"), false, 0);
            if (btnClicked == "1")
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
            else
            {
                e.Cancel = true;
            }
        }

        void timer1_Tick(object sender, EventArgs e)
        {
            if (timer1.Interval == 1000) // once there window found still deactivated for perticular interval
            {
                timer1.Stop();
                MessageBox.Show("No activity");    // here you can log off the user & call the new window
            }
        }

        private void suModeBtn_Click(object sender, EventArgs e)
        {
            if (GlobalFunc.loginStatus == 0)
            {
                if (GlobalFunc.loginForm == null)
                {
                    GlobalFunc.loginForm = new LoginForm();
                }
                /*else
                {
                    GlobalFunc.loginForm.Dispose();
                    GlobalFunc.loginForm = new LoginForm();
                }*/
                GlobalFunc.loginForm.Show();
                GlobalFunc.loginForm.BringToFront();
            }
            else
            {
                if (GlobalFunc.dummyScreen == null)
                {
                    GlobalFunc.dummyScreen = new DummyScreen();
                }

                if (GlobalFunc.settingForm != null)
                {
                    GlobalFunc.settingForm.Dispose();
                }

                /*else
                {
                    GlobalFunc.dummyScreen.Dispose();
                    GlobalFunc.dummyScreen = new DummyScreen();
                }*/
                if (GlobalFunc.loginStatus == 1)
                {
                    GlobalFunc.logManager.WriteUserLog("Supervisor userID:" + GlobalFunc.userID + " Logout");
                }
                else if (GlobalFunc.loginStatus == 2)
                {
                    GlobalFunc.logManager.WriteUserLog("Operator userID:" + GlobalFunc.userID + " Logout");
                }
                else if (GlobalFunc.loginStatus == 3)
                {
                    GlobalFunc.logManager.WriteUserLog("Administrator Logout");
                }
                GlobalFunc.dummyScreen.Show();
            }

            this.Hide();
        }

        private void settingBtn_Click(object sender, EventArgs e)
        {
            if (GlobalFunc.settingForm == null)
            {
                GlobalFunc.settingForm = new SettingForm();
                GlobalFunc.settingForm.Show();
            }
            else
            {
                //GlobalFunc.settingForm = new SettingForm();
                if (GlobalFunc.settingForm.IsDisposed)
                {
                    GlobalFunc.settingForm.Dispose();
                    GlobalFunc.settingForm = new SettingForm();
                    GlobalFunc.settingForm.Show();
                }
                GlobalFunc.settingForm.BringToFront();
            }
            
        }

        public void UpdateLoginBtn_LoginAs(string loginAs)
        {
            loginBtn.Text = GlobalFunc.rm.GetString("logoutText");
            loginAsTxt.Text = loginAs;
            if (GlobalFunc.loginStatus == 1 || GlobalFunc.loginStatus == 3)
            {
                settingBtn.Visible = true;
            }
            else
            {
                settingBtn.Visible = false;
            }
            btnPanel.Enabled = true;
        }
                
        public void UpdateStatusLabel(string text, int r, int g, int b)
        {
            statusLabel.Text = text;
            statusLabel.BackColor = Color.FromArgb(r, g, b);
        }
        
        #region background
        public void SetBKIsotope(Profile loadProfile)
        {           
            newIsoTxt1.Text = loadProfile.IsoSeqList[0].Name + " " + GlobalFunc.rm.GetString("countRate"); ;
            newIsoTxt2.Text = loadProfile.IsoSeqList[1].Name + " " + GlobalFunc.rm.GetString("countRate"); ;
            newIsoTxt3.Text = loadProfile.IsoSeqList[2].Name + " " + GlobalFunc.rm.GetString("countRate"); ;
            newIsoTxt4.Text = loadProfile.IsoSeqList[3].Name + " " + GlobalFunc.rm.GetString("countRate"); ;
            if (loadProfile.IsoSeqList.Count > 4)
            {
                newIsoTxt5.Text = loadProfile.IsoSeqList[4].Name + " " + GlobalFunc.rm.GetString("countRate"); ;
            }
            if (loadProfile.IsoSeqList.Count > 5)
            {
                newIsoTxt6.Text = loadProfile.IsoSeqList[5].Name + " " + GlobalFunc.rm.GetString("countRate"); ;
            }
        }

        public void SetDetectorLabel(string name)
        {
            DetectorLabel.Text = name;
        }

        private void updateBGBtn_Click(object sender, EventArgs e)
        {
            currPanel = "background";
            CheckButtonEnable();

            //selectProfileBtn.Visible = true;
            if (GlobalFunc.connectedToDetector1 && GlobalFunc.connectedToDetector2)
            {
                connectedLabel.Text = "Dual Detector";
            }
            else if (GlobalFunc.connectedToDetector1 && !GlobalFunc.connectedToDetector2)
            {
                connectedLabel.Text = "Top Detector";
            }
            else if (!GlobalFunc.connectedToDetector1 && GlobalFunc.connectedToDetector2)
            {
                connectedLabel.Text = "Bottom Detector";
            }            

            //LoadPreviousBkStatus();
        }
        
        public void LoadPreviousBkStatus()
        {
            updateBkStatus.Enabled = false;
            int bkStatusID = 0;
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand cmd = new SqlCeCommand("select Top(1) * from [bkStatus] order by id_key desc", con);
            cmd.CommandType = CommandType.Text;
            SqlCeDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                bkStatusID = Convert.ToInt32(dr["id_key"]);
                bkAcquiredTxt.Text = dr["date"].ToString();
                bkLocationTxt.Text = dr["location"].ToString();
                bkCountTimeTxt.Text = dr["countTime"].ToString() + " Seconds";
                totalTxt.Text = dr["totalCount"].ToString() + " CPS";
            }
            dr.Close();
            newAcquiredTxt.Text = "";
            newLocationTxt.Text = "";
            newTotalTxt.Text = "";
            newCountTimeTxt.Text = "";
            SqlCeCommand detailCmd = new SqlCeCommand("select * from bkStatusDetail with (nolock) where bkStatusID = @bkStatusID order by displayOrder", con);
            detailCmd.CommandType = CommandType.Text;
            detailCmd.Parameters.AddWithValue("@bkStatusID", bkStatusID);
            SqlCeDataReader detailDr = detailCmd.ExecuteReader();            
            while (detailDr.Read())
            {
                switch (Convert.ToInt32(detailDr["displayOrder"]))
                {
                    case 1: preIsoTxt1.Text = detailDr["name"].ToString() + " " + GlobalFunc.rm.GetString("countRate"); 
                            preIsoValue1.Text = detailDr["countRate"].ToString() + " CPS";
                        break;
                    case 2: preIsoTxt2.Text = detailDr["name"].ToString() + " " + GlobalFunc.rm.GetString("countRate");
                            preIsoValue2.Text = detailDr["countRate"].ToString() + " CPS";
                        break;
                    case 3: preIsoTxt3.Text = detailDr["name"].ToString() + " " + GlobalFunc.rm.GetString("countRate");
                            preIsoValue3.Text = detailDr["countRate"].ToString() + " CPS";
                        break;
                    case 4: preIsoTxt4.Text = detailDr["name"].ToString() + " " + GlobalFunc.rm.GetString("countRate");
                            preIsoValue4.Text = detailDr["countRate"].ToString() + " CPS";
                        break;
                    case 5: preIsoTxt5.Text = detailDr["name"].ToString() + " " + GlobalFunc.rm.GetString("countRate");
                            preIsoValue5.Text = detailDr["countRate"].ToString() + " CPS";
                        break;
                    case 6: preIsoTxt6.Text = detailDr["name"].ToString() + " " + GlobalFunc.rm.GetString("countRate");
                            preIsoValue6.Text = detailDr["countRate"].ToString() + " CPS";
                        break;
                }
            }
            detailDr.Close();

            switch (detectorName.ToLower())
            {
                case "top": BKManager.beginBdgScript = GlobalFunc.topScriptSet.BackgroundBegin;
                    DetectorLabel.Text = "Top Detector"; 
                    BKManager.endBdgScript = GlobalFunc.topScriptSet.BackgroundEnd;
                    break;
                case "bottom": BKManager.beginBdgScript = GlobalFunc.bottomScriptSet.BackgroundBegin;
                    DetectorLabel.Text = "Bottom Detector";
                    BKManager.endBdgScript = GlobalFunc.bottomScriptSet.BackgroundEnd;
                    break;
                case "dual": BKManager.beginBdgScript = GlobalFunc.dualScriptSet.BackgroundBegin;
                    DetectorLabel.Text = "Dual Detector";
                    BKManager.endBdgScript = GlobalFunc.dualScriptSet.BackgroundEnd;
                    break;
            }

            newIsoValue1.Text = "";
            newIsoValue2.Text = "";
            newIsoValue3.Text = "";
            newIsoValue4.Text = "";
            newIsoValue5.Text = "";
            newIsoValue6.Text = "";
            con.Close();
        }

        BackgroundWorker bkWorker = new BackgroundWorker();

        private void runBkOpBtn_Click(object sender, EventArgs e)
        {
            GlobalFunc.loadProfile = new Profile();
            SetProfile(false);
            Mea_RunBtn.Enabled = true;
            finalResult = new List<double>();
            detected = new List<int>();
            detectorName = GlobalFunc.loadProfile.Detector;

            topCPSList.Clear();
            bottomCPSList.Clear();
            BKManager.bkList.Clear();
            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("confirmBkTxt"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), GlobalFunc.rm.GetString("noBtnTxt"), false, 0);
            if (btnClicked == "1")
            {
                detectorName = GlobalFunc.loadProfile.Detector;
              
                bgOpTimer.Dispose();
                bgOpTimer = new System.Windows.Forms.Timer();
                
                UpdateStatusLabel("Connecting...", 0, 255, 0);
                this.Enabled = false;
                #region check detector
                bool passDetector = false;
                if (DetectorLabel.Text.Contains("Top") && GlobalFunc.basicSetting.PresetDetector.ToLower() == "top")
                {
                    GlobalFunc.tc.checkDetector1Connection();
                    if (GlobalFunc.connectedToDetector1)
                    {
                        passDetector = true;
                    }
                }
                else if (DetectorLabel.Text.Contains("Bottom") && GlobalFunc.basicSetting.PresetDetector.ToLower() == "bottom")
                {
                    GlobalFunc.tc.checkDetector2Connection();
                    if (GlobalFunc.connectedToDetector2)
                    {
                        passDetector = true;
                    }
                }
                else if (DetectorLabel.Text.Contains("Dual") && GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
                {
                    GlobalFunc.tc.checkDetector1Connection();
                    GlobalFunc.tc.checkDetector2Connection();
                    if (GlobalFunc.connectedToDetector1 && GlobalFunc.connectedToDetector2)
                    {
                        passDetector = true;
                    }
                }
                #endregion
       
                if (!passDetector)
                {
                    #region fail to connect sesonsor
                    if (DetectorLabel.Text.Contains("Top"))
                    {
                        GlobalFunc.SetAlarmBox(6);
                        string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector1"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        if (buttonID == "1")
                        {
                            GlobalFunc.SetAlarmBox(0);
                            this.Enabled = true;
                            UpdateStatusLabel("Alarm", 255, 0, 0);
                        }
                    }
                    else if (DetectorLabel.Text.Contains("Bottom"))
                    {
                        GlobalFunc.SetAlarmBox(6);
                        string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector2"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        if (buttonID == "1")
                        {
                            GlobalFunc.SetAlarmBox(0);
                            this.Enabled = true;
                            UpdateStatusLabel("Alarm", 255, 0, 0);
                        }
                    }
                    else
                    {
                        GlobalFunc.SetAlarmBox(6);
                        string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector1") + System.Environment.NewLine + GlobalFunc.rm.GetString("failConnectDetector2"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        if (buttonID == "1")
                        {
                            GlobalFunc.SetAlarmBox(0);
                            this.Enabled = true;
                            UpdateStatusLabel("Alarm", 255, 0, 0);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region run background
                    try
                    {
                        GlobalFunc.SetAlarmBox(0);
                    }
                    catch { }

                    bool allowRun = true;
                    if (detectorName.ToLower() == "dual")
                    {
                        if (!File.Exists(GlobalFunc.loadProfile.RoiPath1))
                        {
                            allowRun = false;
                            string buttonID = CustomMessageBox.Show("Can't find Top Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        }
                        if (!File.Exists(GlobalFunc.loadProfile.RoiPath2))
                        {
                            allowRun = false;
                            string buttonID = CustomMessageBox.Show("Can't find Bottom Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        }
                    }
                    else if (detectorName.ToLower() == "top")
                    {
                        if (!File.Exists(GlobalFunc.loadProfile.RoiPath1))
                        {
                            allowRun = false;
                            string buttonID = CustomMessageBox.Show("Can't find Top Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        }                    
                    }
                    else if (detectorName.ToLower() == "bottom")
                    {
                        if (!File.Exists(GlobalFunc.loadProfile.RoiPath2))
                        {
                            allowRun = false;
                            string buttonID = CustomMessageBox.Show("Can't find Bottom Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        }                    
                    }

                    if (allowRun)
                    {
                        UpdateStatusLabel("Counting...", 0, 255, 0);
                        clockLabel.Text = "0";
                        countSeconds = 0;
                        Thread.Sleep(4000);
                        bgOpTimer = new System.Windows.Forms.Timer();
                        bgOpTimer.Interval = 1000;
                        bgOpTimer.Tick += bgOpTimer_Tick;
                        bgOpTimer.Start();

                        bkWorker = new BackgroundWorker();
                        bkWorker.DoWork += bkWorker_DoWork;
                        bkWorker.ProgressChanged += bkWorker_ProgressChanged;
                        bkWorker.RunWorkerCompleted += bkWorker_RunWorkerCompleted;

                        bkWorker.RunWorkerAsync();
                        bkWorker.WorkerReportsProgress = true;//啟動回報進度
                        bk_pbr.Value = 0;
                        BKManager.SetLiveTime();
                        bk_pbr.Maximum = Convert.ToInt32(BKManager.supposeSecond) + Convert.ToInt32(GlobalFunc.bufferTime) + 10;//ProgressBar上限
                        bk_pbr.Minimum = 0;//ProgressBar下限*/
                    }
                    else
                    {
                        this.Enabled = true;
                        UpdateStatusLabel("Stand by", 255, 255, 0);
                    }
                    #endregion
                }
                GlobalFunc.ResetConnectDetector();
            }
        }

        protected void bkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            BKManager.jobID = GlobalFunc.logManager.SaveCurrentJob("Background Operation");
            GlobalFunc.logManager.WriteLog("Run background operation");
            ExecuteManager exm = new ExecuteManager();
            switch (DetectorLabel.Text.ToLower())
            {
                case "top": BKManager.beginBdgScript = GlobalFunc.topScriptSet.BackgroundBegin;
                    BKManager.endBdgScript = GlobalFunc.topScriptSet.BackgroundEnd;
                    break;
                case "bottom": BKManager.beginBdgScript = GlobalFunc.bottomScriptSet.BackgroundBegin;
                    BKManager.endBdgScript = GlobalFunc.bottomScriptSet.BackgroundEnd;
                    break;
                case "dual": BKManager.beginBdgScript = GlobalFunc.dualScriptSet.BackgroundBegin;
                    BKManager.endBdgScript = GlobalFunc.dualScriptSet.BackgroundEnd;
                    break;
            }
            BKManager.SetLiveTime();
            BKManager.SetTemp(GlobalFunc.loadProfile.Qty, GlobalFunc.loadProfile.Detector, BKManager.beginBdgScript,
                GlobalFunc.loadProfile.RoiPath1, GlobalFunc.loadProfile.RoiPath2);
            //MessageBox.Show(BKManager.tempRunScript);
            exm.scriptFilePath = BKManager.tempRunScript;
            Thread runThread = new Thread(new ThreadStart(exm.RunScript));           
            runThread.Start();

            int countSleep = 0;
            while (true)
            {
                bk_pbr.Invoke(new MethodInvoker(delegate
                {
                    UpdateStatusLabel("Checking Active", 0, 255, 0);
                }));
                Thread.Sleep(1000);
                if (GlobalFunc.checkActive(thisDetector))
                {
                    break;
                }
                countSleep++;
                if (countSleep == 5)
                {
                    break;
                }
            }
            bk_pbr.Invoke(new MethodInvoker(delegate
            {
                UpdateStatusLabel("Counting...", 0, 255, 0);
            }));

            for (int i = 0; i < Convert.ToInt32(BKManager.supposeSecond) + Convert.ToInt32(GlobalFunc.bufferTime); i++)
            {
                bkWorker.ReportProgress(i, i);
                Thread.Sleep(1000);
            }

            if (runThread.IsAlive)
            {
                runThread.Abort();
            }
        
            GetBKData();
            GlobalFunc.logManager.WriteLog("End background operation");
        }

        List<string> topCPSList = new List<string>();
        List<string> bottomCPSList = new List<string>();
        public void GetBKData()
        {
            try
            {
                GlobalFunc.logManager.WriteLog("Get background operation data");
                int countTryFile = 0;
                while (BKManager.bkList.Count == 0)
                {
                    int countCheck = 0;
                    while (!GlobalFunc.checkActive(selDetectorLabel.Text))
                    {
                        Thread.Sleep(1000);
                        countCheck++;
                        if (countCheck == 20)
                        {
                            break;
                        }
                        if (GlobalFunc.checkActive(selDetectorLabel.Text))
                        {
                            break;
                        }
                    }
                    ExecuteManager exm = new ExecuteManager();
                    BKManager.SetTemp(GlobalFunc.loadProfile.Qty, GlobalFunc.loadProfile.Detector, BKManager.endBdgScript, GlobalFunc.loadProfile.RoiPath1, GlobalFunc.loadProfile.RoiPath2);
                    exm.scriptFilePath = BKManager.tempRunScript;
                    Thread runThread = new Thread(new ThreadStart(exm.RunScript));
                    runThread.Start();

                    clockLabel.Invoke(new MethodInvoker
                           (delegate
                           {
                               clockLabel.Text = "Updating";
                           }
                           )
                    );

                    countSeconds = 0;
                    bk_pbr.Invoke(new MethodInvoker
                           (delegate
                           {                              
                               bk_pbr.Value = 0;
                               bk_pbr.Maximum = GlobalFunc.bufferTime;
                               UpdateStatusLabel("Data Analyzing", 255, 255, 0);
                               //MessageBox.Show(DetectorLabel.Text);
                               //MessageBox.Show(selDetectorLabel.Text);
                           }
                           )
                       );
                    
                    for (int i = 0; i < GlobalFunc.bufferTime; i++)  //20
                    {
                        bkWorker.ReportProgress(i, i);
                        Thread.Sleep(1000);
                    }

                    int countSleep = 0;
                    while (true)
                    {
                        bk_pbr.Invoke(new MethodInvoker(delegate
                        {
                            UpdateStatusLabel("Checking Active", 255, 128, 0);
                        }));
                        Thread.Sleep(1000);
                        if (GlobalFunc.checkActive(thisDetector))
                        {
                            break;
                        }
                        countSleep++;
                        if (countSleep == 5)
                        {
                            break;
                        }
                    }

                    if (runThread.IsAlive)
                    {
                        runThread.Abort();
                    }

                    if (selDetectorLabel.Text.Contains("Top"))
                    {
                        if (File.Exists(GlobalFunc.topScriptSet.BackgrounData))
                        {
                            List<Roi> roiList1 = GlobalFunc.GetRoiData(GlobalFunc.topScriptSet.BackgrounData);
                            if (roiList1.Count > 0)
                            {
                                topCPSList = BKManager.CalBk(roiList1, ref BKManager.dt, ref BKManager.lifeTime, 1);
                                BKManager.bkList = topCPSList;
                                GlobalFunc.logManager.WriteLog("Top Detector Result");
                            }
                        }
                    }
                    else if (selDetectorLabel.Text.Contains("Bottom"))
                    {
                        List<Roi> roiList2 = GlobalFunc.GetRoiData(GlobalFunc.bottomScriptSet.BackgrounData);
                        if (roiList2.Count > 0)
                        {
                            bottomCPSList = BKManager.CalBk(roiList2, ref BKManager.dt, ref BKManager.lifeTime, 1);
                            BKManager.bkList = bottomCPSList;
                            GlobalFunc.logManager.WriteLog("Bottom Detector Result");
                        }
                    }
                    else if (selDetectorLabel.Text.Contains("Dual"))
                    {
                        List<Roi> roiList1 = GlobalFunc.GetRoiData(GlobalFunc.topScriptSet.BackgrounData);
                        List<Roi> roiList2 = GlobalFunc.GetRoiData(GlobalFunc.bottomScriptSet.BackgrounData);
                        if (roiList1.Count > 0)
                        {
                            topCPSList = BKManager.CalBk(roiList1, ref BKManager.dt, ref BKManager.lifeTime, 1);
                            bottomCPSList = BKManager.CalBk(roiList2, ref BKManager.dt, ref BKManager.lifeTime, 1);
                            BKManager.bkList = BKManager.CalDualBk(roiList1, roiList2, ref BKManager.dt, ref BKManager.lifeTime, 1);
                            GlobalFunc.logManager.WriteLog("Dual Detector");
                        }
                    }
                    if (BKManager.bkList.Count == 0)
                    {
                        countTryFile++;
                    }
                    else
                    {
                        break;
                    }

                    GlobalFunc.logManager.WriteLog("Background Work getting result count " +  BKManager.bkList.Count.ToString());

                    if (countTryFile % 3 == 0)
                    {
                        string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("tryAgain"), GlobalFunc.rm.GetString("noticeMsg"),
                                    GlobalFunc.rm.GetString("yesBtnTxt"), GlobalFunc.rm.GetString("noBtnTxt"), false, 0);
                        if (btnClicked == "1")
                        {

                        }
                        else if (btnClicked == "2")
                        {
                            break;
                        }
                    }
                }
                GlobalFunc.logManager.WriteLog("End background operation data");
            }
            catch(Exception ex) {
                MessageBox.Show("Error please retry again" + System.Environment.NewLine + ex.Message);
            }
        }

        protected void bkWorker_GetDataWork(object sender, DoWorkEventArgs e)
        {
            GetBKData();
        }

        protected void bkWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            bk_pbr.Value = e.ProgressPercentage;            
        }

        protected void bkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bgOpTimer.Stop();
            clockLabel.Text = "";
            bk_pbr.Value = 0;
            double totalCPS = 0.0;
            //write to label
            if (BKManager.bkList.Count > 0)
            {
                newIsoTxt1.Text = GlobalFunc.loadProfile.IsoSeqList[0].Name + " " + GlobalFunc.rm.GetString("countRate");
                newIsoValue1.Text = BKManager.bkList[0];
                totalCPS += Convert.ToDouble(BKManager.bkList[0].Replace(" CPS", ""));
                
                newIsoTxt2.Text = GlobalFunc.loadProfile.IsoSeqList[1].Name + " " + GlobalFunc.rm.GetString("countRate");
                newIsoValue2.Text = BKManager.bkList[1];
                totalCPS += Convert.ToDouble(BKManager.bkList[1].Replace(" CPS", ""));
                
                newIsoTxt3.Text = GlobalFunc.loadProfile.IsoSeqList[2].Name + " " + GlobalFunc.rm.GetString("countRate");
                newIsoValue3.Text = BKManager.bkList[2];
                totalCPS += Convert.ToDouble(BKManager.bkList[2].Replace(" CPS", ""));
                
                newIsoTxt4.Text = GlobalFunc.loadProfile.IsoSeqList[3].Name + " " + GlobalFunc.rm.GetString("countRate");
                newIsoValue4.Text = BKManager.bkList[3];
                totalCPS += Convert.ToDouble(BKManager.bkList[3].Replace(" CPS", ""));

                if (GlobalFunc.loadProfile.IsoSeqList.Count > 4)
                {
                    newIsoTxt5.Text = GlobalFunc.loadProfile.IsoSeqList[4].Name + " " + GlobalFunc.rm.GetString("countRate");
                    newIsoValue5.Text = BKManager.bkList[4];
                    totalCPS += Convert.ToDouble(BKManager.bkList[4].Replace(" CPS", ""));
                }
                if (GlobalFunc.loadProfile.IsoSeqList.Count > 5)
                {
                    newIsoTxt6.Text = GlobalFunc.loadProfile.IsoSeqList[5].Name + " " + GlobalFunc.rm.GetString("countRate");
                    newIsoValue6.Text = BKManager.bkList[5];
                    totalCPS += Convert.ToDouble(BKManager.bkList[5].Replace(" CPS", ""));
                }
                               
                newAcquiredTxt.Text = BKManager.dt.ToShortDateString() + " " + BKManager.dt.ToShortTimeString();
                newCountTimeTxt.Text = BKManager.lifeTime.ToString() + " Seconds";
                //newTotalTxt.Text = BKManager.bkList[BKManager.bkList.Count - 1];
                newTotalTxt.Text = totalCPS + " CPS";

                //Location location = (Location)selBkLocation.SelectedItem;
                //newLocationTxt.Text = location.Code;
                updateBkStatus.Enabled = true;
                UpdateStatusLabel("Standby", 255, 255, 0);                
                GlobalFunc.logManager.UpdateJobStatus(BKManager.jobID);
                getBKPrevDataBtn.Visible = false;

                selProfileLabel.Text = "";
                selDetectorLabel.Text = "";
                selQtyIsotope.Text = "";

                //mainProfileComboBox.SelectedIndex = -1;
                int index = mainProfileComboBox.SelectedIndex;
                mainProfileComboBox.SelectedIndex = index;

                runBkOpBtn.Enabled = true;
                updateBkStatus.Enabled = true;

                //compare value
                /*double originValue1 = 0.0, originValue2 = 0.0, originValue3 = 0.0, originValue4 = 0.0, originValue5 = 0.0, originValue6 = 0.0;
                double newValue1 = 0.0, newValue2 = 0.0, newValue3 = 0.0, newValue4 = 0.0, newValue5 = 0.0, newValue6 = 0.0;
                originValue1 = Convert.ToDouble(preIsoValue1.Text.Replace(" CPS", ""));
                originValue2 = Convert.ToDouble(preIsoValue2.Text.Replace(" CPS", ""));
                originValue3 = Convert.ToDouble(preIsoValue3.Text.Replace(" CPS", ""));
                originValue4 = Convert.ToDouble(preIsoValue4.Text.Replace(" CPS", ""));

                if (preIsoValue5.Text != "")
                {
                    originValue5 = Convert.ToDouble(preIsoValue5.Text.Replace(" CPS", ""));
                }
                if (preIsoValue6.Text != "")
                {
                    originValue6 = Convert.ToDouble(preIsoValue6.Text.Replace(" CPS", ""));
                }

                newValue1 = Convert.ToDouble(newIsoValue1.Text.Replace(" CPS", ""));
                newValue2 = Convert.ToDouble(newIsoValue2.Text.Replace(" CPS", ""));
                newValue3 = Convert.ToDouble(newIsoValue3.Text.Replace(" CPS", ""));
                newValue4 = Convert.ToDouble(newIsoValue4.Text.Replace(" CPS", ""));

                if (newIsoValue5.Text != "")
                {
                    newValue5 = Convert.ToDouble(newIsoValue5.Text.Replace(" CPS", ""));
                }
                if (newIsoValue6.Text != "")
                {
                    newValue6 = Convert.ToDouble(newIsoValue6.Text.Replace(" CPS", ""));
                }

                string compareMsg = "";
                if (compareBkCount(newValue1, originValue1) != "")
                {
                    compareMsg += preIsoTxt1.Text + " new value " + compareBkCount(newValue1, originValue1) + System.Environment.NewLine;
                }
                if (compareBkCount(newValue2, originValue2) != "")
                {
                    compareMsg += preIsoTxt2.Text + " new value " + compareBkCount(newValue2, originValue2) + System.Environment.NewLine;
                }
                if (compareBkCount(newValue3, originValue3) != "")
                {
                    compareMsg += preIsoTxt3.Text + " new value " + compareBkCount(newValue3, originValue3) + System.Environment.NewLine;
                }
                if (compareBkCount(newValue4, originValue4) != "")
                {
                    compareMsg += preIsoTxt4.Text + " new value " + compareBkCount(newValue4, originValue4) + System.Environment.NewLine;
                }
                if (newIsoValue5.Text != "")
                {
                    if (compareBkCount(newValue5, originValue5) != "")
                    {
                        compareMsg += preIsoTxt5.Text + " new value " + compareBkCount(newValue5, originValue5) + System.Environment.NewLine;
                    }
                }
                if (newIsoValue6.Text != "")
                {
                    if (compareBkCount(newValue6, originValue6) != "")
                    {
                        compareMsg += preIsoTxt6.Text + " new value " + compareBkCount(newValue6, originValue6) + System.Environment.NewLine;
                    }
                }*/

                double newTotalValue = 0.0, originTotalValue = 0.0;
                originTotalValue = Convert.ToDouble(totalTxt.Text.Replace(" CPS", ""));
                newTotalValue = Convert.ToDouble(newTotalTxt.Text.Replace(" CPS", ""));
                string compareMsg = "";
                if (compareBkCount(newTotalValue, originTotalValue) != "")
                {
                    compareMsg += compareBkCount(newTotalValue, originTotalValue) + System.Environment.NewLine;
                }        
                if (compareMsg != "")
                {
                    string btnClicked = CustomMessageBox.Show(compareMsg, GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
            }
            else
            {
               
            }
            this.Enabled = true;
        }

        public string compareBkCount(double newValue, double originValue)
        {
            string showMsg = "";
            if (newValue == 0 )
            {
                showMsg = "New Total Count Rate is 0, Detector failed.";
            }
            else
            {
                double child = newValue - originValue;
                double result = (child / originValue) * 100;
                if (result < -15)
                {
                    showMsg = "New Total Background Count Rate " + "< -15%";
                }
                else if (result > 15)
                {
                    showMsg = "New Total Background Count Rate " + "> 15%";
                }
            }
            return showMsg;
        }

        private void updateBkStatus_Click(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();

            SqlCeCommand insertCmd = new SqlCeCommand("insert into bkStatus (date, location, totalCount, countTime) values (@date, @location, @totalCount, @countTime)", con);
            insertCmd.CommandType = CommandType.Text;
            insertCmd.Parameters.AddWithValue("@date", newAcquiredTxt.Text);
            bkAcquiredTxt.Text = newAcquiredTxt.Text;
            newAcquiredTxt.Text = "";
            insertCmd.Parameters.AddWithValue("@location", newLocationTxt.Text.Replace("CPS", "").Trim());
            insertCmd.Parameters.AddWithValue("@totalCount", newTotalTxt.Text.Replace("CPS", "").Trim());
            totalTxt.Text = newTotalTxt.Text;
            newTotalTxt.Text = "";
            insertCmd.Parameters.AddWithValue("@countTime", newCountTimeTxt.Text.Replace("Seconds", "").Trim());
            bkCountTimeTxt.Text = newCountTimeTxt.Text;
            newCountTimeTxt.Text = "";
            insertCmd.ExecuteNonQuery();
            insertCmd.Dispose();

            int bkStatusID = 0;

            SqlCeCommand idCmd = new SqlCeCommand("select top 1 id_key from bkStatus order by id_key desc", con);
            idCmd.CommandType = CommandType.Text;
            bkStatusID = Convert.ToInt32(idCmd.ExecuteScalar());
            idCmd.Dispose();

            for (int i = 0; i < GlobalFunc.loadProfile.IsoSeqList.Count; i++)
            {
                SqlCeCommand insertDetailCmd = new SqlCeCommand(@"insert bkStatusDetail (name, displayOrder, bkStatusID, countRate) 
                                                        values (@name, @displayOrder, @bkStatusID, @countRate)", con);
                insertDetailCmd.CommandType = CommandType.Text;
                insertDetailCmd.Parameters.AddWithValue("@name", GlobalFunc.loadProfile.IsoSeqList[i].Name);
                insertDetailCmd.Parameters.AddWithValue("@displayOrder", (i + 1));
                insertDetailCmd.Parameters.AddWithValue("@bkStatusID", bkStatusID);
                switch(i)
                {
                    case 0: insertDetailCmd.Parameters.AddWithValue("@countRate", newIsoValue1.Text.Replace("CPS", "").Trim());
                        preIsoValue1.Text = newIsoValue1.Text;
                        newIsoValue1.Text = "";
                        break;
                    case 1: insertDetailCmd.Parameters.AddWithValue("@countRate", newIsoValue2.Text.Replace("CPS", "").Trim());
                        preIsoValue2.Text = newIsoValue2.Text;
                        newIsoValue2.Text = "";
                        break;
                    case 2: insertDetailCmd.Parameters.AddWithValue("@countRate", newIsoValue3.Text.Replace("CPS", "").Trim());
                        preIsoValue3.Text = newIsoValue3.Text;
                        newIsoValue3.Text = "";
                        break;
                    case 3: insertDetailCmd.Parameters.AddWithValue("@countRate", newIsoValue4.Text.Replace("CPS", "").Trim());
                        preIsoValue4.Text = newIsoValue4.Text;
                        newIsoValue4.Text = "";
                        break;
                    case 4: insertDetailCmd.Parameters.AddWithValue("@countRate", newIsoValue5.Text.Replace("CPS", "").Trim());
                        preIsoValue5.Text = newIsoValue5.Text;
                        newIsoValue5.Text = "";
                        break;
                    case 5: insertDetailCmd.Parameters.AddWithValue("@countRate", newIsoValue6.Text.Replace("CPS", "").Trim());
                        preIsoValue6.Text = newIsoValue6.Text;
                        newIsoValue6.Text = "";
                        break;
                    default: insertDetailCmd.Parameters.AddWithValue("@countRate", "");
                        break;
                }
                insertDetailCmd.ExecuteNonQuery();
                insertDetailCmd.Dispose();
            }
            con.Close();

            if (File.Exists(@"C:\LCMS\Profile\" + profile.FileName.Replace(".txt", "") + ".txt"))
            {
                string line;
                string path = @"C:\LCMS\Profile\" + profile.FileName.Replace(".txt", "") + ".txt";
                StringBuilder sb = new StringBuilder();
                using (StreamReader reader = File.OpenText(path))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("Date:"))
                        {
                            sb.AppendLine("Date:" + BKManager.dt.ToString());
                        }
                        else if (line.Contains("Live Time:"))
                        {
                            sb.AppendLine("Live Time:" + BKManager.lifeTime);
                        }
                        else if (line.Contains("Background Top CPS:"))
                        {
                            string innerText = "";
                            for (int i = 0; i < topCPSList.Count; i++)
                            {
                                innerText += topCPSList[i].Replace(" CPS", "") + ",";
                            }
                            if (innerText.Length > 0)
                            {
                                innerText = innerText.Remove(innerText.Length - 1);
                            }
                            sb.AppendLine("Background Top CPS:" + innerText);
                        }
                        else if (line.Contains("Background Bottom CPS:"))
                        {
                            string innerText = "";
                            for (int i = 0; i < bottomCPSList.Count; i++)
                            {
                                innerText += bottomCPSList[i].Replace(" CPS", "") + ",";
                            }
                            if (innerText.Length > 0)
                            {
                                innerText = innerText.Remove(innerText.Length - 1);
                            }
                            sb.AppendLine("Background Bottom CPS:" + innerText);
                        }
                        else
                        {
                            sb.AppendLine(line);
                        }
                    }
                }
                try
                {
                    if (File.Exists(@"C:\LCMS\Profile\" + profile.FileName.Replace(".txt", "") + ".txt"))
                    {
                        File.Delete(@"C:\LCMS\Profile\" + profile.FileName.Replace(".txt", "") + ".txt");
                    }
                }
                catch (Exception ex)
                {

                }
                File.WriteAllText(@"C:\LCMS\Profile\" + profile.FileName.Replace(".txt", "") + ".txt", sb.ToString());
            }

            runBkOpBtn.Enabled = false;
            updateBkStatus.Enabled = false;
            mainProfileComboBox.SelectedIndex = -1;
            
            //LoadPreviousBkStatus();
        }

        private void backgroundBtn_Click(object sender, EventArgs e)
        {           
            if (GlobalFunc.loginStatus == 0)
            {
                if (GlobalFunc.loginForm == null)
                {
                    GlobalFunc.loginForm = new LoginForm();
                }
                GlobalFunc.loginForm.Show();
                this.Hide();
            }
            else
            {
                currPanel = "";
                mainProfileComboBox.SelectedIndex = -1;                
                CheckButtonEnable();
                backgroundPanel.Visible = true;
            }
        }

        #endregion

        #region warm up
        string thisDetector = "Top";
        private void warmUpBtn_Click(object sender, EventArgs e)
        {
            currPanel = "warmup";
            CheckButtonEnable();
            
            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("WarmUpTxt"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), GlobalFunc.rm.GetString("noBtnTxt"), false, 0);

            if (btnClicked == "1")
            {
                this.Enabled = false;
                UpdateStatusLabel("Connecting...", 0, 255, 0);                
                bool passDetector = false;
                
                GlobalFunc.tc.checkDetector1Connection();
                GlobalFunc.tc.checkDetector2Connection();
                              
                if (GlobalFunc.connectedToDetector1 && GlobalFunc.connectedToDetector2 && GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
                {
                    thisDetector = "Dual";
                    passDetector = true;
                    GlobalFunc.warmupBeginScript = GlobalFunc.dualScriptSet.WarmUpBegin;
                    GlobalFunc.warmupEndScript = GlobalFunc.dualScriptSet.WarmUpEnd;
                }
                else if (GlobalFunc.connectedToDetector1 && GlobalFunc.basicSetting.PresetDetector.ToLower() == "top")
                {
                    thisDetector = "Top";
                    passDetector = true;
                    GlobalFunc.warmupBeginScript = GlobalFunc.topScriptSet.WarmUpBegin;
                    GlobalFunc.warmupEndScript = GlobalFunc.topScriptSet.WarmUpEnd;
                }
                else if (GlobalFunc.connectedToDetector2 && GlobalFunc.basicSetting.PresetDetector.ToLower() == "bottom")
                {
                    thisDetector = "Bottom";
                    passDetector = true;
                    GlobalFunc.warmupBeginScript = GlobalFunc.bottomScriptSet.WarmUpBegin;
                    GlobalFunc.warmupEndScript = GlobalFunc.bottomScriptSet.WarmUpEnd;
                }
      
                if (!passDetector)
                {
                    #region fail to connect sesonsor
                    if (DetectorLabel.Text.Contains("Top"))
                    {
                        GlobalFunc.SetAlarmBox(6);
                        string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector1"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        if (buttonID == "1")
                        {
                            GlobalFunc.SetAlarmBox(0);
                            this.Enabled = true;
                            UpdateStatusLabel("Alarm", 255, 0, 0);
                        }
                    }
                    else if (DetectorLabel.Text.Contains("Bottom"))
                    {
                        GlobalFunc.SetAlarmBox(6);
                        string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector2"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        if (buttonID == "1")
                        {
                            GlobalFunc.SetAlarmBox(0);
                            this.Enabled = true;
                            UpdateStatusLabel("Alarm", 255, 0, 0);
                        }
                    }
                    else
                    {
                        GlobalFunc.SetAlarmBox(6);
                        string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector1") + System.Environment.NewLine + GlobalFunc.rm.GetString("failConnectDetector2"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        if (buttonID == "1")
                        {
                            GlobalFunc.SetAlarmBox(0);
                            this.Enabled = true;
                            UpdateStatusLabel("Alarm", 255, 0, 0);
                        }
                    }
                    #endregion
                }
                else
                {
                    #region run background    
                    try
                    {
                        GlobalFunc.SetAlarmBox(0);
                    }
                    catch { }
                    UpdateStatusLabel("Warm up", 255, 128, 0);
                    clockLabel.Text = "0";
                    countSeconds = 0;
                    Thread.Sleep(4000);
                    bgOpTimer = new System.Windows.Forms.Timer();
                    bgOpTimer.Interval = 1000;
                    bgOpTimer.Tick += bgOpTimer_Tick;
                    bgOpTimer.Start();

                    bkWorker = new BackgroundWorker();
                    bkWorker.DoWork += WarmUpWork_DoWork;
                    bkWorker.ProgressChanged += WarmUpWork_ProgressChanged;
                    bkWorker.RunWorkerCompleted += WarmUpWork_RunWorkerCompleted;

                    bkWorker.RunWorkerAsync();
                    bkWorker.WorkerReportsProgress = true;//啟動回報進度
                    bk_pbr.Value = 0;   
                    bk_pbr.Maximum = Convert.ToInt32(GlobalFunc.warmupTime * 60) + GlobalFunc.bufferTime;//ProgressBar上限
                    bk_pbr.Minimum = 0;//ProgressBar下限*/

                    #endregion
                }                
            }
        }

        protected void WarmUpWork_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Enabled = false;
            BKManager.SetWarmUpTime();
            BKManager.SetTemp(0, thisDetector, GlobalFunc.warmupBeginScript, "", "");
            GlobalFunc.logManager.WriteLog("Run Warm up operation");
           
            ExecuteManager exm = new ExecuteManager();
            exm.scriptFilePath = BKManager.tempRunScript;

            //Thread runThread = new Thread(new ThreadStart(exm.RunScript));
            Thread runThread = new Thread(new ThreadStart(exm.ReadJobFileSend));
            runThread.Start();

            for (int i = 0; i < GlobalFunc.warmupTime * 60 + GlobalFunc.bufferTime; i++)
            {
                bkWorker.ReportProgress(i, i);
                Thread.Sleep(1000);
            }
            if (runThread.IsAlive)
            {
                runThread.Abort();
            }
            int countCheck = 0;
            while (!GlobalFunc.checkActive(thisDetector))
            {
                Thread.Sleep(1000);
                countCheck++;
                if (countCheck == 20)
                {
                    break;
                }
                if (GlobalFunc.checkActive(thisDetector))
                {
                    break;
                }
            }
            exm = new ExecuteManager();
            BKManager.SetTemp(0, thisDetector, GlobalFunc.warmupEndScript, "", "");
            exm.scriptFilePath = BKManager.tempRunScript;
            Thread endThread = new Thread(new ThreadStart(exm.RunScript));
            endThread.Start();

            bk_pbr.Invoke(new MethodInvoker(delegate { bk_pbr.Value = 0; bk_pbr.Maximum = GlobalFunc.bufferTime; UpdateStatusLabel("Data Analyzing", 255, 255, 0); }));

            for (int i = 0; i < GlobalFunc.bufferTime; i++)
            {
                bkWorker.ReportProgress(i, i);
                Thread.Sleep(1000);
            }

            int countSleep = 0;
            while (true)
            {
                bk_pbr.Invoke(new MethodInvoker(delegate
                {
                    UpdateStatusLabel("Checking Active", 255, 128, 0);
                }));
                Thread.Sleep(1000);
                if (GlobalFunc.checkActive(thisDetector))
                {
                    break;
                }
                countSleep++;
                if (countSleep == 5)
                {
                    break;
                }
            }

            if (endThread.IsAlive)
            {
                endThread.Abort();
            }

            BKManager.SetMeasureTime();
            GlobalFunc.logManager.WriteLog("End Warm up operation");

            bk_pbr.Invoke(new MethodInvoker(delegate
            {
                bgOpTimer.Stop();
                clockLabel.Text = "";
                bk_pbr.Value = 0;
                this.Enabled = true;
                UpdateStatusLabel("Standby", 255, 255, 0);
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("complete"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }));
        }

        protected void WarmUpWork_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            bk_pbr.Value = e.ProgressPercentage;            
        }

        protected void WarmUpWork_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bgOpTimer.Stop();
            clockLabel.Text = "";
            bk_pbr.Value = 0;
            this.Enabled = true;
            UpdateStatusLabel("Standby", 255, 255, 0);
            string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("complete"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
        }
        #endregion

        private void measureTxt_Click(object sender, EventArgs e)
        {
            if (GlobalFunc.loginStatus == 0)
            {
                if (GlobalFunc.loginForm == null)
                {
                    GlobalFunc.loginForm = new LoginForm();
                }
                GlobalFunc.loginForm.Show();
                this.Hide();
            }
            else
            {
                currPanel = "measure";
                mainProfileComboBox.SelectedIndex = -1;
                CheckButtonEnable();
                backgroundPanel.Visible = false;

                #region Get ddl data (Wicked)
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                // location
                SqlCeCommand cmd = new SqlCeCommand("select * from location order by id_key", con);
                cmd.CommandType = CommandType.Text;
                SqlCeDataReader dr = cmd.ExecuteReader();
                List<Location> sampleLocationList = new List<Location>();
                while (dr.Read())
                {
                    Location parameter = new Location();
                    parameter.Id_key = Convert.ToInt32(dr["id_key"]);
                    parameter.Code = dr["code"].ToString();
                    parameter.Description = dr["description"].ToString();

                    DisplayText dt = new DisplayText();
                    dt.code = parameter.Code;
                    dt.description = parameter.Description;
                    parameter.DisplayText = dt;

                    sampleLocationList.Add(parameter);
                }
                dr.Close();

                Mea_Location.DataSource = sampleLocationList;
                Mea_Location.DisplayMember = "Description";
                Mea_Location.ValueMember = "Id_key";
                Mea_Location.SelectedIndex = -1;
                // Farm Code
                cmd = new SqlCeCommand("select * from farmCode with (nolock) order by id_key", con);
                cmd.CommandType = CommandType.Text;
                dr = cmd.ExecuteReader();
                List<FarmCode> sampleFarmCodeList = new List<FarmCode>();
                while (dr.Read())
                {
                    FarmCode parameter = new FarmCode();
                    parameter.Id_key = Convert.ToInt32(dr["id_key"]);
                    parameter.Code = dr["code"].ToString();
                    parameter.Description = dr["description"].ToString();

                    DisplayText dt = new DisplayText();
                    dt.code = parameter.Code;
                    dt.description = parameter.Description;
                    parameter.DisplayText = dt;

                    sampleFarmCodeList.Add(parameter);
                }
                dr.Close();

                Mea_FarmCode.DataSource = sampleFarmCodeList;
                Mea_FarmCode.DisplayMember = "Description";
                Mea_FarmCode.ValueMember = "Id_key";
                Mea_FarmCode.SelectedIndex = -1;
                // Place Of Origin
                cmd = new SqlCeCommand("select * from origin with (nolock) order by Id", con);
                cmd.CommandType = CommandType.Text;
                dr = cmd.ExecuteReader();
                List<PlaceOfOrigin> samplePlaceOfOriginList = new List<PlaceOfOrigin>();
                while (dr.Read())
                {
                    PlaceOfOrigin parameter = new PlaceOfOrigin();
                    parameter.Id_key = Convert.ToInt32(dr["Id"]);
                    parameter.EngName = dr["engname"].ToString();
                    parameter.ChiName = dr["chiname"].ToString();

                    DisplayText dt = new DisplayText();
                    dt.code = parameter.EngName;
                    dt.description = parameter.ChiName;
                    parameter.DisplayText = dt;

                    samplePlaceOfOriginList.Add(parameter);
                }
                dr.Close();

                Mea_PlaceOfOrigin.DataSource = samplePlaceOfOriginList;
                Mea_PlaceOfOrigin.DisplayMember = "DisplayText";
                Mea_PlaceOfOrigin.ValueMember = "Id_key";
                Mea_PlaceOfOrigin.SelectedIndex = -1;
                // Species
                cmd = new SqlCeCommand("select * from species with (nolock) order by id_key", con);
                cmd.CommandType = CommandType.Text;
                dr = cmd.ExecuteReader();
                List<Species> sampleSpeciesList = new List<Species>();
                while (dr.Read())
                {
                    Species parameter = new Species();
                    parameter.Id_key = Convert.ToInt32(dr["id_key"]);
                    parameter.Code = dr["code"].ToString();
                    parameter.Description = dr["description"].ToString();

                    DisplayText dt = new DisplayText();
                    dt.code = parameter.Code;
                    dt.description = parameter.Description;
                    parameter.DisplayText = dt;

                    sampleSpeciesList.Add(parameter);
                }
                dr.Close();

                Mea_Species.DataSource = sampleSpeciesList;
                Mea_Species.DisplayMember = "Description";
                Mea_Species.ValueMember = "Id_key";
                Mea_Species.SelectedIndex = -1;
                // Destination
                cmd = new SqlCeCommand("select * from destination with (nolock) order by id_key", con);
                cmd.CommandType = CommandType.Text;
                dr = cmd.ExecuteReader();
                List<Destination> sampleDestinationList = new List<Destination>();
                while (dr.Read())
                {
                    Destination parameter = new Destination();
                    parameter.Id_key = Convert.ToInt32(dr["id_key"]);
                    parameter.Code = dr["code"].ToString();
                    parameter.Description = dr["description"].ToString();

                    DisplayText dt = new DisplayText();
                    dt.code = parameter.Code;
                    dt.description = parameter.Description;
                    parameter.DisplayText = dt;

                    sampleDestinationList.Add(parameter);
                }
                dr.Close();

                Mea_Destination.DataSource = sampleDestinationList;
                Mea_Destination.DisplayMember = "Description";
                Mea_Destination.ValueMember = "Id_key";
                Mea_Destination.SelectedIndex = -1;
                con.Close();
                #endregion
            }
        }

        public void RefreshData()
        {
            profileLabel.Visible = true;
            mainProfileComboBox.Visible = true;
            backgroundPanel.Visible = false;
            //selectProfileBtn.Visible = true;
            measurePanel.Visible = true;
            #region Get ddl data (Wicked)
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            // location
            SqlCeCommand cmd = new SqlCeCommand("select * from location order by id_key", con);
            cmd.CommandType = CommandType.Text;
            SqlCeDataReader dr = cmd.ExecuteReader();
            List<Location> sampleLocationList = new List<Location>();
            while (dr.Read())
            {
                Location parameter = new Location();
                parameter.Id_key = Convert.ToInt32(dr["id_key"]);
                parameter.Code = dr["code"].ToString();
                parameter.Description = dr["description"].ToString();

                DisplayText dt = new DisplayText();
                dt.code = parameter.Code;
                dt.description = parameter.Description;
                parameter.DisplayText = dt;

                sampleLocationList.Add(parameter);
            }
            dr.Close();

            Mea_Location.DataSource = sampleLocationList;
            Mea_Location.DisplayMember = "Description";
            Mea_Location.ValueMember = "Id_key";
            Mea_Location.SelectedIndex = -1;
            // Farm Code
            cmd = new SqlCeCommand("select * from farmCode with (nolock) order by id_key", con);
            cmd.CommandType = CommandType.Text;
            dr = cmd.ExecuteReader();
            List<FarmCode> sampleFarmCodeList = new List<FarmCode>();
            while (dr.Read())
            {
                FarmCode parameter = new FarmCode();
                parameter.Id_key = Convert.ToInt32(dr["id_key"]);
                parameter.Code = dr["code"].ToString();
                parameter.Description = dr["description"].ToString();

                DisplayText dt = new DisplayText();
                dt.code = parameter.Code;
                dt.description = parameter.Description;
                parameter.DisplayText = dt;

                sampleFarmCodeList.Add(parameter);
            }
            dr.Close();

            Mea_FarmCode.DataSource = sampleFarmCodeList;
            Mea_FarmCode.DisplayMember = "Description";
            Mea_FarmCode.ValueMember = "Id_key";
            Mea_FarmCode.SelectedIndex = -1;
            // Place Of Origin
            cmd = new SqlCeCommand("select * from origin with (nolock) order by Id", con);
            cmd.CommandType = CommandType.Text;
            dr = cmd.ExecuteReader();
            List<PlaceOfOrigin> samplePlaceOfOriginList = new List<PlaceOfOrigin>();
            while (dr.Read())
            {
                PlaceOfOrigin parameter = new PlaceOfOrigin();
                parameter.Id_key = Convert.ToInt32(dr["Id"]);
                parameter.EngName = dr["engname"].ToString();
                parameter.ChiName = dr["chiname"].ToString();

                DisplayText dt = new DisplayText();
                dt.code = parameter.EngName;
                dt.description = parameter.ChiName;
                parameter.DisplayText = dt;

                samplePlaceOfOriginList.Add(parameter);
            }
            dr.Close();

            Mea_PlaceOfOrigin.DataSource = samplePlaceOfOriginList;
            Mea_PlaceOfOrigin.DisplayMember = "DisplayText";
            Mea_PlaceOfOrigin.ValueMember = "Id_key";
            Mea_PlaceOfOrigin.SelectedIndex = -1;
            // Species
            cmd = new SqlCeCommand("select * from species with (nolock) order by id_key", con);
            cmd.CommandType = CommandType.Text;
            dr = cmd.ExecuteReader();
            List<Species> sampleSpeciesList = new List<Species>();
            while (dr.Read())
            {
                Species parameter = new Species();
                parameter.Id_key = Convert.ToInt32(dr["id_key"]);
                parameter.Code = dr["code"].ToString();
                parameter.Description = dr["description"].ToString();

                DisplayText dt = new DisplayText();
                dt.code = parameter.Code;
                dt.description = parameter.Description;
                parameter.DisplayText = dt;

                sampleSpeciesList.Add(parameter);
            }
            dr.Close();

            Mea_Species.DataSource = sampleSpeciesList;
            Mea_Species.DisplayMember = "Description";
            Mea_Species.ValueMember = "Id_key";
            Mea_Species.SelectedIndex = -1;
            // Destination
            cmd = new SqlCeCommand("select * from destination with (nolock) order by id_key", con);
            cmd.CommandType = CommandType.Text;
            dr = cmd.ExecuteReader();
            List<Destination> sampleDestinationList = new List<Destination>();
            while (dr.Read())
            {
                Destination parameter = new Destination();
                parameter.Id_key = Convert.ToInt32(dr["id_key"]);
                parameter.Code = dr["code"].ToString();
                parameter.Description = dr["description"].ToString();

                DisplayText dt = new DisplayText();
                dt.code = parameter.Code;
                dt.description = parameter.Description;
                parameter.DisplayText = dt;

                sampleDestinationList.Add(parameter);
            }
            dr.Close();

            Mea_Destination.DataSource = sampleDestinationList;
            Mea_Destination.DisplayMember = "Description";
            Mea_Destination.ValueMember = "Id_key";
            Mea_Destination.SelectedIndex = -1;
            con.Close();
            #endregion        
        }

        int countSeconds = 0;
        private void bgOpTimer_Tick(object sender, EventArgs e)
        {
            countSeconds++;
            clockLabel.Text = countSeconds + " seconds";
        }

        private void getBKPrevDataBtn_Click(object sender, EventArgs e)        
        {
            /*this.Enabled = false;
            clockLabel.Text = "0";
            countSeconds = 0;
            Thread.Sleep(4000);
            bgOpTimer = new System.Windows.Forms.Timer();
            bgOpTimer.Interval = 1000;
            bgOpTimer.Tick += bgOpTimer_Tick;
            bgOpTimer.Start();

            bkWorker = new BackgroundWorker();
            bkWorker.DoWork += bkWorker_GetDataWork;
            bkWorker.ProgressChanged += bkWorker_ProgressChanged;
            bkWorker.RunWorkerCompleted += bkWorker_RunWorkerCompleted;

            bkWorker.RunWorkerAsync();
            bkWorker.WorkerReportsProgress = true;//啟動回報進度
            bk_pbr.Value = 0;
            bk_pbr.Maximum = 20;//ProgressBar上限
            bk_pbr.Minimum = 0;//ProgressBar下限*/
        }

        Profile profile = new Profile();
        private void selectProfileBtn_Click(object sender, EventArgs e)
        {
            GlobalFunc.loadProfile = new Profile();
            SetProfile(true);
            Mea_RunBtn.Enabled = true;
            finalResult = new List<double>();
            detected = new List<int>();
            detectorName = GlobalFunc.loadProfile.Detector;
        }

        private bool SetProfile(bool display)
        {
            bool loadProfileSuccessful = false;
            try
            {
                profile = (Profile)mainProfileComboBox.SelectedItem;
            }
            catch { }

            if (profile.ProfileName != "")
            {
                label14.Visible = true;
                label15.Visible = true;
                label16.Visible = true;
                selProfileLabel.Visible = true;
                selDetectorLabel.Visible = true;
                selQtyIsotope.Visible = true;
                SetBKLabelEmpty();
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand deleteCmd = new SqlCeCommand("delete from currProfile", con);
                deleteCmd.CommandType = CommandType.Text;
                deleteCmd.ExecuteNonQuery();
                deleteCmd.Dispose();
                SqlCeCommand insertCmd = new SqlCeCommand("insert into currProfile values (@profileName, @fileName)", con);
                insertCmd.CommandType = CommandType.Text;
                insertCmd.Parameters.AddWithValue("@profileName", profile.ProfileName);
                insertCmd.Parameters.AddWithValue("@fileName", profile.FileName.Replace(".txt", "") + ".txt");
                insertCmd.ExecuteNonQuery();
                insertCmd.Dispose();
                con.Close();
                if (display)
                {
                    string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("select"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                #region read profile file

                if (File.Exists(@"C:\LCMS\Profile\" + profile.FileName.Replace(".txt", "") + ".txt"))
                {                    
                    GlobalFunc.loadProfile = GlobalFunc.LoadProfile(profile.FileName.Replace(".txt", "") + ".txt");
                    DetectorLabel.Text = GlobalFunc.loadProfile.Detector;
                    bkLocationTxt.Text = GlobalFunc.loadProfile.Location;
                    newLocationTxt.Text = GlobalFunc.loadProfile.Location;
                    bkAcquiredTxt.Text = GlobalFunc.loadProfile.Date;
                    bkCountTimeTxt.Text = GlobalFunc.loadProfile.LiveTime;
                    selDetectorLabel.Text = GlobalFunc.loadProfile.Detector;
                    selProfileLabel.Text = GlobalFunc.loadProfile.ProfileName;
                    selQtyIsotope.Text = GlobalFunc.loadProfile.Qty.ToString();

                    if (GlobalFunc.loadProfile.Detector == "Top")
                    {
                        if (GlobalFunc.loadProfile.BkTopCPS.Count != 0)
                        {
                            preIsoValue1.Text = GlobalFunc.loadProfile.BkTopCPS[0] + " CPS";
                            preIsoValue2.Text = GlobalFunc.loadProfile.BkTopCPS[1] + " CPS";
                            preIsoValue3.Text = GlobalFunc.loadProfile.BkTopCPS[2] + " CPS";
                            preIsoValue4.Text = GlobalFunc.loadProfile.BkTopCPS[3] + " CPS";
                            if (GlobalFunc.loadProfile.Qty == 5)
                            {
                                preIsoValue5.Text = GlobalFunc.loadProfile.BkTopCPS[4] + " CPS";
                            }
                            else if (GlobalFunc.loadProfile.Qty == 6)
                            {
                                preIsoValue5.Text = GlobalFunc.loadProfile.BkTopCPS[4] + " CPS";
                                preIsoValue6.Text = GlobalFunc.loadProfile.BkTopCPS[5] + " CPS";
                            }
                            totalTxt.Text = GlobalFunc.loadProfile.BkTopCPS[GlobalFunc.loadProfile.BkTopCPS.Count - 1] + " CPS";
                        }

                    }
                    else if (GlobalFunc.loadProfile.Detector == "Bottom")
                    {
                        if (GlobalFunc.loadProfile.BKBottomCPS.Count != 0)
                        {
                            preIsoValue1.Text = GlobalFunc.loadProfile.BKBottomCPS[0] + " CPS";
                            preIsoValue2.Text = GlobalFunc.loadProfile.BKBottomCPS[1] + " CPS";
                            preIsoValue3.Text = GlobalFunc.loadProfile.BKBottomCPS[2] + " CPS";
                            preIsoValue4.Text = GlobalFunc.loadProfile.BKBottomCPS[3] + " CPS";
                            if (GlobalFunc.loadProfile.Qty == 5)
                            {
                                preIsoValue5.Text = GlobalFunc.loadProfile.BKBottomCPS[4] + " CPS";
                            }
                            else if (GlobalFunc.loadProfile.Qty == 6)
                            {
                                preIsoValue5.Text = GlobalFunc.loadProfile.BKBottomCPS[4] + " CPS";
                                preIsoValue6.Text = GlobalFunc.loadProfile.BKBottomCPS[5] + " CPS";
                            }
                            totalTxt.Text = GlobalFunc.loadProfile.BKBottomCPS[GlobalFunc.loadProfile.BKBottomCPS.Count - 1] + " CPS";
                        }
                    }
                    else if (GlobalFunc.loadProfile.Detector == "Dual")
                    {
                        if (GlobalFunc.loadProfile.BkTopCPS.Count != 0 && GlobalFunc.loadProfile.BKBottomCPS.Count != 0)
                        {
                            preIsoValue1.Text = (Convert.ToDouble(GlobalFunc.loadProfile.BkTopCPS[0]) + Convert.ToDouble(GlobalFunc.loadProfile.BKBottomCPS[0])) + " CPS";
                            preIsoValue2.Text = (Convert.ToDouble(GlobalFunc.loadProfile.BkTopCPS[1]) + Convert.ToDouble(GlobalFunc.loadProfile.BKBottomCPS[1])) + " CPS";
                            preIsoValue3.Text = (Convert.ToDouble(GlobalFunc.loadProfile.BkTopCPS[2]) + Convert.ToDouble(GlobalFunc.loadProfile.BKBottomCPS[2])) + " CPS";
                            preIsoValue4.Text = (Convert.ToDouble(GlobalFunc.loadProfile.BkTopCPS[3]) + Convert.ToDouble(GlobalFunc.loadProfile.BKBottomCPS[3])) + " CPS";
                            if (GlobalFunc.loadProfile.Qty == 5)
                            {
                                preIsoValue5.Text = (Convert.ToDouble(GlobalFunc.loadProfile.BkTopCPS[4]) + Convert.ToDouble(GlobalFunc.loadProfile.BKBottomCPS[4])) + " CPS";
                            }
                            else if (GlobalFunc.loadProfile.Qty == 6)
                            {
                                preIsoValue5.Text = (Convert.ToDouble(GlobalFunc.loadProfile.BkTopCPS[4]) + Convert.ToDouble(GlobalFunc.loadProfile.BKBottomCPS[4])) + " CPS";
                                preIsoValue6.Text = (Convert.ToDouble(GlobalFunc.loadProfile.BkTopCPS[5]) + Convert.ToDouble(GlobalFunc.loadProfile.BKBottomCPS[5])) + " CPS";
                            }
                            totalTxt.Text = (Convert.ToDouble(GlobalFunc.loadProfile.BkTopCPS[GlobalFunc.loadProfile.BkTopCPS.Count - 1]) + Convert.ToDouble(GlobalFunc.loadProfile.BKBottomCPS[GlobalFunc.loadProfile.BKBottomCPS.Count - 1])) + " CPS";
                        }
                    }

                    preIsoTxt1.Text = GlobalFunc.loadProfile.IsoSeqList[0].Name + " " + GlobalFunc.rm.GetString("countRate");
                    preIsoTxt2.Text = GlobalFunc.loadProfile.IsoSeqList[1].Name + " " + GlobalFunc.rm.GetString("countRate");
                    preIsoTxt3.Text = GlobalFunc.loadProfile.IsoSeqList[2].Name + " " + GlobalFunc.rm.GetString("countRate");
                    preIsoTxt4.Text = GlobalFunc.loadProfile.IsoSeqList[3].Name + " " + GlobalFunc.rm.GetString("countRate");
                    newIsoTxt1.Text = GlobalFunc.loadProfile.IsoSeqList[0].Name + " " + GlobalFunc.rm.GetString("countRate");
                    newIsoTxt2.Text = GlobalFunc.loadProfile.IsoSeqList[1].Name + " " + GlobalFunc.rm.GetString("countRate");
                    newIsoTxt3.Text = GlobalFunc.loadProfile.IsoSeqList[2].Name + " " + GlobalFunc.rm.GetString("countRate");
                    newIsoTxt4.Text = GlobalFunc.loadProfile.IsoSeqList[3].Name + " " + GlobalFunc.rm.GetString("countRate");
                    if (GlobalFunc.loadProfile.Qty == 5)
                    {
                        preIsoTxt5.Text = GlobalFunc.loadProfile.IsoSeqList[4].Name + " " + GlobalFunc.rm.GetString("countRate");
                        newIsoTxt5.Text = GlobalFunc.loadProfile.IsoSeqList[4].Name + " " + GlobalFunc.rm.GetString("countRate");
                    }
                    else if (GlobalFunc.loadProfile.Qty == 6)
                    {
                        preIsoTxt5.Text = GlobalFunc.loadProfile.IsoSeqList[4].Name + " " + GlobalFunc.rm.GetString("countRate");
                        newIsoTxt5.Text = GlobalFunc.loadProfile.IsoSeqList[4].Name + " " + GlobalFunc.rm.GetString("countRate");
                        preIsoTxt6.Text = GlobalFunc.loadProfile.IsoSeqList[5].Name + " " + GlobalFunc.rm.GetString("countRate");
                        newIsoTxt6.Text = GlobalFunc.loadProfile.IsoSeqList[5].Name + " " + GlobalFunc.rm.GetString("countRate");
                    }
                    loadProfileSuccessful = true;
                }
                else
                {
                    loadProfileSuccessful = false;
                    string btnClicked2 = CustomMessageBox.Show(GlobalFunc.rm.GetString("profileFileError"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                #endregion            
                GlobalFunc.logManager.WriteLog("Selected Profile:" + GlobalFunc.loadProfile.ProfileName);
                GlobalFunc.logManager.WriteLog("IsoSeqList Count:" + GlobalFunc.loadProfile.IsoSeqList.Count.ToString());
            }
            return loadProfileSuccessful;
        }

        public void SetBKLabelEmpty()
        {
            bkAcquiredTxt.Text = "";
            bkLocationTxt.Text = "";
            bkCountTimeTxt.Text = "";
            preIsoTxt1.Text = "";
            preIsoTxt2.Text = "";
            preIsoTxt3.Text = "";
            preIsoTxt4.Text = "";
            preIsoTxt5.Text = "";
            preIsoTxt6.Text = "";
            preIsoValue1.Text = "";
            preIsoValue2.Text = "";
            preIsoValue3.Text = "";
            preIsoValue4.Text = "";
            preIsoValue5.Text = "";
            preIsoValue6.Text = "";
            totalTxt.Text = "";
            newAcquiredTxt.Text = "";
            newLocationTxt.Text = "";
            newCountTimeTxt.Text = "";
            newIsoTxt1.Text = "";
            newIsoTxt2.Text = "";
            newIsoTxt3.Text = "";
            newIsoTxt4.Text = "";
            newIsoTxt5.Text = "";
            newIsoTxt6.Text = "";
            newIsoValue1.Text = "";
            newIsoValue2.Text = "";
            newIsoValue3.Text = "";
            newIsoValue4.Text = "";
            newIsoValue5.Text = "";
            newIsoValue6.Text = "";
            newTotalTxt.Text = "";
        }

        public void LoadProfile()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand proCmd = new SqlCeCommand("select * from profileName order by Id", con);
            proCmd.CommandType = CommandType.Text;
            SqlCeDataReader proDr = proCmd.ExecuteReader();
            List<Profile> pList = new List<Profile>();
            Profile empty = new Profile();
            pList.Add(empty);
            while (proDr.Read())
            {
                Profile pn = new Profile();
                pn.ID = Convert.ToInt32(proDr["id"]);
                pn.ProfileName = proDr["profileName"].ToString();
                pn.FileName = proDr["fileName"].ToString();
                pList.Add(pn);
            }
            proDr.Close();
            con.Close();
            mainProfileComboBox.DataSource = pList;
            mainProfileComboBox.DisplayMember = "ProfileName";
            mainProfileComboBox.ValueMember = "ID";
        }

        private void Mea_RunBtn_Click(object sender, EventArgs e)
        {
            bool passRun = true;
            SetProfile(false);
            if (Mea_Location.SelectedIndex == -1)
            {
                passRun = false;
                string btnClicked = CustomMessageBox.Show("Please Select Location", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            if (Mea_VehNo.Text == "")
            {
                passRun = false;
                string btnClicked = CustomMessageBox.Show("Please Enter Vehicle No.", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);             
            }
            if (Mea_HealthCertNo.Text == "")
            {
                passRun = false;
                string btnClicked = CustomMessageBox.Show("Please Enter Health Cert No.", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);             
            
            }
            if (Mea_FarmCode.SelectedIndex == -1)
            {
                passRun = false;
                string btnClicked = CustomMessageBox.Show("Please Select Farm Code", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);             
            
            }
            if (Mea_PlaceOfOrigin.SelectedIndex == -1)
            {
                passRun = false;
                string btnClicked = CustomMessageBox.Show("Please Select Place of origin", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);             
            
            }
            if (Mea_Species.SelectedIndex == -1)
            {
                passRun = false;
                string btnClicked = CustomMessageBox.Show("Please Select Species", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);             
            
            }
            if (Mea_Quantity.Text == "")
            {
                passRun = false;
                string btnClicked = CustomMessageBox.Show("Please Enter Species", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);             

            }
            if (Mea_SampleSize.Text == "")
            {
                passRun = false;
                string btnClicked = CustomMessageBox.Show("Please Enter Sample Sized", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);             
            
            }
            if (Mea_Destination.SelectedIndex == -1)
            {
                passRun = false;
                string btnClicked = CustomMessageBox.Show("Please Select Destination", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);             
 
            }
            if (Mea_RefNo.Text == "")
            {
                passRun = false;
                string btnClicked = CustomMessageBox.Show("Please Enter Reference No.", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                string _location = ((Location)Mea_Location.SelectedItem).Code;
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("select * from measure where refNo = @refNo", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@refNo", Mea_RefNo.Text);
                SqlCeDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    passRun = false;
                    string btnClicked = CustomMessageBox.Show("Reference No. Existed", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                dr.Close();
                con.Close();
            }

            if (selDetectorLabel.Text.ToLower() == "dual")
            {
                if (!File.Exists(GlobalFunc.loadProfile.RoiPath1))
                {
                    passRun = false;
                    string buttonID = CustomMessageBox.Show("Can't find Top Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                if (!File.Exists(GlobalFunc.loadProfile.RoiPath2))
                {
                    passRun = false;
                    string buttonID = CustomMessageBox.Show("Can't find Bottom Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
            }
            else if (selDetectorLabel.Text.ToLower() == "top")
            {
                if (!File.Exists(GlobalFunc.loadProfile.RoiPath1))
                {
                    passRun = false;
                    string buttonID = CustomMessageBox.Show("Can't find Top Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
            }
            else if (selDetectorLabel.Text.ToLower() == "bottom")
            {
                if (!File.Exists(GlobalFunc.loadProfile.RoiPath2))
                {
                    passRun = false;
                    string buttonID = CustomMessageBox.Show("Can't find Bottom Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
            }
            
            if (passRun)
            {
                string _location = ((Location)Mea_Location.SelectedItem).Code;
                string _vehicle = Mea_VehNo.Text;
                string _hcn = Mea_HealthCertNo.Text;
                string _farmCode = ((FarmCode)Mea_FarmCode.SelectedItem).Code;
                string _poo = ((PlaceOfOrigin)Mea_PlaceOfOrigin.SelectedItem).EngName;
                string _species = ((Species)Mea_Species.SelectedItem).Code;
                string _qty = Mea_Quantity.Text;
                string _size = Mea_SampleSize.Text;
                string _destination = ((Destination)Mea_Destination.SelectedItem).Code;
                string _remarks = Mea_Remarks.Text;
                string _refNo = Mea_RefNo.Text;

                detected.Clear();
                topMeasureCPSList.Clear();
                bottomMeasureCPSList.Clear();

                #region run measure part
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("confirmMeasureTxt"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), GlobalFunc.rm.GetString("noBtnTxt"), false, 0);
                if (btnClicked == "1")
                {
                    detectorName = GlobalFunc.loadProfile.Detector;

                    switch (selDetectorLabel.Text.ToLower())
                    {
                        case "top": BKManager.beginMeasureScript = GlobalFunc.topScriptSet.MeasureBegin;
                            BKManager.endMeasureScript = GlobalFunc.topScriptSet.MeasureEnd;
                            break;
                        case "bottom": BKManager.beginMeasureScript = GlobalFunc.bottomScriptSet.MeasureBegin;
                            BKManager.endMeasureScript = GlobalFunc.bottomScriptSet.MeasureEnd;
                            break;
                        case "dual": BKManager.beginMeasureScript = GlobalFunc.dualScriptSet.MeasureBegin;
                            BKManager.endMeasureScript = GlobalFunc.dualScriptSet.MeasureEnd;
                            break;
                    }
                    
                    bgOpTimer.Dispose();
                    bgOpTimer = new System.Windows.Forms.Timer();                   
                    UpdateStatusLabel("Connecting...", 0, 255, 0);
                    this.Enabled = false;

                    bool passDetector = false;
                    if (selDetectorLabel.Text.Contains("Top"))
                    {
                        GlobalFunc.tc.checkDetector1Connection();
                        if (GlobalFunc.connectedToDetector1)
                        {
                            passDetector = true;
                        }
                    }
                    else if (selDetectorLabel.Text.Contains("Bottom"))
                    {
                        GlobalFunc.tc.checkDetector2Connection();
                        if (GlobalFunc.connectedToDetector2)
                        {
                            passDetector = true;
                        }
                    }
                    else if (selDetectorLabel.Text.Contains("Dual"))
                    {
                        GlobalFunc.tc.checkDetector1Connection();
                        GlobalFunc.tc.checkDetector2Connection();
                        if (GlobalFunc.connectedToDetector1 && GlobalFunc.connectedToDetector2)
                        {
                            passDetector = true;
                        }
                    }

                    //GlobalFunc.connectedToDetector1 = true; //hardcode

                    if (!passDetector)
                    {
                        #region fail to connect sesonsor
                        if (DetectorLabel.Text.Contains("Top"))
                        {
                            GlobalFunc.SetAlarmBox(6);
                            string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector1"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                            if (buttonID == "1")
                            {
                                GlobalFunc.SetAlarmBox(0);
                                this.Enabled = true;
                                UpdateStatusLabel("Alarm", 255, 0, 0);
                            }
                        }
                        else if (DetectorLabel.Text.Contains("Bottom"))
                        {
                            GlobalFunc.SetAlarmBox(6);
                            string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector2"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                            if (buttonID == "1")
                            {
                                GlobalFunc.SetAlarmBox(0);
                                this.Enabled = true;
                                UpdateStatusLabel("Alarm", 255, 0, 0);
                            }
                        }
                        else
                        {
                            GlobalFunc.SetAlarmBox(6);
                            string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector1") + System.Environment.NewLine + GlobalFunc.rm.GetString("failConnectDetector2"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                            if (buttonID == "1")
                            {
                                GlobalFunc.SetAlarmBox(0);
                                this.Enabled = true;
                                UpdateStatusLabel("Alarm", 255, 0, 0);
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        #region run measure
                        UpdateStatusLabel("Measuring...", 0, 255, 0);
                        clockLabel.Text = "0";
                        countSeconds = 0;
                        Thread.Sleep(4000);
                        bgOpTimer = new System.Windows.Forms.Timer();
                        bgOpTimer.Interval = 1000;
                        bgOpTimer.Tick += bgOpTimer_Tick;
                        bgOpTimer.Start();

                        bkWorker = new BackgroundWorker();
                        bkWorker.DoWork += Measure_DoWork;
                        bkWorker.ProgressChanged += Measure_ProgressChanged;
                        bkWorker.RunWorkerCompleted += Measure_RunWorkerCompleted;

                        bkWorker.RunWorkerAsync();
                        bkWorker.WorkerReportsProgress = true;//啟動回報進度
                        bk_pbr.Value = 0;
                        BKManager.SetMeasureTime();
                        bk_pbr.Maximum = Convert.ToInt32(BKManager.supposeSecond) + Convert.ToInt32(GlobalFunc.bufferTime) + 10;//ProgressBar上限
                        bk_pbr.Minimum = 0;//ProgressBar下限*/

                        #endregion
                    }
                    GlobalFunc.ResetConnectDetector();
                }
                #endregion
            }
        }

        List<double> finalResult = new List<double>();
        List<int> detected = new List<int>();
        List<double> uncertainty = new List<double>();
        double ratioSum = 0.0;
        double icr = 0.0;
        protected void Measure_DoWork(object sender, DoWorkEventArgs e)
        {
            GlobalFunc.loadProfile = new Profile();
            mainProfileComboBox.Invoke(new MethodInvoker(delegate
            {
                SetProfile(false);
                Mea_RunBtn.Enabled = true;
            }));

            int countCheck = 0;
            while (!GlobalFunc.checkActive(thisDetector))
            {
                Thread.Sleep(1000);
                countCheck++;
                if (countCheck == 20)
                {
                    break;
                }
                if (GlobalFunc.checkActive(thisDetector))
                {
                    break;
                }
            }
            
            finalResult = new List<double>();
            detected = new List<int>();
            detectorName = GlobalFunc.loadProfile.Detector;

            BKManager.measureList.Clear();
            ratioSum = 0.0;            
            icr = 0.0;
            BKManager.SetMeasureTime();
            BKManager.SetTemp(GlobalFunc.loadProfile.Qty, GlobalFunc.loadProfile.Detector, BKManager.beginMeasureScript, 
                GlobalFunc.loadProfile.RoiPath1, GlobalFunc.loadProfile.RoiPath2);

            BKManager.jobID = GlobalFunc.logManager.SaveCurrentJob("Measurement Operation");
            GlobalFunc.logManager.WriteLog("Run Measurement operation");
            ExecuteManager exm = new ExecuteManager();
            exm.scriptFilePath = BKManager.tempRunScript;
            Thread runThread = new Thread(new ThreadStart(exm.RunScript));
            runThread.Start();

            for (int i = 0; i < Convert.ToInt32(BKManager.supposeSecond) + Convert.ToInt32(GlobalFunc.bufferTime); i++)
            {
                bkWorker.ReportProgress(i, i);
                Thread.Sleep(1000);
            }

            if (runThread.IsAlive)
            {
                runThread.Abort();
            }

            int countSleep = 0;
            while (true)
            {
                bk_pbr.Invoke(new MethodInvoker(delegate
                {
                    UpdateStatusLabel("Checking Active", 255, 128, 0);
                }));
                Thread.Sleep(1000);
                if (GlobalFunc.checkActive(thisDetector))
                {
                    break;
                }
                countSleep++;
                if (countSleep == 5)
                {
                    break;
                }
            }
            
            decimal liveTime = 0;
            DateTime reportDateTime = new DateTime();
            GetMeasureData(ref liveTime, ref reportDateTime);

            double _size = Convert.ToDouble(Mea_SampleSize.Text);

            #region calculate

            if (BKManager.measureList.Count == 0)
            {
                string btnClicked2 = CustomMessageBox.Show("Measurement Error, please try again", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                bool mdcreportOn = true;
                if (GlobalFunc.mlr == 0)
                {
                    mdcreportOn = false;
                }
                                   
                
                GlobalFunc.logManager.WriteLog("FinalBkCPS:" + GlobalFunc.loadProfile.FinalBkCPS.Count);
                GlobalFunc.logManager.WriteLog("BKManager.measureList:" + BKManager.measureList.Count);
                GlobalFunc.logManager.WriteLog("FinalMatrix:" + GlobalFunc.loadProfile.FinalMatrix.Count);
                GlobalFunc.logManager.WriteLog("Live time:" + Convert.ToDouble(liveTime));
                GlobalFunc.logManager.WriteLog("RatioPeak:" + GlobalFunc.loadProfile.RatioPeak.Count);
                GlobalFunc.logManager.WriteLog("Activity:" + GlobalFunc.loadProfile.Activity.Count);
                GlobalFunc.logManager.WriteLog("HalfTime:" + GlobalFunc.loadProfile.HalfTime.Count);
                GlobalFunc.logManager.WriteLog("Ref_Date:" + GlobalFunc.loadProfile.Ref_Date.Count);
                GlobalFunc.logManager.WriteLog("reportDateTime:" + reportDateTime.ToString());
                GlobalFunc.logManager.WriteLog("_size:" + _size);
                GlobalFunc.logManager.WriteLog("mdcreportOn:" + mdcreportOn.ToString());
                GlobalFunc.logManager.WriteLog("AlarmLevel:" + GlobalFunc.loadProfile.AlarmLevel.Count);
                GlobalFunc.logManager.WriteLog("AlarmPCLevel:" + GlobalFunc.loadProfile.AlarmPCLevel.Count);
                GlobalFunc.logManager.WriteLog("Alarm:" + GlobalFunc.loadProfile.Alarm);

                //for test
                //GlobalFunc.loadProfile.FinalBkCPS.RemoveAt(GlobalFunc.loadProfile.FinalBkCPS.Count - 1);
                //
                try
                {
                    string _qty = Mea_Quantity.Text;
                    finalResult = GlobalFunc.cal.Measurement_Activity(ref detected, GlobalFunc.loadProfile.FinalBkCPS, BKManager.measureList,
                    GlobalFunc.loadProfile.FinalMatrix, Convert.ToDouble(liveTime), GlobalFunc.loadProfile.RatioPeak,
                    GlobalFunc.loadProfile.Activity,
                    GlobalFunc.loadProfile.HalfTime, GlobalFunc.loadProfile.Ref_Date, reportDateTime, _size, Convert.ToDouble(_qty), mdcreportOn);
                    GlobalFunc.logManager.WriteLog("Debug Log:" + finalResult.Count);

                    if (GlobalFunc.rsl == 1)
                    {

                        /*ratioSum = GlobalFunc.cal.RatioSum(GlobalFunc.loadProfile.FinalBkCPS, BKManager.measureList, GlobalFunc.loadProfile.FinalMatrix,
                                            GlobalFunc.loadProfile.AlarmLevel, GlobalFunc.loadProfile.AlarmPCLevel, GlobalFunc.loadProfile.Alarm);*/
                        ratioSum = GlobalFunc.cal.RationSum(finalResult, GlobalFunc.loadProfile.AlarmLevel, GlobalFunc.loadProfile.AlarmPCLevel, GlobalFunc.loadProfile.Alarm);
                        ratioSum = GlobalFunc.Math45(ratioSum);
                    }

                    uncertainty = GlobalFunc.cal.Uncertainty(BKManager.measureList);
                }
                catch (Exception ex)
                {
                    GlobalFunc.logManager.WriteLog("Measure final error:" + ex.Message);
                }

                GlobalFunc.logManager.WriteLog("End Measurement operation");
            }

            #endregion
        }

        protected void Measure_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            bk_pbr.Value = e.ProgressPercentage;
        }
       
        protected void Measure_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            int mID = 0;
            bgOpTimer.Stop();
            clockLabel.Text = "";
            bk_pbr.Value = 0;

            bool noError = true;
           
            string errorText = "";

            if (finalResult.Count != GlobalFunc.loadProfile.IsoSeqList.Count)
            {
                noError = false;
                errorText += "Can't get final result";
                if (errorText != "")
                {
                    string btnClicked2 = CustomMessageBox.Show(errorText, GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                GlobalFunc.logManager.WriteLog("Final Result:" + finalResult.Count + " IsoSeqList Count:" + GlobalFunc.loadProfile.IsoSeqList.Count);
                this.Enabled = true;
            }
            else
            {
                if (GlobalFunc.loadProfile.Alarm == "DIL")
                {
                    for (int i = 0; i < GlobalFunc.loadProfile.IsoSeqList.Count; i++)
                    {
                        if ((finalResult[i] / Convert.ToDouble(GlobalFunc.loadProfile.IsoSeqList[i].DIL) * 100) > Convert.ToDouble(GlobalFunc.loadProfile.IsoSeqList[i].AL_PC))
                        {
                            noError = false;
                            errorText += GlobalFunc.loadProfile.IsoSeqList[i].Name + " Alarm" + System.Environment.NewLine;
                        }
                    }
                }
                else if (GlobalFunc.loadProfile.Alarm == "AL")
                {
                    for (int i = 0; i < GlobalFunc.loadProfile.IsoSeqList.Count; i++)
                    {
                        if (finalResult[i] > Convert.ToDouble(GlobalFunc.loadProfile.IsoSeqList[i].AL))
                        {
                            noError = false;
                            errorText += GlobalFunc.loadProfile.IsoSeqList[i].Name + " Alarm" + System.Environment.NewLine;
                        }
                    }
                }

                double insertICR = 0.0;
                if (GlobalFunc.loadProfile.Detector == "Top")
                {
                    insertICR = GlobalFunc.det1_icr;
                    if (GlobalFunc.det1_icr >= GlobalFunc.det1_icr_alarm)
                    {
                        noError = false;
                        //errorText += "Detector Input count rate is reached or  " + System.Environment.NewLine + "over alarm level" + System.Environment.NewLine;
                        errorText += "Count Rate Alarm" + System.Environment.NewLine;
                    }

                }
                else if (GlobalFunc.loadProfile.Detector == "Bottom")
                {
                    insertICR = GlobalFunc.det2_icr;
                    if (GlobalFunc.det2_icr >= GlobalFunc.det2_icr_alarm)
                    {
                        noError = false;
                        //errorText += "Detector Input count rate is reached or " + System.Environment.NewLine + "over alarm level" + System.Environment.NewLine;
                        errorText += "Count Rate Alarm" + System.Environment.NewLine;
                    }
                }
                else if (GlobalFunc.loadProfile.Detector == "Dual")
                {
                    /*if ((GlobalFunc.det1_icr + GlobalFunc.det2_icr) / 2 >= (GlobalFunc.det1_icr_alarm + GlobalFunc.det2_icr_alarm) / 2)
                    {
                        noError = false;
                        errorText += "Detector Input count rate is reached or  " + System.Environment.NewLine + "over alarm level" + System.Environment.NewLine;
                    }*/
                    insertICR = (GlobalFunc.det1_icr + GlobalFunc.det2_icr) / 2;
                    if ((GlobalFunc.det1_icr + GlobalFunc.det2_icr) / 2 >= GlobalFunc.dual_icr_alarm)
                    {
                        noError = false;
                        //errorText += "Detector Input count rate is reached or  " + System.Environment.NewLine + "over alarm level" + System.Environment.NewLine;
                        errorText += "Count Rate Alarm" + System.Environment.NewLine;
                    }
                }

                if (GlobalFunc.rsl == 1)
                {
                    if (ratioSum > 1.0)
                    {
                        noError = false;
                        errorText += "Ratio Sum is over than 1" + System.Environment.NewLine;
                    }
                }

                try
                {
                    if (BKManager.measureList.Count > 0)
                    {
                        if (!noError)
                        {
                            GlobalFunc.SetAlarmBox(6);
                        }
                        #region insert part
                        int satis = 0; // 0 == satisfactory, 1 == Unsatisfactory + alarm condition

                        if (noError)
                        {
                            satis = 0;
                        }
                        else
                        {
                            satis = 1;
                        }

                        string _location = ((Location)Mea_Location.SelectedItem).Code;
                        string _vehicle = Mea_VehNo.Text;
                        string _hcn = Mea_HealthCertNo.Text;
                        string _farmCode = ((FarmCode)Mea_FarmCode.SelectedItem).Code;
                        string _poo = ((PlaceOfOrigin)Mea_PlaceOfOrigin.SelectedItem).EngName;
                        string _species = ((Species)Mea_Species.SelectedItem).Code;
                        string _qty = Mea_Quantity.Text;
                        string _size = Mea_SampleSize.Text;

                        string _destination = ((Destination)Mea_Destination.SelectedItem).Code;
                        string _remarks = Mea_Remarks.Text;
                        string _refNo = Mea_RefNo.Text;
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                        con.Open();
                        SqlCeCommand insertCmd = new SqlCeCommand(@"insert into measure (location , vehicleNo, hcn, farmCode, poo, species, 
                                                        qty, sampleSize, destination, remarks, year, month, day, time, refNo, ratioSum,
                                                        icr, profileName, icrError, rsl, ru, mlr, detector, fullDate, satis, showTemp) values 
                                                          (@location, @vehicleNo, @hcn, @farmCode, @poo, @species, @qty, @sampleSize,
                                                            @destination, @remarks, @year, @month, @day, @time, @refNo, @ratioSum, 
                                                        @icr, @profileName, @icrError, @rsl, @ru, @mlr, @detector, @fullDate, @satis, @showTemp)", con);
                        insertCmd.CommandType = CommandType.Text;
                        insertCmd.Parameters.AddWithValue("@location", _location);
                        insertCmd.Parameters.AddWithValue("@vehicleNo", _vehicle);
                        insertCmd.Parameters.AddWithValue("@hcn", _hcn);
                        insertCmd.Parameters.AddWithValue("@farmCode", _farmCode);
                        insertCmd.Parameters.AddWithValue("@poo", _poo);
                        insertCmd.Parameters.AddWithValue("@species", _species);
                        insertCmd.Parameters.AddWithValue("@qty", _qty);
                        insertCmd.Parameters.AddWithValue("@sampleSize", _size);
                        insertCmd.Parameters.AddWithValue("@destination", _destination);
                        insertCmd.Parameters.AddWithValue("@remarks", _remarks);
                        insertCmd.Parameters.AddWithValue("@year", DateTime.Now.Year);
                        insertCmd.Parameters.AddWithValue("@month", DateTime.Now.Month);
                        insertCmd.Parameters.AddWithValue("@day", DateTime.Now.Day);
                        insertCmd.Parameters.AddWithValue("@time", DateTime.Now.ToShortTimeString());
                        insertCmd.Parameters.AddWithValue("@refNo", _refNo);
                        insertCmd.Parameters.AddWithValue("@ratioSum", ratioSum);
                        insertCmd.Parameters.AddWithValue("@icr", insertICR);

                        insertCmd.Parameters.AddWithValue("@profileName", GlobalFunc.loadProfile.ProfileName);
                        if (noError == true)
                        {
                            insertCmd.Parameters.AddWithValue("@icrError", 1);
                        }
                        else
                        {
                            insertCmd.Parameters.AddWithValue("@icrError", 0);
                        }
                        insertCmd.Parameters.AddWithValue("@rsl", GlobalFunc.rsl);
                        insertCmd.Parameters.AddWithValue("@ru", GlobalFunc.ru);
                        insertCmd.Parameters.AddWithValue("@mlr", GlobalFunc.mlr);
                        insertCmd.Parameters.AddWithValue("@detector", GlobalFunc.loadProfile.Detector);
                        insertCmd.Parameters.AddWithValue("@fullDate", DateTime.Now);
                        insertCmd.Parameters.AddWithValue("@satis", satis);
                        if (GlobalFunc.getTemp == true)
                        {
                            insertCmd.Parameters.AddWithValue("@showTemp", 1);
                        }
                        else
                        {
                            insertCmd.Parameters.AddWithValue("@showTemp", 0);
                        }
                        insertCmd.ExecuteNonQuery();
                        insertCmd.Dispose();


                        SqlCeCommand getCmd = new SqlCeCommand("select max(id) as id from measure where year = @year and month = @month and day = @day", con);
                        getCmd.CommandType = CommandType.Text;
                        getCmd.Parameters.AddWithValue("@year", DateTime.Now.Year);
                        getCmd.Parameters.AddWithValue("@month", DateTime.Now.Month);
                        getCmd.Parameters.AddWithValue("@day", DateTime.Now.Day);
                        SqlCeDataReader getDr = getCmd.ExecuteReader();
                        if (getDr.Read())
                        {
                            mID = Convert.ToInt32(getDr["id"]);
                        }
                        getDr.Close();

                        SqlCeCommand detailCmd = new SqlCeCommand(@"insert into measureDetail (mID, temperature, countTime, icr1, icr2, icrAlarmLevel1, icrAlarmLevel2)
                                                        values (@mID, @temp, @countTime, @icr1, @icr2, @icrAlarmLevel1, @icrAlarmLevel2)", con);
                        detailCmd.CommandType = CommandType.Text;
                        detailCmd.Parameters.AddWithValue("@mID", mID);
                        if (GlobalFunc.loadProfile.Detector == "Top" || GlobalFunc.loadProfile.Detector == "Dual")
                        {
                            detailCmd.Parameters.AddWithValue("@temp", GlobalFunc.det1_temp);
                        }
                        else if (GlobalFunc.loadProfile.Detector == "Bottom")
                        {
                            detailCmd.Parameters.AddWithValue("@temp", GlobalFunc.det2_temp);
                        }
                        else if (GlobalFunc.loadProfile.Detector == "Dual")
                        {
                            detailCmd.Parameters.AddWithValue("@temp", GlobalFunc.Math45Pt1((GlobalFunc.det1_temp + GlobalFunc.det2_temp) / 2));
                        }
                        detailCmd.Parameters.AddWithValue("@countTime", BKManager.supposeSecond.ToString());
                        detailCmd.Parameters.AddWithValue("@icr1", GlobalFunc.det1_icr);
                        detailCmd.Parameters.AddWithValue("@icr2", GlobalFunc.det2_icr);
                        detailCmd.Parameters.AddWithValue("@icrAlarmLevel1", GlobalFunc.det1_icr_alarm);
                        detailCmd.Parameters.AddWithValue("@icrAlarmLevel2", GlobalFunc.det2_icr_alarm);
                        detailCmd.ExecuteNonQuery();
                        detailCmd.Dispose();

                        for (int i = 0; i < GlobalFunc.loadProfile.IsoSeqList.Count; i++)
                        {
                            SqlCeCommand isoCmd = new SqlCeCommand(@"insert into measureIsotope (mID, isotopeName, activity, 
                                                    uncertainty, detected, alarmLevel, alarmType, alarmLevelPC, onAlarm) values (
                                                    @mID, @isotopeName, @activity, @uncertainty, 
                                                    @detected, @alarmLevel, @alarmType, @alarmLevelPC, @onAlarm)", con);
                            isoCmd.CommandType = CommandType.Text;
                            isoCmd.Parameters.AddWithValue("@mID", mID);
                            isoCmd.Parameters.AddWithValue("@isotopeName", GlobalFunc.loadProfile.IsoSeqList[i].Name);
                            isoCmd.Parameters.AddWithValue("@activity", finalResult[i]);

                            isoCmd.Parameters.AddWithValue("@uncertainty", uncertainty[i]);

                            if (detected.Count == GlobalFunc.loadProfile.IsoSeqList.Count)
                            {
                                isoCmd.Parameters.AddWithValue("@detected", detected[i]);
                            }
                            else
                            {
                                isoCmd.Parameters.AddWithValue("@detected", 0);
                            }
                            isoCmd.Parameters.AddWithValue("@alarmLevel", GlobalFunc.loadProfile.AlarmLevel[i]);
                            isoCmd.Parameters.AddWithValue("@alarmType", GlobalFunc.loadProfile.Alarm);
                            isoCmd.Parameters.AddWithValue("@alarmLevelPC", GlobalFunc.loadProfile.AlarmPCLevel[i]);

                            if (GlobalFunc.loadProfile.Alarm == "DIL")
                            {

                                if ((finalResult[i] / Convert.ToDouble(GlobalFunc.loadProfile.IsoSeqList[i].DIL) * 100) > Convert.ToDouble(GlobalFunc.loadProfile.IsoSeqList[i].AL_PC))
                                {
                                    isoCmd.Parameters.AddWithValue("@onAlarm", 1);
                                }
                                else
                                {
                                    isoCmd.Parameters.AddWithValue("@onAlarm", 0);
                                }

                            }
                            else if (GlobalFunc.loadProfile.Alarm == "AL")
                            {
                                if (finalResult[i] > Convert.ToDouble(GlobalFunc.loadProfile.IsoSeqList[i].AL))
                                {
                                    isoCmd.Parameters.AddWithValue("@onAlarm", 1);
                                }
                                else
                                {
                                    isoCmd.Parameters.AddWithValue("@onAlarm", 0);
                                }
                            }

                            isoCmd.ExecuteNonQuery();
                            isoCmd.Dispose();
                        }

                        con.Close();
                        if (GlobalFunc.outputWord != null)
                        {
                            GlobalFunc.outputWord.Dispose();
                        }

                        string filePath = "";

                        GlobalFunc.outputWord = new OutputWord();
                        GlobalFunc.outputWord.CreateTable(_location, _vehicle, _hcn,
                            _farmCode, _poo, _species, _qty, _size, _destination, _remarks,
                            mID, GlobalFunc.loadProfile, finalResult, detected, _refNo, satis, GlobalFunc.loadProfile.Alarm, ratioSum, uncertainty, noError, ref filePath);

                        #region save spectra
                        if (GlobalFunc.loadProfile.Detector == "Top")
                        {
                            if (File.Exists(@"C:\LCMS\Spectra\DET1_Measure.Chn"))
                            {
                                string newName = filePath.Replace(@"C:\LCMS\WordReport\", "").Replace(".docx", "").Replace(".doc", "");
                                newName += "_det1.Chn";
                                File.Copy(@"C:\LCMS\Spectra\DET1_Measure.Chn", @"C:\LCMS\Spectra\" + newName);
                                File.Delete(@"C:\LCMS\Spectra\DET1_Measure.Chn");
                            }
                        }
                        else if (GlobalFunc.loadProfile.Detector == "Bottom")
                        {
                            if (File.Exists(@"C:\LCMS\Spectra\DET2_Measure.Chn"))
                            {
                                string newName = filePath.Replace(@"C:\LCMS\WordReport\", "").Replace(".docx", "").Replace(".doc", "");
                                newName += "_det2.Chn";
                                File.Copy(@"C:\LCMS\Spectra\DET2_Measure.Chn", @"C:\LCMS\Spectra\" + newName);
                                File.Delete(@"C:\LCMS\Spectra\DET2_Measure.Chn");
                            }
                        }
                        else if (GlobalFunc.loadProfile.Detector == "Dual")
                        {
                            if (File.Exists(@"C:\LCMS\Spectra\DET1_Measure.Chn"))
                            {
                                string newName = filePath.Replace(@"C:\LCMS\WordReport\", "").Replace(".docx", "").Replace(".doc", "");
                                newName += "_det1.Chn";
                                File.Copy(@"C:\LCMS\Spectra\DET1_Measure.Chn", @"C:\LCMS\Spectra\" + newName);
                                File.Delete(@"C:\LCMS\Spectra\DET1_Measure.Chn");
                            }
                            if (File.Exists(@"C:\LCMS\Spectra\DET2_Measure.Chn"))
                            {
                                string newName = filePath.Replace(@"C:\LCMS\WordReport\", "").Replace(".docx", "").Replace(".doc", "");
                                newName += "_det2.Chn";
                                File.Copy(@"C:\LCMS\Spectra\DET2_Measure.Chn", @"C:\LCMS\Spectra\" + newName);
                                File.Delete(@"C:\LCMS\Spectra\DET2_Measure.Chn");
                            }
                        }

                        #endregion


                        string btnClicked2 = "1";
                        if (errorText != "")
                        {
                            btnClicked2 = CustomMessageBox.Show(errorText, GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        }
                        if (btnClicked2 == "1")
                        {
                            GlobalFunc.SetAlarmBox(0);
                            btnClicked2 = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("openBtnTxt"), GlobalFunc.rm.GetString("printBtnTxt"), false, 0);
                        }
                        if (btnClicked2 == "1")
                        {
                            Process.Start(filePath);
                        }
                        else if (btnClicked2 == "2")
                        {
                            ProcessStartInfo info2 = new ProcessStartInfo(filePath);
                            info2.Verb = "Print";
                            info2.CreateNoWindow = true;
                            info2.WindowStyle = ProcessWindowStyle.Hidden;
                            Process.Start(info2);
                            /*using (PrintDialog printDialog1 = new PrintDialog())
                            {
                                //printDialog1.PrinterSettings.PrinterName = printer;
                                if (printDialog1.ShowDialog() == DialogResult.OK)
                                {
                                    System.Diagnostics.ProcessStartInfo info = new System.Diagnostics.ProcessStartInfo(filename);
                                    //info.Arguments = “\”” + printDialog1.PrinterSettings.PrinterName + “\””;
                                    info.Arguments = “\”” + printer + “\””;
                                info.CreateNoWindow = true;
                                 info.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                                info.UseShellExecute = true;
                                info.Verb = “PrintTo”;
                                System.Diagnostics.Process.Start(info);
                              }
                            }*/
                        }

                        #endregion
                    }
                    else
                    {
                        MessageBox.Show("Measurement fail, please try again" + BKManager.measureList.Count.ToString());
                    }
                    //SetProfile();
                    #region
                    this.Enabled = true;
                    finalResult.Clear();
                    uncertainty.Clear();
                    detected.Clear();
                    UpdateStatusLabel("Stand by", 255, 255, 0);
                    Mea_RefNo.Text = "";
                    Mea_Location.SelectedIndex = -1;
                    Mea_Destination.SelectedIndex = -1;
                    Mea_VehNo.Text = "";
                    Mea_HealthCertNo.Text = "";
                    Mea_FarmCode.SelectedIndex = -1;
                    Mea_PlaceOfOrigin.SelectedIndex = -1;
                    Mea_Species.SelectedIndex = -1;
                    Mea_Quantity.Text = "";
                    Mea_SampleSize.Text = "";
                    Mea_Remarks.Text = "";
                    Mea_RunBtn.Enabled = false;
                    
                    selProfileLabel.Text = "";
                    selDetectorLabel.Text = "";
                    selQtyIsotope.Text = "";
                    mainProfileComboBox.SelectedIndex = -1;
                    #endregion
                }
                catch (Exception ex)
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    con.Open();
                    SqlCeCommand delCmd = new SqlCeCommand(@"delete from measure where id = @mID;", con);
                    delCmd.CommandType = CommandType.Text;
                    delCmd.Parameters.AddWithValue("@mID", mID);
                    delCmd.ExecuteNonQuery();
                    delCmd.Dispose();

                    delCmd = new SqlCeCommand(@"delete from measureDetail where mID = @mID;", con);
                    delCmd.CommandType = CommandType.Text;
                    delCmd.Parameters.AddWithValue("@mID", mID);
                    delCmd.ExecuteNonQuery();
                    delCmd.Dispose();

                    delCmd = new SqlCeCommand(@"delete from measureIsotope where mID = @mID", con);
                    delCmd.CommandType = CommandType.Text;
                    delCmd.Parameters.AddWithValue("@mID", mID);
                    delCmd.ExecuteNonQuery();
                    delCmd.Dispose();

                    con.Close();

                    string debugText = "IsoSeqListCount: " + GlobalFunc.loadProfile.IsoSeqList.Count.ToString() + System.Environment.NewLine;
                    debugText += "finalResult count: " + finalResult.Count.ToString() + System.Environment.NewLine;
                    debugText += "Uncertainty count: " + uncertainty.Count.ToString() + System.Environment.NewLine;
                    debugText += "Detected count: " + detected.Count.ToString() + System.Environment.NewLine;
                    debugText += "AlarmLevel count: " + GlobalFunc.loadProfile.AlarmLevel.Count.ToString() + System.Environment.NewLine;
                    debugText += "Alarm PC Level count: " + GlobalFunc.loadProfile.AlarmPCLevel.Count.ToString() + System.Environment.NewLine;
                    MessageBox.Show("Error please retry" + System.Environment.NewLine + debugText + ex.Message);

                    this.Enabled = true;
                    finalResult.Clear();
                    uncertainty.Clear();
                    detected.Clear();
                    UpdateStatusLabel("Stand by", 255, 255, 0);
                    Mea_RefNo.Text = "";
                    Mea_Location.Text = "";
                    Mea_VehNo.Text = "";
                    Mea_HealthCertNo.Text = "";
                    Mea_FarmCode.SelectedIndex = -1;
                    Mea_PlaceOfOrigin.SelectedIndex = -1;
                    Mea_Species.SelectedIndex = -1;
                    Mea_Quantity.Text = "";
                    Mea_SampleSize.Text = "";
                    Mea_Remarks.Text = "";
                    Mea_RunBtn.Enabled = true;

                    selProfileLabel.Text = "";
                    selDetectorLabel.Text = "";
                    selQtyIsotope.Text = "";
                    mainProfileComboBox.SelectedIndex = -1;
                }
            }
            Mea_RunBtn.Enabled = false;
        }

        List<string> topMeasureCPSList = new List<string>();
        List<string> bottomMeasureCPSList = new List<string>();
        public void GetMeasureData(ref decimal liveTime, ref DateTime reportDateTime)
        {
            topMeasureCPSList.Clear();
            bottomMeasureCPSList.Clear();

            BKManager.measureList = new List<double>();
            GlobalFunc.logManager.WriteLog("Get Measurement operation data");
            int countTryFile = 0;
            while (BKManager.measureList.Count == 0)
            {
                int countCheck = 0;
                while (!GlobalFunc.checkActive(selDetectorLabel.Text))
                {
                    Thread.Sleep(1000);
                    countCheck++;
                    if (countCheck == 20)
                    {
                        break;
                    }
                    if (GlobalFunc.checkActive(selDetectorLabel.Text))
                    {
                        break;
                    }
                }
                ExecuteManager exm = new ExecuteManager();
                BKManager.SetTemp(GlobalFunc.loadProfile.Qty, GlobalFunc.loadProfile.Detector, BKManager.endMeasureScript, GlobalFunc.loadProfile.RoiPath1, GlobalFunc.loadProfile.RoiPath2);
                exm.scriptFilePath = BKManager.tempRunScript;
                Thread runThread = new Thread(new ThreadStart(exm.RunScript));
                runThread.Start();

                clockLabel.Invoke(new MethodInvoker(delegate { clockLabel.Text = "Updating"; 
                        GlobalFunc.tc.GetDetector1Temp();
                        GlobalFunc.tc.GetDetector2Temp();
                        GlobalFunc.tc.GetDetector1ICR();
                        GlobalFunc.tc.GetDetector2ICR();                     
                }));

                countSeconds = 0;
                bk_pbr.Invoke(new MethodInvoker(delegate
                {
                    bk_pbr.Value = 0; bk_pbr.Maximum = Convert.ToInt32(GlobalFunc.bufferTime) + 10; bk_pbr.Minimum = 0; 
                    UpdateStatusLabel("Data Analyzing", 255, 255, 0); }));

                for (int i = 0; i < Convert.ToInt32(GlobalFunc.bufferTime); i++)
                {
                    bkWorker.ReportProgress(i, i);
                    Thread.Sleep(1000);
                }

                if (runThread.IsAlive)
                {
                    runThread.Abort();
                }

                int countSleep = 0;
                while (true)
                {
                    bk_pbr.Invoke(new MethodInvoker(delegate
                    {
                        UpdateStatusLabel("Checking Active", 0, 255, 0);
                    }));
                    Thread.Sleep(1000);
                    if (GlobalFunc.checkActive(selDetectorLabel.Text))
                    {
                        break;
                    }
                    countSleep++;
                    if (countSleep == 5)
                    {
                        break;
                    }
                }

                if (selDetectorLabel.Text.Contains("Top"))
                {
                    if (File.Exists(GlobalFunc.topScriptSet.MeasureData))
                    {
                        List<Roi> roiList1 = GlobalFunc.GetRoiData(GlobalFunc.topScriptSet.MeasureData);
                        if (roiList1.Count > 0)
                        {
                            liveTime = roiList1[0].lifeTime;
                            reportDateTime = roiList1[0].dt;
                            topMeasureCPSList = BKManager.CalBk(roiList1, ref BKManager.dt, ref BKManager.lifeTime, 1);
                            for (int k = 0; k < topMeasureCPSList.Count; k++)
                            {
                                BKManager.measureList.Add(Convert.ToDouble(topMeasureCPSList[k].Replace(" CPS", "")));

                                GlobalFunc.logManager.WriteLog("Measurement operation (top detector): " +
                                    Convert.ToDouble(topMeasureCPSList[k].Replace(" CPS", "")) + " CPS");
                            }
                            GlobalFunc.logManager.WriteLog("Measurement Top Detector " + BKManager.measureList.Count);
                        }
                    }
                }
                else if (selDetectorLabel.Text.Contains("Bottom"))
                {
                    List<Roi> roiList2 = GlobalFunc.GetRoiData(GlobalFunc.bottomScriptSet.MeasureData);
                    if (roiList2.Count > 0)
                    {
                        liveTime = roiList2[0].lifeTime;
                        reportDateTime = roiList2[0].dt;
                        bottomMeasureCPSList = BKManager.CalBk(roiList2, ref BKManager.dt, ref BKManager.lifeTime, 1);
                        for (int k = 0; k < bottomMeasureCPSList.Count; k++)
                        {
                            BKManager.measureList.Add(Convert.ToDouble(bottomMeasureCPSList[k].Replace(" CPS", "")));
                            GlobalFunc.logManager.WriteLog("Measurement operation (bottom detector): " +
                                                            Convert.ToDouble(bottomMeasureCPSList[k].Replace(" CPS", "")) + " CPS");
                        }
                        GlobalFunc.logManager.WriteLog("Measurement Bottom Detector " + BKManager.measureList.Count);
                    }
                }
                else if (selDetectorLabel.Text.Contains("Dual"))
                {
                    List<Roi> roiList1 = GlobalFunc.GetRoiData(GlobalFunc.topScriptSet.MeasureData);
                    List<Roi> roiList2 = GlobalFunc.GetRoiData(GlobalFunc.bottomScriptSet.MeasureData);
                    if (roiList1.Count > 0)
                    {
                        liveTime = roiList1[0].lifeTime;
                        reportDateTime = roiList1[0].dt;
                        topMeasureCPSList = BKManager.CalBk(roiList1, ref BKManager.dt, ref BKManager.lifeTime, 1);
                        bottomMeasureCPSList = BKManager.CalBk(roiList2, ref BKManager.dt, ref BKManager.lifeTime, 1);
                        List<string> dualList = BKManager.CalDualBk(roiList1, roiList2, ref BKManager.dt, ref BKManager.lifeTime, 1);
                        for(int k = 0; k < dualList.Count; k++)
                        {
                            BKManager.measureList.Add(Convert.ToDouble(dualList[k].Replace(" CPS", "")));
                            GlobalFunc.logManager.WriteLog("Measurement operation (dual detector): " +
                                                            Convert.ToDouble(dualList[k].Replace(" CPS", "")) + " CPS");
                        }
                        GlobalFunc.logManager.WriteLog("Measurement Dual Detector " + BKManager.measureList.Count);
                    }
                }
                if (BKManager.measureList.Count == 0)
                {
                    countTryFile++;
                }
                else
                {
                    break;
                }
                if (countTryFile % 3 == 0)
                {
                    string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("tryAgain"), GlobalFunc.rm.GetString("noticeMsg"),
                                GlobalFunc.rm.GetString("yesBtnTxt"), GlobalFunc.rm.GetString("noBtnTxt"), false, 0);
                    if (btnClicked == "1")
                    {

                    }
                    else if (btnClicked == "2")
                    {
                        break;
                    }
                }
                countSleep = 0;
            }
            GlobalFunc.logManager.WriteLog("End Measurement operation data");
        }

        private void mainProfileComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (mainProfileComboBox.SelectedIndex > 0)
            {
                GlobalFunc.loadProfile = new Profile();
                bool loaded = SetProfile(true);
         
                finalResult = new List<double>();
                detected = new List<int>();
                detectorName = GlobalFunc.loadProfile.Detector;
                if (detectorName.ToLower() == GlobalFunc.basicSetting.PresetDetector.ToLower())
                {
                    CheckButtonEnable();
                    if (currPanel == "background")
                    {
                        if (loaded)
                        {
                            runBkOpBtn.Enabled = true;
                        }
                        else
                        {
                            runBkOpBtn.Enabled = false;
                        }
                        Mea_RunBtn.Enabled = false;
                    }
                    else if (currPanel == "measure")
                    {
                        runBkOpBtn.Enabled = false;
                        if (loaded)
                        {
                            Mea_RunBtn.Enabled = true;
                        }
                        else
                        {
                            Mea_RunBtn.Enabled = false;
                        }
                    }
                }
                else
                {
                    string buttonID = CustomMessageBox.Show("Profile Detector not equal to Preset Detector", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
            }
        }

        public void CheckButtonEnable()
        {
            if (currPanel == "warmup" || currPanel == "")
            {
                profileLabel.Visible = false;
                mainProfileComboBox.Visible = false;
                label14.Visible = false;
                selProfileLabel.Visible = false;
                label15.Visible = false;
                selDetectorLabel.Visible = false;
                label16.Visible = false;
                selQtyIsotope.Visible = false;

                measurePanel.Visible = false;
                Mea_RunBtn.Enabled = false;

                currBgStatusPanel.Visible = false;
                runBkGroup.Visible = false;
                runBkOpBtn.Enabled = false;
                updateBkStatus.Enabled = false;


            }
            else if (currPanel == "background")
            {
                profileLabel.Visible = true;
                mainProfileComboBox.Visible = true;
                label14.Visible = true;
                selProfileLabel.Visible = true;
                label15.Visible = true;
                selDetectorLabel.Visible = true;
                label16.Visible = true;
                selQtyIsotope.Visible = true;

                measurePanel.Visible = false;
                Mea_RunBtn.Enabled = false;

                currBgStatusPanel.Visible = true;
                runBkGroup.Visible = true;
                runBkOpBtn.Enabled = false;                
                updateBkStatus.Enabled = false;
                
            }
            else if (currPanel == "measure")
            {
                profileLabel.Visible = true;
                mainProfileComboBox.Visible = true;
                label14.Visible = true;
                selProfileLabel.Visible = true;
                label15.Visible = true;
                selDetectorLabel.Visible = true;
                label16.Visible = true;
                selQtyIsotope.Visible = true;

                measurePanel.Visible = true;
                Mea_RunBtn.Enabled = false;

                currBgStatusPanel.Visible = false;
                runBkGroup.Visible = false;
                runBkOpBtn.Enabled = false;
                updateBkStatus.Enabled = false;

           
            }
        }
    }
}
