using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Sales
{
    public class MedicineShopSaleDetailViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public decimal ProductStock { get; set; }
        public decimal TaxRate { get; set; }
        public decimal Sequence { get; set; }
        public DateTime SaleDetailDate { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        [DisplayFormat(DataFormatString = "{0:n2}")]
        public decimal TotalAmount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal TotalVat { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal GrossDiscount { get; set; }
        public decimal OverAllDiscount { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        public string SaleType { get; set; }
        public decimal? DiscountPerUnit { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountInPercentage { get; set; }
        public decimal? DiscountInAmount { get; set; }
        public decimal? VatInPercentage { get; set; }
        public decimal? VatInAmount { get; set; }
        public decimal? VatPerUnit { get; set; }
        public decimal? VatAmount { get; set; }
        public string CustomerType { get; set; }
        #endregion

        #region Navigation

        public string CompanyId { get; set; }

        public string BranchId { get; set; }

        public string SaleId { get; set; }

        public string ProductCategoryId { get; set; }

        public string ProductSubCategoryId { get; set; }

        public string ProductSubsidiaryCategoryId { get; set; }

        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }

        public string UOMId { get; set; }
        public string UOMName { get; set; }

        public string SizeId { get; set; }
        public string SizeName { get; set; }
       

        #endregion

        #region Delibery
        public bool Select { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal DeliveryQuantity { get; set; }
        public string OrderNo { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SaleDate { get; set; }
        [Display(Name = "Total Quantity")]
        public decimal TotalQuantity { get; set; }
        #endregion
    }
}