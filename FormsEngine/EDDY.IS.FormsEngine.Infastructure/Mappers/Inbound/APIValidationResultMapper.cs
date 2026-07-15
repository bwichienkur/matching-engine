using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers.Inbound
{
    public class APIValidationResultMapper
    {

        public APIValidationResultDTO MapFormValidationResultToApiValidationResult(FormValidationResult formValidationResult)
        {
            var apiValidationResult = new APIValidationResultDTO();

            if (formValidationResult != null)
            {
                apiValidationResult.Valid = formValidationResult.Valid;
                apiValidationResult.IsTestLead = formValidationResult.IsTestLead;
                apiValidationResult.ValidationMessages = formValidationResult.ValidationMessages;
            }

            return apiValidationResult;
        }

    }
}
