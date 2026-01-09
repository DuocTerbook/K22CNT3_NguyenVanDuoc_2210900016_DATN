using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class SanPhamsController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: SanPham
        public ActionResult Index()
        {
            var sanpham = db.SanPhams.ToList();
            return View(sanpham);
        }

        // GET: SanPham/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            SanPham sanpham = db.SanPhams.Find(id);
            if (sanpham == null) return HttpNotFound();
            return View(sanpham);
        }

        // GET: SanPham/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: SanPham/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TenSP,ThuongHieu,DonGia,SoLuong,DVT,BaoHanhThang,HinhAnh,HinhAnhFile,MoTa,TrangThai")] SanPham sanpham)
        {
            if (sanpham.HinhAnhFile != null && sanpham.HinhAnhFile.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(sanpham.HinhAnhFile.FileName);
                string path = Server.MapPath("~/Content/Images/" + fileName);
                sanpham.HinhAnhFile.SaveAs(path);
                sanpham.HinhAnh = "/Content/Images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                db.SanPhams.Add(sanpham);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(sanpham);
        }

        // GET: SanPham/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            SanPham sanpham = db.SanPhams.Find(id);
            if (sanpham == null) return HttpNotFound();
            return View(sanpham);
        }

        // POST: SanPham/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID_SP,TenSP,ThuongHieu,DonGia,SoLuong,DVT,BaoHanhThang,HinhAnh,HinhAnhFile,MoTa,TrangThai")] SanPham sanpham)
        {
            if (sanpham.HinhAnhFile != null && sanpham.HinhAnhFile.ContentLength > 0)
            {
                string fileName = Guid.NewGuid() + System.IO.Path.GetExtension(sanpham.HinhAnhFile.FileName);
                string path = Server.MapPath("~/Content/Images/" + fileName);
                sanpham.HinhAnhFile.SaveAs(path);
                sanpham.HinhAnh = "/Content/Images/" + fileName;
            }

            if (ModelState.IsValid)
            {
                db.Entry(sanpham).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(sanpham);
        }

        // GET: SanPham/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            SanPham sanpham = db.SanPhams.Find(id);
            if (sanpham == null) return HttpNotFound();
            return View(sanpham);
        }

        // POST: SanPham/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SanPham sanpham = db.SanPhams.Find(id);
            db.SanPhams.Remove(sanpham);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}
