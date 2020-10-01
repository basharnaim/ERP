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
    public class DepartmentService : Service<Department>, IDepartmentService
    {
        #region Ctor
        private readonly IRepository<Department> _departmentRepository;
        private readonly IUnitOfWork _unitOfWork;
        public DepartmentService(
            IRepository<Department> departmentRepository,
            IUnitOfWork unitOfWork
            ) : base(departmentRepository)
        {
            _departmentRepository = departmentRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        #region Action
        private void Check(Department model)
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
        public void Add(Department department)
        {
            try
            {
                Check(department);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                department.Id = GetAutoNumber();
                department.Sequence = GetAutoSequence();
                department.SynchronizationType = SynchronizationType.Server.ToString();
                department.AddedBy = identity.Name;
                department.AddedDate = DateTime.Now;
                department.AddedFromIp = identity.IpAddress;
                _departmentRepository.Add(department);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public void Update(Department department)
        {
            try
            {
                Check(department);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var departmentDb = _departmentRepository.GetOne(department.Id);
                department.Sequence = departmentDb.Sequence;
                department.SynchronizationType = departmentDb.SynchronizationType;
                department.AddedBy = departmentDb.AddedBy;
                department.AddedDate = departmentDb.AddedDate;
                department.AddedFromIp = departmentDb.AddedFromIp;
                department.UpdatedBy = identity.Name;
                department.UpdatedDate = DateTime.Now;
                department.UpdatedFromIp = identity.IpAddress;
                _departmentRepository.Update(department);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public Department GetById(string id)
        {
            try
            {
                return _departmentRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        public IEnumerable<Department> GetAll()
        {
            try
            {
                return _departmentRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _departmentRepository.GetAll(
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
