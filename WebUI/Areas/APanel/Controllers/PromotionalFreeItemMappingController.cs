using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using ERP.WebUI.Controllers;
using Library.Service.Inventory.Customers;
using Library.Service.Inventory.Promotions;
using Library.ViewModel.Inventory.Customers;
using Library.ViewModel.Inventory.Promotions;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class PromotionalFreeItemMappingController : BaseController
    {
        #region Constractor
        private readonly IPromotionalFreeItemMappingService _promotionalFreeItemMappingService;
        private readonly ICustomerService _customerService;
        public PromotionalFreeItemMappingController(
            IPromotionalFreeItemMappingService promotionalFreeItemMappingService,
            ICustomerService customerService
        )
        {
            _promotionalFreeItemMappingService = promotionalFreeItemMappingService;
            _customerService = customerService;
        }
        #endregion

        #region Get
        [Authorize]
        public ActionResult Index(string promotionalFreeItemId)
        {
            try
            {
                return !string.IsNullOrEmpty(promotionalFreeItemId) ? View(AutoMapperConfiguration.mapper.Map<IEnumerable<PromotionalFreeItemMappingViewModel>>(_promotionalFreeItemMappingService.GetAll(promotionalFreeItemId))) : View();
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }
        #endregion

        #region Json
        public ActionResult GetCustomerList(string customerCategoryId, string unitId, string regionId, string areaId, string territoryId, string customerId)
        {
            return View("_CustomerList", AutoMapperConfiguration.mapper.Map<IEnumerable<CustomerViewModel>>(_customerService.GetAllCustomerForMapping(customerCategoryId, unitId, regionId, areaId, territoryId, customerId)));
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
        public JavaScriptResult Create(string customerIdList, string promotionalFreeItemId)
        {
            try
            {
                _promotionalFreeItemMappingService.Assign(new JavaScriptSerializer().Deserialize<string[]>(customerIdList), promotionalFreeItemId);
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region Status Change
        public void ChangeStatus(string customerId, string promotionalFreeItemId)
        {
            _promotionalFreeItemMappingService.ChangeStatus(customerId, promotionalFreeItemId);
        }
        #endregion
    }
}