using System.Data;

namespace Library.Service.Inventory.Products
{
    public interface IBarcodeService
    {
        DataTable GenerateBarcodeList(string[] itemIds, int[] numbers, string companyId, string branchId);
    }
}
