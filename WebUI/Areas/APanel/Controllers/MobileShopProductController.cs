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
using System.Web;
using System.Web.Mvc;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class MobileShopProductController : BaseController
    {
        #region Ctor
        private readonly IProductService _productService;
        private readonly IRawSqlService _rawSqlService;
        public MobileShopProductController(
            IProductService ProductService,
            IRawSqlService rawSqlService
            )
        {
            _productService = ProductService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        #region Get
        public ActionResult Index(string ProductCategoryId, string ProductSubCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(ProductCategoryId) && !string.IsNullOrEmpty(ProductSubCategoryId))
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(ProductCategoryId, ProductSubCategoryId)));
                if (!string.IsNullOrEmpty(ProductCategoryId))
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(ProductCategoryId)));
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
                return View(new ProductViewModel { Active=true});
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
                _productService.Add(AutoMapperConfiguration.mapper.Map<Product>(Productvm));
                return JavaScript($"ShowResult('Data saved successfully.','success','redirect','{"/APanel/MobileShopProduct/?ProductCategoryId=" + Productvm.ProductCategoryId + "&&ProductSubCategoryId=" + Productvm.ProductSubCategoryId}')");
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
        public JavaScriptResult Edit(ProductViewModel Productvm)
        {
            try
            {
                _productService.Update(AutoMapperConfiguration.mapper.Map<Product>(Productvm));
                return JavaScript($"ShowResult('Data updated successfully.','success','redirect','{"/APanel/MobileShopProduct/?ProductCategoryId=" + Productvm.ProductCategoryId + "&&ProductSubCategoryId=" + Productvm.ProductSubCategoryId}')");
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
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    var result = _productService.UploadFromDirectory(file);
                    WriteExcelFile(result, "Error");
                    ViewBag.Success = "Data upload successfully";
                    return View();
                }
                throw new Exception("Please select file");
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }
        #endregion

        #region Reports
        public ActionResult ProductList(string itemCategoryId = "", string itemSubCategoryId = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(itemCategoryId) && !string.IsNullOrEmpty(itemSubCategoryId))
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(itemCategoryId, itemSubCategoryId)));
                if (!string.IsNullOrEmpty(itemCategoryId))
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll(itemCategoryId)));
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModel>>(_productService.GetAll()));
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
                    items = AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll(itemCategoryId, itemSubCategoryId));
                else if (!string.IsNullOrEmpty(itemCategoryId))
                    items = AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll(itemCategoryId));
                else
                    items = AutoMapperConfiguration.mapper.Map<IEnumerable<ProductViewModelForReport>>(_productService.GetAll());
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
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetAllProductStock(companyId, branchId, supplierId, productCategoryId, productSubCategoryId, productId, productCode, dateFrom, dateTo)));
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
                    productStocks = AutoMapperConfiguration.mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetAllProductStock(companyId, branchId, supplierId, productCategoryId, productSubCategoryId, productId, productCode, dateFrom, dateTo));
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
