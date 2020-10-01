using Library.Crosscutting.Helper;
using System;

namespace Library.Model.Inventory.Accounts
{
    public class BankLedger : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string PaymentInfoId { get; set; }
        public string CustomerId { get; set; }
        public string SupplierId { get; set; }
        public string TrackingNo { get; set; }
        public string BankId { get; set; }
        public string BankBranchId { get; set; }
        public string AccountNo { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        public string Particulars { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; } 
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
