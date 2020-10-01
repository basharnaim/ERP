using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
{
    public class FlavorController : BaseController
    {
        #region Ctor
        private readonly IFlavorService _flavorService;
        public FlavorController(
            IFlavorService flavorService
            )
        {
            _flavorService = flavorService;
        }
        #endregion

        #region Get
        [Authorize]
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<FlavorViewModel>>(_flavorService.GetAll()));
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }
        #endregion

        #region JSon
        [Authorize]
        public JsonResult GetFlavorList()
        {
            try
            {
                return Json(new SelectList(_flavorService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region Create
        [HttpGet]
        [Authorize]
        public ActionResult Create()
        {
            try
            {
                return View(new FlavorViewModel { Active = true });
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Create(FlavorViewModel flavorVm)
        {
            try
            {
                _flavorService.Add(AutoMapperConfiguration.mapper.Map<Flavor>(flavorVm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/Flavor"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }
        #endregion

        #region Edit

        [HttpGet]
        [Authorize]
        public ActionResult Edit(string id)
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<FlavorViewModel>(_flavorService.GetById(id)));
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }

        [HttpPost]
        [Authorize]
        public JavaScriptResult Edit(FlavorViewModel flavorVm)
        {
            try
            {
                _flavorService.Update(AutoMapperConfiguration.mapper.Map<Flavor>(flavorVm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/Flavor"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','{"failure"}')");
            }
        }
        #endregion
    }
}
