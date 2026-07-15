using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}