using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Purchases
{
    public class MobileShopPurchaseDetailViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public decimal Sequence { get; set; }
        public DateTime? TargetDate { get; set; }
        [Display(Name = "Invoice Quantity")]
        public decimal? InvoiceQuantity { get; set; }
        [Display(Name = "Purchase DetailDate")]
        public DateTime PurchaseDetailDate { get; set; }
        [Required(ErrorMessage = "Quantity is required.")]
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal TotalAmount { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "PurchaseType is required.")]
        public string PurchaseType { get; set; }
        public string InputSize { get; set; }
        public string Grade { get; set; }
        public string Unit { get; set; }
        public decimal ProductStock { get; set; }
        public decimal PurchaseProfit { get; set; }
        
        public decimal CostOfGoods { get; set; }
        public decimal MaximumRetailPrice { get; set; }
        #endregion

        #region Navigation
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
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        [Display(Name = "UOM")]
        public string UOMId { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        #endregion

        #region Extra
        public bool Select { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal DeliveryQuantity { get; set; }
        public string UOMName { get; set; }
        public string PurchaseNo { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PurchaseDate { get; set; }
        [Display(Name = "Total Quantity")]
        public decimal TotalQuantity { get; set; }
        public string SizeId { get; set; }
        public string SizeName { get; set; }
        public decimal ProfitAmount { get; set; }
        #endregion

    }
}
