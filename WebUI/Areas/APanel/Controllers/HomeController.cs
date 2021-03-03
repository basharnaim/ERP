using Library.Context.Repositories;
using Library.Crosscutting.Securities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class HomeController : Controller
    {
        private readonly IRawSqlService _rawSqlService;
        public HomeController(IRawSqlService rawSqlService)
        {
            _rawSqlService = rawSqlService;
        }
        // GET: Admin/Home
        public ActionResult Index()
        {
             var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
            var serializer = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
            var dataSet = _rawSqlService.GetDashboard(identity.CompanyId, identity.BranchId, "");
            return View(dataSet);
        }

        public JsonResult GetProductList()
        {
            var users = GetUsers();
            return Json(users, JsonRequestBehavior.AllowGet);
        }

        private List<Barchat> GetUsers()
        {
            var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
            var dataSet = _rawSqlService.GetDashboard(identity.CompanyId, identity.BranchId, "");
            var barchats = new List<Barchat>();
            foreach (DataRow dr in dataSet.Tables["Table3"].Rows)
            {
              var barchat = new Barchat();
                barchat.y = dr["SaleDate"].ToString();
                barchat.a = Convert.ToInt32(dr["SaleAmount"]);
                barchat.b = Convert.ToInt32(dr["PurchaseAmount"]);
                barchats.Add(barchat);
            }
            return barchats;
        }

        public class Barchat
        {           
            public string y { get; set; }
            public int a { get; set; }
            public int b { get; set; }
        }
    }
}