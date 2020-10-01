#region Using

using Library.Crosscutting.Helper;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Products;
using System;

#endregion

namespace Library.Model.Inventory.Sales
{
    public class SaleDetail : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public DateTime SaleDate { get; set; }
        public DateTime SaleDetailDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal ProductStock { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal? DiscountPerUnit { get; set; }
        public decimal? MaxDiscount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? DiscountInPercentage { get; set; }
        public decimal? DiscountInAmount { get; set; }
        public decimal? VatRate { get; set; }
        public decimal? VatAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsCancelled { get; set; }
        public bool IsReturned { get; set; }
        public bool IsDelivered { get; set; }
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
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UOMId { get; set; }
        public Uom Uom { get; set; }
        public string RAMId { get; set; }
        public string ROMId { get; set; }
        public string ColorId { get; set; }
        public string StyleId { get; set; }
        public string GradeId { get; set; }
        public string SupplierId { get; set; }
        public string BrandId { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductSubCategoryId { get; set; }
        public string ProductSubsidiaryCategoryId { get; set; }
        public virtual Branch Branch { get; set; }
        public virtual Company Company { get; set; }
        public virtual Product Product { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ProductSubCategory ProductSubCategory { get; set; }
        public virtual ProductSubsidiaryCategory ProductSubsidiaryCategory { get; set; }
        public string SaleId { get; set; }
        public virtual Sale Sale { get; set; }
        public string SizeId { get; set; }
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
