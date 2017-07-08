using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LCMS
{
    public class Cal
    {
        public List<double> InvMatrixNxN(List<float> NumList)
        {
            int size = Convert.ToInt32(Math.Sqrt(NumList.Count));
            double[,] A = new double[size, size];
            double[,] B = new double[size, size];
            List<double> InvNumList = new List<double>();

            // transform into array format
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    A[i, j] = NumList[k];
                    k++;
                }
            }

            // initialize matrix B as identity matrix
            for (int i = 0; i < size; i++)
            {
                B[i, i] = 1.0;
            }

            // find the inverse using Gauss-Jordan elimination
            double temp;

            for (int i = 0; i < size; i++)
            {
                temp = A[i, i];
                for (int j = 0; j < size; j++)    // for 1
                {
                    A[i, j] = A[i, j] / temp;
                    B[i, j] = B[i, j] / temp;
                }

                for (int x = 0; x < size; x++)    // for 0
                {
                    if (x != i)
                    {
                        temp = A[x, i];
                        for (int y = 0; y < size; y++)
                        {
                            A[x, y] = A[x, y] - A[i, y] * temp;
                            B[x, y] = B[x, y] - B[i, y] * temp;
                        }
                    }
                }
            }

            // transform into list format
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    InvNumList.Add(B[i, j]);
                }
            }

            return InvNumList;
        }

        public List<double> MatrixAx_B(List<double> List_A, List<double> List_x)
        {
            int size = Convert.ToInt32(Math.Sqrt(List_A.Count));
            double[,] A = new double[size, size];
            double[,] B = new double[1, size];
            double[,] x = new double[1, size];
            List<double> List_B = new List<double>();

            // transform into array format
/*            for (int i = 0; i < size; i++)      // matrix A
            {
                A[0, i] = List_A[i];
            }

            int k = 0;
            for (int i = 0; i < size; i++)      // matrix x
            {
                for (int j = 0; j < size; j++)
                {
                    x[i, j] = List_x[k];
                    k++;
                }
            }
*/

            int k = 0;
            for (int i = 0; i < size; i++)      // matrix A
            {
                for (int j = 0; j < size; j++)
                {
                    A[i, j] = List_A[k];
                    k++;
                }
            }
            
            for (int i = 0; i < size; i++)      // matrix x
            {
                x[0, i] = List_x[i];
            }

            // Ax=B
            for (int i = 0; i < size; i++)
            {
                B[0, i] = 0.0;
                for (int j = 0; j < size; j++)
                {
                    B[0, i] = B[0, i] +  A[i, j] * x[0, j];
                }
            }

            // transform into list form
            for (int i = 0; i < size; i++)
            {
                List_B.Add(B[0, i]);
            }

            return List_B;
        }

        public double DecayEq(double A0, DateTime A0_Time, DateTime Current_Date, double HalfTime_DayUnit)
        {
            double Activity;
            double HalfTime_min = HalfTime_DayUnit * 1440;  // convert day to min.

            //DateTime now = DateTime.Parse("2014-08-27 13:21:37");//DateTime.Now;
            TimeSpan TimeDiff = Current_Date - A0_Time;

            Activity = A0 * Math.Exp((-0.693 * (TimeDiff.TotalMinutes / HalfTime_min)));
            GlobalFunc.logManager.WriteLog("Decay " + A0 + "* Math.Exp(-0.639 * (" + TimeDiff.TotalMinutes + " / " + HalfTime_min + ")");
            return Activity;
        }

        public List<float> Calibration_MatrixS(List<double> Background_cps, List<double> Nuclide_cps, List<double> Activity, List<double> HalfTime, 
            List<DateTime> Ref_Date, List<DateTime> Report_Date)
        {
            int size = Background_cps.Count;
            List<double> Activity_Current = new List<double>();
            List<float> List_MatrixElement = new List<float>();
            float[,] MatrixElement = new float[size, size];
            double[,] ROI_cps = new double[size, size];

            // calculate current activity of each nuclide
            for (int i = 0; i < size; i++)
            {
                Activity_Current.Add(DecayEq(Activity[i] * 1000, Ref_Date[i], Report_Date[i], HalfTime[i]));
                GlobalFunc.logManager.WriteLog("Cal: Activity[" + i + "] " + Activity[i] * 1000);
                GlobalFunc.logManager.WriteLog("Cal: Ref_Date[" + i + "] " + Ref_Date[i]);
                GlobalFunc.logManager.WriteLog("Cal: Report_Date[" + i + "] " + Report_Date[i]);
                GlobalFunc.logManager.WriteLog("Cal: HalfTime[" + i + "] " + HalfTime[i]);
            }

            // transform into array format
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ROI_cps[i, j] = Nuclide_cps[k];
                 
                    k++;
                }
            }

            // calculate matrix element
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    GlobalFunc.logManager.WriteLog("Cal: ROI_cps[" + i + ", " + j + "] " + ROI_cps[i, j]);
                    GlobalFunc.logManager.WriteLog("Cal: Background_cps["+ j + "] " + Background_cps[j]);
                    GlobalFunc.logManager.WriteLog("Cal: Activity_Current[" + i + "] " + Activity_Current[i]);
                    GlobalFunc.logManager.WriteLog("Cal: Martix:" + (ROI_cps[i, j] - Background_cps[j]) + " / " + GlobalFunc.Math45(Activity_Current[i]));
                    
                    //MatrixElement[j, i] = ;
                    if ((ROI_cps[i, j] - Background_cps[j]) < 0)
                    {
                        MatrixElement[j, i] = 0;
                    }
                    else
                    {
                        MatrixElement[j, i] = (Convert.ToSingle(ROI_cps[i, j]) - Convert.ToSingle(Background_cps[j])) / Convert.ToSingle(Activity_Current[i]);
                    }
                    GlobalFunc.logManager.WriteLog("Cal: MatrixElement[" + j + ", " + i + "] " + MatrixElement[j, i]);
                }
            }

            // transform into list format
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    List_MatrixElement.Add(MatrixElement[i, j]);
                }
            }            
            return List_MatrixElement;
        }

        public List<float> Calibration_MatrixD(List<double> Background1_cps, List<double> Background2_cps, List<double> Nuclide1_cps, List<double> Nuclide2_cps,
            List<double> Activity, List<double> HalfTime, List<DateTime> Ref1_Date, List<DateTime> Ref2_Date, List<DateTime> Report1_Date, List<DateTime> Report2_Date)
        {

            int size = Background1_cps.Count;
            List<double> Activity_Current = new List<double>();
            List<double> Background_cps = new List<double>();
            List<float> List_MatrixElement = new List<float>();
            float[,] MatrixElement = new float[size, size];
            double[,] ROI_cps = new double[size, size];

            // calculate current activity and background of each nuclide
            for (int i = 0; i < size; i++)
            {
                Activity_Current.Add((DecayEq(Activity[i] * 1000, Ref1_Date[i], Report1_Date[i], HalfTime[i]) +
                    DecayEq(Activity[i] * 1000, Ref2_Date[i], Report2_Date[i], HalfTime[i])) / 2);
                Background_cps.Add(Background1_cps[i] + Background2_cps[i]);

                GlobalFunc.logManager.WriteLog("Cal: Activity[" + i + "] " + Activity[i] * 1000);
                GlobalFunc.logManager.WriteLog("Cal: HalfTime[" + i + "] " + HalfTime[i]);

                GlobalFunc.logManager.WriteLog("Cal: Ref1_Date[" + i + "] " + Ref1_Date[i]);
                GlobalFunc.logManager.WriteLog("Cal: Report1_Date[" + i + "] " + Report1_Date[i]);
                GlobalFunc.logManager.WriteLog("Cal: Ref2_Date[" + i + "] " + Ref2_Date[i]);
                GlobalFunc.logManager.WriteLog("Cal: Report2_Date[" + i + "] " + Report1_Date[i]);

                GlobalFunc.logManager.WriteLog("Cal: Background1_cps[" + i + "] " + Background1_cps[i]);
                GlobalFunc.logManager.WriteLog("Cal: Background2_cps[" + i + "] " + Background2_cps[i]);
            }

            // transform into array format
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    ROI_cps[i, j] = Nuclide1_cps[k] + Nuclide2_cps[k];
                    
                    k++;
                }
            }

            // calculate matrix element
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {                    
                    GlobalFunc.logManager.WriteLog("Cal: ROI_cps[" + i + ", " + j + "] " + ROI_cps[i, j]);
                    GlobalFunc.logManager.WriteLog("Cal: Background_cps[" + j + "] " + Background_cps[j]);                   
                    GlobalFunc.logManager.WriteLog("Cal: Activity_Current[" + i + "] " + GlobalFunc.Math45(Activity_Current[i]));                    
                    GlobalFunc.logManager.WriteLog("Cal: Martix:" + (ROI_cps[i, j] - Background_cps[j]) + " / " + Activity_Current[i]);

                    //MatrixElement[j, i] = (ROI_cps[i, j] - Background_cps[j]) / GlobalFunc.Math45(Activity_Current[i]);
                    if ((ROI_cps[i, j] - Background_cps[j]) < 0)
                    {
                        MatrixElement[j, i] = 0;
                        GlobalFunc.logManager.WriteLog("Cal: MatrixElement[" + i + ", " + j + "] is negative -> to zero");
                    }
                    else
                    {
                        MatrixElement[j, i] = (Convert.ToSingle(ROI_cps[i, j]) - Convert.ToSingle(Background_cps[j])) / Convert.ToSingle(Activity_Current[i]);
                    }
                    GlobalFunc.logManager.WriteLog("Cal: MatrixElement[" + j + ", " + i + "] " + MatrixElement[j, i]);
                }
            }

            // transform into list format
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    List_MatrixElement.Add(MatrixElement[i, j]);
                }
            }

            /*for (int i = 0; i < List_MatrixElement.Count; i++)
            {
                if (List_MatrixElement[i] < 0.0)
                {
                    List_MatrixElement[i] = 0.0;
                }
                List_MatrixElement[i] = GlobalFunc.Math45(List_MatrixElement[i]);
            }*/

            return List_MatrixElement;
        }

        public List<double> Measurement_Activity(ref List<int> detected, List<double> Background_cps, List<double> Nuclide_cps, List<float> MatrixElement, double LifeTime_ROI,
            List<double> RatioPeak, List<double> Activity, List<double> HalfTime, List<DateTime> Ref_Date, DateTime Report_Date, double SampleWeight, double qunatity, bool MDCReport_On)
        {
            int size = Background_cps.Count;
            List<double> Activity_Current = new List<double>();
            List<double> MDA = new List<double>();
            List<double> MDC = new List<double>();
            List<double> NetCount = new List<double>();
            List<double> CountEff = new List<double>();
            List<double> Invf = new List<double>();
            List<double> List_Activity = new List<double>();


            // MDA calculation
            // calculate net count
            for (int i = 0; i < size; i++)
            {
                double temp = Nuclide_cps[i] - Background_cps[i];
                GlobalFunc.logManager.WriteLog("Mea cps [" + i +"] " +  Nuclide_cps[i]);
                GlobalFunc.logManager.WriteLog("BG cps  [" + i + "] " + Background_cps[i]);
                if (temp < 0.0)
                {
                    NetCount.Add(0.0);
                }
                else
                {
                    NetCount.Add(temp);
                }
                GlobalFunc.logManager.WriteLog("Net Count [" + i + "] " + NetCount[i]);
                GlobalFunc.logManager.WriteLog(" ");
            }
            
            // calculate MDA
            Invf = InvMatrixNxN(MatrixElement);
            /*
            GlobalFunc.logManager.WriteLog(Invf[0] + ", " + Invf[1] + ", " + Invf[2] + ", " + Invf[3] + ", " + Invf[4] + ", ");
            GlobalFunc.logManager.WriteLog(Invf[5] + ", " + Invf[6] + ", " + Invf[7] + ", " + Invf[8] + ", " + Invf[9] + ", ");
            GlobalFunc.logManager.WriteLog(Invf[10] + ", " + Invf[11] + ", " + Invf[12] + ", " + Invf[13] + ", " + Invf[14] + ", ");
            GlobalFunc.logManager.WriteLog(Invf[15] + ", " + Invf[16] + ", " + Invf[17] + ", " + Invf[18] + ", " + Invf[19] + ", ");
            GlobalFunc.logManager.WriteLog(Invf[20] + ", " + Invf[21] + ", " + Invf[22] + ", " + Invf[23] + ", " + Invf[24] + ", ");
            */

            //MDA = MatrixAx_B(NetCount, Invf);
            MDA = MatrixAx_B(Invf, NetCount);

            GlobalFunc.logManager.WriteLog("SampleWeight: " + SampleWeight);
            GlobalFunc.logManager.WriteLog("Q: " + qunatity);

            for (int i = 0; i < size; i++)
            {
                GlobalFunc.logManager.WriteLog("MDA before div SW["+i+"] " + MDA[i]);
                if (MDA[i] > 0.0)
                {
                    MDA[i] /= (SampleWeight * qunatity);
                }
                GlobalFunc.logManager.WriteLog("MDA after div  SW[" + i + "] " + MDA[i]);
                GlobalFunc.logManager.WriteLog(" ");
            }

            // MDC calculation
            // calculate current activity of each nuclide
            for (int i = 0; i < size; i++)
            {
                Activity_Current.Add(DecayEq(Activity[i] * 1000, Ref_Date[i], Report_Date, HalfTime[i]));
            }

            // calculate counting eff.
            //for (int i = 0; i < size; i++)
            //{
            //CountEff.Add(NetCount[i] / Activity_Current[i]);
            //}
            if (size == 4)
            {
                CountEff.Add(MatrixElement[0]);
                CountEff.Add(MatrixElement[5]);
                CountEff.Add(MatrixElement[10]);
                CountEff.Add(MatrixElement[15]);
            }
            else if (size == 5)
            {
                CountEff.Add(MatrixElement[0]);
                CountEff.Add(MatrixElement[6]);
                CountEff.Add(MatrixElement[12]);
                CountEff.Add(MatrixElement[18]);
                CountEff.Add(MatrixElement[24]);
            }
            else if (size == 6)
            {
                CountEff.Add(MatrixElement[0]);
                CountEff.Add(MatrixElement[7]);
                CountEff.Add(MatrixElement[14]);
                CountEff.Add(MatrixElement[21]);
                CountEff.Add(MatrixElement[28]);
                CountEff.Add(MatrixElement[35]);
            }
            GlobalFunc.logManager.WriteLog(" ");
            GlobalFunc.logManager.WriteLog("Live Time: " + LifeTime_ROI);
            GlobalFunc.logManager.WriteLog("Report_Date: " + Report_Date);
            GlobalFunc.logManager.WriteLog("Measurement Time: " + BKManager.supposeSecond/60 + " mins");
            GlobalFunc.logManager.WriteLog(" ");
            // calculate MDC
            for (int i = 0; i < size; i++)
            {
                double b = Math.Sqrt(Background_cps[i] * LifeTime_ROI);
                double child = b * 4.66;
                double t = BKManager.supposeSecond / 60;
                double parent = t * CountEff[i];

                MDC.Add( child / parent);

                GlobalFunc.logManager.WriteLog("Net Count [" + i + "]: " + NetCount[i]);
                GlobalFunc.logManager.WriteLog("Activity [" + i + "]: " + Activity[i] * 1000);
                GlobalFunc.logManager.WriteLog("Ref_Date [" + i + "]: " + Ref_Date[i]);
                GlobalFunc.logManager.WriteLog("HalfTime [" + i + "]: " + HalfTime[i]);
                GlobalFunc.logManager.WriteLog("Activity_Current [" + i + "]: " + Activity_Current[i]);
                GlobalFunc.logManager.WriteLog("Background_cps[" + i + "]: " + Background_cps[i]);
                //GlobalFunc.logManager.WriteLog("CountEff[" + i + "]: " + CountEff[i] + " = (" + NetCount[i] + "/" + Activity_Current[i] + ")");
                GlobalFunc.logManager.WriteLog("CountEff[" + i + "]: " + CountEff[i]);
                GlobalFunc.logManager.WriteLog("RatioPeak[" + i + "]: " + RatioPeak[i]);

                string equlation = "((Math.Sqrt(" + Background_cps[i] + "*" + LifeTime_ROI + ") * 4.66)) / (" + t + "*" + CountEff[i]+ ")";
                GlobalFunc.logManager.WriteLog("MDC[" + i + "]: " + equlation );
                GlobalFunc.logManager.WriteLog(" ");
            }

            for (int i = 0; i < MDC.Count; i++)
            {
                if (MDC[i] < 0.0)                
                {
                    MDC[i] = 0;
                }
            }

            // report result
            if (MDCReport_On)
            {
                for (int i = 0; i < size; i++)
                {
                    GlobalFunc.logManager.WriteLog("MDC[" + i + "] = " + MDC[i] + ", Measued Value[" + i + "] = " + MDA[i]);
                    
                    if (MDA[i] >= MDC[i])
                    {
                        List_Activity.Add(MDA[i]);
                        detected.Add(0);
                    }
                    else
                    {
                        List_Activity.Add(MDA[i]);
                        detected.Add(1);
                    }
                }
            }
            else
            {
                for (int i = 0; i < size; i++)
                {
                    List_Activity.Add(MDA[i]);
                    detected.Add(0);
                }
            }

            for (int i = 0; i < size; i++)
            {
                //List_Activity[i] = Math.Round(List_Activity[i]);
            }
            return List_Activity;
        }

        public List<double> Uncertainty(List<double> Nuclide_cps)
        {
            List<double> Uncertainty = new List<double>();
            int size = Nuclide_cps.Count;

            for (int i = 0; i < size; i++)
            {
                Uncertainty.Add(100 / Math.Sqrt(Nuclide_cps[i]));
            }

            return Uncertainty;
        }

        public double RationSum(List<double> finalResult, List<double> AbsAlarmLevel, List<double> AbsAlarmPercentage, string mode)
        { 
            double ratioSum = 0.0;
            GlobalFunc.logManager.WriteLog("Ratio Sum Mode:" + mode);
            for (int i = 0; i < finalResult.Count; i++)
            {
                if (finalResult[i] < 0)
                {
                    finalResult[i] = 0;
                }
                if (mode == "DIL")
                {
                    GlobalFunc.logManager.WriteLog("Isotope " + i + ": " + finalResult[i] + "/" + "(" + 
                            AbsAlarmLevel[i] + ") * ( " + AbsAlarmPercentage[i] + "%)");
                    ratioSum += finalResult[i] / (AbsAlarmLevel[i] * (AbsAlarmPercentage[i] / 100.0));
                }
                else if (mode == "AL")
                {
                    GlobalFunc.logManager.WriteLog("Isotope " + i + ": " + finalResult[i]  + "/" + AbsAlarmLevel[i]);
                    ratioSum += finalResult[i] / AbsAlarmLevel[i];
                }
                //
            }
            if (double.IsNaN(ratioSum))
            {
                ratioSum = 0.00;
            }
            return ratioSum;
        }
        /*public double RatioSum(List<double> Background_cps, List<double> Nuclide_cps, List<float> MatrixElement, List<double> AbsAlarmLevel, List<double> AbsAlarmPercentage, string Mode)
        {
            double RatioSum = 0.0;
            double temp = 0.0;
            int size = Background_cps.Count;
            double[,] Matrix_f = new double[size, size];

            // transform into array form
            int k = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Matrix_f[i, j] = MatrixElement[k];
                    k++;
                }
            }

            // calculate ratio sum
            for (int i = 0; i < size; i++)
            {
                temp = Nuclide_cps[i] - Background_cps[i];
                if (temp < 0.0)
                {
                    temp = 0.0;
                }
                if (Mode == "DIL")
                {
                    RatioSum += (temp / (Matrix_f[i, i] * );  // alarm if ratio sum > 1
                }
                else if (Mode == "AL")
                {
                    RatioSum += (temp / (Matrix_f[i, i] * AbsAlarmLevel[i]));  // alarm if ratio sum > 1
                }
            }

            return RatioSum;
        }*/
    }
}
