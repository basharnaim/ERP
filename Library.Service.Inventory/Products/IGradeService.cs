using System.Collections.Generic;
using Library.Model.Inventory.Products;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IGradeService
    /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IGradeService
    {
        /// <summary>
        /// Adds the specified grade.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        /// <param name="grade">The grade.</param>
        void Add(Grade grade);

        /// <summary>
        /// Updates the specified grade.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        /// <param name="grade">The grade.</param>
        void Update(Grade grade);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Grade.</returns>
        Grade GetById(string id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;Grade&gt;.</returns>
        IEnumerable<Grade> GetAll();

        /// -------------------------------------------------------------------------------------------------
        ///  <summary>   Enumerates lists in this collection. </summary>
        /// <returns>
        ///  An enumerator that allows foreach to be used to process lists in this collection.
        ///  </returns>
        /// -------------------------------------------------------------------------------------------------
        IEnumerable<object> Lists();

        string GetGradeNameById(string id);
    }
}
