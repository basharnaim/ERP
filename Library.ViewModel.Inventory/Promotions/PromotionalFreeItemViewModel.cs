using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Promotions
{
    public class PromotionalFreeItemViewModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
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

        #region Collection
        public List<PromotionalFreeItemDetailViewModel> PromotionalFreeItemDetails { get; set; }
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
