using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Library.ViewModel.Inventory.Products
{
    public class ProductViewModel
    {
        #region Scalar
        public string Id { get; set; }
        public int Sequence { get; set; }
       
        [Required(ErrorMessage = "Code is required.")]
        [MaxLength(50, ErrorMessage = "Code cannot be longer than 50 characters.")]
        [Display(Name = "Code")]
        public string Code { get; set; }
        [Display(Name = "Supplier Product Code")]
        public string SupplierProductCode { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(150, ErrorMessage = "Name cannot be longer than 150 characters.")]
        public string Name { get; set; }

        public string Description { get; set; }
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
        [Display(Name = "Purchase price(TP)")]
        [Required(ErrorMessage = "Purchase price is required.")]
        public decimal PurchasePrice { get; set; }
        [Display(Name = "Wholesale price(MRP)")]
        [Required(ErrorMessage = "Wholesale price is required.")]
        public decimal WholeSalePrice { get; set; }
        [Display(Name = "Retail price(MRP)")]
        [Required(ErrorMessage = "Purchase price is required.")]
        public decimal RetailPrice { get; set; }

        [Display(Name = "Shelf-life")]
        public int ShelfLife { get; set; }
        [Display(Name = "Reorder Level")]
        public decimal ReorderLevel { get; set; }
        public bool Archive { get; set; }
        public bool Active { get; set; }
        public bool IsSynchronized { get; set; }
        public bool IsUpdated { get; set; }
        public string SynchronizationType { get; set; }

        public decimal ProductStock { get; set; }

        [Display(Name = "Stock Value")]
        public decimal ProductStockValue { get; set; }

        public decimal OpeningValue { get; set; }
        public decimal ClosingValue { get; set; }

        public byte[] Barcode { get; set; }
        public byte[] Picture { get; set; }
        [Display(Name = "Picture")]
        public HttpPostedFileBase Picturep { get; set; }

        #endregion

        #region Navigation
        [Required(ErrorMessage = "Category is required.")]
        public string ProductCategoryId { get; set; }

        public string ProductCategoryName { get; set; }
        public string ProductName { get; set; }
        public string ProductSubCategoryId { get; set; }

        public string ProductSubCategoryName { get; set; }

        public string ProductSubsidiaryCategoryId { get; set; }
        public string ProductSubsidiaryCategoryName { get; set; }
        [Required(ErrorMessage = "Supplier is required.")]
        public string SupplierId { get; set; }

        public string SupplierName { get; set; }
        public string BrandId { get; set; }
        public string BrandName { get; set; }

        [Required(ErrorMessage = "UOM is required.")]
        public string UOMId { get; set; }
        public string UOMName { get; set; }
        public string RAMId { get; set; }
        public string ROMId { get; set; }
        public string SizeId { get; set; }
        public string SizeName { get; set; }

        [Display(Name = "Shade")]
        public string ColorId { get; set; }
        public string StyleId { get; set; }
        public string StyleName { get; set; }

        [Display(Name = "Country Of Origin")]
        public string CountryId { get; set; }

        [Required(ErrorMessage = "Grade is required.")]
        public string GradeId { get; set; }

        [Required(ErrorMessage = "Upper Conversion is required.")]
        public string UpperConversion { get; set; }


        [Required(ErrorMessage = "Lower Conversion is required.")]
        public string LowerConversion { get; set; }

        [Required(ErrorMessage = "Conversion unit is required.")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal? ConversionUnit { get; set; }

        [Display(Name = "Maximum discount(Tk)")]
        [Required(ErrorMessage = "Maximum discount is required.")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal MaxDiscount { get; set; }

        [Display(Name = "Vat")]
        [Required(ErrorMessage = "Vat is required.")]
        [DisplayFormat(DataFormatString = "{0:n2}", ApplyFormatInEditMode = true)]
        public decimal Vat { get; set; }

        public string ManufacturerId { get; set; }
        public string VatCategoryId { get; set; }

        public string RackId { get; set; }
        public string RackName { get; set; }
        public string EditionNo { get; set; }

        public string FloorId { get; set; }
        public string FloorName { get; set; }

        [Display(Name = "Marginal Profit")]
        public decimal ProfitAmountInPercentage { get; set; }
        public decimal ProfitAmount { get; set; }
        public decimal Quantity { get; set; }
        #endregion
    }
}