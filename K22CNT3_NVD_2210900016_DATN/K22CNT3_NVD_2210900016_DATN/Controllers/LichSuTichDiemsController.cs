using System;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class LichSuTichDiemsController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: LichSuTichDiem
        public ActionResult Index(int? hoiVienId)
        {
            IQueryable<LichSuTichDiem> lichSuQuery = db.LichSuTichDiems
                .Include("HoiVien")
                .Include("DonHang")
                .Include("DonDichVu");

            if (hoiVienId.HasValue)
            {
                lichSuQuery = lichSuQuery.Where(l => l.ID_HoiVien == hoiVienId);
                ViewBag.HoiVienId = hoiVienId;
                ViewBag.HoiVien = db.HoiViens.Include("KhachHang").FirstOrDefault(h => h.ID_HoiVien == hoiVienId);
            }

            var lichSu = lichSuQuery.OrderByDescending(l => l.NgayTichDiem).ToList();
            return View(lichSu);
        }

        // GET: LichSuTichDiem/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            LichSuTichDiem lichSu = db.LichSuTichDiems
                .Include("HoiVien")
                .Include("DonHang")
                .Include("DonDichVu")
                .FirstOrDefault(l => l.ID_LichSu == id);
            if (lichSu == null)
            {
                return HttpNotFound();
            }
            return View(lichSu);
        }

        // GET: LichSuTichDiem/Create
        public ActionResult Create(int? hoiVienId)
        {
            ViewBag.ID_HoiVien = new SelectList(db.HoiViens.Include("KhachHang"), "ID_HoiVien", "KhachHang.TenKhach", hoiVienId);
            ViewBag.ID_DonHang = new SelectList(db.DonHangs.Where(d => d.TrangThai == "HoanThanh"), "ID_DonHang", "ID_DonHang");
            ViewBag.ID_DonDV = new SelectList(db.DonDichVus.Where(d => d.TrangThai == "Hoàn thành"), "ID_DonDV", "ID_DonDV");

            if (hoiVienId.HasValue)
            {
                var hoiVien = db.HoiViens.Include("KhachHang").FirstOrDefault(h => h.ID_HoiVien == hoiVienId);
                if (hoiVien != null)
                {
                    ViewBag.HoiVienTen = $"{hoiVien.KhachHang.TenKhach} - {hoiVien.MaThe}";
                }
            }

            return View();
        }

        // POST: LichSuTichDiem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_HoiVien,ID_DonHang,ID_DonDV,DiemCong,DiemTru,LyDo")] LichSuTichDiem lichSu)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra nếu có đơn hàng hoặc đơn dịch vụ, không cho nhập điểm thủ công
                if ((lichSu.ID_DonHang.HasValue || lichSu.ID_DonDV.HasValue) && (lichSu.DiemCong > 0 || lichSu.DiemTru > 0))
                {
                    ModelState.AddModelError("", "Không được nhập điểm khi chọn đơn hàng hoặc đơn dịch vụ!");
                    ViewBag.ID_HoiVien = new SelectList(db.HoiViens, "ID_HoiVien", "KhachHang.TenKhach", lichSu.ID_HoiVien);
                    ViewBag.ID_DonHang = new SelectList(db.DonHangs, "ID_DonHang", "ID_DonHang", lichSu.ID_DonHang);
                    ViewBag.ID_DonDV = new SelectList(db.DonDichVus, "ID_DonDV", "ID_DonDV", lichSu.ID_DonDV);
                    return View(lichSu);
                }

                // Nếu chọn đơn hàng, tính điểm tự động
                if (lichSu.ID_DonHang.HasValue)
                {
                    var donHang = db.DonHangs.Find(lichSu.ID_DonHang.Value);
                    if (donHang != null)
                    {
                        // Tính điểm: 1 điểm cho mỗi 10,000đ
                        lichSu.DiemCong = (int)(donHang.TongTien / 10000);
                        lichSu.LyDo = $"Tích điểm từ đơn hàng #{donHang.ID_DonHang} - {donHang.TongTien:N0}đ";
                    }
                }

                // Nếu chọn đơn dịch vụ, tính điểm tự động
                if (lichSu.ID_DonDV.HasValue)
                {
                    var donDichVu = db.DonDichVus.Find(lichSu.ID_DonDV.Value);
                    if (donDichVu != null)
                    {
                        // Tính điểm: 1 điểm cho mỗi 10,000đ
                        lichSu.DiemCong = (int)(donDichVu.TongTien / 10000);
                        lichSu.LyDo = $"Tích điểm từ dịch vụ #{donDichVu.ID_DonDV} - {donDichVu.TongTien:N0}đ";
                    }
                }

                lichSu.NgayTichDiem = DateTime.Now;

                db.LichSuTichDiems.Add(lichSu);

                // Cập nhật điểm tích lũy cho hội viên
                var hoiVien = db.HoiViens.Find(lichSu.ID_HoiVien);
                if (hoiVien != null)
                {
                    hoiVien.DiemTichLuy += (lichSu.DiemCong - lichSu.DiemTru);

                    // Cập nhật cấp độ
                    UpdateCapDo(hoiVien);
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm lịch sử tích điểm thành công!";
                return RedirectToAction("Index", new { hoiVienId = lichSu.ID_HoiVien });
            }

            ViewBag.ID_HoiVien = new SelectList(db.HoiViens, "ID_HoiVien", "KhachHang.TenKhach", lichSu.ID_HoiVien);
            ViewBag.ID_DonHang = new SelectList(db.DonHangs, "ID_DonHang", "ID_DonHang", lichSu.ID_DonHang);
            ViewBag.ID_DonDV = new SelectList(db.DonDichVus, "ID_DonDV", "ID_DonDV", lichSu.ID_DonDV);
            return View(lichSu);
        }

        // GET: LichSuTichDiem/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            LichSuTichDiem lichSu = db.LichSuTichDiems.Find(id);
            if (lichSu == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_HoiVien = new SelectList(db.HoiViens, "ID_HoiVien", "KhachHang.TenKhach", lichSu.ID_HoiVien);
            ViewBag.ID_DonHang = new SelectList(db.DonHangs, "ID_DonHang", "ID_DonHang", lichSu.ID_DonHang);
            ViewBag.ID_DonDV = new SelectList(db.DonDichVus, "ID_DonDV", "ID_DonDV", lichSu.ID_DonDV);
            return View(lichSu);
        }

        // POST: LichSuTichDiem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_LichSu,ID_HoiVien,ID_DonHang,ID_DonDV,DiemCong,DiemTru,LyDo,NgayTichDiem")] LichSuTichDiem lichSu)
        {
            if (ModelState.IsValid)
            {
                // Lấy dữ liệu cũ để tính chênh lệch
                var lichSuCu = db.LichSuTichDiems.AsNoTracking().FirstOrDefault(l => l.ID_LichSu == lichSu.ID_LichSu);
                if (lichSuCu != null)
                {
                    // Tính chênh lệch điểm
                    int chenhLechCong = (lichSu.DiemCong ?? 0) - (lichSuCu.DiemCong ?? 0);
                    int chenhLechTru = (lichSu.DiemTru ?? 0) - (lichSuCu.DiemTru ?? 0);
                    int tongChenhLech = chenhLechCong - chenhLechTru;


                    // Cập nhật điểm tích lũy cho hội viên
                    var hoiVien = db.HoiViens.Find(lichSu.ID_HoiVien);
                    if (hoiVien != null)
                    {
                        hoiVien.DiemTichLuy += tongChenhLech;
                        UpdateCapDo(hoiVien);
                    }
                }

                db.Entry(lichSu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật lịch sử tích điểm thành công!";
                return RedirectToAction("Index", new { hoiVienId = lichSu.ID_HoiVien });
            }
            ViewBag.ID_HoiVien = new SelectList(db.HoiViens, "ID_HoiVien", "KhachHang.TenKhach", lichSu.ID_HoiVien);
            ViewBag.ID_DonHang = new SelectList(db.DonHangs, "ID_DonHang", "ID_DonHang", lichSu.ID_DonHang);
            ViewBag.ID_DonDV = new SelectList(db.DonDichVus, "ID_DonDV", "ID_DonDV", lichSu.ID_DonDV);
            return View(lichSu);
        }

        // GET: LichSuTichDiem/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            LichSuTichDiem lichSu = db.LichSuTichDiems
                .Include("HoiVien")
                .Include("DonHang")
                .Include("DonDichVu")
                .FirstOrDefault(l => l.ID_LichSu == id);
            if (lichSu == null)
            {
                return HttpNotFound();
            }
            return View(lichSu);
        }

        // POST: LichSuTichDiem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LichSuTichDiem lichSu = db.LichSuTichDiems.Find(id);
            int hoiVienId = lichSu.ID_HoiVien;

            // Trừ điểm tích lũy cho hội viên
            var hoiVien = db.HoiViens.Find(hoiVienId);
            if (hoiVien != null)
            {
                hoiVien.DiemTichLuy -= (lichSu.DiemCong - lichSu.DiemTru);
                UpdateCapDo(hoiVien);
            }

            db.LichSuTichDiems.Remove(lichSu);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa lịch sử tích điểm thành công!";
            return RedirectToAction("Index", new { hoiVienId = hoiVienId });
        }

        // Báo cáo tích điểm
        public ActionResult BaoCao()
        {
            var baoCao = db.HoiViens
                .Include("KhachHang")
                .Select(h => new
                {
                    h.ID_HoiVien,
                    h.KhachHang.TenKhach,
                    h.MaThe,
                    h.CapDo,
                    h.DiemTichLuy,
                    TongDiemCong = db.LichSuTichDiems
                        .Where(l => l.ID_HoiVien == h.ID_HoiVien)
                        .Sum(l => (int?)l.DiemCong) ?? 0,
                    TongDiemTru = db.LichSuTichDiems
                        .Where(l => l.ID_HoiVien == h.ID_HoiVien)
                        .Sum(l => (int?)l.DiemTru) ?? 0,
                    SoLanTichDiem = db.LichSuTichDiems
                        .Count(l => l.ID_HoiVien == h.ID_HoiVien && l.DiemCong > 0)
                })
                .OrderByDescending(h => h.DiemTichLuy)
                .ToList();

            ViewBag.BaoCao = baoCao;
            return View();
        }

        private void UpdateCapDo(HoiVien hoiVien)
        {
            if (hoiVien.DiemTichLuy >= 5000)
            {
                hoiVien.CapDo = "Kim cương";
                hoiVien.UuDai = 15;
            }
            else if (hoiVien.DiemTichLuy >= 2000)
            {
                hoiVien.CapDo = "Vàng";
                hoiVien.UuDai = 10;
            }
            else if (hoiVien.DiemTichLuy >= 500)
            {
                hoiVien.CapDo = "Bạc";
                hoiVien.UuDai = 5;
            }
            else
            {
                hoiVien.CapDo = "Đồng";
                hoiVien.UuDai = 0;
            }
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