using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Suppliers;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.Inventory.Suppliers
{
    /// <summary>
    /// Class SupplierCategoryService.
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class SupplierCategoryService : Service<SupplierCategory>, ISupplierCategoryService
    {
        #region Ctor
        private readonly IRepository<SupplierCategory> _supplierCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public SupplierCategoryService(
            IRepository<SupplierCategory> supplierCategoryRepository,
            IUnitOfWork unitOfWork
            ) : base(supplierCategoryRepository)
        {
            _supplierCategoryRepository = supplierCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(SupplierCategory model)
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
        
        public void Add(SupplierCategory supplierCategory)
        {
            try
            {
                Check(supplierCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                supplierCategory.Id = GetAutoNumber();
                supplierCategory.Sequence = GetAutoSequence();
                supplierCategory.SynchronizationType = SynchronizationType.Server.ToString();
                supplierCategory.AddedBy = identity.Name;
                supplierCategory.AddedDate = DateTime.Now;
                supplierCategory.AddedFromIp = identity.IpAddress;
                _supplierCategoryRepository.Add(supplierCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        
        public void Update(SupplierCategory supplierCategory)
        {
            try
            {
                Check(supplierCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _supplierCategoryRepository.GetOne(supplierCategory.Id);
                supplierCategory.Sequence = dbdata.Sequence;
                supplierCategory.SynchronizationType = dbdata.SynchronizationType;
                supplierCategory.AddedBy = dbdata.AddedBy;
                supplierCategory.AddedDate = dbdata.AddedDate;
                supplierCategory.AddedFromIp = dbdata.AddedFromIp;
                supplierCategory.UpdatedBy = identity.Name;
                supplierCategory.UpdatedDate = DateTime.Now;
                supplierCategory.UpdatedFromIp = identity.IpAddress;
                _supplierCategoryRepository.Update(supplierCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Archive(string id)
        {
            try
            {
                var dbdata = _supplierCategoryRepository.GetOne(id);
                dbdata.Archive = true;
                _supplierCategoryRepository.Update(dbdata);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public SupplierCategory GetById(string id)
        {
            try
            {
                return _supplierCategoryRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public IEnumerable<SupplierCategory> GetAll()
        {
            try
            {
                return _supplierCategoryRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                IEnumerable<object> result = from r in _supplierCategoryRepository.GetAll(r => r.Active && !r.Archive).OrderBy(r => r.Sequence)
                                             select new { Value = r.Id, Text = r.Name };
                return result;
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
                return GetAutoSequence("SupplierCategory");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
