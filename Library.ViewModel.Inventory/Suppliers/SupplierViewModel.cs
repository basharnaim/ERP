using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Suppliers
{
    
    public class SupplierViewModel
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
        [Required(ErrorMessage = "Address1 is required.")]
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        [Required(ErrorMessage = "Country is required.")]
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        [RegularExpression(@"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}", ErrorMessage = "Please enter valid email.")]
        public string Email { get; set; }
        public string Website { get; set; }
        public string Description { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string ContactPerson { get; set; }
        public string ContactPersonDesignation { get; set; }
        #endregion

        #region Navigation
        [Display(Name = "Supplier category")]
        public string SupplierCategoryId { get; set; }
        public string CountryId { get; set; }
        [Required(ErrorMessage = "Division is required.")]
        [Display(Name = "Division")]
        public string DivisionId { get; set; }
        [Required(ErrorMessage = "District is required.")]
        [Display(Name = "District")]
        public string DistrictId { get; set; }
        #endregion
    }
}
