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
    /// Class expenditureCategoryService.
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class ExpenditureCategoryService :Service<ExpenditureCategory>, IExpenditureCategoryService
    {
        #region Ctor
        private readonly IRepository<ExpenditureCategory> _expenditureCategoryRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ExpenditureCategoryService(
            IRepository<ExpenditureCategory> expenditureCategoryRepository,
            IUnitOfWork unitOfWork
            ):base(expenditureCategoryRepository)
        {
            _expenditureCategoryRepository = expenditureCategoryRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        private void Check(ExpenditureCategory model)
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

        public void Add(ExpenditureCategory expenditureCategory)
        {
            try
            {
                Check(expenditureCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                expenditureCategory.Id = GetAutoNumber();
                expenditureCategory.Sequence = GetAutoSequence();
                expenditureCategory.SynchronizationType = SynchronizationType.Server.ToString();
                expenditureCategory.AddedBy = identity.Name;
                expenditureCategory.AddedDate = DateTime.Now;
                expenditureCategory.AddedFromIp = identity.IpAddress;
                _expenditureCategoryRepository.Add(expenditureCategory);
                _unitOfWork.SaveChanges();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public void Update(ExpenditureCategory expenditureCategory)
        {
            try
            {
                Check(expenditureCategory);
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _expenditureCategoryRepository.GetOne(expenditureCategory.Id);
                expenditureCategory.Sequence = dbdata.Sequence;
                expenditureCategory.SynchronizationType = dbdata.SynchronizationType;
                expenditureCategory.AddedBy = dbdata.AddedBy;
                expenditureCategory.AddedDate = dbdata.AddedDate;
                expenditureCategory.AddedFromIp = dbdata.AddedFromIp;
                expenditureCategory.UpdatedBy = identity.Name;
                expenditureCategory.UpdatedDate = DateTime.Now;
                expenditureCategory.UpdatedFromIp = identity.IpAddress;
                _expenditureCategoryRepository.Update(expenditureCategory);
                _unitOfWork.SaveChanges();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public ExpenditureCategory GetById(string id)
        {
            try
            {
                var expenditureCategories = _expenditureCategoryRepository.GetOne(id);
                return expenditureCategories;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<ExpenditureCategory> GetAll()
        {
            try
            {
                var expenditureCategories = _expenditureCategoryRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
                return expenditureCategories;
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
                IEnumerable<object> result = from r in _expenditureCategoryRepository.GetAll(
                                                 r => r.Active && !r.Archive).OrderBy(r => r.Sequence)
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
                return GetAutoSequence("ExpenditureCategory");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
