using EDDY.IS.FormsEngine.Services.Logging;
using EDDY.IS.FormsEngine.Services.Models;
using EDDY.IS.Util.HTMLExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class PrivacyPolicyController : Controller
    {
        // GET: PrivacyPolicy
        public ActionResult Index()
        {
            string privacyPolicyView = string.Empty;

            try
            {
                privacyPolicyView = this.RazorViewToString("~/Templates/Common/PrivacyPolicy.cshtml", model: null, UrlEncoded: false);
            }
            catch (Exception ex)
            {
                WebISException.LogException(HttpContext.Request, EDDY.IS.Base.ISApplication.FormsEngine, ex);
            }

            return new JsonpResult(privacyPolicyView);
        }
    }
}