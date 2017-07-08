using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Reflection;
using System.Resources;
using System.Threading;
using System.Runtime.InteropServices;
using UMCBILib;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Data.SqlServerCe;
using System.Data;

namespace LCMS
{
    public static class GlobalFunc
    {
        public static SplashScreen splashScreen;
        public static DummyScreen dummyScreen;
        public static MainForm mainForm;
        public static LoginForm loginForm;
        public static SettingForm settingForm;
        public static TestConnection tc;
        public static LogManager logManager = new LogManager();
        public static OutputWord outputWord;
        public static ScriptEditor se;

        public static Cal cal = new Cal();
        public static Profile loadProfile = new Profile();
    
        public static BasicSetting basicSetting = new BasicSetting();
        public static int intIOAddress = 0;
        public static bool getTemp = true;
        public static ScriptSet dualScriptSet = new ScriptSet();
        public static ScriptSet topScriptSet = new ScriptSet();
        public static ScriptSet bottomScriptSet = new ScriptSet();

        public static List<Isotope> isotopeList1 = new List<Isotope>();
        public static List<Isotope> isotopeList2 = new List<Isotope>();
        public static List<Isotope> isotopeList3 = new List<Isotope>();
        public static List<Isotope> isotopeList4 = new List<Isotope>();
        public static List<Isotope> isotopeList5 = new List<Isotope>();
        public static List<Isotope> isotopeList6 = new List<Isotope>();
        
        public static int bufferTime = 0;
        public static double warmupTime = 0.0;
        
        public static int rsl = 0;
        public static int ru = 0;
        public static int mlr = 0;
        public static double det1_icr_alarm = 0.0;
        public static double det2_icr_alarm = 0.0;
        public static double dual_icr_alarm = 0.0;

        public static double det1_temp = 0.0;
        public static double det2_temp = 0.0;

        public static double det1_icr = 0.0;
        public static double det2_icr = 0.0;

        public static bool det1_active = true;
        public static bool det2_active = true;

        public static bool showGetBKDataBtn = false;

        //hardcode
        //public static int loginStatus = 1;
        public static int loginStatus = 0; //0 = no login, 1 = supervisor, 2 = operator, 3 = administrator
        public static int userID = 0;

        public static string warmupBeginScript = "";
        public static string warmupEndScript = "";
            
        public static int warmupStatus = 0; // 0 = ready, 1 = running, 2 = error

        public static Assembly assembly = null;
        public static ResourceManager rm = null;

        public static bool connectedToDetector1 = false;
        public static bool connectedToDetector2 = false;

        public static int DetectorID1 = 0;
        public static int DetectorID2 = 0;
        public static int currentDetectorID = 0;

        public static void ResetConnectDetector()
        { 
            connectedToDetector1 = false;
            connectedToDetector2 = false;
        }

        public static bool CheckDetector(string toggleDetector)
        {
            bool passDetector = false;
            if (toggleDetector.Contains("Top"))
            {
                GlobalFunc.tc.Invoke(new MethodInvoker(delegate { GlobalFunc.tc.checkDetector1Connection(); }));
                if (GlobalFunc.connectedToDetector1)
                {
                    passDetector = true;
                }
            }
            else if (toggleDetector.Contains("Bottom"))
            {
                GlobalFunc.tc.Invoke(new MethodInvoker(delegate { GlobalFunc.tc.checkDetector2Connection(); }));
                if (GlobalFunc.connectedToDetector2)
                {
                    passDetector = true;
                }
            }
            else if (toggleDetector.Contains("Dual"))
            {
                GlobalFunc.tc.Invoke(new MethodInvoker(delegate { GlobalFunc.tc.checkDetector1Connection(); }));
                GlobalFunc.tc.Invoke(new MethodInvoker(delegate { GlobalFunc.tc.checkDetector2Connection(); }));
                if (GlobalFunc.connectedToDetector1 && GlobalFunc.connectedToDetector2)
                {
                    passDetector = true;
                }
            }
            return passDetector;
        }

        public static bool checkActive(string toggleDetector)
        {
            bool pass = false;
            if (toggleDetector.Contains("Top"))
            {
                GlobalFunc.tc.Invoke(new MethodInvoker(delegate { GlobalFunc.tc.isDetector1Active(); }));
                if (GlobalFunc.det1_active)
                {
                    pass = true;
                }
            }
            else if (toggleDetector.Contains("Bottom"))
            {
                GlobalFunc.tc.Invoke(new MethodInvoker(delegate { GlobalFunc.tc.isDetector2Active(); }));
                if (GlobalFunc.det2_active)
                {
                    pass = true;
                }
            }
            else if (toggleDetector.Contains("Dual"))
            {
                GlobalFunc.tc.Invoke(new MethodInvoker(delegate { GlobalFunc.tc.isDetector1Active(); }));
                GlobalFunc.tc.Invoke(new MethodInvoker(delegate { GlobalFunc.tc.isDetector2Active(); }));
                if (GlobalFunc.det1_active && GlobalFunc.det2_active)
                {
                    pass = true;
                }
            }
            return pass;
        }

        public static void ShowDetectorError(string toggleDetector)
        {
            if (toggleDetector == "Top" && !GlobalFunc.connectedToDetector1)
            {
                GlobalFunc.SetAlarmBox(6);
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector1"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                if (buttonID == "1")
                {
                    GlobalFunc.settingForm.Invoke(new MethodInvoker(delegate { GlobalFunc.settingForm.Enabled = true;
                                        GlobalFunc.settingForm.EnableControl();
                    }));
                    GlobalFunc.SetAlarmBox(0);
                    GlobalFunc.settingForm.UpdateStatusLabel("Alarm", 255, 0, 0);
                }
            }
            else if (toggleDetector == "Bottom" && !GlobalFunc.connectedToDetector2)
            {
                GlobalFunc.SetAlarmBox(6);
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector2"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                if (buttonID == "1")
                {
                    GlobalFunc.settingForm.Invoke(new MethodInvoker(delegate { GlobalFunc.settingForm.Enabled = true;
                                        GlobalFunc.settingForm.EnableControl();
                    }));
                    GlobalFunc.SetAlarmBox(0);
                    GlobalFunc.settingForm.UpdateStatusLabel("Alarm", 255, 0, 0);
                }
            }
            else
            {
                if (!GlobalFunc.connectedToDetector1 && !GlobalFunc.connectedToDetector2)
                {
                    GlobalFunc.SetAlarmBox(6);
                    string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("failConnectDetector1") + System.Environment.NewLine + GlobalFunc.rm.GetString("failConnectDetector2"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                    if (buttonID == "1")
                    {
                        GlobalFunc.settingForm.Invoke(new MethodInvoker(delegate { GlobalFunc.settingForm.Enabled = true;
                                        GlobalFunc.settingForm.EnableControl();
                        }));
                        GlobalFunc.settingForm.UpdateStatusLabel("Alarm", 255, 0, 0);
                        GlobalFunc.SetAlarmBox(0);
                    }
                }
            }
        }

        public static string EncryptPassword(string password)
        {
            MD5 md5Hasher = MD5.Create();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(password));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        public static string TranslateDate(string date)
        {
            string day = "";
            string month = "";
            string year = "";
            string hour = "";
            string min = "";
            string second = "";

            string datePart = date.Substring(0, date.IndexOf(" "));
            string timePart = date.Substring(date.IndexOf(" ") + 1, date.Length - (date.IndexOf(" ") + 1));

            day = datePart.Substring(0, datePart.IndexOf("-"));
            month = datePart.Substring(datePart.IndexOf("-") + 1, datePart.LastIndexOf("-") - (datePart.IndexOf("-") + 1));
            year = datePart.Substring(datePart.LastIndexOf("-") + 1, datePart.Length - (datePart.LastIndexOf("-") + 1));

            hour = timePart.Substring(0, timePart.IndexOf(":"));
            min = timePart.Substring(timePart.IndexOf(":") + 1, timePart.LastIndexOf(":") - (timePart.IndexOf(":") + 1));
            second = timePart.Substring(timePart.LastIndexOf(":") + 1, timePart.Length - (timePart.LastIndexOf(":") + 1));

            return (year + "-" + month + "-" + day + " " + hour + ":" + min + ":" + second).ToString();
        }

        public static int CountMin(DateTime checkDate)
        {
            DateTime now = DateTime.Now;
            TimeSpan ts = now - checkDate;
            return ts.Minutes;
        }

        public static void EditScript(string searchText, string filePath, string newValue, bool checkRoiExist, string roiFileCommand)
        {
            string line;
            List<string> lineList = new List<string>();
            using (StreamReader reader = File.OpenText(filePath))
            {               
                while ((line = reader.ReadLine()) != null)
                {
                    if (line.Contains(searchText))
                    {
                        if (line.Contains("SET_PRESET_LIVE"))
                        {
                            line = searchText + " " + newValue;
                        }
                        else
                        {
                            line = "RECALL_ROI" + " " + newValue;
                        }
                    }
                    else if (checkRoiExist && line.Contains(roiFileCommand))
                    {
                        //GlobalFunc.FindJobRoi(line, roiFileCommand); //hardcode
                    }
                 
                    lineList.Add(line);                    
                }
            }
            File.Delete(filePath);
            File.WriteAllLines(filePath, lineList);                        
        }

        public static void FindJobRoi(string line, string removeString)
        {
            string roiFileName = line.Replace(removeString, "").Replace("\"", "").Trim();
            if (File.Exists(roiFileName))
            {
                File.Delete(roiFileName);
            }
            File.WriteAllText(roiFileName, "");
        }

        public static List<Roi> GetRoiData(string path)
        {
            List<Roi> roiList = new List<Roi>();
            string line;
            using (StreamReader reader = File.OpenText(path))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    Roi roi = new Roi();
                    if (line.Contains("ACQ")) //get time and life time
                    {
                        string dateString = line.Substring(line.IndexOf("ACQ") + 4, line.IndexOf("RT") - (line.IndexOf("ACQ") + 4)).Trim().Replace("at", "@");
                        string date = dateString.Substring(0, dateString.IndexOf("@"));
                        string time = dateString.Substring(dateString.IndexOf("@") + 1, dateString.Length - (dateString.IndexOf("@") + 1));
                        int day = Convert.ToInt32(date.Substring(0, date.IndexOf("-")));
                        int month = 1;
                        string mon = date.Substring(date.IndexOf("-") + 1, date.LastIndexOf("-") - (date.IndexOf("-") + 1));
                        switch (mon)
                        {
                            case "Jan": month = 1;
                                break;
                            case "Feb": month = 2;
                                break;
                            case "Mar": month = 3;
                                break;
                            case "Apr": month = 4;
                                break;
                            case "May": month = 5;
                                break;
                            case "Jun": month = 6;
                                break;
                            case "Jul": month = 7;
                                break;
                            case "Aug": month = 8;
                                break;
                            case "Sep": month = 9;
                                break;
                            case "Oct": month = 10;
                                break;
                            case "Nov": month = 11;
                                break;
                            case "Dec": month = 12;
                                break;
                        }
                        int year = Convert.ToInt32(date.Substring(date.LastIndexOf("-") + 1, date.Length - (date.LastIndexOf("-") + 1)));
                        roi.dt = DateTime.Parse(year + "-" + month + "-" + day + time);

                        string lt = line.Substring(line.IndexOf("LT = ") + 5, line.Length - (line.IndexOf("LT = ") + 5));
                        roi.lifeTime = Convert.ToDecimal(lt);
                        roiList.Add(roi);
                    }
                    else if (line.Contains("Gross")) // get gross
                    {
                        roi.gross = Convert.ToInt32(line.Substring(line.IndexOf("Gross = ") + 8, line.IndexOf("Net") - (line.IndexOf("Gross = ") + 8)));
                        roiList.Add(roi);
                    }
                    
                }
            }
            return roiList;
        }

        public static Profile LoadProfile(string fileName)
        {
            try
            {
                Profile loadProfile = new Profile();
                string line;
                string path = @"C:\LCMS\Profile\" + fileName;
                using (StreamReader reader = File.OpenText(path))
                {
                    bool matrixTopOn = false;
                    bool matrixBottomOn = false;
                    bool matrixDual = false;
                    IsoSeq ise = new IsoSeq();
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("Profile Name:"))
                        {
                            loadProfile.ProfileName = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("Detector:"))
                        {
                            loadProfile.Detector = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("Location:"))
                        {
                            loadProfile.Location = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("Alarm Type:"))
                        {
                            loadProfile.Alarm = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("No. of Region:"))
                        {
                            loadProfile.NoOfRegion = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1)));
                        }
                        else if (line.Contains("Date:"))
                        {
                            loadProfile.Date = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("Roi Path1:"))
                        {
                            loadProfile.RoiPath1 = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("Roi Path2:"))
                        {
                            loadProfile.RoiPath2 = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("Live Time:"))
                        {
                            loadProfile.LiveTime = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("Background Top CPS:"))
                        {
                            string inner = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                            if (inner != "")
                            {
                                loadProfile.BkTopCPS = inner.Split(',').ToList();
                            }
                        }
                        else if (line.Contains("Background Bottom CPS:"))
                        {
                            string inner = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                            if (inner != "")
                            {
                                loadProfile.BKBottomCPS = inner.Split(',').ToList();
                            }
                        }
                        else if (line.Contains("Quantity of Isotope:"))
                        {
                            loadProfile.Qty = Convert.ToInt32(line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1)));
                        }
                        else if (line.Contains("Roi#"))
                        {
                            ise = new IsoSeq();
                            ise.Number = Convert.ToInt32(line.Substring(line.IndexOf("#") + 1, 1));
                            ise.Name = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("Activity:"))
                        {
                            ise.Activity = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                            ise.Activity = (Convert.ToDouble(ise.Activity) / 1000).ToString();
                        }
                        else if (line.Contains("Ref Datetime:"))
                        {
                            ise.RefDateTime = DateTime.Parse(line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1)));
                        }
                        else if (line.Contains("Measure Time:"))
                        {
                            ise.MeasureTime = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("DIL"))
                        {
                            ise.DIL = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("AL") && !line.Contains("%"))
                        {
                            ise.AL = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("AL%"))
                        {
                            ise.AL_PC = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("Top CPS:") && !line.Contains("Bakcground"))
                        {
                            ise.TopCPS = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                        }
                        else if (line.Contains("Bottom CPS:") && !line.Contains("Background"))
                        {
                            ise.BottomCPS = line.Substring(line.IndexOf(":") + 1, line.Length - (line.IndexOf(":") + 1));
                            loadProfile.IsoSeqList.Add(ise);
                        }
                        else if (line.Contains("Matrix Top:"))
                        {
                            matrixTopOn = true;
                            matrixBottomOn = false;
                            matrixDual = false;
                        }
                        else if (matrixTopOn)
                        {
                            if (!line.Contains("Matrix Top:") && line != "" && !line.Contains("Matrix Bottom:") && !line.Contains("Matrix Dual:"))
                            {
                                string[] mTop = line.Split(',');
                                for (int i = 0; i < mTop.Length; i++)
                                {
                                    if (mTop[i] != "")
                                    {
                                        loadProfile.MatrixTop.Add(Convert.ToSingle(mTop[i]));
                                    }
                                }
                            }
                            else if (line.Contains("Matrix Bottom:"))
                            {
                                matrixTopOn = false;
                                matrixBottomOn = true;
                                matrixDual = false;
                            }
                        }
                        else if (line.Contains("Matrix Bottom:"))
                        {
                            matrixTopOn = false;
                            matrixBottomOn = true;
                            matrixDual = false;
                        }
                        else if (matrixBottomOn)
                        {
                            if (!line.Contains("Matrix Bottom:") && line != "" && !line.Contains("Matrix Top:") && !line.Contains("Matrix Dual:"))
                            {
                                string[] mBottom = line.Split(',');
                                for (int i = 0; i < mBottom.Length; i++)
                                {
                                    if (mBottom[i] != "")
                                    {
                                        loadProfile.MatrixBottom.Add(Convert.ToSingle(mBottom[i]));
                                    }
                                }
                            }
                            else if (line.Contains("Matrix Dual:"))
                            {
                                matrixTopOn = false;
                                matrixBottomOn = false;
                                matrixDual = true;
                            }
                        }
                        else if (line.Contains("Matrix Dual:"))
                        {
                            matrixTopOn = false;
                            matrixBottomOn = false;
                            matrixDual = true;
                        }
                        else if (matrixDual)
                        {
                            if (!line.Contains("Matrix Dual:") && line != "" && !line.Contains("Matrix Top:") && !line.Contains("Matrix Bottom:"))
                            {
                                string[] mDual = line.Split(',');
                                for (int i = 0; i < mDual.Length; i++)
                                {
                                    if (mDual[i] != "")
                                    {
                                        loadProfile.MatrixDual.Add(Convert.ToSingle(mDual[i]));
                                    }
                                }
                            }
                        }
                    }
                }

                GlobalFunc.LoadIsotopXML();
                for (int i = 0; i < loadProfile.IsoSeqList.Count; i++)
                {
                    loadProfile.Activity.Add(Convert.ToDouble(loadProfile.IsoSeqList[i].Activity));
                    loadProfile.Ref_Date.Add(loadProfile.IsoSeqList[i].RefDateTime);
                    if (loadProfile.Alarm == "DIL")
                    {
                        loadProfile.AlarmLevel.Add(Convert.ToDouble(loadProfile.IsoSeqList[i].DIL));
                        loadProfile.AlarmPCLevel.Add(Convert.ToDouble(loadProfile.IsoSeqList[i].AL_PC));
                    }
                    else if (loadProfile.Alarm == "AL")
                    {
                        loadProfile.AlarmLevel.Add(Convert.ToDouble(loadProfile.IsoSeqList[i].AL));
                        loadProfile.AlarmPCLevel.Add(Convert.ToDouble(loadProfile.IsoSeqList[i].AL_PC));
                    }
                    loadProfile.AlarmPCLevel.Add(Convert.ToDouble(loadProfile.IsoSeqList[i].AL_PC));
                    for (int j = 0; j < GlobalFunc.isotopeList1.Count; j++)
                    {
                        if (loadProfile.IsoSeqList[i].Name == GlobalFunc.isotopeList1[j].Code)
                        {
                            loadProfile.HalfTime.Add(Convert.ToDouble(GlobalFunc.isotopeList1[j].HalfLife));
                            loadProfile.RatioPeak.Add(Convert.ToDouble(GlobalFunc.isotopeList1[j].Bop));

                            break;
                        }
                    }
                }

                if (loadProfile.Detector == "Dual")
                {
                    for (int i = 0; i < loadProfile.BkTopCPS.Count; i++)
                    {
                        double value = Convert.ToDouble(loadProfile.BkTopCPS[i]) + Convert.ToDouble(loadProfile.BKBottomCPS[i]);
                        //string strValue = Math.Round(value, 2).ToString();
                        loadProfile.FinalBkCPS.Add(value);
                    }
                    loadProfile.FinalMatrix = loadProfile.MatrixDual;
                }
                else if (loadProfile.Detector == "Top")
                {
                    for (int i = 0; i < loadProfile.BkTopCPS.Count; i++)
                    {
                        loadProfile.FinalBkCPS.Add(Convert.ToDouble(loadProfile.BkTopCPS[i]));
                    }
                    loadProfile.FinalMatrix = loadProfile.MatrixTop;
                }
                else if (loadProfile.Detector == "Bottom")
                {
                    for (int i = 0; i < loadProfile.BKBottomCPS.Count; i++)
                    {
                        loadProfile.FinalBkCPS.Add(Convert.ToDouble(loadProfile.BKBottomCPS[i]));
                    }
                    loadProfile.FinalMatrix = loadProfile.MatrixBottom;
                }
                loadProfile.FinalBkCPS.RemoveAt(loadProfile.FinalBkCPS.Count - 1);
                return loadProfile;
            }
            catch (Exception ex)
            {
                GlobalFunc.logManager.WriteLog("load profile error:" + ex.Message);
                return null;
            }
        }

        public static void SetBackgroundBKList(string toggleDetector)
        {
            if (toggleDetector.Contains("Top"))
            {
                if (File.Exists(GlobalFunc.topScriptSet.BackgrounData))
                {
                    List<Roi> roiList1 = GlobalFunc.GetRoiData(GlobalFunc.topScriptSet.BackgrounData);
                    if (roiList1.Count > 0)
                    {
                        BKManager.bkList = BKManager.CalBk(roiList1, ref BKManager.dt, ref BKManager.lifeTime, 1);
                    }
                }
            }
            else if (toggleDetector.Contains("Bottom"))
            {
                List<Roi> roiList2 = GlobalFunc.GetRoiData(GlobalFunc.bottomScriptSet.BackgrounData);
                if (roiList2.Count > 0)
                {
                    BKManager.bkList = BKManager.CalBk(roiList2, ref BKManager.dt, ref BKManager.lifeTime, 1);
                }
            }
            else if (toggleDetector.Contains("Dual"))
            {
                List<Roi> roiList1 = GlobalFunc.GetRoiData(GlobalFunc.topScriptSet.BackgrounData);
                List<Roi> roiList2 = GlobalFunc.GetRoiData(GlobalFunc.bottomScriptSet.BackgrounData);
                if (roiList1.Count > 0)
                {
                    BKManager.bkList = BKManager.CalDualBk(roiList1, roiList2, ref BKManager.dt, ref BKManager.lifeTime, 1);
                }
            }
            
        }

        public static void SaveIsotopXML()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(Isotope[]), "");
            TextWriter textWriter = new StreamWriter(@Directory.GetCurrentDirectory() + @"\xml\Isotope.xml");
            using (TextWriter tw = new Utf8StringWriter())
            {
                serializer.Serialize(textWriter, GlobalFunc.isotopeList1.ToArray());
            }
            textWriter.Close();
        }

        public static void LoadIsotopXML()
        {
            XmlSerializer deserializer3 = new XmlSerializer(typeof(List<Isotope>));
            TextReader textReader3 = new StreamReader(@Directory.GetCurrentDirectory() + @"\xml\Isotope.xml");
            GlobalFunc.isotopeList1 = (List<Isotope>)deserializer3.Deserialize(textReader3);
            GlobalFunc.isotopeList1.Sort((x, y) => x.Mpe.CompareTo(y.Mpe));

            textReader3.Close();
            GlobalFunc.isotopeList2 = GlobalFunc.isotopeList1;
            GlobalFunc.isotopeList3 = GlobalFunc.isotopeList1;
            GlobalFunc.isotopeList4 = GlobalFunc.isotopeList1;
            GlobalFunc.isotopeList5 = GlobalFunc.isotopeList1;
            GlobalFunc.isotopeList6 = GlobalFunc.isotopeList1;
        }

        public class Utf8StringWriter : TextWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        public static double Math45(double value)
        {
            double returnValue = 0.0;
            try
            {
                string strValue = value.ToString();
                string threeDigit = strValue.Substring(strValue.IndexOf(".") + 1, 3);
                string previous = strValue.Substring(0, strValue.IndexOf("."));
                double lastOne = 0.0;
                if (threeDigit.Substring(2, 1) == "E")
                {
                    lastOne = Convert.ToDouble("0.001");
                }
                else
                {
                    lastOne = Convert.ToDouble("0.00" + threeDigit.Substring(2, 1));
                }
                double firstTwo = Convert.ToDouble("0." + threeDigit.Substring(0, 2));
                if (lastOne >= 0.005)
                {
                    firstTwo += 0.01;
                }
                string lastValue = (Convert.ToDouble(previous) + firstTwo).ToString();
                returnValue = Convert.ToDouble(lastValue);
            }
            catch
            {
                returnValue = value;
            }
            return returnValue;
        }

        public static double Math45Pt1(double value)
        {
            double returnValue = 0.0;
            try 
            {
                string strValue = value.ToString();
                string twoDigit = strValue.Substring(strValue.IndexOf(".") + 1, 2);
                string previous = strValue.Substring(0, strValue.IndexOf("."));
                double lastOne = Convert.ToDouble("0.0" + twoDigit.Substring(1, 1));
                double firstone = Convert.ToDouble("0." + twoDigit.Substring(0, 1));
                if (lastOne >= 0.05)
                {
                    firstone += 0.1;
                }
                string lastValue = (Convert.ToDouble(previous) + firstone).ToString();
                returnValue = Convert.ToDouble(lastValue);
            }
            catch
            {
                returnValue = value;
            }
            return returnValue;
        }

        public static void SetMeasureSetting()
        {
            SqlCeConnection con = new SqlCeConnection(Properties.Settings.Default.lcmsConnectionString);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand bkCmd = new SqlCeCommand("select * from time", con);
            bkCmd.CommandType = CommandType.Text;
            SqlCeDataReader bkDr = bkCmd.ExecuteReader();
            if (bkDr.Read())
            {
                GlobalFunc.bufferTime = Convert.ToInt32(bkDr["bufferTime"]);
                GlobalFunc.warmupTime = Convert.ToDouble(bkDr["warmupTime"]);
                GlobalFunc.dual_icr_alarm = Convert.ToDouble(bkDr["alarm"]);
                GlobalFunc.rsl = Convert.ToInt32(bkDr["rsl"]);
                GlobalFunc.ru = Convert.ToInt32(bkDr["ru"]);
                GlobalFunc.mlr = Convert.ToInt32(bkDr["mlr"]);
            }
            bkDr.Close();

            SqlCeCommand cmd = new SqlCeCommand("select * from icr", con);
            cmd.CommandType = CommandType.Text;
            SqlCeDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (dr["detector"].ToString() == "Top")
                {
                   
                    GlobalFunc.det1_icr_alarm = Convert.ToDouble(dr["icr"]);
                }
                else if (dr["detector"].ToString() == "Bottom")
                {
                    
                    GlobalFunc.det2_icr_alarm = Convert.ToDouble(dr["icr"]);
                }
            }
            dr.Close();
            con.Close();
        }

        public static void SetAlarmBox(int indicator)
        {
            try
            {
                PortAccess.Output(GlobalFunc.intIOAddress, indicator);
            }
            catch { }
        }       
    }
}
