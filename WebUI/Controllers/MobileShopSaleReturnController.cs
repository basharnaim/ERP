#region Using

using AutoMapper;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Sales;
using Library.Service.Inventory.Sales;
using Library.ViewModel.Inventory.Sales;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

#endregion

namespace ERP.WebUI.Controllers
{
    public class MobileShopSaleReturnController : BaseController
    {
        #region Ctor
        private readonly ISaleReturnService _saleReturnService;
        private readonly ISaleService _saleService;
        public MobileShopSaleReturnController(
            ISaleService saleService,
            ISaleReturnService saleReturnService
            )
        {
            _saleService = saleService;
            _saleReturnService = saleReturnService;
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
                if (saleId != null)
                {
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
                                SoldQuantity = saleDetail.Quantity,
                                SoldAmount = saleDetail.TotalAmount,
                                RemainingQuantity = saleDetail.Quantity - _saleReturnService.GetSumOfAlreadyReturnQty(saleDetail.SaleId, saleDetail.ProductId),
                                SalePrice = saleDetail.SalePrice,
                                ReturnUnitPrice = saleDetail.SalePrice,

                            };
                            saleReturnDetail.ReturnQuantity = saleReturnDetail.RemainingQuantity;
                            saleReturnDetail.ReturnAmount = saleDetail.SalePrice * saleReturnDetail.ReturnQuantity;
                            saleReturnDetails.Add(saleReturnDetail);
                        }
                    }
                }
                saleReturnVm.SelectAll = true;
                saleReturnVm.SaleId = saleVm.Id;
                saleReturnVm.SaleDate = saleVm.SaleDate;
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
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/MobileShopSaleReturn/SaleReturnIndex?companyId=" + saleReturn.CompanyId + "&&branchId=" + saleReturn.BranchId}')");
                }
                return JavaScript($"ShowResult('{"Enter quantity!"}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

    }
}
