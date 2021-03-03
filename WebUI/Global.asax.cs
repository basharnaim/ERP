using AutoMapper;
using ERP.WebUI.App_Start;
using Library.Crosscutting.Securities;
using System;
using System.Net.Http;
using System.Threading;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

namespace ERP.WebUI
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Mapper.Initialize(cfg=>cfg.AddProfile<AutoMapperProfile>());
            //AutoMapperConfig.ConfigureMapping();
        }

        ///-------------------------------------------------------------------------------------------------
        /// <summary>   Event handler. Called by Application for acquire request state events. </summary>
        ///
        /// <remarks>   Rafiqul Islam, 12/2/2015. </remarks>
        ///
        /// <param name="sender">   Source of the event. </param>
        /// <param name="e">        Event information. </param>
        ///-------------------------------------------------------------------------------------------------

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            try
            {
                HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authCookie != null && authCookie.Value != "")
                {
                    FormsAuthenticationTicket ticket = FormsAuthentication.Decrypt(authCookie.Value);
                    LoginIdentity identity = new LoginIdentity(ticket.UserData);
                    var basicTicket = Application["BasicTicket" + identity.Name];
                    var roleTicket = Application["RoleTicket" + identity.Name];
                    if (basicTicket != null && roleTicket != null && basicTicket.ToString() == ticket.UserData)
                    {
                        identity.SetRoles(roleTicket.ToString());
                        LoginPrincipal principal = new LoginPrincipal(identity);
                        HttpContext.Current.User = principal;
                        Thread.CurrentPrincipal = principal;
                        FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(1, FormsAuthentication.FormsCookieName, DateTime.Now, DateTime.Now.AddMinutes(720), ticket.IsPersistent, ticket.UserData);
                        string encTicket = FormsAuthentication.Encrypt(authTicket);
                        HttpContext.Current.Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, encTicket));
                        return;
                    }
                    authCookie.Expires = DateTime.Now.AddDays(-1);
                    HttpContext.Current.Response.Cookies.Add(authCookie);
                    Application["BasicTicket" + identity.Name] = null;
                    Application["RoleTicket" + identity.Name] = null;
                    if (HttpContext.Current.Request.Path.ToUpper().StartsWith("/CPANEL"))
                        HttpContext.Current.Response.Redirect("/Login/cpanel");
                    if (HttpContext.Current.Request.Path.ToUpper().StartsWith("/APanel"))
                        HttpContext.Current.Response.Redirect("/Login");
                }
                else
                {
                    //HttpContext.Current.Response.Redirect("http://localhost:44372");  // = //new RedirectToRouteResult(new RouteValueDictionary { { "action", "Index" }, { "controller", "Login" } });
                    //HttpContext.Current.RewritePath("http://localhost:44372/");
                }
            }
            catch (Exception ex)
            {
                var fail = ex.Message;
                FormsAuthentication.SignOut();
            }
        }
    }
}