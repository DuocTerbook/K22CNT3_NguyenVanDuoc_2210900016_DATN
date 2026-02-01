using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class ChiTietDonDichVusController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // ================= LOAD DROPDOWN =================
        private void LoadDropDown(ChiTietDonDichVu model = null)
        {
            ViewBag.ID_DichVu = new SelectList(
                db.DichVus, "ID_DichVu", "TenDichVu",
                model?.ID_DichVu
            );

            ViewBag.ID_DonDV = new SelectList(
                db.DonDichVus, "ID_DonDV", "TenKhach",
                model?.ID_DonDV
            );

            ViewBag.ID_SP = new SelectList(
                db.SanPhams, "ID_SP", "TenSP",
                model?.ID_SP
            );
        }

        // ================= INDEX =================
        public ActionResult Index()
        {
            var data = db.ChiTietDonDichVus
                .Include(c => c.DichVu)
                .Include(c => c.DonDichVu)
                .Include(c => c.SanPham);

            return View(data.ToList());
        }

        // ================= DETAILS =================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var ct = db.ChiTietDonDichVus
                .Include(c => c.DichVu)
                .Include(c => c.DonDichVu)
                .Include(c => c.SanPham)
                .FirstOrDefault(c => c.ID_CTDonDV == id);

            if (ct == null)
                return HttpNotFound();

            return View(ct);
        }

        // ================= CREATE =================
        public ActionResult Create()
        {
            LoadDropDown();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChiTietDonDichVu chiTietDonDichVu)
        {
            if (ModelState.IsValid)
            {
                // TÍNH THÀNH TIỀN
                chiTietDonDichVu.ThanhTien =
                    chiTietDonDichVu.SoLuong * chiTietDonDichVu.DonGia;

                db.ChiTietDonDichVus.Add(chiTietDonDichVu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            LoadDropDown(chiTietDonDichVu);
            return View(chiTietDonDichVu);
        }

        // ================= EDIT =================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var ct = db.ChiTietDonDichVus.Find(id);
            if (ct == null)
                return HttpNotFound();

            LoadDropDown(ct);
            return View(ct);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChiTietDonDichVu chiTietDonDichVu)
        {
            if (ModelState.IsValid)
            {
                // TÍNH LẠI THÀNH TIỀN
                chiTietDonDichVu.ThanhTien =
                    chiTietDonDichVu.SoLuong * chiTietDonDichVu.DonGia;

                db.Entry(chiTietDonDichVu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            LoadDropDown(chiTietDonDichVu);
            return View(chiTietDonDichVu);
        }

        // ================= DELETE =================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var ct = db.ChiTietDonDichVus
                .Include(c => c.DichVu)
                .Include(c => c.DonDichVu)
                .Include(c => c.SanPham)
                .FirstOrDefault(c => c.ID_CTDonDV == id);

            if (ct == null)
                return HttpNotFound();

            return View(ct);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var ct = db.ChiTietDonDichVus.Find(id);
            db.ChiTietDonDichVus.Remove(ct);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        // ================= DISPOSE =================
        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
