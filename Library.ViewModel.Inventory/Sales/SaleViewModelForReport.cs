using System;

namespace Library.ViewModel.Inventory.Sales
{
    public class SaleViewModelForReport
    {
        #region Scaler
        public string Id { get; set; }
        public string OrderNo { get; set; }
        public DateTime SaleDate { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobileNumber { get; set; }
        public decimal TotalBoxes { get; set; }
        public decimal TotalPcses { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal CustomerDiscountInAmount { get; set; }
        public decimal ProductDiscount { get; set; }
        public decimal OverAllDiscount { get; set; }
        public decimal DiscountRate { get; set; }
        public decimal LoadingCharge { get; set; }
        public decimal TransportCharge { get; set; }
        public decimal OtherCharge { get; set; }
        public decimal TotalVat { get; set; }
        public decimal NetAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public decimal DueAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public bool IsApprove { get; set; }
        public bool IsApproveNeeded { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string Contact { get; set; }
        public string Address1 { get; set; }
        public string PaymentType { get; set; }
        public decimal TotalProft { get; set; }
        public decimal NetProfit { get; set; }
        public decimal CustomerPreviousDueAmount { get; set; }
        public decimal CustomerPreviousAdvanceAmount { get; set; }
        public decimal CustomerCurrentDueAmount { get; set; }
        public decimal CustomerCurrentAdvanceAmount { get; set; }
        public decimal CustomerPoint { get; set; }
        public decimal CustomerPointAmount { get; set; }
        public decimal EarningPoint { get; set; }
        public decimal EarningPointAmount { get; set; }
        public decimal ExpensePoint { get; set; }
        public decimal ExpensePointAmount { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Description { get; set; }
        #endregion

        #region Navigation
        public string AddedBy { get; set; }
        public string CustomerId { get; set; }
        public string SaleId { get; set; }
        public string CustomerAddress1 { get; set; } 
        public string Email { get; set; }
        #endregion
    }
}