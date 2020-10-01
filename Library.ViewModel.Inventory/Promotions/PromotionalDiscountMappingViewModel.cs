using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Promotions
{
    public class PromotionalDiscountMappingViewModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        [Required(ErrorMessage = "Date From is required.")]
        public DateTime DateFrom { get; set; }
        [Required(ErrorMessage = "Date To is required.")]
        public DateTime DateTo { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        #endregion

        #region Navigation
        public string PromotionalDiscountId { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string PromotionalDiscountName { get; set; }
        public string CustomerId { get; set; }
        public string CustomerCode { get; set; }
        public string CustomerName { get; set; }
        public string CustomerPhone1 { get; set; }
        public string CustomerAddress1 { get; set; }
        #endregion
    }
}
