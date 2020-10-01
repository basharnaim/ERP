using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Sales
{
    
    public class SaleReturnDetailViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public string SaleDetailId { get; set; }
        public string SaleId { get; set; }
        public string WarehouseId { get; set; }
        public int Sequence { get; set; }
        public DateTime SaleReturnDetailDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ReturnUnitPrice { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? SoldBoxes { get; set; }
        public decimal? SoldPcses { get; set; }
        public decimal? SoldQuantity { get; set; }
        public decimal? RemainingBoxes { get; set; }
        public decimal? RemainingPcses { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal ReturnBoxes { get; set; }
        public decimal ReturnPcses { get; set; }
        public decimal ReturnQuantity { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        public bool Select { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string Unit { get; set; }
        public decimal SoldAmount { get; set; }
        public decimal AlreadyReturnQuantity { get; set; }
        public decimal ReturnAmount { get; set; }
        public string CustomerType { get; set; }
        public decimal? DiscountPerUnit { get; set; }
        public decimal? DiscountAmount { get; set; }

        #endregion

        #region Audit
        [Display(Name = "Created by")]
        public string CreatedBy { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Created date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime CreatedDate { get; set; }
        public string CreatedIP { get; set; }

        [Display(Name = "Modified by")]
        public string ModifiedBy { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Modified date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedDate { get; set; }
        public string ModifiedIP { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string SaleReturnId { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductSubCategoryId { get; set; }
        public string ProductSubsidiaryCategoryId { get; set; }
        public string ProductId { get; set; }
        public string UOMId { get; set; }
        public string UOMName { get; set; }

        #endregion
    }
}
