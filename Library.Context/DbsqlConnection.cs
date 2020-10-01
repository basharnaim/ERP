using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Library.Context
{
    public class DbsqlConnection
    {
        public SqlConnection GetConnection()
        {
            string connectionString = new AppSettingsReader().GetValue("dbConnectionStrings", typeof(string)).ToString();
            string typePrefix1 = new AppSettingsReader().GetValue("PREFIX_PROCUREMENT_ERF", typeof(string)).ToString();
            SqlConnection conn = new SqlConnection("");
            //tt++;
            //Debug.WriteLine(tt.ToString());
            //string ConnectionString = "Data Source=192.168.15.1\\SQLEXPRESS;Initial Catalog=VATSample_DB;user id=sa;password=Sa123456_ ;connect Timeout=600;";
            //string ConnectionString = "Data Source=" + SysDBInfoVM.SysdataSource + ";Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + SysDBInfoVM.SysUserName
            //    + ";password=" + SysDBInfoVM.SysPassword + ";connect Timeout=600;";

            //string ConnectionString = "Data Source=120;Initial Catalog=" + DatabaseInfoVM.DatabaseName + ";user id=" + DatabaseInfoVM.dbUserName
            //    + ";password=" + DatabaseInfoVM.dbPassword + ";connect Timeout=600;";
            // string conn = System.Configuration.ConfigurationSettings.AppSettings["VATDB"];
            //string conn = System.Configuration.ConfigurationManager.AppSettings["VATDB"].ToString();
            if (DateTime.Now <= Convert.ToDateTime("10/10/2015"))
            {
                 conn = new SqlConnection(connectionString);
            }

            return conn;
        }

        public SqlConnection GetMobileConnection()
        {
            string connectionString = new AppSettingsReader().GetValue("MobiledbConnectionStrings", typeof(string)).ToString();
            SqlConnection conn = new SqlConnection("");

            if (DateTime.Now <= Convert.ToDateTime("10/10/2015"))
            {
                conn = new SqlConnection(connectionString);
            }

            return conn;
        }
    }
}