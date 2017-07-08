using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{
    public class Species
    {
        private int id_key = 0;
        private string code = "";
        private string description = "";
        private string scriptPath = "";
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
        public string ScriptPath
        {
            set { scriptPath = value; }
            get { return scriptPath; }
        }
        public DisplayText DisplayText
        {
            set { displayText = value; }
            get { return displayText; }
        }
        public Species() { }
        public Species(int id_key, string code, string description, string scriptPath, DisplayText displayText)
        {
            this.id_key = id_key;
            this.code = code;
            this.description = description;
            this.displayText = displayText;
        }
    }
}
