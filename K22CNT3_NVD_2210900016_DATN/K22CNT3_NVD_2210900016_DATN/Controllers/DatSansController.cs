using System;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class DatSansController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: DatSan
        public ActionResult Index()
        {
            var datSans = db.DatSans.Include("KhachHang").Include("SanCauLong").ToList();
            return View(datSans);
        }

        // GET: DatSan/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            DatSan datSan = db.DatSans.Include("KhachHang").Include("SanCauLong").FirstOrDefault(d => d.ID_DatSan == id);
            if (datSan == null)
            {
                return HttpNotFound();
            }
            return View(datSan);
        }

        // GET: DatSan/Create
        public ActionResult Create()
        {
            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach");
            ViewBag.ID_San = new SelectList(db.SanCauLongs.Where(s => s.TrangThai == "Trống"), "ID_San", "TenSan");
            return View();
        }

        // POST: DatSan/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_KhachHang,ID_San,NgayDat,GioBatDau,GioKetThuc,TienCoc,GhiChu")] DatSan datSan)
        {
            if (ModelState.IsValid)
            {
                // Tính số giờ và tổng tiền
                var san = db.SanCauLongs.Find(datSan.ID_San);
                if (san != null)
                {
                    TimeSpan gioBatDau = datSan.GioBatDau;
                    TimeSpan gioKetThuc = datSan.GioKetThuc;
                    datSan.SoGio = (int)(gioKetThuc - gioBatDau).TotalHours;
                    datSan.TongTien = datSan.SoGio * san.GiaThueTheoGio;
                }

                // Cập nhật trạng thái sân
                san.TrangThai = "Đang thuê";
                db.Entry(san).State = System.Data.Entity.EntityState.Modified;

                db.DatSans.Add(datSan);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Đặt sân thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", datSan.ID_KhachHang);
            ViewBag.ID_San = new SelectList(db.SanCauLongs, "ID_San", "TenSan", datSan.ID_San);
            return View(datSan);
        }

        // GET: DatSan/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            DatSan datSan = db.DatSans.Find(id);
            if (datSan == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", datSan.ID_KhachHang);
            ViewBag.ID_San = new SelectList(db.SanCauLongs, "ID_San", "TenSan", datSan.ID_San);
            return View(datSan);
        }

        // POST: DatSan/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_DatSan,ID_KhachHang,ID_San,NgayDat,GioBatDau,GioKetThuc,SoGio,TongTien,TrangThai,TienCoc,GhiChu")] DatSan datSan)
        {
            if (ModelState.IsValid)
            {
                db.Entry(datSan).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật đặt sân thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", datSan.ID_KhachHang);
            ViewBag.ID_San = new SelectList(db.SanCauLongs, "ID_San", "TenSan", datSan.ID_San);
            return View(datSan);
        }

        // GET: DatSan/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            DatSan datSan = db.DatSans.Include("KhachHang").Include("SanCauLong").FirstOrDefault(d => d.ID_DatSan == id);
            if (datSan == null)
            {
                return HttpNotFound();
            }
            return View(datSan);
        }

        // POST: DatSan/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DatSan datSan = db.DatSans.Find(id);

            // Cập nhật trạng thái sân về trống
            var san = db.SanCauLongs.Find(datSan.ID_San);
            if (san != null && san.TrangThai == "Đang thuê")
            {
                san.TrangThai = "Trống";
                db.Entry(san).State = System.Data.Entity.EntityState.Modified;
            }

            db.DatSans.Remove(datSan);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa đặt sân thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult HoanThanh(int id)
        {
            var datSan = db.DatSans.Find(id);
            if (datSan != null)
            {
                datSan.TrangThai = "Hoàn thành";

                // Cập nhật trạng thái sân về trống
                var san = db.SanCauLongs.Find(datSan.ID_San);
                if (san != null)
                {
                    san.TrangThai = "Trống";
                    db.Entry(san).State = System.Data.Entity.EntityState.Modified;
                }

                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật trạng thái đặt sân thành công!";
            }
            return RedirectToAction("Index");
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