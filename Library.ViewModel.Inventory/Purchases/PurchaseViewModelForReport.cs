using System;

namespace Library.ViewModel.Inventory.Purchases
{
    public class PurchaseViewModelForReport
    {
        #region Scaler
        public string Id { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string Address1 { get; set; }
        public string Phone1 { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal MemoWiseDiscount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        #endregion
    }
}
