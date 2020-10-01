using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using ERP.Models;

namespace ERP.Controllers
{
    public class ProductStockMasersController : Controller
    {
        private ERP_AmericanTMartEntities db = new ERP_AmericanTMartEntities();

        // GET: ProductStockMasers
        public ActionResult Index()
        {
            return View(db.ProductStockMasers.ToList());
        }

        // GET: ProductStockMasers/Details/5
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

        // GET: ProductStockMasers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ProductStockMasers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "StockID,BranchID,SupplierID,CategoryID,ProductID,CompanyID,UOMID,ReceiveQty,ReceiveValue,LastReceiveDate,IssueQty,IssueValue,LastIssueDate,CurrentRate,CurrentStock,CurrentValue,SalesRate,SalesValue,LastSalesDate,IsActive,CreateBy,CreateOn,CreatePc,UpdateBy,UpdateOn,UpdatePc,IsDeleted,DeleteBy,DeleteOn,DeletePc")] ProductStockMaser productStockMaser)
        {
            if (ModelState.IsValid)
            {
                db.ProductStockMasers.Add(productStockMaser);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productStockMaser);
        }

        // GET: ProductStockMasers/Edit/5
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

        // POST: ProductStockMasers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "StockID,BranchID,SupplierID,CategoryID,ProductID,CompanyID,UOMID,ReceiveQty,ReceiveValue,LastReceiveDate,IssueQty,IssueValue,LastIssueDate,CurrentRate,CurrentStock,CurrentValue,SalesRate,SalesValue,LastSalesDate,IsActive,CreateBy,CreateOn,CreatePc,UpdateBy,UpdateOn,UpdatePc,IsDeleted,DeleteBy,DeleteOn,DeletePc")] ProductStockMaser productStockMaser)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productStockMaser).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(productStockMaser);
        }

        // GET: ProductStockMasers/Delete/5
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

        // POST: ProductStockMasers/Delete/5
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
    }
}
