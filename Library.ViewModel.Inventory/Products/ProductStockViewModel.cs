namespace Library.ViewModel.Inventory.Products
{
    public class ProductStockViewModel
    {
        #region Scaler
        public string Id { get; set; }
        public string value { get; set; }
        public string label { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public int ReorderLevel { get; set; }
        public int ShelfLife { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal WholeSalePrice { get; set; }
        public decimal VatRate { get; set; }
        public decimal ProductStock { get; set; }
        public decimal ProductStockValue { get; set; }
        public decimal ProductSaleValue { get; set; }

        #endregion

        #region Navigation
        public string UOMId { get; set; }
        public string UOMName { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }
        public string ProductSubCategoryId { get; set; }
        public string ProductSubCategoryName { get; set; }
        public string ProductSubsidiaryCategoryId { get; set; }
        public string ProductSubsidiaryCategoryName { get; set; }
        public string SizeId { get; set; }
        public string SizeName { get; set; }
        public string GradeId { get; set; }
        public string GradeName { get; set; }
        public string RackId { get; set; }
        public string RackName { get; set; }
        #endregion
    }
}
