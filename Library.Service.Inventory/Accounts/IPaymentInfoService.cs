using Library.Model.Inventory.Accounts;
using System;
using System.Collections.Generic;

namespace Library.Service.Inventory.Accounts
{
    /// <summary>
    /// Interface ICustomerService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IPaymentInfoService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GenerateTrackingNo(string companyId, string branchId, string tableName);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string GenerateMoneyReceiveNo(string companyId, string branchId, string tableName);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        void GetDueOrAdvanceAmountBySupplierId(string supplierId, out decimal dueAmount, out decimal advanceAmount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        void GetDueOrAdvanceAmountByCustomerId(string customerId, out decimal dueAmount, out decimal advanceAmount);

        
        void PayByCash(PaymentInfo paymentInfo);

        
        void PayByCashUpdate(PaymentInfo paymentInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        void PayByCheque(PaymentInfo paymentInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        void PayByChequeUpdate(PaymentInfo paymentInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfoId"></param>
        void Honour(string paymentInfoId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfoId"></param>
        void Dishonour(string paymentInfoId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        void ReceiveByCash(PaymentInfo paymentInfo);

        void CashReturn(PaymentInfo paymentInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        void ReceiveByCashUpdate(PaymentInfo paymentInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        void ReceiveByCheque(PaymentInfo paymentInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        void ReceiveByChequeUpdate(PaymentInfo paymentInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfo"></param>
        void ReceiveByBank(PaymentInfo paymentInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfoId"></param>
        void CustomerChequeHonour(string paymentInfoId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="paymentInfoId"></param>
        void CustomerChequeDishonour(string paymentInfoId);

        /// <summary>
        /// Getbies the identifier.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>PaymentInfo.</returns>
        PaymentInfo GetById(string id);

        PaymentInfo GetByIdWithCustomer(string id);

        PaymentInfo GetByIdWithSupplier(string id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;PaymentInfo&gt;.</returns>
        IEnumerable<PaymentInfo> GetAll();

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<PaymentInfo> GetAll(string companyId);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<PaymentInfo> GetAll(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="paymentType"></param>
        /// <returns></returns>
        IEnumerable<PaymentInfo> GetAllForSupplier(string companyId, string branchId, string transactionType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="transactionType"></param>
        /// <returns></returns>
        IEnumerable<PaymentInfo> GetAllForCustomer(string companyId, string branchId, string transactionType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="paymentType"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<PaymentInfo> GetAllForSupplier(string companyId, string branchId, string transactionType, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="transactionType"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<PaymentInfo> GetAllForCustomer(string companyId, string branchId, string transactionType, DateTime dateFrom, DateTime dateTo);

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
    }
}
