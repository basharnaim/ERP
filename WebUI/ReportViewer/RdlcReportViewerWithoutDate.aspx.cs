using Library.Service.Core.Organizations;
using Microsoft.Reporting.WebForms;
using System;
using System.Web.UI;
using Unity;

namespace ERP.WebUI.ReportViewer
{
    public partial class RdlcReportViewerWithoutDate : Page
    {
        public static ReportDataSource reportDataSource = new ReportDataSource();
        private static readonly IUnityContainer Ucontainer = UnityConfig.Container;
        private readonly ICompanyService _companyService = Ucontainer.Resolve<ICompanyService>();
        private readonly IBranchService _branchService = Ucontainer.Resolve<IBranchService>();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                ReportViewer1.ProcessingMode = ProcessingMode.Local;
                ReportViewer1.LocalReport.ReportPath = Request.QueryString["rPath"];
                string companyId = Request.QueryString["companyId"];
                string branchId = Request.QueryString["branchId"];
                if (!string.IsNullOrEmpty(companyId))
                {
                    string companyName = _companyService.GetById(companyId)?.Name;
                    ReportParameter[] param = new ReportParameter[1];
                    param[0] = new ReportParameter("cName", companyName);
                    ReportViewer1.LocalReport.SetParameters(param);
                }
                else
                {
                    ReportParameter[] param = new ReportParameter[1];
                    param[0] = new ReportParameter("cName", ".");
                    ReportViewer1.LocalReport.SetParameters(param);
                }
                if (!string.IsNullOrEmpty(branchId))
                {
                    var branch = _branchService.GetById(branchId);
                    string branchName = branch?.Name;
                    string branchPhone = branch?.Phone1;
                    string branchVatRegistryNo = branch?.VatRegistryNo ?? ".";
                    string branchAddress = branch?.Address1;
                    ReportParameter[] param = new ReportParameter[4];
                    param[0] = new ReportParameter("bName", branchName);
                    param[1] = new ReportParameter("bPhone", branchPhone);
                    param[2] = new ReportParameter("bVatRNo", branchVatRegistryNo);
                    param[3] = new ReportParameter("bAddress", branchAddress);
                    ReportViewer1.LocalReport.SetParameters(param);
                }
                else
                {
                    ReportParameter[] param = new ReportParameter[4];
                    param[0] = new ReportParameter("bName", ".");
                    param[1] = new ReportParameter("bPhone", ".");
                    param[2] = new ReportParameter("bVatRNo", ".");
                    param[3] = new ReportParameter("bAddress", ".");
                    ReportViewer1.LocalReport.SetParameters(param);
                }
                ReportViewer1.LocalReport.DataSources.Clear();
                ReportViewer1.LocalReport.DataSources.Add(reportDataSource);
                ReportViewer1.LocalReport.Refresh();
            }
        }
    }
}