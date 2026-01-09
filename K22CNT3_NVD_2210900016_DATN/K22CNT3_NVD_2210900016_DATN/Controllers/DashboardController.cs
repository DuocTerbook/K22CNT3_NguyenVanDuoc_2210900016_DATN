using K22CNT3_NVD_2210900016_DATN.Models.ViewModels;
using K22CNT3_NVD_2210900016_DATN.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class DashboardController : Controller
    {
        QuanLyVotEntities db = new QuanLyVotEntities();

        public ActionResult Index()
        {
            DateTime homNay = DateTime.Today;

            // ==== THỐNG KÊ NHANH ====
            var summary = new DashboardSummaryVM
            {
                TongDonHomNay = db.DonHangs
                    .Count(d => DbFunctions.TruncateTime(d.NgayDat) == homNay),

                DoanhThuHomNay = db.DonHangs
                    .Where(d => DbFunctions.TruncateTime(d.NgayDat) == homNay)
                    .Sum(d => (decimal?)d.TongTien) ?? 0,

                SoKhachHomNay = db.DonHangs
                    .Where(d => DbFunctions.TruncateTime(d.NgayDat) == homNay)
                    .Select(d => d.ID_KhachHang)
                    .Distinct()
                    .Count(),

                TongSanPham = db.SanPhams.Count()
            };

            // ==== BIỂU ĐỒ THEO NGÀY ====
            var chartData = db.DonHangs
                .GroupBy(d => DbFunctions.TruncateTime(d.NgayDat))
                .Select(g => new DashboardVM
                {
                    Ngay = g.Key.Value,
                    TongDon = g.Count(),
                    TongDoanhThu = g.Sum(x => (decimal?)x.TongTien) ?? 0,
                    SoKhach = g.Select(x => x.ID_KhachHang).Distinct().Count()
                })
                .OrderBy(x => x.Ngay)
                .ToList();


            ViewBag.Summary = summary;

            return View(chartData);
        }
    }
}