using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Suppliers;
using Library.Service.Inventory.Suppliers;
using Library.ViewModel.Inventory.Suppliers;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class SupplierCategoryController : BaseController
    {
        #region Ctor
        private readonly ISupplierCategoryService _supplierCategoryService;
        public SupplierCategoryController(
            ISupplierCategoryService supplierCategoryService
            )
        {
            _supplierCategoryService = supplierCategoryService;
        }
        #endregion

        #region Get

        public ActionResult Index()
        {
            try
            {
                return View(Mapper.Map<IEnumerable<SupplierCategoryViewModel>>(_supplierCategoryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }



        #endregion

        #region JSon
        public JsonResult GetSupplierCategoryList()
        {
            try
            {
                return Json(new SelectList(_supplierCategoryService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new SupplierCategoryViewModel { Active = true,Sequence = _supplierCategoryService.GetAutoSequence() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(SupplierCategoryViewModel supplierCategoryvm)
        {
            try
            {
                _supplierCategoryService.Add(Mapper.Map<SupplierCategory>(supplierCategoryvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/SupplierCategory"}')");
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
                return View(Mapper.Map<SupplierCategory, SupplierCategoryViewModel>(_supplierCategoryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(SupplierCategoryViewModel supplierCategoryvm)
        {
            try
            {
                _supplierCategoryService.Update(Mapper.Map<SupplierCategory>(supplierCategoryvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/SupplierCategory"}')");
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
                _supplierCategoryService.Archive(id);
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
