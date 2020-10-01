using System.Collections.Generic;
using Library.Model.Inventory.Customers;

namespace Library.Service.Inventory.Customers
{
    /// <summary>
    /// Interface ICustomerCategoryService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface ICustomerCategoryService
    {
        void Add(CustomerCategory customerCategory);
        
        void Update(CustomerCategory customerCategory);
        
        void Archive(string id);
        
        CustomerCategory GetById(string id);
        
        IEnumerable<CustomerCategory> GetAll();
        
        IEnumerable<object> Lists();
        
        decimal GetCustomerCategoryDiscount(string customerCategoryId);
        
        int GetAutoSequence();
    }
}
