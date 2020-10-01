using Library.Model.Inventory.Suppliers;
using System.Collections.Generic;

namespace Library.Service.Inventory.Suppliers
{
    /// <summary>
    /// Interface ISupplierCategoryService
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public interface ISupplierCategoryService
    {
        void Add(SupplierCategory supplierCategory);
        
        void Update(SupplierCategory supplierCategory);
        
        void Archive(string id);
        
        SupplierCategory GetById(string id);
        
        IEnumerable<SupplierCategory> GetAll();

        IEnumerable<object> Lists();
        
        int GetAutoSequence();
    }
}
