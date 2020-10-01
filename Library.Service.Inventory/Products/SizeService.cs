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
    public class SizeService : Service<Size>, ISizeService
    {

        #region Ctor
        private readonly IRepository<Size> _sizeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public SizeService(
            IRepository<Size> sizeRepository,
            IUnitOfWork unitOfWork) : base(sizeRepository)
        {
            _sizeRepository = sizeRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Action

        private void Check(Size model)
        {
            try
            {
                CheckUniqueColumn("Name", model.Name, r => r.Id != model.Id && r.Name == model.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void BulkUpload(List<Size> productCategoryList)
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
                            bulkCopy1.DestinationTableName = "dbo.Size";
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
                    var secondTable = dt.AsEnumerable().GroupBy(row => row.Field<string>("Name")).Select(group => group.First()).CopyToDataTable();
                    var productCategoryListDb = _sizeRepository.GetAll(x => !x.Archive).ToList();
                    var productCategoryList = new List<Size>();
                    var productCategoryAutoId = Convert.ToInt32(GetAutoNumber());
                    var productCategoryAutoSequence = GetAutoSequence();
                    foreach (DataRow row in secondTable.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["Name"]?.ToString().Trim()))
                        {
                            var name = row["Name"]?.ToString().Trim();
                            if (!productCategoryListDb.Any(x => x.Name == name))
                            {
                                var productCategory = new Size
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
        public void Add(Size size)
        {
            try
            {
                Check(size);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                size.Id = GetAutoNumber();
                size.Sequence = GetAutoSequence();
                size.SynchronizationType = SynchronizationType.Server.ToString();
                size.AddedBy = identity.Name;
                size.AddedDate = DateTime.Now;
                size.AddedFromIp = identity.IpAddress;
                _sizeRepository.Add(size);
                _unitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(Size size)
        {
            try
            {
                Check(size);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var sizeDb = _sizeRepository.GetOne(size.Id);
                size.Sequence = sizeDb.Sequence;
                size.SynchronizationType = sizeDb.SynchronizationType;
                size.AddedBy = sizeDb.AddedBy;
                size.AddedDate = sizeDb.AddedDate;
                size.AddedFromIp = sizeDb.AddedFromIp;
                size.UpdatedBy = identity.Name;
                size.UpdatedDate = DateTime.Now;
                size.UpdatedFromIp = identity.IpAddress;
                _sizeRepository.Update(size);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Size GetById(string id)
        {
            try
            {
                return _sizeRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Size> GetAll()
        {
            try
            {
                return _sizeRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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

                return from r in _sizeRepository.GetAll(r => !r.Archive && r.Active)
                                                 .OrderByDescending(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Size GetSizeById(string sizeId)
        {
            try
            {
                return _sizeRepository.GetOne(sizeId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion
    }
}
