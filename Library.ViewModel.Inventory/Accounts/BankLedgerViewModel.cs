#region Using

using System;
using System.ComponentModel.DataAnnotations;

#endregion

namespace Library.ViewModel.Inventory.Accounts
{
    public class BankLedgerViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string PaymentInfoId { get; set; }
        public string CustomerId { get; set; }
        public string SupplierId { get; set; }
        [Required(ErrorMessage = "Account No is required.")]
        public string TrackingNo { get; set; }
        public string AccountNo { get; set; }
        public string ChequeNo { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ChequeDate { get; set; }
        [Required(ErrorMessage = "Particular is required.")]
        public string Particulars { get; set; }
        [Display(Name = "Transaction Type")]
        [Required(ErrorMessage = "Transaction Type is required.")]
        public string TransactionType { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        [Required(ErrorMessage = "Amount is required.")]
        public decimal Amount { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        
        
        public string Email { get; set; }
        public string Website { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        
        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }
        [Required(ErrorMessage = "Division is required.")]
        public string Division { get; set; }
        [Required(ErrorMessage = "District is required.")]
        public string District { get; set; }
        [Display(Name = "Postal code")]
        public string PostalCode { get; set; }
        public decimal BalanceAmount { get; set; }
        public decimal OverDrafAmount { get; set; }
        public decimal RunningBalance { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        [Required(ErrorMessage = "Bank is required.")]
        public string BankId { get; set; }
        public string BankBranchId { get; set; }
        #endregion
    }
}