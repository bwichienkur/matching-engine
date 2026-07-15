using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class FormValidationResultMapper
    {
        public FormValidationResult MapAPIValidationResultDTOToFormValiationResult(APIValidationResultDTO apiValidationResult)
        {
            return new FormValidationResult
            {
                Valid = apiValidationResult.Valid,
                IsTestLead = apiValidationResult.IsTestLead,
                ValidationMessages = apiValidationResult.ValidationMessages
            };
        }
    }
}
