using Library.Context.Core;
using Library.Crosscutting.Helper;
using Library.Model.Core.Core;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Products;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using Color = System.Drawing.Color;
using Size = Library.Model.Inventory.Products.Size;

namespace Library.Service.Inventory.Products
{
    public class BarcodeService : Service<Product>, IBarcodeService
    {
        #region Ctor
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<Size> _sizeRepository;
        private readonly IRepository<Company> _companyRepository;
        private readonly IRepository<Branch> _branchRepository;
        public BarcodeService(
            IRepository<Product> productRepository,
            IRepository<Size> sizeRepository,
            IRepository<Company> companyRepository,
            IRepository<Branch> branchRepository
            ) : base(productRepository)
        {
            _productRepository = productRepository;
            _sizeRepository = sizeRepository;
            _companyRepository = companyRepository;
            _branchRepository = branchRepository;
        }
        #endregion
        int rowNumber = 1;
        int columnNumber = 0;
        private List<BarcodeGeneratorViewModel> Barcode(string productId, int number, string companyId, string branchId)
        {
            try
            {
                List<BarcodeGeneratorViewModel> bracods = new List<BarcodeGeneratorViewModel>();
                var product = _productRepository.GetOne(productId);
                if (!string.IsNullOrEmpty(product?.Id))
                {
                    var sizeName = _sizeRepository.GetOne(product?.SizeId)?.Name;
                    var companyName = _companyRepository.GetOne(companyId)?.Name;
                    var branchName = _branchRepository.GetOne(branchId)?.Name;
                    for (int i = 1; i <= number; i++)
                    {
                        if (columnNumber < 6)
                        {
                            columnNumber++;
                        }
                        else
                        {
                            rowNumber++;
                            columnNumber = 1;
                        }
                        bracods.Add(new BarcodeGeneratorViewModel { RowNumber = rowNumber, ColumnNumber = columnNumber, ProductId = productId, ProductCode = product.Code, ProductName = product.Name, SizeName = sizeName, RetailPrice = product.RetailPrice, CompanyId = companyId, CompanyName = companyName, BranchId = branchId, BranchName = branchName });
                    }
                }
                return bracods;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public DataTable GenerateBarcodeList(string[] itemIds, int[] numbers, string companyId, string branchId)
        {
            try
            {
                List<BarcodeGeneratorViewModel> bracods = new List<BarcodeGeneratorViewModel>();
                int rn = numbers.Sum();
                for (int i = 0; i < itemIds.Length; i++)
                {
                    bracods.AddRange(Barcode(itemIds[i], Convert.ToInt16(numbers[i]), companyId, branchId));
                }
                var dt = DataUtil.ConvertToDataTable(bracods);
                BarcodeLib.Barcode barcode = new BarcodeLib.Barcode
                {
                    Alignment = BarcodeLib.AlignmentPositions.CENTER
                };
                int W = 260;
                int H = 130;
                BarcodeLib.TYPE type = BarcodeLib.TYPE.CODE128;
                barcode.IncludeLabel = false;
                barcode.RotateFlipType = (RotateFlipType)Enum.Parse(typeof(RotateFlipType), "RotateNoneFlipNone", true);
                barcode.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
                dt.Columns.Add(new DataColumn("Barcode", typeof(byte[])));
                foreach (DataRow dr in dt.Rows)
                {
                    Image generatedBarcode = barcode.Encode(type, dr["ProductCode"].ToString(), Color.Black, Color.White, W, H);
                    MemoryStream stream = new MemoryStream();
                    generatedBarcode.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                    dr["Barcode"] = stream.ToArray();
                }
                return dt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
