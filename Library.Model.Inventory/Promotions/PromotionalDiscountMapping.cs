﻿using System;
using Library.Crosscutting.Helper;
using Library.Model.Inventory.Customers;

namespace Library.Model.Inventory.Promotions
{
    public class PromotionalDiscountMapping : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Navigation
        public string PromotionalDiscountId { get; set; }
        public PromotionalDiscount PromotionalDiscount { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
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
