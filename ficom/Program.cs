using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Reflection;
using System.Resources;
using System.Xml;
using System.Xml.Serialization;
using System.Text;
using System.Runtime.InteropServices;

namespace LCMS
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            
            GlobalFunc.logManager.CreateLogFile();
            GlobalFunc.logManager.CreateUserLogFile();
            GlobalFunc.assembly = Assembly.Load("LCMS");

            if (!Directory.Exists(@"C:\LCMS\defaultSetting"))
            {
                Directory.CreateDirectory(@"C:\LCMS\defaultSetting");
            }



            BKManager.SetLiveTime();
            GlobalFunc.SetMeasureSetting();
            GlobalFunc.tc = new TestConnection();

            Application.Run(new SplashScreen());  

        }

        public class Utf8StringWriter : TextWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }
}

