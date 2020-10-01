using ERP.WebUI.Controllers;
using ERP.WebUI.ReportViewer;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class BarcodeController : BaseController
    {
        #region Ctor
        private readonly IBarcodeService _barcodeService;
        private readonly IProductService _productService;
        public BarcodeController(
            IBarcodeService barcodeService,
            IProductService productService
            )
        {
            _barcodeService = barcodeService;
            _productService = productService;
        }
        #endregion

        #region Partial
        [HttpPost]
        public ViewResult BlankItem()
        {
            try
            {
                return View("_ProductRow", new BarcodeViewModel());
            }
            catch (Exception ex)
            {
                return View($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Index()
        {
            try
            {
                var serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
                ViewBag.ItemList = serializer.Serialize(_productService.GetAllPurchaseItemList());
                BarcodeViewModel vm = new BarcodeViewModel
                {
                    BarcodeList = new List<BarcodeViewModel>()
                };
                vm.BarcodeList.Add(new BarcodeViewModel());
                return View(vm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public ActionResult Index(string TotalIds = "", string TotalNumber = "", string companyId = "", string branchId = "")
        {
            try
            {
                string[] itemIds = new JavaScriptSerializer().Deserialize<string[]>(TotalIds);
                int[] numbers = new JavaScriptSerializer().Deserialize<int[]>(TotalNumber);
                var datatable = _barcodeService.GenerateBarcodeList(itemIds, numbers, companyId, branchId);
                ReportDataSource rpt = new ReportDataSource("BarcodeGenerator", datatable);
                RdlcBarcodeReportViewer.reportDataSource = rpt;
                string rPath = "RdlcReport/RptBarcodeListForAmericanTMartFinal.rdlc";
                Response.Redirect("~/ReportViewer/RdlcBarcodeReportViewer.aspx?rPath=" + rPath);
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