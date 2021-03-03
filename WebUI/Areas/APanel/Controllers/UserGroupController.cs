using System;
using System.Collections.Generic;
using System.Web.Mvc;
using AutoMapper;
using ERP.WebUI.Controllers;
using Library.Model.Core.Securities;
using Library.Service.Core.Securities;
using Library.ViewModel.Core.Securities;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class UserGroupController : BaseController
    {
        #region Ctor
        private readonly IUserGroupService _userGroupService;
        public UserGroupController(IUserGroupService userGroupService)
        {
            _userGroupService = userGroupService;
        }
        #endregion

        #region Get
        public ActionResult Index(string companyId, string branchId)
        {
            try
            {
                if (!string.IsNullOrEmpty(companyId) && !string.IsNullOrEmpty(branchId))
                {
                    return View(Mapper.Map<IEnumerable<UserGroupViewModel>>(_userGroupService.GetAll(companyId, branchId)));
                }
                if (!string.IsNullOrEmpty(companyId))
                {
                    return View(Mapper.Map<IEnumerable<UserGroupViewModel>>(_userGroupService.GetAll(companyId)));
                }
                return View(Mapper.Map<IEnumerable<UserGroupViewModel>>(_userGroupService.GetAll()));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public JsonResult GetUserGroupList(string branchId)
        {
            try
            {
                return Json(new SelectList(_userGroupService.Lists(branchId), "Value", "Text"), JsonRequestBehavior.AllowGet);
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
                return View(new UserGroupViewModel {Active=true, Sequence= _userGroupService.GetAutoSequence() });
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Create(UserGroupViewModel userGroupvm)
        {
            try
            {
                _userGroupService.Add(Mapper.Map<UserGroup>(userGroupvm));
                return JavaScript($"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/UserGroup?companyId=" + userGroupvm.CompanyId + "&&branchId=" + userGroupvm.BranchId}')");
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
                return View(Mapper.Map<UserGroupViewModel>(_userGroupService.GetById(id)));
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Edit(UserGroupViewModel userGroupvm)
        {
            try
            {
                _userGroupService.Update(Mapper.Map<UserGroup>(userGroupvm));
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/APanel/UserGroup?companyId=" + userGroupvm.CompanyId + "&&branchId=" + userGroupvm.BranchId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion
    }
}
