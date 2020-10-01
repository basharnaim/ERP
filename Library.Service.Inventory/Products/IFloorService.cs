using Library.Model.Inventory.Products;
using System.Collections.Generic;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IRackService
    /// <remarks>Jahangir, 21-03-2016</remarks>
    /// </summary>
    public interface IFloorService
    {
        /// <summary>
        /// Adds the specified Rack.
        /// </summary>
        /// <remarks>Jahangir, 21-03-2016</remarks>
        /// <param name="Rack">The Rack.</param>
        void Add(Floor floor);

        /// <summary>
        /// Updates the specified Rack.
        /// </summary>
        /// <remarks>Jahangir, 21-03-2016</remarks>
        /// <param name="Rack">The Rack.</param>
        void Update(Floor floor);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir, 21-03-2016</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Rack.</returns>
        Floor GetById(string id);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets all items in this collection. </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all items in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        IEnumerable<Floor> GetAll();

        IEnumerable<object> Lists();

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetAutoSequence();
    }
}
