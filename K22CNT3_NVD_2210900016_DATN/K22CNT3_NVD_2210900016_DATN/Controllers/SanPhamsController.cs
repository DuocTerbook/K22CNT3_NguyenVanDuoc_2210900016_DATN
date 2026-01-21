using System;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class SanPhamsController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // =======================
        // INDEX
        // =======================
        public ActionResult Index()
        {
            return View(db.SanPhams.ToList());
        }

        // =======================
        // DETAILS
        // =======================
        public ActionResult Details(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
                return HttpNotFound();

            return View(sanPham);
        }

        // =======================
        // CREATE (GET)
        // =======================
        public ActionResult Create()
        {
            // 👉 ÉP RÕ model để tránh MVC tự map sai
            return View(new SanPham());
        }

        // =======================
        // CREATE (POST)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SanPham sanPham)
        {
            // ❗ File upload không map DB → loại khỏi validate
            ModelState.Remove("HinhAnh");

            if (sanPham.HinhAnhFile != null && sanPham.HinhAnhFile.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(sanPham.HinhAnhFile.FileName);
                string path = Server.MapPath("~/Content/Images/" + fileName);
                sanPham.HinhAnhFile.SaveAs(path);
                sanPham.HinhAnh = "/Content/Images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                db.SanPhams.Add(sanPham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sanPham);
        }

        // =======================
        // EDIT (GET)
        // =======================
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
                return HttpNotFound();

            return View(sanPham);
        }

        // =======================
        // EDIT (POST)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SanPham sanPham)
        {
            ModelState.Remove("HinhAnh");

            var sanPhamDb = db.SanPhams.AsNoTracking()
                                      .FirstOrDefault(x => x.ID_SP == sanPham.ID_SP);

            if (sanPhamDb == null)
                return HttpNotFound();

            if (sanPham.HinhAnhFile != null && sanPham.HinhAnhFile.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + Path.GetExtension(sanPham.HinhAnhFile.FileName);
                string path = Server.MapPath("~/Content/Images/" + fileName);
                sanPham.HinhAnhFile.SaveAs(path);
                sanPham.HinhAnh = "/Content/Images/" + fileName;
            }
            else
            {
                sanPham.HinhAnh = sanPhamDb.HinhAnh;
            }

            if (ModelState.IsValid)
            {
                db.Entry(sanPham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sanPham);
        }

        // =======================
        // DELETE
        // =======================
        public ActionResult Delete(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            SanPham sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
                return HttpNotFound();

            return View(sanPham);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sanPham = db.SanPhams.Find(id);
            db.SanPhams.Remove(sanPham);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
                db.Dispose();
            base.Dispose(disposing);
        }
    }
}
