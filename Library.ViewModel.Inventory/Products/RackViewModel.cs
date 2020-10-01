using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Products
{
    /// <summary>
    /// This Class is Color.
    /// <remarks>Jahangir, 27-10-2015</remarks>
    /// </summary>
    public class RackViewModel
    {
        #region Scalar
        public string Id { get; set; }
        [Required(ErrorMessage = "Sequence is required.")]
        public int Sequence { get; set; }
        [Required(ErrorMessage = "Code is required.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public decimal? AllocationSize { get; set; }
        
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }

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

        /// <summary>
        /// Gets or sets the Company identifier.
        /// This is company Id
        /// Db Data Type: int
        /// Allow null : Yes
        /// Key: FK 
        /// </summary>
        /// <remarks>Jahangir, 27-10-15</remarks>
        /// <value>The company identifier.</value>
        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyId { get; set; }

        /// <summary>
        /// Gets or sets the branch identifier.
        /// This is Branch Id
        /// Db Data Type: int
        /// Allow null : No
        /// Key: FK 
        /// </summary>
        /// <remarks>Jahangir, 27-10-15</remarks>
        /// <value>The branch identifier.</value>
        [Required(ErrorMessage = "Branch name is required.")]
        public string BranchId { get; set; }
        public string FloorId { get; set; }

        #endregion
    }
}
