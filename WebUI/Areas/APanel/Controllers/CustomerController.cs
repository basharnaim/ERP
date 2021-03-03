using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using AutoMapper;
using ERP.WebUI.Controllers;
using ERP.WebUI.ReportViewer;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Customers;
using Library.Service.Core.Enums;
using Library.Service.Inventory.Customers;
using Library.ViewModel.Inventory.Customers;
using Microsoft.Reporting.WebForms;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class CustomerController : BaseController
    {
        #region Ctor
        private readonly ICustomerService _customerService;
        private readonly ICustomerCategoryService _customerCategoryService;
        private readonly IEnumService _enumService;

        public CustomerController(
            ICustomerService customerService,
            ICustomerCategoryService customerCategoryService,
            IEnumService enumService
            )
        {
            _customerService = customerService;
            _customerCategoryService = customerCategoryService;
            _enumService = enumService;
        }
        #endregion

        #region Get
        public ActionResult Index(string companyId, string branchId, string customerCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(customerCategoryId))
                {
                    return View(Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll(companyId, branchId, customerCategoryId)));
                }
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    return View(Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll(companyId, branchId)));
                }
                if (!string.IsNullOrEmpty(customerCategoryId))
                {
                    return View(Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll(customerCategoryId)));
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    return View(Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll(companyId)));
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
        public JsonResult GetCustomerList()
        {
            try
            {
                return Json(new SelectList(_customerService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult GetCustomerListByBranch(string branchId)
        {
            try
            {
                return Json(new SelectList(_customerService.Lists(branchId), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult CustomerType()
        {
            try
            {
                return Json(new SelectList(_enumService.CustomerType(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public JsonResult GetCustomerCategoryDiscount(string customerCategoryId)
        {
            var discount = _customerCategoryService.GetCustomerCategoryDiscount(customerCategoryId);
            var data = new
            {
                Discount = discount
            };
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create(string id)
        {
            try
            {
                if (id != "" && id != null)
                {
                    var cstmr = Mapper.Map<CustomerViewModel>(_customerService.GetById(id));
                    return View(cstmr);
                }
                else
                {
                    return View(new CustomerViewModel { Active = true });
                }
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(CustomerViewModel customervm)
        {
            try
            {
                if(string.IsNullOrEmpty(customervm.Id))
                {
                    _customerService.Add(Mapper.Map<Customer>(customervm));
                    return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/SuperShopSale/Create"}')");
                }
                else
                {
                    _customerService.Update(Mapper.Map<Customer>(customervm));
                    return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Customer/?companyId=" + customervm.CompanyId + "&&branchId=" + customervm.BranchId}')");
                }
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
                var cstmr = Mapper.Map<CustomerViewModel>(_customerService.GetById(id));
                return View(cstmr);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(CustomerViewModel customervm)
        {
            try
            {
                _customerService.Update(Mapper.Map<Customer>(customervm));
                return JavaScript( $"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Customer/?companyId=" + customervm.CompanyId + "&&branchId=" + customervm.BranchId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Delete
        public ActionResult Delete(string id)
        {
            try
            {
                _customerService.Archive(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        #endregion
        public ActionResult RdlcReportCustomerList(string companyId, string branchId, string customerCategoryId)
        {
            try
            {               
                IEnumerable<CustomerViewModel> items = new List<CustomerViewModel>();
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                companyId = identity.CompanyId;
                branchId = identity.BranchId;
                var cName = identity.CompanyName;    
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId) && !string.IsNullOrEmpty(customerCategoryId))
                {
                    items = Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll(companyId, branchId, customerCategoryId));
                }
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    items = Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll(companyId, branchId));
                }
                if (!string.IsNullOrEmpty(customerCategoryId))
                {
                    items = Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll(customerCategoryId));
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    items = Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll(companyId));
                }
                ReportDataSource rpt = new ReportDataSource("Customer", items);
                RdlcReportViewerWithoutDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptCustomer.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithoutDate.aspx?rPath=" + rPath + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

    }
}
