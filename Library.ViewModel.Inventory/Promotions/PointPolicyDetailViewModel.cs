using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Promotions
{
    public class PointPolicyDetailViewModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        [Display(Name = "Sale Amount")]
        public decimal SaleAmount { get; set; }
        [Display(Name = "Discount Amount")]
        public decimal DiscountAmount { get; set; }
        [Display(Name = "Equivalant Point")]
        public decimal EquivalantPoint { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        #endregion

        #region Navigation
        public string PointPolicyId { get; set; }
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