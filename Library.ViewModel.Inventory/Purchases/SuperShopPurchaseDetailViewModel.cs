using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Purchases
{ 
    public class SuperShopStockOutDetailViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public int Sequence { get; set; }
        [Display(Name = "Purchase DetailDate")]
        public DateTime PurchaseDetailDate { get; set; }
        public decimal Quantity { get; set; }
        [Display(Name = "Purchase DetailDate")]
        public decimal PurchasePrice { get; set; }
        [Display(Name = "Purchase Price")]
        public decimal SalePrice { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        //public string GradeId { get; set; }
        public string GradeName { get; set; }
        public decimal ProductStock { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        #endregion

        #region Navigation
        
        public string StockOutId { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string PurchaseId { get; set; }
        [Display(Name = "Category")]
        public string ProductCategoryId { get; set; }
        [Display(Name = "Sub Category")]
        public string ProductSubCategoryId { get; set; }
        [Display(Name = "Subsidiary Category")]
        public string ProductSubsidiaryCategoryId { get; set; }
        [Display(Name = "Product")]
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        [Display(Name = "UOM")]
        public string UOMId { get; set; }
        public string ProductCode { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal DeliveryQuantity { get; set; }
        public string UOMName { get; set; }
        public string SupplierName { get; set; }
        public DateTime ExpiryDate { get; set; }

        public string RAMId { get; set; }
        public string ROMId { get; set; }
        public string SizeId { get; set; }
        public string ColorId { get; set; }
        public string StyleId { get; set; }
        public string GradeId { get; set; }
        public string SupplierId { get; set; }
        public string BrandId { get; set; }       
        public string StockOutDate { get; set; }
        public string StockOutDetailDate { get; set; }     
        public string StockOutPrice { get; set; }
        #endregion

        #region Extra
        public bool Select { get; set; }
        #endregion
    }
}
