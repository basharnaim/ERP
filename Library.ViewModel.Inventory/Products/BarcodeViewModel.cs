using System.Collections.Generic;

namespace Library.ViewModel.Inventory.Products
{
    public class BarcodeViewModel
    {
        #region Scaler
        public string ProductId { get; set; }
        public string ProductCode { get; set; }
        public string ProductName { get; set; }
        public decimal RetailPrice { get; set; }
        public int Number { get; set; }
        public byte[] Barcode { get; set; }
        #endregion

        #region Navigation
        public string CompanyId { get; set; }
        public string BranchId { get; set; }
        #endregion

        #region List
        public List<BarcodeViewModel> BarcodeList { get; set; }
        #endregion
    }
}
