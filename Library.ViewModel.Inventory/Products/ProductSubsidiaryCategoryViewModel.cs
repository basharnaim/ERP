using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Products
{
    public class ProductSubsidiaryCategoryViewModel 
    {
        #region Scalar
        
        
        
        public string Id { get; set; }

        
        [Required(ErrorMessage="Sequence is required.")]
        public int Sequence { get; set; }

        
        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(10, ErrorMessage = "Code cannot be longer than 10 characters.")]
        public string Code { get; set; }

        
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        
        public string Description { get; set; }

        
        [Display(Name="Active")]
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
        [Required(ErrorMessage = "Product Category is required.")]
        public string ProductCategoryId { get; set; }

        [Required(ErrorMessage = "Product SubCategory is required.")]
        public string ProductSubCategoryId { get; set; }
        #endregion                       
    }
}
