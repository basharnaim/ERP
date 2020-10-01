#region Using

using Library.Crosscutting.Securities;
using Library.Service.Core.Menus;
using Library.Service.Core.Securities;
using System;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Unity;

#endregion

namespace ERP.WebUI.Filters
{
    [
        AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true)
    ]
    public class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        static readonly IUnityContainer Unitycontainer = UnityConfig.Container;

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException(nameof(httpContext));
            }

            var authorize = false;
            try
            {
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                if (httpContext.Request.IsAjaxRequest())
                    authorize = true;
                else if (identity.SysAdmin || identity.SysAdmin)
                    authorize = true;
                else
                {
                    var targetPage = httpContext.Request.RequestContext.RouteData.Values["controller"] as string;
                    var targetAction = httpContext.Request.RequestContext.RouteData.Values["action"] as string;
                    var menuService = Unitycontainer.Resolve<IMenuService>();
                    var menuId = menuService.GetMenuId(targetPage, targetPage + "/" + targetAction);
                    var userService = Unitycontainer.Resolve<IUserService>();
                    var userGroupId = userService.GetUserByUserId(identity.Name).UserGroupId;
                    var userRoleService = Unitycontainer.Resolve<IUserRoleService>();
                    switch (targetAction)
                    {
                        case "Index":
                            //authorize = _userRoleService.CanView(userGroupId, menuId);
                            authorize = true;
                            break;
                        case "Create":
                            authorize = userRoleService.CanAdd(userGroupId, menuId);
                            break;
                        case "Edit":
                            authorize = userRoleService.CanEdit(userGroupId, menuId);
                            break;
                        case "Delete":
                            authorize = userRoleService.CanDelete(userGroupId, menuId);
                            break;
                        case "Approve":
                            authorize = userRoleService.CanApprove(userGroupId, menuId);
                            break;
                        default:
                            authorize = true;
                            break;
                    }
                }
                return authorize;
            }
            catch (Exception ex)
            {
                return authorize;
            }
        }
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            // Returns HTTP 401 - see comment in HttpUnauthorizedResult.cs.
            filterContext.Result = new RedirectToRouteResult(
                                       new RouteValueDictionary
                                       {
                                           { "action", "Index" },
                                           { "controller", "Login" }
                                       });
        }
    }
}
