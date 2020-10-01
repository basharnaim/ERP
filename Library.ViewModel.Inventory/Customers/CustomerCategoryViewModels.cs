using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Customers
{
    /// <summary>
    /// This Class is CustomerCategory.
    /// </summary>
    /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
    public class CustomerCategoryViewModel 
    {       
        #region Scalar
        
        /// <summary>
        /// This is the customer category identifier.
        /// Db Data Type: decimal.
        /// Db Data length: 18,2
        /// Key: UK
        ///  <remarks>Jahangir, 07-11-15</remarks>
        /// </summary>
        /// <value> The identifier. </value>
        
        public string Id { get; set; }

        /// <summary>
        /// This field is for forcely row ordering.
        /// Db Data Type : Decimal
        /// Db Data Length : 18,2
        /// Allow null : No
        /// Key : UK
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
        /// <value>The sequence.</value>
        [Required(ErrorMessage="Sequence is required.")]
        public int Sequence { get; set; }

        /// <summary>
        /// Gets or sets the code.
        /// This is Customer Category code
        /// Db Data Type: nvarchar
        /// Db Data Length: 10
        /// Allow null :No
        /// Key: UK 
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
        /// <value>The code.</value>
        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(10, ErrorMessage = "Code cannot be longer than 10 characters.")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// This is Customer Category name
        /// Db Data Type: nvarchar
        /// Db Data Length: 100
        /// Allow null : No
        /// Key: UK 
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
        /// <value>The name.</value>
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the comments.
        /// Db Data Type: nvarchar
        /// Db Data Length: 500
        /// Allow null : Yes
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
        /// <value>The comments.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is active.
        /// Db Data Type: bit
        /// Allow null : No 
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
        /// <value><c>true</c> if this instance is active; otherwise, <c>false</c>.</value>
        [Display(Name="Active")]
        public bool Archive { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is archive.
        /// Db Data Type: bit
        /// Allow null : No 
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
        /// <value><c>true</c> if this instance is archive; otherwise, <c>false</c>.</value>
        public bool Active { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is synchronized.
        /// Db Data Type: bit
        /// Allow null : No 
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
        /// <value><c>true</c> if this instance is synchronized; otherwise, <c>false</c>.</value>
        public bool IsSynchronized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is updated.
        /// Db Data Type: bit
        /// Allow null : No 
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
        /// <value><c>true</c> if this instance is updated; otherwise, <c>false</c>.</value>
        public bool IsUpdated { get; set; }

        /// <summary>
        /// Gets or sets the type of the synchronization.
        /// Db Data Type: nvarchar
        /// Db Data length: 8
        /// Allow null : No
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
        /// <value>The type of the synchronization.</value>
        public string SynchronizationType { get; set; }
        [Display(Name="Discount(%)")]
        [Required(ErrorMessage = "Discount rate is required.")]
        public decimal DiscountRate { get; set; }
        

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
        /// Allow null : yes, ? marks indecate the allow null yes
        /// Key: FK 
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
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
        /// <remarks>Jahangir Hossain Sheikh, 27-10-15</remarks>
        /// <value>The branch identifier.</value>
        [Required(ErrorMessage = "Branch name is required.")]
        public int? BranchId { get; set; }        

        #endregion
    }
}
