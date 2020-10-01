using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Products;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Class GradeService.
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class ProductMasterService : Service<ProductMaster>, IProductMasterService
    {
        #region Ctor
        private readonly IRepository<ProductMaster> _productMasterRepository;
        private readonly IRepository<Product> _productRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductMasterService(
            IRepository<ProductMaster> productMasteryRepository,
            IRepository<Product> productRepository,
            IUnitOfWork unitOfWork
            ) : base(productMasteryRepository)
        {
            _productMasterRepository = productMasteryRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void BulkUpload(List<ProductMaster> productMasterList)
        {
            try
            {
                string connectionString = ConfigurationManager.ConnectionStrings["ErpdbEntities"].ConnectionString;

                #region Product Master
                var product = new DataTable();
                product.Columns.Add("Id", typeof(string));
                product.Columns.Add("Sequence", typeof(int));
                product.Columns.Add("CompanyId", typeof(string));
                product.Columns.Add("BranchId", typeof(string));
                product.Columns.Add("ProductId", typeof(string));
                product.Columns.Add("ProductCategoryId", typeof(string));
                product.Columns.Add("Active", typeof(bool));
                product.Columns.Add("Archive", typeof(bool));
                product.Columns.Add("IsSynchronized", typeof(bool));
                product.Columns.Add("IsUpdated", typeof(bool));
                product.Columns.Add("SynchronizationType", typeof(string));
                product.Columns.Add("AddedBy", typeof(string));
                product.Columns.Add("AddedDate", typeof(DateTime));
                product.Columns.Add("AddedFromIp", typeof(string));
                product.Columns.Add("UpdatedBy", typeof(string));
                product.Columns.Add("UpdatedDate", typeof(DateTime));
                product.Columns.Add("UpdatedFromIp", typeof(string));
                foreach (var entity in productMasterList)
                {
                    var row = product.NewRow();
                    row["Id"] = entity.Id;
                    row["Sequence"] = entity.Sequence;
                    row["CompanyId"] = entity.CompanyId;
                    row["BranchId"] = entity.BranchId;
                    row["ProductId"] = entity.ProductId;
                    row["ProductCategoryId"] = entity.ProductCategoryId;
                    row["Active"] = entity.Active;
                    row["Archive"] = entity.Archive;
                    row["IsSynchronized"] = entity.IsSynchronized;
                    row["IsUpdated"] = entity.IsUpdated;
                    row["SynchronizationType"] = entity.SynchronizationType;
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
                            bulkCopy1.DestinationTableName = "dbo.ProductMaster";
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

        public void Add(string[] productIdList, string templateCompany, string templateBranch)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var productList = _productRepository.GetAll(x => !x.Archive).ToList();
                var id = Convert.ToInt32(GetAutoNumber());
                var sqnce = GetAutoSequence();
                List<ProductMaster> productMasterList = new List<ProductMaster>();
                foreach (var productId in productIdList)
                {
                    var itmmstr = new ProductMaster { Id = id.ToString() , Sequence = sqnce, ProductId = productId };
                    itmmstr.ProductCategoryId = productList.Find(x => x.Id == itmmstr.ProductId)?.ProductCategoryId;
                    itmmstr.CompanyId = templateCompany;
                    itmmstr.BranchId = templateBranch;
                    itmmstr.Active = true;
                    itmmstr.SynchronizationType = SynchronizationType.Server.ToString();
                    itmmstr.AddedBy = identity.Name;
                    itmmstr.AddedDate = DateTime.Now;
                    itmmstr.AddedFromIp = identity.IpAddress;
                    productMasterList.Add(itmmstr);
                    id++;
                    sqnce++;
                }
                BulkUpload(productMasterList);
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
        public ProductMaster GetById(string id)
        {
            try
            {
                return _productMasterRepository.GetOne(id);
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
        public IEnumerable<ProductMaster> GetAll()
        {
            try
            {
                return _productMasterRepository.GetAll(r => !r.Archive, "Product, ProductCategory").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public IEnumerable<ProductMaster> GetAll(string companyId, string branchId)
        {
            try
            {
                return _productMasterRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId, "Product, ProductCategory").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public IEnumerable<ProductMaster> GetAll(string companyId)
        {
            try
            {
                return _productMasterRepository.GetAll(r => !r.Archive && r.CompanyId == companyId, "Product, ProductCategory").AsEnumerable();
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
                IEnumerable<string> productMasterIds = _productMasterRepository.GetAll(r => !r.Archive && r.Active).GroupBy(x => x.ProductId).Select(r => r.FirstOrDefault().ProductId);
                return from r in _productRepository.GetAll(r => !r.Archive && r.Active && productMasterIds.Contains(r.Id))
                                            .OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };

            }
            catch
            {
                return null;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<object> Lists(string companyId, string branchId)
        {
            try
            {
                IEnumerable<string> productMasterIds = _productMasterRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId && r.BranchId == branchId).Select(r => r.ProductId);
                return from r in _productRepository.GetAll(r => !r.Archive && r.Active && productMasterIds.Contains(r.Id))
                                            .OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetGradeNameById(string id)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Product> GetAllProductsFromProductMasterByCompanyBranchId(string companyId, string branchId)
        {
            try
            {
                var expiryProductList = (from product in _productRepository.GetAll(x => !x.Archive)
                                         join productMaster in _productMasterRepository.GetAll(x => !x.Archive)
                                         on product.Id equals productMaster.ProductId
                                         where productMaster.CompanyId == companyId &&
                                         productMaster.BranchId == branchId
                                         select product).ToList();
                return expiryProductList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Product> GetAllProductsFromProductMaster()
        {
            try
            {
                DateTime validateDate = DateTime.Now.AddDays(30);
                var expiryProductList = (from product in _productRepository.GetAll(x => !x.Archive)
                                         join productMaster in _productMasterRepository.GetAll(x => !x.Archive)
                                         on product.Id equals productMaster.ProductId
                                         select product).ToList();
                return expiryProductList;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetProductMasterTemplate(string companyId, string branchId)
        {
            try
            {
                IEnumerable<string> productMasterIds = _productMasterRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId).Select(r => r.ProductId).ToList();
                return _productRepository.GetAll(r => !r.Archive && r.Active && !productMasterIds.Contains(r.Id), "ProductCategory, ProductSubCategory").OrderBy(r => r.Sequence);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="productCategoryId"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetProductMasterTemplate(string companyId, string branchId, string productCategoryId)
        {
            try
            {
                IEnumerable<string> productMasterIds = _productMasterRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId).Select(r => r.ProductId);
                if (!string.IsNullOrEmpty(productCategoryId))
                {
                    return _productRepository.GetAll(r => !r.Archive && r.Active && !productMasterIds.Contains(r.Id) && r.ProductCategoryId == productCategoryId, "ProductCategory, ProductSubCategory").OrderBy(r => r.Sequence);
                }
                return _productRepository.GetAll(r => !r.Archive && r.Active && !productMasterIds.Contains(r.Id), "ProductCategory, ProductSubCategory").OrderBy(r => r.Sequence);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Product> GetProductDialogProductList(string companyId, string branchId)
        {
            try
            {
                IEnumerable<string> productMasterIds = _productMasterRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId).Select(r => r.ProductId).ToList();
                return _productRepository.GetAll(r => !r.Archive && r.Active && productMasterIds.Contains(r.Id), "ProductCategory, ProductSubCategory").OrderBy(r => r.Sequence);
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
        public void ChangeStatus(string id)
        {
            try
            {
                var dbdata = _productMasterRepository.GetOne(id);
                dbdata.Active = !dbdata.Active;
                _productMasterRepository.Update(dbdata);
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
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<string> GetProductMasterIds(string companyId, string branchId)
        {
            return _productMasterRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId).Select(r => r.ProductId);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProductByMasterIds(string companyId, string branchId)
        {
            IEnumerable<string> productMasterIds = _productMasterRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId && r.BranchId == branchId).Select(r => r.ProductId);
            return _productRepository.GetAll(r => !r.Archive && r.Active && productMasterIds.Contains(r.Id)).OrderBy(r => r.Sequence).AsEnumerable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="productCategoryId"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProductByMasterIds(string companyId, string branchId, string productCategoryId)
        {
            IEnumerable<string> productMasterIds = _productMasterRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId && r.BranchId == branchId).Select(r => r.ProductId);
            return _productRepository.GetAll(r => !r.Archive && r.Active && productMasterIds.Contains(r.Id) && r.ProductCategoryId == productCategoryId).OrderBy(r => r.Sequence).AsEnumerable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="productCategoryId"></param>
        /// <param name="productSubCategoryId"></param>
        /// <returns></returns>
        public IEnumerable<Product> GetAllProductByMasterIds(string companyId, string branchId, string productCategoryId, string productSubCategoryId)
        {
            IEnumerable<string> productMasterIds = _productMasterRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId && r.BranchId == branchId).Select(r => r.ProductId);
            return _productRepository.GetAll(r => !r.Archive && r.Active && productMasterIds.Contains(r.Id) && r.ProductCategoryId == productCategoryId && r.ProductSubCategoryId == productSubCategoryId).OrderBy(r => r.Sequence).AsEnumerable();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<object> GetProductCategoryByCompanyBranch(string branchId)
        {
            return (from r in _productMasterRepository.GetAll(r => !r.Archive && r.Active && r.BranchId == branchId, "ProductCategory")
                    select new { Value = r.ProductCategoryId, Text = r.ProductCategory?.Name }).Distinct();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<object> GetProductByCompanyBranch(string branchId)
        {
            return (from r in _productMasterRepository.GetAll(r => !r.Archive && r.Active && r.BranchId == branchId, "Product")
                    select new { Value = r.ProductId, Text = r.Product?.Name }).Distinct();
        }

        public override int GetAutoSequence()
        {
            try
            {
                return GetAutoSequence("ProductMaster");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
