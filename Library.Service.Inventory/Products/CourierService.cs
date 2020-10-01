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
    /// Class CourierService.
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class CourierService : Service<Courier>, ICourierService
    {
        #region Ctor
        private readonly IRepository<Courier> _courierRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CourierService(
            IRepository<Courier> courierRepository,
            IUnitOfWork unitOfWork) : base(courierRepository)
        {
            _courierRepository = courierRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion
        private void Check(Courier model)
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

        public void Add(Courier Courier)
        {
            try
            {
                Check(Courier);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                Courier.Id = GetAutoNumber();
                Courier.Sequence = GetAutoSequence();
                Courier.SynchronizationType = SynchronizationType.Server.ToString();
                Courier.AddedBy = identity.Name;
                Courier.AddedDate = DateTime.Now;
                Courier.AddedFromIp = identity.IpAddress;
                _courierRepository.Add(Courier);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(Courier Courier)
        {
            try
            {
                Check(Courier);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _courierRepository.GetOne(Courier.Id);
                Courier.Sequence = dbdata.Sequence;
                Courier.SynchronizationType = dbdata.SynchronizationType;
                Courier.AddedBy = dbdata.AddedBy;
                Courier.AddedDate = dbdata.AddedDate;
                Courier.AddedFromIp = dbdata.AddedFromIp;
                Courier.UpdatedBy = identity.Name;
                Courier.UpdatedDate = DateTime.Now;
                Courier.UpdatedFromIp = identity.IpAddress;
                _courierRepository.Update(Courier);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public Courier GetById(string id)
        {
            try
            {
                return _courierRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Courier> GetAll()
        {
            try
            {
                return _courierRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _courierRepository.GetAll(r => !r.Archive && r.Active)
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
                return GetAutoSequence("Courier");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
