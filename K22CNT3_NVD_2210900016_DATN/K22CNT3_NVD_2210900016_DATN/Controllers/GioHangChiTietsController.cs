using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class GioHangChiTietsController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // =======================
        // HIỂN THỊ GIỎ HÀNG
        // =======================
        public ActionResult Index()
        {
            if (Session["CartId"] == null)
                return View(new List<GioHangChiTiet>());

            int cartId = (int)Session["CartId"];

            var list = db.GioHangChiTiets
                .Include(x => x.SanPham)
                .Include(x => x.GioHang.KhachHang)
                .Where(x => x.ID_GioHang == cartId)
                .ToList();

            return View(list);
        }

        // =======================
        // SỬA GIỎ HÀNG (GET)
        // =======================
        public ActionResult Edit(int id)
        {
            var ct = db.GioHangChiTiets
                .Include(x => x.SanPham)
                .FirstOrDefault(x => x.ID_CTGH == id);

            if (ct == null)
                return HttpNotFound();

            return View(ct);
        }
        // =======================
        // SỬA GIỎ HÀNG (POST)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(GioHangChiTiet model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var ct = db.GioHangChiTiets
                .Include(x => x.GioHang)
                .FirstOrDefault(x => x.ID_CTGH == model.ID_CTGH);

            if (ct == null)
                return HttpNotFound();

            ct.SoLuong = model.SoLuong;
            ct.ThanhTien = ct.SoLuong * ct.DonGia;

            // cập nhật lại tổng tiền giỏ hàng
            ct.GioHang.TongTien = ct.GioHang.GioHangChiTiets.Sum(x => x.ThanhTien ?? 0);

            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // =======================
        // XÓA (GET)
        // =======================
        public ActionResult Delete(int id)
        {
            var ct = db.GioHangChiTiets
                .Include(x => x.SanPham)
                .FirstOrDefault(x => x.ID_CTGH == id);

            if (ct == null)
                return HttpNotFound();

            return View(ct);
        }

        // =======================
        // XÓA (POST)
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int ID_CTGH)
        {
            var ct = db.GioHangChiTiets.Find(ID_CTGH);
            if (ct != null)
            {
                db.GioHangChiTiets.Remove(ct);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // =======================
        // LƯU THÔNG TIN KHÁCH
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LuuThongTinKhach(string TenKhach, string DienThoai, string Email, string DiaChi)
        {
            if (Session["CartId"] == null)
                return RedirectToAction("Index");

            int cartId = (int)Session["CartId"];

            var gioHang = db.GioHangs
                .Include(g => g.KhachHang)
                .Include(g => g.GioHangChiTiets)
                .FirstOrDefault(g => g.ID_GioHang == cartId);

            if (gioHang == null)
                return RedirectToAction("Index");

            if (gioHang.KhachHang == null)
            {
                gioHang.KhachHang = new KhachHang();
                db.KhachHangs.Add(gioHang.KhachHang);
            }

            gioHang.KhachHang.TenKhach = TenKhach;
            gioHang.KhachHang.DienThoai = DienThoai;
            gioHang.KhachHang.Email = Email;
            gioHang.KhachHang.DiaChi = DiaChi;

            gioHang.TongTien = gioHang.GioHangChiTiets.Sum(ct => ct.ThanhTien ?? 0);

            db.SaveChanges();
            TempData["Success"] = "Lưu thông tin khách hàng thành công";

            return RedirectToAction("Index");
        }

        // =======================
        // THANH TOÁN
        // =======================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ThanhToan()
        {
            if (Session["CartId"] == null)
                return RedirectToAction("Index");

            int cartId = (int)Session["CartId"];

            var gioHang = db.GioHangs
                .Include(g => g.KhachHang)
                .Include(g => g.GioHangChiTiets.Select(ct => ct.SanPham))
                .FirstOrDefault(g => g.ID_GioHang == cartId);

            if (gioHang == null || gioHang.KhachHang == null || !gioHang.GioHangChiTiets.Any())
                return RedirectToAction("Index");

            // ✅ TÍNH LẠI TỔNG TIỀN CHUẨN
            decimal tongTien = gioHang.GioHangChiTiets.Sum(ct => ct.ThanhTien ?? 0);

            // 1️⃣ Tạo đơn hàng
            DonHang donHang = new DonHang
            {
                NgayDat = DateTime.Now,
                TongTien = tongTien, // ⭐ QUAN TRỌNG
                TrangThai = "Da thanh toan",
                ID_KhachHang = gioHang.KhachHang.ID_KhachHang
            };

            db.DonHangs.Add(donHang);
            db.SaveChanges();

            // 2️⃣ Chi tiết đơn hàng + trừ kho
            foreach (var ct in gioHang.GioHangChiTiets.ToList())
            {
                if (ct.SanPham.SoLuong < ct.SoLuong)
                {
                    TempData["Error"] = "Sản phẩm " + ct.SanPham.TenSP + " không đủ số lượng";
                    return RedirectToAction("Index");
                }

                db.ChiTietDonHangs.Add(new ChiTietDonHang
                {
                    ID_DonHang = donHang.ID_DonHang,
                    ID_SP = ct.ID_SP,
                    SoLuong = ct.SoLuong,
                    DonGia = ct.DonGia,
                    ThanhTien = ct.ThanhTien
                });

                ct.SanPham.SoLuong -= ct.SoLuong;
            }

            // 3️⃣ Thanh toán
            db.ThanhToans.Add(new ThanhToan
            {
                NgayThanhToan = DateTime.Now,
                PhuongThuc = "Tien mat",
                TrangThai = "Da thanh toan",
                ID_DonHang = donHang.ID_DonHang
            });

            // 4️⃣ Xóa giỏ hàng
            db.GioHangChiTiets.RemoveRange(gioHang.GioHangChiTiets);
            gioHang.TrangThai = true;

            db.SaveChanges();
            Session.Remove("CartId");

            return RedirectToAction("ThanhCong");
        }


        // =======================
        // TRANG THÀNH CÔNG
        // =======================
        public ActionResult ThanhCong()
        {
            return View();
        }
    }
}
