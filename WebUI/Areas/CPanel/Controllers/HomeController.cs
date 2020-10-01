using System.Web.Mvc;
using ERP.WebUI.Controllers;

namespace ERP.WebUI.Areas.CPanel.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
