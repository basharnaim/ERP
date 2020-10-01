using Library.Model.Inventory.Accounts;
using System;
using System.Collections.Generic;

namespace Library.Service.Inventory.Accounts
{
    /// <summary>
    /// Interface ICustomerService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface ISupplierLedgerService
    {
        
        void Add(SupplierLedger supplierLedger);

        
        void Update(SupplierLedger supplierLedger);

        
        SupplierLedger GetById(string id);

        
        IEnumerable<SupplierLedger> GetAll();

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<SupplierLedger> GetAll(string companyId);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<SupplierLedger> GetAll(string companyId, string branchId);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="supplierPhone"></param>
        /// <returns></returns>
        IEnumerable<SupplierLedger> GetAllSupplierLedgerBySupplierPhone(string companyId, string branchId, string supplierPhone);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="supplierId"></param>
        /// <returns></returns>
        IEnumerable<SupplierLedger> GetAllSupplierLedgerBySupplierId(string companyId, string branchId, string supplierId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierMobileNo"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        void GetDueOrAdvanceAmountBySupplierPhone(string supplierMobileNo, out decimal dueAmount, out decimal advanceAmount);

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
        /// <param name="supplierId"></param>
        /// <param name="supplierName"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        void GetDueOrAdvanceAmountBySupplierIdWithSupplierName(string supplierId, out string supplierName, out decimal dueAmount, out decimal advanceAmount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<SupplierLedger> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);
    }
}
