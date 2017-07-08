using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LCMS
{
    public class UserPassword
    {
        private int id_key = 0;
        private string username = "";
        private string password = "";
        private string accountType = "";
        private DisplayText displayText = new DisplayText();
        public int Id_key
        {
            set { id_key = value; }
            get { return id_key; }
        }
        public string Username
        {
            set { username = value; }
            get { return username; }
        }
        public string Password
        {
            set { password = value; }
            get { return password; }
        }
        public string AccountType
        {
            set { accountType = value; }
            get { return accountType; }
        }
        public DisplayText DisplayText
        {
            set { displayText = value; }
            get { return displayText; }
        }
        public UserPassword() { }
        public UserPassword(int id_key, string username, string password, string accountType, DisplayText displayText)
        {
            this.id_key = id_key;
            this.username = username;
            this.password = password;
            this.accountType = accountType;
            this.displayText = displayText;
        }
    }
}
