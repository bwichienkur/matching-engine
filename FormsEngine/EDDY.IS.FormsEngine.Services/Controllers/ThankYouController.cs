using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Enums;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Mappers;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Services.Logging;
using EDDY.IS.FormsEngine.Services.Models;
using EDDY.IS.Util.HTMLExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class ThankYouController : Controller
    {
        private readonly IHtmlRenderingStrategyService _htmlRenderingStrategyService;
        private readonly IThankYouPageService _thankYouPageService;

        public ThankYouController(IHtmlRenderingStrategyService htmlRenderingStrategyService, IThankYouPageService thankYouPageService)
        {
            _htmlRenderingStrategyService = htmlRenderingStrategyService;
            _thankYouPageService = thankYouPageService;
        }

        [HttpGet]
        public ActionResult GetThankYouPage(FormRequest formRequest)
        {
            var thankYouPageResponse = new ThankYouPageResponse();

            try
            {
                string thankYouTemplate = _htmlRenderingStrategyService.GetHtmlRenderingStrategyThankYouTemplate(formRequest.RenderingStrategy);
                ThankYouPage thankYouPage = _thankYouPageService.GetThankYouPage(formRequest);

                if (thankYouPage?.LineItems?.Count() <= 0)
                {
                    thankYouPageResponse.MoveToNoMatch = true;
                }

                thankYouPageResponse.RenderedThankYou = this.RazorViewToString(thankYouTemplate, thankYouPage, true);
            }
            catch (Exception ex)
            {
                WebISException.LogException(Request, EDDY.IS.Base.ISApplication.FormsEngine, ex);
            }

            return new JsonpResult(thankYouPageResponse);
        }
    }
}