using System.Web;
using System.Web.Mvc;

namespace K22CNT3_NguyenVanDuoc_2210900016_DATN
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
