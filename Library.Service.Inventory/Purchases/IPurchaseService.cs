using Library.Model.Inventory.Purchases;
using System;
using System.Collections.Generic;

namespace Library.Service.Inventory.Purchases
{
    /// <summary>
    /// Interface IPurchaseService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IPurchaseService
    {
        
        string GenerateAutoId(string companyId, string branchId, string tableName);

        /// <summary>
        /// Adds the specified purchase.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="purchase">The purchase.</param>
        void Add(Purchase purchase);

        /// <summary>
        /// Updates the specified purchase.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="purchase">The purchase.</param>
        void Update(Purchase purchase);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Purchase.</returns>
        Purchase GetById(string id);

        IEnumerable<Purchase> GetAllForReport(string id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;Purchase&gt;.</returns>
        IEnumerable<Purchase> GetAll();

        IEnumerable<Purchase> GetAllBySupplier(string supplierId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IEnumerable<Purchase> GetAllByCompany(string companyId);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<Purchase> GetAll(string companyId, string branchId);

        IEnumerable<Purchase> GetAll(string companyId, string branchId, string supplierId);


        /// <summary>   Gets all purchase detailby master identifier. </summary>
        ///
        /// <param name="purchaseId">   Identifier for the purchase. </param>
        ///
        /// <returns>   all purchase detailby master identifier. </returns>

        IEnumerable<PurchaseDetail> GetAllPurchaseDetailbyMasterId(string purchaseId);

        IEnumerable<PurchaseDetail> GetAllPurchaseDetailbyMasterIdForReport(string purchaseId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Purchase> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        IEnumerable<Purchase> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo, string supplierId);
    }
}
