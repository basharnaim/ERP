#region Using
using Library.Model.Inventory.Accounts;
using Library.Service.Core.Core;
using System;
using System.Collections.Generic;

#endregion

namespace Library.Service.Inventory.Accounts
{
    public interface IBankLedgerService : IService<BankLedger>
    {
        void Add(BankLedger bankLedger, decimal amount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bankLedger"></param>
        /// <param name="amount"></param>
        void AddOpeningBalance(BankLedger bankLedger, decimal amount);

        /// <summary>
        /// Updates the specified BankLedger.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <param name="BankLedger">The BankLedger.</param>
        void Update(BankLedger bankLedger, decimal amount);

        /// <summary>
        /// Gets the by identifier.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>
        /// <param name="id">The identifier.</param>
        /// <returns>BankLedger.</returns>
        BankLedger GetById(string id);

        /// <summary>
        /// Gets all.
        /// </summary>
        /// <remarks>Jahangir, 2-11-2015</remarks>.
        /// <returns>IEnumerable&lt;BankLedger&gt;.</returns>
        IEnumerable<BankLedger> GetAll();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <returns></returns>
        IEnumerable<BankLedger> GetAll(string companyId, string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="accountNo"></param>
        /// <returns></returns>
        IEnumerable<BankLedger> GetAll(string companyId, string branchId, string accountNo);
        IEnumerable<BankLedger> GetAll(string companyId, string branchId, string accountNo, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="accountNo"></param>
        /// <param name="balance"></param>
        /// <param name="overdraft"></param>
        void GetBalanceOrOverdraftByAccountNo(string accountNo, out decimal balance, out decimal overdraft);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        decimal GetTotalAmountDirectDepositBythePartyToTheBankByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        decimal GetTotalAmountDepositByOwnToTheBankByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="branchId"></param>
        /// <param name="dateFrom"></param>
        /// <param name="dateTo"></param>
        /// <returns></returns>
        decimal GetTotalAmountWithdrawnByOwnFromTheBankByCompanyBranchIdWithDateRange(string companyId, string branchId, DateTime dateFrom, DateTime dateTo);
    }
}
