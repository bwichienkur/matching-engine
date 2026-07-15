using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.DTO.Responses;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Mappers;
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
    public class SchoolPickerWizardController : Controller
    {

        private readonly IUserSelectionService _userSelectionService;
        private readonly IFailedMatchReplacementService _failedMatchReplacementService;
        private readonly IComponentRenderingService _componentRenderingService;
        private readonly IComponentTemplateService _componentTemplateService;
        private readonly IMetaDataService _metaDataService;
        private readonly FormInputMapper _formInputMapper;

        public SchoolPickerWizardController(IUserSelectionService userSelectionService, IFailedMatchReplacementService failedMatchReplacementService, IComponentRenderingService componentRenderingService, IIPAddressService ipAddressService, ILocationValidationService locationValidationService, IComponentTemplateService componentTemplateService, IMetaDataService metaDataService)
        {
            _userSelectionService = userSelectionService;
            _failedMatchReplacementService = failedMatchReplacementService;
            _componentRenderingService = componentRenderingService;
            _componentTemplateService = componentTemplateService;
            _metaDataService = metaDataService;
            _formInputMapper = new FormInputMapper(ipAddressService, locationValidationService);
        }

        [HttpGet]
        public ActionResult GetFailedMatchReplacements(FormRequest request)
        {
            var response = new JsonpResult();

            try
            {
                FormInput formInput = _formInputMapper.MapFormRequestToFormInput(request, HttpContext);
                FailedMatchReplacements failedMatchReplacements = _failedMatchReplacementService.GetReplacementsForFailedMatches(formInput);
                List<string> replacementHtmlComponents = _componentRenderingService.RenderComponents(_componentTemplateService.FailedMatchReplacementComponentTemplateKey, failedMatchReplacements.ReplacementMatches);

                response = new JsonpResult(new
                {
                    FailedMatches = failedMatchReplacements.FailedMatches,
                    ReplacementMatches = failedMatchReplacements.ReplacementMatches,
                    Message = failedMatchReplacements.Message,
                    ReplacementHtmlComponents = replacementHtmlComponents
                });
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex);
            }

            return response;
        }

        [HttpGet]
        public ActionResult GetSchoolPickerCarouselComponents(FormRequest request)
        {
            var response = new JsonpResult();

            try
            {
                FormInput formInput = _formInputMapper.MapFormRequestToFormInput(request, HttpContext);
                UserSelectionResponse userSelectionResponse = _userSelectionService.GetUserSelectionsForSchoolPicker(formInput);
                List<string> components = _componentRenderingService.RenderComponents(_componentTemplateService.SchoolPickerMatchComponentTemplateKey, userSelectionResponse.UserSelections);
                Dictionary<string, string> metaDataMessages = _metaDataService.GetMetaDataMessagesByPrefix("SCHOOLPICKERWIZARD");

                response = new JsonpResult(new
                {
                    components,
                    metaDataMessages,
                    matchResponseGuid = userSelectionResponse.MatchResponseGuid
                });
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex);
            }

            return response;
        }
    }
}