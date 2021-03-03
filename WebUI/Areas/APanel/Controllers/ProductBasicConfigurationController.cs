using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ProductBasicConfigurationController : Controller
    {
        #region Ctor
        private readonly IProductBasicConfigurationService _configurationSettingService;
        public ProductBasicConfigurationController(
            IProductBasicConfigurationService configurationSettingService
            )
        {
            _configurationSettingService = configurationSettingService;
        }
        #endregion

        #region Get
        [Authorize]
        public ActionResult Index(string productBasicConfigurationCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(productBasicConfigurationCategoryId))
                {
                    return View(Mapper.Map<IEnumerable<ProductBasicConfigurationViewModel>>(_configurationSettingService.GetAll(productBasicConfigurationCategoryId)));
                }
                return View(Mapper.Map<IEnumerable<ProductBasicConfigurationViewModel>>(_configurationSettingService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetConfigurationSettingList()
        {
            try
            {
                return Json(new SelectList(_configurationSettingService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new ProductBasicConfigurationViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Create(ProductBasicConfigurationViewModel configurationSetting)
        {
            try
            {
                _configurationSettingService.Add(Mapper.Map<ProductBasicConfiguration>(configurationSetting));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductBasicConfiguration"}')");
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
                return View(Mapper.Map<ProductBasicConfigurationViewModel>(_configurationSettingService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Edit(ProductBasicConfigurationViewModel configurationSetting)
        {
            try
            {
                _configurationSettingService.Update(Mapper.Map<ProductBasicConfiguration>(configurationSetting));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductBasicConfiguration"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}