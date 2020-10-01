using Library.Crosscutting.Securities;
using Library.Model.Core.Organizations;
using Library.Service.Core.Securities;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ERP.WebUI.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        #region Ctor
        private readonly IUserService _userService;
        private readonly IUserGroupService _userGroupService;
        public LoginController(
            IUserService userService,
            IUserGroupService userGroupService
            )
        {
            _userService = userService;
            _userGroupService = userGroupService;
        }
        #endregion

        #region Login
        private static string GetLocalIpAddress(HttpContext http)
        {
            var ips = http.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            return !string.IsNullOrEmpty(ips) ? ips.Split(',')[0] : http.Request.ServerVariables["REMOTE_ADDR"];
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index1()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Index(string userId, string password, string returnUrl)
        {
            try
            {
                string workStationIp = GetLocalIpAddress(System.Web.HttpContext.Current);
                var userInfo = _userService.Login(userId, password);
                if (!string.IsNullOrEmpty(userInfo?.Id))
                {
                    var userGroup = _userGroupService.GetById(userInfo.UserGroupId);
                    var company = new Company();
                    var branch = new Branch();
                    if (!string.IsNullOrEmpty(userGroup?.Id))
                    {
                        company = _userService.GetCompanyInfo(userGroup.CompanyId);
                        branch = _userService.GetBranchInfo(userGroup.BranchId);
                    }
                    string[] roles = _userService.GetUserRoles(userId);
                    string basicTicket = LoginIdentity.CreateBasicTicket(
                                                        userInfo.UserId,
                                                        company?.Id,
                                                        company?.Name,
                                                        branch?.Id,
                                                        branch?.Name,
                                                        workStationIp,
                                                        userInfo.SysAdmin,
                                                        true);
                    string roleTicket = LoginIdentity.CreateRoleTicket(roles);
                    FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName, DateTime.Now, DateTime.Now.AddMinutes(720), true, basicTicket);
                    string encTicket = FormsAuthentication.Encrypt(authTicket);
                    HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                    HttpContext.Application["BasicTicket" + userId] = basicTicket;
                    HttpContext.Application["RoleTicket" + userId] = roleTicket;
                    if (userInfo.SysAdmin)
                    {
                        return RedirectToAction("Index", "Home", new { area = "APanel" });
                    }
                    return RedirectToAction("Index", "Home", new { area = "" });
                }
                ViewBag.Message = "User Id and password does not match or you are not a valid user...";
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
                return View();
            }
        }
        #endregion

        #region CPanel

        [HttpGet]
        public ActionResult CPanel()
        {
            return View();
        }


        [HttpPost]
        public ActionResult CPanel(string userId, string password, string returnUrl)
        {
            string workStationIP = GetLocalIpAddress(System.Web.HttpContext.Current);
            var userInfo = _userService.GetControlUser(userId, password);
            if (!string.IsNullOrEmpty(userInfo?.Id))
            {
                string basicTicket = LoginIdentity.CreateBasicTicket(
                    userInfo.Id,
                    "",
                    null,
                    "",
                    null,
                    workStationIP,
                    true,
                    true);
                string[] roles = _userService.GetUserRoles(userId);
                string roleTicket = LoginIdentity.CreateRoleTicket(roles);
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName, DateTime.Now, DateTime.Now.AddMinutes(720), true, basicTicket);
                string encTicket = FormsAuthentication.Encrypt(authTicket);
                HttpContext.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                HttpContext.Application["BasicTicket" + userId] = basicTicket;
                HttpContext.Application["RoleTicket" + userId] = roleTicket;
                return RedirectToAction("Index", "Home", new { area = "CPanel" });
            }
            ViewBag.Message = "User name or password is invalid!";
            return View();
        }
        #endregion

        #region Logout
        public ActionResult Logout(string road)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null && authCookie.Value != "")
            {
                FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                LoginIdentity identity = new LoginIdentity(ticket.UserData);
                authCookie.Expires = DateTime.Now.AddDays(-1);
                HttpContext.Response.Cookies.Add(authCookie);
                HttpContext.Application["BasicTicket" + identity.Name] = null;
                HttpContext.Application["RoleTicket" + identity.Name] = null;
            }
            switch (road)
            {
                case "cpanel":
                    return Redirect("/Login/CPanel");
                default:
                    return Redirect("/Login");
            }
        }
        #endregion
    }
}