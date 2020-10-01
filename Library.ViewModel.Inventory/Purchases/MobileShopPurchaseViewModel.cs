using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Purchases
{
    /// <summary>
    /// Class Purchase.
    /// <remarks>Jahangir, 31-10-2015</remarks>
    /// </summary>
    public class MobileShopPurchaseViewModel
    {
        #region Scalar
        public string Id { get; set; }
        [Required(ErrorMessage = "Sequence is required.")]
        public decimal Sequence { get; set; }
        [Display(Name = "Memo No.")]
        public string MemoNo { get; set; }
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
        [Required(ErrorMessage = "Amount is reruired.")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Memo Discount")]
        public decimal MemoWiseDiscount { get; set; }
        public decimal NetAmount { get; set; }
        [Display(Name = "Others Charge")]
        public decimal OthersCharge { get; set; }
        
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        [Display(Name = "Profit")]
        public decimal? TotalPurchaseProfit { get; set; }
        [Display(Name = "Paid Amount")]
        public decimal? PaidAmount { get; set; }
        [Display(Name = "Return Amount")]
        public decimal? ChangeAmount { get; set; }
        [Display(Name = "Due Amount")]
        public decimal? DueAmount { get; set; }
        [Display(Name = "Debit Amount")]
        public decimal? DebitAmount { get; set; }
        [Display(Name = "Credit Amount")]
        public decimal? CreditAmount { get; set; }
        #endregion


        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Supplier is required.")]
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPhone1 { get; set; }
        public string SupplierAddress1 { get; set; }
        #endregion

        #region List
        public List<MobileShopPurchaseDetailViewModel> PurchaseDetails { get; set; }
        #endregion
    }
}
