using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Products
{
    public class ProductMasterViewModel
    {
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public string ProductCategoryId { get; set; }
        public string ProductCategoryName { get; set; }

        [Display(Name="Active")]
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string AddedBy { get; set; }
        public DateTime AddedDate { get; set; }
        public string AddedFromIp { get; set; }
        public string UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedFromIp { get; set; }
        public string Description { get; set; }
        public string ProductSubCategoryName { get; set; }
    }
}