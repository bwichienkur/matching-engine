using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Services.Controllers.Base;
using EDDY.IS.Util.HTMLExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class WidgetSupportController : DataBindControllerBase
    {
        private readonly IInstitutionService _institutionService;
        public WidgetSupportController(IInstitutionService institutionService)
        {
            _institutionService = institutionService;
        }

        [HttpPost]
        [AllowCrossSiteJsonAttribute]
        public JsonResult GetInstitutionDetails(FormRequest formRequest)
        {
            JsonResult result = null;

            try
            {

                result = Json(_institutionService.GetInstitution(formRequest), JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return result;
        }
    }
}