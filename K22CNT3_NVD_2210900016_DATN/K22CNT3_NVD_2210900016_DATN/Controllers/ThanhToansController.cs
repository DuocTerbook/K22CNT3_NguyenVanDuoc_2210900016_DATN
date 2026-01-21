using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class ThanhToansController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // ================== TRANG THANH TOÁN ==================
        public ActionResult Index()
        {
            if (Session["CartId"] == null)
                return RedirectToAction("Index", "GioHangChiTiets");

            int cartId = (int)Session["CartId"];

            var gioHang = db.GioHangChiTiets
                .Include(x => x.SanPham)
                .Include(x => x.GioHang.KhachHang)
                .Where(x => x.ID_GioHang == cartId)
                .ToList();

            if (!gioHang.Any() || gioHang.First().GioHang.KhachHang == null)
                return RedirectToAction("Index", "GioHangChiTiets");

            return View(gioHang);
        }

        // ================== XỬ LÝ THANH TOÁN ==================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult XuLyThanhToan(string PhuongThuc)
        {
            if (Session["CartId"] == null)
                return RedirectToAction("Index", "GioHangChiTiets");

            int cartId = (int)Session["CartId"];

            var gioHangChiTiets = db.GioHangChiTiets
                .Include(x => x.SanPham)
                .Include(x => x.GioHang.KhachHang)
                .Where(x => x.ID_GioHang == cartId)
                .ToList();

            if (!gioHangChiTiets.Any())
                return RedirectToAction("Index", "GioHangChiTiets");

            var gioHang = gioHangChiTiets.First().GioHang;
            var khachHang = gioHang.KhachHang;

            if (khachHang == null)
                return RedirectToAction("Index", "GioHangChiTiets");

            // ========= 1. TẠO ĐƠN HÀNG =========
            DonHang donHang = new DonHang
            {
                ID_KhachHang = khachHang.ID_KhachHang,
                NgayDat = DateTime.Now,
                TrangThai = "Cho xu ly",
                TongTien = 0
            };

            db.DonHangs.Add(donHang);
            db.SaveChanges(); // Lấy ID_DonHang

            // ========= 2. CHI TIẾT ĐƠN HÀNG =========
            decimal tongTien = 0;

            foreach (var ct in gioHangChiTiets)
            {
                if (ct.SanPham.SoLuong < ct.SoLuong)
                {
                    TempData["Error"] = "So luong san pham khong du";
                    return RedirectToAction("Index", "GioHangChiTiets");
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
                tongTien += ct.ThanhTien ?? 0;
            }

            // ========= 3. CẬP NHẬT ĐƠN HÀNG =========
            donHang.TongTien = tongTien;
            donHang.TrangThai = "Dang xu ly";

            // ========= 4. TẠO THANH TOÁN =========
            ThanhToan thanhToan = new ThanhToan
            {
                ID_DonHang = donHang.ID_DonHang,
                PhuongThuc = string.IsNullOrEmpty(PhuongThuc) ? "Tien mat" : PhuongThuc,
                TrangThai = "Da thanh toan",
                NgayThanhToan = DateTime.Now
            };

            db.ThanhToans.Add(thanhToan);

            // ========= 5. HOÀN TẤT GIỎ HÀNG =========
            db.GioHangChiTiets.RemoveRange(gioHangChiTiets);
            gioHang.TrangThai = true;

            db.SaveChanges();
            Session.Remove("CartId");

            TempData["Success"] = "Thanh toan thanh cong";
            return RedirectToAction("ThanhCong");
        }

        // ================== THÀNH CÔNG ==================
        public ActionResult ThanhCong()
        {
            return View();
        }
    }
}
