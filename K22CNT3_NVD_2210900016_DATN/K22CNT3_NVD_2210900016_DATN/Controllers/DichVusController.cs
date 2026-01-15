using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class DichVusController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // ============================
        // GET: DichVus
        // ============================
        public ActionResult Index()
        {
            var dichVus = db.DichVus
                            .Include(d => d.LoaiDichVu)
                            .ToList();
            return View(dichVus);
        }

        // ============================
        // GET: DichVus/Details/5
        // ============================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var dichVu = db.DichVus
                           .Include(d => d.LoaiDichVu)
                           .FirstOrDefault(d => d.ID_DichVu == id);

            if (dichVu == null)
                return HttpNotFound();

            return View(dichVu);
        }

        // ============================
        // GET: DichVus/Create
        // ============================
        public ActionResult Create()
        {
            ViewBag.ID_LoaiDV = new SelectList(
                db.LoaiDichVus,
                "ID_LoaiDV",
                "TenLoaiDV"
            );

            return View();
        }

        // ============================
        // POST: DichVus/Create
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include = "TenDichVu,ID_LoaiDV,DonGia,ThoiGianThucHien,TrangThai")]
            DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                // Nếu không tick trạng thái → mặc định false
                if (!dichVu.TrangThai.HasValue)
                    dichVu.TrangThai = false;

                db.DichVus.Add(dichVu);
                db.SaveChanges();

                TempData["SuccessMessage"] = "Thêm dịch vụ thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.ID_LoaiDV = new SelectList(
                db.LoaiDichVus,
                "ID_LoaiDV",
                "TenLoaiDV",
                dichVu.ID_LoaiDV
            );

            return View(dichVu);
        }

        // ============================
        // GET: DichVus/Edit/5
        // ============================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            DichVu dichVu = db.DichVus.Find(id);
            if (dichVu == null)
                return HttpNotFound();

            ViewBag.ID_LoaiDV = new SelectList(
                db.LoaiDichVus,
                "ID_LoaiDV",
                "TenLoaiDV",
                dichVu.ID_LoaiDV
            );

            return View(dichVu);
        }

        // ============================
        // POST: DichVus/Edit/5
        // ============================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include = "ID_DichVu,TenDichVu,ID_LoaiDV,DonGia,ThoiGianThucHien,TrangThai")]
            DichVu dichVu)
        {
            if (ModelState.IsValid)
            {
                if (!dichVu.TrangThai.HasValue)
                    dichVu.TrangThai = false;

                db.Entry(dichVu).State = EntityState.Modified;
                db.SaveChanges();

                TempData["SuccessMessage"] = "Cập nhật dịch vụ thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.ID_LoaiDV = new SelectList(
                db.LoaiDichVus,
                "ID_LoaiDV",
                "TenLoaiDV",
                dichVu.ID_LoaiDV
            );

            return View(dichVu);
        }

        // ============================
        // GET: DichVus/Delete/5
        // ============================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var dichVu = db.DichVus
                           .Include(d => d.LoaiDichVu)
                           .FirstOrDefault(d => d.ID_DichVu == id);

            if (dichVu == null)
                return HttpNotFound();

            return View(dichVu);
        }

        // ============================
        // POST: DichVus/Delete/5
        // ============================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DichVu dichVu = db.DichVus.Find(id);

            if (dichVu != null)
            {
                db.DichVus.Remove(dichVu);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Xóa dịch vụ thành công!";
            }

            return RedirectToAction("Index");
        }

        // ============================
        // Dispose
        // ============================
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
