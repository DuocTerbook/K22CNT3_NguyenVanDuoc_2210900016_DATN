using System;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class ChiTietDonDichVusController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: ChiTietDonDichVu
        public ActionResult Index(int? donDichVuId)
        {
            if (donDichVuId.HasValue)
            {
                var chiTiets = db.ChiTietDonDichVus
                    .Include("DonDichVu")
                    .Include("DichVu")
                    .Include("SanPham")
                    .Where(c => c.ID_DonDV == donDichVuId)
                    .ToList();
                ViewBag.DonDichVuId = donDichVuId;
                return View(chiTiets);
            }
            return View(db.ChiTietDonDichVus.Include("DonDichVu").Include("DichVu").Include("SanPham").ToList());
        }

        // GET: ChiTietDonDichVu/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ChiTietDonDichVu chiTiet = db.ChiTietDonDichVus
                .Include("DonDichVu")
                .Include("DichVu")
                .Include("SanPham")
                .FirstOrDefault(c => c.ID_CTDonDV == id);
            if (chiTiet == null)
            {
                return HttpNotFound();
            }
            return View(chiTiet);
        }

        // GET: ChiTietDonDichVu/Create
        public ActionResult Create(int? donDichVuId)
        {
            ViewBag.ID_DonDV = new SelectList(db.DonDichVus, "ID_DonDV", "TenKhach", donDichVuId);
            ViewBag.ID_DichVu = new SelectList(db.DichVus.Where(d => d.TrangThai == true), "ID_DichVu", "TenDichVu");
            ViewBag.ID_SP = new SelectList(db.SanPhams.Where(s => s.TrangThai == true), "ID_SP", "TenSP");

            if (donDichVuId.HasValue)
            {
                var model = new ChiTietDonDichVu { ID_DonDV = donDichVuId.Value };
                return View(model);
            }

            return View();
        }

        // POST: ChiTietDonDichVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_DonDV,ID_DichVu,ID_SP,SoLuong,DonGia,ThongSoKyThuat")] ChiTietDonDichVu chiTiet)
        {
            if (ModelState.IsValid)
            {
                // Lấy đơn giá từ dịch vụ nếu không nhập
                if (chiTiet.DonGia == 0)
                {
                    var dichVu = db.DichVus.Find(chiTiet.ID_DichVu);
                    if (dichVu != null)
                        chiTiet.DonGia = dichVu.DonGia;
                }

                db.ChiTietDonDichVus.Add(chiTiet);
                db.SaveChanges();

                // Cập nhật tổng tiền đơn dịch vụ
                UpdateTongTienDonDichVu(chiTiet.ID_DonDV);

                TempData["SuccessMessage"] = "Thêm chi tiết dịch vụ thành công!";
                return RedirectToAction("Index", new { donDichVuId = chiTiet.ID_DonDV });
            }

            ViewBag.ID_DonDV = new SelectList(db.DonDichVus, "ID_DonDV", "TenKhach", chiTiet.ID_DonDV);
            ViewBag.ID_DichVu = new SelectList(db.DichVus, "ID_DichVu", "TenDichVu", chiTiet.ID_DichVu);
            ViewBag.ID_SP = new SelectList(db.SanPhams, "ID_SP", "TenSP", chiTiet.ID_SP);
            return View(chiTiet);
        }

        // GET: ChiTietDonDichVu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ChiTietDonDichVu chiTiet = db.ChiTietDonDichVus.Find(id);
            if (chiTiet == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_DonDV = new SelectList(db.DonDichVus, "ID_DonDV", "TenKhach", chiTiet.ID_DonDV);
            ViewBag.ID_DichVu = new SelectList(db.DichVus, "ID_DichVu", "TenDichVu", chiTiet.ID_DichVu);
            ViewBag.ID_SP = new SelectList(db.SanPhams, "ID_SP", "TenSP", chiTiet.ID_SP);
            return View(chiTiet);
        }

        // POST: ChiTietDonDichVu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_CTDonDV,ID_DonDV,ID_DichVu,ID_SP,SoLuong,DonGia,ThongSoKyThuat")] ChiTietDonDichVu chiTiet)
        {
            if (ModelState.IsValid)
            {
                db.Entry(chiTiet).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                // Cập nhật tổng tiền đơn dịch vụ
                UpdateTongTienDonDichVu(chiTiet.ID_DonDV);

                TempData["SuccessMessage"] = "Cập nhật chi tiết dịch vụ thành công!";
                return RedirectToAction("Index", new { donDichVuId = chiTiet.ID_DonDV });
            }
            ViewBag.ID_DonDV = new SelectList(db.DonDichVus, "ID_DonDV", "TenKhach", chiTiet.ID_DonDV);
            ViewBag.ID_DichVu = new SelectList(db.DichVus, "ID_DichVu", "TenDichVu", chiTiet.ID_DichVu);
            ViewBag.ID_SP = new SelectList(db.SanPhams, "ID_SP", "TenSP", chiTiet.ID_SP);
            return View(chiTiet);
        }

        // GET: ChiTietDonDichVu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            ChiTietDonDichVu chiTiet = db.ChiTietDonDichVus
                .Include("DonDichVu")
                .Include("DichVu")
                .Include("SanPham")
                .FirstOrDefault(c => c.ID_CTDonDV == id);
            if (chiTiet == null)
            {
                return HttpNotFound();
            }
            return View(chiTiet);
        }

        // POST: ChiTietDonDichVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ChiTietDonDichVu chiTiet = db.ChiTietDonDichVus.Find(id);
            int donDichVuId = chiTiet.ID_DonDV;
            db.ChiTietDonDichVus.Remove(chiTiet);
            db.SaveChanges();

            // Cập nhật tổng tiền đơn dịch vụ
            UpdateTongTienDonDichVu(donDichVuId);

            TempData["SuccessMessage"] = "Xóa chi tiết dịch vụ thành công!";
            return RedirectToAction("Index", new { donDichVuId = donDichVuId });
        }

        private void UpdateTongTienDonDichVu(int donDichVuId)
        {
            var tongTien = db.ChiTietDonDichVus
                .Where(c => c.ID_DonDV == donDichVuId)
                .Sum(c => (decimal?)c.ThanhTien) ?? 0;

            var donDichVu = db.DonDichVus.Find(donDichVuId);
            if (donDichVu != null)
            {
                donDichVu.TongTien = tongTien;
                db.SaveChanges();
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