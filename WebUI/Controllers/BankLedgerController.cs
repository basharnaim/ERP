#region Using

using AutoMapper;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Accounts;
using Library.Service.Inventory.Accounts;
using Library.ViewModel.Inventory.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

#endregion

namespace ERP.WebUI.Controllers
{
    public class BankLedgerController : BaseController
    {
        #region Ctor
        private readonly IBankLedgerService _bankLedgerService;
        public BankLedgerController(
            IBankLedgerService bankLedgerService
            )
        {
            _bankLedgerService = bankLedgerService;
        }
        #endregion

        #region Get
        public ActionResult Index(string accountNo)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                List<BankLedgerViewModel> bankLedgerVm = new List<BankLedgerViewModel>();
                if (!string.IsNullOrEmpty(accountNo))
                    bankLedgerVm = Mapper.Map<List<BankLedgerViewModel>>(_bankLedgerService.GetAll(identity.CompanyId, identity.BranchId, accountNo).ToList());
                return View(bankLedgerVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult BankOpeningIndex(string companyId, string branchId, string accountNo)
        {
            try
            {
                List<BankLedgerViewModel> bankLedgerVm = new List<BankLedgerViewModel>();
                if (!string.IsNullOrEmpty(accountNo))
                    bankLedgerVm = Mapper.Map<List<BankLedgerViewModel>>(_bankLedgerService.GetAll(companyId, branchId, accountNo).ToList());
                return View(bankLedgerVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                BankLedgerViewModel bankLedgerVm = new BankLedgerViewModel
                {
                    TransactionDate = DateTime.Now
                };
                return View(bankLedgerVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(BankLedgerViewModel bankLedgerVm)
        {
            try
            {
                if (bankLedgerVm.Amount > 0)
                {
                    _bankLedgerService.Add(Mapper.Map<BankLedger>(bankLedgerVm), bankLedgerVm.Amount);
                    return JavaScript(
                        $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/BankLedger?accountNo=" + bankLedgerVm.AccountNo}')");
                }

                return JavaScript($"ShowResult('{"Amount 0 is not valid value!"}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Edit
        [HttpGet]
        public ActionResult Edit(string id)
        {
            try
            {
                BankLedgerViewModel bankLedgerVm = Mapper.Map<BankLedgerViewModel>(_bankLedgerService.GetById(id));
                if (bankLedgerVm.DebitAmount > 0)
                    bankLedgerVm.Amount = bankLedgerVm.DebitAmount;
                else
                    bankLedgerVm.Amount = bankLedgerVm.CreditAmount;
                return View(bankLedgerVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(BankLedgerViewModel bankLedgerVm)
        {
            try
            {
                _bankLedgerService.Update(Mapper.Map<BankLedger>(bankLedgerVm), bankLedgerVm.Amount);
                return JavaScript(
                    $"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/BankLedger?accountNo=" + bankLedgerVm.AccountNo}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region BankOpeningBalance
        [HttpGet]
        public ActionResult BankOpeningBalance(string id)
        {
            try
            {
                BankLedgerViewModel bankLedgerVm = new BankLedgerViewModel();
                if (!string.IsNullOrEmpty(id))
                {
                    BankLedger bankLedger = _bankLedgerService.GetById(id);
                    bankLedgerVm.Id = bankLedger.Id;
                    bankLedgerVm.CompanyId = bankLedger.CompanyId;
                    bankLedgerVm.BranchId = bankLedger.BranchId;
                    bankLedgerVm.BankId = bankLedger.BankId;
                    bankLedgerVm.AccountNo = bankLedger.AccountNo;
                    bankLedgerVm.Amount = bankLedger.DebitAmount;
                    bankLedgerVm.TransactionType = bankLedger.TransactionType;
                    bankLedgerVm.Particulars = bankLedger.Particulars;
                    bankLedgerVm.TransactionDate = bankLedger.TransactionDate;
                    return View(bankLedgerVm);
                }

                bankLedgerVm.TransactionDate = DateTime.Now;
                bankLedgerVm.TransactionType = BankTransactionType.OpeningBalance.ToString();
                return View(bankLedgerVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult BankOpeningBalance(BankLedgerViewModel bankLedgerVm)
        {
            try
            {
                if (bankLedgerVm.Amount > 0)
                {
                    BankLedger bankLedger = Mapper.Map<BankLedger>(bankLedgerVm);
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                    bankLedgerVm.CompanyId = identity.CompanyId;
                    bankLedgerVm.BranchId = identity.BranchId;
                    _bankLedgerService.AddOpeningBalance(bankLedger, bankLedgerVm.Amount);
                    bankLedgerVm.Id = bankLedger.Id;
                    return JavaScript(
                        $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/BankLedger/BankOpeningBalance?id=" + bankLedgerVm.Id}')");
                }

                return JavaScript($"ShowResult('{"Amount 0 is not valid value!"}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        #endregion

        #region BankOpeningBalanceEdit
        [HttpGet]
        public ActionResult BankOpeningBalanceEdit(string id)
        {
            try
            {
                return View(Mapper.Map<BankLedgerViewModel>(_bankLedgerService.GetById(id)));

            }

            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult BankOpeningBalanceEdit(BankLedgerViewModel bankLedgerVm)
        {
            try
            {
                if (bankLedgerVm.Amount > 0)
                {
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                    bankLedgerVm.CompanyId = identity.CompanyId;
                    bankLedgerVm.BranchId = identity.BranchId;
                    _bankLedgerService.Add(Mapper.Map<BankLedger>(bankLedgerVm), bankLedgerVm.Amount);
                    return JavaScript(
                        $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/BankLedger/BankOpeningIndex?accountNo=" + bankLedgerVm.AccountNo}')");
                }

                return JavaScript($"ShowResult('{"Amount 0 is not valid value!"}','{"failure"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
