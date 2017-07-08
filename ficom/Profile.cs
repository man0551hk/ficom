using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{
    public class Profile
    {
        private int id = 0;
        private string profileName = "";
        private string fileName = "";
        private string detector = "";
        private int qty = 0;
        private string liveTime = "";
        private string date = "";
        private string roiPath1 = "";
        private string roiPath2 = "";
        private List<string> bkTopCPS = new List<string>();
        private List<string> bkBottomCPS = new List<string>();
        private List<double> finalBkCPS = new List<double>();
        private List<IsoSeq> isoSeqList = new List<IsoSeq>();      
        private string location = "";
        private string alarm = "";
        private int noOfRegion = 0;
        private List<float> matrixTop = new List<float>();
        private List<float> matrixBottom = new List<float>();
        private List<float> matrixDual = new List<float>();
        private List<float> finalMatrix = new List<float>();
        private List<double> ratioPeak = new List<double>();
        private List<double> activity = new List<double>();
        private List<double> halfTime = new List<double>();
        private List<DateTime> ref_Date = new List<DateTime>();
        private List<double> alarmLevel = new List<double>();
        private List<double> alarmPCLevel = new List<double>();
       
        public int ID
        {
            set { id = value; }
            get { return id; }
        }
        public string ProfileName
        {
            set { profileName = value; }
            get { return profileName; }
        }
        public string FileName
        {
            set { fileName = value; }
            get { return fileName; }
        }
        public string Detector
        {
            set { detector = value; }
            get { return detector; }
        }
        public int Qty
        {
            set { qty = value; }
            get { return qty; }
        }
        public string LiveTime
        {
            set { liveTime = value; }
            get { return liveTime; }
        }
        public string Date
        {
            set { date = value; }
            get { return date; }
        }
        public string RoiPath1
        {
            set { roiPath1 = value; }
            get { return roiPath1; }
        }
        public string RoiPath2
        {
            set { roiPath2 = value; }
            get { return roiPath2; }
        }
        public List<string> BkTopCPS
        {
            set { bkTopCPS = value; }
            get { return bkTopCPS; }
        }
        public List<string> BKBottomCPS
        {
            set { bkBottomCPS = value; }
            get { return bkBottomCPS; }
        }
        public List<double> FinalBkCPS
        {
            set { finalBkCPS = value; }
            get { return finalBkCPS; }
        }
        public List<IsoSeq> IsoSeqList
        {
            set { isoSeqList = value; }
            get { return isoSeqList; }
        }        
        public string Location
        {
            set { location = value; }
            get { return location; }
        }
        public string Alarm
        {
            set { alarm = value; }
            get { return alarm; }
        }
        public int NoOfRegion
        {
            set { noOfRegion = value; }
            get { return noOfRegion; }
        }
        public List<float> MatrixTop
        {
            set { matrixTop = value; }
            get { return matrixTop; }
        }
        public List<float> MatrixBottom
        {
            set { matrixBottom = value; }
            get { return matrixBottom; }
        }
        public List<float> MatrixDual
        {
            set { matrixDual = value; }
            get { return matrixDual; }
        }
        public List<float> FinalMatrix
        {
            set { finalMatrix = value; }
            get { return finalMatrix; }
        }
        public List<double> RatioPeak
        {
            set { ratioPeak = value; }
            get { return ratioPeak; }
        }
        public List<double> Activity
        {
            set { activity = value; }
            get { return activity; }
        }
        public List<double> HalfTime
        {
            set { halfTime = value; }
            get { return halfTime; }
        }
        public List<DateTime> Ref_Date
        {
            set { ref_Date = value; }
            get { return ref_Date; }
        }
        public List<double> AlarmLevel
        {
            set { alarmLevel = value; }
            get { return alarmLevel; }
        }
        public List<double> AlarmPCLevel
        {
            set { alarmPCLevel = value; }
            get { return alarmPCLevel; }
        }
        
        public Profile(int id, string profileName, string fileName, string detector, int qty, string liveTime, 
            string date,string roiPath1, string roiPath2, List<string> bkTopCPS,
            List<string> bkBottomCPS, List<double> finalBkCPS, List<IsoSeq> isoSeqList, string location, string alarm, int noOfRegion,
            List<float> matrixTop, List<float> matrixBottom, List<float> matrixDual, List<float> finalMatrix, 
            List<double> ratioPeak, List<double> activity, List<double> halfTime, List<DateTime> ref_date, List<double> alarmLevel, List<double> alarmPCLevel
            )
        {
            this.id = id;
            this.profileName = profileName;
            this.fileName = fileName;
            this.detector = detector;
            this.qty = qty;
            this.liveTime = liveTime;
            this.date = date;
            this.roiPath1 = roiPath1;
            this.roiPath2 = roiPath2;
            this.bkTopCPS = bkTopCPS;
            this.bkBottomCPS = bkBottomCPS;
            this.finalBkCPS = finalBkCPS;
            this.isoSeqList = isoSeqList;            
            this.location = location;
            this.alarm = alarm;
            this.noOfRegion = noOfRegion;
            this.matrixTop = matrixTop;
            this.matrixBottom = matrixBottom;
            this.matrixDual = matrixDual;
            this.finalMatrix = finalMatrix;
            this.ratioPeak = ratioPeak;
            this.activity = activity;
            this.halfTime = halfTime;
            this.ref_Date = ref_date;
            this.alarmLevel = alarmLevel;
            this.alarmPCLevel = alarmPCLevel;
        }
        public Profile() { }
    }
}
