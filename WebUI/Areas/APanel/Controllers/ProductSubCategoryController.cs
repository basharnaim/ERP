using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ProductSubCategoryController : BaseController
    {
        private readonly IProductSubCategoryService _productSubCategoryService;
        public ProductSubCategoryController(
            IProductSubCategoryService productSubCategoryService
            )
        {
            _productSubCategoryService = productSubCategoryService;
        }

        #region Get
        public ActionResult Index(string productCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(productCategoryId))
                {
                    return View(Mapper.Map<IEnumerable<ProductSubCategoryViewModel>>(_productSubCategoryService.GetAll(productCategoryId)));
                }
                return View(Mapper.Map<IEnumerable<ProductSubCategoryViewModel>>(_productSubCategoryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetProductSubCategoryList(string productCategoryId)
        {
            try
            {
                return Json(new SelectList(_productSubCategoryService.Lists(productCategoryId), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public JsonResult GetProductSubCategory_List() 
        {
            try
            {
                return Json(new SelectList(_productSubCategoryService.GetAllObject(), "Value", "Text"), JsonRequestBehavior.AllowGet); 
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
                return View(new ProductSubCategoryViewModel { Active = true, Sequence = _productSubCategoryService.GetAutoSequence() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(ProductSubCategoryViewModel productSubCategoryvm)
        {
            try
            {
                _productSubCategoryService.Add(Mapper.Map<ProductSubCategory>(productSubCategoryvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductSubCategory/?productCategoryId=" + productSubCategoryvm.ProductCategoryId}')");
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
                return View(Mapper.Map<ProductSubCategoryViewModel>(_productSubCategoryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ProductSubCategoryViewModel productSubCategoryvm)
        {
            try
            {
                _productSubCategoryService.Update(Mapper.Map<ProductSubCategory>(productSubCategoryvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductSubCategory/?productCategoryId=" + productSubCategoryvm.ProductCategoryId}')");
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
                    var result = _productSubCategoryService.UploadFromDirectory(file);
                    WriteExcelFile(result, "Error");
                    return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductSubCategory"}')");
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
