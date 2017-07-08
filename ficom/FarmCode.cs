using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{
    public class FarmCode
    {
        private int id_key = 0;
        private string code = "";
        private string description = "";
        private DisplayText displayText = new DisplayText();
        public int Id_key
        {
            set { id_key = value; }
            get { return id_key; }
        }
        public string Code
        {
            set { code = value; }
            get { return code; }
        }
        public string Description
        {
            set { description = value; }
            get { return description; }
        }
        public DisplayText DisplayText
        {
            set { displayText = value; }
            get { return displayText; }
        }
        public FarmCode() { }
        public FarmCode(int id_key, string code, string description, DisplayText displayText)
        {
            this.id_key = id_key;
            this.code = code;
            this.description = description;
            this.displayText = displayText;
        }
    }
}
