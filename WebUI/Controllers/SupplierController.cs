using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ERP.WebUI.ReportViewer;
using Library.Model.Inventory.Suppliers;
using Library.Service.Inventory.Suppliers;
using Library.ViewModel.Inventory.Suppliers;
using Microsoft.Reporting.WebForms;

namespace ERP.WebUI.Controllers
{
    public class SupplierController : BaseController
    {
        #region Ctor
        private readonly ISupplierService _supplierService;

        public SupplierController(
            ISupplierService supplierService
            )
        {
            _supplierService = supplierService;
        }
        #endregion

        #region Get
        public ActionResult Index(string supplierCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(supplierCategoryId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<SupplierViewModel>>(_supplierService.GetAll(supplierCategoryId)));
                }
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<SupplierViewModel>>(_supplierService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }


        #endregion

        #region JSon
        public JsonResult GetSupplierList()
        {
            try
            {
                return Json(new SelectList(_supplierService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new SupplierViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(SupplierViewModel vm)
        {
            try
            {
                _supplierService.Add(AutoMapperConfiguration.mapper.Map<Supplier>(vm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/Supplier/"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<SupplierViewModel>(_supplierService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(SupplierViewModel suppliervm)
        {
            try
            {
                _supplierService.Update(AutoMapperConfiguration.mapper.Map<SupplierViewModel, Supplier>(suppliervm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/Supplier/"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Delete
        public ActionResult Delete(string id)
        {
            try
            {
                _supplierService.Archive(id);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        #endregion

        #region Report
        public ActionResult SupplierList(string supplierCategoryId)
        {
            try
            {
                if (!string.IsNullOrEmpty(supplierCategoryId))
                {
                    return View(AutoMapperConfiguration.mapper.Map<IEnumerable<Supplier>, IEnumerable<SupplierViewModel>>(_supplierService.GetAll(supplierCategoryId)));
                }

                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<Supplier>, IEnumerable<SupplierViewModel>>(_supplierService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        public ActionResult RdlcReportSupplierList(string supplierCategoryId)
        {
            try
            {
                if (supplierCategoryId == "null")
                {
                    supplierCategoryId = "";
                }
                var suppliers = AutoMapperConfiguration.mapper.Map<IEnumerable<SupplierViewModel>>(!string.IsNullOrEmpty(supplierCategoryId) ? _supplierService.GetAll(supplierCategoryId) : _supplierService.GetAll());
                ReportDataSource rpt = new ReportDataSource("Supplier", suppliers);
                RdlcReportViewerWithoutDate.reportDataSource = rpt;
                string rPath = "RdlcReport/RptSupllier.rdlc";
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
