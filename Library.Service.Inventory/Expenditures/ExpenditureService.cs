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
    /// Class SaleReturnService.
    /// <remarks>Jahangir Hossain Sheikh, 2-11-2015</remarks>
    /// </summary>
    public class ExpenditureService : Service<Expenditure>, IExpenditureService
    {
        #region Ctor
        private readonly IRepository<Expenditure> _expenditureRepository;
        private readonly IUnitOfWork _unitOfWork;
        public ExpenditureService(
            IRepository<Expenditure> expenditureRepository,
            IUnitOfWork unitOfWork
            ) : base(expenditureRepository)
        {
            _expenditureRepository = expenditureRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="expenditure"></param>
        public void Add(Expenditure expenditure)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (!string.IsNullOrEmpty(expenditure.CompanyId))
                {
                    expenditure.CompanyId = identity.CompanyId;
                }
                if (!string.IsNullOrEmpty(expenditure.BranchId))
                {
                    expenditure.BranchId = identity.BranchId;
                }
                expenditure.Id = GetAutoSequence("Expenditure").ToString(); //GenerateAutoId(expenditure.CompanyId, expenditure.BranchId, "Expenditure");
                expenditure.Sequence = GetAutoSequence("Expenditure");
                expenditure.Active = true;
                expenditure.Archive = false;
                expenditure.IsUpdated = false;
                expenditure.SynchronizationType = SynchronizationType.Server.ToString();
                expenditure.AddedBy = identity.Name;
                expenditure.AddedDate = DateTime.Now;
                expenditure.AddedFromIp = identity.IpAddress;                
                _expenditureRepository.Add(expenditure);
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
        /// <param name="expenditure"></param>
        public void Update(Expenditure expenditure)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _expenditureRepository.GetOne(expenditure.Id);
                if (!string.IsNullOrEmpty(expenditure.CompanyId))
                {
                    expenditure.CompanyId = dbdata.CompanyId;
                }
                if (!string.IsNullOrEmpty(expenditure.BranchId))
                {
                    expenditure.BranchId = dbdata.BranchId;
                }
                expenditure.Sequence = dbdata.Sequence;
                expenditure.CompanyId = dbdata.CompanyId;
                expenditure.BranchId = dbdata.BranchId;
                expenditure.SynchronizationType = dbdata.SynchronizationType;
                expenditure.AddedBy = dbdata.AddedBy;
                expenditure.AddedDate = dbdata.AddedDate;
                expenditure.AddedFromIp = dbdata.AddedFromIp;
                expenditure.UpdatedBy = identity.Name;
                expenditure.UpdatedDate = DateTime.Now;
                expenditure.UpdatedFromIp = identity.IpAddress;
                _expenditureRepository.Update(expenditure);
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
        public Expenditure GetById(string id)
        {
            try
            {
                return _expenditureRepository.GetOne(id);
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
        public IEnumerable<Expenditure> GetAll()
        {
            try
            {
                return _expenditureRepository.GetAll(r => !r.Archive, "ExpenditureCategory, ExpenditureSubCategory, ExpenditureSubsidiaryCategory").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Expenditure> GetAll(string companyId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _expenditureRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.ExpenseDate >= dateFrom && r.ExpenseDate <= dateTo, "ExpenditureCategory, ExpenditureSubCategory, ExpenditureSubsidiaryCategory").OrderByDescending(x => x.ExpenseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public IEnumerable<Expenditure> GetAll(string companyId)
        {
            try
            {
                return _expenditureRepository.GetAll(r => r.CompanyId == companyId && !r.Archive, "ExpenditureCategory, ExpenditureSubCategory, ExpenditureSubsidiaryCategory").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Expenditure> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _expenditureRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.ExpenseDate >= dateFrom && r.ExpenseDate <= dateTo, "ExpenditureCategory, ExpenditureSubCategory, ExpenditureSubsidiaryCategory").OrderByDescending(x => x.ExpenseDate).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="expenditureCategoryId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public IEnumerable<Expenditure> GetAll(string companyId, string branchId, string expenditureCategoryId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _expenditureRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.ExpenditureCategoryId == expenditureCategoryId.Trim() && r.ExpenseDate >= dateFrom && r.ExpenseDate <= dateTo, "ExpenditureCategory, ExpenditureSubCategory, ExpenditureSubsidiaryCategory").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="expenditureCategoryId"></param>
        /// <returns></returns>
        public IEnumerable<Expenditure> GetAll(string companyId, string branchId, string expenditureCategoryId)
        {
            try
            {
                return _expenditureRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.ExpenditureCategoryId == expenditureCategoryId.Trim(), "ExpenditureCategory, ExpenditureSubCategory, ExpenditureSubsidiaryCategory").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<Expenditure> GetAll(string companyId, string branchId)
        {
            try
            {
                return _expenditureRepository.GetAll(r => r.CompanyId == companyId && r.BranchId == branchId && !r.Archive, "ExpenditureCategory, ExpenditureSubCategory, ExpenditureSubsidiaryCategory").AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="branchId"></param>
        /// <returns></returns>
        public IEnumerable<object> Lists(string branchId)
        {
            try
            {
                IEnumerable<object> result = from r in _expenditureRepository.GetAll(r => r.Active && !r.Archive && r.BranchId == branchId)
                                                 .OrderByDescending(r => r.Sequence)
                                             select new { Value = r.Id, Text = r.ExpenseName };
                return result;
            }
            catch
            {
                return null;
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
                IEnumerable<object> result = from r in _expenditureRepository.GetAll(r => r.Active && !r.Archive)
                                                 .OrderByDescending(r => r.Sequence)
                                             select new { Value = r.Id, Text = r.ExpenseName };
                return result;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        public decimal GetTotalExpensesBetweenDate(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                decimal totalExpenses = 0m;
                var expenditures = _expenditureRepository.GetAll(x => !x.Archive && x.CompanyId == companyId && x.BranchId == branchId && x.ExpenseDate >= dateFrom && x.ExpenseDate <= dateTo).ToList();
                totalExpenses = expenditures.Sum(x => x.ExpenseAmount);
                return totalExpenses;
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
                return GetAutoSequence("Expenditure");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
