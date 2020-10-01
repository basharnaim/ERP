using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Accounts
{
    public class SupplierLedgerViewModel
    {
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierAddress { get; set; }
        public string SupplierType { get; set; }
        public string TrackingNo { get; set; }
        public string InvoiceNo { get; set; }
        public string MoneyReceiveNo { get; set; }
        [Required(ErrorMessage = "Particulars is required.")]
        public string Particulars { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        [Required(ErrorMessage = "Amount is required.")]
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? RunningBalance { get; set; }
        [Display(Name = "Discount")]
        public bool IsDiscount { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string AddedBy { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AddedDate { get; set; }
        public string AddedFromIp { get; set; }
        public string UpdatedBy { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedFromIp { get; set; }
        public string Description { get; set; }

        public decimal DueAmount { get; set; }
        public decimal AdvanceAmount { get; set; }
    }
}