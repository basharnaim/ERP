using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Model.Core.Organizations;
using Library.Service.Core.Organizations;
using Library.ViewModel.Core.Organizations;

namespace ERP.WebUI.Controllers
{
    public class WarehouseController : BaseController
    {
        #region Ctor
        private readonly IWarehouseService _warehouseService;
        public WarehouseController(
            IWarehouseService warehouseService
            )
        {
            _warehouseService = warehouseService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<WareHouseViewModel>>(_warehouseService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetWareHouseListByCompanyId(string companyId)
        {
            return Json(new SelectList(_warehouseService.Lists(companyId), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new WareHouseViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(WareHouseViewModel warehousevm)
        {
            try
            {
                _warehouseService.Add(AutoMapperConfiguration.mapper.Map<Warehouse>(warehousevm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/Warehouse"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<WareHouseViewModel>(_warehouseService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(WareHouseViewModel warehousevm)
        {
            try
            {
                _warehouseService.Update(AutoMapperConfiguration.mapper.Map<Warehouse>(warehousevm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/Warehouse"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
