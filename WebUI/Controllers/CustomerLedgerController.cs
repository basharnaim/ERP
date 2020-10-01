using ERP.WebUI.ReportViewer;
using Library.Model.Inventory.Accounts;
using Library.Context.Repositories;
using Library.Service.Inventory.Accounts;
using Library.Service.Inventory.Customers;
using Library.ViewModel.Inventory.Accounts;
using Library.ViewModel.Inventory.Customers;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
{
    public class CustomerLedgerController : BaseController
    {
        #region Ctor
        private readonly ICustomerLedgerService _customerLedgerService;
        private readonly ICustomerService _customerService;
        private readonly IRawSqlService _rawSqlService;

        public CustomerLedgerController(
            ICustomerLedgerService customerLedgerService,
            ICustomerService customerService,
            IRawSqlService rawSqlService
            )
        {
            _customerLedgerService = customerLedgerService;
            _customerService = customerService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        #region Get
        public ActionResult Index(string companyId, string branchId, string dateFrom, string dateTo, string phone, string customerId)
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
                CustomerViewModel customervm = new CustomerViewModel();
                List<CustomerLedgerViewModel> customerLedger = new List<CustomerLedgerViewModel>();
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    customervm = AutoMapperConfiguration.mapper.Map<CustomerViewModel>(_customerService.GetCustomerByMobileNumberWithCompanyBranchId(phone, companyId, branchId));
                    customerLedger = AutoMapperConfiguration.mapper.Map<List<CustomerLedgerViewModel>>(_rawSqlService.GetAllCustomerLedger(companyId, branchId, dfrom.ToString(), dto.ToString(), "", phone, "").ToList());
                }
                else if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    customervm = AutoMapperConfiguration.mapper.Map<CustomerViewModel>(_customerService.GetCustomerByIdWithCompanyBranchId(customerId, companyId, branchId));
                    customerLedger = AutoMapperConfiguration.mapper.Map<List<CustomerLedgerViewModel>>(_rawSqlService.GetAllCustomerLedger(companyId, branchId, dfrom.ToString(), dto.ToString(), "", "", customerId).ToList());
                }
                else if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(phone))
                {
                    customervm = AutoMapperConfiguration.mapper.Map<CustomerViewModel>(_customerService.GetCustomerByMobileNumberWithCompanyBranchId(phone, companyId, branchId));
                    customerLedger = AutoMapperConfiguration.mapper.Map<List<CustomerLedgerViewModel>>(_rawSqlService.GetAllCustomerLedger(companyId, branchId, "", "", "", phone, "").ToList());
                }
                else if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(customerId))
                {
                    customervm = AutoMapperConfiguration.mapper.Map<CustomerViewModel>(_customerService.GetCustomerByIdWithCompanyBranchId(customerId, companyId, branchId));
                    customerLedger = AutoMapperConfiguration.mapper.Map<List<CustomerLedgerViewModel>>(_rawSqlService.GetAllCustomerLedger(companyId, branchId, "", "", "", "", customerId).ToList());
                }
                return View(new Tuple<IEnumerable<CustomerLedgerViewModel>, CustomerViewModel>(customerLedger, customervm));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public JsonResult GetCustomerLedgerList(string branchId)
        {
            try
            {
                return Json(new SelectList(_customerLedgerService.Lists(branchId), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create(string companyId, string branchId, string Phone1)
        {
            try
            {
                CustomerLedgerViewModel customerLedgervm = new CustomerLedgerViewModel();
                if (string.IsNullOrEmpty(Phone1))
                {
                    customerLedgervm.Active = true;
                    customerLedgervm.CompanyId = companyId;
                    customerLedgervm.BranchId = branchId;
                    customerLedgervm.Sequence = _customerLedgerService.GetAutoSequence();
                    return View(customerLedgervm);
                }

                var customer = _customerService.GetCustomerByMobileNumberWithCompanyBranchId(Phone1, companyId, branchId);
                if (customer != null)
                {
                    customerLedgervm.CustomerId = customer.Id;
                    customerLedgervm.CustomerName = customer.Name;
                    customerLedgervm.CustomerAddress = customer.Address1;
                    
                }
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                _customerLedgerService.GetDueOrAdvanceAmountByCustomerPhone(Phone1, out dueAmount, out advanceAmount);
                if (dueAmount > 0)
                {
                    customerLedgervm.DueAmount = dueAmount;
                }
                if (advanceAmount > 0)
                {
                    customerLedgervm.AdvanceAmount = advanceAmount;
                }
                customerLedgervm.Sequence = _customerLedgerService.GetAutoSequence();
                return View(customerLedgervm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(CustomerLedgerViewModel customerLedgervm)
        {
            try
            {
                _customerLedgerService.Add(AutoMapperConfiguration.mapper.Map<CustomerLedger>(customerLedgervm));
                CustomerLedgerViewModel vm = new CustomerLedgerViewModel();
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/CustomerLedger?companyId=" + customerLedgervm.CompanyId + "&&branchId=" + customerLedgervm.BranchId}')");
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
                CustomerLedgerViewModel customerLedgervm = AutoMapperConfiguration.mapper.Map<CustomerLedgerViewModel>(_customerLedgerService.GetById(id));
                var customer = _customerService.GetById(customerLedgervm.CustomerId);
                customerLedgervm.CustomerName = customer.Name;
                customerLedgervm.CustomerMobileNumber = customer.Phone1;
                customerLedgervm.CustomerAddress = customer.Address1;
                
                decimal dueAmount = 0m;
                decimal advanceAmount = 0m;
                _customerLedgerService.GetDueOrAdvanceAmountByCustomerPhone(customer.Phone1, out dueAmount, out advanceAmount);
                if (dueAmount > 0)
                {
                    customerLedgervm.DueAmount = dueAmount;
                }
                if (advanceAmount > 0)
                {
                    customerLedgervm.AdvanceAmount = advanceAmount;
                }
                return View(customerLedgervm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(CustomerLedgerViewModel customerLedgervm)
        {
            try
            {
                _customerLedgerService.Update(AutoMapperConfiguration.mapper.Map<CustomerLedgerViewModel, CustomerLedger>(customerLedgervm));
                CustomerLedgerViewModel vm = new CustomerLedgerViewModel();
                return JavaScript(
                    $"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/CustomerLedger?companyId=" + customerLedgervm.CompanyId + "&&branchId=" + customerLedgervm.BranchId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Report

        public ActionResult RdlcReportCustomerLedger(string companyId, string branchId, string dateFrom, string dateTo, string phone, string customerId)
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
                List<CustomerViewModel> customervm = new List<CustomerViewModel>();
                List<CustomerLedgerViewModel> customerLedger = new List<CustomerLedgerViewModel>();
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(phone) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    customervm = AutoMapperConfiguration.mapper.Map<List<CustomerViewModel>>(_customerService.GetAllByMobile(phone, companyId, branchId)).ToList();
                    customerLedger = AutoMapperConfiguration.mapper.Map<List<CustomerLedgerViewModel>>(_rawSqlService.GetAllCustomerLedger(companyId, branchId, dfrom.ToString(), dto.ToString(), "", phone, "").ToList());
                }
                else if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(customerId) && !string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
                {
                    customervm = AutoMapperConfiguration.mapper.Map<List<CustomerViewModel>>(_customerService.GetAllById(customerId, companyId, branchId));
                    customerLedger = AutoMapperConfiguration.mapper.Map<List<CustomerLedgerViewModel>>(_rawSqlService.GetAllCustomerLedger(companyId, branchId, dfrom.ToString(), dto.ToString(), "", "", customerId).ToList());
                }
                else if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(phone))
                {
                    customervm = AutoMapperConfiguration.mapper.Map<List<CustomerViewModel>>(_customerService.GetAllByMobile(phone, companyId, branchId)).ToList();
                    customerLedger = AutoMapperConfiguration.mapper.Map<List<CustomerLedgerViewModel>>(_rawSqlService.GetAllCustomerLedger(companyId, branchId, "", "", "", phone, "").ToList());
                }
                else if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(customerId))
                {
                    customervm = AutoMapperConfiguration.mapper.Map<List<CustomerViewModel>>(_customerService.GetAllById(customerId, companyId, branchId));
                    customerLedger = AutoMapperConfiguration.mapper.Map<List<CustomerLedgerViewModel>>(_rawSqlService.GetAllCustomerLedger(companyId, branchId, "", "", "", "", customerId).ToList());
                }
                ReportDataSource rpt1 = new ReportDataSource("Customer", customervm);
                ReportDataSource rpt2 = new ReportDataSource("CustomerLedger", customerLedger);
                List<ReportDataSource> rptl = new List<ReportDataSource> { rpt1, rpt2 };
                RdlcReportViewerMultipleDataSet.reportDataSource = rptl;
                string rPath = "RdlcReport/RptCustomerLedger.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerMultipleDataSet.aspx?rPath=" + rPath + "&companyId=" + companyId + "&branchId=" + branchId);
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
