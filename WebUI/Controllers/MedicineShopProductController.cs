using AutoMapper;
using ERP.WebUI.ReportViewer;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mime;
using System.Web.Helpers;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
{
    public class MedicineShopProductController : BaseController
    {
        #region Ctor
        private readonly IProductService _productService;
        public MedicineShopProductController(
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
                return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll()));
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
                return View(new ProductViewModel { Active = true, Sequence = _productService.GetAutoSequence() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(ProductViewModel ProductVm)
        {
            try
            {
                if (ProductVm.Picturep != null)
                {
                    var extension = Path.GetExtension(ProductVm.Picturep.FileName)?.Trim().ToLower();
                    if (extension == ".jpg" || extension == ".png" || extension == ".gif")
                    {
                        var reader = new BinaryReader(ProductVm.Picturep.InputStream);
                        var byteArray = reader.ReadBytes(ProductVm.Picturep.ContentLength);
                        var img = new WebImage(byteArray).Resize(300, 300, false, true);
                        ProductVm.Picture = img.GetBytes();
                    }
                    else
                        throw new Exception("Please upload .jpg, PNG, gif file only.");
                }
                _productService.Add(Mapper.Map<Product>(ProductVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/MedicineShopProduct/?ProductCategoryId=" + ProductVm.ProductCategoryId + "&&ProductSubCategoryId=" + ProductVm.ProductSubCategoryId}')");
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
        public JavaScriptResult Edit(ProductViewModel ProductVm)
        {
            try
            {
                if (ProductVm.Picturep != null)
                {
                    var extension = Path.GetExtension(ProductVm.Picturep.FileName)?.Trim().ToLower();
                    if (extension == ".jpg" || extension == ".png" || extension == ".gif")
                    {
                        var reader = new BinaryReader(ProductVm.Picturep.InputStream);
                        var byteArray = reader.ReadBytes(ProductVm.Picturep.ContentLength);
                        var img = new WebImage(byteArray).Resize(300, 300, false, true);
                        ProductVm.Picture = img.GetBytes();
                    }
                    else
                        throw new Exception("Please upload .jpg, PNG, gif file only.");
                }
                _productService.Update(Mapper.Map<Product>(ProductVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/MedicineShopProduct/?ProductCategoryId=" + ProductVm.ProductCategoryId + "&&ProductSubCategoryId=" + ProductVm.ProductSubCategoryId}')");
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

        #region Reports
        public ActionResult ProductList(string ProductCategoryId = "", string ProductSubCategoryId = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(ProductCategoryId) && !string.IsNullOrEmpty(ProductSubCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(ProductCategoryId, ProductSubCategoryId)));
                if (!string.IsNullOrEmpty(ProductCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(ProductCategoryId)));
                return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult RdlcReportProductList(string ProductCategoryId, string ProductSubCategoryId)
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
                string rPath = "RdlcReport/RptProductList.rdlc";
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
