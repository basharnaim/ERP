using Library.Context.UnitOfWorks;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using Library.Crosscutting.Securities;

namespace Library.Context.Core
{
    /// <summary>
    /// Repository.
    /// </summary>
    /// <remarks> Author: Jahangir Hossain Sheikh. </remarks>
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ErpdbEntities _context;
        private readonly DbSet<TEntity> _dbSet;

        public Repository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _context = _unitOfWork.Context;
            // Create objectset to work with.
            if (_unitOfWork != null && _context != null)
            {
                _dbSet = _context.Set<TEntity>();
            }
        }

        /// <summary>
        /// Add new record.
        /// </summary>
        /// <param name="entity"></param>
        public void Add(TEntity entity)
        {
            _dbSet.Add(entity);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entities"></param>
        public void Add(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().AddRange(entities);
        }

        public void AddOrUpdate(List<TEntity> entities)
        {
            entities.ForEach(r => _dbSet.AddOrUpdate(r));
        }

        public object GetEntityKeys(TEntity entity)
        {
            var type = typeof(TEntity);
            var set = ((IObjectContextAdapter)_context).ObjectContext.CreateObjectSet<TEntity>().EntitySet;
            var keys = set.ElementType.KeyMembers.Select(k => type.GetProperty(k.Name)).FirstOrDefault()?.GetValue(entity);
            return keys;
        }

        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity is null");
            }
            if (_dbSet.Local.Count > 0)
            {
                var key = GetEntityKeys(entity);
                var currentEntity = _context.Set<TEntity>().Find(key);
                _context.Entry(currentEntity).CurrentValues.SetValues(entity);
            }
            else
                _context.Entry(entity).State = EntityState.Modified;
        }

        public void Update(List<TEntity> entities)
        {
            entities.ForEach(r => _context.Entry(r).State = EntityState.Modified);
        }

        public void Delete(string id)
        {
            _dbSet.Remove(_dbSet.Find(id));
        }

        public void Delete(IEnumerable<TEntity> entities)
        {
            _context.Set<TEntity>().RemoveRange(entities);
        }

        public void InsertLog(TEntity entities)
        {
            var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
            var properties = TypeDescriptor.GetProperties(entities);
            properties.Find("AddedBy", true).SetValue(entities, identity.Name);
            properties.Find("AddedDate", true).SetValue(entities, DateTime.Now);
            properties.Find("AddedFromIp", true).SetValue(entities, identity.IpAddress);
        }

        public void InsertLog(TEntity entities, string userId, string ip)
        {
            var properties = TypeDescriptor.GetProperties(entities);
            properties.Find("AddedBy", true).SetValue(entities, userId);
            properties.Find("AddedDate", true).SetValue(entities, DateTime.Now);
            properties.Find("AddedFromIp", true).SetValue(entities, ip);
        }

        public void UpdateLog(TEntity entities)
        {
            var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
            var properties = TypeDescriptor.GetProperties(entities);
            properties.Find("UpdatedBy", true).SetValue(entities, identity.Name);
            properties.Find("UpdatedDate", true).SetValue(entities, DateTime.Now);
            properties.Find("UpdatedFromIp", true).SetValue(entities, identity.IpAddress);
        }

        public void UpdateLog(TEntity entities, string userId, string ip)
        {
            var properties = TypeDescriptor.GetProperties(entities);
            properties.Find("UpdatedBy", true).SetValue(entities, userId);
            properties.Find("UpdatedDate", true).SetValue(entities, DateTime.Now);
            properties.Find("UpdatedFromIp", true).SetValue(entities, ip);
        }

        public int Count()
        {
            return Count(x => true);
        }

        public int Count(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.Count(expression);
        }

        public TEntity GetOne(object id)
        {
            return _dbSet.Find(id);
        }

        public TEntity GetOne(Expression<Func<TEntity, bool>> expression)
        {
            return _context.Set<TEntity>().Where(expression).FirstOrDefault();
        }

        public TEntity GetOne(Expression<Func<TEntity, bool>> expression, string include = "")
        {
            IQueryable<TEntity> query = _context.Set<TEntity>().Where(expression);
            foreach (var includeProperty in include.Split
                (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return query.FirstOrDefault();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _dbSet.AsEnumerable();
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.Where(expression);
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression = null, string include = "")
        {
            IQueryable<TEntity> query = _dbSet.Where(expression);
            foreach (var includeProperty in include.Split
                (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            return query;
        }

        public IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> expression = null, string include = "", Expression<Func<TEntity, int>> orderBy = null, int? page = null, int? pageSize = null)
        {
            IQueryable<TEntity> query = _dbSet.Where(expression);
            foreach (var includeProperty in include.Split
                (new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            if (orderBy != null)
            {
                query = query.OrderBy(orderBy);
            }
            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }
            return query;
        }

        public bool Any(Expression<Func<TEntity, bool>> expression)
        {
            return _dbSet.Any(expression);
        }

        public string GetAutoNumber()
        {
            try
            {
                return _context.Database.SqlQuery<int>("SELECT MAX(CAST(Id AS INT)) + 1 FROM " + GetTableName()).FirstOrDefault().ToString();
            }
            catch (Exception)
            {
                return "1";
            }
        }

        public string GetAutoNumberWithCompanybranchId(string companyId, string branchId)
        {
            try
            {
                var stringPrefix = "";
                var branchIdWithPad = branchId.PadLeft(2, '0');
                stringPrefix = "" + companyId + "" + branchIdWithPad + "";
                var id = 0;
                var idList = _context.Database.SqlQuery<string>("select Id from" + GetTableName()).ToList();
                id = idList.Any() ? idList.Max(x => Convert.ToInt32(x.Substring(stringPrefix.Length)) + 1) : 1;
                var stringId = id.ToString();
                var idWithPad = stringId.PadLeft(5, '0');
                var masterIdWithCb = "" + stringPrefix + "" + idWithPad + "";
                return masterIdWithCb;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetAutoSequence()
        {
            try
            {
                decimal sequence = 0;
                var sequenceList = _context.Database.SqlQuery<decimal>("select Sequence from" + GetTableName()).ToList();
                sequence = sequenceList.Any() ? sequenceList.Max(x => x + 1) : 1;
                return Math.Floor(sequence);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GetTableName()
        {
            ObjectContext objectContext = ((IObjectContextAdapter)_context).ObjectContext;
            string sql = objectContext.CreateObjectSet<TEntity>().ToTraceString();
            Regex regex = new Regex("FROM (?<table>.*) AS");
            Match match = regex.Match(sql);

            string table = match.Groups["table"].Value;
            return table;
        }
        public virtual IQueryable<TEntity> SqlQuery(string query, params object[] parameters)
        {
            return _dbSet.SqlQuery(query, parameters).AsQueryable();
        }

        public virtual IEnumerable<TElement> SqlQuery<TElement>(string sql, params object[] parameters)
        {
            return _context.Database.SqlQuery<TElement>(sql, parameters).AsQueryable();
        }
        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
