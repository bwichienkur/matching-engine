using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Services.Logging;
using EDDY.IS.Util.HTMLExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class LocationController : Controller
    {
        private readonly ILocationValidationService _locationValidationService;
        private readonly IIPAddressService _ipAddressService;

        public LocationController(ILocationValidationService locationValidationService, IIPAddressService ipAddressService)
        {
            _locationValidationService = locationValidationService;
            _ipAddressService = ipAddressService;
        }

        [HttpGet]
        public ActionResult GetPostalCode(string ipOverride)
        {
            string postalCode;
            try
            {
                string ipAddress = !string.IsNullOrWhiteSpace(ipOverride) ? ipOverride : _ipAddressService.GetIPAddress(HttpContext.Request.ServerVariables["HTTP_VIA"], HttpContext.Request["HTTP_X_FORWARDED_FOR"], HttpContext.Request["REMOTE_ADDR"]);
                postalCode = _locationValidationService.GetPostalCode(ipAddress);
            }
            catch (Exception ex)
            {
                postalCode = string.Empty;
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex);
            }

            return new JsonpResult(postalCode);
        }
    }
}