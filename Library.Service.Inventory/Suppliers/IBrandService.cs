using Library.Model.Inventory.Suppliers;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace Library.Service.Inventory.Suppliers
{
    /// <summary>
    /// Interface IBrandService
    /// <remarks>Jahangir, 14-03-2016</remarks>
    /// </summary>
    public interface IBrandService
    {
        /// <summary>
        /// Adds the specified Brand.
        /// </summary>
        /// <remarks>Jahangir, 14-03-2016</remarks>
        /// <param name="Brand">The Brand.</param>
        void Add(Brand brand);

        /// <summary>
        /// Updates the specified Brand.
        /// </summary>
        /// <remarks>Jahangir, 14-03-2016</remarks>
        /// <param name="Brand">The Brand.</param>
        void Update(Brand brand);

        DataTable UploadFromDirectory(HttpPostedFileBase file);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void Archive(string id);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir, 14-03-2016</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Brand.</returns>
        Brand GetById(string id);

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Gets all items in this collection. </summary>
        ///
        /// <returns>
        /// An enumerator that allows foreach to be used to process all items in this collection.
        /// </returns>
        ///-------------------------------------------------------------------------------------------------
        IEnumerable<Brand> GetAll();

        IEnumerable<object> Lists();

        IEnumerable<object> Lists(string supplierId);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetAutoSequence();

    }
}
