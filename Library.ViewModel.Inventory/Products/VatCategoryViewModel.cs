using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Products
{
    public class VatCategoryViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        [Display(Name = "Vat Rate")]
        public decimal VatRate { get; set; }
        
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
