using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Products;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Class CurrencyService.
    /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class CurrencyService : Service<Currency>, ICurrencyService
    {
        #region Ctor
        private readonly IRepository<Currency> _currencyRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CurrencyService(
            IRepository<Currency> currencyRepository,
            IUnitOfWork unitOfWork) : base(currencyRepository)
        {
            _currencyRepository = currencyRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Currency entity)
        {
            CheckUniqueColumn("Name", entity.Name, x => x.Id != entity.Id && x.Name == entity.Name);
        }

        /// <summary>
        /// Adds the specified Currencyvm.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <param name="currency">The Currencyvm.</param>
        public void Add(Currency currency)
        {
            try
            {
                Check(currency);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                currency.Id = GetAutoNumber();
                currency.Sequence = GetAutoSequence();
                currency.SynchronizationType = SynchronizationType.Server.ToString();
                currency.AddedBy = identity.Name;
                currency.AddedDate = DateTime.Now;
                currency.AddedFromIp = identity.IpAddress;
                _currencyRepository.Add(currency);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the specified Currencyvm.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <param name="currency">The Currencyvm.</param>
        public void Update(Currency currency)
        {
            try
            {
                Check(currency);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var currencyDb = _currencyRepository.GetOne(currency.Id);
                currency.Sequence = currencyDb.Sequence;
                currency.SynchronizationType = currencyDb.SynchronizationType;
                currency.AddedBy = currencyDb.AddedBy;
                currency.AddedDate = currencyDb.AddedDate;
                currency.AddedFromIp = currencyDb.AddedFromIp;
                currency.UpdatedBy = identity.Name;
                currency.UpdatedDate = DateTime.Now;
                currency.UpdatedFromIp = identity.IpAddress;
                _currencyRepository.Update(currency);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>Currency.</returns>
        public Currency GetById(string id)
        {
            try
            {
                return _currencyRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <returns>IEnumerable&lt;Currency&gt;.</returns>
        public IEnumerable<Currency> GetAll()
        {
            try
            {
                return _currencyRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _currencyRepository.GetAll(r => !r.Archive && r.Active)
                                                 .OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
