using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
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
        private readonly IDistrictService _districtService; 
        public DivisionController(IDivisionService divisionService, IDistrictService districtService)
        {
            _divisionService = divisionService;
            _districtService = districtService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(Mapper.Map<IEnumerable<DivisionViewModel>>(_divisionService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region Json
        public JsonResult GetDivisionList(string id)
        {           
            return Json(new SelectList(_divisionService.GetDivisionList(id), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region JSon
        public JsonResult GetDivisionLists() 
        {
            try
            {
                return Json(new SelectList(_divisionService.Lists(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public JsonResult GetDivisionListByCountryId(string CountryId) 
        {
            if (!string.IsNullOrEmpty(CountryId))
            {
                var model = new List<DistrictViewModel>();
                var districtData = Mapper.Map<IEnumerable<DistrictViewModel>>(_districtService.GetAll());
                model = districtData.Where(x => x.CountryId == CountryId).ToList();
                return Json(new SelectList(model, "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            //var data = _divisionService.GetDivisionList(id).ToList();
            return Json(new SelectList(_divisionService.GetDivisionList(CountryId), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                _divisionService.Add(Mapper.Map<Division>(divisionvm));
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
                return View(Mapper.Map<DivisionViewModel>(_divisionService.GetById(id)));
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
                _divisionService.Update(Mapper.Map<Division>(divisionvm));
                return JavaScript($"ShowResult('{"Data update successfully."}','{"success"}','{"redirect"}','{"../"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
