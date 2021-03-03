using ERP.WebUI.Controllers;
using ERP.WebUI.ReportViewer;
using Library.Model.Inventory.Products;
using Library.Context.Repositories;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mime;
using System.Web.Helpers;
using System.Web.Mvc;
using AutoMapper;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class MedicineShopProductController : BaseController
    {
        #region Ctor
        private readonly IProductService _productService;
        private readonly IRawSqlService _rawSqlService;
        public MedicineShopProductController(
            IProductService productService,
            IRawSqlService rawSqlService
            )
        {
            _productService = productService;
            _rawSqlService = rawSqlService;
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
                return View(new ProductViewModel {Active = true, Sequence = _productService.GetAutoSequence() });
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
        public ActionResult ProductList(string itemCategoryId = "", string itemSubCategoryId = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(itemCategoryId) && !string.IsNullOrEmpty(itemSubCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(itemCategoryId, itemSubCategoryId)));
                if (!string.IsNullOrEmpty(itemCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(itemCategoryId)));
                return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult RdlcReportProductList(string itemCategoryId, string itemSubCategoryId)
        {
            try
            {
                if (itemCategoryId == "null")
                {
                    itemCategoryId = "";
                }
                if (itemSubCategoryId == "null")
                {
                    itemSubCategoryId = "";
                }
                IEnumerable<ProductViewModelForReport> items;
                if (!string.IsNullOrEmpty(itemCategoryId) && !string.IsNullOrEmpty(itemSubCategoryId))
                    items = Mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll(itemCategoryId, itemSubCategoryId));
                else if (!string.IsNullOrEmpty(itemCategoryId))
                    items = Mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll(itemCategoryId));
                else
                    items = Mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll());
                ReportDataSource rpt = new ReportDataSource("Product", items);
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
        public ActionResult ProductStockList(string companyId, string branchId, string supplierId, string productCategoryId, string productSubCategoryId, string productId, string productCode, string dateFrom, string dateTo)
        {
            try
            {
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    DateTime? dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                    dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    DateTime? dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                    dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    return View(Mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetAllProductStock(companyId, branchId, supplierId, productCategoryId, productSubCategoryId, productId, productCode, dateFrom, dateTo)));
                }
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult RdlcProductStockList(string companyId, string branchId, string supplierId, string productCategoryId, string productSubCategoryId, string productId, string productCode, string dateFrom, string dateTo)
        {
            try
            {
                if (companyId == "null")
                {
                    companyId = "";
                }
                if (branchId == "null")
                {
                    branchId = "";
                }
                if (supplierId == "null")
                {
                    supplierId = "";
                }
                if (productCategoryId == "null")
                {
                    productCategoryId = "";
                }
                if (productSubCategoryId == "null")
                {
                    productSubCategoryId = "";
                }
                if (productId == "null")
                {
                    productId = "";
                }
                if (dateFrom == "null")
                {
                    dateFrom = "";
                }
                if (dateTo == "null")
                {
                    dateTo = "";
                }
                if (!string.IsNullOrEmpty(dateFrom))
                {
                    DateTime? dfrom = Convert.ToDateTime(dateFrom);
                    dfrom = new DateTime(dfrom.Value.Year, dfrom.Value.Month, dfrom.Value.Day, 0, 0, 0);
                    dateFrom = dfrom.Value.ToString(CultureInfo.InvariantCulture);
                }
                if (!string.IsNullOrEmpty(dateTo))
                {
                    DateTime? dto = Convert.ToDateTime(dateTo);
                    dto = new DateTime(dto.Value.Year, dto.Value.Month, dto.Value.Day, 23, 59, 59);
                    dateTo = dto.Value.ToString(CultureInfo.InvariantCulture);
                }
                IEnumerable<ProductStockViewModel> productStocks = new List<ProductStockViewModel>();
                if (!string.IsNullOrEmpty(companyId))
                {
                    productStocks = Mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetAllProductStock(companyId, branchId, supplierId, productCategoryId, productSubCategoryId, productId, productCode, dateFrom, dateTo));
                }
                ReportDataSource rpt = new ReportDataSource("ProductStock", productStocks);
                RdlcReportViewerWithDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptProductStock.rdlc";
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&companyId=" + companyId + "&branchId=" + branchId);
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
