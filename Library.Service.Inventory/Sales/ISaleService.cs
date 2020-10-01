using System;
using System.Collections.Generic;
using Library.Model.Core.Core;
using Library.Model.Inventory.Sales;

namespace Library.Service.Inventory.Sales
{
    public interface ISaleService
    {
        #region Interface

        string GenerateAutoId(string companyId, string branchId, string tableName);
        
        string Add(Sale sale);


        
        string Update(Sale sale);
        

        
        void Approve(Sale sale);

        
        Sale GetById(string id);

        
        IEnumerable<Sale> GetAll();

        
        IEnumerable<Sale> GetAllForReport();

        IEnumerable<Sale> GetAllForReport(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllByCustomer(string customerId);

        IEnumerable<Sale> GetAllByCompany(string companyId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAll(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        IEnumerable<Sale> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo, string customerId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="customerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCustomerIdWithDate(string companyId, string branchId, string customerId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="invoiceNo"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByInvoiceNoWithDate(string companyId, string branchId, string invoiceNo, DateTime dateFrom, DateTime dateTo);

        IEnumerable<Sale> GetAllInvoiceByCustomerNameWithDate(string companyId, string branchId, string customerName,
            DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCompanyBranchCustomerId(string companyId, string branchId, string customerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCompanyCustomerId(string companyId, string customerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="customerPhone"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCustomerPhoneWithDate(string companyId, string branchId, string customerPhone, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="invoiceNo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByInvoiceNo(string companyId, string branchId, string invoiceNo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="customerName"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCustomerName(string companyId, string branchId, string customerName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="customerPhone"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCustomerPhone(string companyId, string branchId, string customerPhone);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Sale> GetAllForApprove();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllForApprove(string companyId);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<Sale> GetAllForApprove(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllForApprove(string companyId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// Jahangir
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllForApprove(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        IEnumerable<SaleDetail> GetAllSaleDetailbyMasterId(string masterId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        IEnumerable<SaleDetail> GetAllSaleDetailbyMasterIdForReport(string masterId);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;Sale&gt;.</returns>
        IEnumerable<Sale> GetAllForDelivery();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<Sale> GetAllForReportDelivery();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllForDelivery(string companyId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllForReportDelivery(string companyId);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<Sale> GetAllForDelivery(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllForReportDelivery(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllForDelivery(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllForReportDelivery(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterId"></param>
        /// <returns></returns>
        IEnumerable<SaleDetail> GetAllSaleDetailForDeliberybyMasterId(string masterId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<string> GetSalesIdsByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        IEnumerable<string> GetSalesIdsByCompanyBranchIdWithCustomerId(string companyId, string branchId, string customerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        IEnumerable<string> GetSalesIdsByCompanyCustomerId(string companyId, string customerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<string> GetSalesIdsByCompanyAndBranchId(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<string> GetSalesIdsByCompanyIdWithDateRange(string companyId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IEnumerable<string> GetSalesIdsByCompanyId(string companyId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<string> GetSalesIdsByCustomerIdWithDateRange(string customerId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        IEnumerable<string> GetSalesIdsByCustomerId(string customerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="customerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<string> GetSalesIdsByCompanyBranchCustomerIdWithDate(string companyId, string branchId, string customerId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCompanyAndBranchId(string companyId, string branchId);

        /// <summary>
        /// /
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCompanyIdWithDateRange(string companyId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCompanyId(string companyId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCustomerIdWithDateRange(string customerId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        IEnumerable<Sale> GetAllInvoiceByCustomerId(string customerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Collectionvm> GetCollectionInformation(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleId"></param>
        /// <returns></returns>
        decimal GetNetProfit(string saleId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        decimal GetAllPendingSales(string companyId, string branchId);

        #endregion
    }
}
