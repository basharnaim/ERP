#region Using

using System.ComponentModel.DataAnnotations;

#endregion

namespace Library.ViewModel.Inventory.Products
{
    public class ProductBasicConfigurationViewModel
    {
        #region Scaler
        public string Id { get; set; }
        public int Sequence { get; set; }
        public string Code { get; set; }
        [Required(ErrorMessage= "Name is required.")]
        public string Name { get; set; }
        public bool Active { get; set; }
        public bool Archive { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        public string Description { get; set; }
        #endregion

        #region Navigation
        [Display(Name = "Category name")]
        [Required(ErrorMessage= "Category is required.")]
        public string ProductBasicConfigurationCategoryId { get; set; }
        #endregion
    }
}
