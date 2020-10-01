#region Using

using Library.Crosscutting.Helper;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Products;
using Library.Model.Inventory.Suppliers;
using System;

#endregion

namespace Library.Model.Inventory.Purchases
{
    public class PurchaseReturnDetail : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string PurchaseReturnId { get; set; }
        public string PurchaseId { get; set; }
        public string PurchaseDetailId { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductSubCategoryId { get; set; }
        public string ProductSubsidiaryCategoryId { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UOMId { get; set; }
        public string SizeId { get; set; }
        public string GradeId { get; set; }
        public string ColorId { get; set; }
        public string SupplierId { get; set; }
        public string BrandId { get; set; }
        public DateTime? PurchaseReturnDate { get; set; }
        public DateTime PurchaseReturnDetailDate { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal ReturnPrice { get; set; }
        public decimal ReturnAmount { get; set; }
        public bool IsReturned { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Audit
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedFromIp { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedFromIp { get; set; }
        #endregion

        #region Navigation
        public virtual Branch Branch { get; set; }
        public string BranchId { get; set; }
        public virtual Company Company { get; set; }
        public string CompanyId { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ProductSubCategory ProductSubCategory { get; set; }
        public virtual ProductSubsidiaryCategory ProductSubsidiaryCategory { get; set; }
        public virtual PurchaseReturn PurchaseReturn { get; set; }
        public virtual Supplier Supplier { get; set; }
        #endregion
    }
}
