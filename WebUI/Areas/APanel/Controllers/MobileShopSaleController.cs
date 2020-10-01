using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using ERP.WebUI.ReportViewer;
using Library.Context.Repositories;
using Library.Service.Inventory.Sales;
using Library.ViewModel.Inventory.Sales;
using Microsoft.Reporting.WebForms;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class MobileShopSaleController : Controller
    {
        #region Ctor
        private readonly ISaleService _saleService;
        private readonly IRawSqlService _rawSqlService;
        public MobileShopSaleController(
            ISaleService saleService,
            IRawSqlService rawSqlService
            )
        {
            _saleService = saleService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        #region Rdlc Report
        public ActionResult ReportSaleSummary(string companyId, string branchId, string dateFrom, string dateTo, string customerId)
        {
            try
            {
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    DateTime? dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                    dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    DateTime? dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                    dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
                }
                IEnumerable<MobileShopSaleViewModel> sales = new List<MobileShopSaleViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    sales = AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_rawSqlService.GetAllSalesSummary(companyId, branchId, dateFrom, dateTo, customerId));
                }
                return View(sales);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReportSaleSummary(string companyId, string branchId, string dateFrom, string dateTo, string customerId)
        {
            try
            {
                if (branchId == "null")
                {
                    branchId = "";
                }
                if (dateFrom == "null")
                {
                    dateFrom = "";
                }
                if (dateTo == "null")
                {
                    dateTo = "";
                }
                if (customerId == "null")
                {
                    customerId = "";
                }
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    DateTime? dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                    dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    DateTime? dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                    dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
                }
                IEnumerable<MobileShopSaleViewModel> sales = new List<MobileShopSaleViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    sales = AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_rawSqlService.GetAllSalesSummary(companyId, branchId, dateFrom, dateTo, customerId));
                }
                ReportDataSource rpt = new ReportDataSource("Sale", sales);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptSaleSummary.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult SaleList(string companyId, string branchId, string customerId, string saleNo, string productId, string dateFrom, string dateTo)
        {
            try
            {
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    DateTime? dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                    dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    DateTime? dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                    dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopSaleDetailViewModel>>(_rawSqlService.GetAllSaleDetail(companyId, branchId, customerId, saleNo, productId, dateFrom, dateTo)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReportSaleList(string companyId, string branchId, string customerId, string saleNo, string itemId, string dateFrom, string dateTo)
        {
            try
            {
                if (branchId == "null")
                {
                    branchId = "";
                }
                if (customerId == "null")
                {
                    customerId = "";
                }
                if (saleNo == "null")
                {
                    saleNo = "";
                }
                if (itemId == "null")
                {
                    itemId = "";
                }
                if (dateFrom == "null")
                {
                    dateFrom = "";
                }
                if (dateTo == "null")
                {
                    dateTo = "";
                }
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    DateTime? dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                    dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    DateTime? dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                    dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
                }
                IEnumerable<MobileShopSaleDetailViewModel> salesDetail = new List<MobileShopSaleDetailViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    salesDetail = AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopSaleDetailViewModel>>(_rawSqlService.GetAllSaleDetail(companyId, branchId, customerId, saleNo, itemId, dateFrom, dateTo));
                }
                ReportDataSource rpt = new ReportDataSource("SaleDetail", salesDetail);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptSaleList.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ReportSaleMasterDetail(string companyId, string branchId, string id)
        {
            try
            {
                var master = AutoMapperConfiguration.mapper.Map<IEnumerable<SaleViewModelForReport>>(_saleService.GetAllForReport(id));
                var detail = AutoMapperConfiguration.mapper.Map<IEnumerable<SaleDetailViewModelForReport>>(_saleService.GetAllSaleDetailbyMasterIdForReport(id));
                ReportDataSource rpt1 = new ReportDataSource("Sale", master);
                ReportDataSource rpt2 = new ReportDataSource("SaleDetail", detail);
                List<ReportDataSource> rptl = new List<ReportDataSource> { rpt1, rpt2 };
                RdlcReportViewerMultipleDataSet.reportDataSource = rptl;
                string rPath = "RdlcReport/RptSalesInvoice.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerMultipleDataSet.aspx?rPath=" + rPath + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult DailySalesReport(string companyId, string branchId, string dateFrom, string dateTo)
        {
            try
            {
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    DateTime? dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                    dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    DateTime? dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                    dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_rawSqlService.GetCashBalanceBetweenDate(companyId, branchId, dateFrom, dateTo)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcDailySalesReport(string companyId, string branchId, string dateFrom, string dateTo)
        {
            try
            {
                if (branchId == "null")
                {
                    branchId = "";
                }
                if (dateFrom == "null")
                {
                    dateFrom = "";
                }
                if (dateTo == "null")
                {
                    dateTo = "";
                }
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    DateTime? dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                    dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    DateTime? dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                    dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
                }
                IEnumerable<MobileShopSaleViewModel> salesList = new List<MobileShopSaleViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    salesList = AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_rawSqlService.GetCashBalanceBetweenDate(companyId, branchId, dateFrom, dateTo));
                }
                ReportDataSource rpt = new ReportDataSource("Sale", salesList);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptCashBalanceReport.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}