using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{

    public class IsoSeq
    {
        private int number = 0;
        private string name = "";
        private string activity = "";
        private DateTime refDateTime = new DateTime();         
        private string measureTime = "";
        private string dil = "";
        private string al = "";
        private string al_pc = "";
        private string topCPS = "";
        private string bottomCPS = "";
        public int Number
        {
            set { number = value; }
            get { return number; }
        }
        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        public string Activity
        {
            set { activity = value; }
            get { return activity; }
        }
        public DateTime RefDateTime
        {
            set { refDateTime = value; }
            get { return refDateTime; }
        }

        public string MeasureTime
        {
            set { measureTime = value; }
            get { return measureTime; }
        }
        public string DIL
        {
            set { dil = value; }
            get { return dil; }
        }
        public string AL
        {
            set { al = value; }
            get { return al; }
        }
        public string AL_PC
        {
            set { al_pc = value; }
            get { return al_pc; }
        }
        public string TopCPS
        {
            set { topCPS = value; }
            get { return topCPS; }
        }
        public string BottomCPS
        {
            set { bottomCPS = value; }
            get { return bottomCPS; }
        }
        public IsoSeq() { }
        public IsoSeq(int number, string name, string activity, DateTime refDateTime, string measureTime, string dil, string al, string al_pc, string topCPS, string bottomCPS)
        {
            this.number = number;
            this.name = name;
            this.activity = activity;
            this.refDateTime = refDateTime;       
            this.measureTime = measureTime;
            this.dil = dil;
            this.al = al;
            this.al_pc = al_pc;
            this.topCPS = topCPS;
            this.bottomCPS = bottomCPS;
        }
    }
}
