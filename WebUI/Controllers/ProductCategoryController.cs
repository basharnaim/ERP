﻿using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

namespace ERP.WebUI.Controllers
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
                return View(Mapper.Map<IEnumerable<ProductCategoryViewModel>>(_productCategoryService.GetAll()));
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
                _productCategoryService.Add(Mapper.Map<ProductCategory>(productCategoryVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/ProductCategory"}')");
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
                return View(Mapper.Map<ProductCategoryViewModel>(_productCategoryService.GetById(id)));
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
                _productCategoryService.Update(Mapper.Map<ProductCategory>(productCategoryvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/ProductCategory"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
