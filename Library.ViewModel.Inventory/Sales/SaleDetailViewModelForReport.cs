namespace Library.ViewModel.Inventory.Sales
{
    public class SaleDetailViewModelForReport
    {
        #region Scaler
        public string Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal ProductStock { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal SalePrice { get; set; }
        public decimal ReturnPrice { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal DiscountInAmount { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string SaleId { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string GradeId { get; set; }
        public string GradeName { get; set; }
        public string SizeId { get; set; }
        public string SizeName { get; set; }
        #endregion
    }
}