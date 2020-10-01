#region Using

using Library.Context.Core;
using Library.Context.UnitOfWorks;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Service.Core.Core;
using Library.Context.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#endregion

namespace Library.Service.Inventory.Accounts
{
    public class BankLedgerService : Service<BankLedger>, IBankLedgerService
    {
        #region Ctor
        private readonly IRepository<BankLedger> _bankLedgerRepository;
        private readonly IRawSqlService _rawSqlService;
        private readonly IUnitOfWork _unitOfWork;

        public BankLedgerService(
            IRepository<BankLedger> bankLedgerRepository,
            IRawSqlService rawSqlService,
            IUnitOfWork unitOfWork
            ) : base(bankLedgerRepository)
        {
            _bankLedgerRepository = bankLedgerRepository;
            _rawSqlService = rawSqlService;
            _unitOfWork = unitOfWork;
        }
        #endregion

        public void Add(BankLedger bankLedger, decimal amount)
        {
            bool flag = false;
            try
            {
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (string.IsNullOrEmpty(bankLedger.CompanyId))
                {
                    bankLedger.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(bankLedger.BranchId))
                {
                    bankLedger.BranchId = identity.BranchId;
                }
                bankLedger.Id = GenerateAutoId(bankLedger.CompanyId, bankLedger.BranchId, "BankLedger");
                bankLedger.Sequence = GetAutoSequence("BankLedger");
                bankLedger.TrackingNo = GenerateTrackingNo(bankLedger.CompanyId, bankLedger.BranchId, "BankLedger");
                if (bankLedger.TransactionType == BankTransactionType.Deposit.ToString())
                {
                    bankLedger.DebitAmount = amount;
                    bankLedger.CreditAmount = 0;
                }
                else if (bankLedger.TransactionType == BankTransactionType.Withdrawn.ToString())
                {
                    bankLedger.DebitAmount = 0;
                    bankLedger.CreditAmount = amount;
                }
                bankLedger.SynchronizationType = SynchronizationType.Server.ToString();
                bankLedger.AddedBy = identity.Name;
                bankLedger.AddedDate = DateTime.Now;
                bankLedger.AddedFromIp = identity.IpAddress;
                _bankLedgerRepository.Add(bankLedger);
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                _rawSqlService.UpdateBankLedgerRunningBalance(bankLedger?.AccountNo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }

        public void Update(BankLedger bankLedger, decimal amount)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var dbdata = _bankLedgerRepository.GetOne(bankLedger.Id);
                if (string.IsNullOrEmpty(bankLedger.CompanyId))
                {
                    bankLedger.CompanyId = identity.CompanyId;
                }
                if (string.IsNullOrEmpty(bankLedger.BranchId))
                {
                    bankLedger.BranchId = identity.BranchId;
                }
                bankLedger.TrackingNo = GenerateTrackingNo(bankLedger.CompanyId, bankLedger.BranchId, "BankLedger");
                if (bankLedger.TransactionType == BankTransactionType.Deposit.ToString())
                {
                    bankLedger.DebitAmount = amount;
                    bankLedger.CreditAmount = 0;
                }
                else if (bankLedger.TransactionType == BankTransactionType.Withdrawn.ToString())
                {
                    bankLedger.DebitAmount = 0;
                    bankLedger.CreditAmount = amount;
                }
                bankLedger.Sequence = dbdata.Sequence;
                bankLedger.SynchronizationType = dbdata.SynchronizationType;
                bankLedger.AddedBy = dbdata.AddedBy;
                bankLedger.AddedDate = dbdata.AddedDate;
                bankLedger.AddedFromIp = dbdata.AddedFromIp;
                bankLedger.Active = dbdata.Active;
                bankLedger.Archive = dbdata.Archive;
                bankLedger.UpdatedBy = identity.Name;
                bankLedger.UpdatedDate = DateTime.Now;
                bankLedger.UpdatedFromIp = identity.IpAddress;
                _bankLedgerRepository.Update(bankLedger);
                _unitOfWork.SaveChanges();
                _rawSqlService.UpdateBankLedgerRunningBalance(bankLedger?.AccountNo);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="bankLedger"></param>
        /// <param name="amount"></param>
        public void AddOpeningBalance(BankLedger bankLedger, decimal amount)
        {
            bool flag = false;
            try
            {
                bool hasOldBankLedgerOp = _bankLedgerRepository.Any(x => x.Id != null && x.AccountNo == bankLedger.AccountNo && x.TransactionType == BankTransactionType.OpeningBalance.ToString());
                if (hasOldBankLedgerOp)
                {
                    throw new Exception("This account No (" + bankLedger.AccountNo + ") Opening Balance already exists.");
                }

                #region Not Exist
                _unitOfWork.BeginTransaction();
                flag = true;
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (!string.IsNullOrEmpty(bankLedger.Id))
                {
                    BankLedger oldBankLedger = _bankLedgerRepository.GetOne(bankLedger.Id);
                    oldBankLedger.DebitAmount = amount;
                    oldBankLedger.CreditAmount = 0;
                    oldBankLedger.BankId = bankLedger.BankId;
                    oldBankLedger.AccountNo = bankLedger.AccountNo;
                    oldBankLedger.Particulars = bankLedger.Particulars;
                    oldBankLedger.TransactionType = bankLedger.TransactionType;
                    oldBankLedger.TransactionDate = bankLedger.TransactionDate;
                    oldBankLedger.UpdatedBy = identity.Name;
                    oldBankLedger.UpdatedDate = DateTime.Now;
                    oldBankLedger.UpdatedFromIp = identity.IpAddress;
                    _bankLedgerRepository.Update(oldBankLedger);
                }
                else
                {
                    string stringPrefix = "";
                    if (string.IsNullOrEmpty(bankLedger.CompanyId))
                    {
                        bankLedger.CompanyId = identity.CompanyId;
                    }
                    if (string.IsNullOrEmpty(bankLedger.BranchId))
                    {
                        bankLedger.BranchId = identity.BranchId;
                    }
                    string branchIdWithPad = bankLedger.BranchId.ToString().PadLeft(2, '0');
                    stringPrefix = "" + bankLedger.CompanyId + "" + branchIdWithPad + "";
                    int id = 0;
                    var idList = _bankLedgerRepository.GetAll().Select(x => x.Id).ToList();
                    if (idList.Count() != 0)
                        id = idList.Max(x => Convert.ToInt32(x.Substring(stringPrefix.Length)) + 1);
                    else
                        id = 1;
                    string stringId = id.ToString();
                    string stringIdWithPad = stringId.PadLeft(5, '0');
                    string stringIdWithCB = "" + stringPrefix + "" + stringIdWithPad + "";
                    bankLedger.Id = stringIdWithCB;
                    bankLedger.Sequence = GetAutoSequence();
                    bankLedger.TrackingNo = GenerateTrackingNo(bankLedger.CompanyId, bankLedger.BranchId, "BankLedger");
                    bankLedger.DebitAmount = amount;
                    bankLedger.CreditAmount = 0;
                    bankLedger.SynchronizationType = SynchronizationType.Server.ToString();
                    bankLedger.AddedBy = identity.Name;
                    bankLedger.AddedDate = DateTime.Now;
                    bankLedger.AddedFromIp = identity.IpAddress;
                    _bankLedgerRepository.Add(bankLedger);
                }
                _unitOfWork.SaveChanges();
                flag = false;
                _unitOfWork.Commit();
                #endregion
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (flag)
                    _unitOfWork.Rollback();
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public BankLedger GetById(string id)
        {
            try
            {
                return _bankLedgerRepository.GetOne(id);
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
        public IEnumerable<BankLedger> GetAll()
        {
            try
            {
                return _bankLedgerRepository.GetAll(r => !r.Archive && r.TransactionType != TransactionType.OpeningBalance.ToString()).OrderByDescending(r => r.Sequence).AsEnumerable();
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
        public IEnumerable<BankLedger> GetAll(string companyId, string branchId)
        {
            try
            {
                return _bankLedgerRepository.GetAll(r => !r.Archive && r.TransactionType != TransactionType.OpeningBalance.ToString() && r.CompanyId == companyId && r.BranchId == branchId).OrderByDescending(r => r.Sequence).AsEnumerable();
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
        /// <param name="accountNo"></param>
        /// <returns></returns>
        public IEnumerable<BankLedger> GetAll(string companyId, string branchId, string accountNo)
        {
            try
            {
                return _bankLedgerRepository.GetAll(r => !r.Archive && r.TransactionType != TransactionType.OpeningBalance.ToString() && r.CompanyId == companyId && r.BranchId == branchId && r.AccountNo == accountNo).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public IEnumerable<BankLedger> GetAll(string companyId, string branchId, string accountNo, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _bankLedgerRepository.GetAll(r => !r.Archive && r.TransactionType != TransactionType.OpeningBalance.ToString() && r.CompanyId == companyId && r.BranchId == branchId && r.AccountNo == accountNo && r.TransactionDate >= dateFrom && r.TransactionDate <= dateTo).OrderByDescending(r => r.Sequence).AsEnumerable();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="balance"></param>
        /// <param name="overdraft"></param>
        public void GetBalanceOrOverdraftByAccountNo(string accountNo, out decimal balance, out decimal overdraft)
        {
            try
            {
                decimal localBalanceAmount = 0m;
                decimal localOverdraftAmount = 0m;
                IEnumerable<BankLedger> bankLedgerList = _bankLedgerRepository.GetAll(x => !x.Archive && x.AccountNo == accountNo).ToList();
                decimal sumOfDebitAmount = bankLedgerList.Sum(x => x.DebitAmount);
                decimal sumOfCreditAmount = bankLedgerList.Sum(x => x.CreditAmount);
                if (sumOfDebitAmount > sumOfCreditAmount)
                    localBalanceAmount = sumOfDebitAmount - sumOfCreditAmount;
                else
                    localOverdraftAmount = sumOfCreditAmount - sumOfDebitAmount;
                balance = localBalanceAmount;
                overdraft = localOverdraftAmount;
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
        public decimal GetTotalAmountDirectDepositBythePartyToTheBankByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _bankLedgerRepository.GetAll(x => !x.Archive && x.CompanyId == companyId && x.BranchId == branchId && x.TransactionDate >= dateFrom && x.TransactionDate <= dateTo && x.PaymentInfoId != null && x.TransactionType == BankTransactionType.Deposit.ToString()).Sum(x => x.DebitAmount);
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
        public decimal GetTotalAmountDepositByOwnToTheBankByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _bankLedgerRepository.GetAll(x => !x.Archive && x.CompanyId == companyId && x.BranchId == branchId && x.TransactionDate >= dateFrom && x.TransactionDate <= dateTo && x.PaymentInfoId == null && x.TransactionType == BankTransactionType.Deposit.ToString()).Sum(x => x.DebitAmount);
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
        public decimal GetTotalAmountWithdrawnByOwnFromTheBankByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo)
        {
            try
            {
                return _bankLedgerRepository.GetAll(x => !x.Archive && x.CompanyId == companyId && x.BranchId == branchId && x.TransactionDate >= dateFrom && x.TransactionDate <= dateTo && x.PaymentInfoId == null && x.TransactionType == BankTransactionType.Withdrawn.ToString()).Sum(x => x.CreditAmount);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
