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
    public class RAMService : Service<RAM>, IRAMService
    {
        #region Ctor
        private readonly IRepository<RAM> _ramRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RAMService(
            IRepository<RAM> ramRepository,
            IUnitOfWork unitOfWork) : base(ramRepository)
        {
            _ramRepository = ramRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion
        private void Check(RAM model)
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

        public void Add(RAM ram)
        {
            try
            {
                Check(ram);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                ram.Id = GetAutoNumber();
                ram.Sequence = GetAutoSequence();
                ram.SynchronizationType = SynchronizationType.Server.ToString();
                ram.AddedBy = identity.Name;
                ram.AddedDate = DateTime.Now;
                ram.AddedFromIp = identity.IpAddress;
                _ramRepository.Add(ram);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(RAM ram)
        {
            try
            {
                Check(ram);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _ramRepository.GetOne(ram.Id);
                ram.Sequence = dbdata.Sequence;
                ram.SynchronizationType = dbdata.SynchronizationType;
                ram.AddedBy = dbdata.AddedBy;
                ram.AddedDate = dbdata.AddedDate;
                ram.AddedFromIp = dbdata.AddedFromIp;
                ram.UpdatedBy = identity.Name;
                ram.UpdatedDate = DateTime.Now;
                ram.UpdatedFromIp = identity.IpAddress;
                _ramRepository.Update(ram);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public RAM GetById(string id)
        {
            try
            {
                return _ramRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<RAM> GetAll()
        {
            try
            {
                return _ramRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _ramRepository.GetAll(r => !r.Archive && r.Active)
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
                return GetAutoSequence("RAM");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
