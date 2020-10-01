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
    public class ColorService : Service<Color>, IColorService
    {
        #region Ctor
        private readonly IRepository<Color> _colorRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ColorService(
            IRepository<Color> colorRepository,
            IUnitOfWork unitOfWork) : base(colorRepository)
        {
            _colorRepository = colorRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Color entity)
        {
            CheckUniqueColumn("Name", entity.Name, x => x.Id != entity.Id && x.Name == entity.Name);
        }

        /// <summary>
        /// Adds the specified colorvm.
        /// </summary>
        /// <remarks>Jahangir Sheikh, 8-11-2015</remarks>
        /// <param name="color">The colorvm.</param>
        public void Add(Color color)
        {
            try
            {
                Check(color);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                color.Id = GetAutoNumber();
                color.Sequence = GetAutoSequence();
                color.SynchronizationType = SynchronizationType.Server.ToString();
                color.AddedBy = identity.Name;
                color.AddedDate = DateTime.Now;
                color.AddedFromIp = identity.IpAddress;
                _colorRepository.Add(color);
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
        /// <param name="color">The colorvm.</param>
        public void Update(Color color)
        {
            try
            {
                Check(color);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var colorDb = _colorRepository.GetOne(color.Id);
                color.Sequence = colorDb.Sequence;
                color.SynchronizationType = colorDb.SynchronizationType;
                color.AddedBy = colorDb.AddedBy;
                color.AddedDate = colorDb.AddedDate;
                color.AddedFromIp = colorDb.AddedFromIp;
                color.UpdatedBy = identity.Name;
                color.UpdatedDate = DateTime.Now;
                color.UpdatedFromIp = identity.IpAddress;
                _colorRepository.Update(color);
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
        public Color GetById(string id)
        {
            try
            {
                return _colorRepository.GetOne(id);
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
        public IEnumerable<Color> GetAll()
        {
            try
            {
                return _colorRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _colorRepository.GetAll(r => !r.Archive && r.Active)
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
