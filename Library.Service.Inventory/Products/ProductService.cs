#region Using

using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Products;
using Library.Model.Inventory.Purchases;
using Library.Model.Inventory.Suppliers;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Configuration;

#endregion

namespace Library.Service.Inventory.Products
{    
    public class ProductService : Service<Product>, IProductService
    {
        #region Ctor
        private readonly IRepository<Product> _productRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IRepository<ProductSubCategory> _productSubCategoryRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<Style> _styleRepository;
        private readonly IRepository<Size> _sizeRepository;
        private readonly IRepository<PurchaseDetail> _purchaseDetailRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(
            IRepository<Product> productRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IRepository<ProductSubCategory> productSubCategoryRepository,
            IRepository<Supplier> supplierRepository,
            IRepository<Brand> brandRepository,
            IRepository<Style> styleRepository,
            IRepository<Size> sizeRepository,
            IRepository<PurchaseDetail> purchaseDetailRepository,
            IUnitOfWork unitOfWork
            ) : base(productRepository)
        {
            _productRepository = productRepository;
            _productCategoryRepository = productCategoryRepository;
            _productSubCategoryRepository = productSubCategoryRepository;
            _supplierRepository = supplierRepository;
            _brandRepository = brandRepository;
            _styleRepository = styleRepository;
            _sizeRepository = sizeRepository;
            _purchaseDetailRepository = purchaseDetailRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion
        private void Check(Product model)
        {
            try
            {
                CheckUniqueColumn("Code", model.Code, r => r.Id != model.Id && r.Code == model.Code);
                CheckUniqueColumn("Name", model.Name, r => r.Id != model.Id && r.Name == model.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private string GenerateProductAutoCode(string supplierProductCode, string supplierName, string brandName, int number)
        {
            try
            {
                return supplierProductCode + supplierName.Substring(0, 3) + brandName.Substring(0, 3) + number;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        private void BulkUpload(List<Product> productList)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ErpdbEntities"].ConnectionString;
                
                #region Product
                var product = new DataTable();
                product.Columns.Add("Id", typeof(string));
                product.Columns.Add("Sequence", typeof(int));
                product.Columns.Add("Code", typeof(string));
                product.Columns.Add("SupplierProductCode", typeof(string));
                product.Columns.Add("Name", typeof(string));
                product.Columns.Add("Picture", typeof(byte[]));
                product.Columns.Add("ProductCategoryId", typeof(string));
                product.Columns.Add("ProductSubCategoryId", typeof(string));
                product.Columns.Add("ProductSubsidiaryCategoryId", typeof(string));
                product.Columns.Add("UOMId", typeof(string));
                product.Columns.Add("ColorId", typeof(string));
                product.Columns.Add("StyleId", typeof(string));
                product.Columns.Add("FlavorId", typeof(string));
                product.Columns.Add("GradeId", typeof(string));
                product.Columns.Add("FloorId", typeof(string));
                product.Columns.Add("RackId", typeof(string));
                product.Columns.Add("ManufacturerId", typeof(string));
                product.Columns.Add("VATCategoryId", typeof(string));
                product.Columns.Add("SizeId", typeof(string));
                product.Columns.Add("RAMId", typeof(string));
                product.Columns.Add("ROMId", typeof(string));
                product.Columns.Add("SupplierId", typeof(string));
                product.Columns.Add("BrandId", typeof(string));
                product.Columns.Add("CountryId", typeof(string));
                product.Columns.Add("PurchasePrice", typeof(decimal));
                product.Columns.Add("RetailPrice", typeof(decimal));
                product.Columns.Add("WholeSalePrice", typeof(decimal));
                product.Columns.Add("ProfitAmount", typeof(decimal));
                product.Columns.Add("ProfitAmountInPercentage", typeof(decimal));
                product.Columns.Add("ShelfLife", typeof(int));
                product.Columns.Add("ReorderLevel", typeof(int));
                product.Columns.Add("MaxDiscount", typeof(decimal));
                product.Columns.Add("Active", typeof(bool));
                product.Columns.Add("Archive", typeof(bool));
                product.Columns.Add("IsSynchronized", typeof(bool));
                product.Columns.Add("IsUpdated", typeof(bool));
                product.Columns.Add("SynchronizationType", typeof(string));
                product.Columns.Add("Description", typeof(string));
                product.Columns.Add("AddedBy", typeof(string));
                product.Columns.Add("AddedDate", typeof(DateTime));
                product.Columns.Add("AddedFromIp", typeof(string));
                product.Columns.Add("UpdatedBy", typeof(string));
                product.Columns.Add("UpdatedDate", typeof(DateTime));
                product.Columns.Add("UpdatedFromIp", typeof(string));
                foreach (var entity in productList)
                {
                    var row = product.NewRow();
                    row["Id"] = entity.Id;
                    row["Sequence"] = entity.Sequence;
                    row["Code"] = entity.Code;
                    row["SupplierProductCode"] = entity.SupplierProductCode ?? (object)DBNull.Value;
                    row["Name"] = entity.Name;
                    row["Picture"] = entity.Picture ?? (object)DBNull.Value;
                    row["ProductCategoryId"] = entity.ProductCategoryId ?? (object)DBNull.Value;
                    row["ProductSubCategoryId"] = entity.ProductSubCategoryId ?? (object)DBNull.Value;
                    row["ProductSubsidiaryCategoryId"] = entity.ProductSubsidiaryCategoryId ?? (object)DBNull.Value;
                    row["UOMId"] = entity.UOMId ?? (object)DBNull.Value;
                    row["ColorId"] = entity.ColorId ?? (object)DBNull.Value;
                    row["StyleId"] = entity.StyleId ?? (object)DBNull.Value;
                    row["FlavorId"] = entity.FlavorId ?? (object)DBNull.Value;
                    row["GradeId"] = entity.GradeId ?? (object)DBNull.Value;
                    row["FloorId"] = entity.FloorId ?? (object)DBNull.Value;
                    row["RackId"] = entity.RackId ?? (object)DBNull.Value;
                    row["ManufacturerId"] = entity.ManufacturerId ?? (object)DBNull.Value;
                    row["VatCategoryId"] = entity.VatCategoryId ?? (object)DBNull.Value;
                    row["SizeId"] = entity.SizeId ?? (object)DBNull.Value;
                    row["RAMId"] = entity.RAMId ?? (object)DBNull.Value;
                    row["ROMId"] = entity.ROMId ?? (object)DBNull.Value;
                    row["SupplierId"] = entity.SupplierId ?? (object)DBNull.Value;
                    row["BrandId"] = entity.BrandId ?? (object)DBNull.Value;
                    row["CountryId"] = entity.CountryId ?? (object)DBNull.Value;
                    row["PurchasePrice"] = entity.PurchasePrice;
                    row["RetailPrice"] = entity.RetailPrice;
                    row["WholeSalePrice"] = entity.WholeSalePrice;
                    row["ProfitAmount"] = entity.ProfitAmount;
                    row["ProfitAmountInPercentage"] = entity.ProfitAmountInPercentage;
                    row["ShelfLife"] = entity.ShelfLife;
                    row["ReorderLevel"] = entity.ReorderLevel;
                    row["MaxDiscount"] = entity.MaxDiscount;
                    row["Active"] = entity.Active;
                    row["Archive"] = entity.Archive;
                    row["IsSynchronized"] = entity.IsSynchronized;
                    row["IsUpdated"] = entity.IsUpdated;
                    row["SynchronizationType"] = entity.SynchronizationType ?? (object)DBNull.Value;
                    row["Description"] = entity.Description ?? (object)DBNull.Value;
                    row["AddedBy"] = entity.AddedBy;
                    row["AddedDate"] = entity.AddedDate;
                    row["AddedFromIp"] = entity.AddedFromIp;
                    row["UpdatedBy"] = entity.UpdatedBy ?? (object)DBNull.Value;
                    row["UpdatedDate"] = entity.UpdatedDate ?? (object)DBNull.Value;
                    row["UpdatedFromIp"] = entity.UpdatedFromIp ?? (object)DBNull.Value;
                    product.Rows.Add(row);
                }
                #endregion

                #region Save to table
                using (var sourceConnection = new SqlConnection(connectionString))
                {
                    sourceConnection.Open();
                    using (var tr = sourceConnection.BeginTransaction())
                    {
                        using (var bulkCopy1 = new SqlBulkCopy(sourceConnection, SqlBulkCopyOptions.Default, tr))
                        {
                            bulkCopy1.DestinationTableName = "dbo.Product";
                            // Write from the source to the customer.
                            bulkCopy1.WriteToServer(product);
                        }
                        tr.Commit();
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public string GenerateAutoId ()
        {
            try
            {
                return GetAutoNumber();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Add(Product product)
        {
            try
            {
                Check(product);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                product.Id = GetAutoNumber();
                product.Sequence = GetAutoSequence();
                product.SynchronizationType = SynchronizationType.Server.ToString();
                product.AddedBy = identity.Name;
                product.AddedDate = DateTime.Now;
                product.AddedFromIp = identity.IpAddress;
                _productRepository.Add(product);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Update(Product product)
        {
            try
            {
                Check(product);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var productDb = _productRepository.GetOne(product.Id);
                product.Sequence = productDb.Sequence;
                product.SynchronizationType = productDb.SynchronizationType;
                product.AddedBy = productDb.AddedBy;
                product.AddedDate = productDb.AddedDate;
                product.AddedFromIp = productDb.AddedFromIp;
                product.UpdatedBy = identity.Name;
                product.UpdatedDate = DateTime.Now;
                product.UpdatedFromIp = identity.IpAddress;
                _productRepository.Update(product);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void UpdateMultipelProduct(ICollection<Product> products)
        {
            try
            {
                foreach (var product in products)
                {
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                    var productDb = _productRepository.GetOne(product.Id);
                    product.Sequence = productDb.Sequence;
                    product.Name = productDb.Name;
                    product.ProductCategoryId = productDb.ProductCategoryId;
                    product.ProductSubCategoryId = productDb.ProductSubCategoryId;
                    product.Archive = productDb.Archive;
                    product.IsUpdated = productDb.IsUpdated;
                    product.IsSynchronized = productDb.IsSynchronized;
                    product.AddedBy = productDb.AddedBy;
                    product.AddedDate = productDb.AddedDate;
                    product.AddedFromIp = productDb.AddedFromIp;
                    product.UpdatedBy = identity.Name;
                    product.UpdatedDate = DateTime.Now;
                    product.UpdatedFromIp = identity.IpAddress;
                    _productRepository.Update(product);
                }
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataTable UploadFromDirectory(HttpPostedFileBase file)
        {
            try
            {
                var dt = new DataTable();
                dt.Columns.AddRange(new[] {
                        new DataColumn("GROUP NAME", typeof(string)),
                        new DataColumn("PRODUCTS NAME)", typeof(string)),
                        new DataColumn("BRAND", typeof(string)),
                        new DataColumn("STYLE/SIZE", typeof(string)),
                        new DataColumn("TP", typeof(decimal)),
                        new DataColumn("MRP", typeof(decimal))
                    });
                if (file != null)
                {
                    var directoryLocation = WebConfigurationManager.AppSettings["ErrorFilePath"];
                    if (!Directory.Exists(directoryLocation))
                    {
                        Directory.CreateDirectory(directoryLocation);
                    }
                    var filePath = directoryLocation + Path.GetFileName(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    file.SaveAs(filePath);
                    var conStr = "";
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03
                            conStr = WebConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //Excel 07-16
                            conStr = WebConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                        default:
                            break;
                    }
                    conStr = string.Format(conStr, filePath, "Yes", "2");
                    using (var excelCon = new OleDbConnection(conStr))
                    {
                        excelCon.Open();
                        var dtExcelSchema = excelCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dtExcelSchema != null)
                        {
                            var sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            var query = "select * from [" + sheetName + "]";
                            var da = new OleDbDataAdapter(query, excelCon);
                            da.Fill(dt);
                        }
                        excelCon.Close();
                    }
                    var result = new DataTable();
                    result.Columns.AddRange(new[] {
                        new DataColumn("GROUP NAME", typeof(string)),
                        new DataColumn("PRODUCTS NAME)", typeof(string)),
                        new DataColumn("BRAND", typeof(string)),
                        new DataColumn("STYLE/SIZE", typeof(string)),
                        new DataColumn("TP", typeof(decimal)),
                        new DataColumn("MRP", typeof(decimal))
                    });
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;

                    var manufacturerNames = (from t in dt.AsEnumerable() select t.Field<string>("BRAND")).AsEnumerable().Distinct();
                    var manufacturerList = _supplierRepository.GetAll(x => manufacturerNames.Contains(x.Name)).ToList();

                    var productCategoryNames = (from t in dt.AsEnumerable() select t.Field<string>("GROUP NAME")).AsEnumerable().Distinct();
                    var productCategoryList = _productCategoryRepository.GetAll(x => productCategoryNames.Contains(x.Name)).ToList();

                    var productSubCategoryNames = (from t in dt.AsEnumerable() select t.Field<string>("PRODUCTS NAME")).AsEnumerable().Distinct();
                    var productSubCategoryList = _productSubCategoryRepository.GetAll(x => productSubCategoryNames.Contains(x.Name)).ToList();

                    var styleNames = (from t in dt.AsEnumerable() select t.Field<string>("STYLE/SIZE")).AsEnumerable().Distinct();
                    var styleList = _styleRepository.GetAll(x => styleNames.Contains(x.Name)).ToList();

                    var productListDb = _productRepository.GetAll(x => !x.Archive).ToList();
                    var productList = new List<Product>();
                    var productAutoId = Convert.ToInt32(GetAutoNumber());
                    var productAutoSequence = GetAutoSequence();

                    foreach (DataRow row in dt.Rows)
                    {
                        var productName = "";
                        var manufacturer = manufacturerList.FirstOrDefault(x => x.Name == row["BRAND"]?.ToString().Trim());
                        var productCategory = productCategoryList.FirstOrDefault(x => x.Name == row["GROUP NAME"]?.ToString().Trim());
                        var productSubCategory = productSubCategoryList.FirstOrDefault(x => x.Name == row["PRODUCTS NAME"]?.ToString().Trim());
                        var style = styleList.FirstOrDefault(x => x.Name == row["STYLE/SIZE"]?.ToString().Trim());
                        productName = string.Join(" ", new string[] { manufacturer?.Name.Trim(), productSubCategory?.Name.Trim(), style?.Name.Trim() });
                        if (!string.IsNullOrEmpty(productName))
                        {
                            if (!productListDb.Any(x => x.Name == productName))
                            {
                                var product = new Product
                                {
                                    Id = productAutoId.ToString(),
                                    Sequence = productAutoSequence,
                                    Code = productAutoId.ToString(),
                                    Name = productName,
                                    UOMId = "1",
                                    ManufacturerId = manufacturer?.Id,
                                    ProductCategoryId = productCategory?.Id,
                                    ProductSubCategoryId = productSubCategory?.Id,
                                    StyleId = style?.Id,
                                    ReorderLevel = 0,
                                    ShelfLife = 0,
                                    MaxDiscount = 0,
                                    WholeSalePrice = 0
                                };
                                product.Active = true;
                                product.SynchronizationType = SynchronizationType.Server.ToString();
                                product.AddedBy = identity.Name;
                                product.AddedDate = DateTime.Now;
                                product.AddedFromIp = identity.IpAddress;
                                if (int.TryParse(row["TP"]?.ToString().Trim(), out var purchasePrice))
                                    product.PurchasePrice = purchasePrice;
                                if (decimal.TryParse(row["MRP"]?.ToString().Trim(), out var retailPrice))
                                    product.RetailPrice = retailPrice;
                                productList.Add(product);
                                productAutoId++;
                                productAutoSequence++;
                            }
                        }
                        else
                        {
                            row[0] = "Product is null!";
                            result.Rows.Add(row.ItemArray);
                        }
                    }
                    BulkUpload(productList);
                    return result;
                }
                throw new Exception("File is not found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public DataTable UploadFromDirectoryForAmericanTMart(HttpPostedFileBase file)
        {
            try
            {
                var dt = new DataTable();
                dt.Columns.AddRange(new[] {
                        new DataColumn("Code", typeof(string)),
                        new DataColumn("Supplier", typeof(string)),
                        new DataColumn("Brand", typeof(string)),
                        new DataColumn("Name", typeof(string)),
                        new DataColumn("Category", typeof(string)),
                        new DataColumn("SubCategory", typeof(string)),
                        new DataColumn("Style", typeof(string)),
                        new DataColumn("Size", typeof(string)),
                        new DataColumn("TP", typeof(decimal)),
                        new DataColumn("MRP", typeof(decimal)),
                        new DataColumn("Error", typeof(string))
                    });
                if (file != null)
                {
                    var directoryLocation = WebConfigurationManager.AppSettings["ErrorFilePath"];
                    if (!Directory.Exists(directoryLocation))
                    {
                        Directory.CreateDirectory(directoryLocation);
                    }
                    var filePath = directoryLocation + Path.GetFileName(file.FileName);
                    var extension = Path.GetExtension(file.FileName);
                    file.SaveAs(filePath);
                    var conStr = "";
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03
                            conStr = WebConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
                            break;
                        case ".xlsx": //Excel 07-16
                            conStr = WebConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            break;
                        default:
                            break;
                    }
                    conStr = string.Format(conStr, filePath, "Yes", "2");
                    using (var excelCon = new OleDbConnection(conStr))
                    {
                        excelCon.Open();
                        var dtExcelSchema = excelCon.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                        if (dtExcelSchema != null)
                        {
                            var sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                            var query = "select * from [" + sheetName + "]";
                            var da = new OleDbDataAdapter(query, excelCon);
                            da.Fill(dt);
                        }
                        excelCon.Close();
                    }
                    var result = new DataTable();
                    result.Columns.AddRange(new[] {
                       new DataColumn("Code", typeof(string)),
                        new DataColumn("Supplier", typeof(string)),
                        new DataColumn("Brand", typeof(string)),
                        new DataColumn("Name", typeof(string)),
                        new DataColumn("Category", typeof(string)),
                        new DataColumn("SubCategory", typeof(string)),
                        new DataColumn("Style", typeof(string)),
                        new DataColumn("Size", typeof(string)),
                        new DataColumn("TP", typeof(decimal)),
                        new DataColumn("MRP", typeof(decimal)),
                        new DataColumn("Error", typeof(string))
                    });
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                    var supplierNames = (from t in dt.AsEnumerable() select t.Field<string>("Supplier")).AsEnumerable().Distinct();
                    var supplierList = _supplierRepository.GetAll(x => supplierNames.Contains(x.Name)).ToList();

                    var brandNames = (from t in dt.AsEnumerable() select t.Field<string>("Brand")).AsEnumerable().Distinct();
                    var brandList = _brandRepository.GetAll(x => brandNames.Contains(x.Name)).ToList();

                    var productCategoryNames = (from t in dt.AsEnumerable() select t.Field<string>("Category")).AsEnumerable().Distinct();
                    var productCategoryList = _productCategoryRepository.GetAll(x => productCategoryNames.Contains(x.Name)).ToList();

                    var productSubCategoryNames = (from t in dt.AsEnumerable() select t.Field<string>("SubCategory")).AsEnumerable().Distinct();
                    var productSubCategoryList = _productSubCategoryRepository.GetAll(x => productSubCategoryNames.Contains(x.Name)).ToList();

                    var styleNames = (from t in dt.AsEnumerable() select t.Field<string>("Style")).AsEnumerable().Distinct();
                    var styleList = _styleRepository.GetAll(x => styleNames.Contains(x.Name)).ToList();

                    var sizeNames = (from t in dt.AsEnumerable() select t.Field<string>("Size")).AsEnumerable().Distinct();
                    var sizeList = _sizeRepository.GetAll(x => sizeNames.Contains(x.Name)).ToList();

                    var productListDb = _productRepository.GetAll(x => !x.Archive).ToList();
                    var productList = new List<Product>();
                    var productAutoId = Convert.ToInt32(GetAutoNumber());
                    var productAutoSequence = GetAutoSequence();

                    foreach (DataRow row in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["Code"]?.ToString().Trim()))
                        {
                            var supplierProductCode = row["Code"]?.ToString().Trim();
                            var supplier = supplierList.FirstOrDefault(x => x.Name == row["Supplier"]?.ToString().Trim());
                            var brand = brandList.FirstOrDefault(x => x.Name == row["Brand"]?.ToString().Trim());
                            var productCategory = productCategoryList.FirstOrDefault(x => x.Name == row["Category"]?.ToString().Trim());
                            var productSubCategory = productSubCategoryList.FirstOrDefault(x => x.Name == row["SubCategory"]?.ToString().Trim());
                            var style = styleList.FirstOrDefault(x => x.Name == row["Style"]?.ToString().Trim());
                            var size = sizeList.FirstOrDefault(x => x.Name == row["Size"]?.ToString().Trim());
                            if (!string.IsNullOrEmpty(row["Name"]?.ToString().Trim()))
                            {
                                var name = row["Name"]?.ToString().Trim();
                                if (!productListDb.Any(x => x.Name == name))
                                {
                                    var product = new Product
                                    {
                                        Id = productAutoId.ToString(),
                                        Sequence = productAutoSequence,
                                        Code = supplierProductCode + "-" + supplier?.Name?.Substring(0, 3) + "-" + brand?.Name?.Substring(0, 3) + "-" + productAutoId,
                                        Name = name,
                                        UOMId = "1",
                                        SupplierProductCode=supplierProductCode,
                                        SupplierId = supplier?.Id,
                                        BrandId=brand?.Id,
                                        ProductCategoryId = productCategory?.Id,
                                        ProductSubCategoryId = productSubCategory?.Id,
                                        StyleId = style?.Id,
                                        SizeId=size?.Id,
                                        ReorderLevel = 0,
                                        ShelfLife = 0,
                                        MaxDiscount = 0,
                                        WholeSalePrice = 0
                                    };
                                    product.Active = true;
                                    product.SynchronizationType = SynchronizationType.Server.ToString();
                                    product.AddedBy = identity.Name;
                                    product.AddedDate = DateTime.Now;
                                    product.AddedFromIp = identity.IpAddress;
                                    if (int.TryParse(row["TP"]?.ToString().Trim(), out var purchasePrice))
                                        product.PurchasePrice = purchasePrice;
                                    if (decimal.TryParse(row["MRP"]?.ToString().Trim(), out var retailPrice))
                                        product.RetailPrice = retailPrice;
                                    productList.Add(product);
                                    productAutoId++;
                                    productAutoSequence++;
                                }
                            }
                            else
                            {
                                row[0] = "Product is null!";
                                result.Rows.Add(row.ItemArray);
                            }
                        }
                        else
                        {
                            row[0] = "Supplier Code is null!";
                            result.Rows.Add(row.ItemArray);
                        }
                    }
                    BulkUpload(productList);
                    return result;
                }
                throw new Exception("File is not found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Product GetById(string id)
        {
            try
            {
                return _productRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Product GetByCode(string code)
        {
            try
            {
                return _productRepository.GetOne(x => x.Code == code);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<Product> GetAll()
        {
            try
            {
                return _productRepository.GetAll(r => !r.Archive, "ProductCategory,ProductSubCategory,Manufacturer,UOM,RAM,ROM,Supplier,Brand,Grade").OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<Product> GetAll(string productCategoryId)
        {
            try
            {
                return _productRepository.GetAll(r => !r.Archive && r.ProductCategoryId == productCategoryId, "ProductCategory,ProductSubCategory,Manufacturer,UOM,RAM,ROM,Supplier,Brand,Grade").OrderBy(x => x.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<Product> GetAll(string productCategoryId, string productSubCategoryId)
        {
            try
            {
                return _productRepository.GetAll(r => !r.Archive && r.ProductCategoryId == productCategoryId && r.ProductSubCategoryId == productSubCategoryId,
                    "ProductCategory,ProductSubCategory,Manufacturer,UOM,RAM,ROM,Supplier,Brand,Grade").OrderBy(x => x.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<object> GetAllPurchaseItemList()
        {
            try
            {
                IEnumerable<string> purchaseDetailItemIds = _purchaseDetailRepository.GetAll(r => !r.Archive).GroupBy(r => r.ProductId).Select(r => r.FirstOrDefault().ProductId).ToList();
                return (from r in _productRepository.GetAll(r => !r.Archive && purchaseDetailItemIds.Contains(r.Id)).OrderBy(r => r.Sequence)
                        select new
                        {
                            value = r.Id,
                            label = r.Name
                        }).ToList<object>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<object> GetAllPurchaseItemList(string companyId, string branchId)
        {
            try
            {
                IEnumerable<string> purchaseDetailItemIds = _purchaseDetailRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId).GroupBy(r => r.ProductId).Select(r => r.FirstOrDefault().ProductId).ToList();
                return (from r in _productRepository.GetAll(r => !r.Archive && purchaseDetailItemIds.Contains(r.Id)).OrderBy(r => r.Sequence)
                        select new
                        {
                            value = r.Id,
                            label = r.Name
                        }).ToList<object>();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<object> Lists()
        {
            try
            {
                return from r in _productRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public override int GetAutoSequence()
        {
            try
            {
                return GetAutoSequence("Product");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<OpeningBlance> GetAllProductList()
        {
            return null;
        }
    }
}
