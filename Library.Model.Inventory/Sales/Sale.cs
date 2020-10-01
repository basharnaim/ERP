using Library.Crosscutting.Helper;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Customers;
using Library.Model.Inventory.Products;
using System;
using System.Collections.Generic;

namespace Library.Model.Inventory.Sales
{
    public class Sale : BaseModel
    {
        #region Ctor
        public Sale()
        {
            SaleDetails = new List<SaleDetail>();
        }
        #endregion

        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public byte[] Barcode { get; set; }
        public string OrderNo { get; set; }
        public DateTime? OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public DateTime SaleDate { get; set; }
        public decimal LoadingCharge { get; set; }
        public decimal TransportCharge { get; set; }
        public decimal? OthersCharge { get; set; }
        public decimal TotalQuantity { get; set; }
        public decimal DiscountInPercentage { get; set; }
        public decimal DiscountInAmount { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? OverAllDiscount { get; set; }
        public decimal ProductDiscount { get; set; }
        public decimal? EarningPoint { get; set; }
        public decimal? EarningPointAmount { get; set; }
        public decimal? ExpensePoint { get; set; }
        public decimal? ExpensePointAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal? TotalProfit { get; set; }
        public decimal? TotalVat { get; set; }
        public decimal NetAmount { get; set; }
        public decimal? RetailTotal { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal ChangeAmount { get; set; }
        public decimal DueAmount { get; set; }
        public bool IsApprovedNeeded { get; set; }
        public bool IsApproved { get; set; }
        public string ApprovedBy { get; set; }
        public bool IsFullyDelivered { get; set; }
        public bool IsFullyCancelled { get; set; }
        public bool IsFullyReturned { get; set; }
        public string PaymentType { get; set; }
        public string PaymentStatus { get; set; }
        public string CardType { get; set; }
        public string CardNumber { get; set; }
        public string BankName { get; set; }
        public string ChequeNo { get; set; }
        public DateTime? ChequeDate { get; set; }
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
        public virtual Branch Branch { get; set; }
        public virtual Company Company { get; set; }
        public virtual Customer Customer { get; set; }
        public string CourierId { get; set; }
        public Courier Courier { get; set; }
        public string CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerMobileNumber { get; set; }
        public string CustomerAddress1 { get; set; }
        public string CustomerAddress2 { get; set; }
        public string CustomerEmail { get; set; }
        public decimal? CustomerDiscountInPercentage { get; set; }
        public decimal? CustomerDiscountInAmount { get; set; }
        public decimal? CustomerDiscountAmount { get; set; }
        public decimal? CustomerTotalDiscountAmount { get; set; }
        public decimal? CustomerDueAmount { get; set; }
        public decimal? CustomerAdvanceAmount { get; set; }
        public decimal? CustomerPoint { get; set; }
        public decimal? CustomerPointAmount { get; set; }
        #endregion

        #region Collection
        public virtual ICollection<SaleDetail> SaleDetails { get; set; }
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
