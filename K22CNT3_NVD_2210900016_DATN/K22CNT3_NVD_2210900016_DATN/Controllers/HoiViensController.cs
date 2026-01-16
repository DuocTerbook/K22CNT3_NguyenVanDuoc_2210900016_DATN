using System;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;
using System.Data.Entity;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class HoiViensController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // ================= INDEX =================
        public ActionResult Index()
        {
            var hoiViens = db.HoiViens.Include(h => h.KhachHang).ToList();
            return View(hoiViens);
        }

        // ================= DETAILS =================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var hoiVien = db.HoiViens
                .Include(h => h.KhachHang)
                .FirstOrDefault(h => h.ID_HoiVien == id);

            if (hoiVien == null)
                return HttpNotFound();

            return View(hoiVien);
        }

        // ================= CREATE =================
        public ActionResult Create()
        {
            var khachHangIds = db.HoiViens.Select(h => h.ID_KhachHang).ToList();
            var khachHangs = db.KhachHangs
                .Where(k => !khachHangIds.Contains(k.ID_KhachHang))
                .Select(k => new
                {
                    k.ID_KhachHang,
                    Ten = k.TenKhach + " - " + k.DienThoai
                }).ToList();

            ViewBag.ID_KhachHang = new SelectList(khachHangs, "ID_KhachHang", "Ten");

            ViewBag.MaThe = GenerateMaThe();
            ViewBag.NgayDangKy = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.NgayHetHan = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(HoiVien hoiVien)
        {
            if (ModelState.IsValid)
            {
                if (db.HoiViens.Any(h => h.MaThe == hoiVien.MaThe))
                {
                    ModelState.AddModelError("MaThe", "Ma the da ton tai");
                    return View(hoiVien);
                }

                if (db.HoiViens.Any(h => h.ID_KhachHang == hoiVien.ID_KhachHang))
                {
                    ModelState.AddModelError("ID_KhachHang", "Khach hang da la hoi vien");
                    return View(hoiVien);
                }

                hoiVien.DiemTichLuy = 0;
                hoiVien.CapDo = "Dong";
                hoiVien.UuDai = 0;

                db.HoiViens.Add(hoiVien);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Tao hoi vien thanh cong";
                return RedirectToAction("Index");
            }
            return View(hoiVien);
        }

        // ================= EDIT =================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var hoiVien = db.HoiViens.Find(id);
            if (hoiVien == null)
                return HttpNotFound();

            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", hoiVien.ID_KhachHang);
            return View(hoiVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(HoiVien hoiVien)
        {
            if (ModelState.IsValid)
            {
                db.Entry(hoiVien).State = EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cap nhat hoi vien thanh cong";
                return RedirectToAction("Index");
            }
            return View(hoiVien);
        }

        // ================= DELETE =================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var hoiVien = db.HoiViens
                .Include(h => h.KhachHang)
                .FirstOrDefault(h => h.ID_HoiVien == id);

            if (hoiVien == null)
                return HttpNotFound();

            return View(hoiVien);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var hoiVien = db.HoiViens.Find(id);
            db.HoiViens.Remove(hoiVien);
            db.SaveChanges();

            TempData["SuccessMessage"] = "Xoa hoi vien thanh cong";
            return RedirectToAction("Index");
        }

        // ================= TICH DIEM =================
        public ActionResult TichDiem(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var hoiVien = db.HoiViens
                .Include(h => h.KhachHang)
                .FirstOrDefault(h => h.ID_HoiVien == id);

            if (hoiVien == null)
                return HttpNotFound();

            return View(hoiVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult TichDiem(int id, int diem, string lyDo)
        {
            var hoiVien = db.HoiViens.Find(id);
            if (hoiVien == null)
                return HttpNotFound();

            var lichSu = new LichSuTichDiem
            {
                ID_HoiVien = id,
                DiemCong = diem,
                LyDo = lyDo,
                NgayTichDiem = DateTime.Now
            };

            db.LichSuTichDiems.Add(lichSu);
            hoiVien.DiemTichLuy += diem;
            UpdateCapDo(hoiVien);

            db.SaveChanges();
            TempData["SuccessMessage"] = "Tich diem thanh cong";

            return RedirectToAction("Details", new { id });
        }

        // ================= DOI DIEM =================
        public ActionResult DoiDiem(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var hoiVien = db.HoiViens
                .Include(h => h.KhachHang)
                .FirstOrDefault(h => h.ID_HoiVien == id);

            if (hoiVien == null)
                return HttpNotFound();

            return View(hoiVien);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DoiDiem(int id, int diem, string lyDo)
        {
            var hoiVien = db.HoiViens.Find(id);
            if (hoiVien == null)
                return HttpNotFound();

            if (hoiVien.DiemTichLuy < diem)
            {
                TempData["ErrorMessage"] = "Khong du diem de doi";
                return RedirectToAction("Details", new { id });
            }

            var lichSu = new LichSuTichDiem
            {
                ID_HoiVien = id,
                DiemTru = diem,
                LyDo = lyDo,
                NgayTichDiem = DateTime.Now
            };

            db.LichSuTichDiems.Add(lichSu);
            hoiVien.DiemTichLuy -= diem;
            UpdateCapDo(hoiVien);

            db.SaveChanges();
            TempData["SuccessMessage"] = "Doi diem thanh cong";

            return RedirectToAction("Details", new { id });
        }

        // ================= GIA HAN =================
        public ActionResult GiaHan(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);

            var hoiVien = db.HoiViens.Find(id);
            if (hoiVien == null)
                return HttpNotFound();

            hoiVien.NgayHetHan = hoiVien.NgayHetHan.HasValue
                ? hoiVien.NgayHetHan.Value.AddYears(1)
                : DateTime.Now.AddYears(1);

            db.SaveChanges();
            TempData["SuccessMessage"] = "Gia han the thanh cong";

            return RedirectToAction("Details", new { id });
        }

        // ================= CAP DO =================
        private void UpdateCapDo(HoiVien hv)
        {
            if (hv.DiemTichLuy >= 5000)
            {
                hv.CapDo = "Kim cuong";
                hv.UuDai = 15;
            }
            else if (hv.DiemTichLuy >= 2000)
            {
                hv.CapDo = "Vang";
                hv.UuDai = 10;
            }
            else if (hv.DiemTichLuy >= 500)
            {
                hv.CapDo = "Bac";
                hv.UuDai = 5;
            }
            else
            {
                hv.CapDo = "Dong";
                hv.UuDai = 0;
            }
        }

        private string GenerateMaThe()
        {
            var last = db.HoiViens.OrderByDescending(h => h.ID_HoiVien).FirstOrDefault();
            int next = 1;

            if (last != null && last.MaThe.StartsWith("CARD"))
            {
                int.TryParse(last.MaThe.Substring(4), out next);
                next++;
            }
            return $"CARD{next:D3}";
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
