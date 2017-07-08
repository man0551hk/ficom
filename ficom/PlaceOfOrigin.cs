using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{
    public class PlaceOfOrigin
    {
        private int id_key = 0;
        private string engName = "";
        private string chiName = "";
        private DisplayText displayText = new DisplayText();
              
        public int Id_key
        {
            set { id_key = value; }
            get { return id_key; }
        }
        public string EngName
        {
            set { engName = value; }
            get { return engName; }
        }
        public string ChiName
        {
            set { chiName = value; }
            get { return chiName; }
        }
        public DisplayText DisplayText
        {
            set { displayText = value; }
            get { return displayText; }
        }
        public PlaceOfOrigin() { }
        public PlaceOfOrigin(int id_key, string engName, string chiName, DisplayText displayText)
        {
            this.id_key = id_key;
            this.engName = engName;
            this.chiName = chiName;
            this.displayText = displayText;
        }
    }
}
