using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{
    public class ScriptSet
    {
        private string warmUpBegin = "";
        private string warmUpEnd = "";
        private string warmUpRoi = "";  
        private string warmUpData = "";
        private string backgroundBegin = "";
        private string backgroundEnd = "";
        private string backgroundData = "";
        private string roi4 = "";
        private string roi5 = "";
        private string roi6 = "";
        private string calibrationBegin = "";
        private string calibrationEnd = "";
        private string calibrationUpdate = "";
        private string calibrationData = "";
        private string measureBegin = "";
        private string measureEnd = "";
        private string measureData = "";
        public string WarmUpBegin
        {
            set { warmUpBegin = value; }
            get { return warmUpBegin; }
        }
        public string WarmUpEnd
        {
            set { warmUpEnd = value; }
            get { return warmUpEnd; }
        }
        public string WarmUpRoi
        {
            set { warmUpRoi = value; }
            get { return warmUpRoi; }
        }
        public string WarmUpData
        {
            set { warmUpData = value; }
            get { return warmUpData; }
        }
        public string BackgroundBegin
        {
            set { backgroundBegin = value; }
            get { return backgroundBegin; }
        }
        public string BackgroundEnd
        {
            set { backgroundEnd = value; }
            get { return backgroundEnd; }
        } 
        public string BackgrounData
        {
            set { backgroundData = value; }
            get { return backgroundData; }
        }
        public string Roi4
        {
            set { roi4 = value; }
            get { return roi4; }
        }
        public string Roi5
        {
            set { roi5 = value; }
            get { return roi5; }
        }
        public string Roi6
        {
            set { roi6 = value; }
            get { return roi6; }
        }
        public string CalibrationBegin
        {
            set { calibrationBegin = value; }
            get { return calibrationBegin; }
        }
        public string CalibrationEnd
        {
            set { calibrationEnd = value; }
            get { return calibrationEnd; }
        }
        public string CalibrationUpdate
        {
            set { calibrationUpdate = value; }
            get { return calibrationUpdate; }
        }
        public string CalibrationData
        {
            set { calibrationData = value; }
            get { return calibrationData; }
        }
        public string MeasureBegin
        {
            set { measureBegin = value; }
            get { return measureBegin; }
        }
        public string MeasureEnd
        {
            set { measureEnd = value; }
            get { return measureEnd; }
        }
        public string MeasureData
        {
            set { measureData = value; }
            get { return measureData; }
        }
        public ScriptSet()
        {            
        }

        public ScriptSet(string warmUpBegin, string warmUpEnd, string warmUpRoi, string warmUpData, string backgroundBegin, 
                        string backgroundEnd, string backgroundData, string roi4, string roi5, string roi6, string calibrationBegin, 
                        string calibrationEnd, string calibrationUpdate, string calibrationData, string measureBegin, 
                        string measureEnd, string measureData)
        {
            this.warmUpBegin = warmUpBegin;
            this.warmUpEnd = warmUpEnd;
            this.warmUpRoi = warmUpRoi;        
            this.warmUpData = warmUpData;
            this.backgroundBegin = backgroundBegin;
            this.backgroundEnd = backgroundEnd;          
            this.backgroundData = backgroundData;
            this.roi4 = roi4;
            this.roi5 = roi5;
            this.roi6 = roi6;
            this.calibrationBegin = calibrationBegin;
            this.calibrationEnd = calibrationEnd;
            this.calibrationUpdate = calibrationUpdate;
            this.calibrationData = calibrationData;
            this.measureBegin = measureBegin;
            this.measureEnd = measureEnd;         
            this.measureData = measureData;
        }        
    }
}
