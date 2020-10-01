#region Using

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using ERP.WebUI.ReportViewer;
using Library.Crosscutting.Securities;
using Library.Model.Core.Organizations;
using Library.Model.Inventory.Accounts;
using Library.Context.Repositories;
using Library.Service.Inventory.Accounts;
using Library.ViewModel.Inventory.Accounts;
using Microsoft.Reporting.WebForms;

#endregion

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class BankLedgerController : BaseController
    {
        #region Ctor
        private readonly IBankLedgerService _bankLedgerService;
        private readonly IRawSqlService _rawSqlService;
        public BankLedgerController(
            IBankLedgerService bankLedgerService,
            IRawSqlService rawSqlService
            )
        {
            _bankLedgerService = bankLedgerService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        #region Get
        public ActionResult Index(string companyId, string branchId, string accountNo, string dateFrom, string dateTo)
        {
            try
            {
                DateTime? dfrom = null;
                DateTime? dto = null;
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                }
                List<BankLedgerViewModel> bankLedgerVm = new List<BankLedgerViewModel>();
                if (!string.IsNullOrEmpty(accountNo) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo) )
                    bankLedgerVm = AutoMapperConfiguration.mapper.Map<List<BankLedgerViewModel>>(_bankLedgerService.GetAll(companyId, branchId, accountNo, dfrom.Value, dto.Value).ToList());
                if (!string.IsNullOrEmpty(accountNo))
                    bankLedgerVm = AutoMapperConfiguration.mapper.Map<List<BankLedgerViewModel>>(_bankLedgerService.GetAll(companyId, branchId, accountNo).ToList());
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
                    bankLedgerVm = AutoMapperConfiguration.mapper.Map<List<BankLedgerViewModel>>(_bankLedgerService.GetAll(companyId, branchId, accountNo).ToList());
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
                    _bankLedgerService.Add(AutoMapperConfiguration.mapper.Map<BankLedger>(bankLedgerVm), bankLedgerVm.Amount);
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/BankLedger?accountNo=" + bankLedgerVm.AccountNo}')");
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
                BankLedgerViewModel bankLedgerVm = AutoMapperConfiguration.mapper.Map<BankLedgerViewModel>(_bankLedgerService.GetById(id));
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
                _bankLedgerService.Update(AutoMapperConfiguration.mapper.Map<BankLedger>(bankLedgerVm), bankLedgerVm.Amount);
                return JavaScript(
                    $"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/BankLedger?accountNo=" + bankLedgerVm.AccountNo}')");
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
                    BankLedger bankLedger = AutoMapperConfiguration.mapper.Map<BankLedger>(bankLedgerVm);
                    var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                    bankLedgerVm.CompanyId = identity.CompanyId;
                    bankLedgerVm.BranchId = identity.BranchId;
                    _bankLedgerService.AddOpeningBalance(bankLedger, bankLedgerVm.Amount);
                    bankLedgerVm.Id = bankLedger.Id;
                    return JavaScript(
                        $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/BankLedger/BankOpeningBalance?id=" + bankLedgerVm.Id}')");
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
                return View(AutoMapperConfiguration.mapper.Map<BankLedgerViewModel>(_bankLedgerService.GetById(id)));

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
                    _bankLedgerService.Add(AutoMapperConfiguration.mapper.Map<BankLedger>(bankLedgerVm), bankLedgerVm.Amount);
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


        #region Rdlc Report
        public ActionResult ReportBankLedger(string companyId, string branchId, string accountNo, string dateFrom, string dateTo)
        {
            try
            {
                DateTime? dfrom = null;
                DateTime? dto = null;
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                }
                List<BankLedgerViewModel> bankLedgerVm = new List<BankLedgerViewModel>();
                if (!string.IsNullOrEmpty(accountNo) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                    bankLedgerVm = AutoMapperConfiguration.mapper.Map<List<BankLedgerViewModel>>(_bankLedgerService.GetAll(companyId, branchId, accountNo, dfrom.Value, dto.Value).ToList());
                if (!string.IsNullOrEmpty(accountNo))
                    bankLedgerVm = AutoMapperConfiguration.mapper.Map<List<BankLedgerViewModel>>(_bankLedgerService.GetAll(companyId, branchId, accountNo).ToList());
                return View(bankLedgerVm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult RdlcReportBankLedger(string companyId, string branchId, string accountNo, string dateFrom, string dateTo)
        {
            try
            {
                if (companyId == "null")
                {
                    companyId = "";
                }
                if (accountNo == "null")
                {
                    accountNo = "";
                }
                if (dateFrom == "null")
                {
                    dateFrom = "";
                }
                if (dateTo == "null")
                {
                    dateTo = "";
                }
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    DateTime? dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                    dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    DateTime? dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                    dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
                }
                IEnumerable<BankLedgerViewModel> bankLedgerList = new List<BankLedgerViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    bankLedgerList = AutoMapperConfiguration.mapper.Map<IEnumerable<BankLedgerViewModel>>(_rawSqlService.GetAllBankLedger(companyId, branchId,accountNo ,dateFrom, dateTo));
                }
                ReportDataSource rpt = new ReportDataSource("BankLedger", bankLedgerList);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptBankLedger.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
