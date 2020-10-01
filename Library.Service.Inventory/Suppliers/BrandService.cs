#region Using

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

#endregion

namespace Library.Service.Inventory.Suppliers
{
    /// <summary>
    /// Class BrandService.
    /// <remarks>Jahangir, 14-03-2016</remarks>
    /// </summary>
    public class BrandService : Service<Brand>, IBrandService
    {
        #region Ctor
        private readonly IRepository<Brand> _brandRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IUnitOfWork _unitOfWork;
        public BrandService(
            IRepository<Brand> brandRepository,
            IRepository<Supplier> supplierRepository,
            IUnitOfWork unitOfWork
            ) : base(brandRepository)
        {
            _brandRepository = brandRepository;
            _supplierRepository = supplierRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Brand model)
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

        private void BulkUpload(List<Brand> brandList)
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
                brand.Columns.Add("SupplierId", typeof(string));
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
                    row["SupplierId"] = entity.SupplierId;
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
                            bulkCopy1.DestinationTableName = "dbo.brand";
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
                            var query = "select Supplier,Brand from [" + sheetName + "]";
                            var da = new OleDbDataAdapter(query, excelCon);
                            da.Fill(dt);
                        }
                        excelCon.Close();
                    }
                    var result = new DataTable();
                    result.Columns.AddRange(new[] {
                        new DataColumn("Supplier", typeof(string)),
                        new DataColumn("Brand", typeof(string)),
                        new DataColumn("Error)", typeof(string))
                    });
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                    var uniqueList = dt.AsEnumerable().GroupBy(row => row.Field<string>("Brand")).Select(group => group.First()).CopyToDataTable();
                    var supplierNames = (from t in uniqueList.AsEnumerable() select t.Field<string>("Supplier")).AsEnumerable().Distinct();
                    var supplierListDb = _supplierRepository.GetAll(r => supplierNames.Contains(r.Name)).ToList();
                    var brandListDb = _brandRepository.GetAll(x => !x.Archive).ToList();
                    var productCategoryList = new List<Brand>();
                    var productCategoryAutoId = Convert.ToInt32(GetAutoNumber());
                    var productCategoryAutoSequence = GetAutoSequence();
                    foreach (DataRow row in uniqueList.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["Brand"]?.ToString().Trim()))
                        {
                            var name = row["Brand"]?.ToString().Trim();
                            string supplierName = row["Supplier"].ToString().Trim();
                            var supplier = supplierListDb.FirstOrDefault(r => r.Name == supplierName);
                            if (null != supplier)
                            {
                                if (!brandListDb.Any(x => x.SupplierId ==supplier.Id && x.Name == name))
                                {
                                    var productCategory = new Brand
                                    {
                                        Id = productCategoryAutoId.ToString(),
                                        Sequence = productCategoryAutoSequence,
                                        Code = productCategoryAutoId.ToString(),
                                        Name = name,
                                        SupplierId=supplier.Id,
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="brand"></param>
        public void Add(Brand brand)
        {
            try
            {
                Check(brand);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                brand.Id = GetAutoNumber();
                brand.Sequence = GetAutoSequence();
                brand.SynchronizationType = SynchronizationType.Server.ToString();
                brand.AddedBy = identity.Name;
                brand.AddedDate = DateTime.Now;
                brand.AddedFromIp = identity.IpAddress;
                _brandRepository.Add(brand);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="brand"></param>
        public void Update(Brand brand)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _brandRepository.GetOne(brand.Id);
                brand.Sequence = dbdata.Sequence;
                brand.SynchronizationType = dbdata.SynchronizationType;
                brand.AddedBy = dbdata.AddedBy;
                brand.AddedDate = dbdata.AddedDate;
                brand.AddedFromIp = dbdata.AddedFromIp;
                brand.UpdatedBy = identity.Name;
                brand.UpdatedDate = DateTime.Now;
                brand.UpdatedFromIp = identity.IpAddress;
                _brandRepository.Update(brand);
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
                var dbdata = _brandRepository.GetOne(id);
                dbdata.Archive = true;
                dbdata.Active = false;
                _brandRepository.Update(dbdata);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Brand GetById(string id)
        {
            try
            {
                return _brandRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Brand> GetAll()
        {
            try
            {
                var brands = _brandRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
                return brands;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> Lists()
        {
            try
            {
                IEnumerable<object> result = from r in _brandRepository.GetAll(r =>  !r.Archive && r.Active ).OrderByDescending(r => r.Sequence)
                                             select new { Value = r.Id, Text = r.Name };
                return result;
            }
            catch
            {
                return null;
            }
        }

        public IEnumerable<object> Lists(string supplierId)
        {
            try
            {
                IEnumerable<object> result = from r in _brandRepository.GetAll(r =>  !r.Archive && r.Active && r.SupplierId == supplierId).OrderByDescending(r => r.Sequence)
                                             select new { Value = r.Id, Text = r.Name };
                return result;
            }
            catch
            {
                return null;
            }
        }

        public override int GetAutoSequence()
        {
            try
            {
                return GetAutoSequence("Brand");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
