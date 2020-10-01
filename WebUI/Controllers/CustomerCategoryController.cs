using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Model.Inventory.Customers;
using Library.Service.Inventory.Customers;
using Library.ViewModel.Inventory.Customers;

namespace ERP.WebUI.Controllers
{
    public class CustomerCategoryController : BaseController
    {
        #region Ctor
        private readonly ICustomerCategoryService _customerCategoryService;

        public CustomerCategoryController(ICustomerCategoryService customerCategoryService)
        {
            _customerCategoryService = customerCategoryService;
        }
        #endregion

        #region Get

        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<CustomerCategoryViewModel>>(_customerCategoryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }



        #endregion

        #region JSon

        public JsonResult GetCustomerCategoryList()
        {
            try
            {
                return Json(new SelectList(_customerCategoryService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new CustomerCategoryViewModel { Active = true, Sequence = _customerCategoryService.GetAutoSequence() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(CustomerCategoryViewModel customerCategoryVM)
        {
            try
            {
                _customerCategoryService.Add(AutoMapperConfiguration.mapper.Map<CustomerCategory>(customerCategoryVM));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/CustomerCategory"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<CustomerCategoryViewModel>(_customerCategoryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(CustomerCategoryViewModel customerCategoryVM)
        {
            try
            {
                _customerCategoryService.Update(AutoMapperConfiguration.mapper.Map<CustomerCategory>(customerCategoryVM));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/CustomerCategory"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        public ActionResult Delete(string id)
        {
            try
            {
                _customerCategoryService.Archive(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
    }
}
