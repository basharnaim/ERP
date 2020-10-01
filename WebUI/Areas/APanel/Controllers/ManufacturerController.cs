using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ManufacturerController : BaseController
    {
        #region Ctor
        private readonly IManufacturerService _manufacturerService;
        public ManufacturerController(
            IManufacturerService manufacturerService
            )
        {
            _manufacturerService = manufacturerService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<ManufacturerViewModel>>(_manufacturerService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetManufacturerList()
        {
            try
            {
                return Json(new SelectList(_manufacturerService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new ManufacturerViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(ManufacturerViewModel manufacturervm)
        {
            try
            {
                _manufacturerService.Add(AutoMapperConfiguration.mapper.Map<Manufacturer>(manufacturervm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/Manufacturer"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<Manufacturer, ManufacturerViewModel>(_manufacturerService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ManufacturerViewModel manufacturervm)
        {
            try
            {
                _manufacturerService.Update(AutoMapperConfiguration.mapper.Map<Manufacturer>(manufacturervm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Manufacturer"}')");
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
        public JavaScriptResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file != null)
                {
                    var result = _manufacturerService.UploadFromDirectory(file);
                    WriteExcelFile(result, "Error");
                    return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Manufacturer"}')");
                }
                return JavaScript($"ShowResult('Please select file','failure')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
