using System.Collections.Generic;
using Library.Model.Inventory.Products;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IColorService
    /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IVatCategoryService
    {
        /// <summary>
        /// Adds the specified Color.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        void Add(VatCategory vatCategory);

        /// <summary>
        /// Updates the specified Color.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        void Update(VatCategory vatCategory);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Color.</returns>
        VatCategory GetById(string id);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets all items in this collection. </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all items in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        IEnumerable<VatCategory> GetAll();

        /// -------------------------------------------------------------------------------------------------
        ///  <summary>   Enumerates lists in this collection. </summary>
        /// <returns>
        ///  An enumerator that allows foreach to be used to process lists in this collection.
        ///  </returns>
        /// -------------------------------------------------------------------------------------------------
        IEnumerable<object> Lists();

        decimal GetVatCategoryRate(string vatCategoryId);
    }
}
