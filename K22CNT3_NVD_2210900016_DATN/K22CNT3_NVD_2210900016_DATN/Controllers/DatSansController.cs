using System;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;
using System.Data.Entity;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class DatSansController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        /* =========================
           INDEX
        ========================= */
        public ActionResult Index()
        {
            var datSans = db.DatSans
                .Include(d => d.KhachHang)
                .Include(d => d.SanCauLong)
                .OrderByDescending(d => d.NgayDat)
                .ToList();

            return View(datSans);
        }

        /* =========================
           DETAILS
        ========================= */
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var datSan = db.DatSans
                .Include(d => d.KhachHang)
                .Include(d => d.SanCauLong)
                .FirstOrDefault(d => d.ID_DatSan == id);

            if (datSan == null)
                return HttpNotFound();

            return View(datSan);
        }

        /* =========================
           CREATE - GET
        ========================= */
        public ActionResult Create()
        {
            LoadDropdowns();
            return View();
        }

        /* =========================
           CREATE - POST
        ========================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DatSan datSan)
        {
            if (!ModelState.IsValid)
                goto LOAD;

            if (datSan.GioKetThuc <= datSan.GioBatDau)
            {
                ModelState.AddModelError("", "Giờ kết thúc phải lớn hơn giờ bắt đầu");
                goto LOAD;
            }

            var san = db.SanCauLongs.Find(datSan.ID_San);
            if (san == null)
            {
                ModelState.AddModelError("", "Sân không tồn tại");
                goto LOAD;
            }

            // Tính số giờ
            datSan.SoGio = (int)(datSan.GioKetThuc - datSan.GioBatDau).TotalHours;
            if (datSan.SoGio <= 0)
            {
                ModelState.AddModelError("", "Số giờ không hợp lệ");
                goto LOAD;
            }

            // Tính tiền
            datSan.TongTien = datSan.SoGio * san.GiaThueTheoGio;
            datSan.TrangThai = "Đang thuê";

            // Cập nhật trạng thái sân
            san.TrangThai = "Đang thuê";
            db.Entry(san).State = EntityState.Modified;

            db.DatSans.Add(datSan);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Đặt sân thành công!";
            return RedirectToAction("Index");

LOAD:
            LoadDropdowns(datSan.ID_KhachHang, datSan.ID_San);
            return View(datSan);
        }

        /* =========================
           EDIT - GET
        ========================= */
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var datSan = db.DatSans.Find(id);
            if (datSan == null)
                return HttpNotFound();

            LoadDropdowns(datSan.ID_KhachHang, datSan.ID_San);
            return View(datSan);
        }

        /* =========================
           EDIT - POST
        ========================= */
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DatSan datSan)
        {
            if (!ModelState.IsValid)
            {
                LoadDropdowns(datSan.ID_KhachHang, datSan.ID_San);
                return View(datSan);
            }

            db.Entry(datSan).State = EntityState.Modified;
            db.SaveChanges();

            TempData["SuccessMessage"] = "Cập nhật đặt sân thành công!";
            return RedirectToAction("Index");
        }

        /* =========================
           DELETE - GET
        ========================= */
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var datSan = db.DatSans
                .Include(d => d.KhachHang)
                .Include(d => d.SanCauLong)
                .FirstOrDefault(d => d.ID_DatSan == id);

            if (datSan == null)
                return HttpNotFound();

            return View(datSan);
        }

        /* =========================
           DELETE - POST
        ========================= */
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var datSan = db.DatSans.Find(id);
            if (datSan == null)
                return HttpNotFound();

            var san = db.SanCauLongs.Find(datSan.ID_San);
            if (san != null)
            {
                san.TrangThai = "Trống";
                db.Entry(san).State = EntityState.Modified;
            }

            db.DatSans.Remove(datSan);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Xóa đặt sân thành công!";
            return RedirectToAction("Index");
        }

        /* =========================
           HOÀN THÀNH
        ========================= */
        public ActionResult HoanThanh(int id)
        {
            var datSan = db.DatSans.Find(id);
            if (datSan == null)
                return HttpNotFound();

            datSan.TrangThai = "Hoàn thành";

            var san = db.SanCauLongs.Find(datSan.ID_San);
            if (san != null)
            {
                san.TrangThai = "Trống";
                db.Entry(san).State = EntityState.Modified;
            }

            db.SaveChanges();
            TempData["SuccessMessage"] = "Hoàn thành đặt sân!";
            return RedirectToAction("Index");
        }

        /* =========================
           LOAD DROPDOWNS
        ========================= */
        private void LoadDropdowns(int? khachHangId = null, int? sanId = null)
        {
            ViewBag.ID_KhachHang = new SelectList(
                db.KhachHangs,
                "ID_KhachHang",
                "TenKhach",
                khachHangId
            );

            ViewBag.ID_San = new SelectList(
                db.SanCauLongs.Where(s => s.TrangThai != "Đang thuê"),
                "ID_San",
                "TenSan",
                sanId
            );
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
