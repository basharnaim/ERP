#region Using
using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Customers;
using Library.Model.Inventory.Suppliers;
using Library.Service.Core.Core;
using Library.Context.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#endregion

namespace Library.Service.Inventory.Accounts
{
    /// <summary>
    /// Class CustomerService.
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public class PaymentInfoService : Service<PaymentInfo>, IPaymentInfoService
    {
        #region Ctor
        private readonly IRepository<PaymentInfo> _paymentInfoRepository;
        private readonly IRepository<SupplierLedger> _supplierLedgerRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<CustomerLedger> _customerLedgerRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<BankLedger> _bankLedgerRepository;
        private readonly IRawSqlService _rawSqlService;
        private readonly IUnitOfWork _unitOfWork;
        public PaymentInfoService(
            IRepository<PaymentInfo> paymentInfoRepository,
            IRepository<SupplierLedger> supplierLedgerRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<CustomerLedger> customerLedgerRepository,
            IRepository<Customer> customerRepository,
            IRepository<BankLedger> bankLedgerRepository,
            IRawSqlService rawSqlService,
            IUnitOfWork unitOfWork
            ) : base(paymentInfoRepository)
        {
            _paymentInfoRepository = paymentInfoRepository;
            _supplierLedgerRepository = supplierLedgerRepository;
            _supplierRepository = supplierRepository;
            _customerLedgerRepository = customerLedgerRepository;
            _customerRepository = customerRepository;
            _bankLedgerRepository = bankLedgerRepository;
            _rawSqlService = rawSqlService;
            _unitOfWork = unitOfWork;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GenerateTrackingNo(string companyId, string branchId, string tableName)
        {
            try
            {
                return base.GenerateTrackingNo(companyId, branchId, tableName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GenerateMoneyReceiveNo(string companyId, string branchId, string tableName)
        {
            try
            {
                return base.GenerateMoneyReceiveNo(companyId, branchId, tableName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        public void GetDueOrAdvanceAmountBySupplierId(string supplierId, out decimal dueAmount, out decimal advanceAmount)
        {
            try
            {
                decimal localDueAmount = 0m;
                decimal localAdvanceAmount = 0m;
                decimal sumOfCreditAmount = 0m;
                decimal sumOfDebitAmount = 0m;
                List<SupplierLedger> supplierledger = _supplierLedgerRepository.GetAll(x => !x.Archive && x.SupplierId == supplierId).ToList();
                sumOfCreditAmount = supplierledger.Sum(x => x.CreditAmount);
                sumOfDebitAmount = supplierledger.Sum(x => x.DebitAmount);
                if (sumOfCreditAmount > sumOfDebitAmount)
                    localDueAmount = sumOfCreditAmount - sumOfDebitAmount;
                else
                    localAdvanceAmount = sumOfDebitAmount - sumOfCreditAmount;
                dueAmount = localDueAmount;
                advanceAmount = localAdvanceAmount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        public void GetDueOrAdvanceAmountByCustomerId(string customerId, out decimal dueAmount, out decimal advanceAmount)
        {
            try
            {
                decimal localDueAmount = 0m;
                decimal localAdvanceAmount = 0m;
                decimal sumOfCreditAmount = 0m;
                decimal sumOfDebitAmount = 0m;
                var customerledger = _customerLedgerRepository.GetAll(x => !x.Archive && x.CustomerId == customerId).ToList();
                sumOfCreditAmount = customerledger.Sum(x => x.CreditAmount);
                sumOfDebitAmount = customerledger.Sum(x => x.DebitAmount);
                if (sumOfCreditAmount > sumOfDebitAmount)
                    localDueAmount = sumOfCreditAmount - sumOfDebitAmount;
                else
                    localAdvanceAmount = sumOfDebitAmount - sumOfCreditAmount;
                dueAmount = localDueAmount;
                advanceAmount = localAdvanceAmount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void PayByCash(PaymentInfo paymentInfo)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                Supplier supplier = _supplierRepository.GetOne(x => x.Id == paymentInfo.SupplierId);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                paymentInfo.Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.Sequence = GetAutoSequence("PaymentInfo");
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    paymentInfo.InvoiceNo = paymentInfo.InvoiceNo.Trim();
                }
                paymentInfo.Active = true;
                paymentInfo.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.SupplierPhone = supplier.Phone1;
                
                paymentInfo.TransactionBy = TransactionBy.Supplier.ToString();
                paymentInfo.SynchronizationType = SynchronizationType.Server.ToString();
                paymentInfo.AddedBy = identity.Name;
                paymentInfo.AddedDate = DateTime.Now;
                paymentInfo.AddedFromIp = identity.IpAddress;

                SupplierLedger supplierLedger = new SupplierLedger
                {
                    CompanyId = paymentInfo.CompanyId,
                    BranchId = paymentInfo.BranchId,
                    Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "SupplierLedger"),
                    Sequence = GetAutoSequence("SupplierLedger")
                };
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    supplierLedger.PurchaseId = paymentInfo.InvoiceNo.Trim();
                }
                supplierLedger.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "SupplierLedger");
                supplierLedger.MoneyReceiveNo = GenerateMoneyReceiveNo(paymentInfo.CompanyId, paymentInfo.BranchId, "SupplierLedger");
                supplierLedger.TransactionType = paymentInfo.TransactionType;
                supplierLedger.TransactionDate = DateTime.Now;
                supplierLedger.SupplierId = paymentInfo.SupplierId;
                supplierLedger.SupplierPhone = supplier.Phone1;
                supplierLedger.Particulars = paymentInfo.Particulars;
                supplierLedger.DebitAmount = paymentInfo.Amount;
                supplierLedger.CreditAmount = 0;
                supplierLedger.RunningBalance = paymentInfo.Amount;
                supplierLedger.Active = true;
                if (paymentInfo.IsDiscount)
                {
                    supplierLedger.IsDiscount = true;
                }
                supplierLedger.SynchronizationType = SynchronizationType.Server.ToString();
                supplierLedger.AddedBy = identity.Name;
                supplierLedger.AddedDate = DateTime.Now;
                supplierLedger.AddedFromIp = identity.IpAddress;
                _supplierLedgerRepository.Add(supplierLedger);
                paymentInfo.SupplierLedgerId = supplierLedger.Id;
                _unitOfWork.SaveChanges();
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                GetDueOrAdvanceAmountBySupplierId(paymentInfo.SupplierId, out dueAmount, out advanceAmount);
                paymentInfo.DueAmount = dueAmount;
                paymentInfo.AdvanceAmount = advanceAmount;
                _paymentInfoRepository.Add(paymentInfo);
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateSupplierLedgerRunningBalance(supplier?.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void PayByCashUpdate(PaymentInfo paymentInfo)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                Supplier supplier = _supplierRepository.GetOne(x => x.Id == paymentInfo.SupplierId);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _paymentInfoRepository.GetOne(x => x.Id == paymentInfo.Id);
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                paymentInfo.SupplierPhone = dbdata.SupplierPhone;
                paymentInfo.SupplierType = dbdata.SupplierType;
                paymentInfo.Sequence = dbdata.Sequence;
                paymentInfo.Active = dbdata.Active;
                paymentInfo.Archive = dbdata.Archive;
                paymentInfo.TrackingNo = dbdata.TrackingNo;
                paymentInfo.InvoiceNo = dbdata.InvoiceNo;
                paymentInfo.TransactionDate = dbdata.TransactionDate;
                paymentInfo.TransactionBy = dbdata.TransactionBy;


                paymentInfo.SynchronizationType = dbdata.SynchronizationType;
                paymentInfo.AddedBy = dbdata.AddedBy;
                paymentInfo.AddedDate = dbdata.AddedDate;
                paymentInfo.AddedFromIp = dbdata.AddedFromIp;
                paymentInfo.UpdatedBy = identity.Name;
                paymentInfo.UpdatedDate = DateTime.Now;
                paymentInfo.UpdatedFromIp = identity.IpAddress;

                SupplierLedger supplierLedger = _supplierLedgerRepository.GetOne(x => x.Id == dbdata.SupplierLedgerId);
                if (string.IsNullOrEmpty(supplierLedger.CompanyId))
                {
                    supplierLedger.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(supplierLedger.BranchId))
                {
                    supplierLedger.BranchId = identity.BranchId;
                }
                if (paymentInfo.IsDiscount)
                {
                    supplierLedger.IsDiscount = true;
                }
                supplierLedger.Particulars = paymentInfo.Particulars;
                supplierLedger.DebitAmount = paymentInfo.Amount;
                supplierLedger.CreditAmount = 0;
                supplierLedger.RunningBalance = paymentInfo.Amount;
                supplierLedger.UpdatedBy = identity.Name;
                supplierLedger.UpdatedDate = DateTime.Now;
                supplierLedger.UpdatedFromIp = identity.IpAddress;
                _supplierLedgerRepository.Update(supplierLedger);
                _unitOfWork.SaveChanges();
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                GetDueOrAdvanceAmountBySupplierId(paymentInfo.SupplierId, out dueAmount, out advanceAmount);
                paymentInfo.DueAmount = dueAmount;
                paymentInfo.AdvanceAmount = advanceAmount;
                _paymentInfoRepository.Update(paymentInfo);
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateSupplierLedgerRunningBalance(supplier?.Id);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void PayByCheque(PaymentInfo paymentInfo)
        {
            try
            {
                Supplier supplier = _supplierRepository.GetOne(x => x.Id == paymentInfo.SupplierId);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                paymentInfo.Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.Sequence = GetAutoSequence("PaymentInfo");
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    paymentInfo.InvoiceNo = paymentInfo.InvoiceNo.Trim();
                }
                paymentInfo.Active = true;
                paymentInfo.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.SupplierPhone = supplier.Phone1;
                paymentInfo.ChequeDate = paymentInfo.TransactionDate;
                paymentInfo.SynchronizationType = SynchronizationType.Server.ToString();
                paymentInfo.AddedBy = identity.Name;
                paymentInfo.AddedDate = DateTime.Now;
                paymentInfo.AddedFromIp = identity.IpAddress;
                _paymentInfoRepository.Add(paymentInfo);
                _unitOfWork.SaveChanges();
                _rawSqlService.UpdateSupplierLedgerRunningBalance(supplier?.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void PayByChequeUpdate(PaymentInfo paymentInfo)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                Supplier supplier = _supplierRepository.GetOne(x => x.Id == paymentInfo.SupplierId);
                var dbdata = _paymentInfoRepository.GetOne(x => x.Id == paymentInfo.Id);
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                paymentInfo.SupplierPhone = dbdata.SupplierPhone;
                paymentInfo.SupplierType = dbdata.SupplierType;
                paymentInfo.Sequence = dbdata.Sequence;
                paymentInfo.Active = dbdata.Active;
                paymentInfo.Archive = dbdata.Archive;
                paymentInfo.TrackingNo = dbdata.TrackingNo;
                paymentInfo.InvoiceNo = dbdata.InvoiceNo;
                paymentInfo.TransactionBy = dbdata.TransactionBy;
                paymentInfo.TransactionDate = dbdata.TransactionDate;
                paymentInfo.SynchronizationType = dbdata.SynchronizationType;
                paymentInfo.AddedBy = dbdata.AddedBy;
                paymentInfo.AddedDate = dbdata.AddedDate;
                paymentInfo.AddedFromIp = dbdata.AddedFromIp;
                paymentInfo.UpdatedBy = identity.Name;
                paymentInfo.UpdatedDate = DateTime.Now;
                paymentInfo.UpdatedFromIp = identity.IpAddress;
                _paymentInfoRepository.Update(paymentInfo);
                _unitOfWork.SaveChanges();
                _rawSqlService.UpdateSupplierLedgerRunningBalance(supplier?.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfoId"></param>
        public void Honour(string paymentInfoId)
        {
            bool flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                #region PaymentInfo & supplier Ledger
                PaymentInfo paymentInfo = _paymentInfoRepository.GetOne(x => x.Id == paymentInfoId);
                paymentInfo.CheckStatus = CheckStatus.Honour.ToString();
                Supplier supplier = _supplierRepository.GetOne(x => x.Id == paymentInfo.SupplierId);
                SupplierLedger supplierLedger = new SupplierLedger
                {
                    CompanyId = paymentInfo.CompanyId,
                    BranchId = paymentInfo.BranchId,
                    Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "SupplierLedger"),
                    Sequence = GetAutoSequence("SupplierLedger")
                };
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    supplierLedger.PurchaseId = paymentInfo.InvoiceNo.Trim();
                }
                supplierLedger.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "SupplierLedger");
                supplierLedger.MoneyReceiveNo = GenerateMoneyReceiveNo(paymentInfo.CompanyId, paymentInfo.BranchId, "SupplierLedger");
                supplierLedger.TransactionDate = DateTime.Now;
                supplierLedger.SupplierId = paymentInfo.SupplierId;
                supplierLedger.SupplierPhone = supplier.Phone1;
                supplierLedger.TransactionType = paymentInfo.TransactionType;
                supplierLedger.Particulars = paymentInfo.Particulars;
                supplierLedger.DebitAmount = paymentInfo.Amount;
                supplierLedger.CreditAmount = 0;
                supplierLedger.RunningBalance = paymentInfo.Amount;
                supplierLedger.Active = true;
                supplierLedger.SynchronizationType = SynchronizationType.Server.ToString();
                supplierLedger.AddedBy = identity.Name;
                supplierLedger.AddedDate = DateTime.Now;
                supplierLedger.AddedFromIp = identity.IpAddress;
                _supplierLedgerRepository.Add(supplierLedger);
                paymentInfo.SupplierLedgerId = supplierLedger.Id;
                _paymentInfoRepository.Update(paymentInfo);
                #endregion

                #region BankLedger

                BankLedger bankLedger = new BankLedger
                {
                    CompanyId = paymentInfo.CompanyId,
                    BranchId = paymentInfo.BranchId,
                    Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "BankLedger"),
                    Sequence = GetAutoSequence("BankLedger"),
                    TrackingNo =
                        GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "BankLedger"),
                    BankId = paymentInfo.BankId,
                    AccountNo = paymentInfo.AccountNo,
                    TransactionType = BankTransactionType.Payment.ToString(),
                    DebitAmount = 0,
                    CreditAmount = paymentInfo.Amount,
                    ChequeNo = paymentInfo.ChequeNo,
                    ChequeDate = paymentInfo.ChequeDate,
                    Particulars = paymentInfo.Particulars,
                    TransactionDate = paymentInfo.TransactionDate,
                    PaymentInfoId = paymentInfo.Id,
                    SupplierId = supplier.Id,
                    SynchronizationType = SynchronizationType.Server.ToString(),
                    UpdatedBy = identity.Name,
                    UpdatedDate = DateTime.Now,
                    UpdatedFromIp = identity.IpAddress
                };
                _bankLedgerRepository.Add(bankLedger);
                #endregion
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfoId"></param>
        public void Dishonour(string paymentInfoId)
        {
            PaymentInfo paymentInfo = _paymentInfoRepository.GetOne(x => x.Id == paymentInfoId);
            paymentInfo.CheckStatus = CheckStatus.Dishonour.ToString();
            _paymentInfoRepository.Update(paymentInfo);
            _unitOfWork.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void ReceiveByCash(PaymentInfo paymentInfo)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                Customer customer = _customerRepository.GetOne(x => x.Id == paymentInfo.CustomerId);
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                paymentInfo.Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.Sequence = GetAutoSequence("PaymentInfo");
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    paymentInfo.InvoiceNo = paymentInfo.InvoiceNo.Trim();
                }
                paymentInfo.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.CustomerMobileNumber = customer.Phone1;
                

                paymentInfo.Active = true;
                paymentInfo.SynchronizationType = SynchronizationType.Server.ToString();
                paymentInfo.TransactionBy = TransactionBy.Customer.ToString();
                paymentInfo.AddedBy = identity.Name;
                paymentInfo.AddedDate = DateTime.Now;
                paymentInfo.AddedFromIp = identity.IpAddress;

                CustomerLedger customerLedger = new CustomerLedger
                {
                    Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger"),
                    Sequence = GetAutoSequence("CustomerLedger"),
                    CompanyId = paymentInfo.CompanyId,
                    BranchId = paymentInfo.BranchId
                };
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    customerLedger.SaleId = paymentInfo.InvoiceNo.Trim();
                }
                customerLedger.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger");
                customerLedger.MoneyReceiveNo = GenerateMoneyReceiveNo(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger");
                customerLedger.TransactionDate = DateTime.Now;
                customerLedger.TransactionType = paymentInfo.TransactionType;
                customerLedger.CustomerId = paymentInfo.CustomerId;

                customerLedger.CustomerMobileNumber = customer.Phone1;
                customerLedger.Particulars = paymentInfo.Particulars;
                customerLedger.DebitAmount = paymentInfo.Amount;
                customerLedger.CreditAmount = 0;

                customerLedger.Active = true;
                if (paymentInfo.IsDiscount)
                {
                    customerLedger.Discount = true;
                }
                customerLedger.SynchronizationType = SynchronizationType.Server.ToString();
                customerLedger.AddedBy = identity.Name;
                customerLedger.AddedDate = DateTime.Now;
                customerLedger.AddedFromIp = identity.IpAddress;
                _customerLedgerRepository.Add(customerLedger);
                paymentInfo.CustomerLedgerId = customerLedger.Id;
                _unitOfWork.SaveChanges();
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                GetDueOrAdvanceAmountByCustomerId(paymentInfo.CustomerId, out dueAmount, out advanceAmount);
                paymentInfo.DueAmount = dueAmount;
                paymentInfo.AdvanceAmount = advanceAmount;
                _paymentInfoRepository.Add(paymentInfo);
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateCustomerLedgerRunningBalance(customer?.Id);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void ReceiveByCashUpdate(PaymentInfo paymentInfo)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _paymentInfoRepository.GetOne(x => x.Id == paymentInfo.Id);
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                paymentInfo.CustomerMobileNumber = dbdata.CustomerMobileNumber;
                paymentInfo.CustomerType = dbdata.CustomerType;
                paymentInfo.Sequence = dbdata.Sequence;
                paymentInfo.Active = dbdata.Active;
                paymentInfo.Archive = dbdata.Archive;
                paymentInfo.TrackingNo = dbdata.TrackingNo;
                paymentInfo.InvoiceNo = dbdata.InvoiceNo;
                paymentInfo.TransactionType = dbdata.TransactionType;
                paymentInfo.TransactionBy = dbdata.TransactionBy;
                paymentInfo.TransactionDate = dbdata.TransactionDate;
                paymentInfo.SynchronizationType = dbdata.SynchronizationType;
                paymentInfo.AddedBy = dbdata.AddedBy;
                paymentInfo.AddedDate = dbdata.AddedDate;
                paymentInfo.AddedFromIp = dbdata.AddedFromIp;
                paymentInfo.UpdatedBy = identity.Name;
                paymentInfo.UpdatedDate = DateTime.Now;
                paymentInfo.UpdatedFromIp = identity.IpAddress;

                CustomerLedger customerLedger = _customerLedgerRepository.GetOne(x => x.Id == dbdata.CustomerLedgerId);
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                customerLedger.TransactionType = paymentInfo.TransactionType;
                customerLedger.Particulars = paymentInfo.Particulars;
                customerLedger.DebitAmount = paymentInfo.Amount;
                customerLedger.CreditAmount = 0;

                customerLedger.UpdatedBy = identity.Name;
                customerLedger.UpdatedDate = DateTime.Now;
                customerLedger.UpdatedFromIp = identity.IpAddress;
                _customerLedgerRepository.Update(customerLedger);
                _unitOfWork.SaveChanges();
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                GetDueOrAdvanceAmountByCustomerId(paymentInfo.CustomerId, out dueAmount, out advanceAmount);
                paymentInfo.DueAmount = dueAmount;
                paymentInfo.AdvanceAmount = advanceAmount;
                _paymentInfoRepository.Update(paymentInfo);
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        public void CashReturn(PaymentInfo paymentInfo)
        {
            var flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                Customer customer = _customerRepository.GetOne(x => x.Id == paymentInfo.CustomerId);
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                paymentInfo.Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.Sequence = GetAutoSequence("PaymentInfo");
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    paymentInfo.InvoiceNo = paymentInfo.InvoiceNo.Trim();
                }
                paymentInfo.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.CustomerMobileNumber = customer.Phone1;
                

                paymentInfo.Active = true;
                paymentInfo.SynchronizationType = SynchronizationType.Server.ToString();
                paymentInfo.TransactionBy = TransactionBy.Customer.ToString();
                paymentInfo.AddedBy = identity.Name;
                paymentInfo.AddedDate = DateTime.Now;
                paymentInfo.AddedFromIp = identity.IpAddress;

                CustomerLedger customerLedger = new CustomerLedger
                {
                    Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger"),
                    Sequence = GetAutoSequence("CustomerLedger"),
                    CompanyId = paymentInfo.CompanyId,
                    BranchId = paymentInfo.BranchId
                };
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    customerLedger.SaleId = paymentInfo.InvoiceNo.Trim();
                }
                customerLedger.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger");
                customerLedger.MoneyReceiveNo = GenerateMoneyReceiveNo(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger");
                customerLedger.TransactionDate = DateTime.Now;
                customerLedger.TransactionType = paymentInfo.TransactionType;
                customerLedger.CustomerId = paymentInfo.CustomerId;

                customerLedger.CustomerMobileNumber = customer.Phone1;
                customerLedger.Particulars = paymentInfo.Particulars;
                customerLedger.DebitAmount = 0;
                customerLedger.CreditAmount = paymentInfo.Amount;

                customerLedger.Active = true;
                if (paymentInfo.IsDiscount)
                {
                    customerLedger.Discount = true;
                }
                customerLedger.SynchronizationType = SynchronizationType.Server.ToString();
                customerLedger.AddedBy = identity.Name;
                customerLedger.AddedDate = DateTime.Now;
                customerLedger.AddedFromIp = identity.IpAddress;
                _customerLedgerRepository.Add(customerLedger);
                paymentInfo.CustomerLedgerId = customerLedger.Id;
                _unitOfWork.SaveChanges();
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                GetDueOrAdvanceAmountByCustomerId(paymentInfo.CustomerId, out dueAmount, out advanceAmount);
                paymentInfo.DueAmount = dueAmount;
                paymentInfo.AdvanceAmount = advanceAmount;
                _paymentInfoRepository.Add(paymentInfo);
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateCustomerLedgerRunningBalance(customer?.Id);

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void ReceiveByCheque(PaymentInfo paymentInfo)
        {
            try
            {
                #region PaymentInfo
                Customer customer = _customerRepository.GetOne(x => x.Id == paymentInfo.CustomerId);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                paymentInfo.Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.Sequence = GetAutoSequence("PaymentInfo");
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    paymentInfo.InvoiceNo = paymentInfo.InvoiceNo.Trim();
                }
                paymentInfo.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.CustomerMobileNumber = customer.Phone1;
                paymentInfo.ChequeDate = paymentInfo.TransactionDate;
                paymentInfo.Active = true;

                paymentInfo.TransactionBy = TransactionBy.Customer.ToString();
                paymentInfo.SynchronizationType = SynchronizationType.Server.ToString();
                paymentInfo.AddedBy = identity.Name;
                paymentInfo.AddedDate = DateTime.Now;
                paymentInfo.AddedFromIp = identity.IpAddress;
                _paymentInfoRepository.Add(paymentInfo);
                #endregion
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void ReceiveByChequeUpdate(PaymentInfo paymentInfo)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _paymentInfoRepository.GetOne(x => x.Id == paymentInfo.Id);
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                paymentInfo.CustomerMobileNumber = dbdata.CustomerMobileNumber;
                paymentInfo.CustomerType = dbdata.CustomerType;
                paymentInfo.Sequence = dbdata.Sequence;
                paymentInfo.Active = dbdata.Active;
                paymentInfo.Archive = dbdata.Archive;
                paymentInfo.TrackingNo = dbdata.TrackingNo;
                paymentInfo.InvoiceNo = dbdata.InvoiceNo;
                paymentInfo.TransactionType = dbdata.TransactionType;
                paymentInfo.TransactionBy = dbdata.TransactionBy;
                paymentInfo.TransactionDate = dbdata.TransactionDate;
                paymentInfo.SynchronizationType = dbdata.SynchronizationType;
                paymentInfo.AddedBy = dbdata.AddedBy;
                paymentInfo.AddedDate = dbdata.AddedDate;
                paymentInfo.AddedFromIp = dbdata.AddedFromIp;
                paymentInfo.UpdatedBy = identity.Name;
                paymentInfo.UpdatedDate = DateTime.Now;
                paymentInfo.UpdatedFromIp = identity.IpAddress;
                _paymentInfoRepository.Update(paymentInfo);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        public void ReceiveByBank(PaymentInfo paymentInfo)
        {
            bool flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                #region PaymentInfo
                Customer customer = _customerRepository.GetOne(x => x.Id == paymentInfo.CustomerId);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrEmpty(paymentInfo.CompanyId))
                {
                    paymentInfo.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(paymentInfo.BranchId))
                {
                    paymentInfo.BranchId = identity.BranchId;
                }
                paymentInfo.Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.Sequence = GetAutoSequence("PaymentInfo");
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    paymentInfo.InvoiceNo = paymentInfo.InvoiceNo.Trim();
                }
                paymentInfo.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "PaymentInfo");
                paymentInfo.CustomerMobileNumber = customer.Phone1;
                paymentInfo.Active = true;

                paymentInfo.TransactionBy = TransactionBy.Customer.ToString();
                paymentInfo.CheckStatus = CheckStatus.Honour.ToString();
                paymentInfo.SynchronizationType = SynchronizationType.Server.ToString();
                paymentInfo.AddedBy = identity.Name;
                paymentInfo.AddedDate = DateTime.Now;
                paymentInfo.AddedFromIp = identity.IpAddress;

                #endregion

                #region CustomerLedger
                CustomerLedger customerLedger = new CustomerLedger
                {
                    Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger"),
                    Sequence = GetAutoSequence("CustomerLedger"),
                    CompanyId = paymentInfo.CompanyId,
                    BranchId = paymentInfo.BranchId,
                };
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    customerLedger.SaleId = paymentInfo.InvoiceNo.Trim();
                }
                customerLedger.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger");
                customerLedger.MoneyReceiveNo = GenerateMoneyReceiveNo(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger");
                customerLedger.TransactionType = paymentInfo.TransactionType;
                customerLedger.TransactionDate = paymentInfo.TransactionDate;
                customerLedger.CustomerId = customer.Id;

                customerLedger.CustomerMobileNumber = customer.Phone1;
                customerLedger.Particulars = paymentInfo.Particulars;
                customerLedger.DebitAmount = paymentInfo.Amount;
                customerLedger.CreditAmount = 0;

                customerLedger.Active = true;
                customerLedger.SynchronizationType = SynchronizationType.Server.ToString();
                customerLedger.AddedBy = identity.Name;
                customerLedger.AddedDate = DateTime.Now;
                customerLedger.AddedFromIp = identity.IpAddress;
                _customerLedgerRepository.Add(customerLedger);
                paymentInfo.CustomerLedgerId = customerLedger.Id;
                _paymentInfoRepository.Add(paymentInfo);
                #endregion

                #region BankLedger

                BankLedger bankLedger = new BankLedger
                {
                    CompanyId = paymentInfo.CompanyId,
                    BranchId = paymentInfo.BranchId,
                    Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "BankLedger"),
                    Sequence = GetAutoSequence("BankLedger"),
                    TrackingNo =
                        GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "BankLedger"),
                    BankId = paymentInfo.BankId,
                    AccountNo = paymentInfo.AccountNo,
                    TransactionType = BankTransactionType.Deposit.ToString(),
                    DebitAmount = paymentInfo.Amount,
                    CreditAmount = 0,
                    Particulars = paymentInfo.Particulars,
                    TransactionDate = paymentInfo.TransactionDate,
                    PaymentInfoId = paymentInfo.Id,
                    CustomerId = customer.Id,
                    SynchronizationType = SynchronizationType.Server.ToString(),
                    UpdatedBy = identity.Name,
                    UpdatedDate = DateTime.Now,
                    UpdatedFromIp = identity.IpAddress
                };
                _bankLedgerRepository.Add(bankLedger);
                #endregion

                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateCustomerLedgerRunningBalance(customer?.Id);
                _rawSqlService.UpdateBankLedgerRunningBalance(bankLedger?.AccountNo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfoId"></param>
        public void CustomerChequeHonour(string paymentInfoId)
        {
            bool flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                #region PaymentInfo & Customer Ledger
                PaymentInfo paymentInfo = _paymentInfoRepository.GetOne(x => x.Id == paymentInfoId);
                paymentInfo.CheckStatus = CheckStatus.Honour.ToString();
                Customer customer = _customerRepository.GetOne(x => x.Id == paymentInfo.CustomerId);
                CustomerLedger customerLedger = new CustomerLedger
                {
                    Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger"),
                    Sequence = GetAutoSequence("CustomerLedger"),
                    CompanyId = paymentInfo.CompanyId,
                    BranchId = paymentInfo.BranchId
                };
                if (!string.IsNullOrEmpty(paymentInfo.InvoiceNo))
                {
                    customerLedger.SaleId = paymentInfo.InvoiceNo.Trim();
                }
                customerLedger.TrackingNo = GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger");
                customerLedger.MoneyReceiveNo = GenerateMoneyReceiveNo(paymentInfo.CompanyId, paymentInfo.BranchId, "CustomerLedger");
                customerLedger.TransactionType = paymentInfo.TransactionType;
                customerLedger.TransactionDate = paymentInfo.TransactionDate;
                customerLedger.CustomerId = customer.Id;

                customerLedger.CustomerMobileNumber = customer.Phone1;
                customerLedger.Particulars = paymentInfo.Particulars;
                customerLedger.DebitAmount = paymentInfo.Amount;
                customerLedger.CreditAmount = 0;

                customerLedger.Active = true;
                customerLedger.SynchronizationType = SynchronizationType.Server.ToString();
                customerLedger.AddedBy = identity.Name;
                customerLedger.AddedDate = DateTime.Now;
                customerLedger.AddedFromIp = identity.IpAddress;
                _customerLedgerRepository.Add(customerLedger);
                paymentInfo.CustomerLedgerId = customerLedger.Id;
                _paymentInfoRepository.Update(paymentInfo);
                #endregion

                #region BankLedger

                BankLedger bankLedger = new BankLedger
                {
                    CompanyId = paymentInfo.CompanyId,
                    BranchId = paymentInfo.BranchId,
                    Id = GenerateAutoId(paymentInfo.CompanyId, paymentInfo.BranchId, "BankLedger"),
                    Sequence = GetAutoSequence("BankLedger"),
                    TrackingNo =
                        GenerateTrackingNo(paymentInfo.CompanyId, paymentInfo.BranchId, "BankLedger"),
                    BankId = paymentInfo.BankId,
                    AccountNo = paymentInfo.AccountNo,
                    TransactionType = BankTransactionType.Deposit.ToString(),
                    DebitAmount = paymentInfo.Amount,
                    CreditAmount = 0,
                    Particulars = paymentInfo.Particulars,
                    TransactionDate = paymentInfo.TransactionDate,
                    PaymentInfoId = paymentInfo.Id,
                    CustomerId = customer.Id,
                    SynchronizationType = SynchronizationType.Server.ToString(),
                    UpdatedBy = identity.Name,
                    UpdatedDate = DateTime.Now,
                    UpdatedFromIp = identity.IpAddress
                };
                _bankLedgerRepository.Add(bankLedger);
                #endregion
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateCustomerLedgerRunningBalance(customer?.Id);
                _rawSqlService.UpdateBankLedgerRunningBalance(bankLedger?.AccountNo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfoId"></param>
        public void CustomerChequeDishonour(string paymentInfoId)
        {
            try
            {
                PaymentInfo paymentInfo = _paymentInfoRepository.GetOne(x => x.Id == paymentInfoId);
                paymentInfo.CheckStatus = CheckStatus.Dishonour.ToString();
                _paymentInfoRepository.Update(paymentInfo);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public PaymentInfo GetById(string id)
        {
            try
            {
                return _paymentInfoRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PaymentInfo GetByIdWithCustomer(string id)
        {
            try
            {
                return _paymentInfoRepository.GetOne(x => x.Id == id, "Customer");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public PaymentInfo GetByIdWithSupplier(string id)
        {
            try
            {
                return _paymentInfoRepository.GetOne(x => x.Id ==id, "Supplier");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<PaymentInfo> GetAll()
        {
            try
            {
                return _paymentInfoRepository.GetAll(r => !r.Archive, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public IEnumerable<PaymentInfo> GetAll(string companyId)
        {
            try
            {
                return _paymentInfoRepository.GetAll(r => !r.Archive && r.CompanyId == companyId, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<PaymentInfo> GetAll(string companyId, string branchId)
        {
            try
            {
                return _paymentInfoRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public IEnumerable<PaymentInfo> GetAllForSupplier(string companyId, string branchId, string transactionType)
        {
            try
            {
                return _paymentInfoRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.SupplierId != null && r.TransactionType == transactionType, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        public IEnumerable<PaymentInfo> GetAllForCustomer(string companyId, string branchId, string transactionType)
        {
            try
            {
                return _paymentInfoRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerId != null && r.TransactionType == transactionType, "Supplier, Customer").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<PaymentInfo> GetAllForSupplier(string companyId, string branchId, string transactionType, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _paymentInfoRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.SupplierId != null && r.TransactionType == transactionType && r.TransactionDate >= dateFrom && r.TransactionDate <= dateTo, "Supplier").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="transactionType"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<PaymentInfo> GetAllForCustomer(string companyId, string branchId, string transactionType, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _paymentInfoRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerId != null && r.TransactionType == transactionType && r.TransactionDate >= dateFrom && r.TransactionDate <= dateTo, "Supplier, Customer").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> Lists()
        {
            try
            {
                return from r in _paymentInfoRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Particulars };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public IEnumerable<object> Lists(string companyId)
        {
            try
            {
                return from r in _paymentInfoRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId).OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Particulars };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<object> Lists(string companyId, string branchId)
        {
            try
            {
                return from r in _paymentInfoRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId && r.BranchId == branchId).OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Particulars };
            }
            catch
            {
                return null;
            }
        }
    }
}
