#region Using
using Library.Crosscutting.Helper;
using Library.Model.Inventory.Suppliers;
using System;

#endregion

namespace Library.Model.Inventory.Products
{
    public class Product : BaseModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        public string SupplierProductCode { get; set; }
        public string Name { get; set; }
        public byte[] Picture { get; set; }   
        public decimal PurchasePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal WholeSalePrice { get; set; }
        public decimal ProfitAmountInPercentage { get; set; }
        public decimal ProfitAmount { get; set; }
        public int ShelfLife { get; set; }
        public int ReorderLevel { get; set; }
        public decimal MaxDiscount { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        #endregion

        #region Navigation
        public string ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }
        public string ProductSubCategoryId { get; set; }
        public ProductSubCategory ProductSubCategory { get; set; }
        public string ProductSubsidiaryCategoryId { get; set; }
        public ProductSubsidiaryCategory ProductSubsidiaryCategory { get; set; }
        public string UOMId { get; set; }
        public Uom Uom { get; set; }
        public string ColorId { get; set; }
        public Color Color { get; set; }
        public string FlavorId { get; set; }
        public Flavor Flavor { get; set; }
        public string GradeId { get; set; }
        public Grade Grade { get; set; }
        public string FloorId { get; set; }
        public Floor Floor { get; set; }
        public string RackId { get; set; }
        public Rack Rack { get; set; }
        public string ManufacturerId { get; set; }
        public Manufacturer Manufacturer { get; set; }
        public string VatCategoryId { get; set; }
        public VatCategory VatCategory { get; set; }
        public string SizeId { get; set; }
        public Size Size { get; set; }
        public string RAMId { get; set; }
        public RAM RAM { get; set; }
        public string ROMId { get; set; }
        public ROM ROM { get; set; }
        public string SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public string BrandId { get; set; }
        public Brand Brand { get; set; }
        public string CountryId { get; set; }
        public string StyleId { get; set; }
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
