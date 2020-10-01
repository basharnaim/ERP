using Library.ViewModel.Inventory.Customers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Sales
{
    public class SuperShopSaleViewModel
    {
        #region Scalar
        public string Id { get; set; }

        [Required(ErrorMessage = "Sequence is required.")]
        public int Sequence { get; set; }

        [Display(Name = "Order No")]
        public string OrderNo { get; set; }

        [Display(Name = "Sale Type")]
        [Required(ErrorMessage = "Sale type is required.")]
        public string SaleType { get; set; }

        [Display(Name = "Date")]
        [Required(ErrorMessage = "Sale date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime SaleDate { get; set; }

        [Display(Name = "L.Charge")]
        [Required(ErrorMessage = "Loading Charge is required.")]
        public decimal LoadingCharge { get; set; }

        [Display(Name = "T.Charge")]
        [Required(ErrorMessage = "Transport Charge is required.")]
        public decimal TransportCharge { get; set; }

        [Display(Name = "O.Charge")]
        public decimal OthersCharge { get; set; }

        [Display(Name = "Total Quantity")]
        [Required(ErrorMessage = "Total Quantity is required.")]
        public decimal TotalQuantity { get; set; }

        [Display(Name = "Total Price")]
        [Required(ErrorMessage = "Total Price is required.")]
        public decimal TotalPrice { get; set; }

        [Display(Name = "Total Amount")]
        [Required(ErrorMessage = "Total Amount is required.")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "P.Discount(Tk)")]
        [Required(ErrorMessage = "Product Discount is required.")]
        public decimal ProductDiscount { get; set; }

        [Display(Name = "Net Amount")]
        [Required(ErrorMessage = "Net Amount is required.")]
        public decimal NetAmount { get; set; }


        [Display(Name = "Total Vat")]
        [Required(ErrorMessage = "Total Vat is required.")]
        public decimal TotalVat { get; set; }

        [Display(Name = "Discount In Amount")]
        public decimal DiscountInAmount { get; set; }

        [Display(Name = "Discount(%)")]
        public decimal DiscountInPercentage { get; set; }
        public decimal CustomerDiscountInPercentage { get; set; }
        public decimal CustomerDiscountInAmount { get; set; }

        [Display(Name = "Net Payble")]
        public decimal NetPayble { get; set; }

        [Display(Name = "Paid Amount")]
        [Required(ErrorMessage = "Paid amount is required")]
        public decimal PaidAmount { get; set; }

        [Display(Name = "Earning Point")]
        public decimal EarningPoint { get; set; }

        [Display(Name = "Earning Point Amount")]
        public decimal EarningPointAmount { get; set; }
        public decimal ExpensePoint { get; set; }
        public decimal ExpensePointAmount { get; set; }

        [Display(Name = "Due Amount")]
        public decimal DueAmount { get; set; }

        [Display(Name = "O.Discount(Tk)")]
        public decimal OverAllDiscount { get; set; }
        public decimal TotalDiscount { get; set; }

        [Display(Name = "B.Amount")]
        public decimal ChangeAmount { get; set; }

        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        public string Type { get; set; }


        public bool Active { get; set; }

        public bool Archive { get; set; }

        public bool IsSynchronized { get; set; }

        public bool IsUpdated { get; set; }

        public string SynchronizationType { get; set; }

        public bool IsApprovedNeeded { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        [Display(Name = "Total Profit")]
        public decimal TotalProfit { get; set; }
        [Display(Name = "Net Profit")]
        public decimal NetProfit { get; set; }
        public decimal? TotalBoxes { get; set; }
        public decimal? TotalPcses { get; set; }

        public string ActionType { get; set; }

        public string ProductCode { get; set; }
        public string AddedBy { get; set; }
        #endregion

        #region Payment Type
        [Display(Name = "Payment Type.")]
        public string PaymentType { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string BankName { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string SupplierId { get; set; }
        public string CustomerId { get; set; }
        [Required(ErrorMessage = "Customer is required.")]
        public string CustomerMobileNumber { get; set; }
        public decimal CustomerDueAmount { get; set; }
        public decimal CustomerAdvanceAmount { get; set; }
        public decimal CustomerPoint { get; set; }
        public decimal CustomerPointAmount { get; set; }
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerType { get; set; }
        public string CustomerEmail { get; set; }
        public bool SelectAll { get; set; }

        [Display(Name = "Delivery date")]
        [Required(ErrorMessage = "Delivery date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }

        #endregion

        #region List
        public List<SuperShopSaleDetailViewModel> SaleDetails { get; set; }
        public List<SuperShopSaleDetailViewModel> DeliveredHistoryItems { get; set; }
        public CustomerViewModel Customer { get; set; } 
        
        #endregion

        #region Extra
        public decimal TotalPcsQty { get; set; }
        public decimal TotalSftQty { get; set; }
        public decimal TotalCollection { get; set; }
        public decimal TotalPayment { get; set; }
        public decimal TotalSalesReturnAmount { get; set; }
        public decimal TotalCashReturnAmount { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal Due { get; set; }
        public string UOMId { get; set; }
        public string UOMName { get; set; }
        public decimal TotalExpense { get; set; }
        public decimal TotalCash { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public decimal TotalBankDepositAmountByCustomer { get; set; }
        public decimal TotalBankDepositAmountByBranch { get; set; }
        public decimal TotalBankWithdrawnAmountByBranch { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; } 
        public string CourierName { get; set; }
        [Display(Name = "Product")]
        public string ProductId { get; set; }
        #endregion
    }
}
