using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using NetOffice;
using Word = NetOffice.WordApi;
using NetOffice.WordApi.Enums;
using MSWord = Microsoft.Office.Interop.Word;
using System.IO;
using System.Data.SqlServerCe;
using System.Diagnostics;
namespace LCMS
{
    public partial class OutputWord : Form
    {
        public OutputWord()
        {
            InitializeComponent();
            //CreateTable();
        }

        public void CreateTable(string location, string vehicleNo, string hcn, 
                            string farmCode, string poo, string species, string qty,
                            string sampleSize, string destination, string remarks, int mID, 
                            Profile profile, List<double> finalResult, List<int> detector, string refNo, int satis,
                            string alarmType, double ratio_sum, List<double> uncert, bool noError, ref string filePath)
        {
            try
            {
                
                // start word and turn off msg boxes
                Word.Application wordApplication = new Word.Application();
                wordApplication.DisplayAlerts = WdAlertLevel.wdAlertsNone;

                // add a new document
                Word.Document newDocument = wordApplication.Documents.Add();
                newDocument.PageSetup.TopMargin = (float)20;
                newDocument.PageSetup.LeftMargin = (float)20;
                newDocument.PageSetup.RightMargin = (float)20;
                newDocument.PageSetup.BottomMargin = (float)20;  
                // insert some text       
                wordApplication.Selection.Font.Name = "Times New Roman";
                wordApplication.Selection.Font.Color = WdColor.wdColorBlack;
                wordApplication.Selection.Font.Bold = 3;
                wordApplication.Selection.Font.Size = 12;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                wordApplication.Selection.Font.Spacing = 1;                
                wordApplication.Selection.TypeText("Food and Environmental Hygiene Department" + System.Environment.NewLine + "Veterinary Public Health Section" + System.Environment.NewLine);

                wordApplication.Selection.Font.Color = WdColor.wdColorBlack;
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.Font.Size = 12;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                wordApplication.Selection.Font.Spacing = 1;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.TypeText("Livestock Contamination Monitoring System" + System.Environment.NewLine);

                Word.Table table = newDocument.Tables.Add(wordApplication.Selection.Range, 5, 2);

                // insert some text into the cells
                table.Cell(1, 1).Select();
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.TypeText("Date: " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year);
                wordApplication.Selection.TypeText(System.Environment.NewLine);
                wordApplication.Selection.TypeText("Time: "+ DateTime.Now.ToShortTimeString());
                wordApplication.Selection.TypeText(System.Environment.NewLine);
                SqlCeConnection con = new SqlCeConnection(Properties.Settings.Default.lcmsConnectionString);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("select description from location  where code = @location", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@location", location);
                string locationDes = "";
                SqlCeDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    locationDes = dr["description"].ToString();
                }
                dr.Close();
                wordApplication.Selection.TypeText("Location: " + locationDes);
                
                table.Cell(1, 2).Select();
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;

                string day = DateTime.Now.Day.ToString();
                if (day.Length != 1)
                {
                    day = "0" + day;
                }
                string month = DateTime.Now.Month.ToString();
                if(month.Length != 1)
                {
                    month = "0" + month;
                }

                //wordApplication.Selection.TypeText("Reference No.: (" + refNo +")" + day +"/" + month + "/" + DateTime.Now.Year);
                wordApplication.Selection.TypeText("Reference No.: " + location + "/" + DateTime.Now.Year + "/" + refNo);

                table.Cell(2, 1).Select();                
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Basic Information:" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.TypeText("Vehicle no.: " + vehicleNo + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Health certificate no.: " + hcn + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Farm code: " + farmCode + System.Environment.NewLine);
                cmd = new SqlCeCommand("select chiname from origin where engname = @poo", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@poo", poo);
                dr = cmd.ExecuteReader();
                string chiPoo = "";
                if (dr.Read())
                {
                    chiPoo = dr["chiname"].ToString();
                }
                dr.Close();
                if (chiPoo != "")
                {
                    wordApplication.Selection.TypeText("Place of origin: " + poo + ", " + chiPoo + System.Environment.NewLine);
                }
                else if (chiPoo == "")
                {
                    wordApplication.Selection.TypeText("Place of origin: " + poo + System.Environment.NewLine);
                }                
                wordApplication.Selection.TypeText("Species: " + species + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Quantity: " + qty + " head" + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Sample size: " + sampleSize + " kg" + System.Environment.NewLine);
                
                SqlCeCommand dCmd = new SqlCeCommand("select description from destination with (nolock) where code = @destination", con);
                dCmd.CommandType = CommandType.Text;
                string longDescription = "";
                dCmd.Parameters.AddWithValue("@destination", destination);
                SqlCeDataReader dDr = dCmd.ExecuteReader();
                if (dDr.Read())
                {
                    longDescription = dDr["description"].ToString();
                }
                dDr.Close();              
                wordApplication.Selection.TypeText("Destination: " + longDescription);
                
                table.Cell(2, 2).Select();
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Remarks: " + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.TypeText(remarks);

                table.Cell(3, 1).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Measurement:" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                if (GlobalFunc.getTemp == true)
                {
                    if (profile.Detector == "Top")
                    {
                        wordApplication.Selection.TypeText("Temperature Indicator: " + GlobalFunc.Math45Pt1(GlobalFunc.det1_temp) + "°C" + System.Environment.NewLine);
                    }
                    else if (profile.Detector == "Bottom")
                    {
                        wordApplication.Selection.TypeText("Temperature Indicator: " + GlobalFunc.Math45Pt1(GlobalFunc.det2_temp) + "°C" + System.Environment.NewLine);
                    }
                    else if (profile.Detector == "Dual")
                    {
                        wordApplication.Selection.TypeText("Temperature Indicator: " + GlobalFunc.Math45Pt1((GlobalFunc.det1_temp + GlobalFunc.det2_temp) / 2) + "°C" + System.Environment.NewLine);
                    }
                }
                wordApplication.Selection.TypeText("Count Time:" + BKManager.supposeSecond + "(Seconds)" + System.Environment.NewLine);

                int tableColNum = 2;
                if (GlobalFunc.ru == 1)
                {
                    tableColNum += 1;
                }
                if (GlobalFunc.mlr == 1)
                {
                    tableColNum += 1;
                }
                Word.Table subTable = newDocument.Tables.Add(wordApplication.Selection.Range, 7, tableColNum);
                subTable.Cell(1, 1).Select();
                subTable.Cell(1, 1).SetWidth(78, WdRulerStyle.wdAdjustNone);  
                wordApplication.Selection.TypeText("Radionuclide");

                subTable.Cell(1, 2).Select();                
                subTable.Cell(1, 2).SetWidth(78, WdRulerStyle.wdAdjustNone);  
                wordApplication.Selection.TypeText("Activity");
                
                if (GlobalFunc.ru == 1)
                {
                    subTable.Cell(1, 3).Select();
                    subTable.Cell(1, 3).SetWidth(90, WdRulerStyle.wdAdjustNone);  
                    wordApplication.Selection.TypeText("Uncertainty, %");
                }
                // 0 or 1 
                wordApplication.Selection.FitTextWidth = 1;
                if (GlobalFunc.mlr == 1)
                {
                    subTable.Cell(1, 4).Select();
                    subTable.Cell(1, 4).SetWidth(30, WdRulerStyle.wdAdjustNone);                      
                    wordApplication.Selection.TypeText("MDC");
                }
                //loop depends
                int index = 2;
                for (int i = 0; i < profile.IsoSeqList.Count; i++)
                {
                    subTable.Cell(index, 1).Select();
                    subTable.Cell(index, 1).SetWidth(78, WdRulerStyle.wdAdjustNone);  
                    wordApplication.Selection.TypeText(profile.IsoSeqList[i].Name);
                    index += 1;
                }

                index = 2;
                for (int i = 0; i < finalResult.Count; i++)
                {
                    subTable.Cell(index, 2).Select();
                    subTable.Cell(index, 2).SetWidth(78, WdRulerStyle.wdAdjustNone);  
                    wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    //double actValue = finalResult[i];
                    if (double.IsNaN(finalResult[i]))
                    {
                        wordApplication.Selection.TypeText("0.00 Bq/kg");
                    }
                    else if (double.IsInfinity(finalResult[i]))
                    {
                        wordApplication.Selection.TypeText("0.00 Bq/kg");
                    }
                    else if (finalResult[i] < 0.0000)
                    {
                        wordApplication.Selection.TypeText("0.00 Bq/kg");
                    }
                    else
                    {
                        double actValue = GlobalFunc.Math45(finalResult[i]);
                        wordApplication.Selection.TypeText(actValue + " Bq/kg");                    
                    }
                    
                    index += 1;
                }

                index = 2;
                if (GlobalFunc.ru == 1)
                {
                    for (int i = 0; i < profile.IsoSeqList.Count; i++)
                    {
                        subTable.Cell(index, 3).Select();
                        subTable.Cell(index, 3).SetWidth(90, WdRulerStyle.wdAdjustNone);  
                        wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        double uncertValue = uncert[i];
                        if (double.IsNaN(uncertValue))
                        {
                            uncertValue = 0.0;
                        }
                        else if (double.IsInfinity(uncertValue))
                        {
                            uncertValue = 0.0;
                        }
                        else if (uncertValue < 0.0)
                        {
                            uncertValue = 0.0;
                        }
                        else
                        {
                            uncertValue = GlobalFunc.Math45(uncertValue);
                        }

                        wordApplication.Selection.TypeText(uncertValue.ToString());
                        index += 1;
                    }
                }

                index = 2;
                if (GlobalFunc.mlr == 1)
                {
                    for (int i = 0; i < detector.Count; i++)
                    {
                        subTable.Cell(index, 4).Select();                        
                        wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (detector[i] == 0)
                        {
                            wordApplication.Selection.TypeText("D");
                        }
                        else
                        {
                            wordApplication.Selection.TypeText("U");
                        }

                        index += 1;
                    }
                    table.Cell(3, 1).Select();
                    wordApplication.Selection.InsertAfter("*D=Detected, U=Undetected" + System.Environment.NewLine);
                }
                
                if (GlobalFunc.rsl == 1)
                {
                    table.Cell(3, 1).Select();
                    if (ratio_sum > 1.0)
                    { wordApplication.Selection.InsertAfter("Ratio Sum: **" + ratio_sum); }
                    else
                    {
                        wordApplication.Selection.InsertAfter("Ratio Sum:" + ratio_sum);
                    }
                }
              
                table.Cell(3, 2).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Calibration used: " + profile.ProfileName + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                /*wordApplication.Selection.TypeText("Energy:      FWHM:" + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Efficiencies:      Library:" + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Background:      " + System.Environment.NewLine + System.Environment.NewLine);*/
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                if (alarmType == "DIL")
                {
                    wordApplication.Selection.TypeText("Warning level " + "(" + alarmType + ", Bq/kg, AL%) :" + System.Environment.NewLine);
                }
                else
                {
                    wordApplication.Selection.TypeText("Warning level " + "(" + alarmType + ", Bq/kg) :" + System.Environment.NewLine);
                }
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                for (int k = 0; k < profile.IsoSeqList.Count; k++)
                {
                    string text = profile.IsoSeqList[k].Name + ": ";
                    if (alarmType == "DIL")
                    {
                        text += profile.IsoSeqList[k].DIL + ", " + profile.IsoSeqList[k].AL_PC;
                    }
                    else if (alarmType == "AL")
                    {
                        text += profile.IsoSeqList[k].AL; 
                    }
                    wordApplication.Selection.TypeText(text + System.Environment.NewLine);
                }
                if (GlobalFunc.loadProfile.Detector == "Top")
                {
                    if (GlobalFunc.det1_icr > GlobalFunc.det1_icr_alarm)
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + " " + "**" + GlobalFunc.det1_icr + " counts/s");
                    }
                    else
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + " " + GlobalFunc.det1_icr + " counts/s");
                    }
                }
                else if (GlobalFunc.loadProfile.Detector == "Bottom")
                {
                    if (GlobalFunc.det2_icr > GlobalFunc.det2_icr_alarm)
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + " " + "**" + GlobalFunc.det2_icr + " counts/s");
                    }
                    else
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + " " + GlobalFunc.det2_icr + " counts/s");
                    }
                    
                }
                else if (GlobalFunc.loadProfile.Detector == "Dual")
                {
                    if ((GlobalFunc.det1_icr + GlobalFunc.det2_icr) / 2 >= GlobalFunc.dual_icr_alarm)
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + " " + "**" + (GlobalFunc.det1_icr + GlobalFunc.det2_icr) / 2 + " counts/s");
                    }
                    else
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + " " + (GlobalFunc.det1_icr + GlobalFunc.det2_icr) / 2 + " counts/s");
                    }
                }
                table.Cell(4, 1).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Result assessment" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                if (satis == 0)
                { wordApplication.Selection.TypeText("Satisfactory"); }
                else if (satis == 1)
                { wordApplication.Selection.TypeText("Unsatisfactory"); }
                

                table.Cell(4, 2).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                
                table.Cell(5, 1).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Responsible officer" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.TypeText("Testing Officer" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;

                Word.Table signTable1 = newDocument.Tables.Add(wordApplication.Selection.Range, 4, 2);
                signTable1.Cell(1, 1).Select();
                wordApplication.Selection.TypeText("Signature: ");
                signTable1.Cell(1, 2).Select();
                wordApplication.Selection.TypeText("____________");
                
                signTable1.Cell(2, 1).Select();
                wordApplication.Selection.TypeText("Name: ");
                signTable1.Cell(2, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable1.Cell(3, 1).Select();
                wordApplication.Selection.TypeText("Designation: ");
                signTable1.Cell(3, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable1.Cell(4, 1).Select();
                wordApplication.Selection.TypeText("Date:");
                signTable1.Cell(4, 2).Select();
                wordApplication.Selection.TypeText("____________");

                table.Cell(5, 2).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.TypeText( System.Environment.NewLine + "Reviewing Officer" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;


                Word.Table signTable2 = newDocument.Tables.Add(wordApplication.Selection.Range, 4, 2);
                signTable2.Cell(1, 1).Select();
                wordApplication.Selection.TypeText("Signature: ");
                signTable2.Cell(1, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable2.Cell(2, 1).Select();
                wordApplication.Selection.TypeText("Name: ");
                signTable2.Cell(2, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable2.Cell(3, 1).Select();
                wordApplication.Selection.TypeText("Designation: ");
                signTable2.Cell(3, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable2.Cell(4, 1).Select();
                wordApplication.Selection.TypeText("Date:");
                signTable2.Cell(4, 2).Select();
                wordApplication.Selection.TypeText("____________");

                if (!Directory.Exists(@"C:\LCMS\WordReport"))
                {
                    Directory.CreateDirectory(@"C:\LCMS\WordReport");
                }

                // save the document
                string fileExtension = GetDefaultExtension(wordApplication);
                //object documentFile =  string.Format("c:\abc", Application.StartupPath, fileExtension);

                string strMonth = "";
                string strDay = "";
                string strHour = "";
                string strMin = "";

                if (DateTime.Now.Month <= 9)
                {
                    strMonth = "0" + DateTime.Now.Month;
                }
                else
                {
                    strMonth = DateTime.Now.Month.ToString();
                }

                if (DateTime.Now.Day <= 9)
                {
                    strDay = "0" + DateTime.Now.Day;
                }
                else 
                {
                    strDay = DateTime.Now.Day.ToString();
                }

                if (DateTime.Now.Hour <= 9)
                {
                    strHour = "0" + DateTime.Now.Hour.ToString();
                }
                else
                {
                    strHour = DateTime.Now.Hour.ToString();
                }
                if (DateTime.Now.Minute <= 9)
                {
                    strMin = "0" + DateTime.Now.Minute.ToString();
                }
                else
                {
                    strMin = DateTime.Now.Minute.ToString();
                }

                string date = DateTime.Now.Year.ToString().Substring(2, 2) + strMonth + strDay
                        + strHour + strMin;

                object documentFile = @"C:\LCMS\WordReport\" + profile.ProfileName + "_"
                    + date + "(" + refNo + ")" + fileExtension;
                newDocument.SaveAs(documentFile);

                filePath = documentFile.ToString();

                // close word and dispose reference
                wordApplication.Quit();
                wordApplication.Dispose();
                con.Close();
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private string GetDefaultExtension(Word.Application application)
        {
            double version = Convert.ToDouble(application.Version);
            if (version >= 12.00)
                return ".docx";
            else
                return ".doc";
        }

        public void CreateReportTable(int mID, string pathName)
        {
            try
            {
                SqlCeConnection con = new SqlCeConnection(Properties.Settings.Default.lcmsConnectionString);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();

                #region load db
                string location = "";
                string vehicleNo = "";
                string hcn = "";
                string farmCode = "";
                string poo = "";
                string species = "";
                string qty = "";
                string sampleSize = "";
                string destination = "";
                string remarks = "";
                int year = 0;
                int month = 0;
                int day = 0;
                string time = "";
                string refNo = "";
                string ratioSum = "";
                string icr = "";
                string profileName = "";
                int icrError = 0;
                int rsl = 0;
                int ru = 0;
                int mlr = 0;
                string detector = "";
                int satis = 0;
                int showTemp = 0;
                SqlCeCommand mCmd = new SqlCeCommand("select * from measure where Id = @id", con);
                mCmd.CommandType = CommandType.Text;
                mCmd.Parameters.AddWithValue("@id", mID);
                SqlCeDataReader mDr = mCmd.ExecuteReader();
                if (mDr.Read())
                {
                    location = mDr["location"].ToString();
                    vehicleNo = mDr["vehicleNo"].ToString();
                    hcn = mDr["hcn"].ToString();
                    farmCode = mDr["farmCode"].ToString();
                    poo = mDr["poo"].ToString();
                    species = mDr["species"].ToString();
                    qty = mDr["qty"].ToString();
                    sampleSize = mDr["sampleSize"].ToString();
                    destination = mDr["destination"].ToString();
                    remarks = mDr["remarks"].ToString();
                    year = Convert.ToInt32(mDr["year"]);
                    month = Convert.ToInt32(mDr["month"]);
                    day = Convert.ToInt32(mDr["day"]);
                    time = mDr["time"].ToString();
                    refNo = mDr["refNo"].ToString();
                    ratioSum = mDr["ratioSum"].ToString();
                    icr = mDr["icr"].ToString();
                    profileName = mDr["profileName"].ToString();
                    icrError = Convert.ToInt32(mDr["icrError"]);
                    rsl = Convert.ToInt32(mDr["rsl"]);
                    ru = Convert.ToInt32(mDr["ru"]);
                    mlr = Convert.ToInt32(mDr["mlr"]);
                    detector = mDr["detector"].ToString();
                    if (mDr["satis"] != DBNull.Value)
                    {
                        satis = Convert.ToInt32(mDr["satis"]);
                    }
                    if (mDr["showTemp"] != DBNull.Value)
                    {
                        showTemp = Convert.ToInt32(mDr["showTemp"]);
                    }
                }
                mDr.Close();

                string temperature = "";
                string countTime = "";
                string icr1 = "";
                string icr2 = "";
                string icrAlarmLevel1 = "";
                string icrAlarmLevel2 = "";
                SqlCeCommand dCmd = new SqlCeCommand("select * from measureDetail where mID = @id", con);
                dCmd.CommandType = CommandType.Text;
                dCmd.Parameters.AddWithValue("@id", mID);
                SqlCeDataReader dDr = dCmd.ExecuteReader();
                if (dDr.Read())
                {
                    temperature = dDr["temperature"].ToString();
                    countTime = dDr["countTime"].ToString();
                    icr1 = dDr["icr1"].ToString();
                    icr2 = dDr["icr2"].ToString();
                    icrAlarmLevel1 = dDr["icrAlarmLevel1"].ToString();
                    icrAlarmLevel2 = dDr["icrAlarmLevel2"].ToString();
                }
                dDr.Close();
                #endregion

                // start word and turn off msg boxes
                Word.Application wordApplication = new Word.Application();
                wordApplication.DisplayAlerts = WdAlertLevel.wdAlertsNone;


                // add a new document
                Word.Document newDocument = wordApplication.Documents.Add();
                newDocument.PageSetup.TopMargin = (float)20;
                newDocument.PageSetup.LeftMargin = (float)20;
                newDocument.PageSetup.RightMargin = (float)20;
                newDocument.PageSetup.BottomMargin = (float)20;  

                // insert some text       
                wordApplication.Selection.Font.Name = "Times New Roman";
                wordApplication.Selection.Font.Color = WdColor.wdColorBlack;
                wordApplication.Selection.Font.Bold = 3;
                wordApplication.Selection.Font.Size = 12;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                wordApplication.Selection.Font.Spacing = 1;
                wordApplication.Selection.TypeText("Food and Environmental Hygiene Department" + System.Environment.NewLine + "Veterinary Public Health Section" + System.Environment.NewLine);

                wordApplication.Selection.Font.Color = WdColor.wdColorBlack;
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.Font.Size = 12;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphCenter;
                wordApplication.Selection.Font.Spacing = 1;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.TypeText("Livestock Contamination Monitoring System" + System.Environment.NewLine);

                Word.Table table = newDocument.Tables.Add(wordApplication.Selection.Range, 5, 2);

                // insert some text into the cells
                table.Cell(1, 1).Select();
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.TypeText("Date: " + DateTime.Now.Day + "/" + DateTime.Now.Month + "/" + DateTime.Now.Year);
                wordApplication.Selection.TypeText(System.Environment.NewLine);
                wordApplication.Selection.TypeText("Time: " + DateTime.Now.ToShortTimeString());
                wordApplication.Selection.TypeText(System.Environment.NewLine);
                SqlCeCommand cmd = new SqlCeCommand("select description from location where code = @location", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@location", location);
                string locationDes = "";
                SqlCeDataReader dr = cmd.ExecuteReader();
                if (dr.Read())
                {
                    locationDes = dr["description"].ToString();
                }
                dr.Close();
                wordApplication.Selection.TypeText("Location: " + locationDes);

                table.Cell(1, 2).Select();
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                            

                //wordApplication.Selection.TypeText("Reference No.: (" + refNo +")" + day +"/" + month + "/" + DateTime.Now.Year);
                wordApplication.Selection.TypeText("Reference No.: " + location + "/" + year + "/" + refNo);

                table.Cell(2, 1).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Basic Information:" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.TypeText("Vehicle no.: " + vehicleNo + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Health certificate no.: " + hcn + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Farm code: " + farmCode + System.Environment.NewLine);
                cmd = new SqlCeCommand("select chiname from origin where engname = @poo", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@poo", poo);
                dr = cmd.ExecuteReader();
                string chiPoo = "";
                if (dr.Read())
                {
                    chiPoo = dr["chiname"].ToString();
                }
                dr.Close();
                if (chiPoo != "")
                {
                    wordApplication.Selection.TypeText("Place of origin: " + poo + ", " + chiPoo + System.Environment.NewLine);
                }
                else if (chiPoo == "")
                {
                    wordApplication.Selection.TypeText("Place of origin: " + poo + System.Environment.NewLine);
                }
                wordApplication.Selection.TypeText("Species: " + species + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Quantity: " + qty + " head" + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Sample size: " + sampleSize + " kg" + System.Environment.NewLine);

                dCmd = new SqlCeCommand("select description from destination with (nolock) where code = @destination", con);
                dCmd.CommandType = CommandType.Text;
                string longDescription = "";
                dCmd.Parameters.AddWithValue("@destination", destination);
                dDr = dCmd.ExecuteReader();
                if (dDr.Read())
                {
                    longDescription = dDr["description"].ToString();
                }
                dDr.Close();
                wordApplication.Selection.TypeText("Destination: " + longDescription);

                table.Cell(2, 2).Select();
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Remarks: " + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.TypeText(remarks);

                table.Cell(3, 1).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Measurement:" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                if (showTemp == 1)
                {
                    wordApplication.Selection.TypeText("Temperature Indicator: " + temperature + "°C" + System.Environment.NewLine);
                }
                wordApplication.Selection.TypeText("Count Time:" + countTime + "(Seconds)" + System.Environment.NewLine);

                int tableColNum = 2;
                if (ru == 1)
                {
                    tableColNum += 1;
                }
                if (mlr == 1)
                {
                    tableColNum += 1;
                }
                Word.Table subTable = newDocument.Tables.Add(wordApplication.Selection.Range, 7, tableColNum);
            
                subTable.Cell(1, 1).Select();                
                subTable.Cell(1, 1).SetWidth(78, WdRulerStyle.wdAdjustNone);           
                wordApplication.Selection.TypeText("Radionuclide");

                subTable.Cell(1, 2).Select();
                subTable.Cell(1, 2).SetWidth(78, WdRulerStyle.wdAdjustNone);
                wordApplication.Selection.TypeText("Activity");

                if (ru == 1)
                {
                    subTable.Cell(1, 3).Select();                    
                    subTable.Cell(1, 3).SetWidth(90, WdRulerStyle.wdAdjustNone);
                    wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;    
                    wordApplication.Selection.TypeText("Uncertainty, %");
                }
                // 0 or 1 
                
                if (mlr == 1)
                {
                    subTable.Cell(1, 4).Select();
                    subTable.Cell(1, 4).SetWidth(30, WdRulerStyle.wdAdjustNone);  
                    //wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;   
                    wordApplication.Selection.TypeText("MDC");
                }
                //loop depends
                int index = 2;
                SqlCeCommand miCmd = new SqlCeCommand("select isotopeName from measureIsotope with (nolock) where mID = @id", con);
                miCmd.CommandType = CommandType.Text;
                miCmd.Parameters.AddWithValue("@id", mID);
                SqlCeDataReader miDr = miCmd.ExecuteReader();
                while(miDr.Read()) 
                {
                    subTable.Cell(index, 1).Select();
                    subTable.Cell(index, 1).SetWidth(78, WdRulerStyle.wdAdjustNone);               
                    wordApplication.Selection.TypeText(miDr["isotopeName"].ToString());
                    index += 1;
                }
                miDr.Close();
                index = 2;
                miCmd = new SqlCeCommand("select activity from measureIsotope with (nolock) where mID = @id", con);
                miCmd.CommandType = CommandType.Text;
                miCmd.Parameters.AddWithValue("@id", mID);
                miDr = miCmd.ExecuteReader();
                while(miDr.Read())                 
                {
                    subTable.Cell(index, 2).Select();
                    subTable.Cell(index, 2).SetWidth(78, WdRulerStyle.wdAdjustNone);            
                    wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                    double actValue = Convert.ToDouble(miDr["activity"]);

                    if (double.IsNaN(actValue))
                    {
                        wordApplication.Selection.TypeText("0.00 Bq/kg");
                    }
                    else if(double.IsInfinity(actValue))
                    {
                        wordApplication.Selection.TypeText("0.00 Bq/kg");
                    }
                    else if (actValue < 0.0)
                    {
                        wordApplication.Selection.TypeText("0.00 Bq/kg");
                    }
                    else
                    {
                        actValue = GlobalFunc.Math45(actValue);
                        
                        wordApplication.Selection.TypeText(actValue.ToString() + " Bq/kg");
                    }

                    
                    index += 1;
                }
                miDr.Close();
                index = 2;
                if (ru == 1)
                {
                    miCmd = new SqlCeCommand("select uncertainty from measureIsotope with (nolock) where mID = @id", con);
                    miCmd.CommandType = CommandType.Text;
                    miCmd.Parameters.AddWithValue("@id", mID);
                    miDr = miCmd.ExecuteReader();
                    while (miDr.Read())   
                    {
                        subTable.Cell(index, 3).Select();
                        subTable.Cell(index, 3).SetWidth(90, WdRulerStyle.wdAdjustNone);                        
                        wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;

                        double uncertValue = Convert.ToDouble(miDr["uncertainty"]);
                        if (double.IsNaN(uncertValue))
                        {
                            uncertValue = 0.0;
                        }
                        else if (double.IsInfinity(uncertValue))
                        {
                            uncertValue = 0.0;
                        }
                        else if (uncertValue < 0.0)
                        {
                            uncertValue = 0.0;
                        }
                        else
                        {
                            uncertValue = GlobalFunc.Math45(uncertValue);
                        }

                        wordApplication.Selection.TypeText(uncertValue.ToString());
                        index += 1;
                    }
                    miDr.Close();
                }

                index = 2;
                if (mlr == 1)
                {
                    miCmd = new SqlCeCommand("select detected from measureIsotope with (nolock) where mID = @id", con);
                    miCmd.CommandType = CommandType.Text;
                    miCmd.Parameters.AddWithValue("@id", mID);
                    miDr = miCmd.ExecuteReader();
                    while (miDr.Read())   
                    {
                        subTable.Cell(index, 4).Select();                        
                        wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                        if (Convert.ToInt32(miDr["detected"]) == 0)
                        {
                            wordApplication.Selection.TypeText("D");
                        }
                        else
                        {
                            wordApplication.Selection.TypeText("U");
                        }
                        index += 1;
                    }
                    table.Cell(3, 1).Select();
                    wordApplication.Selection.InsertAfter("*D=Detected, U=Undetected" + System.Environment.NewLine);
                }
                
                if (rsl == 1)
                {
                    table.Cell(3, 1).Select();
                    if (Convert.ToDouble(ratioSum) > 1.0)
                    {
                        wordApplication.Selection.InsertAfter("Ratio Sum: **" + ratioSum);
                    }
                    else
                    {
                        wordApplication.Selection.InsertAfter("Ratio Sum:" + ratioSum);
                    }
                }

                table.Cell(3, 2).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Calibration used: " + profileName + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                /*wordApplication.Selection.TypeText("Energy:      FWHM:" + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Efficiencies:      Library:" + System.Environment.NewLine);
                wordApplication.Selection.TypeText("Background:      " + System.Environment.NewLine + System.Environment.NewLine);*/
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                string alarmType = "";
                miCmd = new SqlCeCommand("select alarmType from measureIsotope with (nolock) where mID = @id", con);
                miCmd.CommandType = CommandType.Text;
                miCmd.Parameters.AddWithValue("@id", mID);
                miDr = miCmd.ExecuteReader();
                if (miDr.Read())
                {
                    alarmType = miDr["alarmType"].ToString();
                }
                miDr.Close();
                if (alarmType == "DIL")
                {
                    wordApplication.Selection.TypeText("Warning level " + "(" + alarmType + ", Bq/kg, AL%) :" + System.Environment.NewLine);
                }
                else
                {
                    wordApplication.Selection.TypeText("Warning level " + "(" + alarmType + ", Bq/kg) :" + System.Environment.NewLine);
                }
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                miCmd = new SqlCeCommand("select isotopeName, alarmType, alarmLevel, alarmLevelPC from measureIsotope with (nolock) where mID = @id", con);
                miCmd.CommandType = CommandType.Text;
                miCmd.Parameters.AddWithValue("@id", mID);
                miDr = miCmd.ExecuteReader();
                while (miDr.Read())   
                {
                    string text = miDr["isotopeName"].ToString() + ": ";
                    if (miDr["alarmType"].ToString() == "DIL")
                    {
                        text += miDr["alarmLevel"].ToString() + ", " + miDr["alarmLevelPC"].ToString();
                    }
                    else if (miDr["alarmType"].ToString() == "AL")
                    {
                        text += miDr["alarmLevel"].ToString();
                    }
                    wordApplication.Selection.TypeText(text + System.Environment.NewLine);
                }
                miDr.Close();
                if (detector == "Top")
                {
                    if (Convert.ToDouble(icr1) > Convert.ToDouble(icrAlarmLevel1))
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + " " + "**" + icr1 + " counts/s");
                    }
                    else
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + " " + icr1 + " counts/s");
                    }
                }
                else if (detector == "Bottom")
                {
                    if (Convert.ToDouble(icr2) > Convert.ToDouble(icrAlarmLevel2))
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + " " + "**" + icr2 + " counts/s");
                    }
                    else
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + " " + icr2 + " counts/s");
                    }
                }
                else if (detector == "Dual")
                {
                    if ((Convert.ToDouble(icr1) + Convert.ToDouble(icr2)) / 2 > (Convert.ToDouble(icrAlarmLevel1) + Convert.ToDouble(icrAlarmLevel2)) / 2)
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + "**" + (Convert.ToDouble(icr1) + Convert.ToDouble(icr2)) / 2 + " counts/s");
                    }
                    else
                    {
                        wordApplication.Selection.TypeText("Input Count Rate:" + (Convert.ToDouble(icr1) + Convert.ToDouble(icr2)) / 2 + " counts/s");
                    }
                }
                table.Cell(4, 1).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Result assessment" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;
                if (satis == 0)
                {
                    wordApplication.Selection.TypeText("Satisfactory");
                }
                else
                {
                    wordApplication.Selection.TypeText("Unsatisfactory");
                }

                table.Cell(4, 2).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;

                table.Cell(5, 1).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 1;
                wordApplication.Selection.TypeText("Responsible officer" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.TypeText("Testing Officer" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;

                Word.Table signTable1 = newDocument.Tables.Add(wordApplication.Selection.Range, 4, 2);
                signTable1.Cell(1, 1).Select();
                wordApplication.Selection.TypeText("Signature: ");
                signTable1.Cell(1, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable1.Cell(2, 1).Select();
                wordApplication.Selection.TypeText("Name: ");
                signTable1.Cell(2, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable1.Cell(3, 1).Select();
                wordApplication.Selection.TypeText("Designation: ");
                signTable1.Cell(3, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable1.Cell(4, 1).Select();
                wordApplication.Selection.TypeText("Date:");
                signTable1.Cell(4, 2).Select();
                wordApplication.Selection.TypeText("____________");

                table.Cell(5, 2).Select();
                wordApplication.Selection.Font.Size = 10;
                wordApplication.Selection.Borders[WdBorderType.wdBorderLeft].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderRight].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderTop].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.Borders[WdBorderType.wdBorderBottom].LineStyle = WdLineStyle.wdLineStyleSingle;
                wordApplication.Selection.ParagraphFormat.Alignment = WdParagraphAlignment.wdAlignParagraphLeft;
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineSingle;
                wordApplication.Selection.Font.Bold = 0;
                wordApplication.Selection.TypeText(System.Environment.NewLine + "Reviewing Officer" + System.Environment.NewLine);
                wordApplication.Selection.Font.Underline = WdUnderline.wdUnderlineNone;
                wordApplication.Selection.Font.Bold = 0;


                Word.Table signTable2 = newDocument.Tables.Add(wordApplication.Selection.Range, 4, 2);
                signTable2.Cell(1, 1).Select();
                wordApplication.Selection.TypeText("Signature: ");
                signTable2.Cell(1, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable2.Cell(2, 1).Select();
                wordApplication.Selection.TypeText("Name: ");
                signTable2.Cell(2, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable2.Cell(3, 1).Select();
                wordApplication.Selection.TypeText("Designation: ");
                signTable2.Cell(3, 2).Select();
                wordApplication.Selection.TypeText("____________");

                signTable2.Cell(4, 1).Select();
                wordApplication.Selection.TypeText("Date:");
                signTable2.Cell(4, 2).Select();
                wordApplication.Selection.TypeText("____________");

                // save the document
                string fileExtension = GetDefaultExtension(wordApplication);
                //object documentFile =  string.Format("c:\abc", Application.StartupPath, fileExtension);

                string strMonth = "";
                string strDay = "";
                string strHour = "";
                string strMin = "";

                if (DateTime.Now.Month <= 9)
                {
                    strMonth = "0" + DateTime.Now.Month;
                }
                else
                {
                    strMonth = DateTime.Now.Month.ToString();
                }

                if (DateTime.Now.Day <= 9)
                {
                    strDay = "0" + DateTime.Now.Day;
                }
                else
                {
                    strDay = DateTime.Now.Day.ToString();
                }

                if (DateTime.Now.Hour <= 9)
                {
                    strHour = "0" + DateTime.Now.Hour.ToString();
                }
                else
                {
                    strHour = DateTime.Now.Hour.ToString();
                }
                if (DateTime.Now.Minute <= 9)
                {
                    strMin = "0" + DateTime.Now.Minute.ToString();
                }
                else
                {
                    strMin = DateTime.Now.Minute.ToString();
                }

                string date = DateTime.Now.Year.ToString().Substring(2, 2) + strMonth + strDay
                        + strHour + strMin;

                object documentFile = pathName;
                newDocument.SaveAs(documentFile);                              

                // close word and dispose reference
                wordApplication.Quit();
                wordApplication.Dispose();
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void PrintWord(string filename)
        {
            /*Microsoft.Office.Interop.Word.Application wordInstance = new Microsoft.Office.Interop.Word.Application();
            //MemoryStream documentStream = getDocStream();
            FileInfo wordFile = new FileInfo(filename);
            object fileObject = wordFile.FullName;
            object oMissing = System.Reflection.Missing.Value;
            Microsoft.Office.Interop.Word.Document doc = wordInstance.Documents.Open(ref fileObject, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing);
            doc.Activate();
            doc.PrintOut(oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
            */
            ProcessStartInfo info2 = new ProcessStartInfo(filename);
            info2.Verb = "Print";
            info2.CreateNoWindow = true;
            info2.WindowStyle = ProcessWindowStyle.Hidden;
            Process.Start(info2);
        }
    }
}
