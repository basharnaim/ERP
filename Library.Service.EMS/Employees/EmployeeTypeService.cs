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
    public class EmployeeTypeService : Service<EmployeeType>, IEmployeeTypeService
    {
        #region Ctor
        private readonly IRepository<EmployeeType> _employeeTypeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeTypeService(
            IRepository<EmployeeType> employeeTypeRepository,
            IUnitOfWork unitOfWork
            ) : base(employeeTypeRepository)
        {
            _employeeTypeRepository = employeeTypeRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Action
        private void Check(EmployeeType model)
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
        public void Add(EmployeeType employeeType)
        {
            try
            {
                Check(employeeType);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                employeeType.Id = GetAutoNumber();
                employeeType.Sequence = GetAutoSequence();
                employeeType.SynchronizationType = SynchronizationType.Server.ToString();
                employeeType.AddedBy = identity.Name;
                employeeType.AddedDate = DateTime.Now;
                employeeType.AddedFromIp = identity.IpAddress;
                _employeeTypeRepository.Add(employeeType);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Update(EmployeeType employeeType)
        {
            try
            {
                Check(employeeType);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var ProductCategoryDb = _employeeTypeRepository.GetOne(employeeType.Id);
                employeeType.Sequence = ProductCategoryDb.Sequence;
                employeeType.SynchronizationType = ProductCategoryDb.SynchronizationType;
                employeeType.AddedBy = ProductCategoryDb.AddedBy;
                employeeType.AddedDate = ProductCategoryDb.AddedDate;
                employeeType.AddedFromIp = ProductCategoryDb.AddedFromIp;
                employeeType.UpdatedBy = identity.Name;
                employeeType.UpdatedDate = DateTime.Now;
                employeeType.UpdatedFromIp = identity.IpAddress;
                _employeeTypeRepository.Update(employeeType);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public EmployeeType GetById(string id)
        {
            try
            {
                return _employeeTypeRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<EmployeeType> GetAll()
        {
            try
            {
                return _employeeTypeRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _employeeTypeRepository.GetAll(
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
