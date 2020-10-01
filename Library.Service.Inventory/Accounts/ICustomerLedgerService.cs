using Library.Model.Inventory.Accounts;
using System;
using System.Collections.Generic;

namespace Library.Service.Inventory.Accounts
{
    /// <summary>
    /// Interface ICustomerService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface ICustomerLedgerService
    {
        /// <summary>
        /// Adds the specified customerLedger.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="customerLedger">The customerLedger.</param>
        void Add(CustomerLedger customerLedger);

        /// <summary>
        /// Updates the specified customerLedger.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="customerLedger">The customerLedger.</param>
        void Update(CustomerLedger customerLedger);

        /// <summary>
        /// Getbies the identifier.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>CustomerLedger.</returns>
        CustomerLedger GetById(string id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;CustomerLedger&gt;.</returns>
        IEnumerable<CustomerLedger> GetAll();


        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<CustomerLedger> GetAll(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        IEnumerable<CustomerLedger> GetAllCustomerLedgerByCustomerId(string companyId, string branchId, string customerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="customerPhone"></param>
        /// <returns></returns>
        IEnumerable<CustomerLedger> GetAllCustomerLedgerByCustomerPhone(string companyId, string branchId, string customerPhone);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <param name="customerPhone"></param>
        /// <returns></returns>
        IEnumerable<CustomerLedger> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo, string customerPhone);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<CustomerLedger> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="phone1"></param>
        /// <returns></returns>
        IEnumerable<CustomerLedger> GetAll(string phone1);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> Lists();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IEnumerable<object> Lists(string companyId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<object> Lists(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetAutoSequence();

        decimal GetTotalSalesReturnAmountByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerMobileNo"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        /// <returns></returns>
        void GetDueOrAdvanceAmountByCustomerPhone(string customerMobileNo, out decimal dueAmount, out decimal advanceAmount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerMobileNo"></param>
        /// <param name="customerId"></param>
        /// <param name="customerName"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        void GetDueOrAdvanceAmountByCustomerPhoneWithCustomerIdName(string customerMobileNo, out string customerId, out string customerName, out decimal dueAmount, out decimal advanceAmount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        void GetDueOrAdvanceAmountByCustomerId(string customerId, out decimal dueAmount, out decimal advanceAmount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="customerName"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        void GetDueOrAdvanceAmountByCustomerIdWithCustomerName(string customerId, out string customerName, out decimal dueAmount, out decimal advanceAmount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        void GetInvoicewiseDueOrAdvanceAmountByCustomerId(string customerId, string invoiceNo, out decimal dueAmount, out decimal advanceAmount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        decimal GetTotalCollectionBetweenDate(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<string> GetPaymentCollectionIdsByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

    }
}
