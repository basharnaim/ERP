using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Library.Model.Core.Organizations;
using Library.Service.Core.Organizations;
using Library.ViewModel.Core.Organizations;

namespace ERP.WebUI.Controllers
{
    public class BranchController : BaseController
    {
        #region Ctor
        private readonly IBranchService _branchService;
        public BranchController(IBranchService branchService)
        {
            _branchService = branchService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                return View(AutoMapperConfiguration.mapper.Map<IEnumerable<BranchViewModel>>(_branchService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetBranchList(string id)
        {
            return Json(new SelectList(_branchService.Lists(id), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                branchvm.Active = true;
                _branchService.Update(AutoMapperConfiguration.mapper.Map<Branch>(branchvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/Branch"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
