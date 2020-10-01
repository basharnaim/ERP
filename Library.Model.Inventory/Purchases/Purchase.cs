using Library.Crosscutting.Helper;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Suppliers;
using System.Collections.Generic;
using System;

namespace Library.Model.Inventory.Purchases
{
    public class Purchase : BaseModel
    {
        #region Ctor
        public Purchase()
        {
            PurchaseDetails = new List<PurchaseDetail>();
            PurchaseReturns = new List<PurchaseReturn>();
        } 
        #endregion

        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string MemoNo { get; set; }
        public DateTime PurchaseDate { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal MemoWiseDiscount { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal DueAmount { get; set; }
        public bool FullyReturned { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; } 
        #endregion

        #region Navigation
        public virtual Branch Branch { get; set; }
        public string BranchId { get; set; }
        public virtual Company Company { get; set; }
        public string CompanyId { get; set; }
        public virtual Supplier Supplier { get; set; }
        public string SupplierId { get; set; }
        public string BrandId { get; set; }
        #endregion

        #region Collection
        public virtual ICollection<PurchaseDetail> PurchaseDetails { get; set; }
        public virtual ICollection<PurchaseReturn> PurchaseReturns { get; set; }
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
