using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.ModelBinding;
//using System.Web.Mvc;
using ERP.Models;
using Microsoft.AspNetCore.Mvc;

namespace ERP.Controllers
{
    public class ProductStockDetailsController : Controller
    {
        private ERP_AmericanTMartEntities db = new ERP_AmericanTMartEntities();

        // GET: ProductStockDetails
        public ActionResult Index()
        {
            return View(db.ProductStockDetails.ToList());
        }

        // GET: ProductStockDetails/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStockDetail productStockDetail = db.ProductStockDetails.Find(id);
            if (productStockDetail == null)
            {
                return HttpNotFound();
            }
            return View(productStockDetail);
        }

        // GET: ProductStockDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductStockDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockDetailID,StockID,BranchID,TransactionID,TransactionTypeID,StockDate,SupplierID,CompanyID,CategoryID,ProductID,UOMID,ReceiveQty,ReceiveValue,ReceiveRate,IssueQty,IssueRate,IssueValue,IsActive,CreateBy,CreateOn,CreatePc,UpdateBy,UpdateOn,UpdatePc,IsDeleted,DeleteBy,DeleteOn,DeletePc")] ProductStockDetail productStockDetail)
        {
            if (ModelState.IsValid)
            {
                db.ProductStockDetails.Add(productStockDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productStockDetail);
        }

        // GET: ProductStockDetails/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStockDetail productStockDetail = db.ProductStockDetails.Find(id);
            if (productStockDetail == null)
            {
                return HttpNotFound();
            }
            return View(productStockDetail);
        }

        // POST: ProductStockDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockDetailID,StockID,BranchID,TransactionID,TransactionTypeID,StockDate,SupplierID,CompanyID,CategoryID,ProductID,UOMID,ReceiveQty,ReceiveValue,ReceiveRate,IssueQty,IssueRate,IssueValue,IsActive,CreateBy,CreateOn,CreatePc,UpdateBy,UpdateOn,UpdatePc,IsDeleted,DeleteBy,DeleteOn,DeletePc")] ProductStockDetail productStockDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productStockDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productStockDetail);
        }

        // GET: ProductStockDetails/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductStockDetail productStockDetail = db.ProductStockDetails.Find(id);
            if (productStockDetail == null)
            {
                return HttpNotFound();
            }
            return View(productStockDetail);
        }

        // POST: ProductStockDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            ProductStockDetail productStockDetail = db.ProductStockDetails.Find(id);
            db.ProductStockDetails.Remove(productStockDetail);
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
    }
}
