using System;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class DonDichVusController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: DonDichVu
        public ActionResult Index()
        {
            var donDichVus = db.DonDichVus.Include("KhachHang").ToList();
            return View(donDichVus);
        }

        // GET: DonDichVu/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            DonDichVu donDichVu = db.DonDichVus.Include("KhachHang").FirstOrDefault(d => d.ID_DonDV == id);
            if (donDichVu == null)
            {
                return HttpNotFound();
            }
            return View(donDichVu);
        }

        // GET: DonDichVu/Create
        public ActionResult Create()
        {
            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach");
            return View();
        }

        // POST: DonDichVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID_KhachHang,TenKhach,SDT,NgayTra,TongTien,TrangThai,GhiChu")] DonDichVu donDichVu)
        {
            if (ModelState.IsValid)
            {
                donDichVu.NgayNhan = DateTime.Now;
                if (donDichVu.TongTien == 0) donDichVu.TongTien = 0;
                db.DonDichVus.Add(donDichVu);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Tạo đơn dịch vụ thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", donDichVu.ID_KhachHang);
            return View(donDichVu);
        }

        // GET: DonDichVu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            DonDichVu donDichVu = db.DonDichVus.Find(id);
            if (donDichVu == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", donDichVu.ID_KhachHang);
            return View(donDichVu);
        }

        // POST: DonDichVu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_DonDV,ID_KhachHang,TenKhach,SDT,NgayNhan,NgayTra,TongTien,TrangThai,GhiChu")] DonDichVu donDichVu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(donDichVu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật đơn dịch vụ thành công!";
                return RedirectToAction("Index");
            }
            ViewBag.ID_KhachHang = new SelectList(db.KhachHangs, "ID_KhachHang", "TenKhach", donDichVu.ID_KhachHang);
            return View(donDichVu);
        }

        // GET: DonDichVu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            DonDichVu donDichVu = db.DonDichVus.Include("KhachHang").FirstOrDefault(d => d.ID_DonDV == id);
            if (donDichVu == null)
            {
                return HttpNotFound();
            }
            return View(donDichVu);
        }

        // POST: DonDichVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DonDichVu donDichVu = db.DonDichVus.Find(id);
            db.DonDichVus.Remove(donDichVu);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa đơn dịch vụ thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult UpdateStatus(int id, string status)
        {
            var donDichVu = db.DonDichVus.Find(id);
            if (donDichVu != null)
            {
                donDichVu.TrangThai = status;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật trạng thái thành công!";
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