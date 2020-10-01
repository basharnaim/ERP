using ERP.WebUI.Controllers;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ProductCategoryController : BaseController
    {
        #region Ctor
        private readonly IProductCategoryService _productCategoryService;
        public ProductCategoryController(
            IProductCategoryService productCategoryService
            )
        {
            _productCategoryService = productCategoryService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductCategoryViewModel>>(_productCategoryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetProductCategoryList()
        {
            try
            {
                return Json(new SelectList(_productCategoryService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new ProductCategoryViewModel { Active = true, Sequence = _productCategoryService.GetAutoSequence() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(ProductCategoryViewModel productCategoryVm)
        {
            try
            {
                _productCategoryService.Add(AutoMapperConfiguration.mapper.Map<ProductCategory>(productCategoryVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductCategory"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<ProductCategoryViewModel>(_productCategoryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ProductCategoryViewModel productCategoryvm)
        {
            try
            {
                _productCategoryService.Update(AutoMapperConfiguration.mapper.Map<ProductCategory>(productCategoryvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductCategory"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Upload
        [HttpGet]
        [Authorize]
        public ActionResult Upload()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    var result = _productCategoryService.UploadFromDirectory(file);
                    WriteExcelFile(result, "Error");
                    return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductCategory"}')");
                }
                return JavaScript($"ShowResult('Please select file','failure')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
