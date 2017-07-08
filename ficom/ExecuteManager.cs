using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
namespace LCMS
{
    public class ExecuteManager
    {
        public string scriptFilePath = "";

        public ExecuteManager() { }
        public ExecuteManager(string scriptFilePath)
        {
            this.scriptFilePath = scriptFilePath;
        }

        public void RunScript()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = GlobalFunc.basicSetting.ExePath;
            startInfo.Arguments = "-P DetL " + scriptFilePath + " -B";
            Process.Start(startInfo);
        }
    }
}
