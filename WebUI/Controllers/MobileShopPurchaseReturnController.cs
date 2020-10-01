using Library.Crosscutting.Securities;
using Library.Model.Inventory.Purchases;
using Library.Service.Inventory.Purchases;
using Library.ViewModel.Inventory.Purchases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
{
    public class MobileShopPurchaseReturnController : BaseController
    {
        #region Ctor
        private readonly IPurchaseService _purchaseService;
        private readonly IPurchaseReturnService _purchaseReturnService;
        public MobileShopPurchaseReturnController(
            IPurchaseService purchaseService,
            IPurchaseReturnService purchaseReturnService
            )
        {
            _purchaseReturnService = purchaseReturnService;
            _purchaseService = purchaseService;
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
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopPurchaseViewModel>>(_purchaseService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value, supplierId)));
                }
                if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopPurchaseViewModel>>(_purchaseService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value)));
                }
                if (!string.IsNullOrEmpty(supplierId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<MobileShopPurchaseViewModel>>(_purchaseService.GetAll(identity.CompanyId, identity.BranchId, supplierId)));
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
                if ( !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<PurchaseReturnViewModel>>(_purchaseReturnService.GetAll(identity.CompanyId, identity.BranchId, dfrom.Value, dto.Value)));
                }
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<PurchaseReturnViewModel>>(_purchaseReturnService.GetAll(identity.CompanyId, identity.BranchId)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create(string purchaseId)
        {
            try
            {
                if (purchaseId != null)
                {
                    decimal totalReturnAmount = 0m;
                    PurchaseReturnViewModel purchaseReturn = new PurchaseReturnViewModel();
                    List<PurchaseReturnDetailViewModel> purchaseReturnDetails = new List<PurchaseReturnDetailViewModel>();
                    MobileShopPurchaseViewModel purchaseVm = AutoMapperConfiguration.mapper.Map<MobileShopPurchaseViewModel>(_purchaseService.GetById(purchaseId));
                    List<SuperShopPurchaseDetailViewModel> purchaseDetails = AutoMapperConfiguration.mapper.Map<List<SuperShopPurchaseDetailViewModel>>(_purchaseService.GetAllPurchaseDetailbyMasterId(purchaseId).ToList());
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
                                UOMName= purchaseDetail.UOMName,
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
                    PurchaseReturn purchaseReturn = AutoMapperConfiguration.mapper.Map<PurchaseReturn>(purchaseReturnvm);
                    List<PurchaseReturnDetail> purchaseReturnDetailList = AutoMapperConfiguration.mapper.Map<List<PurchaseReturnDetail>>(purchaseReturnvm.PurchaseReturnDetails.Where(x => x.Select && x.ReturnQuantity > 0));
                    purchaseReturn.PurchaseReturnDetails = new List<PurchaseReturnDetail>();
                    foreach (var purchaseReturnDetail in purchaseReturnDetailList)
                    {
                        purchaseReturn.PurchaseReturnDetails.Add(purchaseReturnDetail);
                    }
                    _purchaseReturnService.Add(purchaseReturn);
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/MobileShopPurchaseReturn"}')");
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
                var purchaseReturn = AutoMapperConfiguration.mapper.Map<PurchaseReturnViewModel>(_purchaseReturnService.GetById(purchaseReturnId));
                List<PurchaseReturnDetailViewModel> purchaseReturnDetails = AutoMapperConfiguration.mapper.Map<List<PurchaseReturnDetailViewModel>>(_purchaseReturnService.GetAllPurchaseReturnDetailbyMasterId(purchaseReturnId).ToList());
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
                    PurchaseReturn purchaseReturn = AutoMapperConfiguration.mapper.Map<PurchaseReturn>(purchaseReturnvm);
                    List<PurchaseReturnDetail> PurchaseReturnDetailList = AutoMapperConfiguration.mapper.Map<List<PurchaseReturnDetail>>(purchaseReturnvm.PurchaseReturnDetails.Where(x => x.Select && x.ReturnQuantity > 0));
                    purchaseReturn.PurchaseReturnDetails = new List<PurchaseReturnDetail>();
                    foreach (var item in PurchaseReturnDetailList)
                    {
                        purchaseReturn.PurchaseReturnDetails.Add(item);
                    }
                    _purchaseReturnService.Update(purchaseReturn);
                    return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/MobileShopPurchaseReturn"}')");
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
