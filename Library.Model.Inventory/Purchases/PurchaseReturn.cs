using Library.Crosscutting.Helper;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Suppliers;
using System;
using System.Collections.Generic;

namespace Library.Model.Inventory.Purchases
{
    public class PurchaseReturn : BaseModel
    {
        #region Ctor
        public PurchaseReturn()
        {
            PurchaseReturnDetails = new List<PurchaseReturnDetail>();
        }
        #endregion

        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string SupplierId { get; set; }
        public string PurchaseId { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime PurchaseReturnDate { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsFullyReturned { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Navigation
        public virtual Branch Branch { get; set; }
        public virtual Company Company { get; set; }
        public virtual Purchase Purchase { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual ICollection<PurchaseReturnDetail> PurchaseReturnDetails { get; set; }
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
