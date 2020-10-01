using System;
using System.Web.UI;
using Library.Service.Core.Organizations;
using Microsoft.Reporting.WebForms;
using Unity;

namespace ERP.WebUI.ReportViewer
{
    public partial class RdlcReportViewerBranchWise : Page
    {
        public static ReportDataSource reportDataSource = new ReportDataSource();
        private static readonly IUnityContainer Ucontainer = UnityConfig.Container;
        private readonly IBranchService _branchService = Ucontainer.Resolve<IBranchService>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Request.QueryString["rPath"];
                string branchId = Request.QueryString["branchId"];
                if (!string.IsNullOrEmpty(branchId))
                {
                    var branch = _branchService.GetById(branchId);
                    string branchName = branch?.Name;
                    string branchPhone = branch?.Phone1;
                    ReportParameter[] param = new ReportParameter[2];
                    param[0] = new ReportParameter("bName", branchName);
                    param[1] = new ReportParameter("bPhone", branchPhone);
                    ReportViewer1.LocalReport.SetParameters(param);
                }
                else
                {
                    ReportParameter[] param = new ReportParameter[2];
                    param[0] = new ReportParameter("bName", ".");
                    param[1] = new ReportParameter("bPhone", ".");
                    ReportViewer1.LocalReport.SetParameters(param);
                }
                string dfrom = Request.QueryString["dfrom"];
                string dto = Request.QueryString["dto"];
                if (!string.IsNullOrEmpty(dfrom) && !string.IsNullOrEmpty(dto))
                {
                    ReportParameter[] param = new ReportParameter[2];
                    param[0] = new ReportParameter("dfrom", dfrom);
                    param[1] = new ReportParameter("dto", dto);
                    ReportViewer1.LocalReport.SetParameters(param);
                }
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                ReportViewer1.LocalReport.Refresh();
            }
        }
    }
}