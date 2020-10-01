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
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class StyleService : Service<Style>, IStyleService
    {
        #region Ctor
        private readonly IRepository<Style> _styleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public StyleService(
            IRepository<Style> styleRepository,
            IUnitOfWork unitOfWork) : base(styleRepository)
        {
            _styleRepository = styleRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion
        private void Check(Style model)
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

        private void BulkUpload(List<Style> styleList)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ErpdbEntities"].ConnectionString;

                #region style
                var style = new DataTable();
                style.Columns.Add("Id", typeof(string));
                style.Columns.Add("Sequence", typeof(int));
                style.Columns.Add("Code", typeof(string));
                style.Columns.Add("Name", typeof(string));
                style.Columns.Add("Active", typeof(bool));
                style.Columns.Add("Archive", typeof(bool));
                style.Columns.Add("IsSynchronized", typeof(bool));
                style.Columns.Add("IsUpdated", typeof(bool));
                style.Columns.Add("SynchronizationType", typeof(string));
                style.Columns.Add("Description", typeof(string));
                style.Columns.Add("AddedBy", typeof(string));
                style.Columns.Add("AddedDate", typeof(DateTime));
                style.Columns.Add("AddedFromIp", typeof(string));
                style.Columns.Add("UpdatedBy", typeof(string));
                style.Columns.Add("UpdatedDate", typeof(DateTime));
                style.Columns.Add("UpdatedFromIp", typeof(string));
                foreach (var entity in styleList)
                {
                    var row = style.NewRow();
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
                    style.Rows.Add(row);
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
                            bulkCopy1.DestinationTableName = "dbo.Style";
                            // Write from the source to the customer.
                            bulkCopy1.WriteToServer(style);
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
                    var styleListDb = _styleRepository.GetAll(x => !x.Archive).ToList();
                    var styleList = new List<Style>();
                    var styleAutoId = Convert.ToInt32(GetAutoNumber());
                    var styleAutoSequence = GetAutoSequence();
                    foreach (DataRow row in secondTable.Rows)
                    {
                        if (!string.IsNullOrEmpty(row["Name"]?.ToString().Trim()))
                        {
                            var name = row["Name"]?.ToString().Trim();
                            if (!styleListDb.Any(x => x.Name == name))
                            {
                                var style = new Style
                                {
                                    Id = styleAutoId.ToString(),
                                    Sequence = styleAutoSequence,
                                    Code = styleAutoId.ToString(),
                                    Name = name,
                                    Active = true,
                                    SynchronizationType = SynchronizationType.Server.ToString(),
                                    AddedBy = identity.Name,
                                    AddedDate = DateTime.Now,
                                    AddedFromIp = identity.IpAddress
                                };
                                styleList.Add(style);
                                styleAutoId++;
                                styleAutoSequence++;
                            }
                        }
                        else
                        {
                            row[0] = "Style is null!";
                            result.Rows.Add(row.ItemArray);
                        }
                    }
                    BulkUpload(styleList);
                    return result;
                }
                throw new Exception("File is not found");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(Style style)
        {
            try
            {
                Check(style);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                style.Id = GetAutoNumber();
                style.Sequence = GetAutoSequence();
                style.SynchronizationType = SynchronizationType.Server.ToString();
                style.AddedBy = identity.Name;
                style.AddedDate = DateTime.Now;
                style.AddedFromIp = identity.IpAddress;
                _styleRepository.Add(style);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(Style style)
        {
            try
            {
                Check(style);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _styleRepository.GetOne(style.Id);
                style.Sequence = dbdata.Sequence;
                style.SynchronizationType = dbdata.SynchronizationType;
                style.AddedBy = dbdata.AddedBy;
                style.AddedDate = dbdata.AddedDate;
                style.AddedFromIp = dbdata.AddedFromIp;
                style.UpdatedBy = identity.Name;
                style.UpdatedDate = DateTime.Now;
                style.UpdatedFromIp = identity.IpAddress;
                _styleRepository.Update(style);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Style GetById(string id)
        {
            try
            {
                return _styleRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Style> GetAll()
        {
            try
            {
                return _styleRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _styleRepository.GetAll(r => !r.Archive && r.Active)
                                                 .OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
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
                return GetAutoSequence("Style");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
