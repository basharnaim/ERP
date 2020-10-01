#region Using

using Library.Crosscutting.Helper;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Customers;
using System;
using System.Collections.Generic;

#endregion

namespace Library.Model.Inventory.Sales
{
    public class SaleReturn : BaseModel
    {
        #region  Ctor
        public SaleReturn()
        {
            SaleReturnDetails = new List<SaleReturnDetail>();
        }
        #endregion

        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public DateTime SalesReturnDate { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsFullyDamage { get; set; }
        public bool IsFullyReturned { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string SaleId { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerId { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Company Company { get; set; }
        public virtual Customer Customer { get; set; }
        #endregion

        #region Collection
        public virtual ICollection<SaleReturnDetail> SaleReturnDetails { get; set; }

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
