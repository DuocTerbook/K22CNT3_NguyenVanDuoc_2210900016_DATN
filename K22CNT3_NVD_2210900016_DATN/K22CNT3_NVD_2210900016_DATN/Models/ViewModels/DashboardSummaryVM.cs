using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Xml.Linq;

namespace K22CNT3_NVD_2210900016_DATN.Models.ViewModels
{
    public class DashboardSummaryVM
    {
        [Display(Name ="Tổng số đơn hôm nay")]
        public int TongDonHomNay { get; set; }
        [Display(Name = "Tổng doanh thu hôm nay")]
        public decimal DoanhThuHomNay { get; set; }
        [Display(Name = "Tổng số khách hôm nay")]
        public int SoKhachHomNay { get; set; }
        [Display(Name = "Tổng sản phẩm")]
        public int TongSanPham { get; set; }
    }
}