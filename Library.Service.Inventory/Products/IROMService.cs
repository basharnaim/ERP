using Library.Model.Inventory.Products;
using System.Collections.Generic;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IColorService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IROMService
    {
        void Add(ROM rom);
        
        void Update(ROM rom);
        
        ROM GetById(string id);
        
        IEnumerable<ROM> GetAll();
        
        IEnumerable<object> Lists();
        
        int GetAutoSequence();
    }
}
