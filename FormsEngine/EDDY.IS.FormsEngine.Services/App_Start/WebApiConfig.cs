using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using EDDY.IS.FormsEngine.Services.Formatters;

namespace EDDY.IS.FormsEngine.Services
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //Disable XML responses
            config.Formatters.Remove(config.Formatters.XmlFormatter);

            config.Formatters.Insert(0, new JsonpFormatter());
        }
    }
}
