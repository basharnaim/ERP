#region Using

using Library.Crosscutting.Helper;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Products;
using System;

#endregion

namespace Library.Model.Inventory.Sales
{
    public class SaleReturnDetail : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string WarehouseId { get; set; }
        public string SaleReturnId { get; set; }
        public string SaleId { get; set; }
        public string SaleDetailId { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductSubCategoryId { get; set; }
        public string ProductSubsidiaryCategoryId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UOMId { get; set; }
        public string SizeId { get; set; }
        public string ColorId { get; set; }
        public string GradeId { get; set; }
        public string SupplierId { get; set; }
        public DateTime? SalesReturnDate { get; set; }
        public DateTime SaleReturnDetailDate { get; set; }
        public decimal? SoldQuantity { get; set; }
        public decimal SoldAmount { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal? RemainingQuantity { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ReturnPrice { get; set; }
        public decimal? DiscountPerUnit { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public bool IsDamage { get; set; }
        public bool IsReturned { get; set; }
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
        public virtual Product Product { get; set; }
        public string ProductId { get; set; }
        public virtual ProductCategory ProductCategory { get; set; }
        public virtual ProductSubCategory ProductSubCategory { get; set; }
        public virtual ProductSubsidiaryCategory ProductSubsidiaryCategory { get; set; }
        public virtual SaleReturn SaleReturn { get; set; }
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
