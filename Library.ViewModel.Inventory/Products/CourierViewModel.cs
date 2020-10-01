using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Products
{
    public class CourierViewModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        [Display(Name = "Code")]
        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(20, ErrorMessage = "Code cannot be longer than 20 numbers.")]
        public string Code { get; set; }
        [Display(Name = "Name")]
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(100, ErrorMessage = "Name cannot be longer than 100 numbers.")]
        public string Name { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion
    }
}
