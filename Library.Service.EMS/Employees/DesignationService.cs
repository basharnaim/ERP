using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Core.Organizations;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.EMS.Employees
{
    public class DesignationService : Service<Designation>, IDesignationService
    {
        #region Ctor
        private readonly IRepository<Designation> _designationRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DesignationService(
            IRepository<Designation> designationRepository,
            IUnitOfWork unitOfWork
            ) : base(designationRepository)
        {
            _designationRepository = designationRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Action
        private void Check(Designation model)
        {
            try
            {
                CheckUniqueColumn("ShortName", model.ShortName, r => r.Id != model.Id && r.ShortName == model.ShortName);
                CheckUniqueColumn("Name", model.Name, r => r.Id != model.Id && r.Name == model.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Add(Designation designation)
        {
            try
            {
                Check(designation);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                designation.Id = GetAutoNumber();
                designation.Sequence = GetAutoSequence();
                designation.SynchronizationType = SynchronizationType.Server.ToString();
                designation.AddedBy = identity.Name;
                designation.AddedDate = DateTime.Now;
                designation.AddedFromIp = identity.IpAddress;
                _designationRepository.Add(designation);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Update(Designation designation)
        {
            try
            {
                Check(designation);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var designationDb = _designationRepository.GetOne(designation.Id);
                designation.Sequence = designationDb.Sequence;
                designation.SynchronizationType = designationDb.SynchronizationType;
                designation.AddedBy = designationDb.AddedBy;
                designation.AddedDate = designationDb.AddedDate;
                designation.AddedFromIp = designationDb.AddedFromIp;
                designation.UpdatedBy = identity.Name;
                designation.UpdatedDate = DateTime.Now;
                designation.UpdatedFromIp = identity.IpAddress;
                _designationRepository.Update(designation);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Designation GetById(string id)
        {
            try
            {
                return _designationRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<Designation> GetAll()
        {
            try
            {
                return _designationRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _designationRepository.GetAll(
                                                 r => r.Active && !r.Archive).OrderBy(r => r.Sequence)
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
