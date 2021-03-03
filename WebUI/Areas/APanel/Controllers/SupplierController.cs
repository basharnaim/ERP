using AutoMapper;
using ERP.WebUI.Controllers;
using ERP.WebUI.ReportViewer;
using Library.Model.Inventory.Suppliers;
using Library.Service.Inventory.Suppliers;
using Library.ViewModel.Inventory.Suppliers;
using Microsoft.Reporting.WebForms;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;

namespace ERP.WebUI.Areas.APanel.Controllers
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
                    return View(Mapper.Map<IEnumerable<SupplierViewModel>>(_supplierService.GetAll(supplierCategoryId)));
                }
                return View(Mapper.Map<IEnumerable<SupplierViewModel>>(_supplierService.GetAll()));
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
        public JsonResult GetSupplierListBySubCat(string Id)
        {
            try
            {
                return Json(new SelectList(_supplierService.Lists(Id), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                _supplierService.Add(Mapper.Map<Supplier>(vm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/Supplier/"}')");
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
                return View(Mapper.Map<SupplierViewModel>(_supplierService.GetById(id)));
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
                _supplierService.Update(Mapper.Map<SupplierViewModel, Supplier>(suppliervm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Supplier/"}')");
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
        public JavaScriptResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    var result = _supplierService.UploadFromDirectory(file);
                    WriteExcelFile(result, "Error");
                    return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Supplier"}')");
                }
                return JavaScript($"ShowResult('Please select file','failure')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
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
                    return View(Mapper.Map<IEnumerable<Supplier>, IEnumerable<SupplierViewModel>>(_supplierService.GetAll(supplierCategoryId)));
                }

                return View(Mapper.Map<IEnumerable<Supplier>, IEnumerable<SupplierViewModel>>(_supplierService.GetAll()));
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
                var suppliers = Mapper.Map<IEnumerable<SupplierViewModel>>(!string.IsNullOrEmpty(supplierCategoryId) ? _supplierService.GetAll(supplierCategoryId) : _supplierService.GetAll());
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
