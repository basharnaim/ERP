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
    public class VatCategoryService : Service<VatCategory>, IVatCategoryService
    {
        #region Ctor
        private readonly IRepository<VatCategory> _vatCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public VatCategoryService(
            IRepository<VatCategory> vatCategoryRepository,
            IUnitOfWork unitOfWork) : base(vatCategoryRepository)
        {
            _vatCategoryRepository = vatCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(VatCategory entity)
        {
            CheckUniqueColumn("Name", entity.Name, x => x.Id != entity.Id && x.Name == entity.Name);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vatCategory"></param>
        public void Add(VatCategory vatCategory)
        {
            try
            {
                Check(vatCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                vatCategory.Id = GetAutoNumber();
                vatCategory.Sequence = GetAutoSequence();
                vatCategory.SynchronizationType = SynchronizationType.Server.ToString();
                vatCategory.AddedBy = identity.Name;
                vatCategory.AddedDate = DateTime.Now;
                vatCategory.AddedFromIp = identity.IpAddress;
                _vatCategoryRepository.Add(vatCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vatCategory"></param>
        public void Update(VatCategory vatCategory)
        {
            try
            {
                Check(vatCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var vatCategoryDb = _vatCategoryRepository.GetOne(vatCategory.Id);
                vatCategory.Sequence = vatCategoryDb.Sequence;
                vatCategory.SynchronizationType = vatCategoryDb.SynchronizationType;
                vatCategory.AddedBy = vatCategoryDb.AddedBy;
                vatCategory.AddedDate = vatCategoryDb.AddedDate;
                vatCategory.AddedFromIp = vatCategoryDb.AddedFromIp;
                vatCategory.UpdatedBy = identity.Name;
                vatCategory.UpdatedDate = DateTime.Now;
                vatCategory.UpdatedFromIp = identity.IpAddress;
                _vatCategoryRepository.Update(vatCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public VatCategory GetById(string id)
        {
            try
            {
                return _vatCategoryRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<VatCategory> GetAll()
        {
            try
            {
                return _vatCategoryRepository.GetAll(r => !r.Archive).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<object> Lists()
        {
            try
            {
                return from r in _vatCategoryRepository.GetAll(r => r.Active && !r.Archive)
                                            .OrderByDescending(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="vatCategoryId"></param>
        /// <returns></returns>
        public decimal GetVatCategoryRate(string vatCategoryId)
        {
            try
            {
                return _vatCategoryRepository.GetOne(vatCategoryId).VatRate;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
