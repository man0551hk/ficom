using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ficom
{
    public class Script
    {
        private string det1_BeginWarmUp = "";
        private string det1_EndWarmUp = "";
        private string det1_WarmUpChn = "";
        private string det1_WarmUpData = "";
        private string det2_BeginWarmUp = "";
        private string det2_EndWarmUp = "";
        private string det2_WarmUpChn = "";
        private string det2_WarmUpData = "";
        private string det_BeginWarmUp = "";
        private string det_EndWarmUp = "";
        private string det_WarmUpData = "";
        private string det1_BeginBgd = "";
        private string det1_EndBgd = "";
        private string det1_BgdChn = "";
        private string det1_BgdData = "";
        private string det2_BeginBgd = "";
        private string det2_EndBgd = "";
        private string det2_BgdUpChn = "";
        private string det2_BgdData = "";
        private string det_BeginBgd = "";
        private string det_EndBgd = "";
        private string det_BgdData = "";
        private string det1_ROI = "";
        private string det2_ROI = "";
        private string det1_Roi4 = "";
        private string det2_Roi4 = "";
        private string det1_Roi5 = "";
        private string det2_Roi5 = "";
        private string det1_Roi6 = "";
        private string det2_Roi6 = "";
        private string det1_Cal4 = "";
        private string det2_Cal4 = "";
        private string det_Cal4 = "";
        private string det1_Cal5 = "";
        private string det2_Cal5 = "";
        private string det_Cal5 = "";
        private string det1_Cal6 = "";
        private string det2_Cal6 = "";
        private string det_Cal6 = "";
        private string det1_MesChn = "";
        private string det2_MesChn = "";
        private string det1_MesData = "";
        private string det2_MesData = "";
        private string det_MesData = "";
        private string det1_Begin = "";
        private string det1_End = "";
        private string det2_Begin = "";
        private string det2_End = "";
        private string det_Begin = "";
        private string det_End = "";

        public string Det1_BeginWarmUp
        {
	        set { det1_BeginWarmUp = value; }
	        get { return det1_BeginWarmUp; }
        }

        public string Det1_EndWarmUp
        {
            set { det1_EndWarmUp = value; }
            get { return det1_EndWarmUp; }
        }

        public string Det1_WarmUpChn
        {
            set { det1_WarmUpChn = value; }
            get { return det1_WarmUpChn; }
        }

        public string Det1_WarmUpData
        {
            set { det1_WarmUpData = value; }
            get { return det1_WarmUpData; }
        }

        public string Det2_BeginWarmUp
        {
            set { det2_BeginWarmUp = value; }
            get { return det2_BeginWarmUp; }
        }

        public string Det2_EndWarmUp
        {
            set { det2_EndWarmUp = value; }
            get { return det2_EndWarmUp; }
        }

        public string Det2_WarmUpChn
        {
            set { det2_WarmUpChn = value; }
            get { return det2_WarmUpChn; }
        }

        public string Det2_WarmUpData
        {
            set { det2_WarmUpData = value; }
            get { return det2_WarmUpData; }
        }

        public string Det_BeginWarmUp
        {
            set { det_BeginWarmUp = value; }
            get { return det_BeginWarmUp; }
        }

        public string Det_EndWarmUp
        {
            set { det_EndWarmUp = value; }
            get { return det_EndWarmUp; }
        }

        public string Det_WarmUpData
        {
            set { det_WarmUpData = value; }
            get { return det_WarmUpData; }
        }

        public string Det1_BeginBgd
        {
            set { det1_BeginBgd = value; }
            get { return det1_BeginBgd; }
        }

        public string Det1_EndBgd
        {
            set { det1_EndBgd = value; }
            get { return det1_EndBgd; }
        }

        public string Det1_BgdChn
        {
            set { det1_BgdChn = value; }
            get { return det1_BgdChn; }
        }

        public string Det1_BgdData
        {
            set { det1_BgdData = value; }
            get { return det1_BgdData; }
        }

        public string Det2_BeginBgd
        {
            set { det2_BeginBgd = value; }
            get { return det2_BeginBgd; }
        }

        public string Det2_EndBgd
        {
            set { det2_EndBgd = value; }
            get { return det2_EndBgd; }
        }

        public string Det2_BgdUpChn
        {
            set { det2_BgdUpChn = value; }
            get { return det2_BgdUpChn; }
        }

        public string Det2_BgdData
        {
            set { det2_BgdData = value; }
            get { return det2_BgdData; }
        }

        public string Det_BeginBgd
        {
            set { det_BeginBgd = value; }
            get { return det_BeginBgd; }
        }

        public string Det_EndBgd
        {
            set { det_EndBgd = value; }
            get { return det_EndBgd; }
        }

        public string Det_BgdData
        {
            set { det_BgdData = value; }
            get { return det_BgdData; }
        }

        public string Det1_ROI
        {
            set { det1_ROI = value; }
            get { return det1_ROI; }
        }

        public string Det2_ROI
        {
            set { det2_ROI = value; }
            get { return det2_ROI; }
        }

        public string Det1_Roi4
        {
            set { det1_Roi4 = value; }
            get { return det1_Roi4; }
        }

        public string Det2_Roi4
        {
            set { det2_Roi4 = value; }
            get { return det2_Roi4; }
        }

        public string Det1_Roi5
        {
            set { det1_Roi5 = value; }
            get { return det1_Roi5; }
        }

        public string Det2_Roi5
        {
            set { det2_Roi5 = value; }
            get { return det2_Roi5; }
        }

        public string Det1_Roi6
        {
            set { det1_Roi6 = value; }
            get { return det1_Roi6; }
        }

        public string Det2_Roi6
        {
            set { det2_Roi6 = value; }
            get { return det2_Roi6; }
        }

        public string Det1_Cal4
        {
            set { det1_Cal4 = value; }
            get { return det1_Cal4; }
        }

        public string Det2_Cal4
        {
            set { det2_Cal4 = value; }
            get { return det2_Cal4; }
        }

        public string Det_Cal4
        {
            set { det_Cal4 = value; }
            get { return det_Cal4; }
        }

        public string Det1_Cal5
        {
            set { det1_Cal5 = value; }
            get { return det1_Cal5; }
        }

        public string Det2_Cal5
        {
            set { det2_Cal5 = value; }
            get { return det2_Cal5; }
        }

        public string Det_Cal5
        {
            set { det_Cal5 = value; }
            get { return det_Cal5; }
        }

        public string Det1_Cal6
        {
            set { det1_Cal6 = value; }
            get { return det1_Cal6; }
        }

        public string Det2_Cal6
        {
            set { det2_Cal6 = value; }
            get { return det2_Cal6; }
        }

        public string Det_Cal6
        {
            set { det_Cal6 = value; }
            get { return det_Cal6; }
        }

        public string Det1_MesChn
        {
            set { det1_MesChn = value; }
            get { return det1_MesChn; }
        }

        public string Det2_MesChn
        {
            set { det2_MesChn = value; }
            get { return det2_MesChn; }
        }

        public string Det1_MesData
        {
            set { det1_MesData = value; }
            get { return det1_MesData; }
        }

        public string Det2_MesData
        {
            set { det2_MesData = value; }
            get { return det2_MesData; }
        }

        public string Det_MesData
        {
            set { det_MesData = value; }
            get { return det_MesData; }
        }

        public string Det1_Begin
        {
            set { det1_Begin = value; }
            get { return det1_Begin; }
        }

        public string Det1_End
        {
            set { det1_End = value; }
            get { return det1_End; }
        }

        public string Det2_Begin
        {
            set { det2_Begin = value; }
            get { return det2_Begin; }
        }

        public string Det2_End
        {
            set { det2_End = value; }
            get { return det2_End; }
        }

        public string Det_Begin
        {
            set { det_Begin = value; }
            get { return det_Begin; }
        }

        public string Det_End
        {
            set { det_End = value; }
            get { return det_End; }
        }

        public Script (){}

        public Script(string det1_BeginWarmUp,
        string det1_EndWarmUp,
        string det1_WarmUpChn,
        string det1_WarmUpData,
        string det2_BeginWarmUp,
        string det2_EndWarmUp,
        string det2_WarmUpChn,
        string det2_WarmUpdata,
            string det_BeginWarmUp,
            string det_EndWarmUp,
        string det_WarmUpData,
        string det1_BeginBgd,
        string det1_EndBgd,
        string det1_BgdChn,
        string det1_BgdData,
        string det2_BeginBgd,
        string det2_EndBgd,
        string det2_BgdUpChn,
        string det2_BgdData,
            string det_BeginBgd,
            string det_EndBgd,
        string det_Bgddata,
        string det1_ROI,
            string det2_ROI,
        string det1_Roi4,
        string det2_Roi4,
        string det1_Roi5,
        string det2_Roi5,
        string det1_Roi6,
        string det2_Roi6,
        string det1_Cal4,
        string det2_Cal4,
        string det_Cal4,
        string det1_Cal5,
        string det2_Cal5,
        string det_Cal5,
        string det1_Cal6,
        string det2_Cal6,
        string det_Cal6,
        string det1_MesChn,
        string det2_MesChn,
        string det1_MesData,
        string det2_MesData,
        string det_MesData,
        string det1_Begin,
        string det1_End,
        string det2_Begin,
        string det2_End,
        string det_Begin,
        string det_End) 
        {
            this.det1_BeginWarmUp = det1_BeginWarmUp;
            this.det1_EndWarmUp = det1_EndWarmUp;
            this.det1_WarmUpChn = det1_WarmUpChn;
            this.det1_WarmUpData = det1_WarmUpData;
            this.det2_BeginWarmUp = det2_BeginWarmUp;
            this.det2_EndWarmUp = det2_EndWarmUp;
            this.det2_WarmUpChn = det2_WarmUpChn;
            this.det2_WarmUpData = det2_WarmUpData;
            this.det_BeginWarmUp = det_BeginWarmUp;
            this.det_EndWarmUp = det_EndWarmUp;
            this.det_WarmUpData = det_WarmUpData;
            this.det1_BeginBgd = det1_BeginBgd;
            this.det1_EndBgd = det1_EndBgd;
            this.det1_BgdChn = det1_BgdChn;
            this.det1_BgdData = det1_BgdData;
            this.det2_BeginBgd = det2_BeginBgd;
            this.det2_EndBgd = det2_EndBgd;
            this.det2_BgdUpChn = det2_BgdUpChn;
            this.det2_BgdData = det2_BgdData;
            this.det_BeginBgd = det_BeginBgd;
            this.det_EndBgd = det_EndBgd;
            this.det_BgdData = det_BgdData;
            this.det1_ROI = det1_ROI;
            this.det2_ROI = det2_ROI;
            this.det1_Roi4 = det1_Roi4;
            this.det2_Roi4 = det2_Roi4;
            this.det1_Roi5 = det1_Roi5;
            this.det2_Roi5 = det2_Roi5;
            this.det1_Roi6 = det1_Roi6;
            this.det2_Roi6 = det2_Roi6;
            this.det1_Cal4 = det1_Cal4;
            this.det2_Cal4 = det2_Cal4;
            this.det_Cal4 = det_Cal4;
            this.det1_Cal5 = det1_Cal5;
            this.det2_Cal5 = det2_Cal5;
            this.det_Cal5 = det_Cal5;
            this.det1_Cal6 = det1_Cal6;
            this.det2_Cal6 = det2_Cal6;
            this.det_Cal6 = det_Cal6;
            this.det1_MesChn = det1_MesChn;
            this.det2_MesChn = det2_MesChn;
            this.det1_MesData = det1_MesData;
            this.det2_MesData = det2_MesData;
            this.det_MesData = det_MesData;
            this.det1_Begin = det1_Begin;
            this.det1_End = det1_End;
            this.det2_Begin = det2_Begin;
            this.det2_End = det2_End;
            this.det_Begin = det_Begin;
            this.det_End = det_End;     
        }
    }
}
