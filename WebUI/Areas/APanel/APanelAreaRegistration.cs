using System.Web.Mvc;

namespace ERP.WebUI.Areas.APanel
{
    public class APanelAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "APanel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                "APanel_default",
                "APanel/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "ERP.WebUI.Areas.APanel.Controllers" }
            );
        }
    }
}