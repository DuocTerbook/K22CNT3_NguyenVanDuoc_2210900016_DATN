using System.Linq;
using System.Web.Mvc;
using K22CNT3_NVD_2210900016_DATN.Models;

namespace K22CNT3_NVD_2210900016_DATN.Controllers
{
    public class AdminAuthController : Controller
    {
        private QuanLyVotEntities db = new QuanLyVotEntities();

        // GET: /AdminAuth/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: /AdminAuth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string taiKhoan, string matKhau)
        {
            var admin = db.QuanTris.FirstOrDefault(x =>
                x.TaiKhoan == taiKhoan && x.MatKhau == matKhau);

            if (admin != null)
            {
                // CHỈ SET 1 SESSION
                Session["Admin"] = admin;

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Sai tài khoản hoặc mật khẩu";
            return View();
        }

        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
