using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Service.Inventory.Customers;
using Library.Service.Inventory.Promotions;
using Library.ViewModel.Inventory.Customers;
using Library.ViewModel.Inventory.Promotions;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class PromotionalDiscountMappingController : BaseController
    {
        #region Constractor
        private readonly IPromotionalDiscountMappingService _promotionalDiscountMappingService;
        private readonly ICustomerService _customerService;
        public PromotionalDiscountMappingController(
            IPromotionalDiscountMappingService promotionalDiscountMappingService,
            ICustomerService customerService
        )
        {
            _promotionalDiscountMappingService = promotionalDiscountMappingService;
            _customerService = customerService;
        }
        #endregion

        #region Get
        [Authorize]
        public ActionResult Index(string promotionalDiscountId)
        {
            try
            {
                return !string.IsNullOrEmpty(promotionalDiscountId) ? View(Mapper.Map<IEnumerable<PromotionalDiscountMappingViewModel>>(_promotionalDiscountMappingService.GetAll(promotionalDiscountId))) : View();
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }
        #endregion

        #region Partial View
        public ActionResult GetCustomerList(string customerCategoryId, string unitId, string regionId, string areaId, string territoryId, string customerId)
        {
            return View("_CustomerList", Mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAllCustomerForMapping(customerCategoryId, unitId, regionId, areaId, territoryId, customerId)));
        }
        #endregion


        #region Assign
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }

        [HttpPost]
        public JavaScriptResult Create(string customerIdList, string promotionalDiscountId)
        {
            try
            {
                _promotionalDiscountMappingService.Assign(new JavaScriptSerializer().Deserialize<string[]>(customerIdList), promotionalDiscountId);
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region Status Change
        public void ChangeStatus(string customerId, string promotionalDiscountId)
        {
            _promotionalDiscountMappingService.ChangeStatus(customerId, promotionalDiscountId);
        }
        #endregion
    }
}