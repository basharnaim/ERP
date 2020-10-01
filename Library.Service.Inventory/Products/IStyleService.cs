using Library.Model.Inventory.Products;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Interface IColorService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface IStyleService
    {
        DataTable UploadFromDirectory(HttpPostedFileBase file);
        void Add(Style style);
        
        void Update(Style style);
        
        Style GetById(string id);
        
        IEnumerable<Style> GetAll();
        
        IEnumerable<object> Lists();
        
        int GetAutoSequence();
    }
}
