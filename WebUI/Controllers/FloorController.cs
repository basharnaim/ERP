using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.WebUI.Controllers
{
    public class FloorController : BaseController
    {
        #region Ctor
        private readonly IFloorService _floorService;
        public FloorController(
            IFloorService floorService
            )
        {
            _floorService = floorService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<FloorViewModel>>(_floorService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetFloorList()
        {
            try
            {
                return Json(new SelectList(_floorService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new FloorViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(FloorViewModel rackvm)
        {
            try
            {
                _floorService.Add(AutoMapperConfiguration.mapper.Map<Floor>(rackvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/Floor"}')");
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
                var rack = AutoMapperConfiguration.mapper.Map<Floor, FloorViewModel>(_floorService.GetById(id));
                return View(rack);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(FloorViewModel rackvm)
        {
            try
            {
                _floorService.Update(AutoMapperConfiguration.mapper.Map<Floor>(rackvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/Floor"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
