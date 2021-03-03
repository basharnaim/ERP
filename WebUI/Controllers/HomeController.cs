using Library.Crosscutting.Securities;
using Library.Context.Repositories;
using Library.Service.Core.Securities;
using Library.ViewModel.Core.Securities;
using Library.ViewModel.Inventory.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using AutoMapper;

namespace ERP.WebUI.Controllers
{
    public class HomeController : BaseController
    {
        #region Ctor
        private readonly IUserRoleService _userRoleService;
        private readonly IRawSqlService _rawSqlService;
        public HomeController(IUserRoleService userRoleService, IRawSqlService rawSqlService)
        {
            _userRoleService = userRoleService;
            _rawSqlService = rawSqlService;
        }
        #endregion

        public ActionResult Index()
        {
            try
            {
                string company = new System.Configuration.AppSettingsReader().GetValue("POSTYPE", typeof(string)).ToString().ToUpper();
                switch (company)
                {
                    case "SSP":
                        return RedirectToAction("SSPOS");
                    case "MDCN":
                        return RedirectToAction("MDCNPOS");
                    case "MCSPOS":
                        return RedirectToAction("MCSPOS");
                    case "MSPOS":
                        return RedirectToAction("MobileShop");
                    case "HARDP":
                        return RedirectToAction("HARDPOS");
                    case "CRP":
                        return RedirectToAction("CRPOS");

                    case "NTECHPOS":
                        return RedirectToAction("SSPOS");
                    case "P21P":
                        return RedirectToAction("P21POS");
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Fail = ex.Message;
                return View();
            }
        }

        public ActionResult SSPOS()
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return View(Mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId)).ToList());
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult MDCNPOS()
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return View(Mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId)).ToList());
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult MCSPOS()
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return View(Mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId)).ToList());
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }


        public ActionResult MobileShop()
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return View(Mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId)).ToList());
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult HARDPOS()
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return View(Mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId)).ToList());
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }


        public ActionResult P21POS()
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return View(Mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId)).ToList());
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

        public ActionResult CRPOS()
        {
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                return View(Mapper.Map<IEnumerable<ProductStockViewModel>>(_rawSqlService.GetBranchwiseProductStockGreaterThanZero(identity.CompanyId, identity.BranchId)).ToList());
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }


        //[OutputCache(Duration = 43200)]
        public ActionResult MenuBinding()
        {
            try
            {
                var userRoleList = new List<UserRoleViewModel>();
                var userRoles = _userRoleService.GetUserRole();
                var menuPatents = (from u in userRoles
                                   group u by u.MenuParentId into g
                                   select new UserRoleViewModel
                                   {
                                       Id = g.FirstOrDefault()?.Id,
                                       MenuParentId = g.FirstOrDefault()?.MenuParentId.Value
                                   }).Distinct().OrderBy(x => x.MenuParentId).ToList();

                foreach (var item in menuPatents)
                {
                    var userRole = new UserRoleViewModel();
                    int id = Convert.ToInt32(item.MenuParentId);
                    var menu = _userRoleService.GetMenuParentById(id);
                    userRole.MenuParentId = item.MenuParentId;
                    userRole.Name = menu.Name;
                    userRole.Icon = menu.Icon;
                    userRole.MenuItemList = new List<MenuItemListViewModel>();
                    userRole.MenuItemList.AddRange(Mapper.Map<IEnumerable<MenuItemListViewModel>>(userRoles.Where(r => r.MenuParentId == item.MenuParentId)).OrderBy(x => x.MenuId));
                    userRoleList.Add(userRole);
                }
                return PartialView("_MenuBinding", userRoleList);
            }
            catch (Exception ex)
            {
                return JavaScript($"ShowResult('{ex.Message}','failure')");
            }
        }

    }
}