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
    public class UomService:Service<Uom>, IUomService
    {
        #region Ctor
        private readonly IRepository<Uom> _uomRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UomService(
            IRepository<Uom> uomRepository,
            IUnitOfWork unitOfWork):base(uomRepository)
        {
            _uomRepository = uomRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(Uom model)
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

        #region Services
        public void Add(Uom uom)
        {
            try
            {
                Check(uom);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                uom.Id = GetAutoNumber();
                uom.Sequence = GetAutoSequence();
                uom.Sequence = GetAutoSequence();
                uom.SynchronizationType = SynchronizationType.Server.ToString();
                uom.AddedBy = identity.Name;
                uom.AddedDate = DateTime.Now;
                uom.AddedFromIp = identity.IpAddress;
                _uomRepository.Add(uom);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public void Update(Uom uom)
        {
            try
            {
                Check(uom);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _uomRepository.GetOne(uom.Id);
                uom.Sequence = dbdata.Sequence;
                uom.SynchronizationType = dbdata.SynchronizationType;
                uom.AddedBy = dbdata.AddedBy;
                uom.AddedDate = dbdata.AddedDate;
                uom.AddedFromIp = dbdata.AddedFromIp;
                uom.UpdatedBy = identity.Name;
                uom.UpdatedDate = DateTime.Now;
                uom.UpdatedFromIp = identity.IpAddress;
                _uomRepository.Update(uom);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public void Delete(string id)
        {
            try
            {
                _uomRepository.Delete(id);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
        public Uom GetById(string id)
        {
            try
            {
                return _uomRepository.GetOne(x => x.Id == id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<Uom> GetAll(string name, string code)
        {
            try
            {
                return _uomRepository.GetAll(x => x.Name.Contains(name) && x.Code.Contains(code));
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<Uom> GetAll()
        {
            try
            {
                return _uomRepository.GetAll();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<Uom> GetAll(string name)
        {
            try
            {
                return _uomRepository.GetAll(x => x.Name.Contains(name));
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

                return from r in _uomRepository.GetAll(x => x.Active.Equals(true)).OrderBy(r => r.Name)
                         select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion
    }
}