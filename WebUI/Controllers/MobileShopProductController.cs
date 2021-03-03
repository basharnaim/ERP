using AutoMapper;
using ERP.WebUI.ReportViewer;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
{
    public class MobileShopProductController : BaseController
    {
        #region Ctor
        private readonly IProductService _productService;
        public MobileShopProductController(
            IProductService productService
            )
        {
            _productService = productService;
        }
        #endregion

        #region Get
        public ActionResult Index(string ProductCategoryId, string ProductSubCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(ProductCategoryId) && !string.IsNullOrEmpty(ProductSubCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(ProductCategoryId, ProductSubCategoryId)));
                if (!string.IsNullOrEmpty(ProductCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(ProductCategoryId)));
                return View();
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
                return View(new ProductViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(ProductViewModel Productvm)
        {
            try
            {
                _productService.Add(Mapper.Map<Product>(Productvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/MobileShopProduct/?ProductCategoryId=" + Productvm.ProductCategoryId + "&&ProductSubCategoryId=" + Productvm.ProductSubCategoryId}')");
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
                return View(Mapper.Map<ProductViewModel>(_productService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ProductViewModel Productvm)
        {
            try
            {
                _productService.Update(Mapper.Map<Product>(Productvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/MobileShopProduct/?ProductCategoryId=" + Productvm.ProductCategoryId + "&&ProductSubCategoryId=" + Productvm.ProductSubCategoryId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Reports
        public ActionResult ProductList(string ProductCategoryId = "", string ProductSubCategoryId = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(ProductCategoryId) && !string.IsNullOrEmpty(ProductSubCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(ProductCategoryId, ProductSubCategoryId)));
                if (!string.IsNullOrEmpty(ProductCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(ProductCategoryId)));
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcReportProductList(string ProductCategoryId = "", string ProductSubCategoryId = "")
        {
            try
            {
                if (ProductCategoryId == "null")
                {
                    ProductCategoryId = "";
                }
                if (ProductSubCategoryId == "null")
                {
                    ProductSubCategoryId = "";
                }
                IEnumerable<ProductViewModelForReport> Products;
                if (!string.IsNullOrEmpty(ProductCategoryId) && !string.IsNullOrEmpty(ProductSubCategoryId))
                    Products = Mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll(ProductCategoryId, ProductSubCategoryId));
                else if (!string.IsNullOrEmpty(ProductCategoryId))
                    Products = Mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll(ProductCategoryId));
                else
                    Products = Mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll());
                ReportDataSource rpt = new ReportDataSource("Product", Products);
                RdlcReportViewerWithoutDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptMobileShopProductList.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithoutDate.aspx?rPath=" + rPath);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
