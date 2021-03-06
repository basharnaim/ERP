﻿using System.ComponentModel.DataAnnotations;

namespace Library.ViewModel.Inventory.Products
{
    /// <summary>
    /// This Class is ProductCategory.
    /// <remarks>Jahangir Hossain Sheikh, 25-10-2015</remarks>
    /// </summary>
    public class ProductCategoryViewModel
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
        public string Description { get; set; }
        
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }
        #endregion
    }
}
