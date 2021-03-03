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
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using Library.Service.Inventory.Purchases;
using Library.Context.Pos;
using Library.Context;
using System.Linq;
using AutoMapper;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    [Authorize]
    public class SuperShopProductController : BaseController
    {
        #region Ctor
        private readonly IProductService _productService;
        private readonly IRawSqlService _rawSqlService;
        //private readonly IPurchaseService _purchaseService;
        private IPurchaseRepository _purchaseRepository;
        public SuperShopProductController(IProductService productService, IRawSqlService rawSqlService, IPurchaseService purchaseService)
        {
            _productService = productService;
            _rawSqlService = rawSqlService;
            this._purchaseRepository = new PurchaseRepository(new PosContext());
        }
        #endregion

        #region Get
        public ActionResult Index(string productCategoryId, string productSubCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(productCategoryId) && !string.IsNullOrEmpty(productSubCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(productCategoryId, productSubCategoryId)));
                if (!string.IsNullOrEmpty(productCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(productCategoryId)));
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
                _productService.Add(Mapper.Map<Product>(productVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/SuperShopProduct/?productCategoryId=" + productVm.ProductCategoryId + "&&productSubCategoryId=" + productVm.ProductSubCategoryId}')");
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
        public JavaScriptResult Edit(ProductViewModel productVm)
        {
            try
            {
                _productService.Update(Mapper.Map<Product>(productVm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/SuperShopProduct/?productCategoryId=" + productVm.ProductCategoryId + "&&productSubCategoryId=" + productVm.ProductSubCategoryId}')");

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
                //var data = from ob in _purchaseRepository.GetOpeningBlances() select ob;
                return View();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        [HttpPost]
        [Authorize]
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    var result = _productService.UploadFromDirectoryForAmericanTMart(file);
                    WriteExcelFile(result, "Error");
                    return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/SuperShopProduct"}')");
                }
                return JavaScript($"ShowResult('Please select file','failure')");
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
        public ActionResult ProductList(string productCategoryId = "", string productSubCategoryId = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(productCategoryId) && !string.IsNullOrEmpty(productSubCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(productCategoryId, productSubCategoryId)));
                if (!string.IsNullOrEmpty(productCategoryId))
                    return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(productCategoryId)));
                return View(Mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult RdlcReportProductList(string productCategoryId, string productSubCategoryId)
        {
            try
            {
                if (productCategoryId == "null")
                {
                    productCategoryId = "";
                }
                if (productSubCategoryId == "null")
                {
                    productSubCategoryId = "";
                }
                IEnumerable<ProductViewModelForReport> items;
                if (!string.IsNullOrEmpty(productCategoryId) && !string.IsNullOrEmpty(productSubCategoryId))
                    items = Mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll(productCategoryId, productSubCategoryId));
                else if (!string.IsNullOrEmpty(productCategoryId))
                    items = Mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll(productCategoryId));
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
                if (productCode == "null")
                {
                    productCode = "";
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
                Response.Redirect("~/ReportViewer/RdlcReportViewerWithDate.aspx?rPath=" + rPath + "&dfrom=" + dateFrom + "&dto=" + dateTo + "&companyId=" + companyId + "&branchId=" + branchId);
                return View();
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult RdlcPurchaseSummary(string companyId, string branchId, string supplierId)
        {
            return null;
        }
        #endregion
    }
}
