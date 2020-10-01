using Microsoft.Reporting.WebForms;
using System;
using System.Web.UI;

namespace ERP.WebUI.ReportViewer
{
    public partial class RdlcBarcodeReportViewer : Page
    {
        public static ReportDataSource reportDataSource = new ReportDataSource();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Request.QueryString["rPath"];
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                ReportViewer1.LocalReport.Refresh();
            }
        }
    }
}