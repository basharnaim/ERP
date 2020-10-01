using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ERP.WebUI.Models;

namespace ERP.WebUI.Areas.APanel.Controllers
{
    public class ProductStockDetailsController : Controller
    {
        private POS_TmartEntities db = new POS_TmartEntities();

        // GET: APanel/ProductStockDetails
        public ActionResult Index()
        {
            return View(db.ProductStockDetails.ToList());
        }

        // GET: APanel/ProductStockDetails/Details/5
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

        // GET: APanel/ProductStockDetails/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: APanel/ProductStockDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductStockDetail productStockDetail)
        {
            if (ModelState.IsValid)
            {
                db.ProductStockDetails.Add(productStockDetail);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productStockDetail);
        }

        // GET: APanel/ProductStockDetails/Edit/5
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

        // POST: APanel/ProductStockDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( ProductStockDetail productStockDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productStockDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productStockDetail);
        }

        // GET: APanel/ProductStockDetails/Delete/5
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

        // POST: APanel/ProductStockDetails/Delete/5
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
