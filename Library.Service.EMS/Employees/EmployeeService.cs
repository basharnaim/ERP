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
    public class EmployeeService : Service<Employee>, IEmployeeService
    {
        #region Ctor
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeService(
            IRepository<Employee> employeeRepository,
            IUnitOfWork unitOfWork
            ) : base(employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Action
        private void Check(Employee model)
        {
            try
            {
                CheckUniqueColumn("Code", model.Code, r => r.Id != model.Id && r.Code == model.Code);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Add(Employee employee)
        {
            try
            {
                Check(employee);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                employee.Id = GetAutoNumber();
                employee.Sequence = GetAutoSequence();
                employee.SynchronizationType = SynchronizationType.Server.ToString();
                employee.AddedBy = identity.Name;
                employee.AddedDate = DateTime.Now;
                employee.AddedFromIp = identity.IpAddress;
                _employeeRepository.Add(employee);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Update(Employee employee)
        {
            try
            {
                Check(employee);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var ProductCategoryDb = _employeeRepository.GetOne(employee.Id);
                employee.Sequence = ProductCategoryDb.Sequence;
                employee.SynchronizationType = ProductCategoryDb.SynchronizationType;
                employee.AddedBy = ProductCategoryDb.AddedBy;
                employee.AddedDate = ProductCategoryDb.AddedDate;
                employee.AddedFromIp = ProductCategoryDb.AddedFromIp;
                employee.UpdatedBy = identity.Name;
                employee.UpdatedDate = DateTime.Now;
                employee.UpdatedFromIp = identity.IpAddress;
                _employeeRepository.Update(employee);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Employee GetById(string id)
        {
            try
            {
                return _employeeRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<Employee> GetAll()
        {
            try
            {
                return _employeeRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _employeeRepository.GetAll(
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
