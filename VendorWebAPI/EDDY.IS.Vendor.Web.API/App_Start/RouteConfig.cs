using System.Web.Mvc;
using System.Web.Routing;

namespace EDDY.IS.Vendor.Web.API
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");



            routes.MapRoute(
          "Default",
          "",
          new { controller = "Help", action = "Index" }
      ).DataTokens = new RouteValueDictionary(new { area = "HelpPage" });

            routes.MapRoute(
 "LeadDetailsSaveNoteRoute",
 "LeadDetails/SaveNote",
 new { controller = "LeadDetails", action = "SaveNote", id = UrlParameter.Optional }
);
            routes.MapRoute(
             "LeadDetailsRoute",
             "LeadDetails",
             new { controller = "LeadDetails", action = "Index", id = UrlParameter.Optional }

        );

        }
    }
}
