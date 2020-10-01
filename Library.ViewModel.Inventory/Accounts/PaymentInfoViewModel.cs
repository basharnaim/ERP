using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Accounts
{
    public class PaymentInfoViewModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string TrackingNo { get; set; }
        public string InvoiceNo { get; set; }

        [Required(ErrorMessage = "Account No is required.")]
        public string AccountNo { get; set; }
        public string SupplierId { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierName { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierLedgerId { get; set; }
        public string CustomerLedgerId { get; set; }
        public string SupplierType { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobileNumber { get; set; }
        public string CustomerType { get; set; }
        public string TransactionType { get; set; }
        public string TransactionBy { get; set; }

        [Required(ErrorMessage = "Cheque No is required.")]
        public string ChequeNo { get; set; }
        [Required(ErrorMessage = "Cheque date is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? ChequeDate { get; set; }
        [Required(ErrorMessage = "Bank name is required.")]
        public string BankId { get; set; }
        [Required(ErrorMessage = "Bank Branch name is required.")]
        public string BankBranchId { get; set; }
        public string CheckStatus { get; set; }
        [Required(ErrorMessage = "Particular is required.")]
        public string Particulars { get; set; }
        [Required(ErrorMessage = "Transaction Date is required.")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        [Required(ErrorMessage = "Amount is required.")]
        public decimal Amount { get; set; }
        [Display(Name = "Discount")]
        public bool IsDiscount { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal DueAmount { get; set; }
        #endregion
    }
}