using System.Web.Mvc;

namespace ERP.WebUI.Areas.CPanel
{
    public class CPanelAreaRegistration : AreaRegistration 
    {
        public override string AreaName
        {
            get
            {
                return "CPanel";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "CPanel_default",
                "CPanel/{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { "ERP.WebUI.Areas.CPanel.Controllers" }
            );
        }
    }
}