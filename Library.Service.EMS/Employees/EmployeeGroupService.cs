using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.EMS.Employees;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.EMS.Employees
{
    public class EmployeeGroupService : Service<EmployeeGroup>, IEmployeeGroupService
    {
        #region Ctor
        private readonly IRepository<EmployeeGroup> _employeeGroupRepository;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeGroupService(
            IRepository<EmployeeGroup> employeeGroupRepository,
            IUnitOfWork unitOfWork
            ) : base(employeeGroupRepository)
        {
            _employeeGroupRepository = employeeGroupRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Action
        private void Check(EmployeeGroup model)
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
        public void Add(EmployeeGroup employeeGroup)
        {
            try
            {
                Check(employeeGroup);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                employeeGroup.Id = GetAutoNumber();
                employeeGroup.Sequence = GetAutoSequence();
                employeeGroup.SynchronizationType = SynchronizationType.Server.ToString();
                employeeGroup.AddedBy = identity.Name;
                employeeGroup.AddedDate = DateTime.Now;
                employeeGroup.AddedFromIp = identity.IpAddress;
                _employeeGroupRepository.Add(employeeGroup);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Update(EmployeeGroup employeeGroup)
        {
            try
            {
                Check(employeeGroup);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var productCategoryDb = _employeeGroupRepository.GetOne(employeeGroup.Id);
                employeeGroup.Sequence = productCategoryDb.Sequence;
                employeeGroup.SynchronizationType = productCategoryDb.SynchronizationType;
                employeeGroup.AddedBy = productCategoryDb.AddedBy;
                employeeGroup.AddedDate = productCategoryDb.AddedDate;
                employeeGroup.AddedFromIp = productCategoryDb.AddedFromIp;
                employeeGroup.UpdatedBy = identity.Name;
                employeeGroup.UpdatedDate = DateTime.Now;
                employeeGroup.UpdatedFromIp = identity.IpAddress;
                _employeeGroupRepository.Update(employeeGroup);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public EmployeeGroup GetById(string id)
        {
            try
            {
                return _employeeGroupRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<EmployeeGroup> GetAll()
        {
            try
            {
                return _employeeGroupRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _employeeGroupRepository.GetAll(
                                                 r =>  !r.Archive && r.Active).OrderBy(r => r.Sequence)
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
