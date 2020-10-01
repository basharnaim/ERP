using Library.Model.Inventory.Products;
using System.Collections.Generic;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IGradeService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IProductMasterService
    {
        IEnumerable<Product> GetAllProductsFromProductMaster();
        IEnumerable<Product> GetAllProductsFromProductMasterByCompanyBranchId(string companyId, string branchId);
        void Add(string[] productIdList, string templateCompany, string templateBranch);


        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>ProductMaster.</returns>
        ProductMaster GetById(string id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;ProductMaster&gt;.</returns>
        IEnumerable<ProductMaster> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<ProductMaster> GetAll(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        IEnumerable<ProductMaster> GetAll(string companyId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> Lists();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<object> Lists(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetGradeNameById(string id);

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<Product> GetProductMasterTemplate(string companyId, string branchId);

        /// <summary>
        /// GetProductMasterTemplate
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="productCategoryId"></param>
        /// <returns>IEnumerable<Product></returns>
        IEnumerable<Product> GetProductMasterTemplate(string companyId, string branchId, string productCategoryId);


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void ChangeStatus(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<string> GetProductMasterIds(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<Product> GetAllProductByMasterIds(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="productCategoryId"></param>
        /// <returns></returns>
        IEnumerable<Product> GetAllProductByMasterIds(string companyId, string branchId, string productCategoryId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="productCategoryId"></param>
        /// <param name="productSubCategoryId"></param>
        /// <returns></returns>
        IEnumerable<Product> GetAllProductByMasterIds(string companyId, string branchId, string productCategoryId, string productSubCategoryId);

        IEnumerable<Product> GetProductDialogProductList(string companyId, string branchId);
        IEnumerable<object> GetProductCategoryByCompanyBranch(string branchId);

        IEnumerable<object> GetProductByCompanyBranch(string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetAutoSequence();
    }
}
