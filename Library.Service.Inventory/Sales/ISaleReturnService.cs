using Library.Model.Inventory.Sales;
using System;
using System.Collections.Generic;

namespace Library.Service.Inventory.Sales
{
    public interface ISaleReturnService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleReturnId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        decimal GetSumOfAlreadyReturnQty(string saleId, string itemId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleReturnId"></param>
        /// <param name="itemId"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        decimal GetSumOfAlreadyReturnQty(string saleId, string itemId, string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleReturnDetailId"></param>
        /// <param name="itemId"></param>
        /// <returns></returns>
        decimal GetReturnQty(string saleReturnDetailId, string itemId);
        void ChangeSaleStatus(string saleId);
        
        void ChangeSaleReturnStatus(string saleReturnId, string type);
        
        void Add(SaleReturn saleReturn, string type);
        
        void Update(SaleReturn saleReturn);

        SaleReturn GetById(string id);

        IEnumerable<SaleReturn> GetAll();
        IEnumerable<SaleReturn> GetAllForReport(string id);

        IEnumerable<SaleReturn> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);
        
        IEnumerable<SaleReturn> GetAll(string companyId, string branchId);
        
        IEnumerable<SaleReturn> GetAll(string companyId);

        IEnumerable<SaleReturnDetail> GetAllSaleReturnDetailbyMasterId(string masterId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllSale(string companyId);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<Sale> GetAllSale(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllSale(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        IEnumerable<SaleDetail> GetAllSaleDetailbyMasterId(string saleId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<SaleReturn> GetAllSaleReturnByInvoiceNo(string invoiceNo, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        IEnumerable<SaleReturn> GetAllSaleReturnByInvoiceNo(string invoiceNo);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;SaleReturn&gt;.</returns>
        IEnumerable<SaleReturn> GetAllForReport();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleId"></param>
        /// <returns></returns>
        IEnumerable<SaleReturnDetail> GetAllSaleReturnDetailbySaleId(string saleId);

        IEnumerable<SaleReturnDetail> GetAllSaleDetailbyMasterIdForReport(string masterId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<SaleReturn> GetAllSaleReturnForReport(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<SaleReturn> GetAllSaleReturnForReport(string companyId, string branchId);
    }
}
