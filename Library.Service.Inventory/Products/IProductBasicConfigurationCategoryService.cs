using Library.Model.Inventory.Products;
using System.Collections.Generic;

namespace Library.Service.Inventory.Products
{
    public interface IProductBasicConfigurationCategoryService
    {
        void Add(ProductBasicConfigurationCategory configurationCategory);

        void Update(ProductBasicConfigurationCategory configurationCategory);

        ProductBasicConfigurationCategory GetById(string id);

        IEnumerable<ProductBasicConfigurationCategory> GetAll();

        IEnumerable<object> Lists();
    }
}
