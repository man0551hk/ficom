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
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using System.Diagnostics;

namespace LCMS
{
    public partial class SettingForm : Form
    {
        SqlCeConnection con = new SqlCeConnection(Properties.Settings.Default.lcmsConnectionString);
        public string runBk = "no";
  
        public SettingForm()
        {
            InitializeComponent();
            GlobalFunc.loadProfile = new Profile();
            this.BringToFront();

            parameterListBox.DropDownStyle = ComboBoxStyle.DropDownList;
            editAccountType.DropDownStyle = ComboBoxStyle.DropDownList;
            addAccountType.DropDownStyle = ComboBoxStyle.DropDownList;
            profileComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            isotopeNumComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            selBkLocation.DropDownStyle = ComboBoxStyle.DropDownList;
            isoHalfLifeType.DropDownStyle = ComboBoxStyle.DropDownList;
            detectorComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            alarmComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            reportTypeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            reportSampleComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            reportIsotopeComboBox.DropDownStyle = ComboBoxStyle.DropDownList;

            detectorComboBox.SelectedIndex = -1;
            alarmComboBox.SelectedIndex = -1;
            locationListBox.SelectedIndexChanged += locationListBox_SelectedIndexChanged;
            farmCodeListBox.SelectedIndexChanged += farmCodeListBox_SelectedIndexChanged;
            speciesListBox.SelectedIndexChanged += speciesListBox_SelectedIndexChanged;
            destinationListBox.SelectedIndexChanged += destinationListBox_SelectedIndexChanged;

            reportPanel.AutoScroll = true;

            #region user accout is not admin
            if (GlobalFunc.loginStatus != 3)
            {
                smTab.TabPages.Remove(tabPage5);
            }
            
            #endregion
            
            if (GlobalFunc.tc != null)
            {
                GlobalFunc.tc.checkDetector1Connection();
                GlobalFunc.tc.checkDetector2Connection();
            }

            if (!GlobalFunc.connectedToDetector1)
            {
                textingLabel.Text = "Alarm";
                textingLabel.BackColor = Color.FromArgb(255, 0, 0);
            }
            if (!GlobalFunc.connectedToDetector2)
            {
                textingLabel.Text = "Alarm";
                textingLabel.BackColor = Color.FromArgb(255, 0, 0);
            }

            clockLabel.Text = "";
            GetLocation();
            LoadData();
            Reset();
            LoadAccount();
            LoadProfile();        
            LoadTime();
            calBkBtn.Enabled = true;
            isotopeList.DataSource = GlobalFunc.isotopeList1;
            isotopeList.DisplayMember = "Code";
            isotopeList.ValueMember = "Id_key";
            isotopeList.ClearSelected();
            isoAdd.Enabled = true;
            isoUpdate.Enabled = false;
            isoDelete.Enabled = false;
            isoCancel.Enabled = false;
            isoCode.Text = "";
            isoDescription.Text = "";
            isoHalfLife.Text = "";
            isoHalfLifeType.SelectedIndex = 0;
            isoBop.Text = "";
            mpe.Text = "";
            abortBtn.Enabled = false;
            searchReportBtn.Text = GlobalFunc.rm.GetString("searchBtn");

            isoHalfLife.KeyPress += acTb_KeyPress;
            isoBop.KeyPress += acTb_KeyPress;
            nor.KeyPress += acTb_KeyPress;
            mpe.KeyPress += acTb_KeyPress;
            sampleTime.KeyPress += acTb_KeyPress;
            bkTime.KeyPress += acTb_KeyPress;
            alarmTime.KeyPress += acTb_KeyPress;
            topICRTxt.KeyPress += acTb_KeyPress;
            bottomICRTxt.KeyPress += acTb_KeyPress;
            warmUpTimeTxt.KeyPress += acTb_KeyPress;
            bufferTimeTxt.KeyPress += bufferTime_KeyPress;
            profileName.KeyPress +=profileName_KeyPress;
        }

        protected void SettingForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
        
        #region parameter
        public void LoadData()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            List<Location> locationList = new List<Location>();
            SqlCeCommand locationCmd = new SqlCeCommand("select * from location with (nolock) order by id_key", con);
            SqlCeDataReader locationDr = locationCmd.ExecuteReader();
            while (locationDr.Read())
            {
                Location location = new Location();
                location.Id_key = Convert.ToInt32(locationDr["id_key"]);
                location.Code = locationDr["code"].ToString();
                location.Description = locationDr["description"].ToString();
              
                DisplayText dt = new DisplayText();
                dt.code = location.Code;
                dt.description = location.Description;
                location.DisplayText = dt;
                locationList.Add(location);
            }
            locationDr.Close();

            locationListBox.DataSource = locationList;
            locationListBox.DisplayMember = "DisplayText";
            locationListBox.ValueMember = "Id_Key";
            locationListBox.ClearSelected();

            List<FarmCode> farmCodeList = new List<FarmCode>();
            SqlCeCommand farmCmd = new SqlCeCommand("select * from farmCode with (nolock) order by id_key", con);
            SqlCeDataReader farmDr = farmCmd.ExecuteReader();
            while (farmDr.Read())
            {
                FarmCode farmCode = new FarmCode();
                farmCode.Id_key = Convert.ToInt32(farmDr["id_key"]);
                farmCode.Code = farmDr["code"].ToString();
                farmCode.Description = farmDr["description"].ToString();

                DisplayText dt = new DisplayText();
                dt.code = farmCode.Code;
                dt.description = farmCode.Description;
                farmCode.DisplayText = dt;
                farmCodeList.Add(farmCode);
            }
            farmDr.Close();

            farmCodeListBox.DataSource = farmCodeList;
            farmCodeListBox.DisplayMember = "DisplayText";
            farmCodeListBox.ValueMember = "Id_Key";
            farmCodeListBox.ClearSelected();

            List<Species> speciesList = new List<Species>();
            SqlCeCommand speciesCmd = new SqlCeCommand("select * from species with (nolock) order by id_key", con);
            SqlCeDataReader speciesDr = speciesCmd.ExecuteReader();
            while (speciesDr.Read())
            {
                Species species = new Species();
                species.Id_key = Convert.ToInt32(speciesDr["id_key"]);
                species.Code = speciesDr["code"].ToString();
                species.Description = speciesDr["description"].ToString();
                species.ScriptPath = speciesDr["scriptPath"].ToString();

                DisplayText dt = new DisplayText();
                dt.code = species.Code;
                dt.description = species.Description;
                species.DisplayText = dt;
                speciesList.Add(species);
            }
            speciesDr.Close();

            reportSampleComboBox.DataSource = speciesList;
            reportSampleComboBox.DisplayMember = "Description";
            reportSampleComboBox.ValueMember = "Id_Key";
            reportSampleComboBox.Width = DropDownWidth(reportSampleComboBox);

            speciesListBox.DataSource = speciesList;
            speciesListBox.DisplayMember = "DisplayText";
            speciesListBox.ValueMember = "Id_Key";
            speciesListBox.ClearSelected();

            List<Destination> destinationList = new List<Destination>();
            SqlCeCommand destinationCmd = new SqlCeCommand("select * from destination with (nolock) order by id_key", con);
            SqlCeDataReader destinationDr = destinationCmd.ExecuteReader();
            while (destinationDr.Read())
            {
                Destination destination = new Destination();
                destination.Id_key = Convert.ToInt32(destinationDr["id_key"]);
                destination.Code = destinationDr["code"].ToString();
                destination.Description = destinationDr["description"].ToString();

                DisplayText dt = new DisplayText();
                dt.code = destination.Code;
                dt.description = destination.Description;
                destination.DisplayText = dt;
                destinationList.Add(destination);
            }
            destinationDr.Close();

            destinationListBox.DataSource = destinationList;
            destinationListBox.DisplayMember = "DisplayText";
            destinationListBox.ValueMember = "Id_Key";
            destinationListBox.ClearSelected();

            List<PlaceOfOrigin> pooList = new List<PlaceOfOrigin>();
            SqlCeCommand ppoCmd = new SqlCeCommand("select * from origin with (nolock) order by Id", con);
            ppoCmd.CommandType = CommandType.Text;
            SqlCeDataReader pooDr = ppoCmd.ExecuteReader();
            while(pooDr.Read())
            {
                PlaceOfOrigin poo = new PlaceOfOrigin();
                poo.Id_key = Convert.ToInt32(pooDr["id"]);
                poo.EngName = pooDr["engname"].ToString();
                poo.ChiName = pooDr["chiname"].ToString();
                DisplayText dt = new DisplayText();
                dt.code = poo.EngName;
                dt.description = poo.ChiName;
                poo.DisplayText = dt;
                pooList.Add(poo);
            }
            con.Close();

            originListBox.DataSource = pooList;
            originListBox.DisplayMember = "DisplayText";
            originListBox.ValueMember = "Id_Key";
            originListBox.ClearSelected();

            #region calibration top menu style
            detectorComboBox.DropDownWidth = DropDownWidth(detectorComboBox);
            label39.Location = new Point(selBkLocation.Location.X + selBkLocation.Width, selBkLocation.Location.Y);
            alarmComboBox.Location = new Point(label39.Location.X + label39.Width, label39.Location.Y);
            #endregion

        }

        private void parameterListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (parameterListBox.SelectedIndex > -1)
            {
                string type = parameterListBox.SelectedItem.ToString();
                if (type == "Place of Origin")
                {
                    codeLabel.Text = "Eng Name";
                    descriptionLabel.Text = "Chi Name";
                }
                else
                {
                    codeLabel.Text = "Code";
                    descriptionLabel.Text = "Description";
                }
            }
        }

        private void addParameterBtn_Click(object sender, EventArgs e)
        {
            if (parameterListBox.SelectedIndex != -1)
            {
                string type = parameterListBox.SelectedItem.ToString();
                if (type == "Location" || type == "Farm code" || type == "Destination")
                {
                    #region add location , farm code, destination
                    if (codeTxt.Text == "" && descriptionTxt.Text == "")
                    {
                        string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("codeDescription"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);                       
                    }
                    else
                    {
                        string sql = "insert into ";
                        string checkSql = "select * from ";
                        switch (type)
                        {
                            case "Location": sql += "location "; checkSql += "location ";
                                break;
                            case "Farm code": sql += "farmCode "; checkSql += "farmCode ";
                                break;
                            case "Destination": sql += "destination "; checkSql += "destination ";
                                break;                            
                        }
                        
                        sql += "(code, description) values (@code, @description)";
                        checkSql += " where code = @code";
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                        con.Open();

                        SqlCeCommand checkCmd = new SqlCeCommand(checkSql, con);
                        checkCmd.CommandType = CommandType.Text;
                        checkCmd.Parameters.AddWithValue("@code", codeTxt.Text);
                        SqlCeDataReader checkDr = checkCmd.ExecuteReader();
                        bool allow = true;
                        if (checkDr.Read())
                        {
                            allow = false;
                        }
                        checkDr.Close();
                        if (allow)
                        {
                            SqlCeCommand insertCmd = new SqlCeCommand(sql, con);
                            insertCmd.CommandType = CommandType.Text;
                            insertCmd.Parameters.AddWithValue("@code", codeTxt.Text);
                            insertCmd.Parameters.AddWithValue("@description", descriptionTxt.Text);
                            insertCmd.ExecuteNonQuery();
                            insertCmd.Dispose();
                        }
                        else
                        {
                            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("codeDuplicateError"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);       
                        }
                        con.Close();
                        GlobalFunc.logManager.WriteLog("Created + " + type + " parameter");
                        LoadData();
                        
                        if (type == "location")
                        {
                            GetLocation();
                        }
                        Reset();
                    }
                    #endregion

                }
                else if (type == "Species")
                {
                    #region add species
                    if (codeTxt.Text == "" && descriptionTxt.Text == "")
                    {
                        string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("codeDescription"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                    }
                    else
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                        con.Open();
                        SqlCeCommand checkCmd = new SqlCeCommand("select * from species where code = @code", con);
                        checkCmd.CommandType = CommandType.Text;
                        checkCmd.Parameters.AddWithValue("@code", codeTxt.Text);
                        SqlCeDataReader checkDr = checkCmd.ExecuteReader();
                        bool allow = true;
                        if (checkDr.Read())
                        {
                            allow = false;
                        }
                        checkDr.Close();
                        if (allow)
                        {
                            SqlCeCommand insertCmd = new SqlCeCommand("insert into species (code, description, scriptPath) values (@code, @description, '')", con);
                            insertCmd.CommandType = CommandType.Text;
                            insertCmd.Parameters.AddWithValue("@code", codeTxt.Text);
                            insertCmd.Parameters.AddWithValue("@description", descriptionTxt.Text);
                            insertCmd.ExecuteNonQuery();
                            insertCmd.Dispose();
                        }
                        else
                        {
                            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("codeDuplicateError"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);  
                        }
                        con.Close();
                        GlobalFunc.logManager.WriteLog("Created + " + type + " parameter");
                        LoadData();
                        Reset();
                    }
                    #endregion
                }
                else if (type == "Place of Origin")
                {
                    if (codeTxt.Text == "")
                    {
                        string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("eng"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                    }
                    else
                    {
                        if (con.State == ConnectionState.Open)
                        {
                            con.Close();
                        }
                        con.Open();
                        SqlCeCommand checkCmd = new SqlCeCommand("select * from origin where engname = @code", con);
                        checkCmd.CommandType = CommandType.Text;
                        checkCmd.Parameters.AddWithValue("@code", codeTxt.Text);                     
                        SqlCeDataReader checkDr = checkCmd.ExecuteReader();
                        bool allow = true;
                        if (checkDr.Read())
                        {
                            allow = false;
                        }
                        checkDr.Close();
                        if (allow)
                        {
                            SqlCeCommand insertCmd = new SqlCeCommand("insert into origin (engname, chiname) values (@code, @description)", con);
                            insertCmd.CommandType = CommandType.Text;
                            insertCmd.Parameters.AddWithValue("@code", codeTxt.Text);
                            insertCmd.Parameters.AddWithValue("@description", descriptionTxt.Text);
                            insertCmd.ExecuteNonQuery();
                            insertCmd.Dispose();
                        }
                        else
                        {
                            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("originDuplicate"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        }
                        con.Close();
                        GlobalFunc.logManager.WriteLog("Created + " + type + " parameter");
                        LoadData();
                        Reset();
                    }             
                    
                }
                GlobalFunc.mainForm.RefreshData();
            }
            else
            {
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("parameterType"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
        }
        private void openScriptFileDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (openScriptFileDialog.FileName != "")
            {
                OpenFileDialog jfd = sender as OpenFileDialog;
                TextBox tb = scriptPanel.Controls.Find(toggleTextBox, true).FirstOrDefault() as TextBox;
                tb.Text = openScriptFileDialog.FileName;
            }
        }
       
        private void check_KeyPress(object sender, KeyPressEventArgs e)
        {
            bool blHasDot = false;
            //e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            if (Char.IsDigit(e.KeyChar) || e.KeyChar == '\b')
            {
                // Allow Digits and BackSpace char
            }
            else if (e.KeyChar == '.' && !blHasDot)
            {
                //Allows only one Dot Char
                blHasDot = true;
            }
            else
            {
                e.Handled = true;
            }
        }

        string toggleType = "";
        int toggleID = 0;
        private void locationListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (locationListBox.SelectedIndex > -1)
            {
                codeLabel.Text = "Code";
                descriptionLabel.Text = "Description";
                parameterListBox.SelectedIndex = 0;
                parameterListBox.Enabled = false;
                Location location = (Location)locationListBox.SelectedItem;
                codeTxt.Text = location.Code;
                descriptionTxt.Text = location.Description;
                toggleType = "location";
                toggleID = location.Id_key;


                addParameterBtn.Enabled = false;
                updateParameterBtn.Enabled = true;
                deleteParameterBtn.Enabled = true;
                cancelBtn.Enabled = true;

                //locationListBox.ClearSelected();
                farmCodeListBox.ClearSelected();
                speciesListBox.ClearSelected();
                destinationListBox.ClearSelected();
                originListBox.ClearSelected();

            }
        }

        private void farmCodeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (farmCodeListBox.SelectedIndex > -1)
            {
                codeLabel.Text = "Code";
                descriptionLabel.Text = "Description";

                parameterListBox.Enabled = false;
                parameterListBox.SelectedIndex = 1;

                FarmCode farmCode = (FarmCode)farmCodeListBox.SelectedItem;
                codeTxt.Text = farmCode.Code;
                descriptionTxt.Text = farmCode.Description;
                toggleType = "farmCode";
                toggleID = farmCode.Id_key;

                locationListBox.ClearSelected();
                //farmCodeListBox.ClearSelected();
                speciesListBox.ClearSelected();
                destinationListBox.ClearSelected();
                originListBox.ClearSelected();

                addParameterBtn.Enabled = false;
                updateParameterBtn.Enabled = true;
                deleteParameterBtn.Enabled = true;
                cancelBtn.Enabled = true;
            }
        }

        private void speciesListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (speciesListBox.SelectedIndex > -1)
            {
                codeLabel.Text = "Code";
                descriptionLabel.Text = "Description";

                parameterListBox.Enabled = false;
                parameterListBox.SelectedIndex = 2;

                Species species = (Species)speciesListBox.SelectedItem;
                codeTxt.Text = species.Code;
                descriptionTxt.Text = species.Description;
              
                toggleType = "species";
                toggleID = species.Id_key;

                locationListBox.ClearSelected();
                farmCodeListBox.ClearSelected();
                //speciesListBox.ClearSelected();
                destinationListBox.ClearSelected();
                originListBox.ClearSelected();

                addParameterBtn.Enabled = false;
                updateParameterBtn.Enabled = true;
                deleteParameterBtn.Enabled = true;
                cancelBtn.Enabled = true;
            }
        }

        private void destinationListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (destinationListBox.SelectedIndex > -1)
            {
                codeLabel.Text = "Code";
                descriptionLabel.Text = "Description";

                parameterListBox.Enabled = false;
                parameterListBox.SelectedIndex = 3;

                Destination destination = (Destination)destinationListBox.SelectedItem;
                codeTxt.Text = destination.Code;
                descriptionTxt.Text = destination.Description;
                toggleType = "destination";
                toggleID = destination.Id_key;


                locationListBox.ClearSelected();
                farmCodeListBox.ClearSelected();
                speciesListBox.ClearSelected();
                //destinationListBox.ClearSelected();
                originListBox.ClearSelected();

                addParameterBtn.Enabled = false;
                updateParameterBtn.Enabled = true;
                deleteParameterBtn.Enabled = true;
                cancelBtn.Enabled = true;
            }
        }

        private void originListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (originListBox.SelectedIndex > -1)
            {
                codeLabel.Text = "Eng Name";
                descriptionLabel.Text = "Chi Name";

                parameterListBox.Enabled = false;
                parameterListBox.SelectedIndex = 4;

                PlaceOfOrigin poo = (PlaceOfOrigin)originListBox.SelectedItem;
                codeTxt.Text = poo.EngName;
                descriptionTxt.Text = poo.ChiName;
                toggleType = "placeoforigin";
                toggleID = poo.Id_key;
                
                locationListBox.ClearSelected();
                farmCodeListBox.ClearSelected();
                speciesListBox.ClearSelected();
                destinationListBox.ClearSelected();
                //originListBox.ClearSelected();

                addParameterBtn.Enabled = false;
                updateParameterBtn.Enabled = true;
                deleteParameterBtn.Enabled = true;
                cancelBtn.Enabled = true;
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            Reset();
        }

        public void Reset()
        {
            parameterListBox.SelectedIndex = -1;
            parameterListBox.Enabled = true;
            addParameterBtn.Enabled = true;
            updateParameterBtn.Enabled = false;
            deleteParameterBtn.Enabled = false;
            cancelBtn.Enabled = false;
           
            locationListBox.ClearSelected();
            farmCodeListBox.ClearSelected();
            speciesListBox.ClearSelected();
            destinationListBox.ClearSelected();
       
            codeTxt.Text = "";
            descriptionTxt.Text = "";
                     
            toggleID = 0;
            toggleType = "";
            GetLocation();
        }

        private void updateParameterBtn_Click(object sender, EventArgs e)
        {
            string sql = "";
            if (toggleType == "location" || toggleType == "farmCode" || toggleType == "destination")
            {
                #region location/ farmcode / destination
                if (codeTxt.Text == "" && descriptionTxt.Text == "")
                {
                    string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("codeDescription"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);                   
                }
                else
                {
                    switch (toggleType)
                    {
                        case "location": sql = "update location ";
                            break;
                        case "farmCode": sql = "update farmCode ";
                            break;
                        case "destination": sql = "update destination ";
                            break;
                    }
                    sql += " set code = @code, description = @description where id_key = @toggleID";
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    con.Open();
                    SqlCeCommand cmd = new SqlCeCommand(sql, con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@code", codeTxt.Text);
                    cmd.Parameters.AddWithValue("@description", descriptionTxt.Text);
                    cmd.Parameters.AddWithValue("@toggleID", toggleID);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    con.Close();
                    GlobalFunc.logManager.WriteLog("Updated + " + toggleType + " parameter");
                    LoadData();
                    if (toggleType == "location")
                    {
                        GetLocation();
                    }
                    Reset();

                }
                #endregion
            }
            else if (toggleType == "species")
            {
                if (codeTxt.Text == "" && descriptionTxt.Text == "")
                {
                    string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("codeDescription"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                else
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    con.Open();
                    SqlCeCommand cmd = new SqlCeCommand("update species set code = @code, description = @description where id_key = @toggleID", con);
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.AddWithValue("@code", codeTxt.Text);
                    cmd.Parameters.AddWithValue("@description", descriptionTxt.Text);  
                    cmd.Parameters.AddWithValue("@toggleID", toggleID);
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    con.Close();
                    GlobalFunc.logManager.WriteLog("Updated + " + toggleType + " parameter");
                    LoadData();
                    Reset();
                }
            }
            else if (toggleType == "placeoforigin")
            {
                if (codeTxt.Text == "" && descriptionTxt.Text == "")
                {
                    string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("engChi"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                else
                {
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    con.Open();
                    SqlCeCommand checkCmd = new SqlCeCommand("select * from origin where Id != @toggleID and (engname = @code or chiname = @description)", con);
                    checkCmd.CommandType = CommandType.Text;
                    checkCmd.Parameters.AddWithValue("@code", codeTxt.Text);
                    checkCmd.Parameters.AddWithValue("@description", descriptionTxt.Text);
                    checkCmd.Parameters.AddWithValue("@toggleID", toggleID);
                    SqlCeDataReader checkDr = checkCmd.ExecuteReader();
                    bool allow = true;
                    if (checkDr.Read())
                    {
                        allow = false;
                    }
                    checkDr.Close();
                    if (allow)
                    {
                        SqlCeCommand insertCmd = new SqlCeCommand("update origin set engname = @code, chiname = @description where Id = @toggleID", con);
                        insertCmd.CommandType = CommandType.Text;
                        insertCmd.Parameters.AddWithValue("@code", codeTxt.Text);
                        insertCmd.Parameters.AddWithValue("@description", descriptionTxt.Text);
                        insertCmd.Parameters.AddWithValue("@toggleID", toggleID);
                        insertCmd.ExecuteNonQuery();
                        insertCmd.Dispose();
                    }
                    else
                    {
                        string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("originDuplicate"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                    }
                    con.Close();
                    GlobalFunc.logManager.WriteLog("Updated + " + toggleType + " parameter");
                    LoadData();
                    Reset();
                }                
            }
            GlobalFunc.mainForm.RefreshData();
        }

        private void deleteParameterBtn_Click(object sender, EventArgs e)
        {
            string sql = "";
            switch (toggleType)
            {
                case "location": sql = "delete from location where id_key = @toggleID";
                    break;
                case "farmCode": sql = "delete from farmCode where id_key = @toggleID";
                    break;
                case "destination": sql = "delete from destination where id_key = @toggleID";
                    break;
                case "species": sql = "delete from species where id_key = @toggleID";
                    break;
                case "placeoforigin": sql = "delete from origin where Id = @toggleID";
                    break;
            }
            sql += "";
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand cmd = new SqlCeCommand(sql, con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@toggleID", toggleID);
            cmd.ExecuteNonQuery();
            cmd.Dispose();
            con.Close();
            GlobalFunc.logManager.WriteLog("Deleted + " + toggleType + " parameter");
            LoadData();
            Reset();
        }
        #endregion

        #region account
        private void addUserBtn_Click(object sender, EventArgs e)
        {
            if (addUserName.Text == "")
            {
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("username"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            if (addPassword.Text == "" && addPassword.Text.Length < 8)
            {
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("password"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else if (addAccountType.SelectedIndex < 0)
            {
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("accountType"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                int type = 1;
                if (addAccountType.SelectedIndex == 0)
                {
                    type = 1;
                }
                else
                {
                    type = 2;
                }
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand insertCmd = new SqlCeCommand("insert into userPassword (password, type, username) values (@password, @type, @username)", con);
                insertCmd.CommandType = CommandType.Text;
                insertCmd.Parameters.AddWithValue("@password", addPassword.Text);
                insertCmd.Parameters.AddWithValue("@type", type);
                insertCmd.Parameters.AddWithValue("@username", addUserName.Text);
                insertCmd.ExecuteNonQuery();
                insertCmd.Dispose();
                con.Close();
                LoadAccount();
            }
        }

        public void LoadAccount()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            List<UserPassword> upList = new List<UserPassword>();
            SqlCeCommand cmd = new SqlCeCommand("select * from userPassword with (nolock) order by id_key", con); //c.con is the connection string
            cmd.CommandType = CommandType.Text;
            SqlCeDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                UserPassword up = new UserPassword();
                up.Id_key = Convert.ToInt32(dr["id_key"]);
                up.Username = dr["username"].ToString();
                up.Password = dr["password"].ToString();
                if (Convert.ToInt32(dr["type"]) == 1)
                {
                    up.AccountType = "Supervisor";
                }
                else
                {
                    up.AccountType = "Operator";
                }
                DisplayText dt = new DisplayText();
                dt.code = "ID:" + up.Id_key;
                dt.description = up.Username + " " + up.AccountType;
                up.DisplayText = dt;
                upList.Add(up);
            }
            dr.Close();
            userList.DataSource = upList;
            userList.DisplayMember = "DisplayText";
            userList.ValueMember = "Id_Key";
            userList.ClearSelected();
            con.Close();

            ResetUser();
        }

        public void ResetUser() 
        {
            editAccountType.SelectedIndex = -1;
            editPassword.Text = "";
            editUserBtn.Enabled = false;
            deleteUserBtn.Enabled = false;
        }

        int toggleUserID = 0;
        private void userList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (userList.SelectedIndex > -1)
            {               
                UserPassword userPassword = (UserPassword)userList.SelectedItem;
                if (userPassword.Id_key == 1)
                {
                    editUserBtn.Enabled = false;
                    deleteUserBtn.Enabled = false;
                }
                else
                {
                    toggleUserID = userPassword.Id_key;
                    editUserID.Text = toggleUserID.ToString();
                    editUserName.Text = userPassword.Username;
                    editPassword.Text = userPassword.Password;
                    if (userPassword.AccountType == "Supervisor")
                    {
                        editAccountType.SelectedIndex = 0;
                    }
                    else
                    {
                        editAccountType.SelectedIndex = 1;
                    }
                    editUserBtn.Enabled = true;
                    deleteUserBtn.Enabled = true;
                }
            }
        }

        private void deleteUserBtn_Click(object sender, EventArgs e)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand delCmd = new SqlCeCommand("delete from userPassword where id_key = @toggleUserID", con);
            delCmd.CommandType = CommandType.Text;
            delCmd.Parameters.AddWithValue("@toggleUserID", toggleUserID);
            delCmd.ExecuteNonQuery();
            delCmd.Dispose();
            con.Close();
            LoadAccount();
            ResetUser();
        }

        private void editUserBtn_Click(object sender, EventArgs e)
        {
            if (editUserName.Text == "")
            {
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("username"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);           
            }
            if (editPassword.Text == "" && editPassword.Text.Length < 8)
            {
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("password"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else if (editAccountType.SelectedIndex < 0)
            {
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("accountType"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                int type = 1;
                if (editAccountType.SelectedIndex == 0)
                {
                    type = 1;
                }
                else
                {
                    type = 2;
                }
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand insertCmd = new SqlCeCommand("update userPassword set username = @username, password = @password, type = @type where id_key = @toggleUserID", con);
                insertCmd.CommandType = CommandType.Text;
                insertCmd.Parameters.AddWithValue("@username", editUserName.Text);
                insertCmd.Parameters.AddWithValue("@password", editPassword.Text);
                insertCmd.Parameters.AddWithValue("@type", type);
                insertCmd.Parameters.AddWithValue("@toggleUserID", toggleUserID);
                insertCmd.ExecuteNonQuery();
                insertCmd.Dispose();
                con.Close();
                LoadAccount();
                ResetUser();
            }
        }
        #endregion

        #region Measurement
        public void LoadProfile()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand proCmd = new SqlCeCommand("select * from profileName order by Id", con);
            proCmd.CommandType = CommandType.Text;
            SqlCeDataReader proDr = proCmd.ExecuteReader();
            List<Profile> pList = new List<Profile>();
            Profile empty = new Profile();
            pList.Add(empty);
            while (proDr.Read())
            {
                Profile pn = new Profile();
                pn.ID = Convert.ToInt32(proDr["id"]);
                pn.ProfileName = proDr["profileName"].ToString();
                pn.FileName = proDr["fileName"].ToString();
                pList.Add(pn);
            }
            proDr.Close();
            con.Close();
            profileComboBox.DataSource = pList;
            profileComboBox.DisplayMember = "ProfileName";
            profileComboBox.ValueMember = "ID";
        }
        
        #endregion
        
        #region numerical 
        public void LoadTime()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand cmd = new SqlCeCommand("select * from time", con);
            cmd.CommandType = CommandType.Text;
            SqlCeDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                sampleTime.Text = dr["sample"].ToString();
                bkTime.Text = dr["background"].ToString();
                alarmTime.Text = dr["alarm"].ToString();
                GlobalFunc.dual_icr_alarm = Convert.ToDouble(dr["alarm"]);

                bufferTimeTxt.Text = dr["bufferTime"].ToString();
                warmUpTimeTxt.Text = dr["warmupTime"].ToString();
            }
            dr.Close();

            cmd = new SqlCeCommand("select * from icr", con);
            cmd.CommandType = CommandType.Text;
            dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (dr["detector"].ToString() == "Top")
                {
                    topICRTxt.Text = dr["icr"].ToString();
                    GlobalFunc.det1_icr_alarm = Convert.ToDouble(dr["icr"]);
                }
                else if (dr["detector"].ToString() == "Bottom")
                {
                    bottomICRTxt.Text = dr["icr"].ToString();
                    GlobalFunc.det2_icr_alarm = Convert.ToDouble(dr["icr"]);
                }
            }
            con.Close();

            if (GlobalFunc.rsl == 1)
            {
                rsEnable.Checked = true;
            }
            else if (GlobalFunc.rsl == 0)
            {
                rsDisable.Checked = true;
            }

            if (GlobalFunc.ru == 1)
            {
                ruEnable.Checked = true;
            }
            else if (GlobalFunc.ru == 0)
            {
                ruDisable.Checked = true;
            }

            if (GlobalFunc.mlr == 1)
            {
                mlrEnable.Checked = true;
            }
            else if (GlobalFunc.mlr == 0)
            {
                mlrDisable.Checked = true;
            }
        }

        private void updSampleTimeBtn_Click(object sender, EventArgs e)
        {
            if (sampleTime.Text != "")
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("update time set sample = @sample", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@sample", sampleTime.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            { }
        }

        private void updBkTimeBtn_Click(object sender, EventArgs e)
        {
            if (bkTime.Text != "")
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("update time set background = @background", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@background", bkTime.Text);
                cmd.ExecuteNonQuery();

                SqlCeCommand bkCmd = new SqlCeCommand("select background from time", con);
                bkCmd.CommandType = CommandType.Text;
                SqlCeDataReader bkDr = bkCmd.ExecuteReader();
                if (bkDr.Read())
                {
                    BKManager.supposeMillSecond = Convert.ToDouble(bkDr["background"]) * 60 * 1000;
                    BKManager.supposeSecond = Convert.ToDouble(bkDr["background"]) * 60;
                }
                bkDr.Close();                                
                con.Close();
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            { }
        }

        private void updAlarmTimeBtn_Click(object sender, EventArgs e)
        {
            if (alarmTime.Text != "")
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("update time set alarm = @alarm", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@alarm", alarmTime.Text);
                cmd.ExecuteNonQuery();
                con.Close();
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            { }
        }
        #endregion

        string statusText = "";
        int r = 0;
        int g = 0;
        int b = 0;
        public void UpdateStatusLabel()
        {
            textingLabel.Text = statusText;
            textingLabel.BackColor = Color.FromArgb(r, g, b);
        }
        public void UpdateStatusLabel(string statusText, int r, int g, int b)
        {

            textingLabel.Invoke(new MethodInvoker
                   (delegate
                   {
                       textingLabel.Text = statusText;
                       textingLabel.BackColor = Color.FromArgb(r, g, b);
                   }
                   )
            );
        }

        //branching ratio of the peak
        #region maintenance
        private void backupBtn_Click(object sender, EventArgs e)
        {
            BackupXMLDB(true);
        }

        public void BackupXMLDB(bool show)
        { 
            scriptPanel.Visible = false;
            if (!Directory.Exists(@Directory.GetCurrentDirectory() + @"\Backup"))
            {
                Directory.CreateDirectory(@Directory.GetCurrentDirectory() + @"\Backup");
            }

            string path = @Directory.GetCurrentDirectory() + @"\Backup\" + DateTime.Now.Year + "_" + DateTime.Now.Month + "_" + DateTime.Now.Day + "_" +DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            File.Copy(@Directory.GetCurrentDirectory() + @"\lcmsDB.sdf", path + @"\lcmsDB.sdf");

            if (!Directory.Exists(path + @"\xml"))
            {
                Directory.CreateDirectory(path + @"\xml");
            }
            if (!Directory.Exists(path + @"\Profile"))
            {
                Directory.CreateDirectory(path + @"\Profile");
            }
            if (!Directory.Exists(path + @"\JobFiles"))
            {
                Directory.CreateDirectory(path + @"\JobFiles");
            }
            if (!Directory.Exists(path + @"\Roi"))
            {
                Directory.CreateDirectory(path + @"\Roi");
            }
            Copy(path, "xml");
            Copy(path, "Profile");
            Copy(path, "JobFiles");
            Copy(path, "Roi");
            if (show)
            {
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
        }

        public void Copy(string dest, string folderName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(@Directory.GetCurrentDirectory() + @"\" + folderName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(dest + @"\" + folderName, file.Name);
                file.CopyTo(temppath, false);
            }
        }

        public void Rollback(string folderName)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(@Directory.GetCurrentDirectory() + @"\defaultSetting\" + folderName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {              
                string temppath = Path.Combine(@Directory.GetCurrentDirectory() + @"\" + folderName, file.Name);
                File.Delete(temppath);
                file.CopyTo(temppath, false);
            }
        }
        #endregion

        static List<IsoSeq> dummyIsoSeqList = new List<IsoSeq>();

        int selectedProfileID = 0;

        public void LoadSelectedProfile(bool show)
        {
            calBkBtn.Enabled = true;
            saveProfileBtn.Enabled = true;
            GlobalFunc.loadProfile = new Profile();
            isotopeNumComboBox.SelectedIndex = -1;
            panel1.Controls.Clear();
            Profile profile = (Profile)profileComboBox.SelectedItem;

            if (profile.ProfileName != "")
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand deleteCmd = new SqlCeCommand("delete from currProfile", con);
                deleteCmd.CommandType = CommandType.Text;
                deleteCmd.ExecuteNonQuery();
                deleteCmd.Dispose();
                SqlCeCommand insertCmd = new SqlCeCommand("insert into currProfile values (@profileName, @fileName)", con);
                insertCmd.CommandType = CommandType.Text;
                insertCmd.Parameters.AddWithValue("@profileName", profile.ProfileName);
                insertCmd.Parameters.AddWithValue("@fileName", profile.FileName.Replace(".txt", "") + ".txt");
                insertCmd.ExecuteNonQuery();
                insertCmd.Dispose();
                con.Close();
                if (show)
                {
                    string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("select"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                #region read profile file

                if (File.Exists(@"C:\LCMS\Profile\" + profile.FileName.Replace(".txt", "") + ".txt"))
                {
                    GlobalFunc.loadProfile.ID = profile.ID;
                    selectedProfileID = profile.ID;
                    GlobalFunc.loadProfile = GlobalFunc.LoadProfile(profile.FileName.Replace(".txt", "") + ".txt");
                    GlobalFunc.loadProfile.FileName = @"C:\LCMS\Profile\" + profile.FileName.Replace(".txt", "") + ".txt";
                    profileName.Text = GlobalFunc.loadProfile.ProfileName;
                    nor.Text = GlobalFunc.loadProfile.NoOfRegion.ToString();
                    if (GlobalFunc.loadProfile.Alarm == "DIL")
                    {
                        alarmComboBox.SelectedIndex = 0;
                    }
                    else if (GlobalFunc.loadProfile.Alarm == "AL")
                    {
                        alarmComboBox.SelectedIndex = 1;
                    }

                    for (int i = 0; i < selBkLocation.Items.Count; i++)
                    {
                        Location l = (Location)selBkLocation.Items[i];
                        if (l.Description == GlobalFunc.loadProfile.Location)
                        {
                            selBkLocation.SelectedIndex = i;
                            break;
                        }
                    }

                    if (GlobalFunc.loadProfile.Detector == "Top")
                    {
                        detectorComboBox.SelectedIndex = 0;
                        roiPathText2.Enabled = false;
                        selectRoiPathBtn2.Enabled = false;
                    }
                    else if (GlobalFunc.loadProfile.Detector == "Bottom")
                    {
                        detectorComboBox.SelectedIndex = 1;
                        roiPathText1.Enabled = false;
                        selectRoiPathBtn1.Enabled = false;
                    }
                    else if (GlobalFunc.loadProfile.Detector == "Dual")
                    {
                        detectorComboBox.SelectedIndex = 2;
                        roiPathText1.Enabled = true;
                        selectRoiPathBtn1.Enabled = true;
                        roiPathText2.Enabled = true;
                        selectRoiPathBtn2.Enabled = true;
                    }

                    dummyIsoSeqList = GlobalFunc.loadProfile.IsoSeqList;

                    if (GlobalFunc.loadProfile.Qty == 4)
                    {
                        isotopeNumComboBox.SelectedIndex = 0;
                    }
                    else if (GlobalFunc.loadProfile.Qty == 5)
                    {
                        isotopeNumComboBox.SelectedIndex = 1;
                    }
                    else if (GlobalFunc.loadProfile.Qty == 6)
                    {
                        isotopeNumComboBox.SelectedIndex = 2;
                    }
                    isotopeNumComboBox.Enabled = false;
                    roiPathText1.Text = GlobalFunc.loadProfile.RoiPath1;
                    roiPathText2.Text = GlobalFunc.loadProfile.RoiPath2;
                    nor.Text = GlobalFunc.loadProfile.NoOfRegion.ToString();
                }
                else
                {
                    string btnClicked2 = CustomMessageBox.Show(GlobalFunc.rm.GetString("profileFileError"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                #endregion

            }
            else
            {
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("profileError"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }       
        }

        private void selectProfileBtn_Click(object sender, EventArgs e)
        {
            LoadSelectedProfile(true);                
        }

        private void deleteProfileBtn_Click(object sender, EventArgs e)
        {
             Profile profile = (Profile)profileComboBox.SelectedItem;
             calBkBtn.Enabled = true;
             if (profile.ProfileName != "")
             {
                 if (con.State == ConnectionState.Open)
                 {
                     con.Close();
                 }
                 con.Open();
                 SqlCeCommand delCmd = new SqlCeCommand("delete from profileName where Id = @id", con);
                 delCmd.CommandType = CommandType.Text;
                 delCmd.Parameters.AddWithValue("@id", profile.ID);
                 delCmd.ExecuteNonQuery();
                 delCmd.Dispose();
                 con.Close();
                 if (File.Exists(@"C:\LCMS\Profile\" + profile.FileName.Replace(".txt", "") + ".txt"))
                 {
                     File.Delete(@"C:\LCMS\Profile\" + profile.FileName.Replace(".txt", "") + ".txt");
                 }
                 LoadProfile();
                 try
                 {
                     GlobalFunc.mainForm.LoadProfile();
                 }
                 catch { }
                 GlobalFunc.loadProfile = new Profile();
                 calIsotopeList.Clear();
                 panel1.Controls.Clear();
                 isotopeNumComboBox.SelectedIndex = -1;
                 detectorComboBox.SelectedIndex = -1;
                 alarmComboBox.SelectedIndex = -1;
                 profileName.Text = "";
                 doubleTopDetectorCPSList.Clear();
                 doubleBottomDetectorCPSList.Clear();
                 BKManager.activity.Clear();
                 BKManager.halfTime.Clear();
                 BKManager.ref_Date.Clear();
                 BKManager.ref_Date.Clear();
                 topReportDateList.Clear();
                 bottomReportDateList.Clear();
             }
             else
             {
                 string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("profileError"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
             }
        }

        private void refreshXmlBtn_Click(object sender, EventArgs e)
        {
            scriptPanel.Visible = false;
            XmlSerializer deserializer1 = new XmlSerializer(typeof(BasicSetting));
            TextReader textReader = new StreamReader(@Directory.GetCurrentDirectory() + @"\xml\Basic.xml");
            GlobalFunc.basicSetting = (BasicSetting)deserializer1.Deserialize(textReader);
            textReader.Close();

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
            GetLocation();
            LoadData();
            Reset();
            LoadAccount();
            LoadProfile();
            LoadTime();
        }

        public void GetLocation()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand cmd = new SqlCeCommand("select * from location order by id_key", con);
            cmd.CommandType = CommandType.Text;
            SqlCeDataReader dr = cmd.ExecuteReader();
            List<Location> sampleLocationList = new List<Location>();
            while (dr.Read())
            {
                Location parameter = new Location();
                parameter.Id_key = Convert.ToInt32(dr["id_key"]);
                parameter.Code = dr["code"].ToString();
                parameter.Description = dr["description"].ToString();

                DisplayText dt = new DisplayText();
                dt.code = parameter.Code;
                dt.description = parameter.Description;
                parameter.DisplayText = dt;

                sampleLocationList.Add(parameter);
            }
            dr.Close();
            con.Close();

            selBkLocation.DataSource = sampleLocationList;
            selBkLocation.DisplayMember = "DisplayText";
            selBkLocation.ValueMember = "Id_Key";
            selBkLocation.SelectedIndex = -1;
            selBkLocation.Width = DropDownWidth2(selBkLocation);
        }

        #region calibrtation

        public void cb_SelectedIndexChanged(object sender, EventArgs e)
        {
            GlobalFunc.LoadIsotopXML();
            ComboBox cb = sender as ComboBox;
            int index = Convert.ToInt32(cb.Name.Replace("isotopeName", ""));
            int num = Convert.ToInt32(isotopeNumComboBox.SelectedItem.ToString());
            if (cb.SelectedIndex > -1)
            {
                Isotope isotope = (Isotope)cb.SelectedItem;
                if ((index + 1) < num) 
                {
                    ComboBox nextCB = (ComboBox)(panel1.Controls.Find("isotopeName" + (index + 1), true)[0]);
                    if (nextCB != null)
                    {
                        Point pt = nextCB.Location;
                        ComboBox newCB = new ComboBox();
                        newCB.Location = pt;
                        newCB.Name = nextCB.Name;
                        newCB.SelectedIndexChanged += cb_SelectedIndexChanged;
                        newCB.DropDownStyle = ComboBoxStyle.DropDownList;
                       
                        List<Isotope> dummyList = GlobalFunc.isotopeList1;
                        List<int> removeIntList = new List<int>();
                        for (int k = 0; k < dummyList.Count; k++)
                        {
                            if (dummyList[k].Mpe < isotope.Mpe )
                            {
                                removeIntList.Add(k);
                            }
                            else if(dummyList[k].Id_key == isotope.Id_key)
                            {
                                removeIntList.Add(k);
                            }
                        }
                        int removeCount = 0;
                        for (int k = 0; k < removeIntList.Count; k++)
                        {
                            dummyList.RemoveAt(removeIntList[k]-removeCount);
                            removeCount++;
                        }
                        newCB.DataSource = dummyList;
                        newCB.DisplayMember = "Code";
                        newCB.ValueMember = "Id_key";
                        newCB.SelectedIndex = -1;
                        newCB.DropDownWidth = DropDownWidth(newCB);
                        panel1.Controls.Remove(nextCB);
                        panel1.Controls.Add(newCB);                        
                    }
                }
            }
            panel1.Update();
        }

        private void isotopeNumComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isotopeNumComboBox.SelectedIndex > -1)
            {
                panel1.Controls.Clear();
                //GlobalFunc.loadProfile = new Profile();
                calIsotopeList.Clear();
                
                int num = Convert.ToInt32(isotopeNumComboBox.SelectedItem.ToString());

                int x = 0;
                int y = 0;
                //Calibri, 15.75pt, style=Bold
                Font f = new Font("Calibri", 12);

                for (int i = 0; i < num; i++)
                {
                    Label numLabel = new Label();
                    numLabel.Width = 100;                   
                    numLabel.AutoSize = true;
                    numLabel.Text = "ROI # " + (i + 1).ToString();
                    numLabel.Font = new System.Drawing.Font("Calibri", 12, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
                    numLabel.Location = new Point(x, y);
                    panel1.Controls.Add(numLabel);

                    ComboBox cb = new ComboBox();
                    cb.Enabled = false;       
                    cb.BindingContext = new BindingContext();
                    cb.DisplayMember = "Code";
                    cb.ValueMember = "Id_key";                                    
                    cb.Location = new Point(numLabel.Location.X + numLabel.Width, y);
                    cb.DropDownStyle = ComboBoxStyle.DropDownList;    
                    cb.Name = "isotopeName" + i;
                    cb.DropDownWidth = DropDownWidth(cb);

                    /*if (i == 0)
                    {
                        cb.SelectedIndexChanged += cb_SelectedIndexChanged;
                    }*/
                    panel1.Controls.Add(cb);

                    //Activity, Bq
                    Label acLabel = new Label();
                    acLabel.Text = "Activity, kBq";
                    acLabel.AutoSize = true;
                    acLabel.Location = new Point(cb.Location.X + cb.Width+15, y);
                    panel1.Controls.Add(acLabel);

                    TextBox acTb = new TextBox();
                    acTb.Text = "1";
                    acTb.Location = new Point(acLabel.Location.X + acLabel.Width, y);
                    acTb.KeyPress += acTb_KeyPress;
                    if (GlobalFunc.loadProfile.IsoSeqList.Count == num)
                    {
                        acTb.Text = GlobalFunc.loadProfile.IsoSeqList[i].Activity;
                    }
                    acTb.Name = "Activity" + i;
                    acTb.Width = TextRenderer.MeasureText("999999.99", acTb.Font).Width + 20;
                    acTb.MaxLength = 9;
                    panel1.Controls.Add(acTb);

                    //Ref. Date
                    Label dateLable = new Label();
                    dateLable.AutoSize = true;
                    dateLable.Text = "Ref. Date & Time";
                    dateLable.Location = new Point(acTb.Location.X + dateLable.Width+10, y);
                    panel1.Controls.Add(dateLable);

                    DateTimePicker dp = new DateTimePicker();
                    DateTime result = DateTime.Now;
                    dp.CustomFormat = "yyyy/MM/dd HH:mm";
                    dp.Format = DateTimePickerFormat.Custom;
                    if (GlobalFunc.loadProfile.IsoSeqList.Count == num)
                    {
                        DateTime thisDate = GlobalFunc.loadProfile.IsoSeqList[i].RefDateTime;
                        dp.Value = new DateTime(thisDate.Year, thisDate.Month, thisDate.Day, thisDate.Hour, thisDate.Minute, 0);
                        //dp.Value = GlobalFunc.loadProfile.IsoSeqList[i].RefDateTime;
                    }
                    else
                    {
                        dp.Value = result;
                    }
                    dp.Width = TextRenderer.MeasureText("2015/04/19 00:00", dp.Font).Width + 60;
                    dp.Location = new Point(dateLable.Location.X + dateLable.Width, y);
                    dp.Name = "refDate" + i;
                    panel1.Controls.Add(dp);
                              
                    y = numLabel.Location.Y + 30;

                    //dateLable
                    Label measureTimeLabel = new Label();
                    measureTimeLabel.Text = "Measure Time (min)";
                    measureTimeLabel.AutoSize = true;
                    measureTimeLabel.Location = new Point(numLabel.Location.X, y);
                    panel1.Controls.Add(measureTimeLabel);

                    TextBox timeTb = new TextBox();
                    timeTb.Name = "MeasureTime" + i;
                    timeTb.KeyPress += acTb_KeyPress;
                    timeTb.Text = "0.1";
                    if (GlobalFunc.loadProfile.IsoSeqList.Count == num)
                    {
                        timeTb.Text = GlobalFunc.loadProfile.IsoSeqList[i].MeasureTime;
                    }
                    timeTb.Width = TextRenderer.MeasureText("999.9", timeTb.Font).Width + 20;
                    timeTb.MaxLength = 5;
                    timeTb.Location = new Point(measureTimeLabel.Location.X + measureTimeLabel.Width, y);
                    panel1.Controls.Add(timeTb);

                    Label dil = new Label();
                    dil.Text = "DIL(Alarm), Bq/kg";
                    dil.AutoSize = true;
                    dil.Location = new Point(timeTb.Location.X + timeTb.Width, y);
                    panel1.Controls.Add(dil);
                    TextBox dilTb = new TextBox();
                    dilTb.Name = "DIL" + i;
                    dilTb.Text = "1";
                    if (GlobalFunc.loadProfile.IsoSeqList.Count == num)
                    {
                        dilTb.Text = GlobalFunc.loadProfile.IsoSeqList[i].DIL;
                    }
                    dilTb.Width = TextRenderer.MeasureText("99999", dilTb.Font).Width + 20;
                    dilTb.MaxLength = 5;
                    dilTb.KeyPress += acTb_KeyPress;
                    dilTb.Location = new Point(dil.Location.X + dil.Width, y);
                    panel1.Controls.Add(dilTb);

                    Label al = new Label();
                    al.Text = "AL(Alarm), Bq/kg";
                    al.AutoSize = true;
                    al.Location = new Point(dilTb.Location.X + dilTb.Width, y);
                    panel1.Controls.Add(al);
                    TextBox alTb = new TextBox();
                    alTb.Name = "AL" + i;
                    alTb.Text = "1";
                    if (GlobalFunc.loadProfile.IsoSeqList.Count == num)
                    {
                        alTb.Text = GlobalFunc.loadProfile.IsoSeqList[i].AL;
                    }
                    alTb.Width = TextRenderer.MeasureText("99999", dilTb.Font).Width + 20;
                    alTb.MaxLength = 5;
                    alTb.KeyPress += acTb_KeyPress;
                    alTb.Location = new Point(al.Location.X + al.Width, y);
                    panel1.Controls.Add(alTb);

                    Label alpc = new Label();
                    alpc.Text = "AL(Alarm), %";
                    alpc.AutoSize = true;
                    alpc.Location = new Point(alTb.Location.X + alTb.Width, y);
                    panel1.Controls.Add(alpc);
                    TextBox alpcTb = new TextBox();
                    alpcTb.Name = "AL%" + i;
                    alpcTb.Text = "1";
                    if (GlobalFunc.loadProfile.IsoSeqList.Count == num)
                    {
                        alpcTb.Text = GlobalFunc.loadProfile.IsoSeqList[i].AL_PC;
                    }
                    alpcTb.Width = TextRenderer.MeasureText("99.9", alpcTb.Font).Width + 20;
                    alpcTb.MaxLength = 4;
                    alpcTb.KeyPress += acTb_KeyPress;
                    alpcTb.Location = new Point(alpc.Location.X + alpc.Width, y);
                    panel1.Controls.Add(alpcTb);
                    
                    Label runLabel = new Label();
                    runLabel.Text = "Run Count:";
                    runLabel.AutoSize = true;
                    runLabel.Location = new Point(alpcTb.Location.X + alpcTb.Width, y);
                    panel1.Controls.Add(runLabel);

                    Label runCounterLabel = new Label();
                    runCounterLabel.Text = "0";
                    runCounterLabel.Name = "runCounterLabel" + i;
                    runCounterLabel.AutoSize = true;
                    runCounterLabel.Location = new Point(runLabel.Location.X + runLabel.Width, y);
                    panel1.Controls.Add(runCounterLabel);

                    Button btn = new Button();
                    btn.Text = "Run";
                    btn.Click += individual_Click;
                    btn.Name = "Btn" + i;
                    btn.Enabled = false;
                    btn.Location = new Point(runCounterLabel.Location.X + runCounterLabel.Width, y);
                    btn.Height = 28;
                    panel1.Controls.Add(btn);

                    PictureBox pb = new PictureBox();
                    pb.ImageLocation = @Directory.GetCurrentDirectory() + "/tick.png";
                    pb.Location = new Point(btn.Location.X + btn.Width, y);
                    pb.Size = new Size(30, 29);
                    pb.Name = "pb" + i;
                    pb.Visible = false;
                    panel1.Controls.Add(pb);

                    y = measureTimeLabel.Location.Y + 30;
                }
                calibrationBtn.Location = new Point(calibrationBtn.Location.X, panel1.Height + 120);
                //abortBtn.Location = new Point(abortBtn.Location.X, panel1.Height + 120);
                //calBkBtn.Location = new Point(calBkBtn.Location.X, abortBtn.Location.Y - abortBtn.Height);
                if (dummyIsoSeqList.Count > 0)
                {
                    for (int i = 0; i < num; i++)
                    {
                        GlobalFunc.LoadIsotopXML();
                        ComboBox roiCB = (ComboBox)(panel1.Controls.Find("isotopeName" + i, true)[0]);
                        roiCB.DataSource = GlobalFunc.isotopeList1;
                        for (int k = 0; k < roiCB.Items.Count; k++)
                        {
                            Isotope iso = (Isotope)roiCB.Items[k];
                            if (iso.Code == dummyIsoSeqList[i].Name)
                            {
                                roiCB.SelectedIndex = k;
                                break;
                            }
                        }
                    }

                    for (int i = 0; i < num; i++)
                    {
                        GlobalFunc.LoadIsotopXML();
                        ComboBox roiCB = (ComboBox)(panel1.Controls.Find("isotopeName" + i, true)[0]);
                        roiCB.SelectedIndexChanged += cb_SelectedIndexChanged;
                        roiCB.Enabled = true;
                    }
                }
                else
                {
                    for (int i = 0; i < num; i++)
                    {
                        GlobalFunc.LoadIsotopXML();
                        ComboBox roiCB = (ComboBox)(panel1.Controls.Find("isotopeName" + i, true)[0]);
                        if (i == 0)
                        {
                            roiCB.DataSource = GlobalFunc.isotopeList1;
                            roiCB.SelectedIndex = -1;
                            roiCB.SelectedIndexChanged += cb_SelectedIndexChanged;
                            roiCB.Enabled = true;
                        }
                        else
                        { 
                        
                        }
                    }
                   
                }
            }
        }

        protected void acTb_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && (e.KeyChar != '.'))
            {
                e.Handled = true;
            }
        }

        protected void bufferTime_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        protected void profileName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 92)
            {
                e.Handled = true;
            }
        }

        public void KillMe()
        {
            bkWorker.CancelAsync();
            bkWorker.Dispose();
            bkWorker = null;
            GC.Collect();
        }
        #endregion 

        #region cal background 
        List<CalIsotope> calIsotopeList = new List<CalIsotope>();
        string _detector = "";
        private void calBkBtn_Click(object sender, EventArgs e)
        {
            try
            {
                calIsotopeList = new List<CalIsotope>();
                runBk = "yes";
                try
                {
                    GlobalFunc.SetAlarmBox(0);
                }
                catch { }
                countSeconds = 0;
                toggleFirstTime = true;
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("confirmCalibrationTxt"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), GlobalFunc.rm.GetString("noBtnTxt"), false, 0);
                if (btnClicked == "1")
                {
                    nor.Enabled = false;
                    selBkLocation.Enabled = false;
                    alarmComboBox.Enabled = false;
                    isotopeNumComboBox.Enabled = false;
                    detectorComboBox.Enabled = false;
                    
                    bool passToRun = true;

                    if (detectorComboBox.SelectedIndex < 0)
                    {
                        passToRun = false;
                        string buttonID = CustomMessageBox.Show("Number of region cannot empty", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);                     
                    }
                    else
                    {
                        if (detectorComboBox.SelectedIndex == 0)
                        {
                            _detector = "Top";
                            toggleDetector = "Top";
                        }
                        else if (detectorComboBox.SelectedIndex == 1)
                        {
                            _detector = "Bottom";
                            toggleDetector = "Bottom";
                        }
                        else if (detectorComboBox.SelectedIndex == 2)
                        {
                            _detector = "Dual";
                            toggleDetector = "Dual";
                        }

                        switch (_detector.ToLower())
                        {
                            case "top": BKManager.beginBdgScript = GlobalFunc.topScriptSet.BackgroundBegin; BKManager.endBdgScript = GlobalFunc.topScriptSet.BackgroundEnd;
                                break;
                            case "bottom": BKManager.beginBdgScript = GlobalFunc.bottomScriptSet.BackgroundBegin; BKManager.endBdgScript = GlobalFunc.bottomScriptSet.BackgroundEnd;
                                break;
                            case "dual": BKManager.beginBdgScript = GlobalFunc.dualScriptSet.BackgroundBegin; BKManager.endBdgScript = GlobalFunc.dualScriptSet.BackgroundEnd;
                                break;
                        }
                        this.Enabled = false;
                    }

                    if (isotopeNumComboBox.SelectedIndex < 0)
                    {
                        passToRun = false;
                        string buttonID = CustomMessageBox.Show("Please Select Qunatity of Isotope", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);     
                    }
                    else
                    {
                        isotopeQuantity = isotopeNumComboBox.SelectedItem.ToString();
                    }

                    #region get form element
                    GetFromElement(ref passToRun);
                    #endregion

                    if (nor.Text == "")
                    {
                        passToRun = false;
                        string buttonID = CustomMessageBox.Show("Number of region cannot empty", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                    }
                    else
                    {
                        numOfRegion = Convert.ToInt32(nor.Text);
                    }
                    if (selBkLocation.SelectedIndex == -1)
                    {
                        passToRun = false;
                        string buttonID = CustomMessageBox.Show("Please select location", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                    
                    }
                    if (alarmComboBox.SelectedIndex == -1)
                    {
                        passToRun = false;
                        string buttonID = CustomMessageBox.Show("Please select alarm type", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                    
                    }
                    if (profileName.Text == "")
                    {
                        passToRun = false;
                        string buttonID = CustomMessageBox.Show("Please Enter Profile Name", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);                   
                    }

                    if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "top" || GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
                    {
                        if (roiPathText1.Text == "")
                        {
                            passToRun = false;
                            string buttonID = CustomMessageBox.Show("Please Select Top Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        }
                        else
                        {
                            if (!File.Exists(roiPathText1.Text))
                            {
                                passToRun = false;
                                string buttonID = CustomMessageBox.Show("Can't find Top Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                            }
                        }

                    }

                    if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "bottom" || GlobalFunc.basicSetting.PresetDetector.ToLower() == "dual")
                    {
                        if (roiPathText2.Text == "")
                        {
                            passToRun = false;
                            string buttonID = CustomMessageBox.Show("Please Select Bottom Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                        }
                        else
                        {
                            if (!File.Exists(roiPathText2.Text))
                            {
                                passToRun = false;
                                string buttonID = CustomMessageBox.Show("Can't find Bottom Roi File", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

                            }
                        }
                    }


                    #region run background
                    if (passToRun)
                    {                        
                        bgOpTimer.Dispose();
                        bgOpTimer = new System.Windows.Forms.Timer();
                        bgOpTimer.Interval = 1000;
                        bgOpTimer.Tick += bgOpTimer_Tick;
                        bgOpTimer.Start();

                        bkWorker = new BackgroundWorker();
                        bkWorker.DoWork += bkWorker_DoWork;
                        bkWorker.ProgressChanged += bkWorker_ProgressChanged;
                        bkWorker.RunWorkerCompleted += bkWorker_RunWorkerCompleted;
                        bkWorker.WorkerReportsProgress = true;//啟動回報進度
                        bkWorker.RunWorkerAsync();
                        bk_pbr.Value = 0;
                        bk_pbr.Maximum = Convert.ToInt32(BKManager.supposeSecond);//ProgressBar上限
                        bk_pbr.Minimum = 0;//ProgressBar下限*/ 
                    }
                    else
                    {
                        this.Enabled = true;
                        nor.Enabled = true;
                        selBkLocation.Enabled = true;
                        alarmComboBox.Enabled = true;
                        isotopeNumComboBox.Enabled = true;
                        detectorComboBox.Enabled = true;

                        resetCalibration();                        
                        LoadProfile();
                        for (int i = 0; i < profileComboBox.Items.Count; i++)
                        {
                            Profile p = (Profile)profileComboBox.Items[i];
                            if (p.ID == selectedProfileID)
                            {
                                profileComboBox.SelectedIndex = i;
                            }
                        }
                        LoadSelectedProfile(false);  
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                this.Enabled = true;
                resetCalibration();
            }
        }

        public void GetFromElement(ref bool passToRun)
        {
            calIsotopeList = new List<CalIsotope>();
            BKManager.halfTime.Clear();
            GlobalFunc.LoadIsotopXML();
            for (int i = 0; i < Convert.ToInt32(isotopeQuantity); i++)
            {
                CalIsotope ci = new CalIsotope();

                ComboBox roiCB = (ComboBox)(panel1.Controls.Find("isotopeName" + i, true)[0]);
                if (roiCB.SelectedIndex == -1)
                {
                    passToRun = false;
                    string buttonID = CustomMessageBox.Show("Please Select #" + i + " isotope", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

                }
                ci.roiName = ((Isotope)roiCB.SelectedItem).Code;

                for (int k = 0; k < GlobalFunc.isotopeList1.Count; k++)
                {
                    if (((Isotope)roiCB.SelectedItem).Code == GlobalFunc.isotopeList1[k].Code)
                    {
                        BKManager.halfTime.Add(Convert.ToDouble(GlobalFunc.isotopeList1[k].HalfLife));
                    }
                }

                TextBox activityTB = (TextBox)(panel1.Controls.Find("Activity" + i, true)[0]);
                if (activityTB.Text == "")
                {
                    passToRun = false;
                    string buttonID = CustomMessageBox.Show("Activity Cannot empty", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                else
                {
                    ci.activity = activityTB.Text;
                    BKManager.activity.Add(Convert.ToDouble(ci.activity));
                }

                TextBox measureTimeTB = (TextBox)(panel1.Controls.Find("MeasureTime" + i, true)[0]);
                if (measureTimeTB.Text == "")
                {
                    passToRun = false;
                    string buttonID = CustomMessageBox.Show("Measure Time Cannot empty", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                else
                {
                    ci.measureTime = measureTimeTB.Text;
                }

                TextBox dILTB = (TextBox)(panel1.Controls.Find("DIL" + i, true)[0]);
                if (dILTB.Text == "")
                {
                    passToRun = false;
                    string buttonID = CustomMessageBox.Show("DIL Cannot empty", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                else
                {
                    ci.dil = dILTB.Text;
                }

                TextBox aLTB = (TextBox)(panel1.Controls.Find("AL" + i, true)[0]);
                if (aLTB.Text == "")
                {
                    passToRun = false;
                    string buttonID = CustomMessageBox.Show("AL Cannot empty", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                else
                {
                    ci.al = aLTB.Text;
                }

                TextBox aLPCTB = (TextBox)(panel1.Controls.Find("AL%" + i, true)[0]);
                if (aLPCTB.Text == "")
                {
                    passToRun = false;
                    string buttonID = CustomMessageBox.Show("AL% Cannot empty", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                }
                else
                {
                    ci.al_pc = aLPCTB.Text;
                }

                DateTimePicker refDate = (DateTimePicker)(panel1.Controls.Find("refDate" + i, true)[0]);
                ci.refDate = refDate.Value;
                string strDate = ci.refDate.Year + "-" + ci.refDate.Month + "-" + ci.refDate.Day + " " + ci.refDate.Hour + ":" + ci.refDate.Minute;
                BKManager.ref_Date.Add(DateTime.Parse(strDate));

                Button btn = (Button)(panel1.Controls.Find("Btn" + i, true)[0]);
                if (btn.Name == "Btn0")
                {
                    btn.Enabled = true;
                }
                calIsotopeList.Add(ci);
            }        
        }

        int toggleCalIndex = 0;
        int bkStatusID = 0;
        int counterRun = 0;
        int numOfRegion = 0;
        protected void individual_Click(object sender, EventArgs e)
        {
            toggleFirstTime = false;
            Button btn = sender as Button;
            CalIsotope ci = new CalIsotope();
            int roiNum = Convert.ToInt32(btn.Name.Replace("Btn", ""));
            toggleCalIndex = roiNum;

            switch (_detector.ToLower())
            {
                case "top": BKManager.beginCalScript = GlobalFunc.topScriptSet.CalibrationBegin; BKManager.endCalScript = GlobalFunc.topScriptSet.CalibrationEnd;
                    BKManager.updateScript = GlobalFunc.topScriptSet.CalibrationUpdate;
                    break;
                case "bottom": BKManager.beginCalScript = GlobalFunc.bottomScriptSet.CalibrationBegin; BKManager.endCalScript = GlobalFunc.bottomScriptSet.CalibrationEnd;
                    BKManager.updateScript = GlobalFunc.bottomScriptSet.CalibrationUpdate;
                    break;
                case "dual": BKManager.beginCalScript = GlobalFunc.dualScriptSet.CalibrationBegin; BKManager.endCalScript = GlobalFunc.dualScriptSet.CalibrationEnd;
                    BKManager.updateScript = GlobalFunc.dualScriptSet.CalibrationUpdate;
                    break;
            } 

            #region get form element
            CalIsotope thisCal = calIsotopeList[roiNum];
            #endregion

            #region run calibration
            bgOpTimer.Dispose();
            bgOpTimer = new System.Windows.Forms.Timer();
            bgOpTimer.Interval = 1000;
            bgOpTimer.Tick += bgOpTimer_Tick;
            bgOpTimer.Start();

            bkWorker = new BackgroundWorker();
            bkWorker.DoWork += bkWorker_DoWork;
            bkWorker.ProgressChanged += bkWorker_ProgressChanged;
            bkWorker.RunWorkerCompleted += bkWorker_RunWorkerCompleted;
            bkWorker.WorkerReportsProgress = true;//啟動回報進度
            bkWorker.RunWorkerAsync();
            bk_pbr.Value = 0;
            bk_pbr.Maximum = Convert.ToInt32(BKManager.supposeSecond) + GlobalFunc.bufferTime;//ProgressBar上限
            bk_pbr.Minimum = 0;//ProgressBar下限*/ 
            #endregion
        }

        System.Windows.Forms.Timer bgOpTimer = new System.Windows.Forms.Timer();
        BackgroundWorker bkWorker = new BackgroundWorker();
        AutoResetEvent resetEvent = new AutoResetEvent(false);
        int countSeconds = 0;
        string toggleDetector = "";
        bool toggleFirstTime = false;
        string isotopeQuantity = "";
        int runningStatus = 0;
        string text = "";
        string pname = "";
        double supposeMillSecond = BKManager.supposeMillSecond;
        double supposeSecond = BKManager.supposeSecond;

        List<string> topDetectorCPSList = new List<string>();
        List<double> doubleTopDetectorCPSList = new List<double>();
        List<string> bottomDetectorCPSList = new List<string>();
        List<double> doubleBottomDetectorCPSList = new List<double>();
        List<DateTime> topReportDateList = new List<DateTime>();
        List<DateTime> bottomReportDateList = new List<DateTime>();
        List<decimal> topLiveTimeList = new List<decimal>();
        List<decimal> bottomLiveTimeList = new List<decimal>();
        protected void bkWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            this.Invoke(new MethodInvoker(delegate { this.Enabled = false; }));
            if (toggleFirstTime)
            {
                calBkBtn.Enabled = false;
                #region set default measure time
                BKManager.jobID = GlobalFunc.logManager.SaveCurrentJob("Background Operation");
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("update time set background = @background", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@background", bkTime.Text);
                cmd.ExecuteNonQuery();

                SqlCeCommand bkCmd = new SqlCeCommand("select background from time", con);
                bkCmd.CommandType = CommandType.Text;
                SqlCeDataReader bkDr = bkCmd.ExecuteReader();
                if (bkDr.Read())
                {
                    BKManager.supposeMillSecond = Convert.ToDouble(bkDr["background"]) * 60 * 1000;
                    BKManager.supposeSecond = Convert.ToDouble(bkDr["background"]) * 60;
                }
                bkDr.Close();
                con.Close();
                #endregion
            }
            else
            {
                BKManager.supposeMillSecond = supposeMillSecond;
                BKManager.supposeSecond = supposeSecond;
            }
            countSeconds = 0;
        
            #region connect
            UpdateStatusLabel("Connecting...", 0, 255, 0);
            bool passDetector = GlobalFunc.CheckDetector(toggleDetector);            
            #endregion

            #region fail to connect sesonsor
            GlobalFunc.ShowDetectorError(toggleDetector);
            #endregion

            if (passDetector)
            {
                if (toggleFirstTime)
                {
                    UpdateStatusLabel("Counting Background...", 0, 255, 0);
                }
                else
                {
                    //UpdateStatusLabel("Prepare Script for " + calIsotopeList[toggleCalIndex].roiName + "...", 0, 255, 0);
                    UpdateStatusLabel("Configuring Detector...", 0, 255, 0);
                }
                BKManager.jobID = GlobalFunc.logManager.SaveCurrentJob("Background Operation");

                #region send begin script
                ExecuteManager exm = new ExecuteManager();
                try
                {
                    if (toggleFirstTime)
                    {
                        GlobalFunc.logManager.WriteLog("Run background operation");
                        BKManager.SetLiveTime();
                        BKManager.SetTemp(Convert.ToInt32(isotopeQuantity), toggleDetector, BKManager.beginBdgScript,
                            roiPathText1.Text, roiPathText2.Text);
                        exm.scriptFilePath = BKManager.tempRunScript;
                        Thread runThread = new Thread(new ThreadStart(exm.RunScript));
                        runThread.Start();
                        bk_pbr.Invoke(new MethodInvoker(delegate { bk_pbr.Value = 0; 
                             bk_pbr.Maximum = Convert.ToInt32(BKManager.supposeSecond) + GlobalFunc.bufferTime;
                            clockLabel.Visible = true; }));
                        for (int i = 1; i <= BKManager.supposeSecond + GlobalFunc.bufferTime; i++)
                        {
                            bkWorker.ReportProgress(i, i);
                            Thread.Sleep(1000);
                        }
                        if (runThread.IsAlive)
                        {
                            runThread.Abort();
                        }
                    }
                    else if (calIsotopeList[toggleCalIndex].runCounter == 0)
                    {
                        GlobalFunc.logManager.WriteLog("Run Calibration operation");
                        BKManager.supposeSecond = Convert.ToDouble(calIsotopeList[toggleCalIndex].measureTime) * 60;
                        BKManager.SetTemp(Convert.ToInt32(isotopeQuantity), toggleDetector, BKManager.beginCalScript,
                        roiPathText1.Text, roiPathText2.Text);  
                        exm.scriptFilePath = BKManager.tempRunScript;
                        Thread runThread = new Thread(new ThreadStart(exm.RunScript));
                        runThread.Start();
                        bk_pbr.Invoke(new MethodInvoker(delegate {
                            bk_pbr.Value = 0;
                            //bk_pbr.Maximum = Convert.ToInt32(BKManager.supposeSecond);
                            bk_pbr.Maximum = 6;
                            clockLabel.Visible = true;
                        }));
                        for (int i = 1; i <= 6; i++)
                        {
                            bkWorker.ReportProgress(i, i);
                            Thread.Sleep(1000);
                        }
                        if (runThread.IsAlive)
                        {
                            runThread.Abort();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                #endregion

                #region send update script
                Thread.Sleep(4000);
                int countCheck = 0;
                if (!toggleFirstTime)
                {
                    countSeconds = 0;
                    //check active
                    UpdateStatusLabel("Checking detector status...", 0, 255, 0);
                    while (GlobalFunc.checkActive(toggleDetector))
                    {
                        Thread.Sleep(1000);
                        countCheck++;
                        if (countCheck == 20)
                        {
                            break;
                        }
                    }
                    UpdateStatusLabel("Updating...", 0, 255, 0);
                    BKManager.supposeSecond = Convert.ToDouble(calIsotopeList[toggleCalIndex].measureTime) * 60;
                    BKManager.SetTemp(Convert.ToInt32(isotopeQuantity), toggleDetector, BKManager.updateScript,
                        roiPathText1.Text, roiPathText2.Text);                  
                    exm = new ExecuteManager();
                    exm.scriptFilePath =  BKManager.tempRunScript;
                    Thread updateThread = new Thread(new ThreadStart(exm.RunScript));
                    updateThread.Start();

                    bk_pbr.Invoke(new MethodInvoker(delegate { bk_pbr.Value = 0;
                    bk_pbr.Maximum = Convert.ToInt32(BKManager.supposeSecond) + GlobalFunc.bufferTime; 
                        clockLabel.Visible = false;
                    }));
                    for (int i = 0; i < Convert.ToInt32(BKManager.supposeSecond) + GlobalFunc.bufferTime; i++)
                    {
                        Thread.Sleep(1200);
                        bkWorker.ReportProgress(i,i);
                    }

                    if (updateThread.IsAlive)
                    {
                        updateThread.Abort();
                    }
                    countCheck = 0;
                    while (GlobalFunc.checkActive(toggleDetector))
                    {
                        Thread.Sleep(3000);
                        countCheck++;
                        if (countCheck == 20)
                        {
                            break;
                        }
                    }
                }
                #endregion

                #region send end script
                if (toggleFirstTime)
                {
                    UpdateStatusLabel("Checking detector status...", 0, 255, 0);
                    //check active
                    countCheck = 0;
                    while (GlobalFunc.checkActive(toggleDetector))
                    {
                        countCheck++;
                        if (countCheck == 20)
                        {
                            break;
                        }
                        Thread.Sleep(1000);
                    }
                    UpdateStatusLabel("Updating...", 0, 255, 0);
                    countSeconds = 0;
                    bk_pbr.Invoke(new MethodInvoker(delegate { bk_pbr.Value = 0;
                    bk_pbr.Maximum = GlobalFunc.bufferTime; clockLabel.Visible = false;
                    }));
                    exm = new ExecuteManager();
                    BKManager.SetTemp(Convert.ToInt32(isotopeQuantity), toggleDetector, BKManager.endBdgScript,
                        roiPathText1.Text, roiPathText2.Text);
                    exm.scriptFilePath = BKManager.tempRunScript;
                    Thread endThread = new Thread(new ThreadStart(exm.RunScript));                    
                    endThread.Start();
                    for (int i = 1; i <= GlobalFunc.bufferTime; i++)
                    {
                        bkWorker.ReportProgress(i, i);
                        Thread.Sleep(1000);
                    }
                    if (endThread.IsAlive)
                    {
                        endThread.Abort();
                    }
                    UpdateStatusLabel("Finish", 0, 255, 0);
                }
                #endregion
                
                if (toggleFirstTime)
                {
                    #region set cps list
                    //check active
                    countCheck = 0;
                    while (GlobalFunc.checkActive(toggleDetector))
                    {
                        Thread.Sleep(1000);
                        countCheck++;
                        if (countCheck == 20)
                        {
                            break;
                        }
                    }
                    if (toggleDetector.Contains("Top"))
                    {
                        if (File.Exists(GlobalFunc.topScriptSet.BackgrounData))
                        {
                            List<Roi> roiList1 = GlobalFunc.GetRoiData(GlobalFunc.topScriptSet.BackgrounData);
                            if (roiList1.Count > 0)
                            {
                                topDetectorCPSList = BKManager.CalBk(roiList1, ref BKManager.dt, ref BKManager.lifeTime, 1);
                                debugBox.Invoke(new MethodInvoker(delegate { debugBox.AppendText("TOP CPS Count:" + topDetectorCPSList.Count.ToString() + System.Environment.NewLine); }));
                            }
                        }
                    }
                    else if (toggleDetector.Contains("Bottom"))
                    {
                        List<Roi> roiList2 = GlobalFunc.GetRoiData(GlobalFunc.bottomScriptSet.BackgrounData);
                        if (roiList2.Count > 0)
                        {
                            bottomDetectorCPSList = BKManager.CalBk(roiList2, ref BKManager.dt, ref BKManager.lifeTime, 1);
                            debugBox.Invoke(new MethodInvoker(delegate { debugBox.AppendText("Bottom CPS Count:" + bottomDetectorCPSList.Count.ToString() + System.Environment.NewLine); }));
                        }
                    }
                    else if (toggleDetector.Contains("Dual"))
                    {
                        List<Roi> roiList1 = GlobalFunc.GetRoiData(GlobalFunc.topScriptSet.BackgrounData);
                        if (roiList1.Count > 0)
                        {
                            topDetectorCPSList = BKManager.CalBk(roiList1, ref BKManager.dt, ref BKManager.lifeTime, 1);
                            debugBox.Invoke(new MethodInvoker(delegate { debugBox.AppendText("TOP CPS Count:" + topDetectorCPSList.Count.ToString() + System.Environment.NewLine); }));
                        }
                        List<Roi> roiList2 = GlobalFunc.GetRoiData(GlobalFunc.bottomScriptSet.BackgrounData);
                        if (roiList2.Count > 0)
                        {
                            bottomDetectorCPSList = BKManager.CalBk(roiList2, ref BKManager.dt, ref BKManager.lifeTime, 1);
                            debugBox.Invoke(new MethodInvoker(delegate { debugBox.AppendText("Bottom CPS Count:" + bottomDetectorCPSList.Count.ToString() + System.Environment.NewLine); }));
                        }
                    }
                    #endregion

                    #region get bk data
                    GlobalFunc.SetBackgroundBKList(toggleDetector);//get the data and calulatue cps

                    string totalCPS = BKManager.bkList[BKManager.bkList.Count - 1].ToString();

                    GlobalFunc.logManager.WriteLog("Calibration background operation: totalCPS = " + totalCPS);

                    string runDate = BKManager.dt.ToString();
                    string countTime = BKManager.lifeTime.ToString();

                    GlobalFunc.logManager.WriteLog("Calibration background operation: runDate = " + runDate);
                    GlobalFunc.logManager.WriteLog("Calibration background operation: liveTime = " + countTime + " seconds");

                    GlobalFunc.loadProfile.LiveTime = countTime;
                    GlobalFunc.loadProfile.Date = runDate;
                    BKManager.bkList.RemoveAt(BKManager.bkList.Count - 1);
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                    con.Open();

                    SqlCeCommand insertCmd = new SqlCeCommand("insert into bkStatus (date, location, totalCount, countTime) values (@date, @location, @totalCount, @countTime)", con);
                    insertCmd.CommandType = CommandType.Text;
                    insertCmd.Parameters.AddWithValue("@date", runDate);
                    insertCmd.Parameters.AddWithValue("@location", "A");
                    insertCmd.Parameters.AddWithValue("@totalCount", totalCPS.Replace("CPS", "").Trim());
                    insertCmd.Parameters.AddWithValue("@countTime", countTime.Replace("Seconds", "").Trim());
                    insertCmd.ExecuteNonQuery();
                    insertCmd.Dispose();

                    SqlCeCommand idCmd = new SqlCeCommand("select top 1 id_key from bkStatus order by id_key desc", con);
                    idCmd.CommandType = CommandType.Text;
                    bkStatusID = Convert.ToInt32(idCmd.ExecuteScalar());
                    idCmd.Dispose();

                    for (int i = 0; i < calIsotopeList.Count; i++)
                    {
                        SqlCeCommand insertDetailCmd = new SqlCeCommand(@"insert bkStatusDetail (name, displayOrder, bkStatusID, countRate) 
                                                        values (@name, @displayOrder, @bkStatusID, @countRate)", con);
                        insertDetailCmd.CommandType = CommandType.Text;
                        insertDetailCmd.Parameters.AddWithValue("@name", calIsotopeList[i].roiName);
                        insertDetailCmd.Parameters.AddWithValue("@displayOrder", (i + 1));
                        insertDetailCmd.Parameters.AddWithValue("@bkStatusID", bkStatusID);

                        switch (i)
                        {
                            case 0: insertDetailCmd.Parameters.AddWithValue("@countRate", BKManager.bkList[i].Replace("CPS", "").Trim());
                                break;
                            case 1: insertDetailCmd.Parameters.AddWithValue("@countRate", BKManager.bkList[i].Replace("CPS", "").Trim());
                                break;
                            case 2: insertDetailCmd.Parameters.AddWithValue("@countRate", BKManager.bkList[i].Replace("CPS", "").Trim());
                                break;
                            case 3: insertDetailCmd.Parameters.AddWithValue("@countRate", BKManager.bkList[i].Replace("CPS", "").Trim());
                                break;
                            case 4: insertDetailCmd.Parameters.AddWithValue("@countRate", BKManager.bkList[i].Replace("CPS", "").Trim());
                                break;
                            case 5: insertDetailCmd.Parameters.AddWithValue("@countRate", BKManager.bkList[i].Replace("CPS", "").Trim());
                                break;
                            default: insertDetailCmd.Parameters.AddWithValue("@countRate", "");
                                break;
                        }
                        insertDetailCmd.ExecuteNonQuery();
                        insertDetailCmd.Dispose();
                        GlobalFunc.logManager.WriteLog("Calibration background operation: " + calIsotopeList[i].roiName + " count rate: " + BKManager.bkList[i].Replace("CPS", "").Trim() + " CPS");
                    }
                    con.Close();
                    #endregion
                }
                else
                {
                    #region last isotop step
                    if (calIsotopeList[toggleCalIndex].runCounter + 1 == numOfRegion)
                    {
                        //check active
                        countCheck = 0;
                        while (GlobalFunc.checkActive(toggleDetector))
                        {
                            Thread.Sleep(1000);
                            countCheck++;
                            if (countCheck == 20)
                            {
                                break;
                            }
                        }
                        UpdateStatusLabel("Data Analyzing...", 0, 255, 0);
                        countSeconds = 0;
                        bk_pbr.Invoke(new MethodInvoker(delegate { bk_pbr.Value = 0; bk_pbr.Maximum = GlobalFunc.bufferTime; clockLabel.Visible = false; }));
                        exm = new ExecuteManager();
                        BKManager.SetTemp(Convert.ToInt32(isotopeQuantity), toggleDetector, BKManager.endCalScript,
                           roiPathText1.Text, roiPathText2.Text);
                        exm = new ExecuteManager();
                        exm.scriptFilePath = BKManager.tempRunScript;
                        Thread endThread = new Thread(new ThreadStart(exm.RunScript));
                        endThread.Start();
                        for (int i = 0; i < GlobalFunc.bufferTime; i++)
                        {
                            bkWorker.ReportProgress(i, i);
                            Thread.Sleep(1000);
                        }
                        countCheck = 0;
                        while (GlobalFunc.checkActive(toggleDetector))
                        {
                            Thread.Sleep(1000);
                            countCheck++;
                            if (countCheck == 20)
                            {
                                break;
                            }
                        }
                       
                        if (endThread.IsAlive)
                        {
                            endThread.Abort();
                        }
                       
                        List<string> thisTopCPSList = new List<string>();
                        List<string> thisBottomCPSList = new List<string>();
                        DateTime topReportDate = new DateTime();
                        DateTime bottomReportDate = new DateTime();
                        decimal topLiveTime = 0;
                        decimal bottomLiveTime = 0;
                        if (toggleDetector.Contains("Top"))
                        {
                            if (File.Exists(GlobalFunc.topScriptSet.BackgrounData))
                            {
                                List<Roi> roiList1 = GlobalFunc.GetRoiData(GlobalFunc.topScriptSet.CalibrationData);
                                if (roiList1.Count > 0)
                                {
                                    thisTopCPSList = BKManager.CalBk(roiList1, ref topReportDate, ref topLiveTime, numOfRegion);
                                    debugBox.Invoke(new MethodInvoker(delegate { debugBox.AppendText("Isotope " + toggleCalIndex + " Top CPS Count:" + thisTopCPSList.Count.ToString() + System.Environment.NewLine); }));
                                }
                            }
                            topReportDateList.Add(topReportDate);
                            topLiveTimeList.Add(topLiveTime);
                            thisTopCPSList.RemoveAt(thisTopCPSList.Count - 1);
                            for (int i = 0; i < thisTopCPSList.Count; i++)
                            {
                                calIsotopeList[toggleCalIndex].topCpsList.Add(Convert.ToDouble(thisTopCPSList[i].Replace("CPS", "").Trim()));
                                GlobalFunc.logManager.WriteLog("Calibration each isotope operation (top detector): " + calIsotopeList[toggleCalIndex].roiName +
                                            " count rate: " + Convert.ToDouble(thisTopCPSList[i].Replace("CPS", "").Trim()) + " CPS");
                            }
                        }
                        else if (toggleDetector.Contains("Bottom"))
                        {
                            List<Roi> roiList2 = GlobalFunc.GetRoiData(GlobalFunc.bottomScriptSet.CalibrationData);
                            if (roiList2.Count > 0)
                            {
                                thisBottomCPSList = BKManager.CalBk(roiList2, ref bottomReportDate, ref bottomLiveTime, numOfRegion);
                                debugBox.Invoke(new MethodInvoker(delegate { debugBox.AppendText("Isotope " + toggleCalIndex + " Bottom CPS Count:" + thisBottomCPSList.Count.ToString() + System.Environment.NewLine); }));
                            }
                            bottomReportDateList.Add(bottomReportDate);
                            bottomLiveTimeList.Add(bottomLiveTime);
                            thisBottomCPSList.RemoveAt(thisBottomCPSList.Count - 1);
                            for (int i = 0; i < thisBottomCPSList.Count; i++)
                            {
                                calIsotopeList[toggleCalIndex].bottomCpsList.Add(Convert.ToDouble(thisBottomCPSList[i].Replace("CPS", "").Trim()));
                                GlobalFunc.logManager.WriteLog("Calibration each isotope operation (bottom detector): " + calIsotopeList[toggleCalIndex].roiName +
                                            " count rate: " + Convert.ToDouble(thisBottomCPSList[i].Replace("CPS", "").Trim()) + " CPS");
                            }
                        }
                        else if (toggleDetector.Contains("Dual"))
                        {
                            if (File.Exists(GlobalFunc.topScriptSet.BackgrounData))
                            {
                                List<Roi> roiList1 = GlobalFunc.GetRoiData(GlobalFunc.topScriptSet.CalibrationData);
                                if (roiList1.Count > 0)
                                {
                                    thisTopCPSList = BKManager.CalBk(roiList1, ref topReportDate, ref topLiveTime, numOfRegion);
                                    debugBox.Invoke(new MethodInvoker(delegate { debugBox.AppendText("Isotope " + toggleCalIndex + " Top CPS Count:" + thisTopCPSList.Count.ToString() + System.Environment.NewLine); }));
                                }
                                List<Roi> roiList2 = GlobalFunc.GetRoiData(GlobalFunc.bottomScriptSet.CalibrationData);
                                if (roiList2.Count > 0)
                                {
                                    thisBottomCPSList = BKManager.CalBk(roiList2, ref bottomReportDate, ref bottomLiveTime, numOfRegion);
                                    debugBox.Invoke(new MethodInvoker(delegate { debugBox.AppendText("Isotope " + toggleCalIndex + " Bottom CPS Count:" + thisBottomCPSList.Count.ToString() + System.Environment.NewLine); }));
                                }
                            }
                            topReportDateList.Add(topReportDate);
                            bottomReportDateList.Add(topReportDate);
                            thisTopCPSList.RemoveAt(thisTopCPSList.Count - 1);
                            thisBottomCPSList.RemoveAt(thisBottomCPSList.Count - 1);
                            for (int i = 0; i < thisTopCPSList.Count; i++)
                            {
                                calIsotopeList[toggleCalIndex].topCpsList.Add(Convert.ToDouble(thisTopCPSList[i].Replace("CPS", "").Trim()));
                                GlobalFunc.logManager.WriteLog("Calibration each isotope operation (top detector): " + calIsotopeList[toggleCalIndex].roiName +
                                            " count rate: " + Convert.ToDouble(thisTopCPSList[i].Replace("CPS", "").Trim()) + " CPS");
                            }
                            for (int i = 0; i < thisBottomCPSList.Count; i++)
                            {
                                calIsotopeList[toggleCalIndex].bottomCpsList.Add(Convert.ToDouble(thisBottomCPSList[i].Replace("CPS", "").Trim()));
                                GlobalFunc.logManager.WriteLog("Calibration each isotope operation (bottom detector): " + calIsotopeList[toggleCalIndex].roiName +
                                            " count rate: " + Convert.ToDouble(thisBottomCPSList[i].Replace("CPS", "").Trim()) + " CPS");
                            }
                        }
                        UpdateStatusLabel("Standby", 255, 255, 0);
                    }
                    #endregion
                }

                bk_pbr.Invoke(new MethodInvoker(delegate { bk_pbr.Value = 0; }));
                countSeconds = 0;
            }
            else
            {
                detectorComboBox.Enabled = true;
                nor.Enabled = true;
                selBkLocation.Enabled = true;
                alarmComboBox.Enabled = true;
                isotopeNumComboBox.Enabled = true;
                this.Enabled = true;
            }
            GlobalFunc.ResetConnectDetector();
            UpdateStatusLabel("Standby", 255, 255, 0);
            if (toggleFirstTime)
            {
                GlobalFunc.logManager.UpdateJobStatus(BKManager.jobID);
            }
        }

        protected void bkWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            bk_pbr.Value = e.ProgressPercentage;
        }

        protected void bkWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            bgOpTimer.Stop();
            clockLabel.Text = "";
            bk_pbr.Value = 0;
            runningStatus = 1;
            this.Enabled = true;
            Thread.Sleep(1000);
            abortBtn.Enabled = true;
            if (!toggleFirstTime)
            {
                calIsotopeList[toggleCalIndex].runCounter += 1;
                string runName = "runCounterLabel" + toggleCalIndex;
                string name = "pb" + toggleCalIndex;
                string nextBtn = "Btn" + (toggleCalIndex + 1);
                foreach (Control c in panel1.Controls)
                {
                    if (c.Name == name)
                    {
                        if (calIsotopeList[toggleCalIndex].runCounter == numOfRegion)
                        {
                            c.Visible = true;
                        }
                    }
                    else if (c.Name == runName)
                    {
                        Label l = c as Label;
                        l.Text = calIsotopeList[toggleCalIndex].runCounter.ToString();
                    }
                    else if (c.Name == nextBtn)
                    {
                        if (calIsotopeList[toggleCalIndex].runCounter == numOfRegion)
                        {                         
                            Button btn = c as Button;
                            btn.Enabled = true;
                        }
                    }
                    else if (c.Name == "Btn" + toggleCalIndex)
                    {
                        if (calIsotopeList[toggleCalIndex].runCounter == numOfRegion)
                        {
                            Button thisBtn = (Button)(panel1.Controls.Find("Btn" + toggleCalIndex, true)[0]);
                            thisBtn.Enabled = false;                          
                        }
                    }
                }
            }     
            int totalTopCPSCount = 0;
            int totalBottomCPSCount = 0;

            for (int i = 0; i < calIsotopeList.Count; i++)
            {
                totalTopCPSCount += calIsotopeList[i].topCpsList.Count;
                totalBottomCPSCount += calIsotopeList[i].bottomCpsList.Count;     
            }

            if (detectorComboBox.SelectedIndex == 0)
            {
                if (totalTopCPSCount == (Convert.ToInt32(isotopeQuantity) * Convert.ToInt32(isotopeQuantity)))
                {
                    calibrationBtn.Enabled = true;
                    SaveAfterCalibration();
                }
            }
            else if (detectorComboBox.SelectedIndex == 1)
            {
                if (totalBottomCPSCount == (Convert.ToInt32(isotopeQuantity) * Convert.ToInt32(isotopeQuantity)))
                {
                    calibrationBtn.Enabled = true;
                    SaveAfterCalibration();
                }           
            }
            else if (detectorComboBox.SelectedIndex == 2)
            if (totalTopCPSCount == (Convert.ToInt32(isotopeQuantity) * Convert.ToInt32(isotopeQuantity)) &&
                totalBottomCPSCount == (Convert.ToInt32(isotopeQuantity) * Convert.ToInt32(isotopeQuantity)))
            {
                calibrationBtn.Enabled = true;              
                SaveAfterCalibration();
            }

            GlobalFunc.ResetConnectDetector();
        }

        private void bgOpTimer_Tick(object sender, EventArgs e)
        {
            countSeconds++;
            clockLabel.Text = countSeconds + " seconds";
        }
        #endregion

        public void SaveAfterCalibration()
        {
            if (GlobalFunc.loadProfile.ProfileName != "")
            {
                GlobalFunc.loadProfile.ProfileName = profileName.Text;
                GlobalFunc.loadProfile.FileName = GlobalFunc.loadProfile.ProfileName + ".txt";
            }
            else
            {
                GlobalFunc.loadProfile.ProfileName = profileName.Text;
                if (detectorComboBox.SelectedIndex == 0)
                {
                    GlobalFunc.loadProfile.Detector = "Top";
                }
                else if (detectorComboBox.SelectedIndex == 1)
                {
                    GlobalFunc.loadProfile.Detector = "Bottom";
                }
                else if (detectorComboBox.SelectedIndex == 2)
                {
                    GlobalFunc.loadProfile.Detector = "Dual";
                }
                if (isotopeNumComboBox.SelectedIndex == 0)
                {
                    GlobalFunc.loadProfile.Qty = 4;
                }
                else if (isotopeNumComboBox.SelectedIndex == 1)
                {
                    GlobalFunc.loadProfile.Qty = 5;
                }
                else if (isotopeNumComboBox.SelectedIndex == 2)
                {
                    GlobalFunc.loadProfile.Qty = 6;
                }
                GlobalFunc.loadProfile.FileName = GlobalFunc.loadProfile.ProfileName + ".txt";
                GlobalFunc.loadProfile.Location = ((Location)selBkLocation.SelectedItem).Description;
                GlobalFunc.loadProfile.Alarm = alarmComboBox.SelectedItem.ToString();
                GlobalFunc.loadProfile.NoOfRegion = Convert.ToInt32(nor.Text);
                GlobalFunc.loadProfile.RoiPath1 = roiPathText1.Text;
                GlobalFunc.loadProfile.RoiPath2 = roiPathText2.Text;
                calBkBtn.Enabled = true;
            }

            #region save File
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Profile Name:" + GlobalFunc.loadProfile.ProfileName);
            sb.AppendLine("Detector:" + GlobalFunc.loadProfile.Detector);
            sb.AppendLine("Location:" + GlobalFunc.loadProfile.Location);
            sb.AppendLine("Alarm Type:" + GlobalFunc.loadProfile.Alarm);
            sb.AppendLine("Quantity of Isotope:" + GlobalFunc.loadProfile.Qty);
            sb.AppendLine("Live Time:" + GlobalFunc.loadProfile.LiveTime);
            sb.AppendLine("Date:" + GlobalFunc.loadProfile.Date);
            sb.AppendLine("Roi Path1:" + GlobalFunc.loadProfile.RoiPath1);
            sb.AppendLine("Roi Path2:" + GlobalFunc.loadProfile.RoiPath2);
            sb.AppendLine("No. of Region:" + GlobalFunc.loadProfile.NoOfRegion);
            sb.Append("Background Top CPS:");
            for (int i = 0; i < topDetectorCPSList.Count; i++)
            {
                doubleTopDetectorCPSList.Add(Convert.ToDouble(topDetectorCPSList[i].Replace(" CPS", "")));
                if (i == 0)
                {
                    sb.Append(topDetectorCPSList[i].Replace(" CPS", ""));
                }
                else
                {
                    sb.Append("," + topDetectorCPSList[i].Replace(" CPS", ""));
                }
            }
            sb.AppendLine("");
            sb.Append("Background Bottom CPS:");
            for (int i = 0; i < bottomDetectorCPSList.Count; i++)
            {
                doubleBottomDetectorCPSList.Add(Convert.ToDouble(bottomDetectorCPSList[i].Replace(" CPS", "")));
                if (i == 0)
                {
                    sb.Append(bottomDetectorCPSList[i].Replace(" CPS", ""));
                }
                else
                {
                    sb.Append("," + bottomDetectorCPSList[i].Replace(" CPS", ""));
                }
            }
            sb.AppendLine(System.Environment.NewLine);
            sb.AppendLine("Isotope Sequence:");
            List<double> topNulCPS = new List<double>();
            List<double> bottomNulCPS = new List<double>();
            for (int i = 0; i < calIsotopeList.Count; i++)
            {
                sb.AppendLine("Roi#" + (i + 1) + ":" + calIsotopeList[i].roiName);
                sb.AppendLine("Activity:" + Convert.ToDouble(calIsotopeList[i].activity) * 1000);
                sb.AppendLine("Ref Datetime:" + calIsotopeList[i].refDate.Year + "-" + calIsotopeList[i].refDate.Month + "-" + calIsotopeList[i].refDate.Day + " " +
                                                            calIsotopeList[i].refDate.Hour + ":" + calIsotopeList[i].refDate.Minute);
                sb.AppendLine("Measure Time:" + calIsotopeList[i].measureTime);
                sb.AppendLine("DIL:" + calIsotopeList[i].dil);
                sb.AppendLine("AL:" + calIsotopeList[i].al);
                sb.AppendLine("AL%:" + calIsotopeList[i].al_pc);
                sb.Append("Top CPS:");
                for (int j = 0; j < calIsotopeList[i].topCpsList.Count; j++)
                {
                    topNulCPS.Add(calIsotopeList[i].topCpsList[j]);
                    if (j == 0)
                    {
                        sb.Append(calIsotopeList[i].topCpsList[j]);
                    }
                    else
                    {
                        sb.Append("," + calIsotopeList[i].topCpsList[j]);
                    }
                }
                sb.AppendLine("");
                sb.Append("Bottom CPS:");
                for (int j = 0; j < calIsotopeList[i].bottomCpsList.Count; j++)
                {
                    bottomNulCPS.Add(calIsotopeList[i].bottomCpsList[j]);
                    if (j == 0)
                    {
                        sb.Append(calIsotopeList[i].bottomCpsList[j]);
                    }
                    else
                    {
                        sb.Append("," + calIsotopeList[i].bottomCpsList[j]);
                    }
                }
                sb.AppendLine(System.Environment.NewLine);
            }

            sb.AppendLine("Matrix Top:");
            List<float> topResult = new List<float>();
            List<float> bottomResult = new List<float>();
            List<float> dualResult = new List<float>();
            if (toggleDetector.Contains("Top") || toggleDetector.Contains("Dual"))
            {
                doubleTopDetectorCPSList.RemoveAt(doubleTopDetectorCPSList.Count - 1);

                topResult = GlobalFunc.cal.Calibration_MatrixS(doubleTopDetectorCPSList,
                    topNulCPS, BKManager.activity, BKManager.halfTime, BKManager.ref_Date,
                    topReportDateList);
                debugBox.AppendText("doubleTopDetectorCPSList:" + doubleTopDetectorCPSList.Count.ToString() + System.Environment.NewLine);
                debugBox.AppendText("topNulCPS:" + topNulCPS.Count.ToString() + System.Environment.NewLine);
                debugBox.AppendText("BKManager.activity:" + BKManager.activity.Count.ToString() + System.Environment.NewLine);
                debugBox.AppendText("BKManager.halfTime:" + BKManager.halfTime.Count.ToString() + System.Environment.NewLine);
                debugBox.AppendText("BKManager.ref_Date:" + BKManager.ref_Date.Count.ToString() + System.Environment.NewLine);
                debugBox.AppendText("topReportDateList:" + topReportDateList.Count.ToString() + System.Environment.NewLine);

                string textResult = "";
                for (int i = 0; i < topResult.Count; i++)
                {
                    textResult += topResult[i] + ",";
                    if (i % GlobalFunc.loadProfile.Qty == GlobalFunc.loadProfile.Qty - 1 && i > 1)
                    {
                        textResult += System.Environment.NewLine;
                    }
                }
                sb.AppendLine(textResult);
            }
            sb.AppendLine("Matrix Bottom:");
            if (toggleDetector.Contains("Bottom") || toggleDetector.Contains("Dual"))
            {
                doubleBottomDetectorCPSList.RemoveAt(doubleBottomDetectorCPSList.Count - 1);

                bottomResult = GlobalFunc.cal.Calibration_MatrixS(doubleBottomDetectorCPSList, bottomNulCPS,
                    BKManager.activity, BKManager.halfTime, BKManager.ref_Date, bottomReportDateList);

                debugBox.AppendText("doubleBottomDetectorCPSList:" + doubleBottomDetectorCPSList.Count.ToString() + System.Environment.NewLine);
                debugBox.AppendText("bottomNulCPS:" + bottomNulCPS.Count.ToString() + System.Environment.NewLine);
                debugBox.AppendText("BKManager.activity:" + BKManager.activity.Count.ToString() + System.Environment.NewLine);
                debugBox.AppendText("BKManager.halfTime:" + BKManager.halfTime.Count.ToString() + System.Environment.NewLine);
                debugBox.AppendText("BKManager.ref_Date:" + BKManager.ref_Date.Count.ToString() + System.Environment.NewLine);
                debugBox.AppendText("bottomReportDateList:" + bottomReportDateList.Count.ToString() + System.Environment.NewLine);

                string textResult = "";
                for (int i = 0; i < bottomResult.Count; i++)
                {
                    textResult += bottomResult[i] + ",";
                    if (i % GlobalFunc.loadProfile.Qty == GlobalFunc.loadProfile.Qty - 1 && i > 1)
                    {
                        textResult += System.Environment.NewLine;
                    }
                }
                sb.AppendLine(textResult);
            }
            sb.AppendLine("Matrix Dual:");
            if (toggleDetector.Contains("Dual"))
            {
                string debugText = "Top Detector CPS List Count " + doubleTopDetectorCPSList.Count + System.Environment.NewLine;
                debugText += "topNulCPS " + topNulCPS.Count + System.Environment.NewLine;
                debugText += "BKManager.activity " + BKManager.activity.Count + System.Environment.NewLine;
                debugText += "BKManager.halfTime " + BKManager.halfTime.Count + System.Environment.NewLine;
                debugText += "BKManager.ref_Date " + BKManager.ref_Date.Count + System.Environment.NewLine;
                debugText += "topReportDateList " + topReportDateList.Count + System.Environment.NewLine;

                debugText += "Bottom Detector CPS List Count " + doubleBottomDetectorCPSList.Count + System.Environment.NewLine;
                debugText += "BottomNulCPS " + bottomNulCPS.Count + System.Environment.NewLine;
                debugText += "BKManager.activity " + BKManager.activity.Count + System.Environment.NewLine;
                debugText += "BKManager.halfTime " + BKManager.halfTime.Count + System.Environment.NewLine;
                debugText += "BKManager.ref_Date " + BKManager.ref_Date.Count + System.Environment.NewLine;
                debugText += "bottomReportDateList " + bottomReportDateList.Count + System.Environment.NewLine;
                //string debugBtnClicked = CustomMessageBox.Show(debugText, "Debug", GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

                dualResult = GlobalFunc.cal.Calibration_MatrixD(doubleTopDetectorCPSList,
                    doubleBottomDetectorCPSList, topNulCPS,
                    bottomNulCPS, BKManager.activity, BKManager.halfTime, BKManager.ref_Date,
                    BKManager.ref_Date, topReportDateList, bottomReportDateList);
                string textResult = "";
                for (int i = 0; i < dualResult.Count; i++)
                {
                    textResult += dualResult[i] + ",";
                    if (i % GlobalFunc.loadProfile.Qty == GlobalFunc.loadProfile.Qty - 1 && i > 1)
                    {
                        textResult += System.Environment.NewLine;
                    }
                }
                sb.AppendLine(textResult);
                //sb.AppendLine("DualMatrixCount " + dualResult.Count);
            }
            text = sb.ToString();
            if (GlobalFunc.loadProfile.FileName.Replace(" ", "_") == "")
            {
                GlobalFunc.loadProfile.FileName = GlobalFunc.loadProfile.ProfileName.Replace(" ", "_") + ".txt";
            }
            try
            {
                if (File.Exists(@"C:\LCMS\Profile\" + GlobalFunc.loadProfile.FileName.Replace(" ", "_")))
                {
                    File.Delete(@"C:\LCMS\Profile\" + GlobalFunc.loadProfile.FileName.Replace(" ", "_"));
                }
            }
            catch (Exception ex)
            {

            }
            File.WriteAllText(@"C:\LCMS\Profile\" + GlobalFunc.loadProfile.FileName.Replace(" ", "_").Replace(".txt", "") + ".txt", text);
            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand checkCmd = new SqlCeCommand("select * from profileName where profileName = @profileName", con);
            checkCmd.CommandType = CommandType.Text;
            checkCmd.Parameters.AddWithValue("@profileName", GlobalFunc.loadProfile.ProfileName);
            SqlCeDataReader checkDr = checkCmd.ExecuteReader();
            bool insert = true;
            if (checkDr.Read())
            {
                insert = false;
            }
            checkDr.Close();
            if (insert)
            {
                SqlCeCommand cmd = new SqlCeCommand("insert into profileName (profileName, fileName) values (@profileName, @fileName);", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@profileName", GlobalFunc.loadProfile.ProfileName);
                cmd.Parameters.AddWithValue("@fileName", GlobalFunc.loadProfile.FileName.Replace(" ", "_") + ".txt");
                cmd.ExecuteNonQuery();
            }
            con.Close();
            try
            {
                GlobalFunc.mainForm.LoadProfile();
            }
            catch { }
            resetCalibration();
            #endregion       
        }

        private void calibrationBtn_Click(object sender, EventArgs e)
        {
            SaveAfterCalibration();
        }

        private void unSelectBtn_Click(object sender, EventArgs e)
        {
            resetCalibration();
        }

        public void resetCalibration()
        {
            LoadProfile();
            GlobalFunc.loadProfile = new Profile();
            UpdateStatusLabel("Standby", 255, 255, 0);
            saveProfileBtn.Enabled = false;
            roiPathText1.Text = "";
            roiPathText2.Text = "";
            //GlobalFunc.mainForm.LoadProfile();
            GlobalFunc.LoadIsotopXML();
            dummyIsoSeqList.Clear();
            doubleTopDetectorCPSList.Clear();
            doubleBottomDetectorCPSList.Clear();
            calIsotopeList.Clear();
            panel1.Controls.Clear();
            BKManager.activity.Clear();
            BKManager.halfTime.Clear();
            BKManager.ref_Date.Clear();
            BKManager.bkList.Clear();

            topReportDateList.Clear();
            bottomReportDateList.Clear();
            doubleTopDetectorCPSList.Clear();
            doubleBottomDetectorCPSList.Clear();
     
            isotopeNumComboBox.Enabled = true;
            isotopeNumComboBox.SelectedIndex = -1;
            detectorComboBox.SelectedIndex = -1;
            alarmComboBox.SelectedIndex = -1;
            nor.Text = "1";
            selBkLocation.SelectedIndex = -1;
            profileName.Text = "";
            nor.Enabled = true;
            detectorComboBox.Enabled = true;
            selBkLocation.Enabled = true;
            alarmComboBox.Enabled = true;
            calBkBtn.Enabled = true;
        }

        #region isotope setting
        private void isotopeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (isotopeList.SelectedIndex > -1)
            {
                isoAdd.Enabled = false;
                isoUpdate.Enabled = true;
                isoDelete.Enabled = true;
                isoCancel.Enabled = true;
                Isotope isotope = (Isotope)isotopeList.SelectedItem;
                for (int i = 0; i < GlobalFunc.isotopeList1.Count; i++)
                {
                    if (GlobalFunc.isotopeList1[i].Id_key == isotope.Id_key)
                    {
                        isoCode.Text = GlobalFunc.isotopeList1[i].Code;
                        isoDescription.Text = GlobalFunc.isotopeList1[i].Description;
                        isoBop.Text = GlobalFunc.isotopeList1[i].Bop;
                        isoHalfLife.Text = GlobalFunc.isotopeList1[i].HalfLife;
                        isoHalfLifeType.SelectedIndex = 1;
                        mpe.Text = GlobalFunc.isotopeList1[i].Mpe.ToString();
                        break;
                    }
                }
            }
        }
        #endregion

        private void isoAdd_Click(object sender, EventArgs e)
        {
            bool allow = true;
            if (isoCode.Text == "" && isoDescription.Text == "")
            {
                allow = false;
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("codeDescription"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            if (isoHalfLife.Text == "")
            {
                allow = false;
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("halfLife"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            if (isoHalfLifeType.SelectedIndex == -1)
            {
                allow = false;
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("halfLife"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0); 
            }
            if (isoBop.Text == "")
            {
                allow = false;
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("bop"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            if (mpe.Text == "")
            {
                allow = false;
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("mpe"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            if (allow)
            {
                Isotope isotope = new Isotope();
                isotope.Id_key = GlobalFunc.isotopeList1.Count + 1;
                isotope.Code = isoCode.Text;
                isotope.Description = isoDescription.Text;
                isotope.Bop = isoBop.Text;
                isotope.Mpe =Convert.ToDouble(mpe.Text);
                if (isoHalfLifeType.SelectedIndex == 0)
                {
                    isotope.HalfLife = (Convert.ToDouble(isoHalfLife.Text) * 365).ToString() ;
                }
                else
                {
                    isotope.HalfLife = isoHalfLife.Text;
                }
                GlobalFunc.isotopeList1.Add(isotope);
                GlobalFunc.SaveIsotopXML();
                GlobalFunc.LoadIsotopXML();
                isotopeList.DataSource = GlobalFunc.isotopeList1;
                isotopeList.DisplayMember = "Code";
                isotopeList.ValueMember = "Id_key";
                isotopeList.ClearSelected();
                isoAdd.Enabled = true;
                isoUpdate.Enabled = false;
                isoDelete.Enabled = false;
                isoCancel.Enabled = false;
                isoCode.Text = "";
                isoDescription.Text = "";
                isoHalfLife.Text = "";
                isoHalfLifeType.SelectedIndex = -1;
                isoBop.Text = "";
                mpe.Text = "";
            }
        }

        private void isoUpdate_Click(object sender, EventArgs e)
        {
            bool allow = true;
            if (isoCode.Text == "" && isoDescription.Text == "")
            {
                allow = false;
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("codeDescription"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            if (isoHalfLife.Text == "")
            {
                allow = false;
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("halfLife"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            }
            if (isoBop.Text == "")
            {
                allow = false;
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("bop"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            }
            if (mpe.Text == "")
            {
                allow = false;
                string buttonID = CustomMessageBox.Show(GlobalFunc.rm.GetString("mpe"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            if (allow)
            {
                Isotope isotope = (Isotope)isotopeList.SelectedItem;
                for (int i = 0; i < GlobalFunc.isotopeList1.Count; i++)
                {
                    if (GlobalFunc.isotopeList1[i].Id_key == isotope.Id_key)
                    {
                        GlobalFunc.isotopeList1[i].Code = isoCode.Text;
                        GlobalFunc.isotopeList1[i].Description = isoDescription.Text;
                        GlobalFunc.isotopeList1[i].Bop = isoBop.Text;
                        GlobalFunc.isotopeList1[i].Mpe = Convert.ToDouble(mpe.Text);
                        if (isoHalfLifeType.SelectedIndex == 0)
                        {
                            GlobalFunc.isotopeList1[i].HalfLife = (Convert.ToDouble(isoHalfLife.Text) * 365).ToString();
                        }
                        else
                        {
                            GlobalFunc.isotopeList1[i].HalfLife = isoHalfLife.Text;
                        }
                        break;
                    }
                }
                GlobalFunc.SaveIsotopXML();
                GlobalFunc.LoadIsotopXML();
                isotopeList.DataSource = GlobalFunc.isotopeList1;
                isotopeList.DisplayMember = "Code";
                isotopeList.ValueMember = "Id_key";
                isotopeList.ClearSelected();
                isoAdd.Enabled = true;
                isoUpdate.Enabled = false;
                isoDelete.Enabled = false;
                isoCancel.Enabled = false;
                isoCode.Text = "";
                isoDescription.Text = "";
                isoHalfLife.Text = "";
                isoHalfLifeType.SelectedIndex = -1;
                isoBop.Text = "";
                mpe.Text = "";
            }
        }

        private void isoDelete_Click(object sender, EventArgs e)
        {
            Isotope isotope = (Isotope)isotopeList.SelectedItem;
            int index = 0;
            for (int i = 0; i < GlobalFunc.isotopeList1.Count; i++)
            {
                if (GlobalFunc.isotopeList1[i].Id_key == isotope.Id_key)
                {
                    index = i;
                    break;
                }
            }
            GlobalFunc.isotopeList1.RemoveAt(index);
            GlobalFunc.SaveIsotopXML();
            GlobalFunc.LoadIsotopXML();
            isotopeList.DataSource = GlobalFunc.isotopeList1;
            isotopeList.DisplayMember = "Code";
            isotopeList.ValueMember = "Id_key";
            isotopeList.ClearSelected();
            isoAdd.Enabled = true;
            isoUpdate.Enabled = false;
            isoDelete.Enabled = false;
            isoCancel.Enabled = false;
            isoCode.Text = "";
            isoDescription.Text = "";
            isoHalfLife.Text = "";
            isoHalfLifeType.SelectedIndex = -1;
            isoBop.Text = "";
            mpe.Text = "";
        }

        private void isoCancel_Click(object sender, EventArgs e)
        {
            isotopeList.ClearSelected();
            GlobalFunc.LoadIsotopXML();
            isotopeList.DataSource = GlobalFunc.isotopeList1;
            isotopeList.DisplayMember = "Code";
            isotopeList.ValueMember = "Id_key";
            isotopeList.ClearSelected();
            isoAdd.Enabled = true;
            isoUpdate.Enabled = false;
            isoDelete.Enabled = false;
            isoCancel.Enabled = false;
            isoCode.Text = "";
            isoDescription.Text = "";
            isoHalfLife.Text = "";
            isoHalfLifeType.SelectedIndex = -1;
            isoBop.Text = "";
            mpe.Text = "";
        }

        public string toggleTextBox = "";
        private void scriptBtn_Click(object sender, EventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Name)
            {
                case "scriptBtn1": toggleTextBox = "wuBeginTxt";
                    break;
                case "scriptBtn2": toggleTextBox = "wuEndTxt";
                    break;
                case "scriptBtn3": toggleTextBox = "wuRoiTxt";
                    break;
                case "scriptBtn4": toggleTextBox = "wuDataTxt";
                    break;
                case "scriptBtn5": toggleTextBox = "bgBeginTxt";
                    break;
                case "scriptBtn6": toggleTextBox = "bgEndTxt";
                    break;
                case "scriptBtn7": toggleTextBox = "roi4Txt";
                    break;
                case "scriptBtn8": toggleTextBox = "roi5Txt";
                    break;
                case "scriptBtn9": toggleTextBox = "roi6Txt";
                    break;
                case "scriptBtn10": toggleTextBox = "calBeginTxt";
                    break;
                case "scriptBtn11": toggleTextBox = "calEndTxt";
                    break;
                case "scriptBtn12": toggleTextBox = "calUpdTxt";
                    break;
                case "scriptBtn13": toggleTextBox = "calDataTxt";
                    break;
                case "scriptBtn14": toggleTextBox = "meBeginTxt";
                    break;
                case "scriptBtn15": toggleTextBox = "meEndTxt";
                    break;
                case "scriptBtn16": toggleTextBox = "meDataTxt";
                    break;
                case "scriptBtn17": toggleTextBox = "bgDataTxt";
                    break;
            }
            openScriptFileDialog.AddExtension = true;
            openScriptFileDialog.Filter = "(*.*)|*.*";
            openScriptFileDialog.ShowDialog();
        }

        public string toggleXML = "";
        private void loadTopScriptBtn_Click(object sender, EventArgs e)
        {
            toggleXML = "TopScript.xml";
            scriptPanel.Visible = true;
            wuBeginTxt.Text = GlobalFunc.topScriptSet.WarmUpBegin;
            wuEndTxt.Text = GlobalFunc.topScriptSet.WarmUpEnd;
            wuRoiTxt.Text = GlobalFunc.topScriptSet.WarmUpRoi;
            wuDataTxt.Text = GlobalFunc.topScriptSet.WarmUpData;
            bgBeginTxt.Text = GlobalFunc.topScriptSet.BackgroundBegin;
            bgEndTxt.Text = GlobalFunc.topScriptSet.BackgroundEnd;
            bgDataTxt.Text = GlobalFunc.topScriptSet.BackgrounData;           
            calBeginTxt.Text = GlobalFunc.topScriptSet.CalibrationBegin;
            calEndTxt.Text = GlobalFunc.topScriptSet.CalibrationEnd;
            calUpdTxt.Text = GlobalFunc.topScriptSet.CalibrationUpdate;
            calDataTxt.Text = GlobalFunc.topScriptSet.CalibrationData;
            meBeginTxt.Text = GlobalFunc.topScriptSet.MeasureBegin;
            meEndTxt.Text = GlobalFunc.topScriptSet.MeasureEnd;
            meDataTxt.Text = GlobalFunc.topScriptSet.MeasureData;
        }

        private void loadBottomScriptBtn_Click(object sender, EventArgs e)
        {
            toggleXML = "BottomScript.xml";
            scriptPanel.Visible = true;
            wuBeginTxt.Text = GlobalFunc.bottomScriptSet.WarmUpBegin;
            wuEndTxt.Text = GlobalFunc.bottomScriptSet.WarmUpEnd;
            wuRoiTxt.Text = GlobalFunc.bottomScriptSet.WarmUpRoi;
            wuDataTxt.Text = GlobalFunc.bottomScriptSet.WarmUpData;
            bgBeginTxt.Text = GlobalFunc.bottomScriptSet.BackgroundBegin;
            bgEndTxt.Text = GlobalFunc.bottomScriptSet.BackgroundEnd;
            bgDataTxt.Text = GlobalFunc.bottomScriptSet.BackgrounData;
            
            calBeginTxt.Text = GlobalFunc.bottomScriptSet.CalibrationBegin;
            calEndTxt.Text = GlobalFunc.bottomScriptSet.CalibrationEnd;
            calUpdTxt.Text = GlobalFunc.bottomScriptSet.CalibrationUpdate;
            calDataTxt.Text = GlobalFunc.bottomScriptSet.CalibrationData;
            meBeginTxt.Text = GlobalFunc.bottomScriptSet.MeasureBegin;
            meEndTxt.Text = GlobalFunc.bottomScriptSet.MeasureEnd;
            meDataTxt.Text = GlobalFunc.bottomScriptSet.MeasureData;
        }

        private void loadDualScriptBtn_Click(object sender, EventArgs e)
        {
            toggleXML = "DualScript.xml";
            scriptPanel.Visible = true;
            wuBeginTxt.Text = GlobalFunc.dualScriptSet.WarmUpBegin;
            wuEndTxt.Text = GlobalFunc.dualScriptSet.WarmUpEnd;
            wuRoiTxt.Text = GlobalFunc.dualScriptSet.WarmUpRoi;
            wuDataTxt.Text = GlobalFunc.dualScriptSet.WarmUpData;
            bgBeginTxt.Text = GlobalFunc.dualScriptSet.BackgroundBegin;
            bgEndTxt.Text = GlobalFunc.dualScriptSet.BackgroundEnd;
            bgDataTxt.Text = GlobalFunc.dualScriptSet.BackgrounData;

            calBeginTxt.Text = GlobalFunc.dualScriptSet.CalibrationBegin;
            calEndTxt.Text = GlobalFunc.dualScriptSet.CalibrationEnd;
            calUpdTxt.Text = GlobalFunc.dualScriptSet.CalibrationUpdate;
            calDataTxt.Text = GlobalFunc.dualScriptSet.CalibrationData;
            meBeginTxt.Text = GlobalFunc.dualScriptSet.MeasureBegin;
            meEndTxt.Text = GlobalFunc.dualScriptSet.MeasureEnd;
            meDataTxt.Text = GlobalFunc.dualScriptSet.MeasureData;
        }

        private void saveScriptBtn_Click(object sender, EventArgs e)
        {
            if (toggleXML.Contains("Top"))
            {
                GlobalFunc.topScriptSet.WarmUpBegin = wuBeginTxt.Text;
                GlobalFunc.topScriptSet.WarmUpEnd =  wuEndTxt.Text;
                GlobalFunc.topScriptSet.WarmUpRoi =  wuRoiTxt.Text;
                GlobalFunc.topScriptSet.WarmUpData = wuDataTxt.Text;
                GlobalFunc.topScriptSet.BackgroundBegin = bgBeginTxt.Text;
                GlobalFunc.topScriptSet.BackgroundEnd =  bgEndTxt.Text;
                GlobalFunc.topScriptSet.BackgrounData =  bgDataTxt.Text;

                GlobalFunc.topScriptSet.CalibrationBegin = calBeginTxt.Text;
                GlobalFunc.topScriptSet.CalibrationEnd =  calEndTxt.Text;
                GlobalFunc.topScriptSet.CalibrationUpdate = calUpdTxt.Text;
                GlobalFunc.topScriptSet.CalibrationData = calDataTxt.Text;
                GlobalFunc.topScriptSet.MeasureBegin = meBeginTxt.Text;
                GlobalFunc.topScriptSet.MeasureEnd = meEndTxt.Text;
                GlobalFunc.topScriptSet.MeasureData = meDataTxt.Text;
            }
            else if (toggleXML.Contains("Bottom"))
            {
                GlobalFunc.bottomScriptSet.WarmUpBegin = wuBeginTxt.Text;
                GlobalFunc.bottomScriptSet.WarmUpEnd = wuEndTxt.Text;
                GlobalFunc.bottomScriptSet.WarmUpRoi = wuRoiTxt.Text;
                GlobalFunc.bottomScriptSet.WarmUpData = wuDataTxt.Text;
                GlobalFunc.bottomScriptSet.BackgroundBegin = bgBeginTxt.Text;
                GlobalFunc.bottomScriptSet.BackgroundEnd = bgEndTxt.Text;
                GlobalFunc.bottomScriptSet.BackgrounData = bgDataTxt.Text;

                GlobalFunc.bottomScriptSet.CalibrationBegin = calBeginTxt.Text;
                GlobalFunc.bottomScriptSet.CalibrationEnd = calEndTxt.Text;
                GlobalFunc.bottomScriptSet.CalibrationUpdate = calUpdTxt.Text;
                GlobalFunc.bottomScriptSet.CalibrationData = calDataTxt.Text;
                GlobalFunc.bottomScriptSet.MeasureBegin = meBeginTxt.Text;
                GlobalFunc.bottomScriptSet.MeasureEnd = meEndTxt.Text;
                GlobalFunc.bottomScriptSet.MeasureData = meDataTxt.Text;
            }
            else if (toggleXML.Contains("Dual"))
            {
                GlobalFunc.dualScriptSet.WarmUpBegin = wuBeginTxt.Text;
                GlobalFunc.dualScriptSet.WarmUpEnd = wuEndTxt.Text;
                GlobalFunc.dualScriptSet.WarmUpRoi = wuRoiTxt.Text;
                GlobalFunc.dualScriptSet.WarmUpData = wuDataTxt.Text;
                GlobalFunc.dualScriptSet.BackgroundBegin = bgBeginTxt.Text;
                GlobalFunc.dualScriptSet.BackgroundEnd = bgEndTxt.Text;
                GlobalFunc.dualScriptSet.BackgrounData = bgDataTxt.Text;

                GlobalFunc.dualScriptSet.CalibrationBegin = calBeginTxt.Text;
                GlobalFunc.dualScriptSet.CalibrationEnd = calEndTxt.Text;
                GlobalFunc.dualScriptSet.CalibrationUpdate = calUpdTxt.Text;
                GlobalFunc.dualScriptSet.CalibrationData = calDataTxt.Text;
                GlobalFunc.dualScriptSet.MeasureBegin = meBeginTxt.Text;
                GlobalFunc.dualScriptSet.MeasureEnd = meEndTxt.Text;
                GlobalFunc.dualScriptSet.MeasureData = meDataTxt.Text;
            }

            XmlSerializer serializer = new XmlSerializer(typeof(ScriptSet), "");
            TextWriter textWriter = new StreamWriter(@Directory.GetCurrentDirectory() + @"\xml\" + toggleXML);
            using (TextWriter tw = new Utf8StringWriter())
            {
                if (toggleXML.Contains("Top"))
                {
                    serializer.Serialize(textWriter, GlobalFunc.topScriptSet);
                }
                else if (toggleXML.Contains("Bottom"))
                {
                    serializer.Serialize(textWriter, GlobalFunc.bottomScriptSet);
                }
                else if (toggleXML.Contains("Dual"))
                {
                    serializer.Serialize(textWriter, GlobalFunc.dualScriptSet);
                }
            }
            textWriter.Close();
            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
        }

        public class Utf8StringWriter : TextWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }

        public void ReloadReportType1(DateTime getStartDT, DateTime getEndDT)
        {
            reportPanel.Controls.Clear();
            SqlCeCommand checkCmd = new SqlCeCommand("select profileName, refNo, Id, year, month, day from measure where fullDate between @startDate and @endDate order by Id desc", con);
            checkCmd.CommandType = CommandType.Text;
            checkCmd.Parameters.AddWithValue("@startDate", getStartDT);
            checkCmd.Parameters.AddWithValue("@endDate", getEndDT);
            SqlCeDataReader checkDr = checkCmd.ExecuteReader();
            int y = 0;

            while (checkDr.Read())
            {
                Label name = new Label();
                name.Text = "Profile: " + checkDr["profileName"].ToString() + " Date:" + checkDr["year"].ToString()
                    + "-" + checkDr["month"].ToString() + "-" + checkDr["day"].ToString() + " Ref#" + checkDr["refNo"].ToString();

                name.AutoSize = true;
                name.Location = new Point(0, y);
                reportPanel.Controls.Add(name);

                Button btn = new Button();
                btn.Text = GlobalFunc.rm.GetString("download");
                btn.Name = checkDr["Id"].ToString() + "@" + checkDr["year"].ToString() + "-" + checkDr["month"].ToString() + "-" + checkDr["day"].ToString() + "(" + checkDr["Id"].ToString() + ")" + "@download";
                btn.Location = new Point(name.Location.X + name.Width, y);
                btn.Click += GetReportClick;
                btn.AutoSize = true;
                reportPanel.Controls.Add(btn);

                Button delBtn = new Button();
                delBtn.Text = "Delete";
                delBtn.Name = checkDr["Id"].ToString() + "@" + checkDr["year"].ToString() + "-" + checkDr["month"].ToString() + "-" + checkDr["day"].ToString() + "(" + checkDr["Id"].ToString() + ")" + "@delete";
                delBtn.Location = new Point(btn.Location.X + btn.Width, y);
                delBtn.Click += DeleteReportClick;
                delBtn.AutoSize = true;
                reportPanel.Controls.Add(delBtn);

                y += 40;

            }
            checkDr.Close();            
        }

        //report
        private void searchReportBtn_Click(object sender, EventArgs e)
        {
            reportPanel.Controls.Clear();
            DateTime getStartDT = searchStartDate.Value;
            DateTime getEndDT = searchEndDate.Value;
            getStartDT = DateTime.Parse(getStartDT.Year + "-" + getStartDT.Month + "-" + getStartDT.Day + " 00:00:00.000");
            getEndDT = DateTime.Parse(getEndDT.Year + "-" + getEndDT.Month + "-" + getEndDT.Day + " 23:59:59.000");
            
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            if (reportTypeComboBox.SelectedIndex == 0)
            {
                ReloadReportType1(getStartDT, getEndDT);
                con.Close();
            }
            else if (reportTypeComboBox.SelectedIndex == 1)
            {
                Species species = (Species)reportSampleComboBox.SelectedItem;
                SqlCeCommand searchCmd = new SqlCeCommand(@"select Id, fullDate from measure where species = @species and fullDate between @startDate and @endDate ", con);
                searchCmd.CommandType = CommandType.Text;
                searchCmd.Parameters.AddWithValue("@startDate", getStartDT);
                searchCmd.Parameters.AddWithValue("@endDate", getEndDT);
                searchCmd.Parameters.AddWithValue("@species", species.Code);
                SqlCeDataReader searchDr = searchCmd.ExecuteReader();
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Sampling Statistics Report");
                sb.AppendLine("---------------------------");
                sb.AppendLine("Sample: " + species.Code);

                string strStartMonth = "";
                string strStartDay = "";
                string strEndMonth = "";
                string strEndDay = "";
                if (getStartDT.Month <= 9)
                {
                    strStartMonth = "0" + getStartDT.Month;
                }
                else
                {
                    strStartMonth = getStartDT.Month.ToString();
                }

                if (getStartDT.Day <= 9)
                {
                    strStartDay = "0" + getStartDT.Day;
                }
                else
                {
                    strStartDay = getStartDT.Day.ToString();
                }

                if (getEndDT.Month <= 9)
                {
                    strEndMonth = "0" + getEndDT.Month;
                }
                else
                {
                    strEndMonth = getEndDT.Month.ToString();
                }

                if (getEndDT.Day <= 9)
                {
                    strEndDay = "0" + getEndDT.Day;
                }
                else
                {
                    strEndDay = getEndDT.Day.ToString();
                }

                sb.AppendLine("Date: " + getStartDT.Year + "/" + strStartMonth + "/" + strStartDay + " - " + getEndDT.Year + "/" + strEndMonth + "/" + strEndDay);
                sb.AppendLine(" ");
                List<int> measureIDList = new List<int>();
                List<DateTime> dtList = new List<DateTime>();
                while (searchDr.Read())
                {
                    measureIDList.Add(Convert.ToInt32(searchDr["Id"]));
                    dtList.Add(DateTime.Parse(searchDr["fullDate"].ToString()));
                }
                searchDr.Close();

                string isotopeLine = "Date" + emptySpace(15);
                List<int> eachIsotopeNameLengthList = new List<int>();
                for (int i = 0; i < GlobalFunc.isotopeList1.Count; i++)
                {
                    eachIsotopeNameLengthList.Add(GlobalFunc.isotopeList1[i].Code.Length + 3);
                    isotopeLine += GlobalFunc.isotopeList1[i].Code + emptySpace(3);
                }
                isotopeLine += emptySpace(5) + "(Bq/kg)";
                sb.AppendLine(isotopeLine);
                string dashLine = "";
                for (int i = 0; i < isotopeLine.Length; i++)
                {
                    dashLine += "=";
                }
                sb.AppendLine(dashLine);

                for (int i = 0; i < measureIDList.Count; i++)
                {
                    string dtMonth = "";
                    string dtDay = "";
                    if (dtList[i].Month <= 9)
                    {
                        dtMonth = "0" + dtList[i].Month;
                    }
                    else
                    {
                        dtMonth = dtList[i].Month.ToString();
                    }

                    if (dtList[i].Day <= 9)
                    {
                        dtDay = "0" + dtList[i].Day;
                    }
                    else
                    {
                        dtDay = dtList[i].Day.ToString();
                    }

                    string resultLine = dtList[i].Year + "/" + dtMonth + "/" + dtDay +
                        emptySpace(13 - (dtList[i].Year + "/" + dtMonth + "/" + dtDay).ToString().Length);

                    for (int j = 0; j < GlobalFunc.isotopeList1.Count; j++)
                    {
                        SqlCeCommand rCmd = new SqlCeCommand("select isotopeName, activity, onAlarm from measureIsotope where mID = @mID and isotopeName = @isotopeName", con);
                        rCmd.CommandType = CommandType.Text;
                        rCmd.Parameters.AddWithValue("@mID", measureIDList[i]);
                        rCmd.Parameters.AddWithValue("@isotopeName", GlobalFunc.isotopeList1[j].Code);
                        SqlCeDataReader rDr = rCmd.ExecuteReader();
                        if (rDr.Read())
                        {
                            double actValue = Convert.ToDouble(rDr["activity"]);
                            if (double.IsNaN(actValue))
                            {
                                actValue = 0.0;
                            }
                            else if (actValue < 0.0)
                            {
                                actValue = 0.0;
                            }
                            else if (double.IsInfinity(actValue))
                            {
                                actValue = 0.0;
                            }
                            else
                            {
                                actValue = GlobalFunc.Math45(actValue);
                            }
                            string value = actValue.ToString();
                            if(Convert.ToInt32(rDr["onAlarm"]) == 1)
                            {
                                value += "*";
                            }
                            resultLine += value;
                            int codeLength = GlobalFunc.isotopeList1[j].Code.Length + 3;
                            int resultLength = value.Length;
                            resultLine += emptySpace(codeLength - resultLength);
                        }
                        else
                        {
                            resultLine += "-" + emptySpace(GlobalFunc.isotopeList1[j].Code.Length + 2);
                        }
                        rDr.Close();
                    }
                    sb.AppendLine(resultLine);
                }
                sb.AppendLine(" ");
                sb.AppendLine("* Alarm Level Exceeded");
                sb.AppendLine("- No Data");
                if (!Directory.Exists(@"C:\LCMS\TextReport"))
                {
                    Directory.CreateDirectory(@"C:\LCMS\TextReport");
                }

                string fileName = "sampling" + "_" + species.Code+ "_" + getStartDT.Year.ToString() + strStartMonth + strStartDay
                    + "_" + getEndDT.Year.ToString() + strEndMonth + strEndDay;
                File.WriteAllText(@"C:\LCMS\TextReport\" + fileName + ".txt", sb.ToString());
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), GlobalFunc.rm.GetString("openBtnTxt"), false, 0);
                if (btnClicked == "2")
                {
                    Process.Start(@"C:\LCMS\TextReport\" + fileName + ".txt");
                }
                con.Close();
            }
            else if (reportTypeComboBox.SelectedIndex == 2)
            {
                string strStartMonth = "";
                string strStartDay = "";
                string strEndMonth = "";
                string strEndDay = "";
                if (getStartDT.Month <= 9)
                {
                    strStartMonth = "0" + getStartDT.Month;
                }
                else
                {
                    strStartMonth = getStartDT.Month.ToString();
                }

                if (getStartDT.Day <= 9)
                {
                    strStartDay = "0" + getStartDT.Day;
                }
                else
                {
                    strStartDay = getStartDT.Day.ToString();
                }

                if (getEndDT.Month <= 9)
                {
                    strEndMonth = "0" + getEndDT.Month;
                }
                else
                {
                    strEndMonth = getEndDT.Month.ToString();
                }

                if (getEndDT.Day <= 9)
                {
                    strEndDay = "0" + getEndDT.Day;
                }
                else
                {
                    strEndDay = getEndDT.Day.ToString();
                }

                Isotope isotope = (Isotope)reportIsotopeComboBox.SelectedItem;
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Measurement Statistics Report");
                sb.AppendLine("------------------------------");
                sb.AppendLine("Isotope: " + isotope.Code);
                sb.AppendLine("Date: " + getStartDT.Year + "/" + strStartMonth + "/" + strStartDay + " - " + getEndDT.Year + "/" + strEndMonth + "/" + strEndDay);
                sb.AppendLine(" ");

                string elementLine = "";
                elementLine += "Activity(Bq/kg)   ";
                elementLine += "Uncertainty(%)   ";
                elementLine += "Profile          ";
                elementLine += "Sample   ";
                elementLine += "Reference No.   ";

                sb.AppendLine(elementLine);
                string dashLine = "";
                for (int i = 0; i < elementLine.Length; i++)
                {
                    dashLine += "=";
                }
                sb.AppendLine(dashLine);
                SqlCeCommand searchCmd = new SqlCeCommand(@"select Id, location, species, profileName, refNo, year, isotopeName, activity, 
                                                        uncertainty, onAlarm, mID from measure m, measureIsotope d
                                                        where m.Id = d.mID and isotopeName = @isotopeName and m.fullDate between @startDate and @endDate", con);
                searchCmd.CommandType = CommandType.Text;
                searchCmd.Parameters.AddWithValue("@isotopeName", isotope.Code);
                searchCmd.Parameters.AddWithValue("@startDate", getStartDT);
                searchCmd.Parameters.AddWithValue("@endDate", getEndDT);
                SqlCeDataReader rDr = searchCmd.ExecuteReader();
                while (rDr.Read())
                {
                    string line = "";
                    double actValue = Convert.ToDouble(rDr["activity"]);
                    if (double.IsNaN(actValue))
                    {
                        actValue = 0.0;
                    }
                    else if (actValue < 0.0)
                    {
                        actValue = 0.0;
                    }
                    else if (double.IsInfinity(actValue))
                    {
                        actValue = 0.0;
                    }
                    else
                    {
                        actValue = GlobalFunc.Math45(actValue);
                    }
                    string value1 = actValue.ToString();
                    if (Convert.ToInt32(rDr["onAlarm"]) == 1)
                    {
                        value1 += "*";
                    }
                    value1 += emptySpace("Activity   ".Length - value1.Length);

                    double uncertValue = Convert.ToDouble(rDr["uncertainty"]);
                    if (double.IsNaN(uncertValue))
                    {
                        uncertValue = 0.0;
                    }
                    if (double.IsInfinity(uncertValue))
                    {
                        uncertValue = 0.0;
                    }
                    else if (uncertValue < 0.0)
                    {
                        uncertValue = 0.0;
                    }
                    else
                    {
                        uncertValue = GlobalFunc.Math45(uncertValue);
                    }
                    string value2 = uncertValue.ToString();
                    value2 += emptySpace("Uncertainty   ".Length - value2.Length);

                    string value3 = rDr["profileName"].ToString();
                    value3 += emptySpace("Profile          ".Length - value3.Length);

                    string value4 = rDr["species"].ToString();
                    value4 += emptySpace("Sample   ".Length - value4.Length);

                    string value5 = rDr["location"] + "/" + rDr["year"].ToString() + "/" + rDr["refNo"].ToString();

                    line = value1 + value2 + value3 + value4 + value5;
                    sb.AppendLine(line);
                }
                rDr.Close();
                sb.AppendLine(" ");
                sb.AppendLine("* Alarm Level Exceeded");
                if (!Directory.Exists(@"C:\LCMS\TextReport"))
                {
                    Directory.CreateDirectory(@"C:\LCMS\TextReport");
                }

                string fileName = "measurement" + "_" + isotope.Code + "_" + getStartDT.Year.ToString() + strStartMonth + strStartDay
                    + "_" + getEndDT.Year.ToString() + strEndMonth + strEndDay;
                File.WriteAllText(@"C:\LCMS\TextReport\" + fileName + ".txt", sb.ToString());
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), GlobalFunc.rm.GetString("openBtnTxt"), false, 0);
                if (btnClicked == "2")
                {
                    Process.Start(@"C:\LCMS\TextReport\" + fileName + ".txt");
                }
                con.Close();
            }
           
        }

        public string emptySpace(int counter)
        {
            string space = "";
            for(int i = 0; i  < counter; i++)
            {
                space += " ";
            }
            return space;
        }

        string downloadReportID = "";
        private void GetReportClick(object sender, EventArgs e)
        {
            Button dlBtn = sender as Button;
            downloadReportID = dlBtn.Name.Substring(0, dlBtn.Name.IndexOf("@"));
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand cmd = new SqlCeCommand("select year, month, day, time, refNo, profileName from measure where Id = @id", con);
            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@id", downloadReportID);
           
            SqlCeDataReader dr = cmd.ExecuteReader();
            if (dr.Read())
            {
                string year = dr["year"].ToString().Substring(2, 2);
                string month = dr["month"].ToString();
                if (Convert.ToInt32(dr["month"]) <= 9)
                {
                    month = "0" + month;
                }
                string day = dr["day"].ToString();
                if (Convert.ToInt32(dr["day"]) <= 9)
                {
                    day = "0" + day;
                }
                string time = dr["time"].ToString().Replace(" PM", "").Replace(" AM", "");
                string hour = time.Substring(0, time.IndexOf(":"));
                string min = time.Substring(time.IndexOf(":") + 1, time.Length - (time.IndexOf(":") + 1));
                if (hour.Length == 1)
                {
                    hour = "0" + hour;
                }
                if (min.Length == 1)
                {
                    min = "0" + min;
                }
                string fileName = dr["profileName"].ToString() + "_" + year + month + day + hour + min + "(" + dr["refNo"].ToString() + ")";
                saveReportDialog.FileName = fileName;
            }
            dr.Close();
            con.Close();
            saveReportDialog.ShowDialog();
        }

        private void DeleteReportClick(object sender, EventArgs e)
        {
            Button delbtn = sender as Button;
            string id = delbtn.Name.Substring(0, delbtn.Name.IndexOf("@"));
            string fileName = delbtn.Name.Substring(delbtn.Name.IndexOf("@") + 1, delbtn.Name.LastIndexOf("@") - (delbtn.Name.IndexOf("@") + 1));
            if(File.Exists(@"C:\LCMS\WordReport\" + fileName + ".docx"))
            {
                File.Delete(@"C:\LCMS\WordReport\" + fileName + ".docx");
            }
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand delCmd = new SqlCeCommand(@"delete from measure where Id = @mID", con);
            delCmd.CommandType = CommandType.Text;
            delCmd.Parameters.AddWithValue("@mID", id);
            delCmd.ExecuteNonQuery();
            delCmd.Dispose();
            
            delCmd = new SqlCeCommand(@"  delete from measureDetail where mID = @mID", con);
            delCmd.CommandType = CommandType.Text;
            delCmd.Parameters.AddWithValue("@mID", id);
            delCmd.ExecuteNonQuery();
            delCmd.Dispose();

            delCmd = new SqlCeCommand(@"delete from measureIsotope where mID = @mID", con);
            delCmd.CommandType = CommandType.Text;
            delCmd.Parameters.AddWithValue("@mID", id);
            delCmd.ExecuteNonQuery();
            delCmd.Dispose();

            DateTime getStartDT = searchStartDate.Value;
            DateTime getEndDT = searchEndDate.Value;
            getStartDT = DateTime.Parse(getStartDT.Year + "-" + getStartDT.Month + "-" + getStartDT.Day + " 00:00:00.000");
            getEndDT = DateTime.Parse(getEndDT.Year + "-" + getEndDT.Month + "-" + getEndDT.Day + " 23:59:59.000");
            ReloadReportType1(getStartDT, getEndDT);
            
            con.Close();
        }

        private void editScriptBtn_Click(object sender, EventArgs e)
        {
            /*if (GlobalFunc.se != null)
            {
                GlobalFunc.se.Dispose();
            }
            GlobalFunc.se = new ScriptEditor();
            
            GlobalFunc.se.Show();*/

            openEditScriptDialog.AddExtension = true;
            openEditScriptDialog.Filter = "(*.*)|*.*";
            openEditScriptDialog.ShowDialog();
        }

        private void openEditScriptDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (openEditScriptDialog.FileName != "")
            {
                if (GlobalFunc.se != null)
                {
                    GlobalFunc.se.Dispose();
                }
                GlobalFunc.se = new ScriptEditor();
                GlobalFunc.se.SetScript(openEditScriptDialog.FileName);
                GlobalFunc.se.BringToFront();
                GlobalFunc.se.Show();                
            }
        }

        private void saveReportDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (saveReportDialog.FileName != "")
            {
                if (GlobalFunc.outputWord != null)
                {
                    GlobalFunc.outputWord.Dispose();
                }
                GlobalFunc.outputWord = new OutputWord();
                GlobalFunc.outputWord.CreateReportTable(Convert.ToInt32(downloadReportID), saveReportDialog.FileName.ToString());
                if (File.Exists(saveReportDialog.FileName))
                {
                    string btnClicked2 = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("openBtnTxt"), GlobalFunc.rm.GetString("printBtnTxt"), false, 0);

                    if (btnClicked2 == "1")
                    {
                        Process.Start(saveReportDialog.FileName);
                    }
                    else if (btnClicked2 == "2")
                    {
                        /**/
                        GlobalFunc.outputWord.PrintWord(saveReportDialog.FileName);
                    }
                }
            }
        }

        public void EnableControl()
        {
            detectorComboBox.Enabled = true;
            nor.Enabled = true;
            selBkLocation.Enabled = true;
            alarmComboBox.Enabled = true;
            isotopeNumComboBox.Enabled = true;
            this.Enabled = true;
        }

        private void updTopICR_Click(object sender, EventArgs e)
        {
            if (topICRTxt.Text != "")
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("delete from icr where detector = 'Top'", con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                SqlCeCommand insertCmd = new SqlCeCommand("insert into icr values ('Top', @icr)", con);
                insertCmd.CommandType = CommandType.Text;
                insertCmd.Parameters.AddWithValue("@icr", topICRTxt.Text);
                GlobalFunc.det1_icr_alarm = Convert.ToDouble(topICRTxt.Text);
                insertCmd.ExecuteNonQuery();
                insertCmd.Dispose();
                con.Close();
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            }
            else
            { }
        }

        private void updBottomICR_Click(object sender, EventArgs e)
        {
            if (bottomICRTxt.Text != "")
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand cmd = new SqlCeCommand("delete from icr where detector = 'Bottom'", con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                SqlCeCommand insertCmd = new SqlCeCommand("insert into icr values ('Bottom', @icr)", con);
                insertCmd.CommandType = CommandType.Text;
                insertCmd.Parameters.AddWithValue("@icr", bottomICRTxt.Text);
                GlobalFunc.det2_icr_alarm = Convert.ToDouble(bottomICRTxt.Text);
                insertCmd.ExecuteNonQuery();
                insertCmd.Dispose();
                con.Close();
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            }
            else
            { }
        }

        private void updBufferBtn_Click(object sender, EventArgs e)
        {
            if (bufferTimeTxt.Text != "")
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand updateCmd = new SqlCeCommand("update time set bufferTime = @bufferTime", con);
                updateCmd.CommandType = CommandType.Text;
                updateCmd.Parameters.AddWithValue("@bufferTime", bufferTimeTxt.Text);
                GlobalFunc.bufferTime = Convert.ToInt32(bufferTimeTxt.Text);
                updateCmd.ExecuteNonQuery();
                updateCmd.Dispose();
                con.Close();
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            }
            else
            { }
        }

        private void updWUTimeBtn_Click(object sender, EventArgs e)
        {
            if (warmUpTimeTxt.Text != "")
            {
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand updateCmd = new SqlCeCommand("update time set warmupTime = @warmupTime", con);
                updateCmd.CommandType = CommandType.Text;
                updateCmd.Parameters.AddWithValue("@warmupTime", warmUpTimeTxt.Text);
                GlobalFunc.warmupTime = Convert.ToDouble(warmUpTimeTxt.Text);
                updateCmd.ExecuteNonQuery();
                updateCmd.Dispose();
                con.Close();
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            { }
        }

        private void rsUpdBtn_Click(object sender, EventArgs e)
        {
            int rsl = 0;
            if (rsEnable.Checked == true)
            {
                rsl = 1;
            }
            else if (rsDisable.Checked == true)
            {
                rsl = 0;
            }
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand updateCmd = new SqlCeCommand("update time set rsl = @rsl", con);
            updateCmd.CommandType = CommandType.Text;
            updateCmd.Parameters.AddWithValue("@rsl", rsl);
            GlobalFunc.rsl = rsl;
            updateCmd.ExecuteNonQuery();
            updateCmd.Dispose();
            con.Close();
            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
        }

        private void ruUpdBtn_Click(object sender, EventArgs e)
        {
            int ru = 0;
            if (ruEnable.Checked == true)
            {
                ru = 1;
            }
            else if (ruDisable.Checked == true)
            {
                ru = 0;
            }
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand updateCmd = new SqlCeCommand("update time set ru = @ru", con);
            updateCmd.CommandType = CommandType.Text;
            updateCmd.Parameters.AddWithValue("@ru", ru);
            GlobalFunc.ru = ru;
            updateCmd.ExecuteNonQuery();
            updateCmd.Dispose();
            con.Close();
            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

        }

        private void mlrupdBtn_Click(object sender, EventArgs e)
        {
            int mlr = 0;
            if (mlrEnable.Checked == true)
            {
                mlr = 1;
            }
            else if (mlrDisable.Checked == true)
            {
                mlr = 0;
            }
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand updateCmd = new SqlCeCommand("update time set mlr = @mlr", con);
            updateCmd.CommandType = CommandType.Text;
            updateCmd.Parameters.AddWithValue("@mlr", mlr);
            GlobalFunc.mlr = mlr;
            updateCmd.ExecuteNonQuery();
            updateCmd.Dispose();
            con.Close();
            string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

        }

        private void resetAlarmBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalFunc.SetAlarmBox(0);
                string btnClicked = CustomMessageBox.Show("Reset Alarm box successful", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            catch { }
        }

        private void saveProfileBtn_Click(object sender, EventArgs e)
        {
            string line;
            string path = GlobalFunc.loadProfile.FileName.Replace(".txt", "") + ".txt";
            bool passToRun = true ;
            isotopeQuantity = isotopeNumComboBox.SelectedItem.ToString();
            GetFromElement(ref passToRun);

            if (nor.Text == "")
            {
                passToRun = false;
                string buttonID = CustomMessageBox.Show("Number of region cannot empty", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                numOfRegion = Convert.ToInt32(nor.Text);
            }
            if (selBkLocation.SelectedIndex == -1)
            {
                passToRun = false;
                string buttonID = CustomMessageBox.Show("Please select location", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            }
            if (alarmComboBox.SelectedIndex == -1)
            {
                passToRun = false;
                string buttonID = CustomMessageBox.Show("Please select alarm type", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            }
            if (profileName.Text == "")
            {
                passToRun = false;
                string buttonID = CustomMessageBox.Show("Please Enter Profile Name", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            }
            StringBuilder sb = new StringBuilder();
            if (passToRun)
            {
                
                int countToIsotope = 0;
                using (StreamReader reader = File.OpenText(path))
                {
                    while ((line = reader.ReadLine()) != null)
                    {
                        if (line.Contains("Profile Name:"))
                        {
                            sb.AppendLine("Profile Name:" + profileName.Text); 
                        }
                        else if (line.Contains("Location:"))
                        {
                            sb.AppendLine("Location:" + ((Location)selBkLocation.SelectedItem).Description);
                        }
                        else if (line.Contains("Roi Path1:"))
                        {
                            sb.AppendLine("Roi Path1:" + roiPathText1.Text);
                        }
                        else if (line.Contains("Roi Path2:"))
                        {
                            sb.AppendLine("Roi Path2:" + roiPathText2.Text);
                        }
                        else if (line.Contains("Alarm Type:"))
                        {
                            sb.AppendLine("Alarm Type:" + alarmComboBox.SelectedItem);
                        }
                        else if (line.Contains("Bottom CPS:") && !line.Contains("Background"))
                        {
                            countToIsotope += 1;
                            sb.AppendLine(line);
                        }
                        else if (line.Contains("Activity:"))
                        {
                            sb.AppendLine("Activity:" + Convert.ToDouble(calIsotopeList[countToIsotope].activity) * 1000);
                        }
                        else if (line.Contains("Ref Datetime:"))
                        {
                            sb.AppendLine("Ref Datetime:" + calIsotopeList[countToIsotope].refDate.Year + "-" + calIsotopeList[countToIsotope].refDate.Month + "-" + calIsotopeList[countToIsotope].refDate.Day + " " +
                                                            calIsotopeList[countToIsotope].refDate.Hour + ":" + calIsotopeList[countToIsotope].refDate.Minute);
                        }
                        else if (line.Contains("Measure Time:"))
                        {
                            sb.AppendLine("Measure Time:" + calIsotopeList[countToIsotope].measureTime);

                        }
                        else if (line.Contains("DIL:"))
                        {
                            sb.AppendLine("DIL:" + calIsotopeList[countToIsotope].dil);
                        }
                        else if (line.Contains("AL:"))
                        {
                            sb.AppendLine("AL:" + calIsotopeList[countToIsotope].al);
                        }
                        else if (line.Contains("AL%:"))
                        {
                            sb.AppendLine("AL%:" + calIsotopeList[countToIsotope].al_pc);
                        }
                        else
                        {
                            sb.AppendLine(line);
                        }
                    }
                }
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }
                }
                catch (Exception ex)
                {

                }
                string newPath = @"C:\LCMS\Profile\" + profileName.Text + ".txt";
                File.WriteAllText(newPath, sb.ToString());
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand updCmd = new SqlCeCommand("update profileName set profileName = @profileName, fileName = @fileName where Id = @Id", con);
                updCmd.CommandType = CommandType.Text;
                updCmd.Parameters.AddWithValue("@profileName", profileName.Text);
                updCmd.Parameters.AddWithValue("@fileName", profileName.Text + ".txt");
                updCmd.Parameters.AddWithValue("@Id", selectedProfileID);
                updCmd.ExecuteNonQuery();
                con.Close();
                
                resetCalibration();
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                LoadProfile();
                for (int i = 0; i < profileComboBox.Items.Count; i++)
                {
                    Profile p = (Profile)profileComboBox.Items[i];
                    if (p.ID == selectedProfileID)
                    {
                        profileComboBox.SelectedIndex = i;
                    }
                }
                LoadSelectedProfile(false);  
            }
        }

        string toggleRoiTextBox = "roi1";
        private void selectRoiPathBtn_Click(object sender, EventArgs e)
        {
            toggleRoiTextBox = "roi1";
            openRoiPathDialog.ShowDialog();
        }

        private void openRoiPathDialog_FileOk(object sender, CancelEventArgs e)
        {
            if (openRoiPathDialog.FileName != "")
            {
                if (toggleRoiTextBox == "roi1")
                {
                    roiPathText1.Text = openRoiPathDialog.FileName;
                }
                else
                {
                    roiPathText2.Text = openRoiPathDialog.FileName;
                }
            }
        }

        private void factoryResetBtn_Click(object sender, EventArgs e)
        {
            
            string btnClicked = CustomMessageBox.Show("Sure to factory reset?", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), GlobalFunc.rm.GetString("noBtnTxt"), false, 0);

            if (btnClicked == "1")
            {
                GlobalFunc.logManager.WriteLog("Supervisor userID: " + GlobalFunc.userID + " do factory reset");
                BackupXMLDB(false);
                if (con.State == ConnectionState.Open)
                {
                    con.Close();
                }
                con.Open();
                SqlCeCommand dropCmd = new SqlCeCommand("DROP TABLE [bkStatus];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [bkStatusDetail];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("delete from currProfile", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [destination];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [detector];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [farmCode];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [icr];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [isotope];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [job];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [location];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [measure];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [measureDetail];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [measureIsotope];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [origin];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [profileName];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();        
                dropCmd = new SqlCeCommand("DROP TABLE [roiDefine];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [sample_paramter];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [species];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();      
                dropCmd = new SqlCeCommand("DROP TABLE [time];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();
                dropCmd = new SqlCeCommand("DROP TABLE [userPassword];", con);
                dropCmd.ExecuteNonQuery();
                dropCmd.Dispose();

                SqlCeCommand createCmd = new SqlCeCommand(@"CREATE TABLE [bkStatus] (
                                                          [id_key] bigint IDENTITY (1,1) NOT NULL
                                                        , [date] datetime NOT NULL
                                                        , [location] nvarchar(100) NOT NULL
                                                        , [totalCount] nvarchar(100) NOT NULL
                                                        , [countTime] nvarchar(100) NULL
                                                        );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();                                

                SqlCeCommand alterCmd = new SqlCeCommand("ALTER TABLE [bkStatus] ADD CONSTRAINT [PK_bkStatus] PRIMARY KEY ([id_key]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [bkStatusDetail] (
                                                              [id_key] bigint IDENTITY (1,1) NOT NULL
                                                            , [name] nvarchar(100) NOT NULL
                                                            , [displayOrder] bigint NOT NULL
                                                            , [bkStatusID] bigint NOT NULL
                                                            , [countRate] nvarchar(100) NULL
                                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                alterCmd = new SqlCeCommand(@"ALTER TABLE [bkStatusDetail] ADD CONSTRAINT [PK_bkStatusDetail] PRIMARY KEY ([id_key]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [destination] (
                                              [id_key] bigint IDENTITY (1,1) NOT NULL
                                            , [code] nvarchar(100) NOT NULL
                                            , [description] nvarchar(100) NOT NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                alterCmd = new SqlCeCommand(@"ALTER TABLE [destination] ADD CONSTRAINT [PK_destination] PRIMARY KEY ([id_key]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();


                createCmd = new SqlCeCommand(@"CREATE TABLE [detector] ( [name] nvarchar(100) NOT NULL);", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [farmCode] (
                                              [id_key] bigint IDENTITY (1,1) NOT NULL
                                            , [code] nvarchar(100) NOT NULL
                                            , [description] nvarchar(100) NOT NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                alterCmd = new SqlCeCommand(@"ALTER TABLE [farmCode] ADD CONSTRAINT [PK_farmCode] PRIMARY KEY ([id_key]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [icr] (
                                                  [detector] nvarchar(100) NOT NULL
                                                , [icr] nvarchar(100) NOT NULL
                                                );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                alterCmd = new SqlCeCommand(@"ALTER TABLE [icr] ADD CONSTRAINT [PK_icr] PRIMARY KEY ([detector]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [isotope] (
                                          [id_key] bigint IDENTITY (1,1) NOT NULL
                                        , [code] nvarchar(100) NOT NULL
                                        , [description] nvarchar(100) NOT NULL
                                        , [halfLife] nvarchar(100) NOT NULL
                                        , [speciesID] bigint NOT NULL
                                        , [dil] nvarchar(20) NOT NULL
                                        , [al] nvarchar(20) NOT NULL
                                        , [alPercentage] nvarchar(20) NOT NULL
                                        );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                alterCmd = new SqlCeCommand(@"ALTER TABLE [isotope] ADD CONSTRAINT [PK_isotope] PRIMARY KEY ([id_key]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();
                
                createCmd = new SqlCeCommand(@"CREATE TABLE [job] (
                                              [jobID] bigint IDENTITY (1,1) NOT NULL
                                            , [jobName] nvarchar(100) NOT NULL
                                            , [date] nvarchar(25) NULL
                                            , [status] int NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();
                
                alterCmd = new SqlCeCommand(@"ALTER TABLE [job] ADD CONSTRAINT [PK_job] PRIMARY KEY ([jobID]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [location] (
                                              [id_key] bigint IDENTITY (1,1) NOT NULL
                                            , [code] nvarchar(100) NOT NULL
                                            , [description] nvarchar(100) NOT NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                alterCmd = new SqlCeCommand(@"ALTER TABLE [location] ADD CONSTRAINT [PK_location] PRIMARY KEY ([id_key]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();


                createCmd = new SqlCeCommand(@"CREATE TABLE [measure] (
                                      [Id] bigint IDENTITY (1,1) NOT NULL
                                    , [location] nvarchar(100) NOT NULL
                                    , [vehicleNo] nvarchar(100) NOT NULL
                                    , [hcn] nvarchar(100) NOT NULL
                                    , [farmCode] nvarchar(100) NOT NULL
                                    , [poo] nvarchar(100) NOT NULL
                                    , [species] nvarchar(100) NOT NULL
                                    , [qty] bigint NOT NULL
                                    , [sampleSize] numeric(19,2) NOT NULL
                                    , [destination] nvarchar(100) NOT NULL
                                    , [remarks] nvarchar(3000) NOT NULL
                                    , [year] int NULL
                                    , [month] int NULL
                                    , [day] int NULL
                                    , [time] nvarchar(100) NULL
                                    , [refNo] int NULL
                                    , [ratioSum] nvarchar(25) NULL
                                    , [icr] nvarchar(25) NULL
                                    , [profileName] nvarchar(100) NULL
                                    , [icrError] int NULL
                                    , [rsl] int NULL
                                    , [ru] int NULL
                                    , [mlr] int NULL
                                    , [detector] nvarchar(25) NULL
                                    , [fulldate] datetime, NULL
                                    , [satis] int NULL
                                    , [showTemp] int NULL
                                    );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();
                
                alterCmd = new SqlCeCommand(@"ALTER TABLE [measure] ADD CONSTRAINT [PK_measure] PRIMARY KEY ([Id]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [measureDetail] (
                                              [mID] bigint NOT NULL
                                            , [temperature] nvarchar(100) NOT NULL
                                            , [countTime] nvarchar(100) NOT NULL
                                            , [icr1] nvarchar(100) NOT NULL
                                            , [icr2] nvarchar(100) NOT NULL
                                            , [icrAlarmLevel1] nvarchar(25) NULL
                                            , [icrAlarmLevel2] nvarchar(25) NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();
                

                createCmd = new SqlCeCommand(@"CREATE TABLE [measureIsotope] (
                                              [mID] bigint NOT NULL
                                            , [isotopeName] nvarchar(100) NOT NULL
                                            , [activity] nvarchar(100) NOT NULL
                                            , [uncertainty] nvarchar(100) NOT NULL
                                            , [detected] int NOT NULL
                                            , [alarmLevel] nvarchar(100) NOT NULL
                                            , [alarmType] nvarchar(100) NOT NULL
                                            , [alarmLevelPC] nvarchar(100) NULL
                                            , [onAlarm] int NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [origin] (
                                              [Id] bigint IDENTITY (1,1) NOT NULL
                                            , [engname] nvarchar(100) NOT NULL
                                            , [chiname] nvarchar(100) NOT NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                alterCmd = new SqlCeCommand(@"ALTER TABLE [origin] ADD CONSTRAINT [PK_origin] PRIMARY KEY ([Id]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [profileName] (
                                              [Id] bigint IDENTITY (1,1) NOT NULL
                                            , [profileName] nvarchar(100) NOT NULL
                                            , [fileName] nvarchar(100) NOT NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                alterCmd = new SqlCeCommand(@"ALTER TABLE [profileName] ADD CONSTRAINT [PK_profileName] PRIMARY KEY ([Id]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [roiDefine] (
                                              [displayOrder] int NOT NULL
                                            , [name] nvarchar(100) NOT NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [sample_paramter] (
                  [id_key] bigint IDENTITY (1,1) NOT NULL
                , [code] nvarchar(100) NOT NULL
                , [description] nvarchar(500) NOT NULL
                , [type] nvarchar(100) NOT NULL
                );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                alterCmd = new SqlCeCommand(@"ALTER TABLE [sample_paramter] ADD CONSTRAINT [PK_sample_paramter] PRIMARY KEY ([id_key]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [species] (
                                              [id_key] bigint IDENTITY (1,1) NOT NULL
                                            , [code] nvarchar(100) NOT NULL
                                            , [description] nvarchar(100) NOT NULL
                                            , [scriptPath] nvarchar(500) NOT NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                alterCmd = new SqlCeCommand(@"ALTER TABLE [species] ADD CONSTRAINT [PK_species] PRIMARY KEY ([id_key]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [time] (
                                              [sample] numeric(10,1) NOT NULL
                                            , [background] numeric(10,1) NOT NULL
                                            , [alarm] numeric(10,1) NOT NULL
                                            , [bufferTime] int NULL
                                            , [warmupTime] numeric(10,1) NULL
                                            , [rsl] int NULL
                                            , [ru] int NULL
                                            , [mlr] int NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();
                

                createCmd = new SqlCeCommand(@"insert into time (sample, background, alarm, bufferTime, warmupTime, rsl, ru, mlr)
                                                values 
                                               (0.0, 0.0, 0.0, 10, 1, 1, 1, 1)", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                createCmd = new SqlCeCommand(@"CREATE TABLE [userPassword] (
                                              [id_key] bigint IDENTITY (1,1) NOT NULL
                                            , [password] nvarchar(100) NOT NULL
                                            , [type] nvarchar(100) NOT NULL
                                            );", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();
              
                alterCmd = new SqlCeCommand(@"ALTER TABLE [userPassword] ADD CONSTRAINT [PK_userPassword] PRIMARY KEY ([id_key]);", con);
                alterCmd.ExecuteNonQuery();
                alterCmd.Dispose();

                createCmd = new SqlCeCommand(@"insert into [userPassword] (password, type) values ('p@ssw0rd', '1');", con);
                createCmd.ExecuteNonQuery();
                createCmd.Dispose();

                con.Close();
                try
                {
                    Rollback("xml");
                    //File.Delete(@Directory.GetCurrentDirectory() + @"\lcmsDB.sdf");
                    //File.Copy(@Directory.GetCurrentDirectory() + @"\defaultSetting\lcmsDB.sdf", @Directory.GetCurrentDirectory() + @"\lcmsDB.sdf");
                    string btnClicked2 = CustomMessageBox.Show("Factory Reset Successful, please close the program and start again", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
                    if (btnClicked2 == "1")
                    {
                        GlobalFunc.tc.axUCONN21.Close();
                        GlobalFunc.logManager.WriteLog("Close LCMS by factory reset");
                        Process.GetCurrentProcess().Kill();
                        Application.Exit();
                    }
                }
                catch { }

            }
            else if (btnClicked == "2")
            {
                
            }
        }

        private void openAlarmBoxBtn_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalFunc.SetAlarmBox(6);
                string btnClicked = CustomMessageBox.Show("Reset Alarm box successful", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            catch { }
        }

        private void selectRoiPathBtn2_Click(object sender, EventArgs e)
        {
            toggleRoiTextBox = "roi2";
            openRoiPathDialog.ShowDialog();
        }

        private void refNoResetBtn_Click(object sender, EventArgs e)
        {
            BackupXMLDB(false);
            GlobalFunc.logManager.WriteLog("Supervisor userID: " + GlobalFunc.userID + " do measurement ref No. reset");
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand dropCmd = new SqlCeCommand("DROP TABLE [measure];", con);
            dropCmd.ExecuteNonQuery();
            dropCmd.Dispose();

            dropCmd = new SqlCeCommand("DROP TABLE [measureDetail];", con);
            dropCmd.ExecuteNonQuery();
            dropCmd.Dispose();

            dropCmd = new SqlCeCommand("DROP TABLE [measureIsotope];", con);
            dropCmd.ExecuteNonQuery();
            dropCmd.Dispose();
            
            SqlCeCommand createCmd = new SqlCeCommand(@"CREATE TABLE [measure] (
                                                          [Id] bigint IDENTITY (1,1) NOT NULL
                                                        , [location] nvarchar(100) NOT NULL
                                                        , [vehicleNo] nvarchar(100) NOT NULL
                                                        , [hcn] nvarchar(100) NOT NULL
                                                        , [farmCode] nvarchar(100) NOT NULL
                                                        , [poo] nvarchar(100) NOT NULL
                                                        , [species] nvarchar(100) NOT NULL
                                                        , [qty] bigint NOT NULL
                                                        , [sampleSize] numeric(19,2) NOT NULL
                                                        , [destination] nvarchar(100) NOT NULL
                                                        , [remarks] nvarchar(3000) NOT NULL
                                                        , [year] int NULL
                                                        , [month] int NULL
                                                        , [day] int NULL
                                                        , [time] nvarchar(100) NULL
                                                        , [refNo] nvarchar(3) NULL
                                                        , [ratioSum] nvarchar(25) NULL
                                                        , [icr] nvarchar(25) NULL
                                                        , [profileName] nvarchar(100) NULL
                                                        , [icrError] int NULL
                                                        , [rsl] int NULL
                                                        , [ru] int NULL
                                                        , [mlr] int NULL
                                                        , [detector] nvarchar(25) NULL
                                                        , [fulldate] datetime NULL
                                                        );
                                                         ", con);
            createCmd.ExecuteNonQuery();
            createCmd.Dispose();

            createCmd = new SqlCeCommand(@" CREATE TABLE [measureDetail] (
                                                          [mID] bigint NOT NULL
                                                        , [temperature] nvarchar(100) NOT NULL
                                                        , [countTime] nvarchar(100) NOT NULL
                                                        , [icr1] nvarchar(100) NOT NULL
                                                        , [icr2] nvarchar(100) NOT NULL
                                                        , [icrAlarmLevel1] nvarchar(25) NULL
                                                        , [icrAlarmLevel2] nvarchar(25) NULL
                                                        );", con);
            createCmd.ExecuteNonQuery();
            createCmd.Dispose();


            createCmd = new SqlCeCommand(@" CREATE TABLE [measureIsotope] (
                                                          [mID] bigint NOT NULL
                                                        , [isotopeName] nvarchar(100) NOT NULL
                                                        , [activity] nvarchar(100) NOT NULL
                                                        , [uncertainty] nvarchar(100) NOT NULL
                                                        , [detected] int NOT NULL
                                                        , [alarmLevel] nvarchar(100) NOT NULL
                                                        , [alarmType] nvarchar(100) NOT NULL
                                                        , [alarmLevelPC] nvarchar(100) NULL
                                                        , [onAlarm] int NULL
                                                        );", con);
            createCmd.ExecuteNonQuery();
            createCmd.Dispose();

            SqlCeCommand alterCmd = new SqlCeCommand("ALTER TABLE [measure] ADD CONSTRAINT [PK_measure] PRIMARY KEY ([Id]);", con);
            alterCmd.ExecuteNonQuery();
            alterCmd.Dispose();
            con.Close();
            string btnClicked = CustomMessageBox.Show("Reset measurement successful", GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
        }

        private void tipsButton_Click(object sender, EventArgs e)
        {
            string btnClicked = CustomMessageBox.Show("Procedure to find backup measurement reports" + System.Environment.NewLine +                
                "1. Go to \"Maintenance\" and click \"Backup Database, Profile, Xml files\" on left side. " + System.Environment.NewLine + 
                @"2. Go to C:\LCMS\Backup\ and find the suitable backup folder" + System.Environment.NewLine + 
                "3. Copy \"lcmsDB.sdf\"" + @" to C:\LCMS\Backup\" + System.Environment.NewLine + 
                "4. After get the reports, please copy the latest backup of lcmsDB.sdf to " +  @"C:\LCMS\Backup\" + System.Environment.NewLine 
                , GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
        }

        private void reportTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (reportTypeComboBox.SelectedIndex == 0)
            {
                reportPanel.Controls.Clear();
                reportSampleComboBox.Visible = false;
                reportIsotopeComboBox.Visible = false;
                searchReportBtn.Location = new Point(reportTypeComboBox.Location.X + reportTypeComboBox.Width + 5, reportTypeComboBox.Location.Y);
                searchReportBtn.Text = GlobalFunc.rm.GetString("searchBtn");
            }
            else if (reportTypeComboBox.SelectedIndex == 1)
            {
                reportPanel.Controls.Clear();
                searchReportBtn.Location = new Point(reportSampleComboBox.Location.X + reportSampleComboBox.Width + 5, reportSampleComboBox.Location.Y);
                reportSampleComboBox.Visible = true;
                reportIsotopeComboBox.Visible = false;
                searchReportBtn.Text = GlobalFunc.rm.GetString("download");
                searchReportBtn.AutoSize = true;
            }
            else if (reportTypeComboBox.SelectedIndex == 2)
            {
                reportPanel.Controls.Clear();
                reportIsotopeComboBox.DataSource = GlobalFunc.isotopeList1;
                reportIsotopeComboBox.DisplayMember = "Code";
                reportIsotopeComboBox.ValueMember = "Id_key";
                reportIsotopeComboBox.Width = DropDownWidth(reportIsotopeComboBox);
                reportSampleComboBox.Visible = false;
                reportIsotopeComboBox.Visible = true;
                reportIsotopeComboBox.Location = new Point(reportTypeComboBox.Location.X + reportTypeComboBox.Width + 5, reportTypeComboBox.Location.Y);
                searchReportBtn.Location = new Point(reportIsotopeComboBox.Location.X + reportIsotopeComboBox.Width + 5, reportIsotopeComboBox.Location.Y);
                searchReportBtn.Text = GlobalFunc.rm.GetString("download");
                searchReportBtn.AutoSize = true;
            }
        }

        public int DropDownWidth(ComboBox myCombo)
        {
            int maxWidth = 0;
            int temp = 0;
            Label label1 = new Label();
            label1.AutoSize = true;
            foreach (var obj in myCombo.Items)
            {
                label1.Text = obj.ToString();
                temp = label1.Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }
            label1.Dispose();
            return maxWidth + 20;
        }

        public int DropDownWidth2(ComboBox myCombo)
        {
            int maxWidth = 0, temp = 0;
            for (int i = 0; i < myCombo.Items.Count; i++)
            { 
                Location l = (Location)myCombo.Items[i];
                string text = l.DisplayText.code + ", " +l.DisplayText.description ;
                temp = TextRenderer.MeasureText(text, myCombo.Font).Width;
                if (temp > maxWidth)
                {
                    maxWidth = temp;
                }
            }

            return maxWidth;
        }

        private void abortBtn_Click(object sender, EventArgs e)
        {
            resetCalibration();
            LoadProfile();
            for (int i = 0; i < profileComboBox.Items.Count; i++)
            {
                Profile p = (Profile)profileComboBox.Items[i];
                if (p.ID == selectedProfileID)
                {
                    profileComboBox.SelectedIndex = i;
                }
            }
            LoadSelectedProfile(false);  
            abortBtn.Enabled = false;
        }

        private void meaMassUpdate_Click(object sender, EventArgs e)
        {
            string errorText = "";
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            if (sampleTime.Text != "")
            {   
                SqlCeCommand cmd = new SqlCeCommand("update time set sample = @sample", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@sample", sampleTime.Text);
                cmd.ExecuteNonQuery();
                
                //string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                errorText += "Sample Measuring Time can't be empty" + System.Environment.NewLine;
            }

            if (bkTime.Text != "")
            {
                SqlCeCommand cmd = new SqlCeCommand("update time set background = @background", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@background", bkTime.Text);
                cmd.ExecuteNonQuery();

                SqlCeCommand bkCmd = new SqlCeCommand("select background from time", con);
                bkCmd.CommandType = CommandType.Text;
                SqlCeDataReader bkDr = bkCmd.ExecuteReader();
                if (bkDr.Read())
                {
                    BKManager.supposeMillSecond = Convert.ToDouble(bkDr["background"]) * 60 * 1000;
                    BKManager.supposeSecond = Convert.ToDouble(bkDr["background"]) * 60;
                }
                bkDr.Close();
 
                //string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                errorText += "Background Measuring Time can't be empty" + System.Environment.NewLine;
            }

            if (warmUpTimeTxt.Text != "")
            {
                SqlCeCommand updateCmd = new SqlCeCommand("update time set warmupTime = @warmupTime", con);
                updateCmd.CommandType = CommandType.Text;
                updateCmd.Parameters.AddWithValue("@warmupTime", warmUpTimeTxt.Text);
                GlobalFunc.warmupTime = Convert.ToDouble(warmUpTimeTxt.Text);
                updateCmd.ExecuteNonQuery();
                updateCmd.Dispose();                
                //string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                errorText += "Warm up Time can't be empty" + System.Environment.NewLine;
            }

            if (alarmTime.Text != "")
            {
                SqlCeCommand cmd = new SqlCeCommand("update time set alarm = @alarm", con);
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.AddWithValue("@alarm", alarmTime.Text);
                cmd.ExecuteNonQuery();             
                //string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                errorText += "Total Count Alarm Level in CPS can't be empty" + System.Environment.NewLine;
            }

            if (topICRTxt.Text != "")
            {
                SqlCeCommand cmd = new SqlCeCommand("delete from icr where detector = 'Top'", con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                SqlCeCommand insertCmd = new SqlCeCommand("insert into icr values ('Top', @icr)", con);
                insertCmd.CommandType = CommandType.Text;
                insertCmd.Parameters.AddWithValue("@icr", topICRTxt.Text);
                GlobalFunc.det1_icr_alarm = Convert.ToDouble(topICRTxt.Text);
                insertCmd.ExecuteNonQuery();
                insertCmd.Dispose();

                //string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            }
            else
            {
                errorText += "Top Detector Input Count Rate Alarm Level (cps) can't be empty" + System.Environment.NewLine;
            }

            if (bottomICRTxt.Text != "")
            {
                SqlCeCommand cmd = new SqlCeCommand("delete from icr where detector = 'Bottom'", con);
                cmd.CommandType = CommandType.Text;
                cmd.ExecuteNonQuery();
                cmd.Dispose();
                SqlCeCommand insertCmd = new SqlCeCommand("insert into icr values ('Bottom', @icr)", con);
                insertCmd.CommandType = CommandType.Text;
                insertCmd.Parameters.AddWithValue("@icr", bottomICRTxt.Text);
                GlobalFunc.det2_icr_alarm = Convert.ToDouble(bottomICRTxt.Text);
                insertCmd.ExecuteNonQuery();
                insertCmd.Dispose();
              
                //string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            }
            else
            {
                errorText += "Bottom Detector Input Count Rate Alarm Level (cps) can't be empty" + System.Environment.NewLine;
            }

            if (bufferTimeTxt.Text != "")
            {

                SqlCeCommand updateCmd = new SqlCeCommand("update time set bufferTime = @bufferTime", con);
                updateCmd.CommandType = CommandType.Text;
                updateCmd.Parameters.AddWithValue("@bufferTime", bufferTimeTxt.Text);
                GlobalFunc.bufferTime = Convert.ToInt32(bufferTimeTxt.Text);
                updateCmd.ExecuteNonQuery();
                updateCmd.Dispose();
               
                //string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                errorText += "Buffer Time can't be empty";
            }

            int rsl = 0;
            if (rsEnable.Checked == true)
            {
                rsl = 1;
            }
            else if (rsDisable.Checked == true)
            {
                rsl = 0;
            }

            SqlCeCommand rsCmd = new SqlCeCommand("update time set rsl = @rsl", con);
            rsCmd.CommandType = CommandType.Text;
            rsCmd.Parameters.AddWithValue("@rsl", rsl);
            GlobalFunc.rsl = rsl;
            rsCmd.ExecuteNonQuery();
            rsCmd.Dispose();
            
            //string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            int ru = 0;
            if (ruEnable.Checked == true)
            {
                ru = 1;
            }
            else if (ruDisable.Checked == true)
            {
                ru = 0;
            }

            rsCmd = new SqlCeCommand("update time set ru = @ru", con);
            rsCmd.CommandType = CommandType.Text;
            rsCmd.Parameters.AddWithValue("@ru", ru);
            GlobalFunc.ru = ru;
            rsCmd.ExecuteNonQuery();
            rsCmd.Dispose();
            
            //string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);

            int mlr = 0;
            if (mlrEnable.Checked == true)
            {
                mlr = 1;
            }
            else if (mlrDisable.Checked == true)
            {
                mlr = 0;
            }

            rsCmd = new SqlCeCommand("update time set mlr = @mlr", con);
            rsCmd.CommandType = CommandType.Text;
            rsCmd.Parameters.AddWithValue("@mlr", mlr);
            GlobalFunc.mlr = mlr;
            rsCmd.ExecuteNonQuery();
            rsCmd.Dispose();

            if (errorText != "")
            {
                string btnClicked = CustomMessageBox.Show(errorText, GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }
            else
            {
                string btnClicked = CustomMessageBox.Show(GlobalFunc.rm.GetString("save"), GlobalFunc.rm.GetString("noticeMsg"), GlobalFunc.rm.GetString("yesBtnTxt"), "", false, 0);
            }

        }

        private void smTab_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (smTab.SelectedTab == smTab.TabPages["tabPage3"])//your specific tabname
            {
                if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "top")
                {
                    detectorComboBox.SelectedIndex = 0;
                    detectorComboBox.Enabled = false;
                    roiPathText2.Enabled = false;
                    selectRoiPathBtn2.Enabled = false;
                }
                else if (GlobalFunc.basicSetting.PresetDetector.ToLower() == "bottom")
                {
                    detectorComboBox.SelectedIndex = 1;
                    roiPathText1.Enabled = false;
                    selectRoiPathBtn1.Enabled = false;
                    detectorComboBox.Enabled = false;
                }
                else
                {
                    detectorComboBox.SelectedIndex = 2;
                    detectorComboBox.Enabled = false;
                }
            }
        }


    }
}
