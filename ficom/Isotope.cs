using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{
    public class Isotope
    {
        private int id_key = 0;
        private string code = "";
        private string description = "";
        private string halfLife = "";
        private string bop = "";
        private double mpe = 0.0;
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
        public string HalfLife
        {
            set { halfLife = value; }
            get { return halfLife; }
        }
        public string Bop
        {
            set { bop = value; }
            get { return bop; }
        }
        public double Mpe
        {
            set { mpe = value; }
            get { return mpe; }
        }
        public Isotope() { }
        public Isotope(int id_key, string code, string description, string halfLife, string bop, double mpe)
        {
            this.id_key = id_key;
            this.code = code;
            this.description = description;
            this.halfLife = halfLife;
            this.bop = bop;
            this.mpe = mpe;
        }
    }
}
