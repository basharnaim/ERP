using Library.Model.Inventory.Purchases;
using System;
using System.Collections.Generic;

namespace Library.Service.Inventory.Purchases
{
    /// <summary>
    /// Interface IPurchaseReturnService
    /// <remarks>Jahangir Hossain Sheikh, 3-11-2015</remarks>
    /// </summary>
    public interface IPurchaseReturnService
    {
        void ChangePurchaseStatus(string purchaseId);
        void ChangePurchaseReturnStatus(string purchaseReturnId);
        /// <summary>
        /// Adds the specified purchase return.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 3-11-2015</remarks>
        /// <param name="purchaseReturn">The purchase return.</param>
        void Add(PurchaseReturn purchaseReturn);

        /// <summary>
        /// Updates the specified purchase return.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 3-11-2015</remarks>
        /// <param name="purchaseReturn">The purchase return.</param>
        void Update(PurchaseReturn purchaseReturn);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 3-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>PurchaseReturn.</returns>
        PurchaseReturn GetById(string id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 3-11-2015</remarks>
        /// <returns>IEnumerable&lt;PurchaseReturn&gt;.</returns>
        IEnumerable<PurchaseReturn> GetAll();


        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<PurchaseReturn> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<PurchaseReturn> GetAll(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IEnumerable<PurchaseReturn> GetAll(string companyId);

        
        /// <summary>   Gets all purchase return detailby master identifier. </summary>
        ///
        /// <param name="masterId"> Identifier for the master. </param>
        ///
        /// <returns>   all purchase return detailby master identifier. </returns>
        
        IEnumerable<PurchaseReturnDetail> GetAllPurchaseReturnDetailbyMasterId(string masterId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="salesId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        decimal GetAlreadyReturnQty(string purchaseId, string itemId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="salesId"></param>
        /// <param name="itemId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        decimal GetRemainingQty(string purchaseId, string itemId, string id);

        IEnumerable<PurchaseReturn> GetAllPurchaseReturnByInvoiceNo(string invoiceNo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseReturnId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        decimal GetSumOfAlreadyReturnQty(string purchaseId, string itemId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseReturnId"></param>
        /// <param name="itemId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        decimal GetSumOfAlreadyReturnQty(string purchaseId, string itemId, string id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<PurchaseReturn> GetAllForReport();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseReturnDetailId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        decimal GetReturnQty(string purchaseReturnDetailId, string itemId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="purchaseId"></param>
        /// <returns></returns>
        IEnumerable<PurchaseReturnDetail> GetAllPurchaseReturnDetailbyPurchaseId(string purchaseId);
    }
}
