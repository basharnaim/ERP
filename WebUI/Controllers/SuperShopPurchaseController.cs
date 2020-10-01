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
using System.Web.Script.Serialization;

namespace ERP.WebUI.Controllers
{
    public class SuperShopPurchaseController : BaseController
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

        #region Get
        public ActionResult Index(string dateFrom, string dateTo, string supplierId)
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
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo) && !string.IsNullOrEmpty(supplierId))
                {
                    View(AutoMapperConfiguration.mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_purchaseService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value, supplierId)).ToList());
                }
                else if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    View(AutoMapperConfiguration.mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_purchaseService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value)).ToList());
                }
                else if (!string.IsNullOrEmpty(supplierId))
                {
                    View(AutoMapperConfiguration.mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_purchaseService.GetAllBySupplier(supplierId)).ToList());
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region BlankItem
        [HttpPost]
        public ViewResult BlankItem()
        {
            try
            {
                return View("_PurchaseItemRow", new SuperShopPurchaseDetailViewModel());
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
                ViewBag.ItemList = serializer.Serialize(_rawSqlService.GetBranchwiseProductStockAll(identity.CompanyId, identity.BranchId));
                SuperShopPurchaseViewModel vm = new SuperShopPurchaseViewModel
                {
                    Id = _purchaseService.GenerateAutoId(identity.CompanyId, identity.BranchId, "Purchase"),
                    PurchaseDate = DateTime.Now,
                    PurchaseDetails = new List<SuperShopPurchaseDetailViewModel>()
                };
                vm.PurchaseDetails.Add(new SuperShopPurchaseDetailViewModel());
                return View(vm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(SuperShopPurchaseViewModel purchaseVm)
        {
            try
            {
                _purchaseService.Add(AutoMapperConfiguration.mapper.Map<Purchase>(purchaseVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/SuperShopPurchase?companyId=" + purchaseVm.CompanyId + "&&branchId=" + purchaseVm.BranchId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Edit
        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
                ViewBag.ItemList = serializer.Serialize(_rawSqlService.GetBranchwiseProductStockAll(identity.CompanyId, identity.BranchId));
                var purchase = AutoMapperConfiguration.mapper.Map<SuperShopPurchaseViewModel>(_purchaseService.GetById(id));
                List<SuperShopPurchaseDetailViewModel> purchaseItems = AutoMapperConfiguration.mapper.Map<List<SuperShopPurchaseDetailViewModel>>(_purchaseService.GetAllPurchaseDetailbyMasterId(id).ToList());
                purchase.PurchaseDetails = new List<SuperShopPurchaseDetailViewModel>();
                purchase.PurchaseDetails.AddRange(purchaseItems);
                return View(purchase);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(SuperShopPurchaseViewModel purchaseVm)
        {
            try
            {
                _purchaseService.Update(AutoMapperConfiguration.mapper.Map<Purchase>(purchaseVm));
                return JavaScript($"ShowResult('{"Data Updated successfully."}','{"success"}','{"redirect"}','{"/SuperShopPurchase?companyId=" + purchaseVm.CompanyId + "&&branchId=" + purchaseVm.BranchId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Rdlc Report
        public ActionResult PurchaseSummary(string dateFrom, string dateTo, string supplierId)
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
                IEnumerable<SuperShopPurchaseViewModel> purchases = new List<SuperShopPurchaseViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    purchases = AutoMapperConfiguration.mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_rawSqlService.GetAllPurchaseSummary(identity.CompanyId.ToString(), identity.BranchId.ToString(), dateFrom, dateTo, supplierId));
                }
                return View(purchases);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReportPurchaseSummary(string dateFrom, string dateTo, string supplierId)
        {
            try
            {
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
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                IEnumerable<SuperShopPurchaseViewModel> purchases = new List<SuperShopPurchaseViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    purchases = AutoMapperConfiguration.mapper.Map<IEnumerable<SuperShopPurchaseViewModel>>(_rawSqlService.GetAllPurchaseSummary(identity.CompanyId.ToString(), identity.BranchId.ToString(), dateFrom, dateTo, supplierId));
                }
                ReportDataSource rpt = new ReportDataSource("Purchase", purchases);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptPurchaseSummary.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + identity.CompanyId + "&branchId=" + identity.BranchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult PurchaseList(string dateFrom, string dateTo, string supplierId)
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
                IEnumerable<SuperShopPurchaseDetailViewModel> purchaseDetails = new List<SuperShopPurchaseDetailViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    purchaseDetails = AutoMapperConfiguration.mapper.Map<IEnumerable<SuperShopPurchaseDetailViewModel>>(_rawSqlService.GetAllPurchaseDetail(identity.CompanyId.ToString(), identity.BranchId.ToString(), dateFrom, dateTo, supplierId));
                }
                return View(purchaseDetails);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReportPurchaseList(string dateFrom, string dateTo, string supplierId)
        {
            try
            {
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
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                IEnumerable<SuperShopPurchaseDetailViewModel> purchaseDetails = new List<SuperShopPurchaseDetailViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    purchaseDetails = AutoMapperConfiguration.mapper.Map<IEnumerable<SuperShopPurchaseDetailViewModel>>(_rawSqlService.GetAllPurchaseDetail(identity.CompanyId.ToString(), identity.BranchId.ToString(), dateFrom, dateTo, supplierId));
                }
                ReportDataSource rpt = new ReportDataSource("PurchaseDetail", purchaseDetails);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptPurchaseList.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + identity.CompanyId + "&branchId=" + identity.BranchId);
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ActionResult ReportPurchaseMasterDetail(string id)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var master = AutoMapperConfiguration.mapper.Map<IEnumerable<PurchaseViewModelForReport>>(_purchaseService.GetAll().Where(x => x.Id == id).ToList()).ToList();
                var detail = AutoMapperConfiguration.mapper.Map<IEnumerable<PurchaseDetailViewModelForReport>>(_purchaseService.GetAllPurchaseDetailbyMasterIdForReport(id).ToList()).ToList();
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
        #endregion
    }
}
