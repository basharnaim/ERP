using System;

namespace Library.ViewModel.Inventory.Purchases
{
    public class PurchaseReturnDetailViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public int Sequence { get; set; }
        public DateTime PurchaseReturnDetailDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal PurchasePrice { get; set; }
        public decimal TotalAmount { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        public bool Select { get; set; }
        public string PurchaseId { get; set; }
        public string PurchaseDetailId { get; set; }
        public decimal PurchaseQuantity { get; set; }
        public decimal PurchaseAmount { get; set; }
        public decimal ReturnQuantity { get; set; }
        public decimal ReturnUnitPrice { get; set; }
        public decimal ReturnAmount { get; set; }
        public decimal RemainingQuantity { get; set; }
        public decimal AlreadyReturnQuantity { get; set; }

        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string UOMId { get; set; }
        public string UOMName { get; set; }
        public string GradeId { get; set; }
        public string ColorId { get; set; }
        public string SupplierId { get; set; }
        public string BrandId { get; set; }
        public string PurchaseReturnId { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductSubCategoryId { get; set; }
        public string ProductSubsidiaryCategoryId { get; set; }
        public string ProductId { get; set; }
        #endregion
    }
}
