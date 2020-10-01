using Library.Model.Core.Core;
using System;
using System.Collections.Generic;
using System.Data;

namespace Library.Context.Repositories
{
    public interface IRawSqlService
    {
        void UpdateCustomerLedgerRunningBalance(string customerId);
        void UpdateSupplierLedgerRunningBalance(string supplierId);
        void UpdateBankLedgerRunningBalance(string accountNo);
        decimal GetCustomerOpeningBalanceByCustomerId(string customerId, DateTime date);
        decimal GetCustomerOpeningBalanceByCustomerPhone(string customerPhone, DateTime date);
        decimal GetBankLedgerOpeningBalance(string accountNo, DateTime date);
        decimal GetSupershopCurrentStock(string companyId, string branchId, string ProductId);
        decimal GetCeramicStockInSft(string companyId, string branchId, string ProductId);
        decimal GetCeramicStockInBox(string companyId, string branchId, string ProductId);
        decimal GetCeramicStockInPcs(string companyId, string branchId, string ProductId);
        decimal GetStockQty(string companyId, string branchId, string ProductId);
        ProductVm GetProductDetailWithStock(string companyId, string branchId, string productId);
        IEnumerable<PromotionalDiscountVm> GetPromotionalPointAndDiscount(DateTime today);
        IEnumerable<AutoCompleteForWarehouse> GetWareHouseList(string term);
        IEnumerable<ProductVm> GetPurchaseImeiList(string companyId, string branchId);
        IEnumerable<ProductVm> GetProductList(string companyId, string branchId);
        IEnumerable<ProductVm> GetAllSalesProductListWithPositiveStockByCode(string companyId, string branchId);
        IEnumerable<ProductVm> GetAllSalesProductListWithPositiveStockByName(string companyId, string branchId);
        IEnumerable<CeramicProductVm> GetCeramicProductListForPurchase(string companyId, string branchId);
        IEnumerable<CeramicProductVm> GetCeramicProductListForSale(string companyId, string branchId);
        IEnumerable<LowStockProductVm> GetAllLowStockProduct(string companyId, string branchId);
        IEnumerable<ProductStockVm> GetAllProductStock(string companyId, string branchId, string supplierId, string ProductCategoryId, string ProductSubCategoryId, string ProductId, string productCode, string dateFrom, string dateTo);
        IEnumerable<ProductStockVm> GetAllProductStockDetail(string companyId, string branchId, string supplierId, string ProductCategoryId, string ProductSubCategoryId, string ProductId, string dateFrom, string dateTo);
        IEnumerable<ExpenditureVm> GetExpenses(string companyId, string branchId, string dateFrom, string dateTo, string expenditureCategoryId);
        IEnumerable<CustomerLedgerVm> GetAllCustomerLedger(string companyId, string branchId, string dateFrom, string dateTo, string customerType, string phone, string customerId);
        IEnumerable<SupplierLedgerVm> GetAllSupplierLedger(string companyId, string branchId, string dateFrom, string dateTo, string supplierType, string phone, string supplierId);
        IEnumerable<BankLedgerVm> GetAllBankLedger(string companyId, string branchId, string accountNo, string dateFrom, string dateTo);
        IEnumerable<SaleVm> GetAllSalesSummary(string companyId, string branchId, string dateFrom, string dateTo, string customerId);
        IEnumerable<SaleVm> GetAllSaleClosing(string companyId, string branchId, string dateFrom, string dateTo, string saleById);
        IEnumerable<SaleVm> GetCategoryWiseSale(string companyId, string branchId, string supplierId, string dateFrom, string dateTo);
        IEnumerable<SaleVm> GetAllSaleReturnSummary(string companyId, string branchId, string dateFrom, string dateTo, string customerId);
        IEnumerable<SaleVm> GetCounterWiseSale(string companyId, string branchId, string dateFrom, string dateTo);
        IEnumerable<SaleDetailVm> GetAllSaleDetail(string companyId, string branchId, string customerId, string saleNo, string productCategoryId, string dateFrom, string dateTo);
        IEnumerable<PurchaseVm> GetAllPurchaseSummary(string companyId, string branchId, string dateFrom, string dateTo, string supplierId);
        IEnumerable<PurchaseVm> GetAllPurchaseReturnSummary(string companyId, string branchId, string dateFrom, string dateTo, string supplierId);
        IEnumerable<PurchaseDetailVm> GetAllPurchaseDetail(string companyId, string branchId, string dateFrom, string dateTo, string supplierId);
        IEnumerable<CustomerVm> GetAllCustomerInformation(string companyId, string branchId, string customerMobileNumber);
        IEnumerable<ProductVm> GetAllProductForBarcodeSale(string companyId, string branchId);
        IEnumerable<CashBalanceVm> GetCashBalanceBetweenDate(string companyId, string branchId, string dateFrom, string dateTo);
        IEnumerable<AutoComplete> GetSupplierCategoryIdNameForAutoComplete();
        IEnumerable<AutoComplete> GetCustomers(string companyId, string branchId);
        IEnumerable<AutoComplete> GetProductSubCategoryIdNameForAutoComplete(string ProductCategoryId);
        IEnumerable<AutoComplete> GetSupplierIdNameForAutoComplete();
        IEnumerable<AutoComplete> GetProductIdCodeForAutoComplete();
        IEnumerable<AutoComplete> GetProductIdNameForAutoComplete();
        IEnumerable<AutoComplete> GetProductIdCodeForAutoCompleteByCompanyBranchId(string companyId, string branchId);
        IEnumerable<AutoComplete> GetProductIdNameForAutoCompleteByCompanyBranchId(string companyId, string branchId);
        IEnumerable<SaleVm> GetAllSalesForMultiplePrint(string saleIds);
        IEnumerable<SaleDetailVm> GetAllSaleDetailsForMultiplePrint(string saleIds);
        IEnumerable<MultipleInvoicePrint> GetAllForMultiplePrint(string saleIds);
        IEnumerable<MultipleInvoicePrint> GetAllForMultipleLabelPrint(string saleIds);
        IEnumerable<ProductVm> GetReceiveProductForArtisti(string companyId, string branchId);
        IEnumerable<ProductVm> GetReceiveProductForArtisti();
        IEnumerable<ProductVm> GetBranchwiseProductStockAll(string companyId, string branchId);
        IEnumerable<ProductVm> GetBranchwiseProductStockGreaterThanZero(string companyId, string branchId);
        IEnumerable<SaleVm> GetAllSalesForMobileCover(string companyId, string branchId, string dateFrom, string dateTo, string customerId, string courierId, string orderStatus, string phone);
        IEnumerable<SaleVm> GetCategoryWiseItemSales(string companyId, string branchId, string supplierId, string dateFrom, string dateTo);
        IEnumerable<SaleVm> GetCategoryWiseDailySale(string companyId, string branchId, string supplierId, string dateFrom, string dateTo);

        /// <summary>
        /// Return DataTable
        /// </summary>
        /// <returns></returns>
        DataSet GetPurchaseSummary(string companyId, string branchId, string supplierId);

    }
}
