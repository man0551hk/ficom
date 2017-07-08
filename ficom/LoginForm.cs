using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace LCMS
{
    public partial class LoginForm : Form
    {
        SqlCeConnection con = new SqlCeConnection(Properties.Settings.Default.lcmsConnectionString);
        //Data Source=D:\Project\ficom\ficom\ficom\bin\Debug\lcmsDB.sdf;Password=p@ssw0rd
        public LoginForm()
        {
            InitializeComponent();
            passwordTxt.Select();
            passwordTxt.KeyUp += new KeyEventHandler(passwordTxt_KeyUp);
            noticeLabel.Text = "";
            this.FormClosing += LoginForm_FormClosing;
        }

        protected void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            if (GlobalFunc.dummyScreen == null)
            {
                GlobalFunc.dummyScreen = new DummyScreen();
            }
            GlobalFunc.dummyScreen.Show();
            noticeLabel.Text = "";
            this.Hide();
        }

        private void passwordTxt_KeyUp(object sender, KeyEventArgs e)
        
        {
            if (e.KeyCode == Keys.Enter)
            {
                CheckPassword();
            }
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            CheckPassword();
        }

        public void CheckPassword()
        {
            string _getPassword =passwordTxt.Text;
            bool successful = false;

            if (_getPassword == GlobalFunc.basicSetting.Administrator1 || _getPassword == GlobalFunc.basicSetting.Administrator2)
            {
                GlobalFunc.loginStatus = 3;
                GlobalFunc.userID = -1;
                passwordTxt.Text = "";
                noticeLabel.Text = "";
                successful = true;
            }
            else
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();

                SqlCeCommand cmd = new SqlCeCommand("select * from userPassword where CAST (password AS varbinary(100)) = CAST('" + _getPassword + "' as varbinary(100))", con);
                cmd.CommandType = CommandType.Text;
                //cmd.Parameters.AddWithValue("@password", _getPassword);
                SqlCeDataReader dr = cmd.ExecuteReader();
    
                if (dr.Read())
                {
                    GlobalFunc.loginStatus = Convert.ToInt32(dr["type"]);
                    GlobalFunc.userID = Convert.ToInt32(dr["id_key"]);
                    successful = true;
                    passwordTxt.Text = "";
                    noticeLabel.Text = "";
                }
                dr.Close();
                con.Close();
            }

            if (!successful)
            {
                noticeLabel.Text = "登入錯誤 Login Fail";
                GlobalFunc.logManager.WriteUserLog("Login Fail by password " + passwordTxt.Text);
            }
            else
            {
                GlobalFunc.logManager.WriteUserLog("Login Successful userID:" + GlobalFunc.userID);
                if (GlobalFunc.mainForm == null)
                {
                    GlobalFunc.mainForm = new MainForm();
                }
                if (GlobalFunc.loginStatus == 1)
                {
                    GlobalFunc.mainForm.UpdateLoginBtn_LoginAs("Supervisor");
                }
                else if (GlobalFunc.loginStatus == 2)
                {
                    GlobalFunc.mainForm.UpdateLoginBtn_LoginAs("Operator");
                }
                else if (GlobalFunc.loginStatus == 3)
                {
                    GlobalFunc.mainForm.UpdateLoginBtn_LoginAs("Administrator");
                }

                GlobalFunc.mainForm.Show();
                this.Hide();
            }
            
        }

        private void noticeLabel_Click(object sender, EventArgs e)
        {

        }
    }
}
