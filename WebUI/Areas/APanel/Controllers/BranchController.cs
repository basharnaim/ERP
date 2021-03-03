using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Core.Organizations;
using Library.Service.Core.Organizations;
using Library.ViewModel.Core.Organizations;
using WebGrease.Css.Extensions;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class BranchController : BaseController
    {
        #region Ctor
        private readonly IBranchService _branchService;
        private readonly ICompanyService _companyService;
        public BranchController(IBranchService branchService, ICompanyService companyService)
        {
            _branchService = branchService;
            _companyService = companyService;
        }
        #endregion

        #region Get
        public ActionResult Index()
        {
            try
            {
                var branchList = new List<BranchViewModel>();
                var branchData = Mapper.Map<IEnumerable<BranchViewModel>>(_branchService.GetAll());
                foreach (var item in branchData)
                {
                    item.CompanyName = _companyService.GetById(item.CompanyId).Name;
                    branchList.Add(item);
                }
                return View(branchList);
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
                return View(Mapper.Map<BranchViewModel>(_branchService.GetById(id)));
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
                _branchService.Update(Mapper.Map<Branch>(branchvm));
                return JavaScript($"ShowResult('{"Data updated successfully."}','{"success"}','{"redirect"}','{"/APanel/Branch"}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
