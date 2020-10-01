using System.Collections.Generic;
using Library.Model.Inventory.Customers;

namespace Library.Service.Inventory.Customers
{
    /// <summary>
    /// Interface ICustomerService
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public interface ICustomerService
    {
        string GenerateAutoId(string companyId, string branchId, string tableName);
        decimal GetCustomerDiscountRate(string customerId);
        /// <summary>
        /// Adds the specified customer.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="customer">The customer.</param>
        void Add(Customer customer);

        /// <summary>
        /// Updates the specified customer.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="customer">The customer.</param>
        void Update(Customer customer);

        void BulkUpload(List<Customer> customerList);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        void Archive(string id);

        /// <summary>
        /// Getbies the identifier.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Customer.</returns>
        Customer GetById(string id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;Customer&gt;.</returns>
        IEnumerable<Customer> GetAll();

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<Customer> GetAll(string companyId);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<Customer> GetAll(string companyId, string branchId);

        IEnumerable<Customer> GetAllByMobile(string customerMobile, string companyId, string branchId);
        IEnumerable<Customer> GetAllById(string customerId, string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="customerCategoryId"></param>
        /// <returns></returns>
        IEnumerable<Customer> GetAll(string companyId, string branchId, string customerCategoryId);


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> Lists();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<object> Lists(string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<object> Lists(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        List<string> GetCustomerListForAutoComplete(string term);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerMobile"></param>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        Customer GetCustomerByMobileNumberWithCompanyBranchId(string customerMobile, string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        Customer GetCustomerByIdWithCompanyBranchId(string customerId, string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        string GetCustomerNameById(string id);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        int GetAutoSequence();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        ICollection<object> GetCustomerList(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        decimal GetCustomerDiscountRateByCustomerId(string customerId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<object> GetAllCustomerByCompanyBranchId(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> GetAllCustomers();

        IEnumerable<Customer> GetAllCustomerForMapping(string customerCategoryId, string unitId, string regionId,
            string areaId, string territoryId, string customerId);

    }
}
