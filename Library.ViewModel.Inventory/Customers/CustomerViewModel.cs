using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Library.ViewModel.Inventory.Customers
{
    public class CustomerViewModel
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
        [Required(ErrorMessage = "Customer type is required.")]
        public string CustomerType { get; set; }
        public string Description { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Contact")]
        public string ContactPerson { get; set; }
        [Display(Name = "Designation")]
        public string ContactPersonDesignation { get; set; }
        [Required(ErrorMessage = "Address is required.")]
        [Display(Name = "Home Address")]
        public string Address1 { get; set; }
        [Display(Name = "Office Address")]
        public string Address2 { get; set; }
        
        [Required(ErrorMessage = "Country is required.")]
        [Display(Name = "Country")]
        public string CountryId { get; set; }
        [Required(ErrorMessage = "Division is required.")]
        [Display(Name = "Division")]
        public string DivisionId { get; set; }
        [Required(ErrorMessage = "District is required.")]
        [Display(Name = "District")]
        public string DistrictId { get; set; }
        [Required(ErrorMessage = "Phone1 is required.")]
        [Display(Name = "Home Phone")]
        public string Phone1 { get; set; }
        [Display(Name = "Office Phone")]
        public string Phone2 { get; set; }
        
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", ErrorMessage = "Please enter valid email.")]
        public string Email { get; set; }
        public string Website { get; set; }
        
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        [Display(Name = "Discount(%)")]
        [Required(ErrorMessage = "Discount rate is required.")]
        public decimal DiscountRate { get; set; }

        [Display(Name = "Payment Amount")]
        public decimal PaymentAmount { get; set; }
        [Display(Name = "Discount Amount")]
        public decimal DiscountAmount { get; set; }
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal BalanceAmount { get; set; }
        [Display(Name = "Accounts Payable")]
        public decimal? AccountsPayable { get; set; }
        [Display(Name = "Accounts Receivable")]
        public decimal? AccountsReceivable { get; set; }
        #endregion

        #region Navigation
        [Required(ErrorMessage = "Company name is required.")]
        public string CompanyId { get; set; }
        [Required(ErrorMessage = "Branch name is required.")]
        public string BranchId { get; set; }
        [Display(Name = "Customer category")]
        public string CustomerCategoryId { get; set; }
        public string CustomerSubCategoryId { get; set; }
        #endregion

        public List<CustomerViewModel> ACustomers { get; set; }
        public DataSet SalesInvoice { get; set; }  
    }
}
