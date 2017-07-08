using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using System.Data.Sql;
using System.Data.SqlServerCe;
namespace LCMS
{
    public class LogManager
    {
        public string logPath = "";
        public string userLogPath = "";
        public LogManager() { }

        public void CreateLogFile() 
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\Log"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\Log");
            }
            string date = DateTime.Now.Year.ToString() + "_" +  DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString();
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\Log\" + date + ".log"))
            {
                StreamWriter temp = File.CreateText(Directory.GetCurrentDirectory() + @"\Log\" + date + ".log");
                temp.Close();
                //File.Create(Directory.GetCurrentDirectory() + @"\Log\" + date + ".log");
            }
            logPath = Directory.GetCurrentDirectory() + @"\Log\" + date + ".log";
        }

        public void CreateUserLogFile()
        {
            if (!Directory.Exists(Directory.GetCurrentDirectory() + @"\userLog"))
            {
                Directory.CreateDirectory(Directory.GetCurrentDirectory() + @"\userLog");
            }
            string date = DateTime.Now.Year.ToString() + "_" + DateTime.Now.Month.ToString() + "_" + DateTime.Now.Day.ToString();
            if (!File.Exists(Directory.GetCurrentDirectory() + @"\userLog\" + date + ".log"))
            {
                StreamWriter temp = File.CreateText(Directory.GetCurrentDirectory() + @"\userLog\" + date + ".log");
                temp.Close();
            }
            userLogPath = Directory.GetCurrentDirectory() + @"\userLog\" + date + ".log";
        }

        public void WriteLog(string action)
        {
            StreamWriter sw = File.AppendText(logPath);
            try
            {
                sw.WriteLine(DateTime.Now.ToString() + " " + action);
                sw.Close();
            }
            catch (Exception)
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }

        public void WriteUserLog(string action)
        {
            StreamWriter sw = File.AppendText(userLogPath);
            try
            {
                sw.WriteLine(DateTime.Now.ToString() + " " + action);
                sw.Close();
            }
            catch (Exception)
            {
                if (sw != null)
                {
                    sw.Close();
                }
            }
        }
        
        SqlCeConnection con = new SqlCeConnection(Properties.Settings.Default.lcmsConnectionString);
        public int SaveCurrentJob(string jobname) // 1 = running, 2 = finish
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand insertCmd = new SqlCeCommand("insert into job (jobName, date, status) values (@jobName, GETDATE(), 1)", con);
            insertCmd.CommandType = CommandType.Text;
            insertCmd.Parameters.AddWithValue("@jobName", jobname);
            insertCmd.ExecuteNonQuery();
            insertCmd.Dispose();

            SqlCeCommand idCmd = new SqlCeCommand("select top 1 jobID from job order by jobID desc", con);
            idCmd.CommandType = CommandType.Text;
            int jobID = Convert.ToInt32(idCmd.ExecuteScalar());
            con.Close();
            return jobID;
        }

        public void UpdateJobStatus(int jobID)
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand updCmd = new SqlCeCommand("update job set status = 2 where jobID = @jobID", con);
            updCmd.CommandType = CommandType.Text;
            updCmd.Parameters.AddWithValue("@jobID", jobID);
            updCmd.ExecuteNonQuery();
            updCmd.Dispose();
            con.Close();
        }

        public void FindPreviousJob()
        {
            if (con.State == ConnectionState.Open)
            {
                con.Close();
            }
            con.Open();
            SqlCeCommand cmd = new SqlCeCommand("select * from job where status = 1", con);
            cmd.CommandType = CommandType.Text;
            SqlCeDataReader dr = cmd.ExecuteReader();
            while (dr.Read())
            {
                if (dr["jobName"].ToString() == "Background Operation")                 
                {
                    GlobalFunc.showGetBKDataBtn = true;
                    BKManager.jobID = Convert.ToInt32(dr["jobID"]);
                }
            }
            dr.Close();
            con.Close();
        }
    }
}
