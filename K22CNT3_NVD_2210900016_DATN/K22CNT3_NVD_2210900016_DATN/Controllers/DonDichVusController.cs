using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class DonDichVusController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // ================= INDEX =================
        public ActionResult Index()
        {
            var donDichVus = db.DonDichVus
                .Include(d => d.KhachHang)
                .OrderByDescending(d => d.NgayNhan);

            return View(donDichVus.ToList());
        }

        // ================= DETAILS =================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var donDichVu = db.DonDichVus
                .Include(d => d.KhachHang)
                .FirstOrDefault(d => d.ID_DonDV == id);

            if (donDichVu == null)
                return HttpNotFound();

            return View(donDichVu);
        }

        // ================= CREATE =================
        public ActionResult Create()
        {
            ViewBag.ID_KhachHang = new SelectList(
                db.KhachHangs,
                "ID_KhachHang",
                "TenKhach"
            );

            ViewBag.TrangThai = new SelectList(
                new[] { "ChuaXuLy", "DangXuLy", "HoanThanh" }
            );

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DonDichVu donDichVu)
        {
            if (ModelState.IsValid)
            {
                donDichVu.NgayNhan = DateTime.Now;
                db.DonDichVus.Add(donDichVu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID_KhachHang = new SelectList(
                db.KhachHangs,
                "ID_KhachHang",
                "TenKhach",
                donDichVu.ID_KhachHang
            );

            ViewBag.TrangThai = new SelectList(
                new[] { "ChuaXuLy", "DangXuLy", "HoanThanh" },
                donDichVu.TrangThai
            );

            return View(donDichVu);
        }

        // ================= EDIT =================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var donDichVu = db.DonDichVus.Find(id);
            if (donDichVu == null)
                return HttpNotFound();

            ViewBag.ID_KhachHang = new SelectList(
                db.KhachHangs,
                "ID_KhachHang",
                "TenKhach",
                donDichVu.ID_KhachHang
            );

            ViewBag.TrangThai = new SelectList(
                new[] { "ChuaXuLy", "DangXuLy", "HoanThanh" },
                donDichVu.TrangThai
            );

            return View(donDichVu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DonDichVu donDichVu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donDichVu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(donDichVu);
        }

        // ================= DELETE =================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var donDichVu = db.DonDichVus
                .Include(d => d.KhachHang)
                .FirstOrDefault(d => d.ID_DonDV == id);

            if (donDichVu == null)
                return HttpNotFound();

            return View(donDichVu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var donDichVu = db.DonDichVus.Find(id);
            db.DonDichVus.Remove(donDichVu);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
