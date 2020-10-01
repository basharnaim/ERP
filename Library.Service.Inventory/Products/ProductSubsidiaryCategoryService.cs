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
    /// Class ProductSubsidiaryCategoryService.
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class ProductSubsidiaryCategoryService :Service<ProductSubsidiaryCategory>, IProductSubsidiaryCategoryService
    {
        #region Ctor
        private readonly IRepository<ProductSubsidiaryCategory> _productSubsidiaryCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ProductSubsidiaryCategoryService(
            IRepository<ProductSubsidiaryCategory> productSubsidiaryCategoryRepository,
            IUnitOfWork unitOfWork
            ):base(productSubsidiaryCategoryRepository)
        {
            _productSubsidiaryCategoryRepository = productSubsidiaryCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(ProductSubsidiaryCategory model)
        {
            try
            {
                CheckUniqueColumn("Name", model.Name, r => r.Id != model.Id && r.ProductCategoryId == model.ProductCategoryId && r.ProductSubCategoryId == model.ProductSubCategoryId && r.Name == model.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        public void Add(ProductSubsidiaryCategory productSubsidiaryCategory)
        {
            try
            {
                Check(productSubsidiaryCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                productSubsidiaryCategory.Id = GetAutoNumber();
                productSubsidiaryCategory.Sequence = GetAutoSequence();
                productSubsidiaryCategory.SynchronizationType = SynchronizationType.Server.ToString();
                productSubsidiaryCategory.AddedBy = identity.Name;
                productSubsidiaryCategory.AddedDate = DateTime.Now;
                productSubsidiaryCategory.AddedFromIp = identity.IpAddress;
                _productSubsidiaryCategoryRepository.Add(productSubsidiaryCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public void Update(ProductSubsidiaryCategory productSubsidiaryCategory)
        {
            try
            {
                Check(productSubsidiaryCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _productSubsidiaryCategoryRepository.GetOne(productSubsidiaryCategory.Id);
                productSubsidiaryCategory.Sequence = dbdata.Sequence;
                productSubsidiaryCategory.SynchronizationType = dbdata.SynchronizationType;
                productSubsidiaryCategory.AddedBy = dbdata.AddedBy;
                productSubsidiaryCategory.AddedDate = dbdata.AddedDate;
                productSubsidiaryCategory.AddedFromIp = dbdata.AddedFromIp;
                productSubsidiaryCategory.UpdatedBy = identity.Name;
                productSubsidiaryCategory.UpdatedDate = DateTime.Now;
                productSubsidiaryCategory.UpdatedFromIp = identity.IpAddress;
                _productSubsidiaryCategoryRepository.Update(productSubsidiaryCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public ProductSubsidiaryCategory GetById(string id)
        {
            try
            {
                return _productSubsidiaryCategoryRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        
        public IEnumerable<ProductSubsidiaryCategory> GetAll()
        {
            try
            {
                return _productSubsidiaryCategoryRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productCategoryId"></param>
        /// <returns></returns>
        public IEnumerable<ProductSubsidiaryCategory> GetAll(string productCategoryId)
        {
            try
            {
                return _productSubsidiaryCategoryRepository.GetAll(r => !r.Archive && r.ProductCategoryId == productCategoryId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productCategoryId"></param>
        /// <param name="productSubcategoryId"></param>
        /// <returns></returns>
        public IEnumerable<ProductSubsidiaryCategory> GetAll(string productCategoryId, string productSubcategoryId)
        {
            try
            {
                return _productSubsidiaryCategoryRepository.GetAll(r => !r.Archive && r.ProductCategoryId == productCategoryId && r.ProductSubCategoryId == productSubcategoryId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="productSubCategoryId"></param>
        /// <returns></returns>
        public IEnumerable<object> Lists(string productSubCategoryId)
        {
            try
            {
                return from r in _productSubsidiaryCategoryRepository.GetAll(r => !r.Archive && r.Active && r.ProductSubCategoryId == productSubCategoryId)
                                                 .OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Name };
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
                return GetAutoSequence("ProductSubsidiaryCategory");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
