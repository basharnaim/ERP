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
using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Products;
using Library.Service.Core.Core;

namespace Library.Service.Inventory.Products
{
    public class ManufacturerService : Service<Manufacturer>, IManufacturerService
    {
        #region Ctor
        private readonly IRepository<Manufacturer> _manufacturerRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ManufacturerService(
            IRepository<Manufacturer> manufacturerRepository,
            IUnitOfWork unitOfWork
            ) : base(manufacturerRepository)
        {
            _manufacturerRepository = manufacturerRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Manufacturer model)
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

        private void BulkUpload(List<Manufacturer> manufacturerList)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ErpdbEntities"].ConnectionString;

                #region Manufacturer
                var Manufacturer = new DataTable();
                Manufacturer.Columns.Add("Id", typeof(string));
                Manufacturer.Columns.Add("Sequence", typeof(int));
                Manufacturer.Columns.Add("Code", typeof(string));
                Manufacturer.Columns.Add("Name", typeof(string));
                Manufacturer.Columns.Add("Active", typeof(bool));
                Manufacturer.Columns.Add("Archive", typeof(bool));
                Manufacturer.Columns.Add("IsSynchronized", typeof(bool));
                Manufacturer.Columns.Add("IsUpdated", typeof(bool));
                Manufacturer.Columns.Add("SynchronizationType", typeof(string));
                Manufacturer.Columns.Add("Description", typeof(string));
                Manufacturer.Columns.Add("AddedBy", typeof(string));
                Manufacturer.Columns.Add("AddedDate", typeof(DateTime));
                Manufacturer.Columns.Add("AddedFromIp", typeof(string));
                Manufacturer.Columns.Add("UpdatedBy", typeof(string));
                Manufacturer.Columns.Add("UpdatedDate", typeof(DateTime));
                Manufacturer.Columns.Add("UpdatedFromIp", typeof(string));
                foreach (var entity in manufacturerList)
                {
                    var row = Manufacturer.NewRow();
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
                    Manufacturer.Rows.Add(row);
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
                            bulkCopy1.DestinationTableName = "dbo.Manufacturer";
                            // Write from the source to the customer.
                            bulkCopy1.WriteToServer(Manufacturer);
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
                dt.Columns.AddRange(new[] {
                        new DataColumn("Name", typeof(string)),
                        new DataColumn("Error)", typeof(string))
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
                        new DataColumn("Name", typeof(string)),
                        new DataColumn("Error)", typeof(string))
                    });
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                    var manufacturerListDb = _manufacturerRepository.GetAll(x => !x.Archive).ToList();
                    var manufacturerList = new List<Manufacturer>();
                    var manufacturerAutoId = Convert.ToInt32(GetAutoNumber());
                    var manufacturerAutoSequence = GetAutoSequence();
                    foreach (DataRow row in dt.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["Name"]?.ToString().Trim()))
                        {
                            var manufactureName = row["Name"]?.ToString().Trim();
                            if (!manufacturerListDb.Any(x => x.Name == manufactureName))
                            {
                                var manufacturer = new Manufacturer
                                {
                                    Id = manufacturerAutoId.ToString(),
                                    Sequence = manufacturerAutoSequence,
                                    Code = manufacturerAutoId.ToString(),
                                    Name = manufactureName,
                                    Active = true,
                                    SynchronizationType = SynchronizationType.Server.ToString(),
                                    AddedBy = identity.Name,
                                    AddedDate = DateTime.Now,
                                    AddedFromIp = identity.IpAddress
                                };
                                manufacturerList.Add(manufacturer);
                                manufacturerAutoId++;
                                manufacturerAutoSequence++;
                            }
                        }
                        else
                        {
                            row[0] = "Manufacturer is null!";
                            result.Rows.Add(row.ItemArray);
                        }
                    }
                    BulkUpload(manufacturerList);
                    return result;
                }
                throw new Exception("File is not found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(Manufacturer manufacturer)
        {
            try
            {
                Check(manufacturer);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                manufacturer.Id = GetAutoNumber();
                manufacturer.Sequence = GetAutoSequence();
                manufacturer.Active = true;
                manufacturer.SynchronizationType = SynchronizationType.Server.ToString();
                manufacturer.AddedBy = identity.Name;
                manufacturer.AddedDate = DateTime.Now;
                manufacturer.AddedFromIp = identity.IpAddress;
                _manufacturerRepository.Add(manufacturer);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(Manufacturer manufacturer)
        {
            try
            {
                Check(manufacturer);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _manufacturerRepository.GetOne(manufacturer.Id);
                manufacturer.Sequence = dbdata.Sequence;
                manufacturer.SynchronizationType = dbdata.SynchronizationType;
                manufacturer.AddedBy = dbdata.AddedBy;
                manufacturer.AddedDate = dbdata.AddedDate;
                manufacturer.AddedFromIp = dbdata.AddedFromIp;
                manufacturer.UpdatedBy = identity.Name;
                manufacturer.UpdatedDate = DateTime.Now;
                manufacturer.UpdatedFromIp = identity.IpAddress;
                _manufacturerRepository.Update(manufacturer);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Manufacturer GetById(string id)
        {
            try
            {
                return _manufacturerRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Manufacturer> GetAll()
        {
            try
            {
                return _manufacturerRepository.GetAll(r => !r.Archive).OrderBy(r => r.Sequence).AsEnumerable();
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
                return from r in _manufacturerRepository.GetAll(r => !r.Archive && r.Active)
                                            .OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
