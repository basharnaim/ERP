using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Accounts
{
    public class CustomerLedgerViewModel
    {
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string InvoiceNo { get; set; }
        public string MoneyReceiveNo { get; set; }
        public string TransactionType { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime TransactionDate { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerMobileNumber { get; set; }
        [Required(ErrorMessage = "Amount is required")]
        public decimal? DebitAmount { get; set; }
        public decimal? CreditAmount { get; set; }
        public decimal? RunningBalance { get; set; }


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

        public decimal AdvanceAmount { get; set; }
        public decimal DueAmount { get; set; }
        public string TrackingNo { get; set; }
        [Required(ErrorMessage="Particuler is required")]
        public string Particulars { get; set; }
        [Display(Name="C.Name")]
        public string CustomerName { get; set; }
        [Display(Name = "C.Address")]
        public string CustomerAddress { get; set; }
        [Display(Name = "C.Type")]
        public string CustomerType { get; set; }
        [Display(Name = "Discount")]
        public bool IsDiscount { get; set; }
    }

}