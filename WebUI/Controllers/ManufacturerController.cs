using AutoMapper;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
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
                return View(Mapper.Map<IEnumerable<ManufacturerViewModel>>(_manufacturerService.GetAll()));
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
        public JavaScriptResult Create(ManufacturerViewModel manufacturerVm)
        {
            try
            {
                _manufacturerService.Add(Mapper.Map<Manufacturer>(manufacturerVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/Manufacturer"}')");
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
                return View(Mapper.Map<Manufacturer, ManufacturerViewModel>(_manufacturerService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(ManufacturerViewModel manufacturerVm)
        {
            try
            {
                _manufacturerService.Update(Mapper.Map<Manufacturer>(manufacturerVm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/Manufacturer"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
