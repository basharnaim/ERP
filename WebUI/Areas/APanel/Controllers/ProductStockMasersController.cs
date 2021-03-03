using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using ERP.WebUI.Models;
using ERP.WebUI.Models.ViewModels;
using Library.Context.Repositories;
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
        private readonly IRawSqlService _rawSqlService;
        public ProductStockMasersController(ISaleService saleService, IRawSqlService rawSqlService)
        {
            _saleService = saleService;
            _rawSqlService = rawSqlService;
        }
        // GET: APanel/ProductStockMasers
        public ActionResult Index(List<ProductStockMaser> productStockMasters, string companyId, string branchId, string productCategoryId, string productSubCategoryId, string supplierId, string productId, string productcode)
        {
            try
            {
                var result = new List<VmProductStockMaster>().AsEnumerable();
                ProductStockMasersList = Session["ProductStock"] as List<VmProductStockMaster>;
                if (ProductStockMasersList == null)
                {
                    ProductStockMasersList = new List<VmProductStockMaster>();
                }

                if (productcode != null && productcode != "")
                {
                    productId = db.Products.Where(x => x.Code == productcode).FirstOrDefault().Id;
                    result = getdataByProductId(productId);
                }
                if (!string.IsNullOrEmpty(productId))
                {
                    supplierId = "";
                    productCategoryId = "";
                    productSubCategoryId = "";
                    result = getdataByProductId(productId);
                    var count = ProductStockMasersList.Where(x => x.ProductID == productId).Count();
                    if (count == 0)
                    {
                        ProductStockMasersList.AddRange(result);
                    }
                }
                if (!string.IsNullOrEmpty(supplierId))
                {
                    result = getdataByProductId(productSubCategoryId, supplierId);
                    productCategoryId = "";
                    productSubCategoryId = "";
                    ProductStockMasersList = new List<VmProductStockMaster>();
                    ProductStockMasersList.AddRange(result);
                }
                if (!string.IsNullOrEmpty(productSubCategoryId))
                {
                    result = getdataByProductId("", productCategoryId, productSubCategoryId);
                    productCategoryId = "";
                    ProductStockMasersList = new List<VmProductStockMaster>();
                    ProductStockMasersList.AddRange(result);
                }
                if (!string.IsNullOrEmpty(productCategoryId))
                {
                    result = getdataByProductId("", "", "", productCategoryId);
                    ProductStockMasersList = new List<VmProductStockMaster>();
                    ProductStockMasersList.AddRange(result);
                }

                ProductStockMasersLists = ProductStockMasersList.AsEnumerable();
                Session["ProductStock"] = ProductStockMasersLists;
                ViewBag.ProductList = ProductStockMasersLists;
                return View(ProductStockMasersLists);

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
        public ActionResult Create(List<ProductStockMaser> productStockMasters, string companyId, string branchId, string productCategoryId, string productSubCategoryId, string supplierId, string productId, string productcode)
        {
            var result = new List<VmProductStockMaster>().AsEnumerable();
            ProductStockMasersList = Session["ProductStock"] as List<VmProductStockMaster>;
            if (ProductStockMasersList == null)
            {
                ProductStockMasersList = new List<VmProductStockMaster>();
            }

            if (productcode != null && productcode != "")
            {
                productId = db.Products.Where(x => x.Code == productcode).FirstOrDefault().Id;
                result = getdataByProductId(productId);
            }
            if (!string.IsNullOrEmpty(productId))
            {
                supplierId = "";
                productCategoryId = "";
                productSubCategoryId = "";
                result = getdataByProductId(productId);
                var count = ProductStockMasersList.Where(x => x.ProductID == productId).Count();
                if (count == 0)
                {
                    ProductStockMasersList.AddRange(result);
                }
            }
            if (!string.IsNullOrEmpty(supplierId))
            {
                result = getdataByProductId(productSubCategoryId, supplierId);
                productCategoryId = "";
                productSubCategoryId = "";
                ProductStockMasersList = new List<VmProductStockMaster>();
                ProductStockMasersList.AddRange(result);
            }
            if (!string.IsNullOrEmpty(productSubCategoryId))
            {
                result = getdataByProductId("", productCategoryId, productSubCategoryId);
                productCategoryId = "";
                ProductStockMasersList = new List<VmProductStockMaster>();
                ProductStockMasersList.AddRange(result);
            }
            if (!string.IsNullOrEmpty(productCategoryId))
            {
                result = getdataByProductId("", "", "", productCategoryId);
                ProductStockMasersList = new List<VmProductStockMaster>();
                ProductStockMasersList.AddRange(result);
            }

            ProductStockMasersLists = ProductStockMasersList.AsEnumerable();
            Session["ProductStock"] = ProductStockMasersLists;
            ViewBag.ProductList = ProductStockMasersLists;
            return View(ProductStockMasersLists);

            //if (!string.IsNullOrEmpty(productId))
            //{
            //    var result = getdataByProductId(productId);
            //    //if(!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(supplierId))
            //    //{
            //    //    result = getdataByProductId(productId, supplierId);
            //    //}
            //    //else if(!string.IsNullOrEmpty(productId) && !string.IsNullOrEmpty(supplierId) && !string.IsNullOrEmpty(supplierId))
            //    //{
            //    //    result = getdataByProductId(productId, supplierId);
            //    //}
            //    int count = 0;
            //    ProductStockMasersList = Session["ProductStock"] as List<VmProductStockMaster>;
            //    if (ProductStockMasersList == null)
            //    {
            //        ProductStockMasersList = new List<VmProductStockMaster>();
            //    }
            //    else
            //    {
            //        count = ProductStockMasersList.Where(x => x.ProductID == result.FirstOrDefault().ProductID).Count();
            //    }
            //    if (count == 0)
            //    {
            //        ProductStockMasersList.AddRange(result);
            //    }
            //    ProductStockMasersLists = ProductStockMasersList.AsEnumerable();
            //    Session["ProductStock"] = ProductStockMasersLists;
            //    ViewBag.ProductList = ProductStockMasersLists;
            //    return View(ProductStockMasersLists);
            //}
            //else
            //{
            //    ProductStockMasersList = Session["ProductStock"] as List<VmProductStockMaster>;
            //    ProductStockMasersLists = ProductStockMasersList.AsEnumerable();
            //    return View(ProductStockMasersLists);
            //}
        }

        private object getdataByCategoryId(string productCategoryId)
        {
            throw new NotImplementedException();
        }

        private object getdataBySubCategoryId(string productSubCategoryId)
        {
            throw new NotImplementedException();
        }

        private object getdataBySupplierId(string productId)
        {
            throw new NotImplementedException();
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
                                  CurrentRate = pm.CurrentRate,
                                  ProductID = p.Id,
                                  ProductName = p.Name,
                                  ProductCode = p.Code,
                                  ProdcutDescription = p.Description,
                                  IsActive = p.Active,
                                  UOMName = pu.Name,
                                  BranchName = pbr.Name,
                                  CategoryName = pc.Name
                              }).AsEnumerable();

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
        public ActionResult Edit(List<ProductStockMaser> productStockMasters, string companyId, string branchId, string productId, string productcode)
        {

            if (productcode != null && productcode != "")
            {
                productId = db.Products.Where(x => x.Code == productcode).FirstOrDefault().Id;
            }
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
                              where p.Id == productId
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
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            //ProductStockMaser productStockMaser = db.ProductStockMasers.Find(id);
            //if (productStockMaser == null)
            //{
            //    return HttpNotFound();
            //}
            //return View(productStockMaser);
        }

        // POST: APanel/ProductStockMasers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(List<ProductStockMaser> customers, string companyId, string branchId, string productId)
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
            //    db.Entry(productStockMaser).State = EntityState.Modified;
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}
            //return View(productStockMaser);
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
            if (ProductStockMasersLists != null)
            {
                var productStockList = new List<ProductStockMaser>();
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                //_saleService.GenerateAutoId(identity.CompanyId, identity.BranchId, "ProductStockMaser");

                try
                {
                    foreach (VmProductStockMaster vmpsm in ProductStockMasersLists)
                    {
                        var stockId = "0";
                        int count = db.ProductStockMasers.Where(x => x.ProductID == vmpsm.ProductID).Count();
                        Product p = db.Products.Where(x => x.Id == vmpsm.ProductID).FirstOrDefault();
                        if (count > 0)
                        {
                            stockId = vmpsm.StockID;
                            var sql = "UPDATE [dbo].[ProductStockMaser] SET " +
                               "[BranchID] = " + vmpsm.BranchID + ",[SupplierID] = " + vmpsm.SupplierID + " ,[CategoryID]= " + vmpsm.CategoryID + " ,[CompanyID] = " + vmpsm.CompanyID + " " +
                               ",[UOMID]= " + vmpsm.UOMID + " ,[ReceiveQty] = " + vmpsm.ReceiveQty + " ,[ReceiveValue] = " + vmpsm.ReceiveValue + " ,[LastReceiveDate] = '" + DateTime.Now.ToShortDateString() + "',[IssueQty] = " + vmpsm.IssueQty + " ,[IssueValue] = " + vmpsm.IssueValue +
                               ",[LastIssueDate] = null ,[CurrentRate] = " + vmpsm.CurrentRate + " ,[SalesRate] = " + vmpsm.SalesRate + " ,[SalesValue] = " + vmpsm.SalesValue + " , UpdateBy = 1, UpdateOn ='" + DateTime.Now.ToShortDateString() + "', UpdatePc ='" + identity.IpAddress + "',[LastSalesDate] = null,[IsActive] = 1 " +
                               "WHERE [ProductId] = '" + vmpsm.ProductID + "'";
                            db.Database.ExecuteSqlCommand(sql);
                        }
                        else
                        {
                            stockId = (db.ProductStockMasers.Count() + 1).ToString();
                            var sql = "INSERT INTO [dbo].[ProductStockMaser] " +
                                "([StockID],[BranchID] ,[SupplierID] ,[CategoryID] ,[ProductID] ,[CompanyID] " +
                                ",[UOMID] ,[ReceiveQty] ,[ReceiveValue] ,[LastReceiveDate] ,[IssueQty] ,[IssueValue] " +
                                ",[LastIssueDate] ,[CurrentRate] ,[SalesRate] ,[SalesValue], CreateBy, CreateOn, CreatePc ,[LastSalesDate] ,[IsActive]) " +
                                "VALUES (" + stockId + " , " + identity.BranchId + " , " + p.SupplierId + " , " + p.ProductCategoryId + " , " +
                                vmpsm.ProductID + " , " + identity.CompanyId + " , " + p.UOMId + " , " + vmpsm.ReceiveQty + " , " + vmpsm.ReceiveValue + " , '" + DateTime.Now.ToShortDateString() + "' , " + vmpsm.IssueQty + " , " + vmpsm.IssueValue + " , null , " +
                                vmpsm.CurrentRate + " , " + vmpsm.SalesRate + " , " + vmpsm.SalesValue + ", 1 ,'" + DateTime.Now.ToShortDateString() + "','" + identity.IpAddress + "', null , 1)";
                            db.Database.ExecuteSqlCommand(sql);

                            //var sqlTest = "INSERT INTO test_product(product_name, brand_id, category_id, model_year, list_price) " +
                            //    "VALUES('Test2 product', 1, 1, 2018, 599);";
                            //db.Database.ExecuteSqlCommand(sqlTest);
                        }
                        int dcount = db.ProductStockDetails.Where(x => x.StockID.ToString() == vmpsm.StockID && x.TransactionTypeID == 1).Count();
                        int type = 1;
                        if (dcount > 0)
                        {
                            var sql = "UPDATE [dbo].[ProductStockDetail] SET " +
                               "[BranchID] = " + vmpsm.BranchID + ",[SupplierID] = " + vmpsm.SupplierID + " ,[CategoryID]= " + vmpsm.CategoryID + " ,[CompanyID] = " + vmpsm.CompanyID + " " +
                               ",[UOMID]= " + vmpsm.UOMID + " ,[ReceiveQty] = " + vmpsm.ReceiveQty + " ,[ReceiveValue] = " + vmpsm.ReceiveValue + " ,[IssueQty] = " + vmpsm.IssueQty + " ,[IssueValue] = " + vmpsm.IssueValue +
                               ", UpdateOn ='" + DateTime.Now.ToShortDateString() + "',UpdateBy = 1, UpdatePc ='" + identity.IpAddress + "',[IsActive] = 1 " +
                               "WHERE [ProductId] = '" + vmpsm.ProductID + "'";
                            db.Database.ExecuteSqlCommand(sql);
                        }
                        else
                        {
                            var sql = "INSERT INTO [dbo].[ProductStockDetail] " +
                                "(TransactionTypeID, [TransactionID], [StockID], [StockDate],[BranchID] ,[SupplierID] ,[CategoryID] ,[ProductID] ,[CompanyID] " +
                                ",[UOMID] ,[ReceiveQty] ,[ReceiveValue] ,[IssueQty] ,[IssueValue] " +
                                ", CreateBy, CreateOn, CreatePc ,[IsActive], IsDeleted ) " +
                                "VALUES (" + type + " , '" + 1111 + "' ," + stockId + " ,'" + DateTime.Now.ToShortDateString() + "'," + identity.BranchId + " , " + p.SupplierId + " , " + p.ProductCategoryId + " , " +
                                vmpsm.ProductID + " , " + identity.CompanyId + " , " + p.UOMId + " , " + vmpsm.ReceiveQty + " , " +
                                vmpsm.ReceiveValue + " , " + vmpsm.IssueQty + " , " + vmpsm.IssueValue + ", 1 ,'" + DateTime.Now.ToShortDateString() + "','" + identity.IpAddress + "', 1, 0)";
                            db.Database.ExecuteSqlCommand(sql);

                            //var sqlTest = "INSERT INTO test_product(product_name, brand_id, category_id, model_year, list_price) " +
                            //    "VALUES('Test2 product', 1, 1, 2018, 599);";
                            //db.Database.ExecuteSqlCommand(sqlTest);
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
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "ProductStockMasers");
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
                            vmpsm.ReceiveValue = vmpsm.ReceiveQty * vmpsm.ReceiveRate;
                            vmpsm.SalesValue = (decimal)vmpsm.ReceiveQty * vmpsm.SalesRate;
                            vmpsm.CurrentStock = customers.Where(x => x.ProductID == vmpsm.ProductID).FirstOrDefault().ReceiveQty;
                            vmpsm.CurrentRate = (decimal)vmpsm.ReceiveRate;
                            vmpsm.CurrentValue = (decimal)vmpsm.ReceiveQty * (decimal)vmpsm.ReceiveRate;
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
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "ProductStockMasers");
            return Json(new { Url = redirectUrl });
            //return RedirectToAction("Create");
        }

        public ActionResult RemoveItem()
        {
            Session["ProductStock"] = null;
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("StockIn", "ProductStockMasers");
            return Json(new { Url = redirectUrl });

        }
        public ActionResult RemoveOneItem(string Id)
        {
            ProductStockMasersLists = Session["ProductStock"] as List<VmProductStockMaster>;
            if (ProductStockMasersLists != null)
            {
                var productStockList = new List<VmProductStockMaster>();
                try
                {
                    var sql = "UPDATE [dbo].[ProductStockMaser] SET [IsDeleted] = 1 WHERE [ProductId] = '" + Id + "'";
                    db.Database.ExecuteSqlCommand(sql);
                    foreach (VmProductStockMaster vmpsm in ProductStockMasersLists)
                    {

                        if (vmpsm.ProductID != Id)
                        {
                            productStockList.Add(vmpsm);
                        }
                    }
                    Session["ProductStock"] = null;
                    Session["ProductStock"] = productStockList;
                }
                catch (Exception e)
                {
                    return null;
                }
            }
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("Index", "ProductStockMasers");
            return Json(new { Url = redirectUrl });
        }
        [HttpGet]
        public JsonResult StockInJson(string companyId = "0", string branchId = "0", string productCategoryId = "0", string productSubCategoryId = "0", string supplierId = "0", string productId = "0", string productcode = "")
        {
            var data = _rawSqlService.GetStockInProductList(companyId, branchId, productCategoryId, productSubCategoryId, supplierId, productId, productcode);
            return Json(data, JsonRequestBehavior.AllowGet);
        }
        public ActionResult StockIn(string companyId = "0", string branchId = "0", string productCategoryId = "0", string productSubCategoryId = "0", string supplierId = "0", string productId = "0", string productcode = "")
        {
            DataSet data = _rawSqlService.GetStockInProductList(companyId, branchId, productCategoryId, productSubCategoryId, supplierId, productId, productcode);
                 
            var productStocks = new List<VmProductStockMaster>();
            foreach (DataRow dr in data.Tables[0].Rows)
            {
                var vmpsm = new VmProductStockMaster();
                vmpsm.ProductID = dr["ProductID"].ToString();
                vmpsm.ProductName = dr["ProductName"].ToString();
                vmpsm.ProductCode = dr["ProductCode"].ToString();
                vmpsm.ProdcutDescription = dr["ProdcutDescription"].ToString();
                vmpsm.UOMName =  dr["UOMName"].ToString();

                vmpsm.BranchID = Convert.ToInt32(dr["BranchID"].ToString()); 
                vmpsm.CompanyID = Convert.ToInt32(dr["CompanyID"].ToString());
                vmpsm.SupplierID = Convert.ToInt32(dr["SupplierID"].ToString());

                vmpsm.ReceiveRate = (decimal)dr["ReceiveRate"];
                vmpsm.ReceiveQty = (decimal)dr["ReceiveQty"];
                vmpsm.CurrentStock = (decimal)dr["CurrentStock"];
                vmpsm.ReceiveValue = vmpsm.CurrentStock * vmpsm.ReceiveRate;
                vmpsm.SalesRate = (decimal)dr["SalesRate"];
                vmpsm.SalesValue = (decimal)vmpsm.CurrentStock * vmpsm.SalesRate;                         
                vmpsm.CurrentValue = (decimal)vmpsm.CurrentStock * (decimal)vmpsm.ReceiveRate;                
                productStocks.Add(vmpsm);
            }             
            return View(productStocks);
        }

        [HttpPost]
        public ActionResult StockIn(List<VmProductStockMaster> productStockIn, string companyId, string branchId, string productCategoryId, string productSubCategoryId, string supplierId, string productId, string productcode)
        {
           // ProductStockMasersLists = Session["ProductStock"] as List<VmProductStockMaster>;
            if (productStockIn != null)
            {
                var productStockList = new List<ProductStockMaser>();
                var identity = (LoginIdentity)Thread.CurrentPrincipal.Identity;
                //_saleService.GenerateAutoId(identity.CompanyId, identity.BranchId, "ProductStockMaser");

                try
                {
                    foreach (VmProductStockMaster vmpsm in productStockIn)
                    {
                        var stockId = "0";
                        int count = db.ProductStockMasers.Where(x => x.ProductID == vmpsm.ProductID).Count();
                        Product p = db.Products.Where(x => x.Id == vmpsm.ProductID).FirstOrDefault();
                        vmpsm.ReceiveValue = vmpsm.ReceiveQty * p.PurchasePrice;
                        vmpsm.SalesRate = p.RetailPrice;
                        vmpsm.SalesValue = (decimal)vmpsm.ReceiveQty * p.RetailPrice;
                        if (count > 0)
                        {
                            var pid = vmpsm.ProductID.ToString();
                            stockId = (db.ProductStockMasers.Count() + 1).ToString();
                            var sql = "UPDATE [dbo].[ProductStockMaser] SET " +
                               "[BranchID] = " + identity.BranchId + ",[CompanyID] = " + identity.CompanyId + " " +
                               ",[UOMID]= " + p.UOMId + " ,[ReceiveQty] = " + vmpsm.ReceiveQty + " ,[ReceiveValue] = " + vmpsm.ReceiveValue + " ,[LastReceiveDate] = '" + DateTime.Now.ToShortDateString() + "',[IssueQty] = " + vmpsm.IssueQty + " ,[IssueValue] = " + vmpsm.IssueValue +
                               ",[LastIssueDate] = null ,[CurrentRate] = " + p.RetailPrice + " ,[SalesRate] = " + vmpsm.SalesRate + " ,[SalesValue] = " + vmpsm.SalesValue + " , UpdateBy = 1, UpdateOn ='" + DateTime.Now.ToShortDateString() + "', UpdatePc ='" + identity.IpAddress + "',[LastSalesDate] = null,[IsActive] = 1 " +
                               "WHERE [ProductId] = " + pid + "";
                            db.Database.ExecuteSqlCommand(sql);
                        }
                        else
                        {
                            stockId = (db.ProductStockMasers.Count() + 1).ToString();
                            var sql = "INSERT INTO [dbo].[ProductStockMaser] " +
                                "([StockID],[BranchID] ,[SupplierID] ,[CategoryID] ,[ProductID] ,[CompanyID] " +
                                ",[UOMID] ,[ReceiveQty] ,[ReceiveValue] ,[LastReceiveDate] ,[IssueQty] ,[IssueValue] " +
                                ",[LastIssueDate] ,[CurrentRate] ,[SalesRate] ,[SalesValue], CreateBy, CreateOn, CreatePc ,[LastSalesDate] ,[IsActive]) " +
                                "VALUES (" + stockId + " , " + identity.BranchId + " , " + p.SupplierId + " , " + p.ProductCategoryId + " , " +
                                vmpsm.ProductID + " , " + identity.CompanyId + " , " + p.UOMId + " , " + vmpsm.ReceiveQty + " , " + vmpsm.ReceiveValue + " , '" + DateTime.Now.ToShortDateString() + "' , " + vmpsm.IssueQty + " , " + vmpsm.IssueValue + " , null , " +
                                 p.RetailPrice + " , " + vmpsm.SalesRate + " , " + vmpsm.SalesValue + ", 1 ,'" + DateTime.Now.ToShortDateString() + "','" + identity.IpAddress + "', null , 1)";
                            db.Database.ExecuteSqlCommand(sql);

                            //var sqlTest = "INSERT INTO test_product(product_name, brand_id, category_id, model_year, list_price) " +
                            //    "VALUES('Test2 product', 1, 1, 2018, 599);";
                            //db.Database.ExecuteSqlCommand(sqlTest);
                        }
                        int dcount = db.ProductStockDetails.Where(x => x.StockID.ToString() == vmpsm.StockID && x.TransactionTypeID == 1).Count();
                        int type = 1;
                        if (dcount > 0)
                        {
                            var sql = "UPDATE [dbo].[ProductStockDetail] SET " +
                               "[BranchID] = " + identity.BranchId + ",[SupplierID] = " + vmpsm.SupplierID + " ,[CompanyID] = " + identity.CompanyId + " " +
                               ",[UOMID]= " + p.UOMId + " ,[ReceiveQty] = " + vmpsm.ReceiveQty + " ,[ReceiveValue] = " + vmpsm.ReceiveValue + " ,[IssueQty] = " + vmpsm.IssueQty + " ,[IssueValue] = " + vmpsm.IssueValue +
                               ", UpdateOn ='" + DateTime.Now.ToShortDateString() + "',UpdateBy = 1, UpdatePc ='" + identity.IpAddress + "',[IsActive] = 1 " +
                               "WHERE [ProductId] = '" + vmpsm.ProductID + "'";
                            db.Database.ExecuteSqlCommand(sql);
                        }
                        else
                        {
                            //stockId = (db.ProductStockMasers.Count() + 1).ToString();
                            var sql = "INSERT INTO [dbo].[ProductStockDetail] " +
                                "(TransactionTypeID, [TransactionID], [StockID], [StockDate],[BranchID] ,[SupplierID] ,[CategoryID] ,[ProductID] ,[CompanyID] " +
                                ",[UOMID] ,[ReceiveQty] ,[ReceiveValue] ,[IssueQty] ,[IssueValue] " +
                                ", CreateBy, CreateOn, CreatePc ,[IsActive], IsDeleted ) " +
                                "VALUES (" + type + " , '" + 1111 + "' ," + stockId + " ,'" + DateTime.Now.ToShortDateString() + "'," + identity.BranchId + " , " + p.SupplierId + " , " + p.ProductCategoryId + " , " +
                                vmpsm.ProductID + " , " + identity.CompanyId + " , " + p.UOMId + " , " + vmpsm.ReceiveQty + " , " +
                                vmpsm.ReceiveValue + " , " + vmpsm.IssueQty + " , " + vmpsm.IssueValue + ", 1 ,'" + DateTime.Now.ToShortDateString() + "','" + identity.IpAddress + "', 1, 0)";
                            db.Database.ExecuteSqlCommand(sql);

                            //var sqlTest = "INSERT INTO test_product(product_name, brand_id, category_id, model_year, list_price) " +
                            //    "VALUES('Test2 product', 1, 1, 2018, 599);";
                            //db.Database.ExecuteSqlCommand(sqlTest);
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
            //var redirectUrl = new UrlHelper(Request.RequestContext).Action("StockIn", "ProductStockMasers");
            var redirectUrl = new UrlHelper(Request.RequestContext).Action("StockInJson", "ProductStockMasers");
            return Json(new { Url = redirectUrl });
            //var data = _rawSqlService.GetStockOutProductList(companyId, branchId, productCategoryId, productSubCategoryId, supplierId, productId, productcode);
            //return View(_rawSqlService.GetStockInProductList(companyId, branchId, productCategoryId, productSubCategoryId, supplierId, productId, productcode));
        }
        public ActionResult StockOut(string companyId, string branchId, string productCategoryId, string productSubCategoryId, string supplierId, string productId, string productcode)
        {   
            //var data = _rawSqlService.GetStockOutProductList(companyId, branchId, productCategoryId, productSubCategoryId, supplierId, productId, productcode);
            return View(_rawSqlService.GetStockOutProductList(companyId, branchId, productCategoryId, productSubCategoryId, supplierId, productId, productcode));
        }

        private IEnumerable<VmProductStockMaster> getdataByProductId(string productId)
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
                          where p.Id == productId
                          select new VmProductStockMaster
                          {
                              StockID = (pm.StockID == "" ? "0" : pm.StockID),
                              CategoryID = (pm.CategoryID == null ? 0 : pm.CategoryID),
                              CompanyID = (pm.CompanyID == null ? 0 : pm.CompanyID),
                              BranchID = (pm.BranchID == null ? 0 : pm.BranchID),
                              SupplierID = (pm.SupplierID == null ? 0 : pm.SupplierID),
                              UOMID = (pm.UOMID == null ? 0 : pm.UOMID),
                              ReceiveRate = p.PurchasePrice,
                              ReceiveQty = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                              ReceiveValue = (pm.ReceiveValue == null ? 0 : pm.ReceiveValue),

                              SalesRate = p.RetailPrice,
                              SalesValue = (pm.SalesValue == null ? 0 : pm.SalesValue),
                              CurrentStock = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                              CurrentRate = (pm.CurrentRate == null ? 0 : pm.CurrentRate),
                              CurrentValue = (pm.CurrentValue == null ? 0 : pm.CurrentValue),
                              ProductID = p.Id,
                              ProductName = p.Name,
                              ProductCode = p.Code,
                              ProdcutDescription = p.Description,
                              IsActive = p.Active,
                              UOMName = pu.Name,
                              BranchName = pbr.Name,
                              CategoryName = pc.Name
                          }).AsEnumerable();
            return result;
        }
        private IEnumerable<VmProductStockMaster> getdataByProductId(string subId, string supplierId)
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
                          where p.SupplierId == supplierId && p.ProductSubCategoryId == subId
                          select new VmProductStockMaster
                          {
                              StockID = (pm.StockID == "" ? "0" : pm.StockID),
                              CategoryID = (pm.CategoryID == null ? 0 : pm.CategoryID),
                              CompanyID = (pm.CompanyID == null ? 0 : pm.CompanyID),
                              BranchID = (pm.BranchID == null ? 0 : pm.BranchID),
                              SupplierID = (pm.SupplierID == null ? 0 : pm.SupplierID),
                              UOMID = (pm.UOMID == null ? 0 : pm.UOMID),
                              ReceiveRate = p.PurchasePrice,
                              ReceiveQty = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                              ReceiveValue = (pm.ReceiveValue == null ? 0 : pm.ReceiveValue),

                              SalesRate = p.RetailPrice,
                              SalesValue = (pm.SalesValue == null ? 0 : pm.SalesValue),
                              CurrentStock = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                              CurrentRate = (pm.CurrentRate == null ? 0 : pm.CurrentRate),
                              CurrentValue = (pm.CurrentValue == null ? 0 : pm.CurrentValue),
                              ProductID = p.Id,
                              ProductName = p.Name,
                              ProductCode = p.Code,
                              ProdcutDescription = p.Description,
                              IsActive = p.Active,
                              UOMName = pu.Name,
                              BranchName = pbr.Name,
                              CategoryName = pc.Name
                          }).AsEnumerable();
            return result;
        }
        private IEnumerable<VmProductStockMaster> getdataByProductId(string proid, string categoryId, string subCategoryId)
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
                          where p.ProductSubCategoryId == subCategoryId && p.ProductCategoryId == categoryId && !pm.IsDeleted 
                          select new VmProductStockMaster
                          {
                              StockID = (pm.StockID == "" ? "0" : pm.StockID),
                              CategoryID = (pm.CategoryID == null ? 0 : pm.CategoryID),
                              CompanyID = (pm.CompanyID == null ? 0 : pm.CompanyID),
                              BranchID = (pm.BranchID == null ? 0 : pm.BranchID),
                              SupplierID = (pm.SupplierID == null ? 0 : pm.SupplierID),
                              UOMID = (pm.UOMID == null ? 0 : pm.UOMID),
                              ReceiveRate = p.PurchasePrice,
                              ReceiveQty = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                              ReceiveValue = (pm.ReceiveValue == null ? 0 : pm.ReceiveValue),

                              SalesRate = p.RetailPrice,
                              SalesValue = (pm.SalesValue == null ? 0 : pm.SalesValue),
                              CurrentStock = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                              CurrentRate = (pm.CurrentRate == null ? 0 : pm.CurrentRate),
                              CurrentValue = (pm.CurrentValue == null ? 0 : pm.CurrentValue),
                              ProductID = p.Id,
                              ProductName = p.Name,
                              ProductCode = p.Code,
                              ProdcutDescription = p.Description,
                              IsActive = p.Active,
                              UOMName = pu.Name,
                              BranchName = pbr.Name,
                              CategoryName = pc.Name
                          }).AsEnumerable();
            return result;
        }
        private IEnumerable<VmProductStockMaster> getdataByProductId(string proid, string pid, string pcid, string categoryId)
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
                          where p.ProductCategoryId == categoryId
                          select new VmProductStockMaster
                          {
                              StockID = (pm.StockID == "" ? "0" : pm.StockID),
                              CategoryID = (pm.CategoryID == null ? 0 : pm.CategoryID),
                              CompanyID = (pm.CompanyID == null ? 0 : pm.CompanyID),
                              BranchID = (pm.BranchID == null ? 0 : pm.BranchID),
                              SupplierID = (pm.SupplierID == null ? 0 : pm.SupplierID),
                              UOMID = (pm.UOMID == null ? 0 : pm.UOMID),
                              ReceiveRate = p.PurchasePrice,
                              ReceiveQty = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                              ReceiveValue = (pm.ReceiveValue == null ? 0 : pm.ReceiveValue),

                              SalesRate = p.RetailPrice,
                              SalesValue = (pm.SalesValue == null ? 0 : pm.SalesValue),
                              CurrentStock = (pm.ReceiveQty == null ? 0 : pm.ReceiveQty),
                              CurrentRate = (pm.CurrentRate == null ? 0 : pm.CurrentRate),
                              CurrentValue = (pm.CurrentValue == null ? 0 : pm.CurrentValue),
                              ProductID = p.Id,
                              ProductName = p.Name,
                              ProductCode = p.Code,
                              ProdcutDescription = p.Description,
                              IsActive = p.Active,
                              UOMName = pu.Name,
                              BranchName = pbr.Name,
                              CategoryName = pc.Name
                          }).AsEnumerable();
            return result;
        }

        private static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();
            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }
            return data;
        }
        private static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == column.ColumnName)
                        pro.SetValue(obj, dr[column.ColumnName], null);
                    else
                        continue;
                }
            }
            return obj;
        }
    }
}
