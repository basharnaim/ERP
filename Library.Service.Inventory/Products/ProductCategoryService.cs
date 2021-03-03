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
    /// Class ColorService.
    /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class ProductCategoryService : Service<ProductCategory>, IProductCategoryService
    {
        #region Ctor
        private readonly IRepository<ProductCategory> _productCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductCategoryService(
            IRepository<ProductCategory> productCategoryRepository,
            IUnitOfWork unitOfWork) : base(productCategoryRepository)
        {
            _productCategoryRepository = productCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(ProductCategory entity)
        {
            CheckUniqueColumn("Name", entity.Name, x => x.Id != entity.Id && x.Name == entity.Name);
        }

        private void BulkUpload(List<ProductCategory> productCategoryList)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ErpdbEntities"].ConnectionString;

                #region productCategory
                var productCategory = new DataTable();
                productCategory.Columns.Add("Id", typeof(string));
                productCategory.Columns.Add("Sequence", typeof(int));
                productCategory.Columns.Add("Code", typeof(string));
                productCategory.Columns.Add("Name", typeof(string));
                productCategory.Columns.Add("Active", typeof(bool));
                productCategory.Columns.Add("Archive", typeof(bool));
                productCategory.Columns.Add("IsSynchronized", typeof(bool));
                productCategory.Columns.Add("IsUpdated", typeof(bool));
                productCategory.Columns.Add("SynchronizationType", typeof(string));
                productCategory.Columns.Add("Description", typeof(string));
                productCategory.Columns.Add("AddedBy", typeof(string));
                productCategory.Columns.Add("AddedDate", typeof(DateTime));
                productCategory.Columns.Add("AddedFromIp", typeof(string));
                productCategory.Columns.Add("UpdatedBy", typeof(string));
                productCategory.Columns.Add("UpdatedDate", typeof(DateTime));
                productCategory.Columns.Add("UpdatedFromIp", typeof(string));
                foreach (var entity in productCategoryList)
                {
                    var row = productCategory.NewRow();
                    row["Id"] = entity.Id;
                    row["Sequence"] = entity.Sequence;
                    row["Code"] = entity.Code;
                    row["Name"] = entity.Name;
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
                    productCategory.Rows.Add(row);
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
                            bulkCopy1.DestinationTableName = "dbo.ProductCategory";
                            // Write from the source to the customer.
                            bulkCopy1.WriteToServer(productCategory);
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
                            var query = "select Name from [" + sheetName + "]";
                            var da = new OleDbDataAdapter(query, excelCon);
                            da.Fill(dt);
                        }
                        excelCon.Close();
                    }
                    var result = new DataTable();
                    result.Columns.AddRange(new[] {
                        new DataColumn("Name", typeof(string)),
                        new DataColumn("Error)", typeof(string))
                    });
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                    var secondTable = dt; //.AsEnumerable().GroupBy(row => row.Field<string>("Name")).Select(group => group.First()).CopyToDataTable();
                    var productCategoryListDb = _productCategoryRepository.GetAll(x => !x.Archive).ToList();
                    var productCategoryList = new List<ProductCategory>();
                    var productCategoryAutoId = Convert.ToInt32(GetAutoNumber());
                    var productCategoryAutoSequence = GetAutoSequence();
                    foreach (DataRow row in secondTable.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["Name"]?.ToString().Trim()))
                        {
                            var name = row["Name"]?.ToString().Trim();
                            if (!productCategoryListDb.Any(x => x.Name == name))
                            {
                                var productCategory = new ProductCategory
                                {
                                    Id = productCategoryAutoId.ToString(),
                                    Sequence = productCategoryAutoSequence,
                                    Code = productCategoryAutoId.ToString(),
                                    Name = name,
                                    Active = true,
                                    SynchronizationType = SynchronizationType.Server.ToString(),
                                    AddedBy = identity.Name,
                                    AddedDate = DateTime.Now,
                                    AddedFromIp = identity.IpAddress
                                };
                                productCategoryList.Add(productCategory);
                                productCategoryAutoId++;
                                productCategoryAutoSequence++;
                            }
                        }
                        else
                        {
                            row[0] = "Category is null!";
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

        /// <summary>
        /// Adds the specified colorvm.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <param name="productCategory">The colorvm.</param>
        public void Add(ProductCategory productCategory)
        {
            try
            {
                Check(productCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                productCategory.Id = GetAutoNumber();
                productCategory.Sequence = GetAutoSequence();
                productCategory.SynchronizationType = SynchronizationType.Server.ToString();
                productCategory.AddedBy = identity.Name;
                productCategory.AddedDate = DateTime.Now;
                productCategory.AddedFromIp = identity.IpAddress;
                _productCategoryRepository.Add(productCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the specified colorvm.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <param name="productCategory">The colorvm.</param>
        public void Update(ProductCategory productCategory)
        {
            try
            {
                Check(productCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var productCategoryDb = _productCategoryRepository.GetOne(productCategory.Id);
                productCategory.Sequence = productCategoryDb.Sequence;
                productCategory.SynchronizationType = productCategoryDb.SynchronizationType;
                productCategory.AddedBy = productCategoryDb.AddedBy;
                productCategory.AddedDate = productCategoryDb.AddedDate;
                productCategory.AddedFromIp = productCategoryDb.AddedFromIp;
                productCategory.UpdatedBy = identity.Name;
                productCategory.UpdatedDate = DateTime.Now;
                productCategory.UpdatedFromIp = identity.IpAddress;
                _productCategoryRepository.Update(productCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Color.</returns>
        public ProductCategory GetById(string id)
        {
            try
            {
                return _productCategoryRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <returns>IEnumerable&lt;Color&gt;.</returns>
        public IEnumerable<ProductCategory> GetAll()
        {
            try
            {
                return  _productCategoryRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BranchId"></param>
        /// <returns></returns>
        public IEnumerable<object> Lists()
        {
            try
            {
                return from r in _productCategoryRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence).ToList() select new { Value = r.Id, Text = r.Name };
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
                return GetAutoSequence("ProductCategory");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
