using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Products;
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

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Class ProductSubCategoryService.
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class ProductSubCategoryService : Service<ProductSubCategory>, IProductSubCategoryService
    {
        #region Ctor
        private readonly IRepository<ProductSubCategory> _productSubCategoryRepository;
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductSubCategoryService(
            IRepository<ProductSubCategory> productSubCategoryRepository,
            IRepository<ProductCategory> productCategoryRepository,
            IUnitOfWork unitOfWork
            ) : base(productSubCategoryRepository)
        {
            _productSubCategoryRepository = productSubCategoryRepository;
            _productCategoryRepository = productCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion
        private void Check(ProductSubCategory model)
        {
            try
            {
                CheckUniqueColumn("Name", model.Name, r => r.Id != model.Id && r.ProductCategoryId == model.ProductCategoryId && r.Name == model.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void BulkUpload(List<ProductSubCategory> brandList)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ErpdbEntities"].ConnectionString;

                #region brand
                var brand = new DataTable();
                brand.Columns.Add("Id", typeof(string));
                brand.Columns.Add("Sequence", typeof(int));
                brand.Columns.Add("Code", typeof(string));
                brand.Columns.Add("Name", typeof(string));
                brand.Columns.Add("ProductCategoryId", typeof(string));
                brand.Columns.Add("Active", typeof(bool));
                brand.Columns.Add("Archive", typeof(bool));
                brand.Columns.Add("IsSynchronized", typeof(bool));
                brand.Columns.Add("IsUpdated", typeof(bool));
                brand.Columns.Add("SynchronizationType", typeof(string));
                brand.Columns.Add("Description", typeof(string));
                brand.Columns.Add("AddedBy", typeof(string));
                brand.Columns.Add("AddedDate", typeof(DateTime));
                brand.Columns.Add("AddedFromIp", typeof(string));
                brand.Columns.Add("UpdatedBy", typeof(string));
                brand.Columns.Add("UpdatedDate", typeof(DateTime));
                brand.Columns.Add("UpdatedFromIp", typeof(string));
                foreach (var entity in brandList)
                {
                    var row = brand.NewRow();
                    row["Id"] = entity.Id;
                    row["Sequence"] = entity.Sequence;
                    row["Code"] = entity.Code;
                    row["Name"] = entity.Name;
                    row["ProductCategoryId"] = entity.ProductCategoryId;
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
                    brand.Rows.Add(row);
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
                            bulkCopy1.DestinationTableName = "dbo.ProductSubCategory";
                            // Write from the source to the customer.
                            bulkCopy1.WriteToServer(brand);
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

        public void Add(ProductSubCategory productSubCategory)
        {
            try
            {
                Check(productSubCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                productSubCategory.Id = GetAutoNumber();
                productSubCategory.Sequence = GetAutoSequence();
                productSubCategory.SynchronizationType = SynchronizationType.Server.ToString(); ;
                productSubCategory.AddedBy = identity.Name;
                productSubCategory.AddedDate = DateTime.Now;
                productSubCategory.AddedFromIp = identity.IpAddress;
                _productSubCategoryRepository.Add(productSubCategory);
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
                            var query = "select Category,SubCategory from [" + sheetName + "]";
                            var da = new OleDbDataAdapter(query, excelCon);
                            da.Fill(dt);
                        }
                        excelCon.Close();
                    }
                    var result = new DataTable();
                    result.Columns.AddRange(new[] {
                        new DataColumn("Category", typeof(string)),
                        new DataColumn("SubCategory", typeof(string)),
                        new DataColumn("Error)", typeof(string))
                    });
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                    var uniqueList = dt.AsEnumerable().GroupBy(row => row.Field<string>("SubCategory")).Select(group => group.First()).CopyToDataTable();
                    var productCategoryNames = (from t in uniqueList.AsEnumerable() select t.Field<string>("Category")).AsEnumerable().Distinct();
                    var productCategoryListDb = _productCategoryRepository.GetAll(r => productCategoryNames.Contains(r.Name)).ToList();
                    var brandListDb = _productSubCategoryRepository.GetAll(x => !x.Archive).ToList();
                    var productCategoryList = new List<ProductSubCategory>();
                    var productCategoryAutoId = Convert.ToInt32(GetAutoNumber());
                    var productCategoryAutoSequence = GetAutoSequence();
                    foreach (DataRow row in uniqueList.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["SubCategory"]?.ToString().Trim()))
                        {
                            var name = row["SubCategory"]?.ToString().Trim();
                            string categoryName = row["Category"].ToString().Trim();
                            var productCategory = productCategoryListDb.FirstOrDefault(r => r.Name == categoryName);
                            if (null != productCategory)
                            {
                                if (!brandListDb.Any(x => x.ProductCategoryId == productCategory.Id && x.Name == name))
                                {
                                    var productSubCategory = new ProductSubCategory
                                    {
                                        Id = productCategoryAutoId.ToString(),
                                        Sequence = productCategoryAutoSequence,
                                        Code = productCategoryAutoId.ToString(),
                                        Name = name,
                                        ProductCategoryId = productCategory.Id,
                                        Active = true,
                                        SynchronizationType = SynchronizationType.Server.ToString(),
                                        AddedBy = identity.Name,
                                        AddedDate = DateTime.Now,
                                        AddedFromIp = identity.IpAddress
                                    };
                                    productCategoryList.Add(productSubCategory);
                                    productCategoryAutoId++;
                                    productCategoryAutoSequence++;
                                }
                            }
                            else
                            {
                                row[0] = "productCategory is null!";
                                result.Rows.Add(row.ItemArray);
                            }
                        }
                        else
                        {
                            row[0] = "productCategory is null!";
                            result.Rows.Add(row.ItemArray);
                        }
                    }
                    BulkUpload(productCategoryList);
                    return result;
                }
                throw new Exception("File is not found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void Update(ProductSubCategory productSubCategory)
        {
            try
            {
                Check(productSubCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _productSubCategoryRepository.GetOne(productSubCategory.Id);
                productSubCategory.Sequence = dbdata.Sequence;
                productSubCategory.SynchronizationType = dbdata.SynchronizationType;
                productSubCategory.AddedBy = dbdata.AddedBy;
                productSubCategory.AddedDate = dbdata.AddedDate;
                productSubCategory.AddedFromIp = dbdata.AddedFromIp;
                productSubCategory.UpdatedBy = identity.Name;
                productSubCategory.UpdatedDate = DateTime.Now;
                productSubCategory.UpdatedFromIp = identity.IpAddress;
                _productSubCategoryRepository.Update(productSubCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public ProductSubCategory GetById(string id)
        {
            try
            {
                return _productSubCategoryRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public IEnumerable<ProductSubCategory> GetAll()
        {
            try
            {
                return  _productSubCategoryRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public IEnumerable<ProductSubCategory> GetAll(string productCategoryId)
        {
            try
            {
                return _productSubCategoryRepository.GetAll(r => !r.Archive && r.ProductCategoryId == productCategoryId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public IEnumerable<object> Lists(string productCategoryId)
        {
            try
            {
                return from r in _productSubCategoryRepository.GetAll(r => !r.Archive && r.Active && r.ProductCategoryId == productCategoryId)
                                                 .OrderBy(r => r.Sequence)
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
                return GetAutoSequence("ProductSubCategory");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
