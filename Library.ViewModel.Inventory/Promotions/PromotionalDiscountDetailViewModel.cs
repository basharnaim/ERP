using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Promotions
{
    public class PromotionalDiscountDetailViewModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        [Required(ErrorMessage = "Batch No")]
        public string BatchNo { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Bigger than 0")]
        public int EligibleQuantity { get; set; }
        [Display(Name = "Discount In Percentage")]
        public decimal DiscountInPercentage { get; set; }
        [Display(Name = "Discount In Amount")]
        public decimal DiscountInAmount { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        #endregion

        #region Navigation
        public string PromotionalDiscountId { get; set; }
        [Required(ErrorMessage = "Product Name(SKU).")]
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        [Required(ErrorMessage = "Product Name(SKU).")]
        public string ProductName { get; set; }
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