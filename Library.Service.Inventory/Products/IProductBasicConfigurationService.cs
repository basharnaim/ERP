using Library.Model.Inventory.Products;
using System.Collections.Generic;

namespace Library.Service.Inventory.Products
{
    public interface IProductBasicConfigurationService
    {
        void Add(ProductBasicConfiguration configurationSetting);

        void Update(ProductBasicConfiguration configurationSetting);

        ProductBasicConfiguration GetById(string id);

        IEnumerable<ProductBasicConfiguration> GetAll();

        IEnumerable<ProductBasicConfiguration> GetAll(string productBasicConfigurationCategoryId);

        IEnumerable<object> Lists();
    }
}
