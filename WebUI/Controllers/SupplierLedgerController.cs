#region Using

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Suppliers;
using Library.Service.Core.Organizations;
using Library.Context.Repositories;
using Library.Service.Inventory.Accounts;
using Library.Service.Inventory.Suppliers;
using Library.ViewModel.Inventory.Accounts;
using Library.ViewModel.Inventory.Suppliers;

#endregion

namespace ERP.WebUI.Controllers
{
    public class SupplierLedgerController : BaseController
    {
        #region Ctor
        private readonly ISupplierLedgerService _supplierLedgerService;
        private readonly ICompanyService _companyService;
        private readonly IBranchService _branchService;
        private readonly ISupplierService _supplierService;
        private readonly IRawSqlService _rawSqlService;
        public SupplierLedgerController(
            ISupplierLedgerService supplierLedgerService,
            ICompanyService companyService,
            IBranchService branchService,
            ISupplierService supplierService,
            IRawSqlService rawSqlService
            )
        {
            _supplierLedgerService = supplierLedgerService;
            _companyService = companyService;
            _branchService = branchService;
            _supplierService = supplierService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        #region Get
        public ActionResult Index(string companyId, string branchId, string dateFrom, string dateTo, string customerType, string phone, string supplierId)
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
                SupplierViewModel suppliervm = new SupplierViewModel();
                List<SupplierLedgerViewModel> supplierLedger = new List<SupplierLedgerViewModel>();
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    suppliervm = AutoMapperConfiguration.mapper.Map<SupplierViewModel>(_supplierService.GetSupplierBySupplierMobileNumber(phone));
                    supplierLedger = AutoMapperConfiguration.mapper.Map<List<SupplierLedgerViewModel>>(_rawSqlService.GetAllSupplierLedger(companyId, branchId, dfrom.ToString(), dto.ToString(), "", phone, "").ToList());
                }
                else if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(supplierId) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    suppliervm = AutoMapperConfiguration.mapper.Map<SupplierViewModel>(_supplierService.GetSupplierBySupplierId(supplierId));
                    supplierLedger = AutoMapperConfiguration.mapper.Map<List<SupplierLedgerViewModel>>(_rawSqlService.GetAllSupplierLedger(companyId, branchId, dfrom.ToString(), dto.ToString(), "", "", supplierId).ToList());
                }
                else if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(phone))
                {
                    suppliervm = AutoMapperConfiguration.mapper.Map<SupplierViewModel>(_supplierService.GetSupplierBySupplierMobileNumber(phone));
                    supplierLedger = AutoMapperConfiguration.mapper.Map<List<SupplierLedgerViewModel>>(_rawSqlService.GetAllSupplierLedger(companyId, branchId, "", "", "", phone, "").ToList());
                }
                else if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(supplierId))
                {
                    suppliervm = AutoMapperConfiguration.mapper.Map<SupplierViewModel>(_supplierService.GetSupplierBySupplierId(supplierId));
                    supplierLedger = AutoMapperConfiguration.mapper.Map<List<SupplierLedgerViewModel>>(_rawSqlService.GetAllSupplierLedger(companyId, branchId, "", "", "", "", supplierId).ToList());
                }
                return View(new Tuple<IEnumerable<SupplierLedgerViewModel>, SupplierViewModel>(supplierLedger, suppliervm));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create(string phone, string supplierId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                SupplierLedgerViewModel supplierLedgervm = new SupplierLedgerViewModel();
                if (string.IsNullOrEmpty(phone) && string.IsNullOrEmpty(supplierId))
                {
                    return View(supplierLedgervm);
                }

                Supplier supplier = new Supplier();
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                if (!string.IsNullOrEmpty(phone))
                {
                    supplier = _supplierService.GetSupplierBySupplierMobileNumber(phone);
                    _supplierLedgerService.GetDueOrAdvanceAmountBySupplierPhone(phone, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        supplierLedgervm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        supplierLedgervm.AdvanceAmount = advanceAmount;
                    }
                }
                else if (!string.IsNullOrEmpty(supplierId))
                {
                    supplier = _supplierService.GetSupplierBySupplierId(supplierId);
                    _supplierLedgerService.GetDueOrAdvanceAmountBySupplierId(supplierId, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        supplierLedgervm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        supplierLedgervm.AdvanceAmount = advanceAmount;
                    }
                }
                if (supplier != null)
                {
                    supplierLedgervm.SupplierId = supplier.Id;
                    supplierLedgervm.SupplierName = supplier.Name;
                    supplierLedgervm.SupplierAddress = supplier.Address1;
                }
                return View(supplierLedgervm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(SupplierLedgerViewModel supplierLedgervm)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                supplierLedgervm.CompanyId = identity.CompanyId;
                supplierLedgervm.BranchId = identity.BranchId;
                _supplierLedgerService.Add(AutoMapperConfiguration.mapper.Map<SupplierLedger>(supplierLedgervm));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/SupplierLedger?supplierId=" + supplierLedgervm.SupplierId}')");
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
                SupplierLedgerViewModel supplierLedgervm = AutoMapperConfiguration.mapper.Map<SupplierLedgerViewModel>(_supplierLedgerService.GetById(id));
                Supplier supplier = _supplierService.GetById(supplierLedgervm.SupplierId);
                supplierLedgervm.SupplierName = supplier.Name;
                supplierLedgervm.SupplierPhone = supplier.Phone1;
                supplierLedgervm.SupplierAddress = supplier.Address1;
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                _supplierLedgerService.GetDueOrAdvanceAmountBySupplierPhone(supplier.Phone1, out dueAmount, out advanceAmount);
                if (dueAmount > 0)
                {
                    supplierLedgervm.DueAmount = dueAmount;
                }
                if (advanceAmount > 0)
                {
                    supplierLedgervm.AdvanceAmount = advanceAmount;
                }
                return View(supplierLedgervm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(SupplierLedgerViewModel supplierLedgervm)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                supplierLedgervm.CompanyId = identity.CompanyId;
                supplierLedgervm.BranchId = identity.BranchId;
                _supplierLedgerService.Update(AutoMapperConfiguration.mapper.Map<SupplierLedger>(supplierLedgervm));
                return JavaScript(
                    $"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/SupplierLedger?supplierId=" + supplierLedgervm.SupplierId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        
    }
}
