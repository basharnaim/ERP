using Library.Model.Inventory.Products;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace Library.Service.Inventory.Products
{
    public interface IManufacturerService
    {
        DataTable UploadFromDirectory(HttpPostedFileBase file);
        void Add(Manufacturer manufacturer);

        void Update(Manufacturer manufacturer);

        Manufacturer GetById(string id);

        IEnumerable<Manufacturer> GetAll();

        IEnumerable<object> Lists();
    }
}
