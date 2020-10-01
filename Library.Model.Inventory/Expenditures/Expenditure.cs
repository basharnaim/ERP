#region Using

using Library.Crosscutting.Helper;
using System;

#endregion

namespace Library.Model.Inventory.Expenditures
{
    public class Expenditure : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        public string ExpenseName { get; set; }
        public DateTime ExpenseDate { get; set; }
        public decimal ExpenseAmount { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public bool IsOffline { get; set; }
        public string SynchronizationType { get; set; }
        public string Comment { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string ExpenditureCategoryId { get; set; }
        public string ExpenditureSubCategoryId { get; set; }
        public string ExpenditureSubsidiaryCategoryId { get; set; }
        public virtual ExpenditureCategory ExpenditureCategory { get; set; }
        public virtual ExpenditureSubCategory ExpenditureSubCategory { get; set; }
        public virtual ExpenditureSubsidiaryCategory ExpenditureSubsidiaryCategory { get; set; }
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
