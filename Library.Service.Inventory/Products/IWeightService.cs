using Library.Model.Inventory.Products;
using System.Collections.Generic;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IWeightService
    /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IWeightService
    {
        /// <summary>
        /// Adds the specified Weight.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        /// <param name="weight">The Weight.</param>
        void Add(Weight weight);

        /// <summary>
        /// Updates the specified Weight.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        void Update(Weight weight);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Weight.</returns>
        Weight GetById(string id);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets all items in this collection. </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all items in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        IEnumerable<Weight> GetAll();

        decimal GetTotalWeightInTons(string weightId, int lineTotalPcses);

        IEnumerable<object> Lists();
    }
}
