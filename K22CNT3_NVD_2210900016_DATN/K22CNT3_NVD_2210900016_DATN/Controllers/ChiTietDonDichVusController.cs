using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class ChiTietDonDichVusController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // ================= INDEX =================
        public ActionResult Index(int? donDichVuId)
        {
            var data = db.ChiTietDonDichVus
                .Include("DonDichVu")
                .Include("DichVu")
                .Include("SanPham")
                .AsQueryable();

            if (donDichVuId.HasValue)
            {
                data = data.Where(x => x.ID_DonDV == donDichVuId);
                ViewBag.DonDichVuId = donDichVuId;
            }

            return View(data.ToList());
        }

        // ================= CREATE (GET) =================
        public ActionResult Create(int? donDichVuId)
        {
            var model = new ChiTietDonDichVu
            {
                SoLuong = 1
            };

            // Nếu có đơn → gán
            if (donDichVuId.HasValue &&
                db.DonDichVus.Any(x => x.ID_DonDV == donDichVuId))
            {
                model.ID_DonDV = donDichVuId.Value;
            }

            // Dropdown chọn đơn dịch vụ
            ViewBag.ID_DonDV = new SelectList(
                db.DonDichVus,
                "ID_DonDV",
                "MaDon" // hoặc cột bạn muốn hiển thị
            );

            LoadDropdowns();
            return View(model);
        }


        // ================= CREATE (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ChiTietDonDichVu chiTiet)
        {
            // ❌ BỎ VALIDATE CÁC FIELD KHÔNG NHẬP
            ModelState.Remove("ThongSoKyThuat");
            ModelState.Remove("ThanhTien");

            // ===== KIỂM TRA FK =====
            if (!db.DonDichVus.Any(d => d.ID_DonDV == chiTiet.ID_DonDV))
                ModelState.AddModelError("", "Don dich vu khong ton tai");

            if (!db.DichVus.Any(d => d.ID_DichVu == chiTiet.ID_DichVu))
                ModelState.AddModelError("ID_DichVu", "Vui long chon dich vu");

            // ===== XỬ LÝ GIÁ TRỊ =====
            if (chiTiet.ID_SP == 0)
                chiTiet.ID_SP = null;

            if (string.IsNullOrWhiteSpace(chiTiet.ThongSoKyThuat))
                chiTiet.ThongSoKyThuat = "";

            if (chiTiet.SoLuong == null || chiTiet.SoLuong <= 0)
                chiTiet.SoLuong = 1;

            if (chiTiet.DonGia == null || chiTiet.DonGia <= 0)
            {
                var dv = db.DichVus.Find(chiTiet.ID_DichVu);
                if (dv != null)
                    chiTiet.DonGia = dv.DonGia;
            }

            chiTiet.ThanhTien = chiTiet.SoLuong * chiTiet.DonGia;

            // ===== SAVE =====
            if (ModelState.IsValid)
            {
                db.ChiTietDonDichVus.Add(chiTiet);
                db.SaveChanges();

                UpdateTongTienDonDichVu(chiTiet.ID_DonDV);

                TempData["SuccessMessage"] = "Them chi tiet dich vu thanh cong";
                return RedirectToAction("Index", new { donDichVuId = chiTiet.ID_DonDV });
            }

            LoadDropdowns(chiTiet);
            return View(chiTiet);
        }

        // ================= EDIT (GET) =================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var chiTiet = db.ChiTietDonDichVus.Find(id);
            if (chiTiet == null)
                return HttpNotFound();

            LoadDropdowns(chiTiet);
            return View(chiTiet);
        }

        // ================= EDIT (POST) =================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ChiTietDonDichVu chiTiet)
        {
            ModelState.Remove("ThongSoKyThuat");
            ModelState.Remove("ThanhTien");

            if (chiTiet.ID_SP == 0)
                chiTiet.ID_SP = null;

            if (string.IsNullOrWhiteSpace(chiTiet.ThongSoKyThuat))
                chiTiet.ThongSoKyThuat = "";

            chiTiet.ThanhTien = chiTiet.SoLuong * chiTiet.DonGia;

            if (ModelState.IsValid)
            {
                db.Entry(chiTiet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                UpdateTongTienDonDichVu(chiTiet.ID_DonDV);

                TempData["SuccessMessage"] = "Cap nhat thanh cong";
                return RedirectToAction("Index", new { donDichVuId = chiTiet.ID_DonDV });
            }

            LoadDropdowns(chiTiet);
            return View(chiTiet);
        }
        // ================= DETAILS =================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var chiTiet = db.ChiTietDonDichVus
                .Include("DonDichVu")
                .Include("DichVu")
                .Include("SanPham")
                .FirstOrDefault(x => x.ID_CTDonDV == id);

            if (chiTiet == null)
                return HttpNotFound();

            return View(chiTiet);
        }


        // ================= DELETE =================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var chiTiet = db.ChiTietDonDichVus
                .Include("DichVu")
                .Include("SanPham")
                .FirstOrDefault(x => x.ID_CTDonDV == id);

            if (chiTiet == null)
                return HttpNotFound();

            return View(chiTiet);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var chiTiet = db.ChiTietDonDichVus.Find(id);
            int donId = chiTiet.ID_DonDV;

            db.ChiTietDonDichVus.Remove(chiTiet);
            db.SaveChanges();

            UpdateTongTienDonDichVu(donId);

            return RedirectToAction("Index", new { donDichVuId = donId });
        }

        // ================= HÀM DÙNG CHUNG =================
        private void LoadDropdowns(ChiTietDonDichVu model = null)
        {
            ViewBag.ID_DichVu = new SelectList(
                db.DichVus.Where(x => x.TrangThai == true),
                "ID_DichVu",
                "TenDichVu",
                model?.ID_DichVu
            );

            ViewBag.ID_SP = new SelectList(
                db.SanPhams.Where(x => x.TrangThai == true),
                "ID_SP",
                "TenSP",
                model?.ID_SP
            );
        }

        private void UpdateTongTienDonDichVu(int donId)
        {
            var tong = db.ChiTietDonDichVus
                .Where(x => x.ID_DonDV == donId)
                .Sum(x => (decimal?)x.ThanhTien) ?? 0;

            var don = db.DonDichVus.Find(donId);
            if (don != null)
            {
                don.TongTien = tong;
                db.SaveChanges();
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
