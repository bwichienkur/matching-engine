using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Services.Controllers.Base;
using EDDY.IS.Util.HTMLExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class OpenMailController : OpenMailControllerBase
    {

        [HttpGet]
        public ActionResult ValidateRules(int ProfileId)
        {
            DTO.OpenMailValidationDTO Result = new DTO.OpenMailValidationDTO() { SentToNoMatch = false };
            Dictionary<string, string> formFields = Request.QueryString.Keys.Cast<string>()
                .ToDictionary(key => key, value => Request.QueryString[value]);

            try
            {
                Result = FormsEngineService.ShouldSendToNoMatch(ProfileId, formFields);
            }
            catch (Exception ex)
            {
                new ISException(IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return new JsonpResult(Result);
        }
    }
}