using System.Collections.Generic;
using System.Data;
using System.Web;
using Library.Model.Inventory.Products;

namespace Library.Service.Inventory.Products
{
    public interface ISizeService
    {
        void Add(Size size);
        DataTable UploadFromDirectory(HttpPostedFileBase file);
        void Update(Size size);
        Size GetById(string id);
        IEnumerable<Size> GetAll();
        IEnumerable<object> Lists();
        Size GetSizeById(string sizeId);
    }
}
