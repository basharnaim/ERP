using System.Collections.Generic;
using Library.Model.Inventory.Products;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface ICourierService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface ICourierService
    {
        void Add(Courier Courier);

        void Update(Courier Courier);

        Courier GetById(string id);

        IEnumerable<Courier> GetAll();

        IEnumerable<object> Lists();

        int GetAutoSequence();
    }
}
