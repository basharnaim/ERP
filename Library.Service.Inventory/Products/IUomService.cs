using System.Collections.Generic;
using Library.Model.Inventory.Products;

namespace Library.Service.Inventory.Products
{
    public interface IUomService
    {
        void Add(Uom uom);
        void Update(Uom uom);
        void Delete(string id);
        Uom GetById(string id);
        IEnumerable<Uom> GetAll(string name, string code);
        IEnumerable<Uom> GetAll();
        IEnumerable<Uom> GetAll(string name);
        IEnumerable<object> Lists();
    }
}
