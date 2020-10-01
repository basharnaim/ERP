using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Sales
{
    public class MedicineShopSaleViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public decimal Sequence { get; set; }
        [Display(Name = "Order No.")]
        public string OrderNo { get; set; }
        [Required(ErrorMessage = "Courier is required.")]
        [Display(Name = "Courier")]
        public string CourierId { get; set; }
        [Display(Name = "Sale Type")]
        public string SaleType { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime SaleDate { get; set; }

        [Display(Name = "Order Date")]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy hh:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? OrderDate { get; set; }

        public string OrderStatus { get; set; }

        [Display(Name = "Total Quantity")]
        public decimal TotalQuantity { get; set; }
        [Display(Name = "Total Price")]
        public decimal TotalPrice { get; set; }
        [Display(Name = "Total Profit")]
        public decimal TotalProfit { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Total Discount(Tk)")]
        public decimal TotalDiscount { get; set; }
        public decimal? OverAllDiscount { get; set; }
        [Display(Name = "Net Amount")]
        public decimal? NetAmount { get; set; }

        [Display(Name = "Discount Amount")]
        public decimal DiscountAmount { get; set; }

        [Display(Name = "Total Vat")]
        public decimal TotalVat { get; set; }

        [Display(Name = "Discount In Amount")]
        public decimal DiscountInAmount { get; set; }
          
        [Display(Name = "Discount(%)")]
        public decimal DiscountInPercentage { get; set; }
        
        [Display(Name = "Net Payble")]
        public decimal NetPayble { get; set; }
        
        [Display(Name = "Paid Amount")]
        [Required(ErrorMessage = "Paid amount is required.")]
        public decimal? PaidAmount { get; set; }

        [Display(Name = "Change Amount")]
        public decimal? ChangeAmount { get; set; }
        [Display(Name = "Due Amount")]
        public decimal? DueAmount { get; set; }

        public DateTime dateFrom { get; set; }
        public DateTime dateTo { get; set; }

        public string Type { get; set; }
        public decimal CustomerDueAmount { get; set; }
        public decimal CustomerAdvanceAmount { get; set; }

        [Display(Name = "Customer Discount(%)")]
        public decimal? CustomerDiscountInPercentage { get; set; }
        [Display(Name = "Customer Discount")]
        public decimal? CustomerDiscountInAmount { get; set; }

        public decimal? CustomerDiscountAmount { get; set; }
        public decimal? CustomerTotalDiscountAmount { get; set; }

        [Display(Name = "Loading Charge")]
        [Required(ErrorMessage = "Loading Charge is required.")]
        public decimal LoadingCharge { get; set; }

        [Display(Name = "Transport Charge")]
        [Required(ErrorMessage = "Transport Charge is required.")]
        public decimal TransportCharge { get; set; }

        [Display(Name = "Others Charge")]
        public decimal OthersCharge { get; set; }

        
        

        
        public bool Active { get; set; }
        
        public bool Archive { get; set; }

        public bool IsSynchronized { get; set; }
        
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }

        public string ActionType { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerType { get; set; }
        public string CustomerName { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        [Required(ErrorMessage = "Phone is required.")]
        public string CustomerMobileNumber { get; set; }
        public string CustomerEmail { get; set; }
        public bool SelectAll { get; set; }
        [Display(Name = "Delivery date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
        public string ItemId { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal GrossDiscount { get; set; }
        #endregion

        #region List
        public List<MedicineShopSaleDetailViewModel> SaleDetails { get; set; }
        #endregion

        #region Extra
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        [Display(Name = "Website")]
        public string BankName { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
        #endregion
    }
}