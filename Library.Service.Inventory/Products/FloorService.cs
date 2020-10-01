using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Model.Inventory.Products;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Library.Crosscutting.Securities;

namespace Library.Service.Inventory.Products
{
    /// <summary>
    /// Class ColorService.
    /// <remarks>Jahangir, 21-03-2016</remarks>
    /// </summary>
    public class FloorService :Service<Floor>, IFloorService
    {
        #region Ctor
        private readonly IRepository<Floor> _floorRepository;
        private readonly IUnitOfWork _unitOfWork;
        public FloorService(
            IRepository<Floor> floorRepository,
            IUnitOfWork unitOfWork):base(floorRepository)
        {
            _floorRepository = floorRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion
        private void Check(Floor model)
        {
            try
            {
                CheckUniqueColumn("Name", model.Name, r => r.Id != model.Id && r.Name == model.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="rack"></param>
        public void Add(Floor floor)
        {
            try
            {
                Check(floor);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                floor.Id = GetAutoNumber();
                floor.SynchronizationType = SynchronizationType.Server.ToString();
                floor.AddedBy = identity.Name;
                floor.AddedDate = DateTime.Now;
                floor.AddedFromIp = identity.IpAddress;
                _floorRepository.Add(floor);
                _unitOfWork.SaveChanges(); ;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="floor"></param>
        public void Update(Floor floor)
        {
            try
            {
                Check(floor);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _floorRepository.GetOne(floor.Id);
                floor.SynchronizationType = dbdata.SynchronizationType;
                floor.AddedBy = dbdata.AddedBy;
                floor.AddedDate = dbdata.AddedDate;
                floor.AddedFromIp = dbdata.AddedFromIp;
                floor.UpdatedBy = identity.Name;
                floor.UpdatedDate = DateTime.Now;
                floor.UpdatedFromIp = identity.IpAddress;
                _floorRepository.Update(floor);
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
        public Floor GetById(string id)
        {
            try
            {
                return _floorRepository.GetOne(id);
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
        public IEnumerable<Floor> GetAll()
        {
            try
            {
                return _floorRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _floorRepository.GetAll(r => !r.Archive && r.Active)
                                                 .OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
            }
            catch
            {
                return null;
            }
        }

        public override int GetAutoSequence()
        {
            try
            {
                return GetAutoSequence("Floor");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
