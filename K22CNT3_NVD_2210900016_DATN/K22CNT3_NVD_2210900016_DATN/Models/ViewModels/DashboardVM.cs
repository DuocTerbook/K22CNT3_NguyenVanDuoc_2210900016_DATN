using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace K22CNT3_NVD_2210900016_DATN.Models.ViewModels
{
    public class DashboardVM
    {
        [Display(Name ="Ngày")]
        public DateTime Ngay { get; set; }
        [Display(Name = "Tổng đơn")]
        public int TongDon { get; set; }
        [Display(Name = "Tổng doanh thu")]
        public decimal TongDoanhThu { get; set; }
        [Display(Name = "Số khách")]
        public int SoKhach { get; set; }
    }
}