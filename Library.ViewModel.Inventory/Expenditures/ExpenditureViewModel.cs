using System;
using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Expenditures
{
    public class ExpenditureViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        [Display(Name = "Ex.Name")]
        [Required(ErrorMessage = "Ex.Name is required.")]
        public string ExpenseName { get; set; }
        public string SubExpenseName { get; set; }
        [Display(Name = "Date")]
        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd-MMM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ExpenseDate { get; set; }
        [Display(Name = "Ex.Amount")]
        [Required(ErrorMessage = "Ex.Amount is required.")]
        public decimal? ExpenseAmount { get; set; }
        public string Comment { get; set; }

        
        
        public bool Archive { get; set; }

        
        public bool Active { get; set; }

        
        public bool IsSynchronized { get; set; }

        
        public bool IsUpdated { get; set; }

        
        public string SynchronizationType { get; set; }

        
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
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        [Required(ErrorMessage = "Expenditure category is required.")]
        public string ExpenditureCategoryId { get; set; }
        public string ExpenditureCategoryName { get; set; }
        public string ExpenditureSubCategoryId { get; set; }
        public string ExpenditureSubCategoryName { get; set; }
        public string ExpenditureSubsidiaryCategoryId { get; set; }
        public string ExpenditureSubsidiaryCategoryName { get; set; }
        #endregion
    }
}