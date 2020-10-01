#region Using

using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

#endregion

namespace ERP.WebUI.Controllers
{
    public class CourierController : BaseController
    {
        #region Ctor
        private readonly ICourierService _courierService;
        public CourierController(ICourierService courierService)
        {
            _courierService = courierService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<CourierViewModel>>(_courierService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetCourierList()
        {
            try
            {
                return Json(new SelectList(_courierService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new CourierViewModel { Active=true, Sequence = _courierService.GetAutoSequence() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(CourierViewModel Couriervm)
        {
            try
            {
                _courierService.Add(AutoMapperConfiguration.mapper.Map<Courier>(Couriervm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/Courier"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<CourierViewModel>(_courierService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(CourierViewModel Couriervm)
        {
            try
            {
                _courierService.Update(AutoMapperConfiguration.mapper.Map<Courier>(Couriervm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/Courier"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
