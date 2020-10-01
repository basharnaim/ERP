using System.Collections.Generic;
using Library.Model.Inventory.Products;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface ICurrencyService
    /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface ICurrencyService
    {
        /// <summary>
        /// Adds the specified Currency.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        void Add(Currency currency);

        /// <summary>
        /// Updates the specified Currency.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        void Update(Currency currency);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Currency.</returns>
        Currency GetById(string id);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets all items in this collection. </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all items in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        IEnumerable<Currency> GetAll();

        IEnumerable<object> Lists();
    }
}
