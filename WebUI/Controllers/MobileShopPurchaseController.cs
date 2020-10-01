using ERP.WebUI.ReportViewer;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Purchases;
using Library.Context.Repositories;
using Library.Service.Inventory.Purchases;
using Library.ViewModel.Inventory.Purchases;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ERP.WebUI.Controllers
{
    public class MobileShopPurchaseController : BaseController
    {
        #region Ctor
        private readonly IPurchaseService _purchaseService;
        private readonly IRawSqlService _rawSqlService;
        public MobileShopPurchaseController(
            IPurchaseService purchaseService,
            IRawSqlService rawSqlService
            )
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
                List<MobileShopPurchaseViewModel> sales = new List<MobileShopPurchaseViewModel>();
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo) && !string.IsNullOrEmpty(supplierId))
                {
                    sales = AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopPurchaseViewModel>>(_purchaseService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value, supplierId)).ToList();
                }
                else if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    sales = AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopPurchaseViewModel>>(_purchaseService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value)).ToList();
                }
                else if (!string.IsNullOrEmpty(supplierId))
                {
                    sales = AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopPurchaseViewModel>>(_purchaseService.GetAllBySupplier(supplierId)).ToList();
                }
                return View(sales);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Partial
        public ViewResult BlankItem()
        {
            try
            {
                return View("_PurchaseItemRow", new MobileShopPurchaseDetailViewModel());
            }
            catch (Exception ex)
            {
                return View($"ShowResult('{ex.Message}','{"failure"}')");
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
                MobileShopPurchaseViewModel vm = new MobileShopPurchaseViewModel
                {
                    Id = _purchaseService.GenerateAutoId(identity.CompanyId, identity.BranchId, "Purchase"),
                    PurchaseDate = DateTime.Now,
                    PurchaseDetails = new List<MobileShopPurchaseDetailViewModel>()
                };
                vm.PurchaseDetails.Add(new MobileShopPurchaseDetailViewModel());
                ViewBag.ItemList = new JavaScriptSerializer().Serialize(_rawSqlService.GetPurchaseImeiList(identity.CompanyId, identity.BranchId));
                return View(vm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(MobileShopPurchaseViewModel purchasevm)
        {
            try
            {
                Purchase purchase = AutoMapperConfiguration.mapper.Map<Purchase>(purchasevm);
                List<PurchaseDetail> purchaseItems = AutoMapperConfiguration.mapper.Map<List<PurchaseDetail>>(purchasevm.PurchaseDetails);
                purchase.PurchaseDetails = new List<PurchaseDetail>();
                foreach (var item in purchaseItems)
                {
                    purchase.PurchaseDetails.Add(item);
                }
                _purchaseService.Add(purchase);
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/MobileShopPurchase?companyId=" + purchase.CompanyId + "&&branchId=" + purchase.BranchId}')");
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
                MobileShopPurchaseViewModel purchase = AutoMapperConfiguration.mapper.Map<MobileShopPurchaseViewModel>(_purchaseService.GetById(id));
                List<MobileShopPurchaseDetailViewModel> purchaseItems = AutoMapperConfiguration.mapper.Map<List<MobileShopPurchaseDetailViewModel>>(_purchaseService.GetAllPurchaseDetailbyMasterId(id).ToList());
                purchase.PurchaseDetails = new List<MobileShopPurchaseDetailViewModel>();
                purchase.PurchaseDetails.AddRange(purchaseItems);
                ViewBag.ItemList = new JavaScriptSerializer().Serialize(_rawSqlService.GetPurchaseImeiList(identity.CompanyId, identity.BranchId));
                return View(purchase);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(MobileShopPurchaseViewModel purchasevm)
        {
            try
            {
                Purchase purchase = AutoMapperConfiguration.mapper.Map<Purchase>(purchasevm);
                List<PurchaseDetail> purchaseItems = AutoMapperConfiguration.mapper.Map<List<PurchaseDetail>>(purchasevm.PurchaseDetails);
                purchase.PurchaseDetails = new List<PurchaseDetail>();
                foreach (var item in purchaseItems)
                {
                    purchase.PurchaseDetails.Add(item);
                }
                _purchaseService.Update(purchase);
                return JavaScript($"ShowResult('{"Data Updated successfully."}','{"success"}','{"redirect"}','{"/MobileShopPurchase?companyId=" + purchase.CompanyId + "&&branchId=" + purchase.BranchId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Rdlc Report
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
                string rPath = "RdlcReport/RptPurchaseInvoiceForMobileShop.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerMultipleDataSet.aspx?rPath=" + rPath + "&companyId=" + identity.CompanyId + "&branchId=" + identity.BranchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult PurchaseSummary(string companyId, string branchId, string dateFrom, string dateTo, string supplierId)
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
                List<MobileShopPurchaseViewModel> purchases = new List<MobileShopPurchaseViewModel>();

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
                List<MobileShopPurchaseViewModel> purchases = new List<MobileShopPurchaseViewModel>();
                ReportDataSource rpt = new ReportDataSource("Purchase", purchases);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptPurchaseSummary.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }

        }

        public ActionResult RdlcSSPurchaseReportList(string companyId, string branchId, string purchaseId)
        {
            try
            {
                var master = AutoMapperConfiguration.mapper.Map<IEnumerable<PurchaseViewModelForReport>>(_purchaseService.GetAll().Where(x => x.Id == purchaseId).ToList()).ToList();
                var detail = AutoMapperConfiguration.mapper.Map<IEnumerable<PurchaseDetailViewModelForReport>>(_purchaseService.GetAllPurchaseDetailbyMasterIdForReport(purchaseId).ToList()).ToList();
                ReportDataSource rpt1 = new ReportDataSource("Purchase", master);
                ReportDataSource rpt2 = new ReportDataSource("PurchaseDetail", detail);
                List<ReportDataSource> rptl = new List<ReportDataSource> { rpt1, rpt2 };
                RdlcReportViewerMultipleDataSet.reportDataSource = rptl;
                string rPath = "RdlcReport/RptPurchase.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerMultipleDataSet.aspx?rPath=" + rPath + "&companyId=" + companyId + "&branchId=" + branchId);
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
