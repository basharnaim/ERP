using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Sales
{
    public class SuperShopSaleDetailViewModel
    {
        #region Scalar
        public string Id { get; set; }

        public int Sequence { get; set; }

        public DateTime SaleDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Sales Detail date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SaleDetailDate { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        public decimal Quantity { get; set; }

        public decimal SalePrice { get; set; }

        public decimal TotalAmount { get; set; }
        
        public string SizeId { get; set; }

        public bool Active { get; set; }

        public bool Archive { get; set; }

        public bool IsSynchronized { get; set; }

        public bool IsUpdated { get; set; }

        public string SynchronizationType { get; set; }

        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:0.##}")]
        public decimal ProductStock { get; set; }

        public decimal? DiscountInPercentage { get; set; }

        public decimal? DiscountInAmount { get; set; }

        public decimal? DiscountPerUnit { get; set; }
        public decimal? DiscountAmount { get; set; }

        public decimal? VatRate { get; set; }
        public decimal? VatAmount { get; set; }

        public decimal? MaxDiscount { get; set; }

        public decimal PurchasePrice { get; set; }
        public decimal Profit { get; set; }
        public decimal RowProfit { get; set; }

        public decimal ProfitPerUnit { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal DeliveryQuantity { get; set; }
        public decimal WarehouseStockQty { get; set; }
        public string WarehouseId { get; set; }
        public string WarehouseName { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string SaleId { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductSubCategoryId { get; set; }
        public string ProductSubsidiaryCategoryId { get; set; }
        [Required(ErrorMessage = "Product is required.")]
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        [Required(ErrorMessage = "Product is required.")]
        public string ProductName { get; set; }
        public string UOMId { get; set; }
        public string UOMName { get; set; }
        public string DeliveryId { get; set; }
        #endregion

        #region Extra
        public bool Select { get; set; }

        #endregion
    }
}
