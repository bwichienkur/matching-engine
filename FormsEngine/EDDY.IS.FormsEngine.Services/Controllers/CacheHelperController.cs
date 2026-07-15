using EDDY.IS.Util.HTMLExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class CacheHelperController : Controller
    {
        [HttpGet]
        public ActionResult ResetTemplateCache(int TemplateId)
        {
            return new JsonpResult(true);
        }

    }
}
