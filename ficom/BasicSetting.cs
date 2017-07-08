using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{
    public class BasicSetting
    {
        private string lang = "";
        private string exePath = "";
        private string ioAddress = "";
        private string getTemp = "";
        private string administrator1 = "";
        private string administrator2 = "";
        private string presetDetector = "";
        private string installedIO = "";
        public string Lang
        {
            set { lang = value; }
            get { return lang; }
        }

        public string ExePath
        {
            set { exePath = value; }
            get { return exePath; }
        }

        public string IoAddress
        {
            set { ioAddress = value; }
            get { return ioAddress; }
        }
        public string GetTemp
        {
            set { getTemp = value; }
            get { return getTemp; }
        }
        public string Administrator1
        {
            set { administrator1 = value; }
            get { return administrator1; }
        }
        public string Administrator2
        {
            set { administrator2 =value; }
            get { return administrator2; }
        }

        public string PresetDetector
        {
            set { presetDetector = value; }
            get { return presetDetector; }
        }

        public string InsalledIO
        {
            set { installedIO = value; }
            get { return installedIO; }
        }

        public BasicSetting(){}
        public BasicSetting(string lang, string exePath, string ioAddress, string getTemp, string administrator1, string administrator2, string presetDetector, string installedIO)
        {
            this.lang = lang;
            this.exePath = exePath;
            this.ioAddress = ioAddress;
            this.getTemp = getTemp;
            this.administrator1 = administrator1;
            this.administrator2 = administrator2;
            this.presetDetector = presetDetector;
            this.installedIO = installedIO;
        }
    }
}
