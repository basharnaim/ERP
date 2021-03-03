#region Using

using AutoMapper;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Customers;
using Library.Service.Inventory.Accounts;
using Library.ViewModel.Inventory.Accounts;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;

#endregion

namespace ERP.WebUI.Controllers
{
    public class PaymentInfoController : BaseController
    {
        #region Ctor
        private readonly IPaymentInfoService _paymentInfoService;
        private readonly ISupplierLedgerService _supplierLedgerService;
        private readonly ICustomerLedgerService _customerLedgerService;
        public PaymentInfoController(
            IPaymentInfoService paymentInfoService,
            ISupplierLedgerService supplierLedgerService,
            ICustomerLedgerService customerLedgerService
            )
        {
            _paymentInfoService = paymentInfoService;
            _supplierLedgerService = supplierLedgerService;
            _customerLedgerService = customerLedgerService;
        }
        #endregion

        #region PayIndex
        public ActionResult PayIndex(string transactionType, string dateFrom, string dateTo)
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
                if (!string.IsNullOrEmpty(transactionType) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<PaymentInfoViewModel>>(_paymentInfoService.GetAllForSupplier(identity.CompanyId, identity.BranchId, transactionType, dfrom.Value, dto.Value)));
                }

                if (!string.IsNullOrEmpty(transactionType))
                {
                    return View(Mapper.Map<IEnumerable<PaymentInfoViewModel>>(_paymentInfoService.GetAllForSupplier(identity.CompanyId, identity.BranchId, transactionType)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region PayByCash
        [HttpGet]
        public ActionResult PayByCash(string splrId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                PaymentInfoViewModel paymentInfoVm = new PaymentInfoViewModel();
                if (!string.IsNullOrEmpty(splrId))
                {
                    decimal dueAmount = 0m;
                    decimal advanceAmount = 0m;
                    string supplierName = "";
                    _supplierLedgerService.GetDueOrAdvanceAmountBySupplierIdWithSupplierName(splrId, out supplierName, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    else if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(identity.CompanyId, identity.BranchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cash.ToString();
                    paymentInfoVm.SupplierId = splrId;
                    paymentInfoVm.SupplierName = supplierName;
                    paymentInfoVm.CompanyId = identity.CompanyId;
                    paymentInfoVm.BranchId = identity.BranchId;
                    paymentInfoVm.TransactionDate = DateTime.Now;
                }
                return View(paymentInfoVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult PayByCash(PaymentInfoViewModel paymentInfoVM)
        {
            try
            {
                _paymentInfoService.PayByCash(Mapper.Map<PaymentInfo>(paymentInfoVM));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/PayIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region PayByCashEdit
        [HttpGet]
        public ActionResult PayByCashEdit(string id)
        {
            try
            {
                return View(Mapper.Map<PaymentInfoViewModel>(_paymentInfoService.GetByIdWithSupplier(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult PayByCashEdit(PaymentInfoViewModel paymentInfoVM)
        {
            try
            {
                _paymentInfoService.PayByCashUpdate(Mapper.Map<PaymentInfo>(paymentInfoVM));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/PayIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region PayByCheque
        [HttpGet]
        public ActionResult PayByCheque(string splrId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                PaymentInfoViewModel paymentInfoVm = new PaymentInfoViewModel();
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                string supplierName = "";
                if (!string.IsNullOrEmpty(splrId))
                {
                    _supplierLedgerService.GetDueOrAdvanceAmountBySupplierIdWithSupplierName(splrId, out supplierName, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    else if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(identity.CompanyId, identity.BranchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cheque.ToString();
                    paymentInfoVm.SupplierId = splrId;
                    paymentInfoVm.SupplierName = supplierName;
                    paymentInfoVm.CompanyId = identity.CompanyId;
                    paymentInfoVm.BranchId = identity.BranchId;
                    paymentInfoVm.TransactionDate = DateTime.Now;
                }
                return View(paymentInfoVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult PayByCheque(PaymentInfoViewModel paymentInfoVM)
        {
            try
            {
                _paymentInfoService.PayByCheque(Mapper.Map<PaymentInfo>(paymentInfoVM));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/PayIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region PayByChequeEdit
        [HttpGet]
        public ActionResult PayByChequeEdit(string id)
        {
            try
            {
                return View(Mapper.Map<PaymentInfoViewModel>(_paymentInfoService.GetByIdWithSupplier(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult PayByChequeEdit(PaymentInfoViewModel paymentInfoVM)
        {
            try
            {
                _paymentInfoService.PayByChequeUpdate(Mapper.Map<PaymentInfo>(paymentInfoVM));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/PayIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Honour
        public JavaScriptResult Honour(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    _paymentInfoService.Honour(id);
                    return JavaScript(
                        $"ShowResult('{"Cheque honoured successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/PayIndex?transactionType=" + TransactionType.Cheque.ToString()}')");
                }

                return JavaScript($"ShowResult('{"Please select the details information."}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{"Other error" + ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region Dishonour
        public JavaScriptResult Dishonour(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    _paymentInfoService.Dishonour(id);
                    return JavaScript(
                        $"ShowResult('{"Cheque dishonoured successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/PayIndex?transactionType=" + TransactionType.Cheque.ToString()}')");
                }

                return JavaScript($"ShowResult('{"Please select the details information."}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{"Other error" + ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region ReceiveIndex
        public ActionResult ReceiveIndex(string transactionType, string dateFrom, string dateTo)
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
                if (!string.IsNullOrEmpty(transactionType) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<PaymentInfoViewModel>>(_paymentInfoService.GetAllForCustomer(identity.CompanyId, identity.BranchId, transactionType, dfrom.Value, dto.Value)));
                }

                if (!string.IsNullOrEmpty(transactionType))
                {
                    return View(Mapper.Map<IEnumerable<PaymentInfoViewModel>>(_paymentInfoService.GetAllForCustomer(identity.CompanyId, identity.BranchId, transactionType)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region ReceiveByCash
        [HttpGet]
        public ActionResult ReceiveByCash(string phone, string cstmrId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                PaymentInfoViewModel paymentInfoVm = new PaymentInfoViewModel();
                string customerName = "";
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                if (!string.IsNullOrEmpty(phone))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerPhoneWithCustomerIdName(phone, out cstmrId, out customerName, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(identity.CompanyId, identity.BranchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cash.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = identity.CompanyId;
                    paymentInfoVm.BranchId = identity.BranchId;
                }
                if (!string.IsNullOrEmpty(cstmrId))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerIdWithCustomerName(cstmrId, out customerName, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(identity.CompanyId, identity.BranchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cash.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = identity.CompanyId;
                    paymentInfoVm.BranchId = identity.BranchId;
                }
                return View(paymentInfoVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult ReceiveByCash(PaymentInfoViewModel paymentInfoVM)
        {
            try
            {
                _paymentInfoService.ReceiveByCash(Mapper.Map<PaymentInfo>(paymentInfoVM));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/ReceiveIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region ReceiveByCash
        [HttpGet]
        public ActionResult CashReturn(string phone, string cstmrId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                PaymentInfoViewModel paymentInfoVm = new PaymentInfoViewModel();
                string customerName = "";
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                if (!string.IsNullOrEmpty(phone))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerPhoneWithCustomerIdName(phone, out cstmrId, out customerName, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(identity.CompanyId, identity.BranchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.CashReturn.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = identity.CompanyId;
                    paymentInfoVm.BranchId = identity.BranchId;
                }
                if (!string.IsNullOrEmpty(cstmrId))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerIdWithCustomerName(cstmrId, out customerName, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(identity.CompanyId, identity.BranchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.CashReturn.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = identity.CompanyId;
                    paymentInfoVm.BranchId = identity.BranchId;
                }
                return View(paymentInfoVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult CashReturn(PaymentInfoViewModel paymentInfoVM)
        {
            try
            {
                _paymentInfoService.CashReturn(Mapper.Map<PaymentInfo>(paymentInfoVM));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/ReceiveIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region ReceiveByCashEdit
        [HttpGet]
        public ActionResult ReceiveByCashEdit(string id)
        {
            try
            {
                return View(Mapper.Map<PaymentInfoViewModel>(_paymentInfoService.GetByIdWithCustomer(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult ReceiveByCashEdit(PaymentInfoViewModel paymentInfoVM)
        {
            try
            {
                _paymentInfoService.ReceiveByCashUpdate(Mapper.Map<PaymentInfo>(paymentInfoVM));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/ReceiveIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region ReceiveByCheque
        [HttpGet]
        public ActionResult ReceiveByCheque(string phone, string cstmrId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                PaymentInfoViewModel paymentInfoVm = new PaymentInfoViewModel();
                string customerName = "";
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                Customer customer = new Customer();
                if (!string.IsNullOrEmpty(phone))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerPhoneWithCustomerIdName(phone, out cstmrId, out customerName, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(identity.CompanyId, identity.BranchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cash.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = identity.CompanyId;
                    paymentInfoVm.BranchId = identity.BranchId;
                }
                else if (!string.IsNullOrEmpty(cstmrId))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerIdWithCustomerName(cstmrId, out customerName, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(identity.CompanyId, identity.BranchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cheque.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = identity.CompanyId;
                    paymentInfoVm.BranchId = identity.BranchId;
                }
                return View(paymentInfoVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult ReceiveByCheque(PaymentInfoViewModel paymentInfoVM)
        {
            try
            {
                _paymentInfoService.ReceiveByCheque(Mapper.Map<PaymentInfo>(paymentInfoVM));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/ReceiveIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region ReceiveByChequeEdit
        [HttpGet]
        public ActionResult ReceiveByChequeEdit(string id)
        {
            try
            {
                return View(Mapper.Map<PaymentInfoViewModel>(_paymentInfoService.GetByIdWithCustomer(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult ReceiveByChequeEdit(PaymentInfoViewModel paymentInfoVM)
        {
            try
            {
                _paymentInfoService.ReceiveByChequeUpdate(Mapper.Map<PaymentInfo>(paymentInfoVM));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/ReceiveIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region CustomerChequeHonour
        public JavaScriptResult CustomerChequeHonour(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    _paymentInfoService.CustomerChequeHonour(id);
                    return JavaScript(
                        $"ShowResult('{"Cheque honoured successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/ReceiveIndex?transactionType=" + TransactionType.Cheque.ToString()}')");
                }

                return JavaScript($"ShowResult('{"Please select the details information."}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{"Other error" + ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region CustomerChequeDishonour
        public JavaScriptResult CustomerChequeDishonour(string id)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(id))
                {
                    _paymentInfoService.CustomerChequeDishonour(id);
                    return JavaScript(
                        $"ShowResult('{"Cheque dishonoured successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/ReceiveIndex?transactionType=" + TransactionType.Cheque.ToString()}')");
                }

                return JavaScript($"ShowResult('{"Please select the details information."}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{"Other error" + ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region ReceiveByBank
        [HttpGet]
        public ActionResult ReceiveByBank(string phone, string cstmrId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                PaymentInfoViewModel paymentInfoVM = new PaymentInfoViewModel();
                string customerName = "";
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                Customer customer = new Customer();
                if (!string.IsNullOrEmpty(phone))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerPhoneWithCustomerIdName(phone, out cstmrId, out customerName, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVM.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVM.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVM.TrackingNo = _paymentInfoService.GenerateTrackingNo(identity.CompanyId, identity.BranchId, "PaymentInfo");
                    paymentInfoVM.TransactionType = TransactionType.Bank.ToString();
                    paymentInfoVM.CustomerId = cstmrId;
                    paymentInfoVM.CustomerName = customerName;
                    paymentInfoVM.TransactionDate = DateTime.Now;
                    paymentInfoVM.CompanyId = identity.CompanyId;
                    paymentInfoVM.BranchId = identity.BranchId;
                }
                else if (!string.IsNullOrEmpty(cstmrId))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerIdWithCustomerName(cstmrId, out customerName, out dueAmount, out advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVM.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVM.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVM.TrackingNo = _paymentInfoService.GenerateTrackingNo(identity.CompanyId, identity.BranchId, "PaymentInfo");
                    paymentInfoVM.TransactionType = TransactionType.Bank.ToString();
                    paymentInfoVM.CustomerId = cstmrId;
                    paymentInfoVM.CustomerName = customerName;
                    paymentInfoVM.TransactionDate = DateTime.Now;
                    paymentInfoVM.CompanyId = identity.CompanyId;
                    paymentInfoVM.BranchId = identity.BranchId;
                }
                return View(paymentInfoVM);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult ReceiveByBank(PaymentInfoViewModel paymentInfoVM)
        {
            try
            {
                if (paymentInfoVM.Amount > 0)
                {
                    _paymentInfoService.ReceiveByBank(Mapper.Map<PaymentInfo>(paymentInfoVM));
                    return JavaScript(
                        $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/PaymentInfo/ReceiveIndex"}')");
                }

                return JavaScript($"ShowResult('{"Amount 0 is not valid value!"}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
