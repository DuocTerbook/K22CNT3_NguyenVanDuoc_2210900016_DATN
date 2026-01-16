using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class DonHangsController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // ================= INDEX =================
        public ActionResult Index()
        {
            var donHangs = db.DonHangs
                .Include(d => d.KhachHang)
                .OrderByDescending(d => d.NgayDat);

            return View(donHangs.ToList());
        }

        // ================= DETAILS =================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var donHang = db.DonHangs
                .Include(d => d.KhachHang)
                .FirstOrDefault(d => d.ID_DonHang == id);

            if (donHang == null)
                return HttpNotFound();

            return View(donHang);
        }

        // ================= CREATE =================
        public ActionResult Create()
        {
            ViewBag.ID_KhachHang = new SelectList(
                db.KhachHangs,
                "ID_KhachHang",
                "TenKhach"
            );
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                donHang.NgayDat = DateTime.Now;
                db.DonHangs.Add(donHang);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_KhachHang = new SelectList(
                db.KhachHangs,
                "ID_KhachHang",
                "TenKhach",
                donHang.ID_KhachHang
            );
            return View(donHang);
        }

        // ================= EDIT =================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var donHang = db.DonHangs.Find(id);
            if (donHang == null)
                return HttpNotFound();

            ViewBag.ID_KhachHang = new SelectList(
                db.KhachHangs,
                "ID_KhachHang",
                "TenKhach",
                donHang.ID_KhachHang
            );

            return View(donHang);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DonHang donHang)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donHang).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_KhachHang = new SelectList(
                db.KhachHangs,
                "ID_KhachHang",
                "TenKhach",
                donHang.ID_KhachHang
            );
            return View(donHang);
        }

        // ================= DELETE =================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var donHang = db.DonHangs
                .Include(d => d.KhachHang)
                .FirstOrDefault(d => d.ID_DonHang == id);

            if (donHang == null)
                return HttpNotFound();

            return View(donHang);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var donHang = db.DonHangs.Find(id);
            db.DonHangs.Remove(donHang);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
