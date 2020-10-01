using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
{
    public class SuperShopProductController : BaseController
    {
        #region Ctor
        private readonly IProductService _productService;
        public SuperShopProductController(IProductService productService)
        {
            _productService = productService;
        }
        #endregion

        #region Get
        public ActionResult Index(string productCategoryId, string productSubCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(productCategoryId) && !string.IsNullOrEmpty(productSubCategoryId))
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(productCategoryId, productSubCategoryId)));
                if (!string.IsNullOrEmpty(productCategoryId))
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(productCategoryId)));
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetProductList()
        {
            return Json(new SelectList(_productService.Lists(), "Id", "Code"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new ProductViewModel { Active = true, Id = _productService.GenerateAutoId() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(ProductViewModel productVm)
        {
            try
            {
                _productService.Add(AutoMapperConfiguration.mapper.Map<Product>(productVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/SuperShopProduct/?productCategoryId=" + productVm.ProductCategoryId + "&&productSubCategoryId=" + productVm.ProductSubCategoryId}')");
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
                return View(AutoMapperConfiguration.mapper.Map<ProductViewModel>(_productService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ProductViewModel productVm)
        {
            try
            {
                _productService.Update(AutoMapperConfiguration.mapper.Map<Product>(productVm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/SuperShopProduct/?productCategoryId=" + productVm.ProductCategoryId + "&&productSubCategoryId=" + productVm.ProductSubCategoryId}')");

            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Get Image
        public FileContentResult GetPicture(string id)
        {
            var byteArray = _productService.GetById(id)?.Picture;
            return byteArray != null
                ? new FileContentResult(byteArray, MediaTypeNames.Image.Jpeg)
                : null;
        }
        #endregion
    }
}
