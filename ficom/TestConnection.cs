using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using UMCBILib;    

namespace LCMS
{
    public partial class TestConnection : Form
    {
        public TestConnection()
        {
            InitializeComponent();            
        }

        public void checkDetector1Connection()
        {
            bool retry = true;
            int retryCount = 0;
            while (retry)
            {
                try
                {
                    if (axUCONN21.IsOpen)
                    {
                        axUCONN21.Close();
                    }
                    axUDROP1.SelIndex = 1;
                    axUCONN21.Address = axUDROP1.SelAddress;
                    axUCONN21.Open();
                    axUCONN21.Comm("SHOW_VERSION");
                    axUCONN21.Comm("ENAB_HV");
                    if (axUCONN21.ID == 1)
                    {
                        GlobalFunc.DetectorID1 = axUCONN21.ID;
                        GlobalFunc.connectedToDetector1 = true;
                        retry = false;
                    }
                    else
                    {
                        retry = true;
                        retryCount++;
                        GlobalFunc.connectedToDetector1 = false;
                    }
                    axUCONN21.Close();
                }
                catch (Exception ex)
                {
                    retryCount++;
                    GlobalFunc.connectedToDetector1 = false;
                }
                if (retryCount == 10)
                {
                    GlobalFunc.connectedToDetector1 = false;
                    break;
                }
            }            
        }

        public void checkDetector2Connection()
        {
            bool retry = true;
            int retryCount = 0;
            axUDROP1.SelIndex = 2;
            while (retry)
            {
                try
                {
                    if (axUCONN21.IsOpen)
                    {
                        axUCONN21.Close();
                    }
                    
                    axUCONN21.Address = axUDROP1.SelAddress;
                    axUCONN21.Open();
                    axUCONN21.Comm("SHOW_VERSION");
                    axUCONN21.Comm("ENAB_HV");
                    if (axUCONN21.ID == 2)
                    {
                        GlobalFunc.DetectorID1 = axUCONN21.ID;
                        GlobalFunc.connectedToDetector2 = true;
                        retry = false;
                    }
                    else
                    {
                        retry = true;
                        retryCount++;
                        GlobalFunc.connectedToDetector2 = false;
                    }
                    axUCONN21.Close();
                }
                catch (Exception ex)
                {
                    retryCount++;
                    GlobalFunc.connectedToDetector2 = false;
                }
                if (retryCount == 5)
                {
                    try
                    {
                        axUDROP1.SelIndex = 1;
                        axUCONN21.Address = axUDROP1.SelAddress;
                    }
                    catch { }
                }
                if (retryCount == 10)
                {
                    GlobalFunc.connectedToDetector2 = false;
                    break;
                }
            }  
        }

        public void GetDetector1Temp()
        {
            if (GlobalFunc.getTemp == true)
            {
                bool retry = true;
                int retryCount = 0;
                while (retry)
                {
                    try
                    {
                        if (axUCONN21.IsOpen)
                        {
                            axUCONN21.Close();
                        }
                        axUDROP1.SelIndex = 1;
                        axUCONN21.Address = axUDROP1.SelAddress;
                        axUCONN21.Open();
                        if (axUCONN21.ID == 1)
                        {
                            string thisTemp = GlobalFunc.tc.axUCONN21.Comm("SHOW_DET_TEMP");
                            thisTemp = thisTemp.Replace("DET_TEMP ", "");
                            GlobalFunc.det1_temp = GlobalFunc.Math45Pt1(Convert.ToDouble(thisTemp));
                        }
                        else
                        {
                            retry = true;
                            retryCount++;
                        }
                        axUCONN21.Close();
                        retry = false;
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                    }
                    if (retryCount == 10)
                    {
                        break;
                    }
                }
            }
        }

        public void GetDetector2Temp()
        {
            if (GlobalFunc.getTemp == true)
            {
                bool retry = true;
                int retryCount = 0;
                axUDROP1.SelIndex = 2;
                while (retry)
                {
                    try
                    {
                        if (axUCONN21.IsOpen)
                        {
                            axUCONN21.Close();
                        }

                        axUCONN21.Address = axUDROP1.SelAddress;
                        axUCONN21.Open();
                        axUCONN21.Comm("SHOW_VERSION");
                        axUCONN21.Comm("ENAB_HV");
                        if (axUCONN21.ID == 2)
                        {
                            string thisTemp = GlobalFunc.tc.axUCONN21.Comm("SHOW_DET_TEMP");
                            thisTemp = thisTemp.Replace("DET_TEMP ", "");
                            GlobalFunc.det2_temp = GlobalFunc.Math45Pt1(Convert.ToDouble(thisTemp));
                            retry = false;
                        }
                        else
                        {
                            retry = true;
                            retryCount++;
                        }
                        axUCONN21.Close();
                    }
                    catch (Exception ex)
                    {
                        retryCount++;
                    }
                    if (retryCount == 5)
                    {
                        try
                        {
                            axUDROP1.SelIndex = 1;
                            axUCONN21.Address = axUDROP1.SelAddress;
                        }
                        catch { }
                    }
                    if (retryCount == 10)
                    {
                        break;
                    }
                }
            }
        }

        public void GetDetector1ICR()
        {
            bool retry = true;
            int retryCount = 0;
            while (retry)
            {
                try
                {
                    if (axUCONN21.IsOpen)
                    {
                        axUCONN21.Close();
                    }
                    axUDROP1.SelIndex = 1;
                    axUCONN21.Address = axUDROP1.SelAddress;
                    axUCONN21.Open();
                    if (axUCONN21.ID == 1)
                    {
                        string thisICR = GlobalFunc.tc.axUCONN21.Comm("SHOW_CRM");
                        thisICR = thisICR.Replace("CRM ", "");
                        GlobalFunc.det1_icr = Convert.ToDouble(thisICR);
                    }
                    else
                    {
                        retry = true;
                        retryCount++;
                    }
                    axUCONN21.Close();
                    retry = false;
                }
                catch (Exception ex)
                {
                    retryCount++;
                }
                if (retryCount == 10)
                {
                    break;
                }
            }            
        }

        public void GetDetector2ICR()
        {
            bool retry = true;
            int retryCount = 0;
            axUDROP1.SelIndex = 2;
            while (retry)
            {
                try
                {
                    if (axUCONN21.IsOpen)
                    {
                        axUCONN21.Close();
                    }

                    axUCONN21.Address = axUDROP1.SelAddress;
                    axUCONN21.Open();
                    axUCONN21.Comm("SHOW_VERSION");
                    axUCONN21.Comm("ENAB_HV");
                    if (axUCONN21.ID == 2)
                    {
                        string thisICR = GlobalFunc.tc.axUCONN21.Comm("SHOW_CRM");
                        thisICR = thisICR.Replace("CRM ", "");
                        GlobalFunc.det2_icr = Convert.ToDouble(thisICR);
                        retry = false;
                    }
                    else
                    {
                        retry = true;
                        retryCount++;
                    }
                    axUCONN21.Close();
                }
                catch (Exception ex)
                {
                    retryCount++;
                }
                if (retryCount == 5)
                {
                    axUDROP1.SelIndex = 1;
                    axUCONN21.Address = axUDROP1.SelAddress;
                }
                if (retryCount == 10)
                {
                    break;
                }
            }     
        }

        public void isDetector1Active()
        {
            bool retry = true;
            int retryCount = 0;
            while (retry)
            {
                try
                {
                    if (axUCONN21.IsOpen)
                    {
                        axUCONN21.Close();
                    }
                    axUDROP1.SelIndex = 1;
                    axUCONN21.Address = axUDROP1.SelAddress;
                    axUCONN21.Open();
                    if (axUCONN21.ID == 1)
                    {
                        string thisActive = GlobalFunc.tc.axUCONN21.Comm("SHOW_ACTIVE");
                        if (thisActive.Contains("87"))
                        {
                            GlobalFunc.det1_active = false;
                        }
                        else if (thisActive.Contains("1088"))
                        {
                            GlobalFunc.det1_active = true;
                        }
                    }
                    else
                    {
                        retry = true;
                        retryCount++;
                    }
                    axUCONN21.Close();
                    retry = false;
                }
                catch (Exception ex)
                {
                    retryCount++;
                }
                if (retryCount == 10)
                {
                    break;
                }
            }            
        }

        public void isDetector2Active()
        {
            bool retry = true;
            int retryCount = 0;
            axUDROP1.SelIndex = 2;
            while (retry)
            {
                try
                {
                    if (axUCONN21.IsOpen)
                    {
                        axUCONN21.Close();
                    }

                    axUCONN21.Address = axUDROP1.SelAddress;
                    axUCONN21.Open();
                    axUCONN21.Comm("SHOW_VERSION");
                    axUCONN21.Comm("ENAB_HV");
                    if (axUCONN21.ID == 2)
                    {
                        string thisActive = GlobalFunc.tc.axUCONN21.Comm("SHOW_ACTIVE");
                        if (thisActive.Contains("87"))
                        {
                            GlobalFunc.det2_active = false;
                        }
                        else if (thisActive.Contains("1088"))
                        {
                            GlobalFunc.det2_active = true;
                        }
                        retry = false;
                    }
                    else
                    {
                        retry = true;
                        retryCount++;
                    }
                    axUCONN21.Close();
                }
                catch (Exception ex)
                {
                    retryCount++;
                }
                if (retryCount == 5)
                {
                    axUDROP1.SelIndex = 1;
                    axUCONN21.Address = axUDROP1.SelAddress;
                }
                if (retryCount == 10)
                {
                    break;
                }
            }     
        }

        public void CleanUp()
        {
            // Disable the screen update timer
            //timer1.Enabled = false;

            // Allow picking a new instrument
            axUDROP1.Enabled = true;

            try
            {
                // If a connection to the instrument is open, ...
                if (axUCONN21.IsOpen)
                {
                    // Stop any current data collection
                    axUCONN21.Comm("STOP");

                    // Close the connection
                    axUCONN21.Close();
                }
            }
            catch
            {
                // Since we are shutting down the acquisition anyway, we'll ignore anything that goes wrong.
            }
        }

        public void UpdateDetector1Temp()
        {
            bool retry = true;
            int retryCount = 0;
            while (retry)
            {
                try
                {
                    if (axUCONN21.IsOpen)
                    {
                        axUCONN21.Close();
                    }
                    axUDROP1.SelIndex = 1;
                    axUCONN21.Address = axUDROP1.SelAddress;
                    axUCONN21.Open();
                    if (axUCONN21.ID == 1)
                    {
                        string thisTemp = GlobalFunc.tc.axUCONN21.Comm("SHOW_DET_TEMP");
                        thisTemp = thisTemp.Replace("DET_TEMP ", "");
                        GlobalFunc.det1_temp = GlobalFunc.Math45(Convert.ToDouble(thisTemp));
                    }
                    else
                    {
                        retry = true;
                        retryCount++;
                    }
                    axUCONN21.Close();
                    retry = false;
                }
                catch (Exception ex)
                {
                    retryCount++;
                }
                if (retryCount == 10)
                {
                    break;
                }
            }   
        }

        public void UpdateDetector2Temp()
        {
            bool retry = true;
            int retryCount = 0;
            axUDROP1.SelIndex = 2;
            while (retry)
            {
                try
                {
                    if (axUCONN21.IsOpen)
                    {
                        axUCONN21.Close();
                    }

                    axUCONN21.Address = axUDROP1.SelAddress;
                    axUCONN21.Open();
                    axUCONN21.Comm("SHOW_VERSION");
                    axUCONN21.Comm("ENAB_HV");
                    if (axUCONN21.ID == 2)
                    {
                        string thisTemp = GlobalFunc.tc.axUCONN21.Comm("SHOW_DET_TEMP");
                        thisTemp = thisTemp.Replace("DET_TEMP ", "");
                        GlobalFunc.det2_temp = GlobalFunc.Math45(Convert.ToDouble(thisTemp));
                        retry = false;
                    }
                    else
                    {
                        retry = true;
                        retryCount++;
                    }
                    axUCONN21.Close();
                }
                catch (Exception ex)
                {
                    retryCount++;
                }
                if (retryCount == 5)
                {
                    axUDROP1.SelIndex = 1;
                    axUCONN21.Address = axUDROP1.SelAddress;
                }
                if (retryCount == 10)
                {
                    break;
                }
            }             
        }

        public void DisableDetector1()
        {
            bool retry = true;
            int retryCount = 0;
            while (retry)
            {
                try
                {
                    if (axUCONN21.IsOpen)
                    {
                        axUCONN21.Close();
                    }
                    axUDROP1.SelIndex = 1;
                    axUCONN21.Address = axUDROP1.SelAddress;
                    axUCONN21.Open();
                    axUCONN21.Comm("SHOW_VERSION");
                    axUCONN21.Comm("DISABLE_HV");
                    if (axUCONN21.ID == 1)
                    {
                        GlobalFunc.DetectorID1 = axUCONN21.ID;
                        GlobalFunc.connectedToDetector1 = true;
                        retry = false;
                    }
                    else
                    {
                        retry = true;
                        retryCount++;
                        GlobalFunc.connectedToDetector1 = false;
                    }
                    axUCONN21.Close();
                }
                catch (Exception ex)
                {
                    retryCount++;
                    GlobalFunc.connectedToDetector1 = false;
                }
                if (retryCount == 10)
                {
                    GlobalFunc.connectedToDetector1 = false;
                    break;
                }
            }                    
        }

        public void DisableDetector2()
        {
            bool retry = true;
            int retryCount = 0;
            axUDROP1.SelIndex = 2;
            while (retry)
            {
                try
                {
                    if (axUCONN21.IsOpen)
                    {
                        axUCONN21.Close();
                    }

                    axUCONN21.Address = axUDROP1.SelAddress;
                    axUCONN21.Open();
                    axUCONN21.Comm("SHOW_VERSION");
                    axUCONN21.Comm("DISABLE_HV");
                    if (axUCONN21.ID == 2)
                    {
                        GlobalFunc.DetectorID1 = axUCONN21.ID;
                        GlobalFunc.connectedToDetector2 = true;
                        retry = false;
                    }
                    else
                    {
                        retry = true;
                        retryCount++;
                        GlobalFunc.connectedToDetector2 = false;
                    }
                    axUCONN21.Close();
                }
                catch (Exception ex)
                {
                    retryCount++;
                    GlobalFunc.connectedToDetector2 = false;
                }
                if (retryCount == 5)
                {
                    try
                    {
                        axUDROP1.SelIndex = 1;
                        axUCONN21.Address = axUDROP1.SelAddress;
                    }
                    catch
                    {
                        GlobalFunc.connectedToDetector2 = false;
                    }
                }
                if (retryCount == 10)
                {
                    GlobalFunc.connectedToDetector2 = false;
                    break;
                }
            }          
        }

        private void axUDROP1_NewSelection(object sender, EventArgs e)
        {
            int a = axUDROP1.SelIndex;
            axUCONN21.Address = axUDROP1.SelAddress;

            if (axUCONN21.IsOpen)
            {
                axUCONN21.Close();
            }
            axUCONN21.Open();
            MessageBox.Show(axUCONN21.Name.ToString());

            MessageBox.Show(axUCONN21.ID.ToString());
            MessageBox.Show(axUCONN21.Comm("SHOW_DET_TEMP"));
            axUCONN21.Close();
        }
    }
}
