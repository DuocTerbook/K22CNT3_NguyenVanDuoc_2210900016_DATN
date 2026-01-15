using System;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class SanCauLongsController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: SanCauLong
        public ActionResult Index()
        {
            return View(db.SanCauLongs.ToList());
        }

        // GET: SanCauLong/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            SanCauLong san = db.SanCauLongs.Find(id);
            if (san == null)
            {
                return HttpNotFound();
            }
            return View(san);
        }

        // GET: SanCauLong/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SanCauLong/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TenSan,LoaiSan,GiaThueTheoGio,TrangThai,MoTa")] SanCauLong san)
        {
            if (ModelState.IsValid)
            {
                db.SanCauLongs.Add(san);
                db.SaveChanges();
                TempData["SuccessMessage"] = "Thêm sân cầu lông thành công!";
                return RedirectToAction("Index");
            }

            return View(san);
        }

        // GET: SanCauLong/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            SanCauLong san = db.SanCauLongs.Find(id);
            if (san == null)
            {
                return HttpNotFound();
            }
            return View(san);
        }

        // POST: SanCauLong/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_San,TenSan,LoaiSan,GiaThueTheoGio,TrangThai,MoTa")] SanCauLong san)
        {
            if (ModelState.IsValid)
            {
                db.Entry(san).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật sân cầu lông thành công!";
                return RedirectToAction("Index");
            }
            return View(san);
        }

        // GET: SanCauLong/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.BadRequest);
            }
            SanCauLong san = db.SanCauLongs.Find(id);
            if (san == null)
            {
                return HttpNotFound();
            }
            return View(san);
        }

        // POST: SanCauLong/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanCauLong san = db.SanCauLongs.Find(id);
            db.SanCauLongs.Remove(san);
            db.SaveChanges();
            TempData["SuccessMessage"] = "Xóa sân cầu lông thành công!";
            return RedirectToAction("Index");
        }

        public ActionResult UpdateStatus(int id, string status)
        {
            var san = db.SanCauLongs.Find(id);
            if (san != null)
            {
                san.TrangThai = status;
                db.SaveChanges();
                TempData["SuccessMessage"] = "Cập nhật trạng thái sân thành công!";
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