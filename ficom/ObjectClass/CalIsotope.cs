using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{
    public class CalIsotope
    {
        public string roiName = "";
        public string activity = "";
        public DateTime refDate = new DateTime();      
        public string branch = "";
        public string measureTime = "";
        public string dil = "";
        public string al = "";
        public string al_pc = "";
        public int runCounter = 0;
        public List<double> topCpsList = new List<double>();
        public List<double> bottomCpsList = new List<double>();
        public CalIsotope() { }
    }
}
