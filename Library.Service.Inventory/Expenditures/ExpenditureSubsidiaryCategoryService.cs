using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Expenditures;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.Inventory.Expenditures
{
    /// <summary>
    /// Class expenditureSubsidiaryCategoryService.
    /// <remarks>Jahangir, 2-11-2015</remarks>
    /// </summary>
    public class ExpenditureSubsidiaryCategoryService :Service<ExpenditureSubsidiaryCategory>, IExpenditureSubsidiaryCategoryService
    {
        #region Ctor
        private readonly IRepository<ExpenditureSubsidiaryCategory> _expenditureSubsidiaryCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ExpenditureSubsidiaryCategoryService(
            IRepository<ExpenditureSubsidiaryCategory> expenditureSubsidiaryCategoryRepository,
            IUnitOfWork unitOfWork
            ):base(expenditureSubsidiaryCategoryRepository)
        {
            _expenditureSubsidiaryCategoryRepository = expenditureSubsidiaryCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(ExpenditureSubsidiaryCategory model)
        {
            try
            {
                CheckUniqueColumn("Name", model.Name, r => r.Id != model.Id && r.ExpenditureCategoryId == model.ExpenditureCategoryId && r.ExpenditureSubCategoryId == model.ExpenditureSubCategoryId && r.Name == model.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(ExpenditureSubsidiaryCategory expenditureSubsidiaryCategory)
        {
            try
            {
                Check(expenditureSubsidiaryCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                expenditureSubsidiaryCategory.Id = GetAutoNumber();
                expenditureSubsidiaryCategory.Sequence = GetAutoSequence();
                expenditureSubsidiaryCategory.SynchronizationType = SynchronizationType.Server.ToString();
                expenditureSubsidiaryCategory.AddedBy = identity.Name;
                expenditureSubsidiaryCategory.AddedDate = DateTime.Now;
                expenditureSubsidiaryCategory.AddedFromIp = identity.IpAddress;
                _expenditureSubsidiaryCategoryRepository.Add(expenditureSubsidiaryCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(ExpenditureSubsidiaryCategory expenditureSubsidiaryCategory)
        {
            try
            {
                Check(expenditureSubsidiaryCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _expenditureSubsidiaryCategoryRepository.GetOne(expenditureSubsidiaryCategory.Id);
                expenditureSubsidiaryCategory.Sequence = dbdata.Sequence;
                expenditureSubsidiaryCategory.SynchronizationType = dbdata.SynchronizationType;
                expenditureSubsidiaryCategory.AddedBy = dbdata.AddedBy;
                expenditureSubsidiaryCategory.AddedDate = dbdata.AddedDate;
                expenditureSubsidiaryCategory.AddedFromIp = dbdata.AddedFromIp;
                expenditureSubsidiaryCategory.UpdatedBy = identity.Name;
                expenditureSubsidiaryCategory.UpdatedDate = DateTime.Now;
                expenditureSubsidiaryCategory.UpdatedFromIp = identity.IpAddress;
                _expenditureSubsidiaryCategoryRepository.Update(expenditureSubsidiaryCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ExpenditureSubsidiaryCategory GetById(string id)
        {
            try
            {
                var expenditureSubsidiaryCategories = _expenditureSubsidiaryCategoryRepository.GetOne(id);
                return expenditureSubsidiaryCategories;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ExpenditureSubsidiaryCategory> GetAll()
        {
            try
            {
                var expenditureSubsidiaryCategories = _expenditureSubsidiaryCategoryRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
                return expenditureSubsidiaryCategories;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ExpenditureSubsidiaryCategory> GetAll(string expenditureCategoryId)
        {
            try
            {
                var expenditureSubsidiaryCategories = _expenditureSubsidiaryCategoryRepository.GetAll(r => r.ExpenditureCategoryId == expenditureCategoryId && !r.Archive).AsEnumerable();
                return expenditureSubsidiaryCategories;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ExpenditureSubsidiaryCategory> GetAll(string expenditureCategoryId, string expenditureSubcategoryId)
        {
            try
            {
                var expenditureSubsidiaryCategories = _expenditureSubsidiaryCategoryRepository.GetAll(r => r.ExpenditureCategoryId == expenditureCategoryId && r.ExpenditureSubCategoryId == expenditureSubcategoryId && !r.Archive).AsEnumerable();
                return expenditureSubsidiaryCategories;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<object> Lists(string expenditureSubCategoryId)
        {
            try
            {
                IEnumerable<object> result = from r in _expenditureSubsidiaryCategoryRepository.GetAll(r => r.Active && !r.Archive && r.ExpenditureSubCategoryId == expenditureSubCategoryId)
                                                 .OrderBy(r => r.Sequence)
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
                return GetAutoSequence("ExpenditureSubsidiaryCategory");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
