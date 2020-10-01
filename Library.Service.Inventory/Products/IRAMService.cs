using Library.Model.Inventory.Products;
using System.Collections.Generic;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IColorService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IRAMService
    {
        void Add(RAM ram);
        
        void Update(RAM ram);
        
        RAM GetById(string id);
        
        IEnumerable<RAM> GetAll();
        
        IEnumerable<object> Lists();
        
        int GetAutoSequence();
    }
}
