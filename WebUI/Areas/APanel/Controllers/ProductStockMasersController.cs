using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ERP.WebUI.Models;
using ERP.WebUI.Models.ViewModels;
using Library.Crosscutting.Securities;
using Library.Service.Inventory.Sales;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ProductStockMasersController : Controller
    {
        private POS_TmartEntities db = new POS_TmartEntities();

        //IEnumerable<VmProductStockMaster> ProductStockMasers = new List<VmProductStockMaster>();
        List<VmProductStockMaster> ProductStockMasersList = new List<VmProductStockMaster>();
        IEnumerable<VmProductStockMaster> ProductStockMasersLists = new List<VmProductStockMaster>();

        private readonly ISaleService _saleService;

        public ProductStockMasersController(ISaleService saleService)
        {
            _saleService = saleService;
        }
        // GET: APanel/ProductStockMasers
        public ActionResult Index()
        {
            try
            {
                var result = (from p in db.Products
                              join pm in db.ProductStockMasers on p.Id equals pm.ProductID
                              //into psmObj
                              //from pm in psmObj.DefaultIfEmpty()
                              //where p.Id == productId
                              select new VmProductStockMaster
                              {
                                  StockID = pm.StockID,
                                  CategoryID = pm.CategoryID,
                                  CompanyID = pm.CompanyID,
                                  BranchID = pm.BranchID,
                                  SupplierID = pm.SupplierID,
                                  UOMID = pm.UOMID,
                                  ReceiveQty = pm.ReceiveQty,
                                  ReceiveValue = pm.ReceiveValue,
                                  SalesValue = pm.SalesValue,
                                  CurrentStock = pm.CurrentStock,

                                  ProductID = p.Id,
                                  ProductName = p.Name,
                                  ProductCode = p.Code,
                                  ProdcutDescription = p.Description,
                                  IsActive = p.Active
                              }).AsEnumerable();

                return View(ProductStockMasersList);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        // GET: APanel/ProductStockMasers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStockMaser productStockMaser = db.ProductStockMasers.Find(id);
            if (productStockMaser == null)
            {
                return HttpNotFound();
            }
            return View(productStockMaser);
        }

        // GET: APanel/ProductStockMasers/Create
        public ActionResult Create(List<ProductStockMaser> productStockMasters, string companyId, string branchId, string productId, string productIds = "0")
        {
            if (productId != null && productId != "")
            {
                var productIds_ = productIds.ToArray();
                var result = (from p in db.Products
                              join psm in db.ProductStockMasers on p.Id equals psm.ProductID
                              into psmObj
                              from pm in psmObj.DefaultIfEmpty()
                              join u in db.UOMs on p.UOMId equals u.Id
                              into puObj
                              from pu in puObj.DefaultIfEmpty()
                              join br in db.Branches on p.BrandId equals br.Id
                              into brObj
                              from pbr in brObj.DefaultIfEmpty()
                              join c in db.ProductCategories on p.ProductCategoryId equals c.Id
                              into pcObj
                              from pc in pcObj.DefaultIfEmpty()
                              where p.Id == productId //productIds.Contains(p.Id) //
                              select new VmProductStockMaster
                              {
                                  StockID = pm.StockID, 
                                  CategoryID = (pm.CategoryID == null ? 0 : pm.CategoryID),
                                  CompanyID = (pm.CompanyID == null ? 0 : pm.CompanyID),
                                  BranchID = (pm.BranchID == null ? 0 : pm.BranchID),
                                  SupplierID = (pm.SupplierID == null ? 0 : pm.SupplierID),
                                  UOMID = (pm.UOMID == null ? 0 : pm.UOMID),
                                  ReceiveQty = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                                  ReceiveValue = (pm.ReceiveValue == null ? 0 : pm.ReceiveValue),
                                  SalesValue = (pm.SalesValue == null ? 0 : pm.SalesValue),
                                  CurrentStock = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                                  ProductID = p.Id,
                                  ProductName = p.Name,
                                  ProductCode = p.Code,
                                  ProdcutDescription = p.Description,
                                  IsActive = p.Active,
                                  UOMName = pu.Name,
                                  BranchName = pbr.Name,
                                  CategoryName = pc.Name
                              }).AsEnumerable();
                //ViewBag.ids = productIds + "," + productId;
                int count = 0;
                ProductStockMasersList = Session["ProductStock"] as List<VmProductStockMaster>;
                if (ProductStockMasersList == null)
                {
                    ProductStockMasersList = new List<VmProductStockMaster>();
                }
                else
                {
                    count = ProductStockMasersList.Where(x => x.ProductID == result.FirstOrDefault().ProductID).Count();
                }

                if (count == 0)
                {
                    ProductStockMasersList.AddRange(result);
                }
                ProductStockMasersLists = ProductStockMasersList.AsEnumerable();
                Session["ProductStock"] = ProductStockMasersLists;
                ViewBag.ProductList = ProductStockMasersLists;
                return View(ProductStockMasersLists);
            }
            else
            {
                ProductStockMasersList = Session["ProductStock"] as List<VmProductStockMaster>;
                ProductStockMasersLists = ProductStockMasersList.AsEnumerable();               
                return View(ProductStockMasersLists);                
            }
        }

        // POST: APanel/ProductStockMasers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(List<ProductStockMaser> customers, string companyId, string branchId, string productId)
        {
            if (productId != null && productId != "")
            {
                var result = (from p in db.Products
                              join psm in db.ProductStockMasers on p.Id equals psm.ProductID
                              into psmObj
                              from pm in psmObj.DefaultIfEmpty()
                              join u in db.UOMs on p.UOMId equals u.Id
                              into puObj
                              from pu in puObj.DefaultIfEmpty()
                              join br in db.Branches on p.BrandId equals br.Id
                              into brObj
                              from pbr in brObj.DefaultIfEmpty()
                              join c in db.ProductCategories on p.ProductCategoryId equals c.Id
                              into pcObj
                              from pc in pcObj.DefaultIfEmpty()
                              where p.Id == productId //productIds.Contains(p.Id) //
                              select new VmProductStockMaster
                              {
                                  StockID = pm.StockID,
                                  CategoryID = (pm.CategoryID == null ? 0 : pm.CategoryID),
                                  CompanyID = (pm.CompanyID == null ? 0 : pm.CompanyID),
                                  BranchID = (pm.BranchID == null ? 0 : pm.BranchID),
                                  SupplierID = (pm.SupplierID == null ? 0 : pm.SupplierID),
                                  UOMID = (pm.UOMID == null ? 0 : pm.UOMID),
                                  ReceiveQty = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                                  ReceiveValue = (pm.ReceiveValue == null ? 0 : pm.ReceiveValue),
                                  SalesValue = (pm.SalesValue == null ? 0 : pm.SalesValue),
                                  CurrentStock = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                                  ProductID = p.Id,
                                  ProductName = p.Name,
                                  ProductCode = p.Code,
                                  ProdcutDescription = p.Description,
                                  IsActive = p.Active,
                                  UOMName = pu.Name,
                                  BranchName = pbr.Name,
                                  CategoryName = pc.Name
                              }).AsEnumerable();
                //ViewBag.ids = productIds + "," + productId;
                int count = 0;

                ProductStockMasersList = Session["ProductStock"] as List<VmProductStockMaster>;
                if (ProductStockMasersList == null)
                {
                    ProductStockMasersList = new List<VmProductStockMaster>();
                }
                else
                {
                    count = ProductStockMasersList.Where(x => x.ProductID == result.FirstOrDefault().ProductID).Count();
                }

                if (count == 0)
                {
                    ProductStockMasersList.AddRange(result);
                }
                ProductStockMasersLists = ProductStockMasersList.AsEnumerable();
                Session["ProductStock"] = ProductStockMasersLists;
                ViewBag.ProductList = ProductStockMasersLists;
                return View(ProductStockMasersLists);
            }
            else
            {
                return View();
            }
            //if (ModelState.IsValid)
            //{
            //    ProductStockMaser productStockMaser = new ProductStockMaser();
            //    db.ProductStockMasers.Add(productStockMaser);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //return View(productStockMasters);
        }

        // GET: APanel/ProductStockMasers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStockMaser productStockMaser = db.ProductStockMasers.Find(id);
            if (productStockMaser == null)
            {
                return HttpNotFound();
            }
            return View(productStockMaser);
        }

        // POST: APanel/ProductStockMasers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductStockMaser productStockMaser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productStockMaser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productStockMaser);
        }

        // GET: APanel/ProductStockMasers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStockMaser productStockMaser = db.ProductStockMasers.Find(id);
            if (productStockMaser == null)
            {
                return HttpNotFound();
            }
            return View(productStockMaser);
        }

        // POST: APanel/ProductStockMasers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductStockMaser productStockMaser = db.ProductStockMasers.Find(id);
            db.ProductStockMasers.Remove(productStockMaser);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        [HttpPost]
        public ActionResult SetItem(List<ProductStockMaser> customers, string companyId, string branchId, string productId, string productIds = "0")
        {
            ProductStockMasersLists = Session["ProductStock"] as List<VmProductStockMaster>;
            if (ProductStockMasersLists != null )
            {
                var productStockList = new List<ProductStockMaser>();
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                var StockID = _saleService.GenerateAutoId(identity.CompanyId, identity.BranchId, "Sale");
                try
                {
                    foreach (VmProductStockMaster vmpsm in ProductStockMasersLists)
                    {
                        int count = db.ProductStockMasers.Where(x => x.ProductID == vmpsm.ProductID).Count();
                        Product p = db.Products.Where(x => x.Id == vmpsm.ProductID).FirstOrDefault();
                        if (count > 0)
                        {
                            var sql = "UPDATE [dbo].[ProductStockMaser] SET " +
                               "[BranchID] = " + vmpsm.BranchID + "  ,[SupplierID] = " + vmpsm.SupplierID + " ,[CategoryID]= " + vmpsm.CategoryID + " ,[CompanyID] = " + vmpsm.CompanyID + " " +
                               ",[UOMID]= " + vmpsm.UOMID + " ,[ReceiveQty] = " + vmpsm.ReceiveQty + " ,[ReceiveValue] = " + vmpsm.ReceiveValue + " ,[LastReceiveDate] = '" + DateTime.Now.ToShortDateString() + "',[IssueQty] = " + vmpsm.IssueQty + " ,[IssueValue] = " + vmpsm.IssueValue +
                               ",[LastIssueDate] = null ,[CurrentRate] = " + vmpsm.CurrentRate + " ,[SalesRate] = " + vmpsm.SalesRate + " ,[SalesValue] = " + vmpsm.SalesValue + " ,[LastSalesDate] = null,[IsActive] = 1 WHERE [ProductId] = '"+ vmpsm.ProductID + "'";
                            db.Database.ExecuteSqlCommand(sql);
                        }
                        else
                        {
                            var sql = "INSERT INTO [dbo].[ProductStockMaser] " +
                                "([StockID] ,[BranchID] ,[SupplierID] ,[CategoryID] ,[ProductID] ,[CompanyID] " +
                                ",[UOMID] ,[ReceiveQty] ,[ReceiveValue] ,[LastReceiveDate] ,[IssueQty] ,[IssueValue] " +
                                ",[LastIssueDate] ,[CurrentRate] ,[SalesRate] ,[SalesValue] ,[LastSalesDate] ,[IsActive]) " +
                                "VALUES ('" + StockID + "' , " + vmpsm.BranchID + " , " + p.SupplierId + " , " + p.ProductCategoryId + " , " +
                                vmpsm.ProductID + " , " + vmpsm.CompanyID + " , " + p.UOMId + " , " + vmpsm.ReceiveQty + " , " +
                                vmpsm.ReceiveValue + " , '" + DateTime.Now.ToShortDateString() + "' , " + vmpsm.IssueQty + " , " + vmpsm.IssueValue + " , null , " +
                                vmpsm.CurrentRate + " , " + vmpsm.SalesRate + " , " + vmpsm.SalesValue + " , null , 1)";
                            db.Database.ExecuteSqlCommand(sql);
                        }
                    }
                    Session["ProductStock"] = String.Empty;
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Create", "ProductStockMasers");
            return Json(new { Url = redirectUrl });
            //return RedirectToAction("Create");
        }

        [HttpPost]
        public ActionResult SetOneItem(List<ProductStockMaser> customers, string companyId, string branchId, string productId, string productIds = "0")
        {
            ProductStockMasersLists = Session["ProductStock"] as List<VmProductStockMaster>;
            if (ProductStockMasersLists != null)
            {
                var productStockList = new List<VmProductStockMaster>();                
                try
                {
                    foreach (VmProductStockMaster vmpsm in ProductStockMasersLists)
                    {
                        int count = customers.Where(x => x.ProductID == vmpsm.ProductID).Count();                        
                        if (count > 0)
                        {
                            vmpsm.ReceiveQty = customers.Where(x => x.ProductID == vmpsm.ProductID).FirstOrDefault().ReceiveQty;
                            vmpsm.CurrentStock = customers.Where(x => x.ProductID == vmpsm.ProductID).FirstOrDefault().ReceiveQty;
                        }
                        productStockList.Add(vmpsm);
                    }
                    Session["ProductStock"] = productStockList;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Create", "ProductStockMasers");
            return Json(new { Url = redirectUrl });
            //return RedirectToAction("Create");
        }

    }
}
