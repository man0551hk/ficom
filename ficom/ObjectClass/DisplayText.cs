using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{
    public class DisplayText
    {
        public string code = "";
        public string description = "";
        public DisplayText() { }
        public override string ToString()
        {
            if (code != "" && description == "")
            { 
                return string.Format("{0}", code);
            }
            else if (code == "" && description != "")
            {
                return string.Format("{0}", description);
            }
            else
            {
                return string.Format("{0}, {1}", code, description);
            }
        }
    }
}
