namespace Library.ViewModel.Inventory.Products
{
    public class ProductViewModelForReport
    {
        #region Scaler
        public string Id { get; set; }
        public decimal Sequence { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string AccountCode { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal RetailPrice { get; set; }
        public decimal WholeSalePrice { get; set; }
        public decimal ProfitAmountInPercentage { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal LastPurchasePrice { get; set; }
        public decimal CommissionPerUnit { get; set; }
        public string InputSize { get; set; }
        public string OutputSize { get; set; }
        public decimal InputConversion { get; set; }
        public decimal OutputConversion { get; set; }
        public string UpperConversion { get; set; }
        public string LowerConversion { get; set; }
        public decimal ConversionUnit { get; set; }
        public decimal TargetCommision { get; set; }
        public bool IsFixedCommision { get; set; }
        public decimal MesurmentCommision { get; set; }
        public decimal CostPriceForProfit { get; set; }
        public decimal ReorderLevel { get; set; }
        public decimal MaxDiscount { get; set; }
        #endregion

        #region Navigation
        public string ItemCategoryId { get; set; }
        public string ItemSubCategoryId { get; set; }
        public string ItemSubsidiaryCategoryId { get; set; }
        public string UOMId { get; set; }
        public string UOMName { get; set; }
        public string SizeId { get; set; }
        public string ColorId { get; set; }
        public string GradeId { get; set; }
        public string GradeName { get; set; }
        public string RackId { get; set; }
        public string ManufacturerCompanyName { get; set; }
        public string TaxCategoryId { get; set; }
        public string SupplierId { get; set; }
        public string SupplierName { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }
        public string RAMName { get; set; }
        public string ROMName { get; set; }
        #endregion
    }
}