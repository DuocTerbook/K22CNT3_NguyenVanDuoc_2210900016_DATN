using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class LichSuTichDiemsController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // ================= INDEX =================
        public ActionResult Index(int? hoiVienId)
        {
            var data = db.LichSuTichDiems
                .Include(l => l.HoiVien.KhachHang)
                .Include(l => l.DonHang)
                .Include(l => l.DonDichVu)
                .OrderByDescending(l => l.NgayTichDiem)
                .AsQueryable();

            if (hoiVienId.HasValue)
            {
                data = data.Where(l => l.ID_HoiVien == hoiVienId);
            }

            return View(data.ToList());
        }

        // ================= CREATE =================
        public ActionResult Create()
        {
            LoadDropDowns();
            return View(new LichSuTichDiem
            {
                NgayTichDiem = DateTime.Now
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(LichSuTichDiem model)
        {
            if (model.ID_DonHang.HasValue && model.ID_DonDV.HasValue)
            {
                ModelState.AddModelError("", "Chỉ được chọn Đơn hàng hoặc Đơn dịch vụ.");
            }

            if (!model.ID_DonHang.HasValue && !model.ID_DonDV.HasValue)
            {
                ModelState.AddModelError("", "Vui lòng chọn Đơn hàng hoặc Đơn dịch vụ.");
            }

            if (!ModelState.IsValid)
            {
                LoadDropDowns(model.ID_HoiVien, model.ID_DonHang, model.ID_DonDV);
                return View(model);
            }

            if (model.ID_DonHang.HasValue)
            {
                var dh = db.DonHangs.Find(model.ID_DonHang);
                model.DiemCong = (int)(dh.TongTien / 10000);
                model.LyDo = "Tích điểm từ đơn hàng #" + dh.ID_DonHang;
            }
            else
            {
                var dv = db.DonDichVus.Find(model.ID_DonDV);
                model.DiemCong = (int)(dv.TongTien / 10000);
                model.LyDo = "Tích điểm từ đơn dịch vụ #" + dv.ID_DonDV;
            }

            model.NgayTichDiem = DateTime.Now;

            db.LichSuTichDiems.Add(model);

            var hv = db.HoiViens.Find(model.ID_HoiVien);
            hv.DiemTichLuy += model.DiemCong ?? 0;
            UpdateCapDo(hv);

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ================= EDIT =================
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var lichSu = db.LichSuTichDiems.Find(id);
            if (lichSu == null) return HttpNotFound();

            LoadDropDowns(lichSu.ID_HoiVien, lichSu.ID_DonHang, lichSu.ID_DonDV);
            return View(lichSu);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LichSuTichDiem model)
        {
            if (!ModelState.IsValid)
            {
                LoadDropDowns(model.ID_HoiVien, model.ID_DonHang, model.ID_DonDV);
                return View(model);
            }

            var old = db.LichSuTichDiems.AsNoTracking()
                .FirstOrDefault(x => x.ID_LichSu == model.ID_LichSu);

            var hv = db.HoiViens.Find(model.ID_HoiVien);
            hv.DiemTichLuy -= old.DiemCong ?? 0;

            if (model.ID_DonHang.HasValue)
            {
                var dh = db.DonHangs.Find(model.ID_DonHang);
                model.DiemCong = (int)(dh.TongTien / 10000);
                model.LyDo = "Tích điểm từ đơn hàng #" + dh.ID_DonHang;
            }
            else
            {
                var dv = db.DonDichVus.Find(model.ID_DonDV);
                model.DiemCong = (int)(dv.TongTien / 10000);
                model.LyDo = "Tích điểm từ đơn dịch vụ #" + dv.ID_DonDV;
            }

            hv.DiemTichLuy += model.DiemCong ?? 0;
            UpdateCapDo(hv);

            db.Entry(model).State = EntityState.Modified;
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ================= DETAILS =================
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var lichSu = db.LichSuTichDiems
                .Include(l => l.HoiVien.KhachHang)
                .Include(l => l.DonHang)
                .Include(l => l.DonDichVu)
                .FirstOrDefault(l => l.ID_LichSu == id);

            if (lichSu == null) return HttpNotFound();

            return View(lichSu);
        }

        // ================= DELETE =================
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var lichSu = db.LichSuTichDiems
                .Include(l => l.HoiVien)
                .FirstOrDefault(l => l.ID_LichSu == id);

            if (lichSu == null) return HttpNotFound();

            return View(lichSu);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var lichSu = db.LichSuTichDiems.Find(id);
            var hv = db.HoiViens.Find(lichSu.ID_HoiVien);

            hv.DiemTichLuy -= lichSu.DiemCong ?? 0;
            UpdateCapDo(hv);

            db.LichSuTichDiems.Remove(lichSu);
            db.SaveChanges();

            return RedirectToAction("Index");
        }

        // ================= DROPDOWN =================
        private void LoadDropDowns(
    int? hoiVienId = null,
    int? donHangId = null,
    int? donDvId = null)
        {
            // Hội viên
            ViewBag.ID_HoiVien = new SelectList(
                db.HoiViens.Include(h => h.KhachHang),
                "ID_HoiVien",
                "KhachHang.TenKhach",
                hoiVienId
            );

            // Đơn hàng - chỉ lấy đơn HOÀN THÀNH
            ViewBag.ID_DonHang = new SelectList(
                db.DonHangs
                  .Where(x => x.TrangThai == "HoanThanh"),
                "ID_DonHang",
                "ID_DonHang",
                donHangId
            );

            // Đơn dịch vụ - chỉ lấy đơn HOÀN THÀNH
            ViewBag.ID_DonDV = new SelectList(
                db.DonDichVus
                  .Where(x => x.TrangThai == "HoanThanh"),
                "ID_DonDV",
                "ID_DonDV",
                donDvId
            );
        }


        // ================= CẤP ĐỘ =================
        private void UpdateCapDo(HoiVien hv)
        {
            if (hv.DiemTichLuy >= 5000)
            {
                hv.CapDo = "Kim cương";
                hv.UuDai = 15;
            }
            else if (hv.DiemTichLuy >= 2000)
            {
                hv.CapDo = "Vàng";
                hv.UuDai = 10;
            }
            else if (hv.DiemTichLuy >= 500)
            {
                hv.CapDo = "Bạc";
                hv.UuDai = 5;
            }
            else
            {
                hv.CapDo = "Đồng";
                hv.UuDai = 0;
            }
        }
    }
}
