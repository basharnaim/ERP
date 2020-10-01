using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Model.Inventory.Suppliers;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Library.Service.Inventory.Accounts
{

    public class SupplierLedgerService : Service<SupplierLedger>, ISupplierLedgerService
    {
        #region Ctor
        private readonly IRepository<SupplierLedger> _supplierLedgerRepository;
        private readonly IRepository<Supplier> _supplierRepository;
        private readonly IUnitOfWork _unitOfWork;
        public SupplierLedgerService(
            IRepository<SupplierLedger> supplierLedgerRepository,
            IRepository<Supplier> supplierRepository,
            IUnitOfWork unitOfWork) : base(supplierLedgerRepository)
        {
            _supplierLedgerRepository = supplierLedgerRepository;
            _supplierRepository = supplierRepository;
            _unitOfWork = unitOfWork;
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string GenerateTrackingNo(string companyId, string branchId, string tableName)
        {
            try
            {
                return base.GenerateTrackingNo(companyId, branchId, tableName);
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
        public override string GenerateMoneyReceiveNo(string companyId, string branchId, string tableName)
        {
            try
            {
                return base.GenerateMoneyReceiveNo(companyId, branchId, tableName);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierLedger"></param>
        public void Add(SupplierLedger supplierLedger)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                Supplier supplier = _supplierRepository.GetOne(x => x.Id == supplierLedger.SupplierId);
                if (string.IsNullOrEmpty(supplierLedger.CompanyId))
                {
                    supplierLedger.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(supplierLedger.BranchId))
                {
                    supplierLedger.BranchId = identity.BranchId;
                }

                supplierLedger.Id = GenerateAutoId(supplierLedger.CompanyId, supplierLedger.BranchId, "SupplierLedger");
                supplierLedger.Sequence = GetAutoSequence();
                if (!string.IsNullOrEmpty(supplierLedger.PurchaseId))
                {
                    supplierLedger.PurchaseId = supplierLedger.PurchaseId.Trim();
                }
                supplierLedger.TrackingNo = GenerateTrackingNo(supplierLedger.CompanyId, supplierLedger.BranchId, "SupplierLedger");
                supplierLedger.MoneyReceiveNo = GenerateMoneyReceiveNo(supplierLedger.CompanyId, supplierLedger.BranchId, "SupplierLedger");
                supplierLedger.TransactionDate = DateTime.Now;
                supplierLedger.SupplierPhone = supplier.Phone1;
                supplierLedger.Active = true;
                supplierLedger.SynchronizationType = SynchronizationType.Server.ToString();
                supplierLedger.AddedBy = identity.Name;
                supplierLedger.AddedDate = DateTime.Now;
                supplierLedger.AddedFromIp = identity.IpAddress;
                _supplierLedgerRepository.Add(supplierLedger);
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
        /// <param name="supplierLedger"></param>
        public void Update(SupplierLedger supplierLedger)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                SupplierLedger dbdata = _supplierLedgerRepository.GetOne(x => x.Id == supplierLedger.Id);
                if (string.IsNullOrEmpty(supplierLedger.CompanyId))
                {
                    supplierLedger.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(supplierLedger.BranchId))
                {
                    supplierLedger.BranchId = identity.BranchId;
                }
                supplierLedger.Sequence = dbdata.Sequence;
                supplierLedger.PurchaseId = supplierLedger.PurchaseId.Trim();
                supplierLedger.TrackingNo = dbdata.TrackingNo;
                supplierLedger.MoneyReceiveNo = dbdata.MoneyReceiveNo;
                supplierLedger.TransactionDate = dbdata.TransactionDate;
                supplierLedger.SupplierPhone = dbdata.SupplierPhone;
                supplierLedger.Active = dbdata.Active;
                supplierLedger.Archive = dbdata.Archive;
                supplierLedger.SynchronizationType = dbdata.SynchronizationType;
                supplierLedger.AddedBy = dbdata.AddedBy;
                supplierLedger.AddedDate = dbdata.AddedDate;
                supplierLedger.AddedFromIp = dbdata.AddedFromIp;
                supplierLedger.UpdatedBy = identity.Name;
                supplierLedger.UpdatedDate = DateTime.Now;
                supplierLedger.UpdatedFromIp = identity.IpAddress;
                _supplierLedgerRepository.Update(supplierLedger);
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
        public SupplierLedger GetById(string id)
        {
            try
            {
                return _supplierLedgerRepository.GetOne(id);
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
        public IEnumerable<SupplierLedger> GetAll()
        {
            try
            {
                return _supplierLedgerRepository.GetAll(r => !r.Archive).OrderByDescending(r => r.Sequence).AsEnumerable();
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
        public IEnumerable<SupplierLedger> GetAll(string companyId)
        {
            try
            {
                return _supplierLedgerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId).OrderByDescending(r => r.Sequence).AsEnumerable();
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
        public IEnumerable<SupplierLedger> GetAll(string companyId, string branchId)
        {
            try
            {
                return _supplierLedgerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId).OrderByDescending(r => r.Sequence).AsEnumerable();
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
                return from r in _supplierLedgerRepository.GetAll(r => !r.Archive && r.Active).OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Particulars };
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
        /// <returns></returns>
        public IEnumerable<object> Lists(string companyId)
        {
            try
            {
                return from r in _supplierLedgerRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId).OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Particulars };
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
        /// <returns></returns>
        public IEnumerable<object> Lists(string companyId, string branchId)
        {
            try
            {
                return from r in _supplierLedgerRepository.GetAll(r => !r.Archive && r.Active && r.CompanyId == companyId && r.BranchId == branchId).OrderBy(r => r.Sequence)
                       select new { Value = r.Id, Text = r.Particulars };
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
        /// <param name="supplierPhone"></param>
        /// <returns></returns>
        public IEnumerable<SupplierLedger> GetAllSupplierLedgerBySupplierPhone(string companyId, string branchId, string supplierPhone)
        {
            try
            {
                return _supplierLedgerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.SupplierPhone == supplierPhone.Trim()).AsEnumerable();
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
        /// <param name="customerId"></param>
        /// <returns></returns>
        public IEnumerable<SupplierLedger> GetAllSupplierLedgerBySupplierId(string companyId, string branchId, string supplierId)
        {
            try
            {
                return _supplierLedgerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.SupplierId == supplierId).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierMobileNo"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        public void GetDueOrAdvanceAmountBySupplierPhone(string supplierMobileNo, out decimal dueAmount, out decimal advanceAmount)
        {
            try
            {
                decimal localDueAmount = 0m;
                decimal localAdvanceAmount = 0m;
                decimal sumOfCreditAmount = 0m;
                decimal sumOfDebitAmount = 0m;
                List<SupplierLedger> supplierledger = _supplierLedgerRepository.GetAll(x => !x.Archive && x.SupplierPhone == supplierMobileNo.Trim()).ToList();
                sumOfCreditAmount = supplierledger.Sum(x => x.CreditAmount);
                sumOfDebitAmount = supplierledger.Sum(x => x.DebitAmount);
                if (sumOfCreditAmount > sumOfDebitAmount)
                    localDueAmount = sumOfCreditAmount - sumOfDebitAmount;
                else
                    localAdvanceAmount = sumOfDebitAmount - sumOfCreditAmount;
                dueAmount = localDueAmount;
                advanceAmount = localAdvanceAmount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        public void GetDueOrAdvanceAmountBySupplierId(string supplierId, out decimal dueAmount, out decimal advanceAmount)
        {
            try
            {
                decimal localDueAmount = 0m;
                decimal localAdvanceAmount = 0m;
                decimal sumOfCreditAmount = 0m;
                decimal sumOfDebitAmount = 0m;
                List<SupplierLedger> supplierledger = _supplierLedgerRepository.GetAll(x => !x.Archive && x.SupplierId == supplierId).ToList();
                sumOfCreditAmount = supplierledger.Sum(x => x.CreditAmount );
                sumOfDebitAmount = supplierledger.Sum(x => x.DebitAmount );
                if (sumOfCreditAmount > sumOfDebitAmount)
                    localDueAmount = sumOfCreditAmount - sumOfDebitAmount;
                else
                    localAdvanceAmount = sumOfDebitAmount - sumOfCreditAmount;
                dueAmount = localDueAmount;
                advanceAmount = localAdvanceAmount;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="supplierId"></param>
        /// <param name="supplierName"></param>
        /// <param name="dueAmount"></param>
        /// <param name="advanceAmount"></param>
        public void GetDueOrAdvanceAmountBySupplierIdWithSupplierName(string supplierId, out string supplierName, out decimal dueAmount, out decimal advanceAmount)
        {
            try
            {
                decimal localDueAmount = 0m;
                decimal localAdvanceAmount = 0m;
                decimal sumOfCreditAmount = 0m;
                decimal sumOfDebitAmount = 0m;
                List<SupplierLedger> supplierledger = _supplierLedgerRepository.GetAll(x => !x.Archive && x.SupplierId == supplierId).ToList();
                sumOfCreditAmount = supplierledger.Sum(x => x.CreditAmount);
                sumOfDebitAmount = supplierledger.Sum(x => x.DebitAmount);
                if (sumOfCreditAmount > sumOfDebitAmount)
                    localDueAmount = sumOfCreditAmount - sumOfDebitAmount;
                else
                    localAdvanceAmount = sumOfDebitAmount - sumOfCreditAmount;
                dueAmount = localDueAmount;
                advanceAmount = localAdvanceAmount;
                supplierName = _supplierRepository.GetOne(x => x.Id == supplierId).Name;
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
        public IEnumerable<SupplierLedger> GetAll(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _supplierLedgerRepository.GetAll(r => !r.Archive && r.CompanyId == companyId && r.BranchId == branchId && r.TransactionDate >= dateFrom && r.TransactionDate <= dateTo).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
