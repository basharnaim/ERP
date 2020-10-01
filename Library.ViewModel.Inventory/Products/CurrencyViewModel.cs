using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Products
{
    public class CurrencyViewModel
    {       
        #region Scalar
        public string Id { get; set; }
        public int Sequence { get; set; }
        [MaxLength(10, ErrorMessage = "Code cannot be longer than 10 characters.")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 characters.")]
        [Display(Name="Color")]
        public string Name { get; set; }
        public string Description { get; set; }
        [Display(Name="Active")]
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        #endregion
    }
}
