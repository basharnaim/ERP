using AutoMapper;
using ERP.WebUI.ReportViewer;
using Library.Context.Repositories;
using Library.Model.Core.Core;
using Library.Service.Inventory.Purchases;
using Library.ViewModel.Inventory.Products;
using Library.ViewModel.Inventory.Purchases;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class SuperShopPurchaseController : Controller
    {
        #region Ctor
        private readonly IPurchaseService _purchaseService;
        private readonly IRawSqlService _rawSqlService;
        public SuperShopPurchaseController(IPurchaseService purchaseService, IRawSqlService rawSqlService)
        {
            _purchaseService = purchaseService;
            _rawSqlService = rawSqlService;
        }

        #endregion

        #region Rdlc Report
        public ActionResult PurchaseSummary(string companyId, string branchId, string dateFrom, string dateTo, string supplierId)
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
                IEnumerable<SuperShopPurchaseViewModel> purchases = new List<SuperShopPurchaseViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    purchases = Mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_rawSqlService.GetAllPurchaseSummary(companyId, branchId, dateFrom, dateTo, supplierId));
                }
                return View(purchases);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReportPurchaseSummary(string companyId, string branchId, string dateFrom, string dateTo, string supplierId)
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
                if (supplierId == "null")
                {
                    supplierId = "";
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
                IEnumerable<SuperShopPurchaseViewModel> purchases = new List<SuperShopPurchaseViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    purchases = Mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_rawSqlService.GetAllPurchaseSummary(companyId, branchId, dateFrom, dateTo, supplierId));
                }
                ReportDataSource rpt = new ReportDataSource("Purchase", purchases);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptPurchaseSummary.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult PurchaseList(string companyId, string branchId, string dateFrom, string dateTo, string supplierId)
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
                IEnumerable<SuperShopPurchaseDetailViewModel> purchaseDetails = new List<SuperShopPurchaseDetailViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    purchaseDetails = Mapper.Map<IEnumerable<SuperShopPurchaseDetailViewModel>>(_rawSqlService.GetAllPurchaseDetail(companyId, branchId, dateFrom, dateTo, supplierId));
                }
                return View(purchaseDetails);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReportPurchaseList(string companyId, string branchId, string dateFrom, string dateTo, string supplierId)
        {
            try
            {
                if (companyId == "null")
                {
                    companyId = "";
                }
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
                if (supplierId == "null")
                {
                    supplierId = "";
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
                IEnumerable<SuperShopPurchaseDetailViewModel> purchaseDetails = new List<SuperShopPurchaseDetailViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    purchaseDetails = Mapper.Map<IEnumerable<SuperShopPurchaseDetailViewModel>>(_rawSqlService.GetAllPurchaseDetail(companyId, branchId, dateFrom, dateTo, supplierId));
                }
                ReportDataSource rpt = new ReportDataSource("PurchaseDetail", purchaseDetails);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptPurchaseList.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ReportPurchaseMasterDetail(string companyId, string branchId, string purchaseId)
        {
            try
            {
                var master = Mapper.Map<IEnumerable<PurchaseViewModelForReport>>(_purchaseService.GetAllForReport(purchaseId));
                var detail = Mapper.Map<IEnumerable<PurchaseDetailViewModelForReport>>(_purchaseService.GetAllPurchaseDetailbyMasterIdForReport(purchaseId));
                ReportDataSource rpt1 = new ReportDataSource("Purchase", master);
                ReportDataSource rpt2 = new ReportDataSource("PurchaseDetail", detail);
                List<ReportDataSource> rptl = new List<ReportDataSource> { rpt1, rpt2 };
                RdlcReportViewerMultipleDataSet.reportDataSource = rptl;
                string rPath = "RdlcReport/RptPurchaseInvoice.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerMultipleDataSet.aspx?rPath=" + rPath + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult ReportPurchaseSummary(string companyId, string branchId, string supplierId, string dateFrom, string dateTo)
        {
            try
            {
                var dataSet = _rawSqlService.GetPurchaseSummary(companyId, branchId, supplierId, dateFrom, dateTo);                
                return View(dataSet);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReport_PurchaseSummary(string companyId, string branchId, string supplierId, string fdate, string todate)
        {
            try
            {
                var dataSet = new DataSet();
                dataSet = _rawSqlService.GetPurchaseSummary(companyId, branchId, supplierId, fdate, todate);
                ReportDataSource rpt = new ReportDataSource("Purchase", dataSet.Tables[0]);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptPurchaseSummary.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + fdate + "&dto=" + todate + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
                //ReportDataSource rpt = new ReportDataSource("Purchase", dataSet.Tables[0]);
                //RdlcReportViewerWithDate.reportDataSource = rpt;
                //string rPath = "RdlcReport/RptCategoryWiseDailySales.rdlc";
                //Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + fdate + "&dto=" + todate + "&companyId=" + companyId + "&branchId=" + branchId);
                //return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        public ActionResult RdlcReport_Reorder(string companyId, string branchId, string categoryId, string subCategoryId, string supplierId, string productId, string productCode)
        {
            try
            {                
                var dataSet = _rawSqlService.GetProductReorder(companyId, branchId, categoryId, subCategoryId, supplierId, productId, productCode);
                ReportDataSource rpt = new ReportDataSource("Product", dataSet.Tables["ProductReorder"]);
                RdlcReportViewerWithoutDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptProductListReorder.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithoutDate.aspx?rPath=" + rPath + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult ReorderList(string companyId, string branchId, string productCategoryId, string productSubCategoryId, string supplierId, string productId, string productCode)
        {
            try
            {               
                return View(_rawSqlService.GetProductReorder(companyId, branchId, productCategoryId, productSubCategoryId, supplierId, productId, productCode));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult ExpireList(string companyId, string branchId, string productCategoryId, string productSubCategoryId, string supplierId, string productId, string productCoce, string expireDate, string isExpired)
        {
            try
            {
                return View(_rawSqlService.GetProductExpire(companyId, branchId, productCategoryId, productSubCategoryId, supplierId, productId, productCoce, expireDate, isExpired));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult RdlcReport_ExpireList(string companyId, string branchId, string productCategoryId, string productSubCategoryId, string supplierId, string productId, string productCoce, string expireDate, string isExpired)
        {
            try
            {
                var data = _rawSqlService.GetProductExpire(companyId, branchId, productCategoryId, productSubCategoryId, supplierId, productId, productCoce, expireDate, isExpired);
                ReportDataSource rpt = new ReportDataSource("Product", data.Tables["ProductExpire"]);
                RdlcReportViewerWithoutDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptProductListExpire.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithoutDate.aspx?rPath=" + rPath + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
    }
}