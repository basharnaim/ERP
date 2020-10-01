using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Library.Context.Core
{
    /// <summary>
    /// Core repository class.
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <remarks> Author: Jahangir Hossain Sheikh. </remarks>
    public interface IRepository<T> : IDisposable
                where T : class
    {
        /// <summary>
        /// Insert function.
        /// </summary>
        /// <param name="entity">Entity</param>
        void Add(T entity);

        /// <summary>
        /// Insert list of data.
        /// </summary>
        /// <param name="entities">entities</param>
        void Add(IEnumerable<T> entities);

        /// <summary>
        /// Add or update record in same function.
        /// </summary>
        /// <param name="entities"></param>
        void AddOrUpdate(List<T> entities);

        /// <summary>
        /// Update enity record.
        /// </summary>
        /// <param name="entity"></param>
        void Update(T entity);

        /// <summary>
        /// Update list of record.
        /// </summary>
        /// <param name="entities"></param>
        void Update(List<T> entities);

        /// <summary>
        /// Delete record.
        /// </summary>
        /// <param name="id">string</param>
        void Delete(string id);

        /// <summary>
        /// Delete list of record.
        /// </summary>
        /// <param name="entities"></param>
        void Delete(IEnumerable<T> entities);

        /// <summary>
        /// Count total entity record.
        /// </summary>
        /// <returns>Integer</returns>
        int Count();

        /// <summary>
        /// Count total record of entity with condition.
        /// </summary>
        /// <param name="expression">Func</param>
        /// <returns>Integer</returns>
        int Count(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Get single record by primary key.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        T GetOne(object id);

        /// <summary>
        /// Get single record by condition.
        /// </summary>
        /// <param name="expression">Func</param>
        /// <returns>Entity</returns>
        T GetOne(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Get single record by condition and also include another entity record.
        /// </summary>
        /// <param name="expression">expression</param>
        /// <param name="include">string</param>
        /// <returns>Entity</returns>
        T GetOne(Expression<Func<T, bool>> expression, string include);

        /// <summary>
        /// Get all record of entity.
        /// </summary>
        /// <returns>Entity List</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        /// Get list of record by passing condition.
        /// </summary>
        /// <param name="expression">expression</param>
        /// <returns>Entity List</returns>
        IEnumerable<T> GetAll(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Get list of record by passing condition and include depencies.
        /// </summary>
        /// <param name="expression">expression</param>
        /// <param name="include">string</param>
        /// <returns>Entity List</returns>
        IEnumerable<T> GetAll(Expression<Func<T, bool>> expression, string include = "");

        /// <summary>
        /// Get list of record by passing condition.
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="include"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        IEnumerable<T> GetAll(Expression<Func<T, bool>> expression = null, string include = "", Expression<Func<T, int>> orderBy = null, int? page = null, int? pageSize = null);

        /// <summary>
        /// Check if exist any record in entity.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns>Boolean</returns>
        bool Any(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Get last primary key.
        /// </summary>
        /// <returns>string</returns>
        string GetAutoNumber();
        string GetAutoNumberWithCompanybranchId(string companyId, string branchId);
        decimal GetAutoSequence();
        IQueryable<T> SqlQuery(string query, params object[] parameters);

        IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters);
    }
}
