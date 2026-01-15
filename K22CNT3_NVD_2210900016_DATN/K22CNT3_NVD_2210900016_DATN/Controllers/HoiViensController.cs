using System;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class HoiViensController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: HoiVien
        public ActionResult Index()
        {
            var hoiViens = db.HoiViens.Include("KhachHang").ToList();
            return View(hoiViens);
        }

        // GET: HoiVien/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            HoiVien hoiVien = db.HoiViens.Include("KhachHang").FirstOrDefault(h => h.ID_HoiVien == id);
            if (hoiVien == null)
            {
                return HttpNotFound();
            }
            return View(hoiVien);
        }

        // GET: HoiVien/Create
        public ActionResult Create()
        {
            // Chỉ lấy khách hàng chưa là hội viên
            var khachHangIds = db.HoiViens.Select(h => h.ID_KhachHang).ToList();
            var khachHangs = db.KhachHangs
                .Where(k => !khachHangIds.Contains(k.ID_KhachHang))
                .Select(k => new {
                    ID_KhachHang = k.ID_KhachHang,
                    Ten = k.TenKhach + " - " + k.DienThoai
                })
                .ToList();

            ViewBag.ID_KhachHang = new SelectList(khachHangs, "ID_KhachHang", "Ten");

            // Tạo mã thẻ tự động
            ViewBag.MaThe = GenerateMaThe();
            ViewBag.NgayDangKy = DateTime.Now.ToString("yyyy-MM-dd");
            ViewBag.NgayHetHan = DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");

            return View();
        }

        // POST: HoiVien/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_KhachHang,MaThe,NgayDangKy,NgayHetHan,CapDo,UuDai")] HoiVien hoiVien)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra mã thẻ trùng
                if (db.HoiViens.Any(h => h.MaThe == hoiVien.MaThe))
                {
                    ModelState.AddModelError("MaThe", "Mã thẻ đã tồn tại!");
                    ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", hoiVien.ID_KhachHang);
                    return View(hoiVien);
                }

                // Kiểm tra khách hàng đã là hội viên
                if (db.HoiViens.Any(h => h.ID_KhachHang == hoiVien.ID_KhachHang))
                {
                    ModelState.AddModelError("ID_KhachHang", "Khách hàng này đã là hội viên!");
                    ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", hoiVien.ID_KhachHang);
                    return View(hoiVien);
                }

                hoiVien.DiemTichLuy = 0;
                db.HoiViens.Add(hoiVien);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Tạo hội viên thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", hoiVien.ID_KhachHang);
            return View(hoiVien);
        }

        // GET: HoiVien/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            HoiVien hoiVien = db.HoiViens.Find(id);
            if (hoiVien == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", hoiVien.ID_KhachHang);
            return View(hoiVien);
        }

        // POST: HoiVien/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_HoiVien,ID_KhachHang,MaThe,NgayDangKy,NgayHetHan,CapDo,DiemTichLuy,UuDai")] HoiVien hoiVien)
        {
            if (ModelState.IsValid)
            {
                // Kiểm tra mã thẻ trùng (trừ chính nó)
                if (db.HoiViens.Any(h => h.MaThe == hoiVien.MaThe && h.ID_HoiVien != hoiVien.ID_HoiVien))
                {
                    ModelState.AddModelError("MaThe", "Mã thẻ đã tồn tại!");
                    ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", hoiVien.ID_KhachHang);
                    return View(hoiVien);
                }

                // Kiểm tra khách hàng đã là hội viên (trừ chính nó)
                if (db.HoiViens.Any(h => h.ID_KhachHang == hoiVien.ID_KhachHang && h.ID_HoiVien != hoiVien.ID_HoiVien))
                {
                    ModelState.AddModelError("ID_KhachHang", "Khách hàng này đã là hội viên!");
                    ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", hoiVien.ID_KhachHang);
                    return View(hoiVien);
                }

                db.Entry(hoiVien).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật hội viên thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", hoiVien.ID_KhachHang);
            return View(hoiVien);
        }

        // GET: HoiVien/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            HoiVien hoiVien = db.HoiViens.Include("KhachHang").FirstOrDefault(h => h.ID_HoiVien == id);
            if (hoiVien == null)
            {
                return HttpNotFound();
            }
            return View(hoiVien);
        }

        // POST: HoiVien/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            HoiVien hoiVien = db.HoiViens.Find(id);
            db.HoiViens.Remove(hoiVien);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa hội viên thành công!";
            return RedirectToAction("Index");
        }

        // Tích điểm thủ công
        public ActionResult TichDiem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            HoiVien hoiVien = db.HoiViens.Include("KhachHang").FirstOrDefault(h => h.ID_HoiVien == id);
            if (hoiVien == null)
            {
                return HttpNotFound();
            }

            ViewBag.HoiVien = hoiVien;
            return View();
        }

        [HttpPost]
        public ActionResult TichDiem(int id, int diem, string lyDo)
        {
            var hoiVien = db.HoiViens.Find(id);
            if (hoiVien != null)
            {
                // Thêm lịch sử tích điểm
                var lichSu = new LichSuTichDiem
                {
                    ID_HoiVien = id,
                    DiemCong = diem,
                    LyDo = lyDo,
                    NgayTichDiem = DateTime.Now
                };
                db.LichSuTichDiems.Add(lichSu);

                // Cập nhật điểm tích lũy
                hoiVien.DiemTichLuy += diem;

                // Nâng cấp cấp độ dựa trên điểm
                UpdateCapDo(hoiVien);

                db.SaveChanges();
                TempData["SuccessMessage"] = $"Đã tích {diem} điểm cho hội viên!";
            }
            return RedirectToAction("Details", new { id = id });
        }

        // Đổi điểm
        public ActionResult DoiDiem(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            HoiVien hoiVien = db.HoiViens.Include("KhachHang").FirstOrDefault(h => h.ID_HoiVien == id);
            if (hoiVien == null)
            {
                return HttpNotFound();
            }

            ViewBag.HoiVien = hoiVien;
            return View();
        }

        [HttpPost]
        public ActionResult DoiDiem(int id, int diem, string lyDo)
        {
            var hoiVien = db.HoiViens.Find(id);
            if (hoiVien != null)
            {
                if (hoiVien.DiemTichLuy < diem)
                {
                    TempData["ErrorMessage"] = "Số điểm không đủ để đổi!";
                    return RedirectToAction("Details", new { id = id });
                }

                // Thêm lịch sử trừ điểm
                var lichSu = new LichSuTichDiem
                {
                    ID_HoiVien = id,
                    DiemTru = diem,
                    LyDo = lyDo,
                    NgayTichDiem = DateTime.Now
                };
                db.LichSuTichDiems.Add(lichSu);

                // Trừ điểm tích lũy
                hoiVien.DiemTichLuy -= diem;

                // Cập nhật cấp độ
                UpdateCapDo(hoiVien);

                db.SaveChanges();
                TempData["SuccessMessage"] = $"Đã đổi {diem} điểm của hội viên!";
            }
            return RedirectToAction("Details", new { id = id });
        }

        // Gia hạn thẻ
        public ActionResult GiaHan(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            HoiVien hoiVien = db.HoiViens.Find(id);
            if (hoiVien == null)
            {
                return HttpNotFound();
            }

            hoiVien.NgayHetHan = hoiVien.NgayHetHan.HasValue ?
                hoiVien.NgayHetHan.Value.AddYears(1) :
                DateTime.Now.AddYears(1);

            db.SaveChanges();
            TempData["SuccessMessage"] = "Gia hạn thẻ thành công thêm 1 năm!";

            return RedirectToAction("Details", new { id = id });
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

        private string GenerateMaThe()
        {
            var lastThe = db.HoiViens.OrderByDescending(h => h.ID_HoiVien).FirstOrDefault();
            int nextNumber = 1;
            if (lastThe != null && lastThe.MaThe.StartsWith("CARD"))
            {
                if (int.TryParse(lastThe.MaThe.Substring(4), out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }
            return $"CARD{nextNumber:D3}";
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