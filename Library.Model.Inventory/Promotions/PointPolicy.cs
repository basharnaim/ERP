using Library.Crosscutting.Helper;
using System;
using System.Collections.Generic;

namespace Library.Model.Inventory.Promotions
{
    public class PointPolicy : BaseModel
    {
        #region Ctor
        public PointPolicy()
        {
            PointPolicyDetails = new List<PointPolicyDetail>();
        }
        #endregion

        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string Name { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Collection
        public virtual ICollection<PointPolicyDetail> PointPolicyDetails { get; set; }
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
