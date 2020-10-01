#region Using

using Library.Crosscutting.Helper;
using System;
using System.Collections.Generic;

#endregion

namespace Library.Model.Inventory.Customers
{
    public class CustomerCategory : BaseModel
    {
        #region Ctor
        public CustomerCategory()
        {
            Customers = new List<Customer>();
        }
        #endregion

        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public decimal DiscountRate { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Collection
        public virtual ICollection<Customer> Customers { get; set; }
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
