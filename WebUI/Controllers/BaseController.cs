using ERP.WebUI.Filters;
using System;
using System.Data;
using System.IO;
using System.Web.Configuration;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
{
    [CustomAuthorize]
    public class BaseController : Controller
    {
        
       

        public void WriteExcelFile(DataTable dt, string fileName)
        {
            try
            {
                var directoryLocation = WebConfigurationManager.AppSettings["ErrorFilePath"];
                if (!Directory.Exists(directoryLocation))
                {
                    Directory.CreateDirectory(directoryLocation);
                }
                var wr = new StreamWriter(directoryLocation + fileName + ".xls");
                for (var i = 0; i < dt.Columns.Count; i++)
                {
                    wr.Write(dt.Columns[i].ToString().ToUpper() + "\t");
                }
                wr.WriteLine();

                //write rows to excel file
                for (var i = 0; i < dt.Rows.Count; i++)
                {
                    for (var j = 0; j < dt.Columns.Count; j++)
                    {
                        if (dt.Rows[i][j] != null)
                        {
                            wr.Write(Convert.ToString(dt.Rows[i][j]) + "\t");
                        }
                        else
                        {
                            wr.Write("\t");
                        }
                    }
                    //go to next line
                    wr.WriteLine();
                }
                //close file
                wr.Close();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}