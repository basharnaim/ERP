using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Inventory.Products;
using Library.Service.Inventory.Products;
using Library.ViewModel.Inventory.Products;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class RackController : BaseController
    {
        #region Ctor
        private readonly IRackService _rackService;
        public RackController(IRackService rackService)
        {
            _rackService = rackService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<RackViewModel>>(_rackService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetRackList()
        {
            try
            {
                return Json(new SelectList(_rackService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new RackViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]

        public JavaScriptResult Create(RackViewModel rackvm)
        {
            try
            {
                _rackService.Add(AutoMapperConfiguration.mapper.Map<Rack>(rackvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/Rack"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<RackViewModel>(_rackService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]

        public JavaScriptResult Edit(RackViewModel rackvm)
        {
            try
            {
                _rackService.Update(AutoMapperConfiguration.mapper.Map<Rack>(rackvm));
                return JavaScript(
                    $"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Rack"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
