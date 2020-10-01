using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
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

namespace Library.Service.Inventory.Suppliers
{
    /// <summary>
    /// Class SupplierService.
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class SupplierService : Service<Supplier>, ISupplierService
    {
        #region Ctor
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IUnitOfWork _unitOfWork;
        public SupplierService(
            IRepository<Supplier> supplierRepository,
            IUnitOfWork unitOfWork
            ) : base(supplierRepository)
        {
            _supplierRepository = supplierRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Supplier model)
        {
            try
            {
                CheckUniqueColumn("Phone1", model.Phone1, r => r.Id != model.Id && r.Phone1 == model.Phone1);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void BulkUpload(List<Supplier> productCategoryList)
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
                productCategory.Columns.Add("SupplierCategoryId", typeof(string));
                productCategory.Columns.Add("AccountCode", typeof(string));
                productCategory.Columns.Add("Address1", typeof(string));
                productCategory.Columns.Add("Address2", typeof(string));
                productCategory.Columns.Add("CountryId", typeof(string));
                productCategory.Columns.Add("DivisionId", typeof(string));
                productCategory.Columns.Add("DistrictId", typeof(string));
                productCategory.Columns.Add("ContactPerson", typeof(string));
                productCategory.Columns.Add("ContactPersonDesignation", typeof(string));
                productCategory.Columns.Add("Phone1", typeof(string));
                productCategory.Columns.Add("Phone2", typeof(string));
                productCategory.Columns.Add("Email", typeof(string));
                productCategory.Columns.Add("Website", typeof(string));
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
                    row["SupplierCategoryId"] = entity.SupplierCategoryId ?? (object)DBNull.Value;
                    row["AccountCode"] = entity.AccountCode ?? (object)DBNull.Value;
                    row["Address1"] = entity.Address1 ?? (object)DBNull.Value;
                    row["Address2"] = entity.Address2 ?? (object)DBNull.Value;
                    row["CountryId"] = entity.CountryId ?? (object)DBNull.Value;
                    row["DivisionId"] = entity.DivisionId ?? (object)DBNull.Value;
                    row["DistrictId"] = entity.DistrictId ?? (object)DBNull.Value;
                    row["ContactPerson"] = entity.ContactPerson ?? (object)DBNull.Value;
                    row["ContactPersonDesignation"] = entity.ContactPersonDesignation ?? (object)DBNull.Value;
                    row["Phone1"] = entity.Phone1 ?? (object)DBNull.Value;
                    row["Phone2"] = entity.Phone2 ?? (object)DBNull.Value;
                    row["Email"] = entity.Email ?? (object)DBNull.Value;
                    row["Website"] = entity.Website ?? (object)DBNull.Value;
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
                            bulkCopy1.DestinationTableName = "dbo.Supplier";
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
                    var supplierListDb = _supplierRepository.GetAll(x => !x.Archive).ToList();
                    var productCategoryList = new List<Supplier>();
                    var productCategoryAutoId = Convert.ToInt32(GetAutoNumber());
                    var productCategoryAutoSequence = GetAutoSequence();
                    foreach (DataRow row in secondTable.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["Name"]?.ToString().Trim()))
                        {
                            var name = row["Name"]?.ToString().Trim();
                            if (!supplierListDb.Any(x => x.Name == name))
                            {
                                var productCategory = new Supplier
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
                            row[0] = "Supplier is null!";
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

        public void Add(Supplier supplier)
        {
            try
            {
                Check(supplier);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                supplier.Id = GetAutoNumber();
                supplier.Sequence = GetAutoSequence();
                supplier.SynchronizationType = SynchronizationType.Server.ToString();
                supplier.AddedBy = identity.Name;
                supplier.AddedDate = DateTime.Now;
                supplier.AddedFromIp = identity.IpAddress;
                _supplierRepository.Add(supplier);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(Supplier supplier)
        {
            try
            {
                Check(supplier);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _supplierRepository.GetOne(supplier.Id);
                supplier.Sequence = dbdata.Sequence;
                supplier.SynchronizationType = dbdata.SynchronizationType;
                supplier.AddedBy = dbdata.AddedBy;
                supplier.AddedDate = dbdata.AddedDate;
                supplier.AddedFromIp = dbdata.AddedFromIp;
                supplier.UpdatedBy = identity.Name;
                supplier.UpdatedDate = DateTime.Now;
                supplier.UpdatedFromIp = identity.IpAddress;
                _supplierRepository.Update(supplier);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Archive(string id)
        {
            try
            {
                var dbdata = _supplierRepository.GetOne(id);
                dbdata.Archive = true;
                _supplierRepository.Update(dbdata);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Supplier GetById(string id)
        {
            try
            {
                return _supplierRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Supplier> GetAll()
        {
            try
            {
                return _supplierRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Supplier> GetAll(string supplierCategoryId)
        {
            try
            {
                return _supplierRepository.GetAll(r => !r.Archive && r.SupplierCategoryId == supplierCategoryId).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                IEnumerable<object> result = from r in _supplierRepository.GetAll(r => r.Active && !r.Archive).OrderByDescending(r => r.Sequence)
                                             select new { Value = r.Id, Text = r.Name };
                return result;
            }
            catch
            {
                return null;
            }
        }

        public Supplier GetSupplierBySupplierMobileNumber(string supplierMobile)
        {
            try
            {
                return _supplierRepository.GetOne(x => x.Phone1 == supplierMobile.Trim());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Supplier> GetAllSupplierBySupplierMobileNumber(string supplierMobile)
        {
            try
            {
                return _supplierRepository.GetAll(x => x.Phone1 == supplierMobile.Trim()).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Supplier GetSupplierBySupplierId(string supplierId)
        {
            try
            {
                return _supplierRepository.GetOne(x => x.Id == supplierId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Supplier> GetAllSupplierBySupplierId(string supplierId)
        {
            try
            {
                return _supplierRepository.GetAll(x => x.Id == supplierId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public string GetSupplierNameById(string id)
        {
            try
            {
                var supplierName = _supplierRepository.GetOne(id).Name;
                return supplierName;
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
                return GetAutoSequence("Supplier");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
