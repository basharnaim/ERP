using AutoMapper;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.WebUI.Areas.CPanel.Controllers
{
    public class ProductBasicConfigurationCategoryController : Controller
    {
        #region Ctor
        private readonly IProductBasicConfigurationCategoryService _configurationCategoryService;
        public ProductBasicConfigurationCategoryController(
            IProductBasicConfigurationCategoryService configurationCategoryService
            )
        {
            _configurationCategoryService = configurationCategoryService;
        }
        #endregion

        #region Get
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                return View(Mapper.Map<IEnumerable<ProductBasicConfigurationCategoryViewModel>>(_configurationCategoryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetConfigurationCategoryList()
        {
            try
            {
                return Json(new SelectList(_configurationCategoryService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #endregion

        #region Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            try
            {
                return View(new ProductBasicConfigurationCategoryViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Create(ProductBasicConfigurationCategoryViewModel configurationCategory)
        {
            try
            {
                _configurationCategoryService.Add(Mapper.Map<ProductBasicConfigurationCategory>(configurationCategory));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/CPanel/ProductBasicConfigurationCategory"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Edit

        [HttpGet]
        [Authorize]
        public ActionResult Edit(string id)
        {
            try
            {
                return View(Mapper.Map<ProductBasicConfigurationCategoryViewModel>(_configurationCategoryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Edit(ProductBasicConfigurationCategoryViewModel configurationCategory)
        {
            try
            {
                _configurationCategoryService.Update(Mapper.Map<ProductBasicConfigurationCategory>(configurationCategory));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/CPanel/ProductBasicConfigurationCategory"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}