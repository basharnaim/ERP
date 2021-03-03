using Library.Model.Inventory.Products;
using Library.Model.Inventory.Purchases;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace Library.Service.Inventory.Products
{    
    public interface IProductService
    {
        string GenerateAutoId();
        /// <summary>
        /// Adds the specified Product.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <param name="Product">The Product.</param>
        void Add(Product Product);
        /// <summary>
        /// Updates the specified Product.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <param name="Product">The Product.</param>
        void Update(Product Product);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Products"></param>
        void UpdateMultipelProduct(ICollection<Product> Products);
        DataTable UploadFromDirectoryForAmericanTMart(HttpPostedFileBase file);
        DataTable UploadFromDirectory(HttpPostedFileBase file);
        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Product.</returns>
        Product GetById(string id);
        Product GetByCode(string code);
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;Product&gt;.</returns>
        IEnumerable<Product> GetAll();
        /// <summary>
        /// GetAll
        /// </summary>
        /// <param name="ProductCategoryId"></param>
        /// <returns>IEnumerable<Product></returns>
        IEnumerable<Product> GetAll(string ProductCategoryId);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ProductCategoryId"></param>
        /// <param name="ProductSubCategoryId"></param>
        /// <returns></returns>
        IEnumerable<Product> GetAll(string ProductCategoryId, string ProductSubCategoryId);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> Lists();        
        IEnumerable<object> GetAllPurchaseItemList();
        IEnumerable<object> GetAllPurchaseItemList(string companyId, string branchId);
        int GetAutoSequence();

        //IEnumerable<OpeningBlance> GetAllProductList(); 
    }
}
