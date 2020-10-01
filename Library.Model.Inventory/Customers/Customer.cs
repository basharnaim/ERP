#region Using

using Library.Crosscutting.Helper;
using Library.Model.Core.Organizations;
using System;

#endregion

namespace Library.Model.Inventory.Customers
{
    public class Customer : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal AccountsPayable { get; set; }
        public decimal AccountsReceivable { get; set; }
        public string AccountCode { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Email { get; set; }
        public string Website { get; set; }
        public bool Member { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Navigation
        public string CountryId { get; set; }
        public string DivisionId { get; set; }
        public string DistrictId { get; set; }
        public string CompanyId { get; set; }
        public virtual Company Company { get; set; }
        public string BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        public string CustomerCategoryId { get; set; }
        public virtual CustomerCategory CustomerCategory { get; set; }
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
