using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Purchases
{

    public class SuperShopPurchaseViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public int Sequence { get; set; }
        [Display(Name = "Date")]
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PurchaseDate { get; set; }        
        
        [Display(Name = "Total Quantity")]
        [Required(ErrorMessage = "Quantity is reruired.")]
        public decimal TotalQuantity { get; set; }
        
        [Display(Name = "Total Price")]
        [Required(ErrorMessage = "Price is reruired.")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Total Amount")]
        
        public decimal TotalAmount { get; set; }
        
        [Display(Name = "Others Charge")]
        public decimal OthersCharge { get; set; }
        
        
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }

        [Display(Name = "Memo Discount")]
        [Range(0, 100000, ErrorMessage = "Value must be between 0 and 100000")]
        public decimal? MemoWiseDiscount { get; set; }
        public string Description { get; set; }
        
        
        public string MemoNo { get; set; }
        public string RefNo { get; set; } 
        public decimal NetAmount { get; set; }
        #endregion

        #region Navigation
        [Required(ErrorMessage = "Supplier is reruired.")]
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string BrandId { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string TypeId { get; set; } 
        #endregion

        #region List
        public List<SuperShopPurchaseDetailViewModel> PurchaseDetails { get; set; }
        #endregion
    }
}
