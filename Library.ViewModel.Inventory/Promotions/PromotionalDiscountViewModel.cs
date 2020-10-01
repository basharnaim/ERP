using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Promotions
{
    public class PromotionalDiscountViewModel
    {
        #region Scaler
        [Required(ErrorMessage = "Promotional Discount is Required.")]
        public string Id { get; set; }
        public int Sequence { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Display(Name = "Date From")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateFrom { get; set; }
        [Display(Name = "Date To")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DateTo { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        #endregion

        #region Navigation
        public string ProductId { get; set; }
        public string WeightId { get; set; }
        #endregion

        #region List
        public List<PromotionalDiscountDetailViewModel> PromotionalDiscountDetails { get; set; }
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