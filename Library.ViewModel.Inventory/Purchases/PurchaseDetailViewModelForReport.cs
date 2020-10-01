using System;

namespace Library.ViewModel.Inventory.Purchases
{
    public class PurchaseDetailViewModelForReport
    {
        #region Scaler
        public string Id { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime PurchaseDetailDate { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string PurchaseId { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string UOMId { get; set; }
        public string UOMName { get; set; }
        #endregion
    }
}