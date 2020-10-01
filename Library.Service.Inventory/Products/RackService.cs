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
    /// <remarks>Jahangir, 21-03-2016</remarks>
    /// </summary>
    public class RackService :Service<Rack>, IRackService
    {
        #region Ctor
        private readonly IRepository<Rack> _rackRepository;
        private readonly IUnitOfWork _unitOfWork;
        public RackService(
            IRepository<Rack> rackRepository,
            IUnitOfWork unitOfWork):base(rackRepository)
        {
            _rackRepository = rackRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Rack model)
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
        
        public void Add(Rack rack)
        {
            try
            {
                Check(rack);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                rack.Id = GetAutoNumber();
                rack.SynchronizationType = SynchronizationType.Server.ToString();
                rack.AddedBy = identity.Name;
                rack.AddedDate = DateTime.Now;
                rack.AddedFromIp = identity.IpAddress;
                _rackRepository.Add(rack);
                _unitOfWork.SaveChanges(); 
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public void Update(Rack rack)
        {
            try
            {
                Check(rack);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _rackRepository.GetOne(rack.Id);
                rack.SynchronizationType = dbdata.SynchronizationType;
                rack.AddedBy = dbdata.AddedBy;
                rack.AddedDate = dbdata.AddedDate;
                rack.AddedFromIp = dbdata.AddedFromIp;
                rack.UpdatedBy = identity.Name;
                rack.UpdatedDate = DateTime.Now;
                rack.UpdatedFromIp = identity.IpAddress;
                _rackRepository.Update(rack);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public Rack GetById(string id)
        {
            try
            {
                return _rackRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public IEnumerable<Rack> GetAll()
        {
            try
            {
                return _rackRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _rackRepository.GetAll(r => !r.Archive && r.Active)
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
                return GetAutoSequence("Rack");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
