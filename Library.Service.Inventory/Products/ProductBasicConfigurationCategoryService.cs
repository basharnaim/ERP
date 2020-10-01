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
    /// Class ColorService.
    /// <remarks>Jahangir Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class ProductBasicConfigurationCategoryService : Service<ProductBasicConfigurationCategory>, IProductBasicConfigurationCategoryService
    {
        #region Ctor
        private readonly IRepository<ProductBasicConfigurationCategory> _configurationCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductBasicConfigurationCategoryService(
            IRepository<ProductBasicConfigurationCategory> configurationCategoryRepository,
            IUnitOfWork unitOfWork) : base(configurationCategoryRepository)
        {
            _configurationCategoryRepository = configurationCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(ProductBasicConfigurationCategory entity)
        {
            CheckUniqueColumn("Name", entity.Name, x => x.Id != entity.Id && x.Name == entity.Name);
        }

        
        public void Add(ProductBasicConfigurationCategory configurationCategory)
        {
            try
            {
                Check(configurationCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                configurationCategory.Id = GetAutoNumber();
                configurationCategory.Sequence = GetAutoSequence();
                configurationCategory.SynchronizationType = SynchronizationType.Server.ToString();
                configurationCategory.AddedBy = identity.Name;
                configurationCategory.AddedDate = DateTime.Now;
                configurationCategory.AddedFromIp = identity.IpAddress;
                _configurationCategoryRepository.Add(configurationCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public void Update(ProductBasicConfigurationCategory configurationCategory)
        {
            try
            {
                Check(configurationCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var configurationCategoryDb = _configurationCategoryRepository.GetOne(configurationCategory.Id);
                configurationCategory.Sequence = configurationCategoryDb.Sequence;
                configurationCategory.SynchronizationType = configurationCategoryDb.SynchronizationType;
                configurationCategory.AddedBy = configurationCategoryDb.AddedBy;
                configurationCategory.AddedDate = configurationCategoryDb.AddedDate;
                configurationCategory.AddedFromIp = configurationCategoryDb.AddedFromIp;
                configurationCategory.UpdatedBy = identity.Name;
                configurationCategory.UpdatedDate = DateTime.Now;
                configurationCategory.UpdatedFromIp = identity.IpAddress;
                _configurationCategoryRepository.Update(configurationCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public ProductBasicConfigurationCategory GetById(string id)
        {
            try
            {
                return _configurationCategoryRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public IEnumerable<ProductBasicConfigurationCategory> GetAll()
        {
            try
            {
                return _configurationCategoryRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _configurationCategoryRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence).ToList() select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
