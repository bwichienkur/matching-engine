using EDDY.IS.FormsEngine.Core.DTO.Responses;
using EDDY.IS.FormsEngine.Core.Interfaces;
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
    public class CCPAController : Controller
    {
        private readonly ICCPAMessageService _ccpaMessageService;

        public CCPAController(ICCPAMessageService ccpaMessageService)
        {
            _ccpaMessageService = ccpaMessageService;
        }

        public ActionResult CCPAMessage()
        {
            var ccpaMessageModel = new CCPAMessageModel();

            try
            {
                ccpaMessageModel.CCPAMessage = _ccpaMessageService.BaseCCPAMessage;
            }
            catch (Exception ex)
            {
                WebISException.LogException(HttpContext.Request, EDDY.IS.Base.ISApplication.FormsEngine, ex);
            }

            return PartialView("~/Templates/Common/CCPAMessage.cshtml", ccpaMessageModel);
        }
    }
}