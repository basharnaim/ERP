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
    public class FlavorService : Service<Flavor>, IFlavorService
    {
        #region Ctor
        private readonly IRepository<Flavor> _flavorRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FlavorService(
            IRepository<Flavor> flavorRepository,
            IUnitOfWork unitOfWork) : base(flavorRepository)
        {
            _flavorRepository = flavorRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Flavor entity)
        {
            CheckUniqueColumn("Name", entity.Name, x => x.Id != entity.Id && x.Name == entity.Name);
        }

        /// <summary>
        /// Adds the specified colorvm.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <param name="flavor">The colorvm.</param>
        public void Add(Flavor flavor)
        {
            try
            {
                Check(flavor);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                flavor.Id = GetAutoNumber();
                flavor.Sequence = GetAutoSequence();
                flavor.SynchronizationType = SynchronizationType.Server.ToString();
                flavor.AddedBy = identity.Name;
                flavor.AddedDate = DateTime.Now;
                flavor.AddedFromIp = identity.IpAddress;
                _flavorRepository.Add(flavor);
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
        /// <param name="flavor">The colorvm.</param>
        public void Update(Flavor flavor)
        {
            try
            {
                Check(flavor);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var flavorDb = _flavorRepository.GetOne(flavor.Id);
                flavor.Sequence = flavorDb.Sequence;
                flavor.SynchronizationType = flavorDb.SynchronizationType;
                flavor.AddedBy = flavorDb.AddedBy;
                flavor.AddedDate = flavorDb.AddedDate;
                flavor.AddedFromIp = flavorDb.AddedFromIp;
                flavor.UpdatedBy = identity.Name;
                flavor.UpdatedDate = DateTime.Now;
                flavor.UpdatedFromIp = identity.IpAddress;
                _flavorRepository.Update(flavor);
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
        public Flavor GetById(string id)
        {
            try
            {
                return _flavorRepository.GetOne(id);
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
        public IEnumerable<Flavor> GetAll()
        {
            try
            {
                return _flavorRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PlantId"></param>
        /// <returns></returns>
        public IEnumerable<object> Lists()
        {
            try
            {
                return from r in _flavorRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence).ToList() select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
