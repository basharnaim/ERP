using Library.Model.Inventory.Products;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IProductSubCategoryService
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public interface IProductSubCategoryService
    {
        /// <summary>
        /// Adds the specified item sub category.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <param name="productSubCategory">The item sub category.</param>
        void Add(ProductSubCategory productSubCategory);


        DataTable UploadFromDirectory(HttpPostedFileBase file);

        /// <summary>
        /// Updates the specified item sub category.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <param name="productSubCategory">The item sub category.</param>
        void Update(ProductSubCategory productSubCategory);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>ProductSubCategory.</returns>
        ProductSubCategory GetById(string id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;ProductSubCategory&gt;.</returns>
        IEnumerable<ProductSubCategory> GetAll();
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="productCategoryId"></param>
        /// <returns></returns>
        IEnumerable<ProductSubCategory> GetAll(string productCategoryId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productCategoryId"></param>
        /// <returns></returns>
        IEnumerable<object> Lists(string productCategoryId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetAutoSequence();
    }
}
