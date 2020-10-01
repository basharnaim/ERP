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
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class ROMService : Service<ROM>, IROMService
    {
        #region Ctor
        private readonly IRepository<ROM> _romRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ROMService(
            IRepository<ROM> romRepository,
            IUnitOfWork unitOfWork) : base(romRepository)
        {
            _romRepository = romRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion
        private void Check(ROM model)
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

        public void Add(ROM ROM)
        {
            try
            {
                Check(ROM);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                ROM.Id = GetAutoNumber();
                ROM.Sequence = GetAutoSequence();
                ROM.SynchronizationType = SynchronizationType.Server.ToString();
                ROM.AddedBy = identity.Name;
                ROM.AddedDate = DateTime.Now;
                ROM.AddedFromIp = identity.IpAddress;
                _romRepository.Add(ROM);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(ROM ROM)
        {
            try
            {
                Check(ROM);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _romRepository.GetOne(ROM.Id);
                ROM.Sequence = dbdata.Sequence;
                ROM.SynchronizationType = dbdata.SynchronizationType;
                ROM.AddedBy = dbdata.AddedBy;
                ROM.AddedDate = dbdata.AddedDate;
                ROM.AddedFromIp = dbdata.AddedFromIp;
                ROM.UpdatedBy = identity.Name;
                ROM.UpdatedDate = DateTime.Now;
                ROM.UpdatedFromIp = identity.IpAddress;
                _romRepository.Update(ROM);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ROM GetById(string id)
        {
            try
            {
                return _romRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ROM> GetAll()
        {
            try
            {
                return _romRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<object> Lists()
        {
            try
            {
                return from r in _romRepository.GetAll(r => !r.Archive && r.Active)
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
                return GetAutoSequence("ROM");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
