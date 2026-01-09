using System.Web.Mvc;
using System.Web.Routing;

namespace K22CNT3_NVD_2210900016_DATN.Filters
{
    public class AdminAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["Admin"] == null)
            {
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new { controller = "AdminAuth", action = "Login" }
                    )
                );
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}
