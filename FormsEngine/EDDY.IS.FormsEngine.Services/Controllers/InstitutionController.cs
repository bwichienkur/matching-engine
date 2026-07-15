using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Services.Logging;
using EDDY.IS.Util.HTMLExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class InstitutionController : Controller
    {
        private readonly IInstitutionService _institutionService;

        public InstitutionController(IInstitutionService institutionService)
        {
            _institutionService = institutionService;
        }

        public ActionResult GetInstitution(FormRequest formRequest)
        {
            var institution = new Institution();

            try
            {
                var performanceLog = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "InstitutionController.GetInstitution", null, formRequest);
                institution = _institutionService.GetInstitution(formRequest);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex);
            }

            return new JsonpResult(institution);
        }
    }
}