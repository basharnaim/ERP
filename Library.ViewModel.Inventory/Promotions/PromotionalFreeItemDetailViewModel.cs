using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Promotions
{
    public sealed class PromotionalFreeItemDetailViewModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        [Required(ErrorMessage = "Batch No")]
        public string BatchNo { get; set; }
        [Display(Name = "Eligible Quantity")]
        [Range(1, int.MaxValue, ErrorMessage = "Bigger than 0")]
        public int EligibleQuantity { get; set; }
        [Required(ErrorMessage = "Free Quantity.")]
        public int FreeQuantity { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        #endregion

        #region Navigation
        public string PromotionalFreeItemId { get; set; }
        [Required(ErrorMessage = "Product Type Name.")]
        public string ItemTypeId { get; set; }
        public string ItemTypeName { get; set; }
        [Required(ErrorMessage = "Product Name(SKU).")]
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        [Required(ErrorMessage = "Product Name(SKU).")]
        public string ProductName { get; set; }
        [Required(ErrorMessage = "Promotional free Item.")]
        public string FreeItemId { get; set; }
        public string FreeItemName { get; set; }
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
