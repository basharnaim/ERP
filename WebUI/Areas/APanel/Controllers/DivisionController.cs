using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Core.Addresses;
using Library.Service.Core.Addresses;
using Library.ViewModel.Core.Addresses;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class DivisionController : BaseController
    {
        #region Ctor
        private readonly IDivisionService _divisionService;
        public DivisionController(IDivisionService divisionService)
        {
            _divisionService = divisionService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<DivisionViewModel>>(_divisionService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetDivisionList(string id)
        {
            return Json(new SelectList(_divisionService.GetDivisionList(id), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new DivisionViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(DivisionViewModel divisionvm)
        {
            try
            {
                _divisionService.Add(AutoMapperConfiguration.mapper.Map<Division>(divisionvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"."}')");
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
                return View(AutoMapperConfiguration.mapper.Map<DivisionViewModel>(_divisionService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(DivisionViewModel divisionvm)
        {
            try
            {
                _divisionService.Update(AutoMapperConfiguration.mapper.Map<Division>(divisionvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"../"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
