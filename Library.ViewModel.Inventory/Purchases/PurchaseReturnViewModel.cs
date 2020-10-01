using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Purchases
{
    public class PurchaseReturnViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public int Sequence { get; set; }
        [Display(Name = "Return Date")]
        [Required(ErrorMessage = "Purchase Return date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime PurchaseReturnDate { get; set; }
        [Display(Name = "Total Quantity")]
        [Required(ErrorMessage = "Total quantity is required.")]
        public decimal TotalQuantity { get; set; }
        [Display(Name = "Total price")]
        [Required(ErrorMessage = "Total price is required.")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Total Amount")]
        [Required(ErrorMessage = "Total amount is required.")]
        public decimal TotalAmount { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        public string InvoiceNo { get; set; }

        [Display(Name = "Purchase Date")]
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]        
        public DateTime PurchaseDate { get; set; }
        public bool SelectAll { get; set; }

        #endregion

        #region Navigation
        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyId { get; set; }
        [Required(ErrorMessage = "Branch name is required.")]
        public string BranchId { get; set; }
        [Required(ErrorMessage = "Supplier name is required.")]
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        [Required(ErrorMessage = "Purchase name is required.")]
        public string PurchaseId { get; set; }
        #endregion

        #region List
        public List<PurchaseReturnDetailViewModel> PurchaseReturnDetails { get; set; }
        #endregion
    }
}
