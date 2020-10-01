#region Using

using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Customers;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#endregion

namespace Library.Service.Inventory.Customers
{
    /// <summary>
    /// Class CustomerCategoryService.
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class CustomerCategoryService : Service<CustomerCategory>, ICustomerCategoryService
    {
        #region Ctor
        private readonly IRepository<CustomerCategory> _customerCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CustomerCategoryService(
            IRepository<CustomerCategory> customerCategoryRepository,
            IUnitOfWork unitOfWork
            ) : base(customerCategoryRepository)
        {
            _customerCategoryRepository = customerCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(CustomerCategory model)
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

        public void Add(CustomerCategory customerCategory)
        {
            try
            {
                Check(customerCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                customerCategory.Id = GetAutoNumber();
                customerCategory.Sequence = GetAutoSequence();
                customerCategory.SynchronizationType = SynchronizationType.Server.ToString();
                customerCategory.AddedBy = identity.Name;
                customerCategory.AddedDate = DateTime.Now;
                customerCategory.AddedFromIp = identity.IpAddress;
                _customerCategoryRepository.Add(customerCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(CustomerCategory customerCategory)
        {
            try
            {
                Check(customerCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _customerCategoryRepository.GetOne(customerCategory.Id);
                customerCategory.Sequence = dbdata.Sequence;
                customerCategory.SynchronizationType = dbdata.SynchronizationType;
                customerCategory.AddedBy = dbdata.AddedBy;
                customerCategory.AddedDate = dbdata.AddedDate;
                customerCategory.AddedFromIp = dbdata.AddedFromIp;
                customerCategory.UpdatedBy = identity.Name;
                customerCategory.UpdatedDate = DateTime.Now;
                customerCategory.UpdatedFromIp = identity.IpAddress;
                _customerCategoryRepository.Update(customerCategory);
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
                var dbdata = _customerCategoryRepository.GetOne(id);
                dbdata.Archive = true;
                dbdata.Active = false;
                _customerCategoryRepository.Update(dbdata);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public CustomerCategory GetById(string id)
        {
            try
            {
                return _customerCategoryRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<CustomerCategory> GetAll()
        {
            try
            {
                return _customerCategoryRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _customerCategoryRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
            }
            catch
            {
                return null;
            }
        }

        public decimal GetCustomerCategoryDiscount(string customerCategoryId)
        {
            try
            {
                
                var discount = _customerCategoryRepository.GetOne(customerCategoryId).DiscountRate;
                return discount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public override int GetAutoSequence()
        {
            try
            {
                return GetAutoSequence("CustomerCategory");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
