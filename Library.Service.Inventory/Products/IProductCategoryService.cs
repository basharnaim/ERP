using System.Collections.Generic;
using System.Data;
using System.Web;
using Library.Model.Inventory.Products;

namespace Library.Service.Inventory.Products
{
    public interface IProductCategoryService
    {
        DataTable UploadFromDirectory(HttpPostedFileBase file);
        void Add(ProductCategory productCategory);

        void Update(ProductCategory productCategory);

        ProductCategory GetById(string id);

        IEnumerable<ProductCategory> GetAll();

        IEnumerable<object> Lists();

        int GetAutoSequence();
    }
}
