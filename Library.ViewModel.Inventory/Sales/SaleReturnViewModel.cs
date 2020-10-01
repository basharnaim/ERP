using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Sales
{
    public class SaleReturnViewModel
    {
        #region Scalar
        public string Id { get; set; }
        [Required(ErrorMessage = "Sequence is required.")]
        public int Sequence { get; set; }
        [Display(Name = "Return Date")]
        [Required(ErrorMessage = "Sale return date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SalesReturnDate { get; set; }
        
        [Display(Name = "Loading Charge")]
        public decimal LoadingCharge { get; set; }
        
        [Display(Name = "Transport Charge")]
        public decimal TransportCharge { get; set; }
        
        [Display(Name = "Others Charge")]
        public decimal OthersCharge { get; set; }
        
        [Display(Name = "Total Quantity")]
        [Required(ErrorMessage = "Total Quantity is required.")]
        public decimal TotalQuantity { get; set; }
        
        [Display(Name = "Total Amount")]
        [Required(ErrorMessage = "Total Amount is required.")]
        public decimal TotalAmount { get; set; }
        
        
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        [Display(Name = "Sale date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SaleDate { get; set; }
        #endregion


        #region Navigation
        [Display(Name = "Invoice No")]
        public string SaleId { get; set; }
        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyId { get; set; }
        [Required(ErrorMessage = "Branch name is required.")]
        public string BranchId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        #endregion

        #region List
        public List<SaleReturnDetailViewModel> SaleReturnDetails { get; set; }
        #endregion

        #region Extra
        public bool SelectAll { get; set; }
        #endregion
    }
}
