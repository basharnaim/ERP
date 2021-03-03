#region Using

using Library.Crosscutting.Helper;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Products;
using Library.Model.Inventory.Suppliers;
using System;

#endregion

namespace Library.Model.Inventory.Purchases
{
    public class PurchaseDetail : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime PurchaseDetailDate { get; set; }
        public decimal ProductStock { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Returned { get; set; }
        public bool Cancelled { get; set; }
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
        public string WarehouseId { get; set; }
        public string PurchaseId { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductSubCategoryId { get; set; }
        public string ProductSubsidiaryCategoryId { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UOMId { get; set; }
        public virtual Uom Uom { get; set; }
        public string RAMId { get; set; }
        public string ROMId { get; set; }
        public string SizeId { get; set; }
        public string ColorId { get; set; }
        public string StyleId { get; set; }
        public string GradeId { get; set; }
        public string SupplierId { get; set; }
        public string BrandId { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Company Company { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ProductSubCategory ProductSubCategory { get; set; }
        public virtual ProductSubsidiaryCategory ProductSubsidiaryCategory { get; set; }
        public virtual Purchase Purchase { get; set; }
        public virtual Supplier Supplier { get; set; }
        public virtual Size Size { get; set; }
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
