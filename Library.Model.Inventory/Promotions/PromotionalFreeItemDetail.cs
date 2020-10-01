using Library.Crosscutting.Helper;
using Library.Model.Inventory.Products;
using System;

namespace Library.Model.Inventory.Promotions
{
    public sealed class PromotionalFreeItemDetail : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string BatchNo { get; set; }
        public int EligibleQuantity { get; set; }
        public int FreeQuantity { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Navigation
        public string PromotionalFreeItemId { get; set; }
        public string ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public Product Product { get; set; }
        public string FreeItemId { get; set; }
        public string FreeItemName { get; set; }
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
