#region Using

using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Expenditures;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#endregion

namespace Library.Service.Inventory.Expenditures
{
    /// <summary>
    /// Class expenditureSubCategoryService.
    /// <remarks>Jahangir, 01-02-2016</remarks>
    /// </summary>
    public class ExpenditureSubCategoryService :Service<ExpenditureSubCategory>, IExpenditureSubCategoryService
    {
        #region Ctor
        private readonly IRepository<ExpenditureSubCategory> _expenditureSubCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ExpenditureSubCategoryService(
            IRepository<ExpenditureSubCategory> expenditureSubCategoryRepository,
            IUnitOfWork unitOfWork
            ):base(expenditureSubCategoryRepository)
        {
            _expenditureSubCategoryRepository = expenditureSubCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(ExpenditureSubCategory model)
        {
            try
            {
                CheckUniqueColumn("Name", model.Name, r => r.Id != model.Id && r.ExpenditureCategoryId == model.ExpenditureCategoryId && r.Name == model.Name);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Add(ExpenditureSubCategory expenditureSubCategory)
        {
            try
            {
                Check(expenditureSubCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                expenditureSubCategory.Id = GetAutoNumber();
                expenditureSubCategory.Sequence = GetAutoSequence();
                expenditureSubCategory.Archive = false;
                expenditureSubCategory.IsSynchronized = false;
                expenditureSubCategory.IsUpdated = false;
                expenditureSubCategory.SynchronizationType = SynchronizationType.Server.ToString();
                expenditureSubCategory.AddedBy = identity.Name;
                expenditureSubCategory.AddedDate = DateTime.Now;
                expenditureSubCategory.AddedFromIp = identity.IpAddress;
                _expenditureSubCategoryRepository.Add(expenditureSubCategory);
                _unitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(ExpenditureSubCategory expenditureSubCategory)
        {
            try
            {
                Check(expenditureSubCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _expenditureSubCategoryRepository.GetOne(expenditureSubCategory.Id);
                expenditureSubCategory.Sequence = dbdata.Sequence;
                expenditureSubCategory.SynchronizationType = dbdata.SynchronizationType;
                expenditureSubCategory.AddedBy = dbdata.AddedBy;
                expenditureSubCategory.AddedDate = dbdata.AddedDate;
                expenditureSubCategory.AddedFromIp = dbdata.AddedFromIp;
                expenditureSubCategory.UpdatedBy = identity.Name;
                expenditureSubCategory.UpdatedDate = DateTime.Now;
                expenditureSubCategory.UpdatedFromIp = identity.IpAddress;
                _expenditureSubCategoryRepository.Update(expenditureSubCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ExpenditureSubCategory GetById(string id)
        {
            try
            {
                return _expenditureSubCategoryRepository.GetOne(id);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ExpenditureSubCategory> GetAll()
        {
            try
            {
                var expenditureSubCategories = _expenditureSubCategoryRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
                return expenditureSubCategories;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ExpenditureSubCategory> GetAll(string expenditureCategoryId)
        {
            try
            {
                var expenditureSubCategories = _expenditureSubCategoryRepository.GetAll(r => r.ExpenditureCategoryId == expenditureCategoryId && !r.Archive).AsEnumerable();
                return expenditureSubCategories;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<object> Lists(string expenditureCategoryId)
        {
            try
            {
                IEnumerable<object> result = from r in _expenditureSubCategoryRepository.GetAll(r => r.Active && !r.Archive && r.ExpenditureCategoryId == expenditureCategoryId)
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
                return GetAutoSequence("ExpenditureSubCategory");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
