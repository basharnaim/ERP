using ERP.WebUI.ReportViewer;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Purchases;
using Library.Context.Repositories;
using Library.Service.Inventory.Purchases;
using Library.ViewModel.Inventory.Purchases;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using AutoMapper;

namespace ERP.WebUI.Controllers
{
    public class SuperShopPurchaseReturnController : BaseController
    {
        #region Ctor
        private readonly IPurchaseService _purchaseService;
        private readonly IPurchaseReturnService _purchaseReturnService;
        private readonly IRawSqlService _rawSqlService;

        public SuperShopPurchaseReturnController(
            IPurchaseService purchaseService,
            IPurchaseReturnService purchaseReturnService,
            IRawSqlService rawSqlService
            )
        {
            _purchaseReturnService = purchaseReturnService;
            _purchaseService = purchaseService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        #region Get
        public ActionResult Index(string dateFrom, string dateTo, string supplierId)
        {
            try
            {
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
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (!string.IsNullOrEmpty(supplierId) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_purchaseService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value, supplierId)));
                }
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_purchaseService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value)));
                }
                if (!string.IsNullOrEmpty(supplierId))
                {
                    return View(Mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_purchaseService.GetAll(identity.CompanyId, identity.BranchId, supplierId)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }


        public ActionResult PurchaseReturnIndex(string dateFrom, string dateTo)
        {
            try
            {
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
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<PurchaseReturnViewModel>>(_purchaseReturnService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value)));
                }
                return View(Mapper.Map<IEnumerable<PurchaseReturnViewModel>>(_purchaseReturnService.GetAll(identity.CompanyId, identity.BranchId)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create(string purchaseId, string supplierName)
        {
            try
            {
                if (purchaseId != null)
                {
                    decimal totalReturnAmount = 0m;
                    PurchaseReturnViewModel purchaseReturn = new PurchaseReturnViewModel();
                    List<PurchaseReturnDetailViewModel> purchaseReturnDetails = new List<PurchaseReturnDetailViewModel>();
                    SuperShopPurchaseViewModel purchaseVm = Mapper.Map<SuperShopPurchaseViewModel>(_purchaseService.GetById(purchaseId));
                    List<SuperShopPurchaseDetailViewModel> purchaseDetails = Mapper.Map<List<SuperShopPurchaseDetailViewModel>>(_purchaseService.GetAllPurchaseDetailbyMasterId(purchaseId).ToList());
                    if (purchaseDetails.Any())
                    {
                        foreach (var purchaseDetail in purchaseDetails)
                        {
                            PurchaseReturnDetailViewModel purchaseReturnDetail = new PurchaseReturnDetailViewModel
                            {
                                Select = true,
                                PurchaseId = purchaseDetail.PurchaseId,
                                PurchaseDetailId = purchaseDetail.Id,
                                ProductId = purchaseDetail.ProductId,
                                ProductCode = purchaseDetail.ProductCode,
                                ProductName = purchaseDetail.ProductName,
                                UOMId = purchaseDetail.UOMId,
                                UOMName = purchaseDetail.UOMName,
                                PurchaseQuantity = purchaseDetail.Quantity,
                                RemainingQuantity = purchaseDetail.Quantity - _purchaseReturnService.GetAlreadyReturnQty(purchaseDetail.PurchaseId, purchaseDetail.ProductId),
                            };
                            purchaseReturnDetail.ReturnQuantity = purchaseReturnDetail.RemainingQuantity;
                            purchaseReturnDetail.ReturnAmount = purchaseReturnDetail.ReturnQuantity * purchaseDetail.PurchasePrice;
                            totalReturnAmount += purchaseReturnDetail.ReturnAmount;
                            purchaseReturnDetail.PurchasePrice = purchaseDetail.PurchasePrice;
                            purchaseReturnDetail.ReturnUnitPrice = purchaseDetail.PurchasePrice;
                            purchaseReturnDetail.PurchaseAmount = purchaseDetail.TotalAmount;
                            purchaseReturnDetails.Add(purchaseReturnDetail);
                        }
                    }
                    if (purchaseVm.SupplierId != null)
                    {
                        purchaseReturn.SupplierId = purchaseVm.SupplierId;
                    }
                    purchaseReturn.SelectAll = true;
                    purchaseReturn.PurchaseId = purchaseVm.Id;
                    purchaseReturn.InvoiceNo = purchaseVm.Id;
                    purchaseReturn.PurchaseDate = purchaseVm.PurchaseDate;
                    purchaseReturn.PurchaseReturnDate = DateTime.Now;
                    purchaseReturn.SupplierName = supplierName == null ? "All Supplier" : supplierName;
                    purchaseReturn.TotalAmount = totalReturnAmount;
                    purchaseReturn.PurchaseReturnDetails = new List<PurchaseReturnDetailViewModel>();
                    purchaseReturn.PurchaseReturnDetails.AddRange(purchaseReturnDetails);
                    return View(purchaseReturn);
                }
                return JavaScript($"ShowResult('{"Enter quantity!"}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(PurchaseReturnViewModel purchaseReturnvm)
        {
            try
            {
                if (purchaseReturnvm.PurchaseReturnDetails.Any(x => x.Select && x.ReturnQuantity > 0))
                {
                    PurchaseReturn purchaseReturn = Mapper.Map<PurchaseReturn>(purchaseReturnvm);
                    List<PurchaseReturnDetail> purchaseReturnDetailList = Mapper.Map<List<PurchaseReturnDetail>>(purchaseReturnvm.PurchaseReturnDetails.Where(x => x.Select && x.ReturnQuantity > 0));
                    purchaseReturn.PurchaseReturnDetails = new List<PurchaseReturnDetail>();
                    //purchaseReturn.PurchaseReturnDetails.ToList().AddRange(purchaseReturnDetailList);

                    foreach (var purchaseReturnDetail in purchaseReturnDetailList)
                    {
                        purchaseReturnDetail.SupplierId = purchaseReturnvm.SupplierId;
                        purchaseReturnDetail.PurchasePrice = purchaseReturnvm.PurchaseReturnDetails.Where(x => x.ProductId == purchaseReturnDetail.ProductId).FirstOrDefault().ReturnUnitPrice;
                        purchaseReturnDetail.ReturnPrice = purchaseReturnDetail.PurchasePrice;
                        purchaseReturn.PurchaseReturnDetails.Add(purchaseReturnDetail);
                    }
                    _purchaseReturnService.Add(purchaseReturn);
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/SuperShopPurchaseReturn/Index"}')");
                }
                return JavaScript($"ShowResult('{"Enter quantity!"}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Edit
        [HttpGet]
        public ActionResult Edit(string purchaseReturnId)
        {
            try
            {
                var purchaseReturn = Mapper.Map<PurchaseReturnViewModel>(_purchaseReturnService.GetById(purchaseReturnId));
                List<PurchaseReturnDetailViewModel> purchaseReturnDetails = Mapper.Map<List<PurchaseReturnDetailViewModel>>(_purchaseReturnService.GetAllPurchaseReturnDetailbyMasterId(purchaseReturnId).ToList());
                foreach (var purchaseReturnDetail in purchaseReturnDetails)
                {
                    purchaseReturnDetail.Select = true;
                    purchaseReturnDetail.RemainingQuantity = purchaseReturnDetail.PurchaseQuantity - _purchaseReturnService.GetSumOfAlreadyReturnQty(purchaseReturnDetail.PurchaseId, purchaseReturnDetail.ProductId);
                    purchaseReturnDetail.PurchasePrice = purchaseReturnDetail.ReturnUnitPrice;
                }
                purchaseReturn.SelectAll = true;
                purchaseReturn.PurchaseReturnDetails = new List<PurchaseReturnDetailViewModel>();
                purchaseReturn.PurchaseReturnDetails.AddRange(purchaseReturnDetails);
                return View(purchaseReturn);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(PurchaseReturnViewModel purchaseReturnvm)
        {
            try
            {
                if (purchaseReturnvm.PurchaseReturnDetails.Any(x => x.Select && x.ReturnQuantity > 0))
                {
                    PurchaseReturn purchaseReturn = Mapper.Map<PurchaseReturn>(purchaseReturnvm);
                    List<PurchaseReturnDetail> PurchaseReturnDetailList = Mapper.Map<List<PurchaseReturnDetail>>(purchaseReturnvm.PurchaseReturnDetails.Where(x => x.Select && x.ReturnQuantity > 0));
                    purchaseReturn.PurchaseReturnDetails = new List<PurchaseReturnDetail>();
                    foreach (var item in PurchaseReturnDetailList)
                    {
                        purchaseReturn.PurchaseReturnDetails.Add(item);
                    }
                    _purchaseReturnService.Update(purchaseReturn);
                    return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/SuperShopPurchaseReturn/Index"}')");
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
        public ActionResult PurchaseReturnSummary(string dateFrom, string dateTo, string supplierId)
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
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                IEnumerable<PurchaseReturnViewModel> purchases = new List<PurchaseReturnViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    purchases = Mapper.Map<IEnumerable<PurchaseReturnViewModel>>(_rawSqlService.GetAllPurchaseReturnSummary(identity.CompanyId.ToString(), identity.BranchId.ToString(), dateFrom, dateTo, supplierId));
                }
                return View(purchases);
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
                var master = Mapper.Map<IEnumerable<PurchaseViewModelForReport>>(_purchaseService.GetAll().Where(x => x.Id == id).ToList()).ToList();
                var detail = Mapper.Map<IEnumerable<PurchaseDetailViewModelForReport>>(_purchaseService.GetAllPurchaseDetailbyMasterIdForReport(id).ToList()).ToList();
                ReportDataSource rpt1 = new ReportDataSource("Purchase", master);
                ReportDataSource rpt2 = new ReportDataSource("PurchaseDetail", detail);
                List<ReportDataSource> rptl = new List<ReportDataSource> { rpt1, rpt2 };
                RdlcReportViewerMultipleDataSet.reportDataSource = rptl;
                string rPath = "RdlcReport/RptPurchaseInvoice.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerMultipleDataSet.aspx?rPath=" + rPath + "&companyId=" + identity.CompanyId + "&branchId=" + identity.BranchId);
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
        //        IEnumerable<SuperShopPurchaseViewModel> purchases = new List<SuperShopPurchaseViewModel>();
        //        if (!string.IsNullOrEmpty(dateFrom))
        //        {
        //            purchases = Mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_rawSqlService.GetAllPurchaseSummary(identity.CompanyId.ToString(), identity.BranchId.ToString(), dateFrom, dateTo, supplierId));
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
