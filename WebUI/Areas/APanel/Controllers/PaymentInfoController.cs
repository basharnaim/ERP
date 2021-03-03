#region Using

using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Customers;
using Library.Service.Inventory.Accounts;
using Library.ViewModel.Inventory.Accounts;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

#endregion

namespace ERP.WebUI.Areas.APanel.Controllers
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
        public ActionResult PayIndex(string companyId, string branchId, string transactionType, string dateFrom, string dateTo)
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
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(transactionType) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<PaymentInfoViewModel>>(_paymentInfoService.GetAllForSupplier(companyId, branchId, transactionType, dfrom.Value, dto.Value)));
                }

                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(transactionType))
                {
                    return View(Mapper.Map<IEnumerable<PaymentInfoViewModel>>(_paymentInfoService.GetAllForSupplier(companyId, branchId, transactionType)));
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
        public ActionResult PayByCash(string companyId, string branchId, string splrId)
        {
            try
            {
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
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(companyId, branchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cash.ToString();
                    paymentInfoVm.SupplierId = splrId;
                    paymentInfoVm.SupplierName = supplierName;
                    paymentInfoVm.CompanyId = companyId;
                    paymentInfoVm.BranchId = branchId;
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
        public JavaScriptResult PayByCash(PaymentInfoViewModel paymentInfoVm)
        {
            try
            {
                _paymentInfoService.PayByCash(Mapper.Map<PaymentInfo>(paymentInfoVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/PayIndex"}')");
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
        public JavaScriptResult PayByCashEdit(PaymentInfoViewModel paymentInfoVm)
        {
            try
            {
                _paymentInfoService.PayByCashUpdate(Mapper.Map<PaymentInfo>(paymentInfoVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/PayIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region PayByCheque
        [HttpGet]
        public ActionResult PayByCheque(string companyId, string branchId, string splrId)
        {
            try
            {
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
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(companyId, branchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cheque.ToString();
                    paymentInfoVm.SupplierId = splrId;
                    paymentInfoVm.SupplierName = supplierName;
                    paymentInfoVm.CompanyId = companyId;
                    paymentInfoVm.BranchId = branchId;
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
        public JavaScriptResult PayByCheque(PaymentInfoViewModel paymentInfoVm)
        {
            try
            {
                _paymentInfoService.PayByCheque(Mapper.Map<PaymentInfo>(paymentInfoVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/PayIndex"}')");
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
        public JavaScriptResult PayByChequeEdit(PaymentInfoViewModel paymentInfoVm)
        {
            try
            {
                _paymentInfoService.PayByChequeUpdate(Mapper.Map<PaymentInfo>(paymentInfoVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/PayIndex"}')");
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
                        $"ShowResult('{"Cheque honoured successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/PayIndex?transactionType=" + TransactionType.Cheque.ToString()}')");
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
                        $"ShowResult('{"Cheque dishonoured successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/PayIndex?transactionType=" + TransactionType.Cheque.ToString()}')");
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
        public ActionResult ReceiveIndex(string companyId, string branchId, string transactionType, string dateFrom, string dateTo)
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
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(transactionType) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(Mapper.Map<IEnumerable<PaymentInfoViewModel>>(_paymentInfoService.GetAllForCustomer(companyId, branchId, transactionType, dfrom.Value, dto.Value)));
                }

                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(transactionType))
                {
                    return View(Mapper.Map<IEnumerable<PaymentInfoViewModel>>(_paymentInfoService.GetAllForCustomer(companyId, branchId, transactionType)));
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
        public ActionResult ReceiveByCash(string companyId, string branchId, string cstmrId, string phone)
        {
            try
            {
                PaymentInfoViewModel paymentInfoVm = new PaymentInfoViewModel();
                if (!string.IsNullOrEmpty(cstmrId))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerIdWithCustomerName(cstmrId, out var customerName, out var dueAmount, out var advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(companyId, branchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cash.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = companyId;
                    paymentInfoVm.BranchId = branchId;
                }
                else if (!string.IsNullOrEmpty(phone))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerPhoneWithCustomerIdName(phone, out  cstmrId, out var customerName, out var dueAmount, out var advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(companyId, branchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cash.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = companyId;
                    paymentInfoVm.BranchId = branchId;
                }
                return View(paymentInfoVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult ReceiveByCash(PaymentInfoViewModel paymentInfoVm)
        {
            try
            {
                _paymentInfoService.ReceiveByCash(Mapper.Map<PaymentInfo>(paymentInfoVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/ReceiveIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region ReceiveByCash
        [HttpGet]
        public ActionResult CashReturn(string companyId, string branchId, string cstmrId, string phone)
        {
            try
            {
                PaymentInfoViewModel paymentInfoVm = new PaymentInfoViewModel();
                if (!string.IsNullOrEmpty(cstmrId))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerIdWithCustomerName(cstmrId, out var customerName, out var dueAmount, out var advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(companyId, branchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.CashReturn.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = companyId;
                    paymentInfoVm.BranchId = branchId;
                }
                else if (!string.IsNullOrEmpty(phone))
                {
                    _customerLedgerService.GetDueOrAdvanceAmountByCustomerPhoneWithCustomerIdName(phone, out cstmrId, out var customerName, out var dueAmount, out var advanceAmount);
                    if (dueAmount > 0)
                    {
                        paymentInfoVm.DueAmount = dueAmount;
                    }
                    if (advanceAmount > 0)
                    {
                        paymentInfoVm.AdvanceAmount = advanceAmount;
                    }
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(companyId, branchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.CashReturn.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = companyId;
                    paymentInfoVm.BranchId = branchId;
                }
                return View(paymentInfoVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult CashReturn(PaymentInfoViewModel paymentInfoVm)
        {
            try
            {
                _paymentInfoService.CashReturn(Mapper.Map<PaymentInfo>(paymentInfoVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/ReceiveIndex"}')");
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
        public JavaScriptResult ReceiveByCashEdit(PaymentInfoViewModel paymentInfoVm)
        {
            try
            {
                _paymentInfoService.ReceiveByCashUpdate(Mapper.Map<PaymentInfo>(paymentInfoVm));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/ReceiveIndex"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region ReceiveByCheque
        [HttpGet]
        public ActionResult ReceiveByCheque(string companyId, string branchId, string cstmrId, string phone)
        {
            try
            {
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
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(companyId, branchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cash.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = companyId;
                    paymentInfoVm.BranchId = branchId;
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
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(companyId, branchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Cheque.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.CompanyId = companyId;
                    paymentInfoVm.BranchId = branchId;
                }
                return View(paymentInfoVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult ReceiveByCheque(PaymentInfoViewModel paymentInfoVm)
        {
            try
            {
                _paymentInfoService.ReceiveByCheque(Mapper.Map<PaymentInfo>(paymentInfoVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/ReceiveIndex"}')");
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
        public JavaScriptResult ReceiveByChequeEdit(PaymentInfoViewModel paymentInfoVm)
        {
            try
            {
                _paymentInfoService.ReceiveByChequeUpdate(Mapper.Map<PaymentInfo>(paymentInfoVm));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/ReceiveIndex"}')");
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
                        $"ShowResult('{"Cheque honoured successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/ReceiveIndex?transactionType=" + TransactionType.Cheque.ToString()}')");
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
                        $"ShowResult('{"Cheque dishonoured successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/ReceiveIndex?transactionType=" + TransactionType.Cheque.ToString()}')");
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
        public ActionResult ReceiveByBank(string companyId, string branchId, string phone, string cstmrId)
        {
            try
            {
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
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(companyId, branchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Bank.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.TransactionDate = DateTime.Now;
                    paymentInfoVm.CompanyId = companyId;
                    paymentInfoVm.BranchId = branchId;
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
                    paymentInfoVm.TrackingNo = _paymentInfoService.GenerateTrackingNo(companyId, branchId, "PaymentInfo");
                    paymentInfoVm.TransactionType = TransactionType.Bank.ToString();
                    paymentInfoVm.CustomerId = cstmrId;
                    paymentInfoVm.CustomerName = customerName;
                    paymentInfoVm.TransactionDate = DateTime.Now;
                    paymentInfoVm.CompanyId = companyId;
                    paymentInfoVm.BranchId = branchId;
                }
                return View(paymentInfoVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult ReceiveByBank(PaymentInfoViewModel paymentInfoVm)
        {
            try
            {
                if (paymentInfoVm.Amount > 0)
                {
                    _paymentInfoService.ReceiveByBank(Mapper.Map<PaymentInfo>(paymentInfoVm));
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/PaymentInfo/ReceiveIndex"}')");
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
