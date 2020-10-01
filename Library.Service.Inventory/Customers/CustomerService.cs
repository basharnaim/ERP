#region Using

using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Customers;
using Library.Service.Core.Core;
using Library.Context.Repositories;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Web.Configuration;

#endregion

namespace Library.Service.Inventory.Customers
{
    /// <summary>
    /// Class CustomerService.
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class CustomerService : Service<Customer>, ICustomerService
    {
        #region Ctor
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<CustomerCategory> _customerCategoryRepository;
        private readonly IRepository<CustomerLedger> _customerLedgerRepository;
        private readonly IRawSqlService _rawSqlService;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerService(
            IRepository<Customer> customerRepository,
            IRepository<CustomerCategory> customerCategoryRepository,
            IRepository<CustomerLedger> customerLedgerRepository,
            IRawSqlService rawSqlService,
            IUnitOfWork unitOfWork
            ) : base(customerRepository)
        {
            _customerRepository = customerRepository;
            _customerCategoryRepository = customerCategoryRepository;
            _customerLedgerRepository = customerLedgerRepository;
            _rawSqlService = rawSqlService;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Customer model)
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
        public void BulkUpload(List<Customer> customerList)
        {
            try
            {
                var connectionString = WebConfigurationManager.ConnectionStrings["ErpdbEntities"].ConnectionString;
                var customer = new DataTable();
                customer.Columns.Add("Id", typeof(string));
                customer.Columns.Add("Sequence", typeof(decimal));
                customer.Columns.Add("Code", typeof(string));
                customer.Columns.Add("Name", typeof(string));
                customer.Columns.Add("DiscountRate", typeof(decimal));
                customer.Columns.Add("AccountsPayable", typeof(decimal));
                customer.Columns.Add("AccountsReceivable", typeof(decimal));
                customer.Columns.Add("CompanyId", typeof(int));
                customer.Columns.Add("BranchId", typeof(int));
                customer.Columns.Add("CustomerCategoryId", typeof(string));
                customer.Columns.Add("CustomerType", typeof(string));
                customer.Columns.Add("AccountCode", typeof(string));
                customer.Columns.Add("Address1", typeof(string));
                customer.Columns.Add("Address2", typeof(string));
                customer.Columns.Add("CountryId", typeof(string));
                customer.Columns.Add("DivisionId", typeof(string));
                customer.Columns.Add("DistrictId", typeof(string));
                customer.Columns.Add("ContactPerson", typeof(string));
                customer.Columns.Add("ContactPersonDesignation", typeof(string));
                customer.Columns.Add("Phone1", typeof(string));
                customer.Columns.Add("Phone2", typeof(string));
                customer.Columns.Add("Fax", typeof(string));
                customer.Columns.Add("Email", typeof(string));
                customer.Columns.Add("Website", typeof(string));
                customer.Columns.Add("IsActive", typeof(bool));
                customer.Columns.Add("IsArchive", typeof(bool));
                customer.Columns.Add("IsSynchronized", typeof(bool));
                customer.Columns.Add("IsUpdated", typeof(bool));
                customer.Columns.Add("SynchronizationType", typeof(string));
                customer.Columns.Add("CreatedBy", typeof(string));
                customer.Columns.Add("CreatedDate", typeof(DateTime));
                customer.Columns.Add("CreatedIP", typeof(string));
                customer.Columns.Add("ModifiedBy", typeof(string));
                customer.Columns.Add("ModifiedDate", typeof(DateTime));
                customer.Columns.Add("ModifiedIP", typeof(string));
                customer.Columns.Add("Comments", typeof(string));
                foreach (var entity in customerList)
                {
                    var row = customer.NewRow();
                    row["Id"] = entity.Id;
                    row["Sequence"] = entity.Sequence;
                    row["Code"] = entity.Code ?? (object)DBNull.Value;
                    row["Name"] = entity.Name;
                    row["DiscountRate"] = entity.DiscountRate;
                    row["AccountsPayable"] = entity.AccountsPayable;
                    row["AccountsReceivable"] = entity.AccountsReceivable;
                    row["CompanyId"] = entity.CompanyId;
                    row["BranchId"] = entity.BranchId;
                    row["CustomerCategoryId"] = entity.CustomerCategoryId ?? (object)DBNull.Value;
                    row["AccountCode"] = entity.AccountCode ?? (object)DBNull.Value;
                    row["Address1"] = entity.Address1;
                    row["Address2"] = entity.Address2 ?? (object)DBNull.Value;
                    row["CountryId"] = entity.CountryId ?? (object)DBNull.Value;
                    row["DivisionId"] = entity.DivisionId ?? (object)DBNull.Value;
                    row["DistrictId"] = entity.DistrictId ?? (object)DBNull.Value;
                    row["ContactPerson"] = entity.ContactPerson ?? (object)DBNull.Value;
                    row["ContactPersonDesignation"] = entity.ContactPersonDesignation ?? (object)DBNull.Value;
                    row["Phone1"] = entity.Phone1;
                    row["Phone2"] = entity.Phone2 ?? (object)DBNull.Value;
                    row["Email"] = entity.Email ?? (object)DBNull.Value;
                    row["Website"] = entity.Website ?? (object)DBNull.Value;
                    row["IsActive"] = entity.Active;
                    row["IsArchive"] = entity.Archive;
                    row["IsSynchronized"] = entity.IsSynchronized;
                    row["IsUpdated"] = entity.IsUpdated;
                    row["SynchronizationType"] = entity.SynchronizationType;
                    row["CreatedBy"] = entity.AddedBy;
                    row["CreatedDate"] = entity.AddedDate;
                    row["CreatedIP"] = entity.AddedFromIp;
                    row["ModifiedBy"] = entity.UpdatedBy ?? (object)DBNull.Value;
                    row["ModifiedDate"] = entity.UpdatedDate ?? (object)DBNull.Value;
                    row["ModifiedIP"] = entity.UpdatedFromIp ?? (object)DBNull.Value;
                    row["Comments"] = entity.Description ?? (object)DBNull.Value;
                    customer.Rows.Add(row);
                }
                using (var sourceConnection = new SqlConnection(connectionString))
                {
                    sourceConnection.Open();
                    using (var tr = sourceConnection.BeginTransaction())
                    {
                        using (var bulkCopy1 = new SqlBulkCopy(sourceConnection, SqlBulkCopyOptions.Default, tr))
                        {
                            bulkCopy1.DestinationTableName = "dbo.Customer";
                            // Write from the source to the customer.
                            bulkCopy1.WriteToServer(customer);
                        }
                        tr.Commit();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override string GenerateAutoId(string companyId, string branchId, string tableName)
        {
            try
            {
                return base.GenerateAutoId(companyId, branchId, tableName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetCustomerDiscountRate(string customerId)
        {
            decimal cstmrdsctrt = 0;
            var customer = _customerRepository.GetOne(customerId);
            if (customer != null)
            {
                var checkRate = customer.DiscountRate;
                if (checkRate > 0)
                {
                    cstmrdsctrt = checkRate;
                }
                else
                {
                    var cstmrctg = _customerCategoryRepository.GetOne(x => x.Id == customer.CustomerCategoryId);
                    if (cstmrctg != null)
                    {
                        checkRate = cstmrctg.DiscountRate;
                        if (checkRate > 0)
                        {
                            cstmrdsctrt = checkRate;
                        }
                    }
                }
            }
            return cstmrdsctrt;
        }

        public void Add(Customer customer)
        {
            var flag = false;
            try
            {
                Check(customer);
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                #region Customer
                if (string.IsNullOrEmpty(customer.CompanyId))
                {
                    customer.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(customer.BranchId))
                {
                    customer.BranchId = identity.BranchId;
                }
                customer.Id = GenerateAutoId(customer.CompanyId, customer.BranchId, "Customer");
                customer.Sequence = GetAutoSequence("Customer");
                customer.SynchronizationType = SynchronizationType.Server.ToString();
                customer.AddedBy = identity.Name;
                customer.AddedDate = DateTime.Now;
                customer.AddedFromIp = identity.IpAddress;
                _customerRepository.Add(customer);
                #endregion

                #region CustomerLedger
                if (customer.AccountsPayable > 0 || customer.AccountsReceivable > 0)
                {
                    CustomerLedger customerLedger = new CustomerLedger
                    {
                        Id = GenerateAutoId(customer.CompanyId, customer.BranchId, "CustomerLedger"),
                        Sequence = GetAutoSequence("CustomerLedger"),
                        TrackingNo = GenerateTrackingNo(customer.CompanyId, customer.BranchId, "CustomerLedger"),
                        MoneyReceiveNo = GenerateMoneyReceiveNo(customer.CompanyId, customer.BranchId, "CustomerLedger"),
                        TransactionDate = DateTime.Now,
                        CustomerId = customer.Id,
                        CustomerMobileNumber = customer.Phone1
                    };
                    customerLedger.TransactionType = TransactionType.OpeningBalance.ToString();
                    customerLedger.TransactionDate = DateTime.Now;
                    if (customer.AccountsPayable > 0)
                    {
                        customerLedger.Particulars = "Accounts Payable";
                        customerLedger.DebitAmount = customer.AccountsPayable;
                        customerLedger.CreditAmount = 0;
                    }
                    else if (customer.AccountsReceivable > 0)
                    {
                        customerLedger.Particulars = "Accounts Receivable";
                        customerLedger.DebitAmount = 0;
                        customerLedger.CreditAmount = customer.AccountsReceivable;
                    }
                    customerLedger.Active = true;
                    customerLedger.SynchronizationType = SynchronizationType.Server.ToString();
                    customerLedger.AddedBy = identity.Name;
                    customerLedger.AddedDate = DateTime.Now;
                    customerLedger.AddedFromIp = identity.IpAddress;
                    _customerLedgerRepository.Add(customerLedger);
                }

                #endregion
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateCustomerLedgerRunningBalance(customer.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        public void Update(Customer customer)
        {
            var flag = false;
            try
            {
                Check(customer);
                _unitOfWork.BeginTransaction();
                flag = true;
                #region Customer
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var customerDb = _customerRepository.GetOne(customer.Id);
                if (string.IsNullOrEmpty(customer.CompanyId))
                {
                    customer.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(customer.BranchId))
                {
                    customer.BranchId = identity.BranchId;
                }
                customer.SynchronizationType = customerDb.SynchronizationType;
                customer.AddedBy = customerDb.AddedBy;
                customer.AddedDate = customerDb.AddedDate;
                customer.AddedFromIp = customerDb.AddedFromIp;
                customer.UpdatedBy = identity.Name;
                customer.UpdatedDate = DateTime.Now;
                customer.UpdatedFromIp = identity.IpAddress;
                _customerRepository.Update(customer);
                #endregion

                #region Customer Ledger


                if (customer.AccountsPayable > 0 || customer.AccountsReceivable > 0)
                {
                    CustomerLedger customerLedgerDb = _customerLedgerRepository.GetOne(x => x.CustomerId == customerDb.Id && x.TransactionType == TransactionType.OpeningBalance.ToString());
                    if (customerLedgerDb != null)
                    {
                        if (!string.IsNullOrEmpty(customerLedgerDb.CompanyId))
                        {
                            customerLedgerDb.CompanyId = identity.CompanyId;
                        }
                        if (!string.IsNullOrEmpty(customerLedgerDb.BranchId))
                        {
                            customerLedgerDb.BranchId = identity.BranchId;
                        }
                        customerLedgerDb.TransactionType = TransactionType.OpeningBalance.ToString();
                        if (customer.AccountsPayable > 0)
                        {
                            customerLedgerDb.Particulars = "Accounts Payable";
                            customerLedgerDb.DebitAmount = customer.AccountsPayable;
                            customerLedgerDb.CreditAmount = 0;
                        }
                        else if (customer.AccountsReceivable > 0)
                        {
                            customerLedgerDb.Particulars = "Accounts Receivable";
                            customerLedgerDb.DebitAmount = 0;
                            customerLedgerDb.CreditAmount = customer.AccountsReceivable;
                        }
                        customerLedgerDb.UpdatedBy = identity.Name;
                        customerLedgerDb.UpdatedDate = DateTime.Now;
                        customerLedgerDb.UpdatedFromIp = identity.IpAddress;
                        _customerLedgerRepository.Update(customerLedgerDb);
                    }
                    else
                    {
                        if (customer.AccountsPayable > 0 || customer.AccountsReceivable > 0)
                        {
                            CustomerLedger customerLedger = new CustomerLedger
                            {
                                Id = GenerateAutoId(customer.CompanyId, customer.BranchId, "CustomerLedger"),
                                Sequence = GetAutoSequence("CustomerLedger"),
                                TrackingNo = GenerateTrackingNo(customer.CompanyId, customer.BranchId, "CustomerLedger"),
                                MoneyReceiveNo = GenerateMoneyReceiveNo(customer.CompanyId, customer.BranchId, "CustomerLedger"),
                                TransactionDate = DateTime.Now,
                                CustomerId = customer.Id,
                                CustomerMobileNumber = customer.Phone1
                            };
                            customerLedger.TransactionType = TransactionType.OpeningBalance.ToString();
                            customerLedger.TransactionDate = DateTime.Now;
                            if (customer.AccountsPayable > 0)
                            {
                                customerLedger.Particulars = "Accounts Payable";
                                customerLedger.DebitAmount = customer.AccountsPayable;
                                customerLedger.CreditAmount = 0;
                            }
                            else if (customer.AccountsReceivable > 0)
                            {
                                customerLedger.Particulars = "Accounts Receivable";
                                customerLedger.DebitAmount = 0;
                                customerLedger.CreditAmount = customer.AccountsReceivable;
                            }
                            customerLedger.Active = true;
                            customerLedger.SynchronizationType = SynchronizationType.Server.ToString();
                            customerLedger.AddedBy = identity.Name;
                            customerLedger.AddedDate = DateTime.Now;
                            customerLedger.AddedFromIp = identity.IpAddress;
                            _customerLedgerRepository.Add(customerLedger);
                        }
                    }
                }
                #endregion

                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateCustomerLedgerRunningBalance(customer?.Id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void Archive(string id)
        {
            try
            {
                var dbdata = _customerRepository.GetOne(id);
                dbdata.Archive = true;
                dbdata.Active = false;
                _customerRepository.Update(dbdata);
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
        public Customer GetById(string id)
        {
            try
            {
                return _customerRepository.GetOne(id);
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
        public IEnumerable<Customer> GetAll()
        {
            try
            {
                return _customerRepository.GetAll(r => !r.Archive, "Company,Branch").OrderByDescending(r => r.Sequence).AsEnumerable();
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
        public IEnumerable<Customer> GetAll(string companyId)
        {
            try
            {
                return _customerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId, "Company,Branch").OrderByDescending(r => r.Sequence).AsEnumerable();
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
        public IEnumerable<Customer> GetAll(string companyId, string branchId)
        {
            try
            {
                return _customerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId, "Company,Branch").OrderByDescending(r => r.Sequence).AsEnumerable();
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
        /// <param name="customerCategoryId"></param>
        /// <returns></returns>
        public IEnumerable<Customer> GetAll(string companyId, string branchId, string customerCategoryId)
        {
            try
            {
                return _customerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.CustomerCategoryId == customerCategoryId, "CustomerCategory,Company,Branch").OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _customerRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence)
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
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<object> Lists(string branchId)
        {
            try
            {
                return from r in _customerRepository.GetAll(r => !r.Archive && r.Active && r.BranchId == branchId).OrderBy(r => r.Sequence)
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
                return from r in _customerRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId && r.BranchId == branchId).OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// /
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public List<string> GetCustomerListForAutoComplete(string term)
        {
            try
            {
                var customer = from cus in _customerRepository.GetAll()
                               select new
                               {
                                   Id = cus.Id,
                                   Name = cus.Name,
                                   Mobile = cus.Phone1
                               };
                var result = customer.Where(x => x.Mobile.Contains(term)).Select(y => y.Name).ToList();
                return result;
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
        public IEnumerable<object> GetAllCustomerByCompanyBranchId(string companyId, string branchId)
        {
            var itemList = (from r in _customerRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId && r.BranchId == branchId).OrderBy(r => r.Sequence)
                            select new
                            {
                                value = r.Id,
                                label = r.Name
                            }).ToList<object>();
            return itemList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerMobile"></param>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public Customer GetCustomerByMobileNumberWithCompanyBranchId(string customerMobile, string companyId, string branchId)
        {
            try
            {
                return _customerRepository.GetOne(x => x.Phone1 == customerMobile.Trim() && x.CompanyId == companyId && x.BranchId == branchId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Customer> GetAllByMobile(string customerMobile, string companyId, string branchId)
        {
            try
            {
                return _customerRepository.GetAll(x => x.Phone1 == customerMobile.Trim() && x.CompanyId == companyId && x.BranchId == branchId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public Customer GetCustomerByIdWithCompanyBranchId(string customerId, string companyId, string branchId)
        {
            try
            {
                return _customerRepository.GetOne(x => x.Id == customerId && x.CompanyId == companyId && x.BranchId == branchId);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Customer> GetAllById(string customerId, string companyId, string branchId)
        {
            try
            {
                return _customerRepository.GetAll(x => x.Id == customerId && x.CompanyId == companyId && x.BranchId == branchId).AsEnumerable();
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
        public string GetCustomerNameById(string id)
        {
            try
            {
                return _customerRepository.GetOne(x => x.Id == id).Name;
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
                return GetAutoSequence("Customer");
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
        public ICollection<object> GetCustomerList(string companyId, string branchId)
        {
            var customerList = (from r in _customerRepository.GetAll().OrderByDescending(x => x.Name)
                                select new
                                {
                                    value = r.Id,
                                    label = r.Name
                                }).ToList<object>();
            return customerList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> GetAllCustomers()
        {
            var customerList = (from r in _customerRepository.GetAll(x => !x.Archive && x.Active).OrderByDescending(x => x.Name)
                                select new
                                {
                                    value = r.Id,
                                    label = r.Name
                                }).ToList<object>();
            return customerList;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public decimal GetCustomerDiscountRateByCustomerId(string customerId)
        {
            try
            {
                return _customerRepository.GetOne(x => x.Id == customerId).DiscountRate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Customer> GetAllCustomerForMapping(string customerCategoryId, string unitId, string regionId, string areaId, string territoryId, string customerId)
        {
            try
            {
                return (from customer in _customerRepository.GetAll(x => !x.Archive, "CustomerCategory")
                                    where
                                    (string.IsNullOrEmpty(customerCategoryId) || customer.CustomerCategoryId == customerCategoryId) &&
                                    (string.IsNullOrEmpty(unitId) || customer.CountryId == unitId) &&
                                    (string.IsNullOrEmpty(regionId) || customer.DivisionId == regionId) &&
                                    (string.IsNullOrEmpty(areaId) || customer.DistrictId == areaId) &&
                                    (string.IsNullOrEmpty(territoryId) || customer.DistrictId == territoryId) &&
                                    (string.IsNullOrEmpty(customerId) || customer.Id == customerId)
                                    orderby customer.Sequence
                                    select customer
                                    ).ToList();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
