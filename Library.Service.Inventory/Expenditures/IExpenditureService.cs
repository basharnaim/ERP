using Library.Model.Inventory.Expenditures;
using System;
using System.Collections.Generic;

namespace Library.Service.Inventory.Expenditures
{
    /// <summary>
    /// Interface ISaleReturnService
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public interface IExpenditureService
    {
        string GenerateAutoId(string companyId, string branchId, string tableName);


        void Add(Expenditure expenditure);

        
        void Update(Expenditure expenditure);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>SaleReturn.</returns>
        Expenditure GetById(string id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;SaleReturn&gt;.</returns>
        IEnumerable<Expenditure> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Expenditure> GetAll(string companyId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <returns>IEnumerable&lt;UserGroup&gt;.</returns>
        IEnumerable<Expenditure> GetAll(string companyId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Expenditure> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="expenditureCategoryId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        IEnumerable<Expenditure> GetAll(string companyId, string branchId, string expenditureCategoryId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="expenditureCategoryId"></param>
        /// <returns></returns>
        IEnumerable<Expenditure> GetAll(string companyId, string branchId, string expenditureCategoryId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<Expenditure> GetAll(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<object> Lists(string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        IEnumerable<object> Lists();

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
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        decimal GetTotalExpensesBetweenDate(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);
    }
}
