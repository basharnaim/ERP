using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using Library.Model.Core.Securities;
using Library.Service.Core.Menus;
using Library.Service.Core.Securities;
using Library.ViewModel.Core.Securities;

namespace ERP.WebUI.Controllers
{
    public class UserRoleController : BaseController
    {
        #region Ctor
        private readonly IUserRoleService _userRoleService;
        private readonly IMenuService _menuService;
        public UserRoleController(
            IUserRoleService userRoleService,
            IMenuService menuService
            )
        {
            _userRoleService = userRoleService;
            _menuService = menuService;
        }
        #endregion

        #region Create & Update
        [HttpGet]
        public ActionResult Index(string companyId, string branchId, string module, string userGroupId)
        {
            try
            {
                UserRoleViewModel vm = new UserRoleViewModel();
                if (!string.IsNullOrEmpty(companyId))
                    vm.CompanyId = companyId;
                if (!string.IsNullOrEmpty(branchId))
                    vm.BranchId = branchId;
                vm.Module = module;
                vm.UserGroupId = userGroupId;

                List<UserRoleDetailViewModel> roledetailitems = new List<UserRoleDetailViewModel>();
                if (string.IsNullOrEmpty(module) || string.IsNullOrEmpty(userGroupId))
                {
                    vm.UserRoleDetailItems = roledetailitems;
                    return View(vm);
                }

                // Form menu table
                var roleDetails = (from rd in _userRoleService.GetAllMenuByUserGroup(module)
                    select new UserRoleDetailViewModel
                    {
                        MenuId = rd.Id,
                        Name = rd.Name
                    }).ToList();
                // From user role table
                var useroles = _userRoleService.GetAll(companyId, branchId, module, userGroupId);
                foreach (var item in roleDetails)
                {
                    if (useroles.Any(r => r.MenuId == item.MenuId))
                    {
                        var role = useroles.First(r => r.MenuId == item.MenuId);
                        item.Id = role.Id;
                        item.IsAdd = role.IsAdd;
                        item.IsEdit = role.IsEdit;
                        item.IsView = role.IsView;
                        item.IsDelete = role.IsDelete;
                        item.IsApprove = role.IsApprove;
                    }
                }
                vm.UserRoleDetailItems = new List<UserRoleDetailViewModel>();
                if (roleDetails.Any() && roleDetails.All(x => x.IsAdd))
                {
                    roleDetails.First().CheckAllAdd = true;
                }
                if (roleDetails.Any() && roleDetails.All(x => x.IsEdit))
                {
                    roleDetails.First().CheckAllEdit = true;
                }
                if (roleDetails.Any() && roleDetails.All(x => x.IsDelete))
                {
                    roleDetails.First().CheckAllDelete = true;
                }
                if (roleDetails.Any() && roleDetails.All(x => x.IsView))
                {
                    roleDetails.First().CheckAllView = true;
                }
                if (roleDetails.Any() && roleDetails.All(x => x.IsApprove))
                {
                    roleDetails.First().CheckAllApprove = true;
                }
                vm.UserRoleDetailItems.AddRange(roleDetails);
                return View(vm);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        [HttpPost]
        public JavaScriptResult Index(UserRoleViewModel userrolevm, string companyId, string branchId, string usergroupId)
        {
            try
            {
                List<UserRoleViewModel> userroles = new List<UserRoleViewModel>();
                var roleDetails = from rd in userrolevm.UserRoleDetailItems
                                  select new UserRoleViewModel
                                  {
                                      Name = rd.Name,
                                      IsAdd = rd.IsAdd,
                                      IsEdit = rd.IsEdit,
                                      IsDelete = rd.IsDelete,
                                      IsView = rd.IsView,
                                      IsApprove = rd.IsApprove,
                                      MenuId = rd.MenuId,
                                      Id = rd.Id
                                  };
                userroles.AddRange(roleDetails);
                var userrole = Mapper.Map< List<UserRole>>(userroles);
                _userRoleService.Add(userrole, companyId, branchId, usergroupId);
                return JavaScript(
                    $"ShowResult('{"Data saved successfully."}','{"success"}','{"redirect"}','{"/UserRole/?companyId=" + userrolevm.CompanyId + "&&branchId=" + userrolevm.BranchId + "&&module=" + userrolevm.Module + "&&UserGroupId=" + userrolevm.UserGroupId}')");
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }
        #endregion

        #region JSon
        public JsonResult GetMenuList()
        {
            try
            {
                return Json(new SelectList(_menuService.GetModule(), "Value", "Text"), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion

    }
}
