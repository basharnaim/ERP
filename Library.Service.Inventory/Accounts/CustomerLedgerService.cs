#region Using

using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Customers;
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
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class CustomerLedgerService : Service<CustomerLedger>, ICustomerLedgerService
    {
        #region Ctor
        private readonly IRepository<CustomerLedger> _customerLedgerRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRawSqlService _rawSqlService;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerLedgerService(
            IRepository<CustomerLedger> customerLedgerRepository,
            IRepository<Customer> customerRepository,
            IRawSqlService rawSqlService,
            IUnitOfWork unitOfWork
            ) : base(customerLedgerRepository)
        {
            _customerLedgerRepository = customerLedgerRepository;
            _customerRepository = customerRepository;
            _rawSqlService = rawSqlService;
            _unitOfWork = unitOfWork;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerLedger"></param>
        public void Add(CustomerLedger customerLedger)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrEmpty(customerLedger.CompanyId))
                {
                    customerLedger.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(customerLedger.BranchId))
                {
                    customerLedger.BranchId = identity.BranchId;
                }
                customerLedger.Id = GenerateAutoId(customerLedger.CompanyId, customerLedger.BranchId, "CustomerLedger");
                customerLedger.Sequence = GetAutoSequence("CustomerLedger");
                Customer customer = _customerRepository.GetOne(x => x.Id == customerLedger.CustomerId);
                customerLedger.TrackingNo = GenerateTrackingNo(customerLedger.CompanyId, customerLedger.BranchId, "CustomerLedger");
                customerLedger.MoneyReceiveNo = GenerateMoneyReceiveNo(customerLedger.CompanyId, customerLedger.BranchId, "CustomerLedger");
                customerLedger.TransactionDate = DateTime.Now;
                customerLedger.CustomerMobileNumber = customer.Phone1;

                customerLedger.Active = true;
                customerLedger.SynchronizationType = SynchronizationType.Server.ToString();
                customerLedger.AddedBy = identity.Name;
                customerLedger.AddedDate = DateTime.Now;
                customerLedger.AddedFromIp = identity.IpAddress;
                _customerLedgerRepository.Add(customerLedger);
                _unitOfWork.SaveChanges();
                _rawSqlService.UpdateCustomerLedgerRunningBalance(customer.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerLedger"></param>
        public void Update(CustomerLedger customerLedger)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _customerLedgerRepository.GetOne(customerLedger.Id);
                if (string.IsNullOrEmpty(customerLedger.CompanyId))
                {
                    customerLedger.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(customerLedger.BranchId))
                {
                    customerLedger.BranchId = identity.BranchId;
                }
                customerLedger.CustomerMobileNumber = dbdata.CustomerMobileNumber;
                customerLedger.Sequence = dbdata.Sequence;
                customerLedger.Active = dbdata.Active;
                customerLedger.Archive = dbdata.Archive;
                customerLedger.TrackingNo = dbdata.TrackingNo;
                customerLedger.SaleId = dbdata.SaleId;
                customerLedger.MoneyReceiveNo = dbdata.MoneyReceiveNo;
                customerLedger.TransactionDate = dbdata.TransactionDate;
                customerLedger.SynchronizationType = dbdata.SynchronizationType;
                customerLedger.AddedBy = dbdata.AddedBy;
                customerLedger.AddedDate = dbdata.AddedDate;
                customerLedger.AddedFromIp = dbdata.AddedFromIp;
                customerLedger.UpdatedBy = identity.Name;
                customerLedger.UpdatedDate = DateTime.Now;
                customerLedger.UpdatedFromIp = identity.IpAddress;
                _customerLedgerRepository.Update(customerLedger);
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
        public CustomerLedger GetById(string id)
        {
            try
            {
                return _customerLedgerRepository.GetOne(id);
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
        public IEnumerable<CustomerLedger> GetAll()
        {
            try
            {
                return _customerLedgerRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone1"></param>
        /// <returns></returns>
        public IEnumerable<CustomerLedger> GetAll(string phone1)
        {
            try
            {
                return _customerLedgerRepository.GetAll(r => !r.Archive && r.CustomerMobileNumber == phone1).AsEnumerable();
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
        public IEnumerable<CustomerLedger> GetAll(string companyId, string branchId)
        {
            try
            {
                return _customerLedgerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId).OrderByDescending(r => r.Sequence).AsEnumerable();
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
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IEnumerable<CustomerLedger> GetAllCustomerLedgerByCustomerId(string companyId, string branchId, string customerId)
        {
            try
            {
                return _customerLedgerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerId == customerId).AsEnumerable();
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
        /// <param name="customerPhone"></param>
        /// <returns></returns>
        public IEnumerable<CustomerLedger> GetAllCustomerLedgerByCustomerPhone(string companyId, string branchId, string customerPhone)
        {
            try
            {
                return _customerLedgerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerMobileNumber == customerPhone.Trim()).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<CustomerLedger> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _customerLedgerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.TransactionDate >= dateFrom && r.TransactionDate <= dateTo).AsEnumerable();
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="customerPhone"></param>
        /// <returns></returns>
        public IEnumerable<CustomerLedger> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo, string customerPhone)
        {
            try
            {
                return _customerLedgerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.TransactionDate >= dateFrom && r.TransactionDate <= dateTo && r.CustomerMobileNumber == customerPhone).AsEnumerable();
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
                return from r in _customerLedgerRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence)
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
                return from r in _customerLedgerRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId).OrderBy(r => r.Sequence)
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
                return from r in _customerLedgerRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId && r.BranchId == branchId).OrderBy(r => r.Sequence)
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
        /// <returns></returns>

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerMobileNo"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        public void GetDueOrAdvanceAmountByCustomerPhone(string customerMobileNo, out decimal dueAmount, out decimal advanceAmount)
        {
            try
            {
                decimal localDueAmount = 0m;
                decimal localAdvanceAmount = 0m;
                var customerledger = _customerLedgerRepository.GetAll(x => !x.Archive && x.CustomerMobileNumber == customerMobileNo.Trim()).ToList();
                var sumOfCreditAmount = customerledger.Sum(x => x.CreditAmount);
                var sumOfDebitAmount = customerledger.Sum(x => x.DebitAmount);
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
                var customerledger = _customerLedgerRepository.GetAll(x => !x.Archive && x.CustomerId == customerId).ToList();
                var sumOfCreditAmount = customerledger.Sum(x => x.CreditAmount);
                var sumOfDebitAmount = customerledger.Sum(x => x.DebitAmount);
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
        /// <param name="invoiceNo"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        public void GetInvoicewiseDueOrAdvanceAmountByCustomerId(string customerId, string invoiceNo, out decimal dueAmount, out decimal advanceAmount)
        {
            try
            {
                decimal localDueAmount = 0m;
                decimal localAdvanceAmount = 0m;
                var customerledger = _customerLedgerRepository.GetAll(x => !x.Archive && x.CustomerId == customerId).TakeWhile(x => Convert.ToInt64(x.SaleId) <= Convert.ToInt64(invoiceNo)).ToList();
                var sumOfCreditAmount = customerledger.Sum(x => x.CreditAmount );
                var sumOfDebitAmount = customerledger.Sum(x => x.DebitAmount );
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
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public decimal GetTotalCollectionBetweenDate(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                var customerLedgers = _customerLedgerRepository.GetAll(x => !x.Archive && !x.Discount && x.TransactionType != TransactionType.SalesReturn.ToString() && x.CompanyId == companyId && x.BranchId == branchId && x.TransactionDate >= dateFrom && x.TransactionDate <= dateTo);
                return customerLedgers.Sum(x => x.DebitAmount);
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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<string> GetPaymentCollectionIdsByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _customerLedgerRepository.GetAll(x => x.CompanyId == companyId && x.BranchId == branchId && x.TransactionDate >= dateFrom && x.TransactionDate <= dateTo).Select(x => x.Id).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public decimal GetTotalSalesReturnAmountByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _customerLedgerRepository.GetAll(x => !x.Archive && x.Particulars == TransactionType.SalesReturn.ToString() && x.CompanyId == companyId && x.BranchId == branchId && x.TransactionDate >= dateFrom && x.TransactionDate <= dateTo).Sum(x => x.CreditAmount);
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
        /// <param name="customerName"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        public void GetDueOrAdvanceAmountByCustomerIdWithCustomerName(string customerId, out string customerName, out decimal dueAmount, out decimal advanceAmount)
        {
            try
            {
                decimal localDueAmount = 0m;
                decimal localAdvanceAmount = 0m;
                List<CustomerLedger> customerLedgers = _customerLedgerRepository.GetAll(x => !x.Archive && x.CustomerId == customerId).ToList();
                var sumOfCreditAmount = customerLedgers.Sum(x => x.CreditAmount);
                var sumOfDebitAmount = customerLedgers.Sum(x => x.DebitAmount);
                if (sumOfCreditAmount > sumOfDebitAmount)
                    localDueAmount = sumOfCreditAmount - sumOfDebitAmount;
                else
                    localAdvanceAmount = sumOfDebitAmount - sumOfCreditAmount;
                dueAmount = localDueAmount;
                advanceAmount = localAdvanceAmount;
                customerName = _customerRepository.GetOne(x => x.Id == customerId).Name;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerMobileNo"></param>
        /// <param name="customerId"></param>
        /// <param name="customerName"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        public void GetDueOrAdvanceAmountByCustomerPhoneWithCustomerIdName(string customerMobileNo, out string customerId, out string customerName, out decimal dueAmount, out decimal advanceAmount)
        {
            try
            {
                decimal localDueAmount = 0m;
                decimal localAdvanceAmount = 0m;
                List<CustomerLedger> customerLedgers = _customerLedgerRepository.GetAll(x => !x.Archive && x.CustomerMobileNumber == customerMobileNo.Trim()).ToList();
                var sumOfCreditAmount = customerLedgers.Sum(x => x.CreditAmount );
                var sumOfDebitAmount = customerLedgers.Sum(x => x.DebitAmount);
                if (sumOfCreditAmount > sumOfDebitAmount)
                    localDueAmount = sumOfCreditAmount - sumOfDebitAmount;
                else
                    localAdvanceAmount = sumOfDebitAmount - sumOfCreditAmount;
                dueAmount = localDueAmount;
                advanceAmount = localAdvanceAmount;
                var customer = _customerRepository.GetOne(x => x.Phone1 == customerMobileNo);
                customerId = customer.Id;
                customerName = customer.Name;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override int GetAutoSequence()
        {
            try
            {
                return GetAutoSequence("CustomerLedger");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
