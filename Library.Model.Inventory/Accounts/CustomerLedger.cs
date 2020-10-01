using Library.Crosscutting.Helper;
using System;

namespace Library.Model.Inventory.Accounts
{
    public class CustomerLedger : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string TrackingNo { get; set; }
        public string MoneyReceiveNo { get; set; }
        public string Particulars { get; set; }
        public string TransactionType { get; set; }
        public DateTime TransactionDate { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal RunningBalance { get; set; }
        public decimal? EarningPoint { get; set; }
        public decimal? EarningPointAmount { get; set; }
        public decimal? ExpensePoint { get; set; }
        public decimal? ExpensePointAmount { get; set; }
        public bool Discount { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Navigation
        public string SaleId { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerMobileNumber { get; set; }
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
