using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Suppliers
{
    /// <summary>
    /// Class SupplierSubCategory.
    /// <remarks>Jahangir Hossain Sheikh, 28-10-15</remarks>
    /// </summary>
    public class SupplierSubCategoryViewModel
    {
        #region Scalar
        
        
        
        public string Id { get; set; }

        
        [Required(ErrorMessage = "Sequence is required.")]
        public int Sequence { get; set; }

        
        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(10, ErrorMessage = "Code cannot be longer than 10 characters.")]
        public string Code { get; set; }

        
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        
        public string Description { get; set; }

        
        
        public bool Archive { get; set; }

        
        public bool Active { get; set; }

        
        public bool IsSynchronized { get; set; }

        
        public bool IsUpdated { get; set; }

        
        public string SynchronizationType { get; set; }

        #endregion

        #region Audit
        [Display(Name = "Created by")]
        public string AddedBy { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Created date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime AddedDate { get; set; }
        public string AddedFromIp { get; set; }

        [Display(Name = "Modified by")]
        public string UpdatedBy { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Modified date")]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? UpdatedDate { get; set; }
        public string UpdatedFromIp { get; set; }
        #endregion

        #region Navigation

        
        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyId { get; set; }

        
        [Required(ErrorMessage = "Branch name is required.")]
        public string BranchId { get; set; }

        
        [Required(ErrorMessage = "Category is required.")]
        public string SupplierCategoryId { get; set; }

        [Required(ErrorMessage = "Category is required.")]
        public string SupplierCategoryName { get; set; }
        #endregion
    }
}
