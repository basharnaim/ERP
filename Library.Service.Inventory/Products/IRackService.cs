using Library.Model.Inventory.Products;
using System.Collections.Generic;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IRackService
    /// <remarks>Jahangir, 21-03-2016</remarks>
    /// </summary>
    public interface IRackService
    {
        void Add(Rack rack);

        void Update(Rack rack);
        
        Rack GetById(string id);
        
        IEnumerable<Rack> GetAll();

        IEnumerable<object> Lists();

        int GetAutoSequence();
    }
}
