using Library.Model.Inventory.Products;
using System.Collections.Generic;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IItemSubsidiaryCategoryService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IProductSubsidiaryCategoryService
    {
        /// <summary>
        /// Adds the specified item subsidiary category.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="itemSubsidiaryCategory">The item subsidiary category.</param>
        void Add(ProductSubsidiaryCategory itemSubsidiaryCategory);

        /// <summary>
        /// Updates the specified item subsidiary category.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="itemSubsidiaryCategory">The item subsidiary category.</param>
        void Update(ProductSubsidiaryCategory itemSubsidiaryCategory);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>ItemSubsidiaryCategory.</returns>
        ProductSubsidiaryCategory GetById(string id);

        IEnumerable<ProductSubsidiaryCategory> GetAll();


        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;ItemSubsidiaryCategory&gt;.</returns>

        IEnumerable<ProductSubsidiaryCategory> GetAll(string ProductCategoryId);

        IEnumerable<ProductSubsidiaryCategory> GetAll(string ProductCategoryId, string ProductSubCategoryId);

        
        /// <summary>   Enumerates lists in this collection. </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process lists in this collection.
        /// </returns>
        
        IEnumerable<object> Lists(string ProductSubCategoryId);

        int GetAutoSequence();
    }
}
