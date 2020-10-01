using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Products
{
    /// <summary>
    /// This Class is Color.
    /// <remarks>Jahangir, 27-10-2015</remarks>
    /// </summary>
    public class FloorViewModel
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
    }
}
