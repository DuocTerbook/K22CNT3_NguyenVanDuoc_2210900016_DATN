using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class KhachHangsController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: KhachHangs
        public ActionResult Index()
        {
            return View(db.KhachHangs.ToList());
        }

        // GET: KhachHangs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null)
                return HttpNotFound();

            return View(khachHang);
        }

        // GET: KhachHangs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: KhachHangs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                db.KhachHangs.Add(khachHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khachHang);
        }

        // GET: KhachHangs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null)
                return HttpNotFound();

            return View(khachHang);
        }

        // POST: KhachHangs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(KhachHang khachHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(khachHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(khachHang);
        }

        // GET: KhachHangs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            KhachHang khachHang = db.KhachHangs.Find(id);
            if (khachHang == null)
                return HttpNotFound();

            return View(khachHang);
        }

        // POST: KhachHangs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            KhachHang khachHang = db.KhachHangs.Find(id);

            if (khachHang == null)
                return HttpNotFound();

            db.KhachHangs.Remove(khachHang);
            db.SaveChanges();

            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();

            base.Dispose(disposing);
        }
    }
}
