using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DotLiquid.Tags;
using ERP.Models;

namespace ERP.Controllers
{
    public class OpeningBlancesController : Controller
    {
        private ERP_AmericanTMartEntities db = new ERP_AmericanTMartEntities();

        // GET: OpeningBlances
        public ActionResult Index()
        {
            return View(db.OpeningBlances.ToList());
        }

        private ActionResult View(List<OpeningBlance> lists)
        {
            throw new NotImplementedException();
        }

        // GET: OpeningBlances/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new Microsoft.AspNet.Mvc.HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpeningBlance openingBlance = db.OpeningBlances.Find(id);
            if (openingBlance == null)
            {
                return HttpNotFound();
            }
            return View(openingBlance);
        }

        // GET: OpeningBlances/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OpeningBlances/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductId,TransectionNo,TransectionType,TransectionDate,TransectionQty,TransectionQtyPrev,StockQty,StockUnitPrice,StockAmount,CompanyId,BranchId,SupplierId,BrandId,Active,Archive,IsSynchronized,IsUpdated,SynchronizationType,Remarks,AddedBy,AddedDate,AddedFromIp,UpdatedBy,UpdatedDate,UpdatedFromIp")] OpeningBlance openingBlance)
        {
            if (ModelState.IsValid)
            {
                db.OpeningBlances.Add(openingBlance);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(openingBlance);
        }

        // GET: OpeningBlances/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpeningBlance openingBlance = db.OpeningBlances.Find(id);
            if (openingBlance == null)
            {
                return HttpNotFound();
            }
            return View(openingBlance);
        }

        // POST: OpeningBlances/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductId,TransectionNo,TransectionType,TransectionDate,TransectionQty,TransectionQtyPrev,StockQty,StockUnitPrice,StockAmount,CompanyId,BranchId,SupplierId,BrandId,Active,Archive,IsSynchronized,IsUpdated,SynchronizationType,Remarks,AddedBy,AddedDate,AddedFromIp,UpdatedBy,UpdatedDate,UpdatedFromIp")] OpeningBlance openingBlance)
        {
            if (ModelState.IsValid)
            {
                db.Entry(openingBlance).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(openingBlance);
        }

        // GET: OpeningBlances/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OpeningBlance openingBlance = db.OpeningBlances.Find(id);
            if (openingBlance == null)
            {
                return HttpNotFound();
            }
            return View(openingBlance);
        }

        // POST: OpeningBlances/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OpeningBlance openingBlance = db.OpeningBlances.Find(id);
            db.OpeningBlances.Remove(openingBlance);
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
