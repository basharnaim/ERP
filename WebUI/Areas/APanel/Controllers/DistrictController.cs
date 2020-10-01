using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ERP.WebUI.Controllers;
using Library.Model.Core.Addresses;
using Library.Service.Core.Addresses;
using Library.ViewModel.Core.Addresses;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class DistrictController : BaseController
    {
        #region Ctor
        private readonly IDistrictService _districtService;
        public DistrictController(IDistrictService districtService)
        {
            _districtService = districtService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<DistrictViewModel>>(_districtService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetDistrictList(string id)
        {
            return Json(new SelectList(_districtService.GetDistrictList(id), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new DistrictViewModel { Active = true, Sequence = _districtService.GetAutoSequence() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(DistrictViewModel districtvm)
        {
            try
            {
                _districtService.Add(AutoMapperConfiguration.mapper.Map<District>(districtvm));
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
                return View(AutoMapperConfiguration.mapper.Map<DistrictViewModel>(_districtService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(DistrictViewModel districtvm)
        {
            try
            {
                _districtService.Update(AutoMapperConfiguration.mapper.Map<District>(districtvm));
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
