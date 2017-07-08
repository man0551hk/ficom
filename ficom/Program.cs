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

            /*ScriptSet dualScript = new ScriptSet();

            XmlSerializer serializer = new XmlSerializer(typeof(ScriptSet), "");
            TextWriter textWriter = new StreamWriter(@Directory.GetCurrentDirectory() + @"\xml\BottomScript.xml");
            using (TextWriter tw = new Utf8StringWriter())
            {
                serializer.Serialize(textWriter, dualScript);
            }
            textWriter.Close();*/
            
            GlobalFunc.logManager.CreateLogFile();
            GlobalFunc.logManager.CreateUserLogFile();
            GlobalFunc.assembly = Assembly.Load("LCMS");

            if (!Directory.Exists(@"C:\LCMS\defaultSetting"))
            {
                Directory.CreateDirectory(@"C:\LCMS\defaultSetting");
            }
            
            try
            {
                XmlSerializer deserializer1 = new XmlSerializer(typeof(BasicSetting));
                TextReader textReader = new StreamReader(@Directory.GetCurrentDirectory() + @"\xml\Basic.xml");
                GlobalFunc.basicSetting = (BasicSetting)deserializer1.Deserialize(textReader);
                textReader.Close();

                if (GlobalFunc.basicSetting.GetTemp.ToLower() == "on")
                {
                    GlobalFunc.getTemp = true; 
                }
                else if(GlobalFunc.basicSetting.GetTemp.ToLower() == "off")
                {
                    GlobalFunc.getTemp = false;
                }

                GlobalFunc.intIOAddress = Convert.ToInt32(GlobalFunc.basicSetting.IoAddress, 16); 

                XmlSerializer deserializer2 = new XmlSerializer(typeof(ScriptSet));
                TextReader textReader2 = new StreamReader(@Directory.GetCurrentDirectory() + @"\xml\DualScript.xml");
                GlobalFunc.dualScriptSet = (ScriptSet)deserializer2.Deserialize(textReader2);
                textReader2.Close();

                XmlSerializer deserializer3 = new XmlSerializer(typeof(ScriptSet));
                TextReader textReader3 = new StreamReader(@Directory.GetCurrentDirectory() + @"\xml\TopScript.xml");
                GlobalFunc.topScriptSet = (ScriptSet)deserializer3.Deserialize(textReader3);
                textReader3.Close();

                XmlSerializer deserializer4 = new XmlSerializer(typeof(ScriptSet));
                TextReader textReader4 = new StreamReader(@Directory.GetCurrentDirectory() + @"\xml\BottomScript.xml");
                GlobalFunc.bottomScriptSet = (ScriptSet)deserializer4.Deserialize(textReader4);
                textReader4.Close();

                GlobalFunc.LoadIsotopXML();

                #region check inpot32.dll is existed
                if (!File.Exists(@"C:\Windows\System32\inpout32.dll"))
                {
                    if (File.Exists(@Directory.GetCurrentDirectory() + @"\inpout32.dll"))
                    {
                        MessageBox.Show(@"Please copy inpout32.dll from C:\LCMS to C:\Windows\System32");
                    }
                    else
                    {
                        MessageBox.Show("Can't find inpout32.dll in program directory");
                    }
                }
                #endregion

                if (GlobalFunc.basicSetting.InsalledIO.ToLower() == "true")
                {
                    try
                    {
                        //reset to only led #1 on
                        PortAccess.Output(GlobalFunc.intIOAddress, 0);
                    }
                    catch
                    {
                        MessageBox.Show("IO Addresws not found");
                    }
                }               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            if (GlobalFunc.basicSetting.Lang == "Zn")
            {
                GlobalFunc.rm = new ResourceManager("LCMS.Lang.LangZn", GlobalFunc.assembly);
            }
            else
            {
                GlobalFunc.rm = new ResourceManager("LCMS.Lang.LangEn", GlobalFunc.assembly);
            }
            //string text = (1.3 * Math.Pow(10, 9) * 365).ToString();

            BKManager.SetLiveTime();
            GlobalFunc.SetMeasureSetting();
            GlobalFunc.tc = new TestConnection();

            Application.Run(new SplashScreen());  
            
            //Application.Run(GlobalFunc.tc);               
            //Application.Run(new SettingForm());
            //Application.Run(new OutputWord());
            //Application.Run(new MainForm()); 
            //Application.Run(new LoginForm());    

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

