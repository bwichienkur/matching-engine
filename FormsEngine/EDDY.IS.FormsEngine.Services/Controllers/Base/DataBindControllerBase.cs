using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Validation;
using EDDY.IS.FormsEngine;

using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers.Base
{
    public class DataBindControllerBase : ControllerCommon
    {
        public static ValidationEngine Validation = new ValidationEngine();

    }
    public class AllowCrossSiteJsonAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            filterContext.RequestContext.HttpContext.Response.AddHeader("Access-Control-Allow-Origin", "*");
            base.OnActionExecuting(filterContext);
        }
    }
}