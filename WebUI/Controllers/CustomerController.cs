﻿using AutoMapper;
using Library.Crosscutting.Securities;
using Library.Model.Inventory.Customers;
using Library.Service.Core.Enums;
using Library.Service.Inventory.Customers;
using Library.ViewModel.Inventory.Customers;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
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
        public ActionResult Index(string customerCategoryId)
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (!string.IsNullOrEmpty(customerCategoryId))
                {
                    return View(Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll(identity.CompanyId, identity.BranchId, customerCategoryId)));
                }
                return View(Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAll(identity.CompanyId, identity.BranchId)));
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
        public ActionResult Create()
        {
            try
            {
                return View(new CustomerViewModel { Active = true });
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
                _customerService.Add(Mapper.Map<Customer>(customervm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/Customer/?companyId=" + customervm.CompanyId + "&&branchId=" + customervm.BranchId}')");
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
                return JavaScript(
                    $"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/Customer/?companyId=" + customervm.CompanyId + "&&branchId=" + customervm.BranchId}')");
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


    }
}
