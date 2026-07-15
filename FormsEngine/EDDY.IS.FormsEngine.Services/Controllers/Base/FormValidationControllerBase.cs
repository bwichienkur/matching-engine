using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Validation;
using EDDY.IS.FormsEngine;

namespace EDDY.IS.FormsEngine.Services.Controllers.Base
{
    public class FormValidationControllerBase : ControllerCommon
    {
        public static ValidationEngine Validation = new ValidationEngine();
        public static LeadPingService.ServiceClient LeadPingService = new LeadPingService.ServiceClient();
    }
}