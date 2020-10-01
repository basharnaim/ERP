using Library.Model.Inventory.Suppliers;
using System.Collections.Generic;
using System.Data;
using System.Web;

namespace Library.Service.Inventory.Suppliers
{
    /// <summary>
    /// Interface ISupplierService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface ISupplierService
    {
        DataTable UploadFromDirectory(HttpPostedFileBase file);
        void Add(Supplier supplier);
        void Update(Supplier supplier);
        void Archive(string id);
        Supplier GetById(string id);
        IEnumerable<Supplier> GetAll();
        Supplier GetSupplierBySupplierMobileNumber(string supplierMobile);
        IEnumerable<Supplier> GetAllSupplierBySupplierMobileNumber(string supplierMobile);
        Supplier GetSupplierBySupplierId(string supplierId);
        IEnumerable<Supplier> GetAllSupplierBySupplierId(string supplierId);
        IEnumerable<Supplier> GetAll(string supplierCategoryId);
        IEnumerable<object> Lists();
        string GetSupplierNameById(string id);
        int GetAutoSequence();
    }
}
