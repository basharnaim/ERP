using ERP.WebUI.Controllers;
using Library.Model.Core.Organizations;
using Library.Service.Core.Addresses;
using Library.Service.Core.Organizations;
using Library.ViewModel.Core.Organizations;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace ERP.WebUI.Areas.CPanel.Controllers
{
    public class BranchController : BaseController
    {
        #region Ctor
        private readonly IBranchService _branchService;
        private readonly ICountryService _countryService;
        private readonly IDivisionService _divisionService;
        private readonly IDistrictService _districtService;
        public BranchController(
            IBranchService branchService,
            ICountryService countryService,
            IDivisionService divisionService,
            IDistrictService districtService
            )
        {
            _branchService = branchService;
            _countryService = countryService;
            _divisionService = divisionService;
            _districtService = districtService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<BranchViewModel>>(_branchService.GetAllForCPanel()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public JsonResult GetBranchList(string id)
        {
            return Json(new SelectList(_branchService.Lists(id), "Value", "Text"), JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Create
        [HttpGet]
        public ActionResult Create()
        {
            try
            {
                return View(new BranchViewModel { Active = true });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(BranchViewModel branchvm)
        {
            try
            {
                _branchService.Add(AutoMapperConfiguration.mapper.Map<Branch>(branchvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/CPanel/Branch"}')");
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
                return View(AutoMapperConfiguration.mapper.Map<BranchViewModel>(_branchService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]

        public JavaScriptResult Edit(BranchViewModel branchvm)
        {
            try
            {
                _branchService.Update(AutoMapperConfiguration.mapper.Map<Branch>(branchvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/CPanel/Branch"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
