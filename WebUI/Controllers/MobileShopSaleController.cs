﻿using ERP.WebUI.DirectPrint;
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
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;

namespace ERP.WebUI.Controllers
{
    public class MobileShopSaleController : BaseController
    {
        #region Ctor
        private readonly ISaleService _saleService;
        private readonly ICompanyService _companyService;
        private readonly IBranchService _branchService;
        private readonly IRawSqlService _rawSqlService;
        public MobileShopSaleController(
            ISaleService saleService,
            ICompanyService companyService,
            IBranchService branchService,
            IRawSqlService rawSqlService
            )
        {
            _saleService = saleService;
            _companyService = companyService;
            _branchService = branchService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        #region Get
        public ActionResult Index(string dateFrom, string dateTo, string customerId)
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
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo) && !string.IsNullOrEmpty(customerId))
                {
                    return View(Mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_saleService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value, customerId)));
                }
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_saleService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value)));
                }
                if (!string.IsNullOrEmpty(customerId))
                {
                    return View(Mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_saleService.GetAllByCustomer(customerId)));
                }
                return View();
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
                return View("_SaleItemRow", new MobileShopSaleDetailViewModel());
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
                var serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
                ViewBag.ProductList = serializer.Serialize(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId));
                ViewBag.PromotionalDiscountList = serializer.Serialize(_rawSqlService.GetPromotionalPointAndDiscount(DateTime.Today));
                MobileShopSaleViewModel vm = new MobileShopSaleViewModel
                {
                    Id = _saleService.GenerateAutoId(identity.CompanyId, identity.BranchId, "Sale"),
                    SaleDate = DateTime.Now,
                    PaymentType = PaymentType.Cash.ToString()
                };
                return View(vm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Create(MobileShopSaleViewModel salevm)
        {
            try
            {
                var id = _saleService.Add(Mapper.Map<Sale>(salevm));
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Invoice")
                    return Redirect("/MobileShopSale/ReportSaleMasterDetail/" + id);
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Label")
                    return Redirect("/MobileShopSale/ReportLevelPrint/" + id);
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Print")
                    return Redirect("/MobileShopSale/ReportInvoicePrint/" + id);
                return Redirect("/MobileShopSale/");
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
                ViewBag.ProductList = serializer.Serialize(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId));
                ViewBag.PromotionalDiscountList = serializer.Serialize(_rawSqlService.GetPromotionalPointAndDiscount(DateTime.Today));
                MobileShopSaleViewModel salevm = Mapper.Map<MobileShopSaleViewModel>(_saleService.GetById(id));
                List<MobileShopSaleDetailViewModel> saleItems = Mapper.Map<List<MobileShopSaleDetailViewModel>>(_saleService.GetAllSaleDetailbyMasterId(id).ToList());
                
                salevm.SaleDetails = new List<MobileShopSaleDetailViewModel>();
                salevm.SaleDetails.AddRange(saleItems);
                return View(salevm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Edit(MobileShopSaleViewModel salevm)
        {
            try
            {
                var id = _saleService.Update(Mapper.Map<Sale>(salevm));
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Invoice")
                    return Redirect("/MobileShopSale/ReportSaleMasterDetail/" + id);
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Label")
                    return Redirect("/MobileShopSale/ReportLevelPrint/" + id);
                if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Print")
                    return Redirect("/MobileShopSale/ReportInvoicePrint/" + id);
                return Redirect("/MobileShopSale/");
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
                MobileShopSaleViewModel salevm = Mapper.Map<MobileShopSaleViewModel>(_saleService.GetById(id));
                List<MobileShopSaleDetailViewModel> saleItems = Mapper.Map<List<MobileShopSaleDetailViewModel>>(_saleService.GetAllSaleDetailbyMasterId(id).ToList());
                
                salevm.SaleDetails = new List<MobileShopSaleDetailViewModel>();
                salevm.SaleDetails.AddRange(saleItems);
                return View(salevm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Details(MobileShopSaleViewModel salevm)
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
                    return Redirect("/MobileShopSale/ReportSaleMasterDetail/" + sale.Id);
                else if (!string.IsNullOrEmpty(salevm.ActionType) && salevm.ActionType == "Label")
                    return Redirect("/MobileShopSale/ReportLevelPrint/" + sale.Id);
                return Redirect("/MobileShopSale/");
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
                IEnumerable<MobileShopSaleViewModel> sales = new List<MobileShopSaleViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    sales = Mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_rawSqlService.GetAllSalesSummary(identity.CompanyId, identity.BranchId, dateFrom, dateTo, customerId));
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
                IEnumerable<MobileShopSaleViewModel> sales = new List<MobileShopSaleViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    sales = Mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_rawSqlService.GetAllSalesSummary(identity.CompanyId, identity.BranchId, dateFrom, dateTo, customerId));
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
                IEnumerable<MobileShopSaleViewModel> sales = new List<MobileShopSaleViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    sales = Mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_rawSqlService.GetAllSaleClosing(identity.CompanyId, identity.BranchId, dateFrom, dateTo, identity.Name));
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
                IEnumerable<MobileShopSaleViewModel> sales = new List<MobileShopSaleViewModel>();
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    sales = Mapper.Map<IEnumerable<MobileShopSaleViewModel>>(_rawSqlService.GetAllSaleClosing(identity.CompanyId, identity.BranchId, dateFrom, dateTo, identity.Name));
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

        public ActionResult ReportLevelPrint(string id)
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
                var master = Mapper.Map<IEnumerable<SaleViewModelForReport>>(_saleService.GetAllForReport(id)).ToList();
                var detail = Mapper.Map<IEnumerable<SaleDetailViewModelForReport>>(_saleService.GetAllSaleDetailbyMasterIdForReport(id).ToList()).ToList();
                ReportDataSource rpt1 = new ReportDataSource("Sale", master);
                ReportDataSource rpt2 = new ReportDataSource("SaleDetail", detail);
                LocalReport report = new LocalReport
                {
                    ReportPath = "RdlcReport/RptSalesInvoiceForSuperShop.rdlc"
                };
                string companyName = _companyService.GetById(identity.CompanyId)?.Name;
                ReportParameter[] param1 = new ReportParameter[1];
                param1[0] = new ReportParameter("cName", companyName);
                report.SetParameters(param1);
                var branch = _branchService.GetById(identity.BranchId);
                string branchName = branch?.Name;
                string branchPhone = branch?.Phone1;
                string branchVatRegistryNo = branch?.VatRegistryNo ?? ".";
                string branchAddress = branch?.Address1;
                ReportParameter[] param2 = new ReportParameter[4];
                param2[0] = new ReportParameter("bName", branchName);
                param2[1] = new ReportParameter("bPhone", branchPhone);
                param2[2] = new ReportParameter("bVatRNo", branchVatRegistryNo);
                param2[3] = new ReportParameter("bAddress", branchAddress);
                report.SetParameters(param2);
                report.DataSources.Add(rpt1);
                report.DataSources.Add(rpt2);
                PrintRDLCReport.PrintToPrinter(report);
                return RedirectToAction("Index");
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
    }
}
