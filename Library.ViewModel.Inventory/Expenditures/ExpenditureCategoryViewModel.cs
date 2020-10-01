using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Expenditures
{
    public class ExpenditureCategoryViewModel
    {
        #region Scalar
        public int Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        [Display(Name = "Ex.Name")]
        [Required(ErrorMessage = "Ex.Name is required.")]
        public string Name { get; set; }
        [Display(Name = "Date")]
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpenseDate { get; set; }
        [Display(Name = "Ex.Amount")]
        [Required(ErrorMessage = "Ex.Amount is required.")]
        public decimal ExpenseAmount { get; set; }
        public string Comment { get; set; }

        
        /// <summary>   
        /// This is indicating whether this object is active. 
        /// Db Data Type: bit.
        /// Allow Null: No
        /// <remarks>Jahangir, 26-10-15</remarks>
        /// </summary>
        
        
        public bool Archive { get; set; }

        
        /// <summary>   
        /// This is indicating whether this object is archive.
        /// Db Data Type: bit.
        /// Allow Null: No
        /// <remarks>Jahangir, 26-10-15</remarks>
        /// <value> true if this object is archive, false if not. </value>
        ///  </summary>
        
        public bool Active { get; set; }

        
        /// <summary>   
        /// This is indicating whether this object is synchronized. 
        /// Db Data Type: bit.
        /// Allow Null: No
        /// <remarks>Jahangir, 26-10-15</remarks>
        /// <value> true if this object is synchronized, false if not. </value>
        /// </summary>
        
        public bool IsSynchronized { get; set; }

        
        /// <summary>   
        /// This is indicating whether this object is updated. 
        /// Db Data Type: bit.
        /// Allow Null: No
        /// <remarks>Jahangir, 26-10-15</remarks>
        /// <value> true if this object is updated, false if not. </value>
        /// </summary>
        
        public bool IsUpdated { get; set; }

        
        /// <summary>   
        /// This is the synchronization of transport. 
        /// Db Data Type: nvarchar.
        /// Db data length: 8
        /// Allow Null: No
        /// <remarks>Jahangir, 26-10-15</remarks>
        /// <value> The type of the synchronization. </value>
        /// </summary>
        
        public string SynchronizationType { get; set; }

        
        /// <summary>   
        /// This is the sales's comments.
        /// Db Data Type: nvarchar.
        /// Db Data length: 500
        /// <value> The comments. </value>
        /// </summary>
        
        [Required(ErrorMessage = "Phone number is required.")]
        public string CustomerMobileNumber { get; set; }
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
        /// This is the Company identifier.
        /// This is company Id
        /// Db Data Type: int
        /// Allow null : No
        /// Key: FK 
        /// </summary>
        /// <remarks>Jahangir, 26-10-15</remarks>
        /// <value>The company identifier.</value>
                
        public string CompanyId { get; set; }

        
        /// <summary>
        /// This is the branch identifier.
        /// This is Branch Id
        /// Db Data Type: int
        /// Allow null : No
        /// Key: FK 
        /// </summary>
        /// <remarks>Jahangir, 26-10-15</remarks>
        /// <value>The branch identifier.</value>
        
        public string BranchId { get; set; }
        #endregion
    }

}