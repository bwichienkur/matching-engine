using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Mappers;
using EDDY.IS.FormsEngine.Core.Services;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Services.Logging;
using EDDY.IS.FormsEngine.Services.Models.Requests;
using EDDY.IS.Util.HTMLExtensions;
using EDDY.IS.Util.StringExtensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class SubmissionController : Controller
    {
        private readonly FormInputMapper _formInputMapper;
        private readonly ISubmissionService _submissionService;

        public SubmissionController(IIPAddressService ipAddressService, ILocationValidationService locationValidationService, ISubmissionService submissionService)
        {
            _formInputMapper = new FormInputMapper(ipAddressService, locationValidationService);
            _submissionService = submissionService;
        }

        [HttpGet]
        public ActionResult SubmitSchoolPickerWizard(FormRequest formRequest)
        {
            var response = new SubmissionResponse();

            try
            {
                var formInput = _formInputMapper.MapFormRequestToFormInput(formRequest, HttpContext);
                var performanceLog = new PerformanceLog(EDDY.IS.Base.ISApplication.FormsEngine, "SubmissionController.SubmitSchoolPickerWizard", null, formInput);
                response = _submissionService.SubmitSchoolPickerWizard(formInput, ref performanceLog);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex);
            }

            return new JsonpResult(response);
        }

    }
}
