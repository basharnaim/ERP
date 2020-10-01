#region Using

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using ERP.WebUI.ReportViewer;
using Library.Model.Inventory.Expenditures;
using Library.Context.Repositories;
using Library.Service.Inventory.Expenditures;
using Library.ViewModel.Inventory.Expenditures;
using Microsoft.Reporting.WebForms;

#endregion

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ExpenditureController : BaseController
    {
        #region Ctor
        private readonly IExpenditureService _expenditureService;
        private readonly IRawSqlService _rawSqlService;

        public ExpenditureController(
            IExpenditureService expenditureService,
            IRawSqlService rawSqlService
            )
        {
            _expenditureService = expenditureService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        #region Get
        public ActionResult Index(string companyId, string branchId, string expenditureCategoryId, string dateFrom, string dateTo)
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
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(expenditureCategoryId) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ExpenditureViewModel>>(_expenditureService.GetAll(companyId, branchId, expenditureCategoryId, dfrom.Value, dto.Value)));
                }

                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(expenditureCategoryId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ExpenditureViewModel>>(_expenditureService.GetAll(companyId, branchId, expenditureCategoryId)));
                }
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ExpenditureViewModel>>(_expenditureService.GetAll(companyId, branchId)));
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ExpenditureViewModel>>(_expenditureService.GetAll(companyId)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        #endregion

        #region JSon
        public JsonResult GetExpenditureList()
        {
            try
            {
                return Json(new SelectList(_expenditureService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new ExpenditureViewModel { Active=true, Sequence= _expenditureService.GetAutoSequence() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(ExpenditureViewModel expenditurevm)
        {
            try
            {
                _expenditureService.Add(AutoMapperConfiguration.mapper.Map<Expenditure>(expenditurevm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/Expenditure/?companyId=" + expenditurevm.CompanyId + "&&branchId=" + expenditurevm.BranchId}')");
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
                return View(AutoMapperConfiguration.mapper.Map<ExpenditureViewModel>(_expenditureService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ExpenditureViewModel expenditurevm)
        {
            try
            {
                _expenditureService.Update(AutoMapperConfiguration.mapper.Map<Expenditure>(expenditurevm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Expenditure/?companyId=" + expenditurevm.CompanyId + "&&branchId=" + expenditurevm.BranchId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Report
        public ActionResult ExpenditureReport(string companyId, string branchId, string dateFrom, string dateTo, string expenditureCategoryId)
        {
            try
            {
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
                if (!string.IsNullOrEmpty(companyId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ExpenditureViewModel>>(_rawSqlService.GetExpenses(companyId, branchId, dateFrom, dateTo, expenditureCategoryId)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReportExpenditureList(string companyId, string branchId, string expenditureCategoryId, string dateFrom, string dateTo)
        {
            try
            {
                if (companyId == "null")
                {
                    companyId = "";
                }
                if (branchId == "null")
                {
                    branchId = "";
                }
                if (expenditureCategoryId == "null")
                {
                    expenditureCategoryId = "";
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
                IEnumerable<ExpenditureViewModel> expenses = new List<ExpenditureViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    expenses = AutoMapperConfiguration.mapper.Map<IEnumerable<ExpenditureViewModel>>(_rawSqlService.GetExpenses(companyId, branchId, dateFrom, dateTo, expenditureCategoryId));
                }
                ReportDataSource rpt = new ReportDataSource("Expenditure", expenses);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptExpenses.rdlc";
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
