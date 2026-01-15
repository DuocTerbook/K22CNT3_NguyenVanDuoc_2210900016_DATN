using System;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class LoaiDichVusController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: LoaiDichVu
        public ActionResult Index()
        {
            return View(db.LoaiDichVus.ToList());
        }

        // GET: LoaiDichVu/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            LoaiDichVu loaiDichVu = db.LoaiDichVus.Find(id);
            if (loaiDichVu == null)
            {
                return HttpNotFound();
            }
            return View(loaiDichVu);
        }

        // GET: LoaiDichVu/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: LoaiDichVu/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TenLoaiDV,MoTa")] LoaiDichVu loaiDichVu)
        {
            if (ModelState.IsValid)
            {
                db.LoaiDichVus.Add(loaiDichVu);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm loại dịch vụ thành công!";
                return RedirectToAction("Index");
            }

            return View(loaiDichVu);
        }

        // GET: LoaiDichVu/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            LoaiDichVu loaiDichVu = db.LoaiDichVus.Find(id);
            if (loaiDichVu == null)
            {
                return HttpNotFound();
            }
            return View(loaiDichVu);
        }

        // POST: LoaiDichVu/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_LoaiDV,TenLoaiDV,MoTa")] LoaiDichVu loaiDichVu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(loaiDichVu).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật loại dịch vụ thành công!";
                return RedirectToAction("Index");
            }
            return View(loaiDichVu);
        }

        // GET: LoaiDichVu/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            LoaiDichVu loaiDichVu = db.LoaiDichVus.Find(id);
            if (loaiDichVu == null)
            {
                return HttpNotFound();
            }
            return View(loaiDichVu);
        }

        // POST: LoaiDichVu/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LoaiDichVu loaiDichVu = db.LoaiDichVus.Find(id);
            db.LoaiDichVus.Remove(loaiDichVu);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa loại dịch vụ thành công!";
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