using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Views
{
    public class ChiTietPhieuNhapsController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: ChiTietPhieuNhaps
        public ActionResult Index()
        {
            var chiTietPhieuNhaps = db.ChiTietPhieuNhaps.Include(c => c.PhieuNhap).Include(c => c.SanPham);
            return View(chiTietPhieuNhaps.ToList());
        }

        // GET: ChiTietPhieuNhaps/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietPhieuNhap chiTietPhieuNhap = db.ChiTietPhieuNhaps.Find(id);
            if (chiTietPhieuNhap == null)
            {
                return HttpNotFound();
            }
            return View(chiTietPhieuNhap);
        }

        // GET: ChiTietPhieuNhaps/Create
        public ActionResult Create()
        {
            ViewBag.ID_PhieuNhap = new SelectList(db.PhieuNhaps, "ID_PhieuNhap", "ID_PhieuNhap");
            ViewBag.ID_SP = new SelectList(db.SanPhams, "ID_SP", "TenSP");
            return View();
        }

        // POST: ChiTietPhieuNhaps/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_CTPhieuNhap,ID_PhieuNhap,ID_SP,SoLuong,DonGiaNhap")] ChiTietPhieuNhap chiTietPhieuNhap)
        {
            if (ModelState.IsValid)
            {
                db.ChiTietPhieuNhaps.Add(chiTietPhieuNhap);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_PhieuNhap = new SelectList(db.PhieuNhaps, "ID_PhieuNhap", "ID_PhieuNhap", chiTietPhieuNhap.ID_PhieuNhap);
            ViewBag.ID_SP = new SelectList(db.SanPhams, "ID_SP", "TenSP", chiTietPhieuNhap.ID_SP);
            return View(chiTietPhieuNhap);
        }

        // GET: ChiTietPhieuNhaps/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietPhieuNhap chiTietPhieuNhap = db.ChiTietPhieuNhaps.Find(id);
            if (chiTietPhieuNhap == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_PhieuNhap = new SelectList(db.PhieuNhaps, "ID_PhieuNhap", "ID_PhieuNhap", chiTietPhieuNhap.ID_PhieuNhap);
            ViewBag.ID_SP = new SelectList(db.SanPhams, "ID_SP", "TenSP", chiTietPhieuNhap.ID_SP);
            return View(chiTietPhieuNhap);
        }

        // POST: ChiTietPhieuNhaps/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_CTPhieuNhap,ID_PhieuNhap,ID_SP,SoLuong,DonGiaNhap")] ChiTietPhieuNhap chiTietPhieuNhap)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiTietPhieuNhap).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID_PhieuNhap = new SelectList(db.PhieuNhaps, "ID_PhieuNhap", "ID_PhieuNhap", chiTietPhieuNhap.ID_PhieuNhap);
            ViewBag.ID_SP = new SelectList(db.SanPhams, "ID_SP", "TenSP", chiTietPhieuNhap.ID_SP);
            return View(chiTietPhieuNhap);
        }

        // GET: ChiTietPhieuNhaps/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ChiTietPhieuNhap chiTietPhieuNhap = db.ChiTietPhieuNhaps.Find(id);
            if (chiTietPhieuNhap == null)
            {
                return HttpNotFound();
            }
            return View(chiTietPhieuNhap);
        }

        // POST: ChiTietPhieuNhaps/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChiTietPhieuNhap chiTietPhieuNhap = db.ChiTietPhieuNhaps.Find(id);
            db.ChiTietPhieuNhaps.Remove(chiTietPhieuNhap);
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
