#region MyRegion

using Library.Crosscutting.Helper;
using Library.Model.Inventory.Customers;
using Library.Model.Inventory.Suppliers;
using System;

#endregion

namespace Library.Model.Inventory.Accounts
{
    public class PaymentInfo : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string TrackingNo { get; set; }
        public string InvoiceNo { get; set; }
        public string AccountNo { get; set; }
       
        public string TransactionType { get; set; }
        public string TransactionBy { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string CheckStatus { get; set; }
        public string Particulars { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal Amount { get; set; }
        public decimal AdvanceAmount { get; set; }
        public decimal DueAmount { get; set; }
        public bool IsDiscount { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public virtual Customer Customer { get; set; }
        public string CustomerId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string SupplierId { get; set; }
        public string BankId { get; set; }
        public string BankBranchId { get; set; }
        public string SupplierPhone { get; set; }
        public string SupplierType { get; set; }
        public string SupplierLedgerId { get; set; }
        public string CustomerLedgerId { get; set; }
        public string CustomerMobileNumber { get; set; }
        public string CustomerType { get; set; }
        #endregion

        #region Audit
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedFromIp { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedFromIp { get; set; }
        #endregion
    }
}
