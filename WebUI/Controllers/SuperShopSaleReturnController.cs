#region Using

using ERP.WebUI.ReportViewer;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Sales;
using Library.Context.Repositories;
using Library.Service.Inventory.Sales;
using Library.ViewModel.Inventory.Sales;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using AutoMapper;

#endregion

namespace ERP.WebUI.Controllers
{
    public class SuperShopSaleReturnController : BaseController
    {
        #region Ctor
        private readonly ISaleReturnService _saleReturnService;
        private readonly ISaleService _saleService;
        private readonly IRawSqlService _rawSqlService;
        public SuperShopSaleReturnController(
            ISaleService saleService,
            ISaleReturnService saleReturnService,
            IRawSqlService rawSqlService
            )
        {
            _saleService = saleService;
            _saleReturnService = saleReturnService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        public ActionResult Index(string invoiceNo, string customerId, string phone, string dateFrom, string dateTo)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                DateTime? dfrom = null;
                DateTime? dto = null;
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                }
                if (!string.IsNullOrEmpty(invoiceNo) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_saleService.GetAllInvoiceByInvoiceNoWithDate(identity.CompanyId, identity.BranchId, invoiceNo, dfrom.Value, dto.Value)));
                }
                if (!string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_saleService.GetAllInvoiceByCustomerNameWithDate(identity.CompanyId, identity.BranchId, customerId, dfrom.Value, dto.Value)));
                }
                if (!string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_saleService.GetAllInvoiceByCustomerPhoneWithDate(identity.CompanyId, identity.BranchId, phone, dfrom.Value, dto.Value)));
                }
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_saleService.GetAllInvoiceByCompanyBranchIdWithDateRange(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value)));
                }
                if (!string.IsNullOrEmpty(invoiceNo))
                {
                    return View(Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_saleService.GetAllInvoiceByInvoiceNo(identity.CompanyId, identity.BranchId, invoiceNo)));
                }
                if (!string.IsNullOrEmpty(customerId))
                {
                    return View(Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_saleService.GetAllInvoiceByCompanyBranchCustomerId(identity.CompanyId, identity.BranchId, customerId)));
                }
                if (!string.IsNullOrEmpty(phone))
                {
                    return View(Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_saleService.GetAllInvoiceByCustomerPhone(identity.CompanyId, identity.BranchId, phone)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        #region Create
        [HttpGet]
        public ActionResult Create(string saleId)
        {
            try
            {
                SaleReturnViewModel saleReturnVm = new SaleReturnViewModel();
                List<SaleReturnDetailViewModel> saleReturnDetails = new List<SaleReturnDetailViewModel>();
                SuperShopSaleViewModel saleVm = Mapper.Map<SuperShopSaleViewModel>(_saleService.GetById(saleId));
                List<SuperShopSaleDetailViewModel> saleDetails = Mapper.Map<List<SuperShopSaleDetailViewModel>>(_saleService.GetAllSaleDetailbyMasterId(saleId).ToList());
                if (saleDetails.Any())
                {
                    foreach (var saleDetail in saleDetails)
                    {
                        SaleReturnDetailViewModel saleReturnDetail = new SaleReturnDetailViewModel
                        {
                            Select = true,
                            SaleId = saleDetail.SaleId,
                            SaleDetailId = saleDetail.Id,
                            ProductId = saleDetail.ProductId,
                            ProductCode = saleDetail.ProductCode,
                            ProductName = saleDetail.ProductName,
                            SoldQuantity = saleDetail.Quantity,
                            SoldAmount = saleDetail.TotalAmount,
                            UOMId = saleDetail.UOMId,
                            UOMName = saleDetail.UOMName,
                            RemainingQuantity = saleDetail.Quantity - _saleReturnService.GetSumOfAlreadyReturnQty(saleDetail.SaleId, saleDetail.ProductId),
                            SalePrice = saleDetail.SalePrice,
                            ReturnUnitPrice = saleDetail.SalePrice,
                            CompanyId = saleDetail.CompanyId,
                            BranchId = saleDetail.BranchId,
                            SupplierId= saleDetail.SupplierId,
                        };
                        
                        saleReturnDetail.ReturnQuantity = saleReturnDetail.RemainingQuantity;
                        saleReturnDetail.ReturnAmount = saleDetail.SalePrice * saleReturnDetail.ReturnQuantity;
                        saleReturnDetails.Add(saleReturnDetail);
                    }
                }
                saleReturnVm.SelectAll = true;
                saleReturnVm.SaleId = saleVm.Id;
                saleReturnVm.SaleDate = saleVm.SaleDate;
                saleReturnVm.CustomerId = saleVm.CustomerId;
                saleReturnVm.CustomerName = saleVm.CustomerName;
                saleReturnVm.CompanyId = saleVm.CompanyId;
                saleReturnVm.BranchId = saleVm.BranchId;
                saleReturnVm.SalesReturnDate = DateTime.Now;
                saleReturnVm.SaleReturnDetails = new List<SaleReturnDetailViewModel>();
                saleReturnVm.SaleReturnDetails.AddRange(saleReturnDetails);
                return View(saleReturnVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(SaleReturnViewModel saleReturnvm, string Type)
        {
            try
            {
                if (saleReturnvm.SaleReturnDetails.Any(x => x.Select && x.ReturnQuantity > 0))
                {
                    SaleReturn saleReturn = Mapper.Map<SaleReturn>(saleReturnvm);
                    List<SaleReturnDetail> saleReturnDetailList = Mapper.Map<List<SaleReturnDetail>>(saleReturnvm.SaleReturnDetails.Where(x => x.Select && x.ReturnQuantity > 0));
                    saleReturn.SaleReturnDetails = new List<SaleReturnDetail>();
                    foreach (var item in saleReturnDetailList)
                    {
                        saleReturn.SaleReturnDetails.Add(item);
                    }
                    _saleReturnService.Add(saleReturn, Type);
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/SuperShopSaleReturn/Index?companyId=" + saleReturn.CompanyId + "&&branchId=" + saleReturn.BranchId}')");
                }
                return JavaScript($"ShowResult('{"Enter quantity!"}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Rdlc Report
        public ActionResult ReportSaleReturnSummary(string dateFrom, string dateTo, string customerId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                DateTime? dfrom = null;
                DateTime? dto = null;
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                }
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<SaleReturnViewModel>>(_rawSqlService.GetAllSaleReturnSummary(identity.CompanyId, identity.BranchId, dateFrom, dateTo, customerId)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult ReportPurchaseMasterDetail(string id)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var master = Mapper.Map<IEnumerable<SaleViewModelForReport>>(_saleService.GetAllForReport(id)).ToList();
                var detail = Mapper.Map<IEnumerable<SaleDetailViewModelForReport>>(_saleService.GetAllSaleDetailbyMasterIdForReport(id).ToList()).ToList();
                ReportDataSource rpt1 = new ReportDataSource("Sale", master);
                ReportDataSource rpt2 = new ReportDataSource("SaleDetail", detail);
                List<ReportDataSource> rptl = new List<ReportDataSource> { rpt1, rpt2 };
                RdlcReportViewerMultipleDataSet.reportDataSource = rptl;
                string rPath = "RdlcReport/RptSalesInvoiceForMobileshop.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerMultipleDataSet.aspx?rPath=" + rPath + "&companyId=" + identity.CompanyId + "&branchId=" + identity.BranchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult ReportLevelPrint(string id)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var master = Mapper.Map<IEnumerable<SaleViewModelForReport>>(_saleReturnService.GetAllForReport(id)).ToList();
                var detail = Mapper.Map<IEnumerable<SaleDetailViewModelForReport>>(_saleReturnService.GetAllSaleDetailbyMasterIdForReport(id).ToList()).ToList();
                ReportDataSource rpt1 = new ReportDataSource("Sale", master);
                ReportDataSource rpt2 = new ReportDataSource("SaleDetail", detail);
                List<ReportDataSource> rptl = new List<ReportDataSource> { rpt1, rpt2 };
                RdlcReportViewerMultipleDataSet.reportDataSource = rptl;
                string rPath = "RdlcReport/RptSalesReturnInvoiceForSuperShop.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerMultipleDataSet.aspx?rPath=" + rPath + "&branchId=" + identity.BranchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        //public ActionResult RdlcReportPurchaseSummary(string dateFrom, string dateTo, string supplierId)
        //{
        //    try
        //    {
        //        if (dateFrom == "null")
        //        {
        //            dateFrom = "";
        //        }
        //        if (dateTo == "null")
        //        {
        //            dateTo = "";
        //        }
        //        if (supplierId == "null")
        //        {
        //            supplierId = "";
        //        }
        //        if (!string.IsNullOrEmpty(dateFrom))
        //        {
        //            DateTime? dfrom = Convert.ToDateTime(dateFrom);
        //            dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
        //            dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
        //        }
        //        if (!string.IsNullOrEmpty(dateTo))
        //        {
        //            DateTime? dto = Convert.ToDateTime(dateTo);
        //            dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
        //            dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
        //        }
        //        var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
        //        IEnumerable<SuperShopSaleViewModel> purchases = new List<SuperShopSaleViewModel>();
        //        if (!string.IsNullOrEmpty(dateFrom))
        //        {
        //            purchases = Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_rawSqlService.GetAllPurchaseSummary(identity.CompanyId.ToString(), identity.BranchId.ToString(), dateFrom, dateTo, supplierId));
        //        }
        //        ReportDataSource rpt = new ReportDataSource("Purchase", purchases);
        //        RdlcReportViewer.reportDataSource = rpt;
        //        string rPath = "RdlcReport/RptPurchaseSummary.rdlc";
        //        Response.Redirect("~/ReportViewer/RdlcReportViewer.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + identity.CompanyId + "&branchId=" + identity.BranchId);
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        return JavaScript($"ShowResult('{ex.Message}','failure')");
        //    }
        //}

        //public ActionResult PurchaseList(string dateFrom, string dateTo, string supplierId)
        //{
        //    try
        //    {
        //        if (!string.IsNullOrEmpty(dateFrom))
        //        {
        //            DateTime? dfrom = Convert.ToDateTime(dateFrom);
        //            dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
        //            dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
        //        }
        //        if (!string.IsNullOrEmpty(dateTo))
        //        {
        //            DateTime? dto = Convert.ToDateTime(dateTo);
        //            dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
        //            dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
        //        }
        //        var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
        //        IEnumerable<SuperShopPurchaseDetailViewModel> purchaseDetails = new List<SuperShopPurchaseDetailViewModel>();
        //        if (!string.IsNullOrEmpty(dateFrom))
        //        {
        //            purchaseDetails = Mapper.Map<IEnumerable<SuperShopPurchaseDetailViewModel>>(_rawSqlService.GetAllPurchaseDetail(identity.CompanyId.ToString(), identity.BranchId.ToString(), dateFrom, dateTo, supplierId));
        //        }
        //        return View(purchaseDetails);
        //    }
        //    catch (Exception ex)
        //    {
        //        return JavaScript($"ShowResult('{ex.Message}','failure')");
        //    }
        //}

        //public ActionResult RdlcReportPurchaseList(string dateFrom, string dateTo, string supplierId)
        //{
        //    try
        //    {
        //        if (dateFrom == "null")
        //        {
        //            dateFrom = "";
        //        }
        //        if (dateTo == "null")
        //        {
        //            dateTo = "";
        //        }
        //        if (supplierId == "null")
        //        {
        //            supplierId = "";
        //        }
        //        if (!string.IsNullOrEmpty(dateFrom))
        //        {
        //            DateTime? dfrom = Convert.ToDateTime(dateFrom);
        //            dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
        //            dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
        //        }
        //        if (!string.IsNullOrEmpty(dateTo))
        //        {
        //            DateTime? dto = Convert.ToDateTime(dateTo);
        //            dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
        //            dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
        //        }
        //        var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
        //        IEnumerable<SuperShopPurchaseDetailViewModel> purchaseDetails = new List<SuperShopPurchaseDetailViewModel>();
        //        if (!string.IsNullOrEmpty(dateFrom))
        //        {
        //            purchaseDetails = Mapper.Map<IEnumerable<SuperShopPurchaseDetailViewModel>>(_rawSqlService.GetAllPurchaseDetail(identity.CompanyId.ToString(), identity.BranchId.ToString(), dateFrom, dateTo, supplierId));
        //        }
        //        ReportDataSource rpt = new ReportDataSource("PurchaseDetail", purchaseDetails);
        //        RdlcReportViewer.reportDataSource = rpt;
        //        string rPath = "RdlcReport/RptPurchaseList.rdlc";
        //        Response.Redirect("~/ReportViewer/RdlcReportViewer.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + identity.CompanyId + "&branchId=" + identity.BranchId);
        //        return View();
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}


        #endregion

    }
}
