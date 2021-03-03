using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ProductSubsidiaryCategoryController : BaseController
    {
        private readonly IProductSubsidiaryCategoryService _productSubsidiaryCategoryService;
        public ProductSubsidiaryCategoryController(
            IProductSubsidiaryCategoryService productSubsidiaryCategoryService
            )
        {
            _productSubsidiaryCategoryService = productSubsidiaryCategoryService;
        }

        #region Get

        public ActionResult Index(string productCategoryId, string productSubCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(productCategoryId) && !string.IsNullOrEmpty(productSubCategoryId))
                {
                    return View(Mapper.Map<IEnumerable<ProductSubsidiaryCategoryViewModel>>(_productSubsidiaryCategoryService.GetAll(productCategoryId, productSubCategoryId)));
                }
                if (!string.IsNullOrEmpty(productSubCategoryId))
                {
                    return View(Mapper.Map<IEnumerable<ProductSubsidiaryCategoryViewModel>>(_productSubsidiaryCategoryService.GetAll(productSubCategoryId)));
                }
                return View(Mapper.Map<IEnumerable<ProductSubsidiaryCategoryViewModel>>(_productSubsidiaryCategoryService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }


        #endregion

        #region JSon

        public JsonResult GetProductSubsidiaryCategoryList(string productSubCategoryId)
        {
            try
            {
                return Json(new SelectList(_productSubsidiaryCategoryService.Lists(productSubCategoryId), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new ProductSubsidiaryCategoryViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(ProductSubsidiaryCategoryViewModel productSubsidiaryCategoryvm)
        {
            try
            {
                _productSubsidiaryCategoryService.Add(Mapper.Map<ProductSubsidiaryCategory>(productSubsidiaryCategoryvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductSubsidiaryCategory?productCategoryId=" + productSubsidiaryCategoryvm.ProductCategoryId + "&&productSubCategoryId=" + productSubsidiaryCategoryvm.ProductSubCategoryId }')");
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
                return View(Mapper.Map<ProductSubsidiaryCategoryViewModel>(_productSubsidiaryCategoryService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ProductSubsidiaryCategoryViewModel productSubsidiaryCategoryvm)
        {
            try
            {
                _productSubsidiaryCategoryService.Update(Mapper.Map<ProductSubsidiaryCategory>(productSubsidiaryCategoryvm));
                return JavaScript($"ShowResult('{"Data Updated successfully."}','{"success"}','{"redirect"}','{"/APanel/ProductSubsidiaryCategory?productCategoryId=" + productSubsidiaryCategoryvm.ProductCategoryId + "&&productSubCategoryId=" + productSubsidiaryCategoryvm.ProductSubCategoryId }')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
