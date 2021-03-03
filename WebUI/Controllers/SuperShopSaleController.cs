using ERP.WebUI.DirectPrint;
using ERP.WebUI.ReportViewer;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Sales;
using Library.Service.Core.Organizations;
using Library.Context.Repositories;
using Library.Service.Inventory.Sales;
using Library.ViewModel.Inventory.Sales;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using Library.ViewModel.Inventory.Customers;
using Library.Service.Inventory.Customers;
using Library.Model.Inventory.Customers;
using System.Data;
using AutoMapper;

namespace ERP.WebUI.Controllers
{
    public class SuperShopSaleController : BaseController
    {
        #region Ctor
        private readonly ISaleService _saleService;
        private readonly ICompanyService _companyService;
        private readonly IBranchService _branchService;
        private readonly IRawSqlService _rawSqlService;
        private readonly ICustomerService _customerService;
        public SuperShopSaleController(
            ISaleService saleService,
            ICompanyService companyService,
            IBranchService branchService,
            IRawSqlService rawSqlService,
            ICustomerService customerService
            )
        {
            _saleService = saleService;
            _companyService = companyService;
            _branchService = branchService;
            _rawSqlService = rawSqlService;
            _customerService = customerService;
        }
        #endregion

        #region Get
        public ActionResult Index(string dateFrom, string dateTo, string customerId, string id)
        {
            try
            {
                ViewBag.Id = id;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dataSet = _rawSqlService.getSalesList(identity.CompanyId, identity.BranchId, customerId, dateFrom, dateTo);
                return View(dataSet);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetAllCustomerInformationByMobileNumber(string customerMobile)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return Json(_rawSqlService.GetAllCustomerInformation(identity.CompanyId, identity.BranchId, customerMobile), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(ex.Message, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region BlankItem
        [HttpPost]
        public ViewResult BlankItem()
        {
            try
            {
                return View("_SaleItemRow", new SuperShopSaleDetailViewModel());
            }
            catch (Exception ex)
            {
                return View($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create(string id)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
                ViewBag.ProductList = serializer.Serialize(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId));
                ViewBag.PromotionalDiscountList = serializer.Serialize(_rawSqlService.GetPromotionalPointAndDiscount(DateTime.Today));
                ViewBag.ItemList = serializer.Serialize(_rawSqlService.GetBranchwiseProductStockAll(identity.CompanyId, identity.BranchId));
                ViewData["SaleDate"] = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
                ViewData["open"] = 0;
                var dataSet = new DataSet();
                var customers = Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll());
                var customer = new CustomerViewModel { ACustomers = customers.ToList() };
                if(!string.IsNullOrEmpty(id))
                {
                    ViewData["open"] = 1;
                    dataSet = _rawSqlService.ReportLevelPrint(id);
                }                
                SuperShopSaleViewModel vm = new SuperShopSaleViewModel
                {
                    Id = _saleService.GenerateAutoId(identity.CompanyId, identity.BranchId, "Sale"),
                    SaleDate = DateTime.Now,
                    PaymentType = PaymentType.Cash.ToString(),
                    Customer = customer,
                    Sales = dataSet
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Create(SuperShopSaleViewModel salevm)
        {
            try
            {
                //var ids = _saleService.Add(Mapper.Map<Sale>(salevm));
                var id = _rawSqlService.AddSaleItems(salevm);
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Invoice")
                    return Redirect("/SuperShopSale/ReportSaleMasterDetail/" + id);
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Label")
                    return Redirect("/SuperShopSale/ReportLevelPrint/" + id);
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Html")
                    return Redirect("/SuperShopSale/HtmlReportPrint/"+ id);
                //return JavaScript($"ShowResult('{"Data save successfully."}','{"success"}','{"redirect"}','{"/SuperShopSale/Create"}')");
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Print")
                {
                    return Redirect("/SuperShopSale?id=" + id);
                }
                return Redirect("/SuperShopSale");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
        [HttpPost]
        public JavaScriptResult Update(CustomerViewModel customervm)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                customervm.CompanyId = identity.CompanyId;
                customervm.BranchId = identity.BranchId;
                customervm.Active = true;
                if (string.IsNullOrEmpty(customervm.Id))
                {
                    _customerService.Add(Mapper.Map<Customer>(customervm));
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/SuperShopSale/Create"}')");
                }
                else
                {
                    _customerService.Update(Mapper.Map<Customer>(customervm));
                    return JavaScript($"ShowResult('{"Data update successfully."}','{"success"}','{"redirect"}','{"/SuperShopSale/Create"}')");
                }
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #region Edit
        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
                ViewBag.ProductList = serializer.Serialize(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId));
                ViewBag.PromotionalDiscountList = serializer.Serialize(_rawSqlService.GetPromotionalPointAndDiscount(DateTime.Today));
                SuperShopSaleViewModel salevm = Mapper.Map<SuperShopSaleViewModel>(_saleService.GetById(id));
                List<SuperShopSaleDetailViewModel> saleItems = Mapper.Map<List<SuperShopSaleDetailViewModel>>(_saleService.GetAllSaleDetailbyMasterId(id).ToList());
                ViewBag.NetAmount = salevm.NetAmount;
                salevm.SaleDetails = new List<SuperShopSaleDetailViewModel>();
                salevm.SaleDetails.AddRange(saleItems);
                return View(salevm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Edit(SuperShopSaleViewModel salevm)
        {
            try
            {
                var id = _saleService.Update(Mapper.Map<Sale>(salevm));
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Invoice")
                    return Redirect("/SuperShopSale/ReportSaleMasterDetail/" + id);
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Label")
                    return Redirect("/SuperShopSale/ReportLevelPrint/" + id);
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Print")
                {
                    return Redirect("/SuperShopSale?id=" + id);
                }
                return Redirect("/SuperShopSale");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Details
        [HttpGet]
        public ActionResult Details(string id)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                SuperShopSaleViewModel salevm = Mapper.Map<SuperShopSaleViewModel>(_saleService.GetById(id));
                List<SuperShopSaleDetailViewModel> saleItems = Mapper.Map<List<SuperShopSaleDetailViewModel>>(_saleService.GetAllSaleDetailbyMasterId(id).ToList());

                salevm.SaleDetails = new List<SuperShopSaleDetailViewModel>();
                salevm.SaleDetails.AddRange(saleItems);
                return View(salevm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Details(SuperShopSaleViewModel salevm)
        {
            try
            {
                Sale sale = Mapper.Map<Sale>(salevm);
                List<SaleDetail> saleItems = Mapper.Map<List<SaleDetail>>(salevm.SaleDetails);
                sale.SaleDetails = new List<SaleDetail>();
                foreach (var item in saleItems)
                {
                    sale.SaleDetails.Add(item);
                }
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Invoice")
                    return Redirect("/SuperShopSale/ReportSaleMasterDetail/" + sale.Id);
                else if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Label")
                    return Redirect("/SuperShopSale/ReportLevelPrint/" + sale.Id);
                return Redirect("/SuperShopSale/");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Rdlc Report
        public ActionResult ReportSaleSummary(string dateFrom, string dateTo, string customerId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
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
                IEnumerable<SuperShopSaleViewModel> sales = new List<SuperShopSaleViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    sales = Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_rawSqlService.GetAllSalesSummary(identity.CompanyId, identity.BranchId, dateFrom, dateTo, customerId));
                }
                return View(sales);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReportSaleSummary(string dateFrom, string dateTo, string customerId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
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
                IEnumerable<SuperShopSaleViewModel> sales = new List<SuperShopSaleViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    sales = Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_rawSqlService.GetAllSalesSummary(identity.CompanyId, identity.BranchId, dateFrom, dateTo, customerId));
                }
                ReportDataSource rpt = new ReportDataSource("Sale", sales);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptSaleSummary.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + identity.CompanyId + "&branchId=" + identity.BranchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult ReportSaleClosing(string dateFrom, string dateTo)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
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
                IEnumerable<SuperShopSaleViewModel> sales = new List<SuperShopSaleViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    sales = Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_rawSqlService.GetAllSaleClosing(identity.CompanyId, identity.BranchId, dateFrom, dateTo, identity.Name));
                }
                return View(sales);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReportSaleClosing(string dateFrom, string dateTo)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
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
                IEnumerable<SuperShopSaleViewModel> sales = new List<SuperShopSaleViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    sales = Mapper.Map<IEnumerable<SuperShopSaleViewModel>>(_rawSqlService.GetAllSaleClosing(identity.CompanyId, identity.BranchId, dateFrom, dateTo, identity.Name));
                }
                ReportDataSource rpt = new ReportDataSource("Sale", sales);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptSaleClosing.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + identity.CompanyId + "&branchId=" + identity.BranchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult ReportSaleMasterDetail(string id)
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
        public ActionResult ReportSaleChallan(string id) 
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
                string rPath = "RdlcReport/RptSalesChallanForMobileshop.rdlc";
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
                var dataSet = _rawSqlService.ReportLevelPrint(id);
                ReportDataSource rpt1 = new ReportDataSource("Sale", dataSet.Tables[0]);
                ReportDataSource rpt2 = new ReportDataSource("SaleDetail", dataSet.Tables[1]);
                List<ReportDataSource> rptl = new List<ReportDataSource> { rpt1, rpt2 };
                RdlcReportViewerMultipleDataSet.reportDataSource = rptl;
                string rPath = "RdlcReport/RptSalesInvoiceForSuperShop.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerMultipleDataSet.aspx?rPath=" + rPath + "&branchId=" + identity.BranchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult ReportInvoicePrint(string id)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return JavaScript($"DirectRdlcReportPrint('" + id + "','" + identity.CompanyId + "','" + identity.BranchId + "');");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult ReportPrintMultipleInvoice(string itemIds)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var itemIdIdList = new JavaScriptSerializer().Deserialize<string[]>(itemIds);
                var maIds = string.Join(",", itemIdIdList.Select(x => "'" + x + "'").ToArray());
                var data = _rawSqlService.GetAllForMultiplePrint(maIds);
                ReportDataSource rpt = new ReportDataSource("MultipleSalesInvoice", data);
                RdlcReportViewerWithoutDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptMultipleSalesInvoiceForMobileshop.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithoutDate.aspx?rPath=" + rPath + "&companyId=" + identity.CompanyId + "&branchId=" + identity.BranchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult ReportPrintMultipleLevel(string itemIds)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var itemIdIdList = new JavaScriptSerializer().Deserialize<string[]>(itemIds);
                var maIds = string.Join(",", itemIdIdList.Select(x => "'" + x + "'").ToArray());
                var data = _rawSqlService.GetAllForMultipleLabelPrint(maIds);
                ReportDataSource rpt = new ReportDataSource("MultipleSalesInvoice", data);
                RdlcReportViewerBranchWise.reportDataSource = rpt;
                string rPath = "RdlcReport/RptMultipleLevelPrint.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerBranchWise.aspx?rPath=" + rPath + "&branchId=" + identity.BranchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpGet]
        public ActionResult ApprovalMessage()
        {
            return View();
        }
        #endregion

        public ActionResult HtmlReportPrint(string id)
        {
            var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
            var dataSet = _rawSqlService.ReportLevelPrint(id);
            //Response.Redirect("~/ReportViewer/RdlcReportViewerMultipleDataSet.aspx");
            return View(dataSet);
        }
    }
}
