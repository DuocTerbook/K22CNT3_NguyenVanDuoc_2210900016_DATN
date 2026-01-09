using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class GioHangsController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // ================== LẤY GIỎ HÀNG HIỆN TẠI ==================
        private int GetCartId()
        {
            if (Session["CartId"] == null)
            {
                GioHang gioHang = new GioHang
                {
                    NgayTao = DateTime.Now,
                    TrangThai = true
                };

                db.GioHangs.Add(gioHang);
                db.SaveChanges();

                Session["CartId"] = gioHang.ID_GioHang;
            }

            return (int)Session["CartId"];
        }

        // ================== ADD TO CART ==================
        public ActionResult AddToCart(int id)
        {
            int cartId = GetCartId();

            var sanPham = db.SanPhams.Find(id);
            if (sanPham == null)
            {
                return HttpNotFound();
            }

            var ct = db.GioHangChiTiets
                .FirstOrDefault(x => x.ID_GioHang == cartId && x.ID_SP == id);

            if (ct != null)
            {
                ct.SoLuong += 1;
                ct.ThanhTien = ct.SoLuong * ct.DonGia;
            }
            else
            {
                GioHangChiTiet newItem = new GioHangChiTiet
                {
                    ID_GioHang = cartId,
                    ID_SP = id,
                    SoLuong = 1,
                    DonGia = sanPham.DonGia,
                    ThanhTien = sanPham.DonGia
                };

                db.GioHangChiTiets.Add(newItem);
            }

            db.SaveChanges();

            return RedirectToAction("Index", "GioHangChiTiets");
        }
    }
}
