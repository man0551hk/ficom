using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace LCMS
{
    public static class BKManager
    {
        public static int jobID = 0;
        public static string beginBdgScript = "";
        public static string endBdgScript = "";
        public static string beginWarmUpScript = "";
        public static string endWarmUpScript = "";
        public static string beginCalScript = "";
        public static string endCalScript = "";
        public static string beginMeasureScript = "";
        public static string endMeasureScript = "";
        public static string updateScript = "";

        public static string tempRunScript = "";               
        public static string msg = "";

        public static double supposeMillSecond = 300000;
        public static double supposeSecond = 300;
        public static List<string> bkList = new List<string>();
        public static List<double> measureList = new List<double>();
        public static DateTime dt = new DateTime();
        public static decimal lifeTime = 0;

        public static List<double> background_cps = new List<double>(); //isotope num
        public static List<double> nuclide_cps = new List<double>(); // isotope num square
        public static List<double> activity = new List<double>(); // isotope num square
        public static List<double> halfTime = new List<double>(); // isotope num squareHalfTime
        public static List<DateTime> ref_Date = new List<DateTime>();
        public static List<DateTime> report_Date = new List<DateTime>();
        
        public static void SetTemp(int isotopeNumber, string toggleDetector, string originScriptName, string roiPath1, string roiPath2)
        {
            string tempName = "";
            string path = "";
            if (File.Exists(originScriptName))
            {
                path = originScriptName.Substring(0, originScriptName.LastIndexOf(@"\"));
                tempName = "temp_" + originScriptName.Substring(originScriptName.LastIndexOf(@"\") + 1, originScriptName.Length - (originScriptName.LastIndexOf(@"\") + 1));

                string fullName = path + "\\" + tempName;

                if (File.Exists(fullName))
                {
                    File.Delete(fullName);
                }

                File.Copy(originScriptName, fullName);
                tempRunScript = fullName;

                //Edit bkscript
                GlobalFunc.EditScript("SET_PRESET_LIVE", tempRunScript, Convert.ToString(supposeSecond), true, "RECALL_ROI");

                if (isotopeNumber == 4)
                {
                    if (toggleDetector == "Top")
                    {
                        if (roiPath1 != "")
                        {
                            GlobalFunc.EditScript(@"C:\LCMS\ROI\DET1_ROI5.ROI", tempRunScript, "\"" + roiPath1 + "\"", true, "RECALL_ROI");

                        }
                    }
                    else if (toggleDetector == "Bottom")
                    {
                        if (roiPath2 != "")
                        {
                            GlobalFunc.EditScript(@"C:\LCMS\ROI\DET2_ROI5.ROI", tempRunScript, "\"" + roiPath2 + "\"", true, "RECALL_ROI");
                        }
                    }
                    else
                    {
                        if (roiPath1 != "" && roiPath2 != "")
                        {
                            GlobalFunc.EditScript(@"C:\LCMS\ROI\DET1_ROI5.ROI", tempRunScript, "\"" + roiPath1 + "\"", true, "RECALL_ROI");
                            GlobalFunc.EditScript(@"C:\LCMS\ROI\DET2_ROI5.ROI", tempRunScript, "\"" + roiPath2 + "\"", true, "RECALL_ROI");
                        }
                    }
                }
                else if (isotopeNumber == 5)
                {
                    if (toggleDetector == "Top")
                    {
                        if (roiPath1 != "")
                        {
                            GlobalFunc.EditScript(@"C:\LCMS\ROI\DET1_ROI5.ROI", tempRunScript, "\"" + roiPath1 + "\"", true, "RECALL_ROI");
                        }
                    }
                    else if (toggleDetector == "Bottom")
                    {
                        if (roiPath2 != "")
                        {
                            GlobalFunc.EditScript(@"C:\LCMS\ROI\DET2_ROI5.ROI", tempRunScript, "\"" + roiPath2 + "\"", true, "RECALL_ROI");
                        }
                    }
                    else
                    {
                        if (roiPath1 != "" && roiPath2 != "")
                        {
                            GlobalFunc.EditScript(@"C:\LCMS\ROI\DET1_ROI5.ROI", tempRunScript, "\"" + roiPath1 + "\"", true, "RECALL_ROI");
                            GlobalFunc.EditScript(@"C:\LCMS\ROI\DET2_ROI5.ROI", tempRunScript, "\"" + roiPath2 + "\"", true, "RECALL_ROI");
                        }
                    }
                }
                else if (isotopeNumber == 6)
                {
                    if (toggleDetector == "Top")
                    {
                        GlobalFunc.EditScript(@"C:\LCMS\ROI\DET1_ROI5.ROI", tempRunScript, "\"" + roiPath1 + "\"", true, "RECALL_ROI");
                    }
                    else if (toggleDetector == "Bottom")
                    {
                        GlobalFunc.EditScript(@"C:\LCMS\ROI\DET2_ROI5.ROI", tempRunScript, "\"" + roiPath2 + "\"", true, "RECALL_ROI");
                    }
                    else
                    {
                        GlobalFunc.EditScript(@"C:\LCMS\ROI\DET1_ROI5.ROI", tempRunScript, "\"" + roiPath1 + "\"", true, "RECALL_ROI");
                        GlobalFunc.EditScript(@"C:\LCMS\ROI\DET2_ROI5.ROI", tempRunScript, "\"" + roiPath2 + "\"", true, "RECALL_ROI");
                    }
                }
               
            }            
        }

        public static List<string> CalDualBk(List<Roi> roiList1, List<Roi> roiList2, ref DateTime dt, ref decimal lifeTime, int numOfRegion)
        {           
            List<string> result = new List<string>();

            if (roiList1.Count > 0)
            {
                try
                {
                    double subCount = Convert.ToDouble((roiList1[1].gross / roiList1[0].lifeTime) + (roiList2[1].gross / roiList2[0].lifeTime));
                    subCount = Math.Round(subCount, 2);
                    result.Add(subCount + " CPS");
                }
                catch { }
                try
                {
                    double subCount = Convert.ToDouble((roiList1[2].gross / roiList1[0].lifeTime) + (roiList2[2].gross / roiList2[0].lifeTime));
                    subCount = Math.Round(subCount, 2);
                    result.Add(subCount + " CPS");
                }
                catch { }
                try
                {
                    double subCount = Convert.ToDouble((roiList1[3].gross / roiList1[0].lifeTime) + (roiList2[3].gross / roiList2[0].lifeTime));
                    subCount = Math.Round(subCount, 2);
                    result.Add(subCount + " CPS");
                }
                catch { }
                try
                {
                    double subCount = Convert.ToDouble((roiList1[4].gross / roiList1[0].lifeTime) + (roiList2[4].gross / roiList2[0].lifeTime));
                    subCount = Math.Round(subCount, 2);
                    result.Add(subCount + " CPS");
                }
                catch { }
            }
            if (roiList1.Count == 7)
            {
                double subCount = Convert.ToDouble((roiList1[5].gross / roiList1[0].lifeTime) + (roiList2[5].gross / roiList2[0].lifeTime) );
                subCount = Math.Round(subCount, 2);
                result.Add(subCount + " CPS");

                subCount = Convert.ToDouble((roiList1[6].gross / roiList1[0].lifeTime) + (roiList2[6].gross / roiList2[0].lifeTime));
                subCount = Math.Round(subCount, 2);
                result.Add(subCount + " CPS");
            }
            else if (roiList1.Count == 6)
            {
                double subCount = Convert.ToDouble((roiList1[5].gross / roiList1[0].lifeTime) + (roiList2[5].gross / roiList2[0].lifeTime));
                subCount = Math.Round(subCount, 2);
                result.Add(subCount + " CPS");
            }
            double totalCps = 0.0;
            for (int i = 0; i < result.Count; i++)
            {
                double cps = Convert.ToDouble(result[i].Replace(" CPS", ""));
                totalCps += cps;
            }
            result.Add(totalCps.ToString() + " CPS");
            dt = roiList1[0].dt;
            lifeTime = roiList1[0].lifeTime;
            return result;
        }

        public static List<string> CalBk(List<Roi> roiList, ref DateTime dt, ref decimal lifeTime, int numOfRegion)
        {
            List<string> result = new List<string>();         
            if (roiList.Count > 0)
            {
                try
                {
                    double subCount = Convert.ToDouble(roiList[1].gross / (roiList[0].lifeTime * numOfRegion));
                    subCount = Math.Round(subCount, 2);
                    result.Add(subCount + " CPS");
                }
                catch { }
                try
                {
                    double subCount = Convert.ToDouble(roiList[2].gross / (roiList[0].lifeTime * numOfRegion));
                    subCount = Math.Round(subCount, 2);
                    result.Add(subCount + " CPS");
                }
                catch { }
                try
                {
                    double subCount = Convert.ToDouble(roiList[3].gross / (roiList[0].lifeTime * numOfRegion));
                    subCount = Math.Round(subCount, 2);
                    result.Add(subCount + " CPS");
                }
                catch { }
                try
                {
                    double subCount = Convert.ToDouble(roiList[4].gross / (roiList[0].lifeTime * numOfRegion));
                    subCount = Math.Round(subCount, 2);
                    result.Add(subCount + " CPS");
                }
                catch { }
            }
            if (roiList.Count == 7)
            {
                double subCount = Convert.ToDouble(roiList[5].gross / (roiList[0].lifeTime * numOfRegion));
                subCount = Math.Round(subCount, 2);
                result.Add(subCount + " CPS");

                subCount = Convert.ToDouble(roiList[6].gross / (roiList[0].lifeTime * numOfRegion));
                subCount = Math.Round(subCount, 2);
                result.Add(subCount + " CPS");
            }
            else if (roiList.Count == 6)
            {
                double subCount = Convert.ToDouble(roiList[5].gross / (roiList[0].lifeTime * numOfRegion));
                subCount = Math.Round(subCount, 2);
                result.Add(subCount + " CPS");
            }
            double totalCps = 0.0;
            for (int i = 0; i < result.Count; i++)
            {
                double cps = Convert.ToDouble(result[i].Replace(" CPS", ""));
                totalCps += cps;
            }
            dt = roiList[0].dt;
            lifeTime = roiList[0].lifeTime;
            result.Add(totalCps.ToString() + " CPS");
            return result;
        }
    
        public static void SetLiveTime()
        {
            SqlCeConnection con = new SqlCeConnection(Properties.Settings.Default.lcmsConnectionString);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand bkCmd = new SqlCeCommand("select background from time", con);
            bkCmd.CommandType = CommandType.Text;
            SqlCeDataReader bkDr = bkCmd.ExecuteReader();
            if (bkDr.Read())
            {
                BKManager.supposeMillSecond = Convert.ToDouble(bkDr["background"]) * 60 * 1000;
                BKManager.supposeSecond = Convert.ToDouble(bkDr["background"]) * 60;
            }
            bkDr.Close();
            con.Close();
        }

        public static void SetMeasureTime()
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
                BKManager.supposeMillSecond = Convert.ToDouble(bkDr["sample"]) * 60 * 1000;
                BKManager.supposeSecond = Convert.ToDouble(bkDr["sample"]) * 60;
            }
            bkDr.Close();
            con.Close();
        }

        public static void SetWarmUpTime()
        {
            BKManager.supposeMillSecond = GlobalFunc.warmupTime * 60 * 1000;
            BKManager.supposeSecond = GlobalFunc.warmupTime * 60;
        }


    }
}
