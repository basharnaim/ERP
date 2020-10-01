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
    public class ProductBasicConfigurationService : Service<ProductBasicConfiguration>, IProductBasicConfigurationService
    {
        #region Ctor
        private readonly IRepository<ProductBasicConfiguration> _configurationSettingRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductBasicConfigurationService(
            IRepository<ProductBasicConfiguration> configurationSettingRepository,
            IUnitOfWork unitOfWork) : base(configurationSettingRepository)
        {
            _configurationSettingRepository = configurationSettingRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(ProductBasicConfiguration entity)
        {
            try
            {
                CheckUniqueColumn("Name", entity.Name, x => x.Id != entity.Id && x.ProductBasicConfigurationCategoryId == entity.ProductBasicConfigurationCategoryId && x.Name == entity.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void Add(ProductBasicConfiguration configurationSetting)
        {
            try
            {
                Check(configurationSetting);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                configurationSetting.Id = GetAutoNumber();
                configurationSetting.Sequence = GetAutoSequence();
                configurationSetting.SynchronizationType = SynchronizationType.Server.ToString();
                configurationSetting.AddedBy = identity.Name;
                configurationSetting.AddedDate = DateTime.Now;
                configurationSetting.AddedFromIp = identity.IpAddress;
                _configurationSettingRepository.Add(configurationSetting);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void Update(ProductBasicConfiguration configurationSetting)
        {
            try
            {
                Check(configurationSetting);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var configurationSettingDb = _configurationSettingRepository.GetOne(configurationSetting.Id);
                configurationSetting.Sequence = configurationSettingDb.Sequence;
                configurationSetting.SynchronizationType = configurationSettingDb.SynchronizationType;
                configurationSetting.AddedBy = configurationSettingDb.AddedBy;
                configurationSetting.AddedDate = configurationSettingDb.AddedDate;
                configurationSetting.AddedFromIp = configurationSettingDb.AddedFromIp;
                configurationSetting.UpdatedBy = identity.Name;
                configurationSetting.UpdatedDate = DateTime.Now;
                configurationSetting.UpdatedFromIp = identity.IpAddress;
                _configurationSettingRepository.Update(configurationSetting);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public ProductBasicConfiguration GetById(string id)
        {
            try
            {
                return _configurationSettingRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public IEnumerable<ProductBasicConfiguration> GetAll()
        {
            try
            {
                return _configurationSettingRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ProductBasicConfiguration> GetAll(string productBasicConfigurationCategoryId)
        {
            try
            {
                return _configurationSettingRepository.GetAll(r => !r.Archive && r.ProductBasicConfigurationCategoryId == productBasicConfigurationCategoryId).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _configurationSettingRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence).ToList() select new { Value = r.Id, Text = r.Name };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
