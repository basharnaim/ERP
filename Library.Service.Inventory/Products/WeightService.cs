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
    /// Class ColorService.
    /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class WeightService : Service<Weight>, IWeightService
    {
        #region Ctor
        private readonly IRepository<Weight> _weightRepository;
        private readonly IUnitOfWork _unitOfWork;
        public WeightService(
            IRepository<Weight> weightRepository,
            IUnitOfWork unitOfWork) : base(weightRepository)
        {
            _weightRepository = weightRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Weight entity)
        {
            CheckUniqueColumn("Name", entity.Name, x => x.Id != entity.Id && x.Name == entity.Name);
        }

        /// <summary>
        /// Adds the specified colorvm.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <param name="weight">The colorvm.</param>
        public void Add(Weight weight)
        {
            try
            {
                Check(weight);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                weight.Id = GetAutoNumber();
                weight.Sequence = GetAutoSequence();
                weight.SynchronizationType = SynchronizationType.Server.ToString();
                weight.AddedBy = identity.Name;
                weight.AddedDate = DateTime.Now;
                weight.AddedFromIp = identity.IpAddress;
                _weightRepository.Add(weight);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Updates the specified colorvm.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <param name="weight">The colorvm.</param>
        public void Update(Weight weight)
        {
            try
            {
                Check(weight);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var weightDb = _weightRepository.GetOne(weight.Id);
                weight.Sequence = weightDb.Sequence;
                weight.SynchronizationType = weightDb.SynchronizationType;
                weight.AddedBy = weightDb.AddedBy;
                weight.AddedDate = weightDb.AddedDate;
                weight.AddedFromIp = weightDb.AddedFromIp;
                weight.UpdatedBy = identity.Name;
                weight.UpdatedDate = DateTime.Now;
                weight.UpdatedFromIp = identity.IpAddress;
                _weightRepository.Update(weight);
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
        /// <returns>Color.</returns>
        public Weight GetById(string id)
        {
            try
            {
                return _weightRepository.GetOne(id);
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
        /// <returns>IEnumerable&lt;Color&gt;.</returns>
        public IEnumerable<Weight> GetAll()
        {
            try
            {
                return _weightRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public decimal GetTotalWeightInTons(string weightId, int lineTotalPcses)
        {
            try
            {
                var weightDb = _weightRepository.GetOne(weightId);
                if (!string.IsNullOrEmpty(weightDb?.Id))
                {
                    var lineTotalWeightInGm = Convert.ToDecimal(lineTotalPcses * weightDb.WeightInGm);
                    return Convert.ToDecimal(lineTotalWeightInGm / 1000000);
                }

                return 0;
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
                return from r in _weightRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence).ToList() select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
