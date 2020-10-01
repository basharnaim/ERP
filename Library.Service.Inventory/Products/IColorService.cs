using System.Collections.Generic;
using Library.Model.Inventory.Products;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IColorService
    /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IColorService
    {
        /// <summary>
        /// Adds the specified Color.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        void Add(Color color);

        /// <summary>
        /// Updates the specified Color.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        void Update(Color color);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Color.</returns>
        Color GetById(string id);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets all items in this collection. </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all items in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        IEnumerable<Color> GetAll();

        IEnumerable<object> Lists();
    }
}
