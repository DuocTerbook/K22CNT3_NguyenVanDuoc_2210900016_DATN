using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Filters;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    [AdminAuthorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}