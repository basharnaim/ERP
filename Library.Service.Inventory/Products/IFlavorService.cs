using Library.Model.Inventory.Products;
using System.Collections.Generic;

namespace Library.Service.Inventory.Products
{
    public interface IFlavorService
    {
        void Add(Flavor flavor);

        void Update(Flavor flavor);

        Flavor GetById(string id);

        IEnumerable<Flavor> GetAll();

        IEnumerable<object> Lists();
    }
}
