using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using EDDY.IS.Util.StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class FormValidationRepository : IFormValidationRepository
    {
        public FormValidationResult ValidateForm(FormInput formInput, ref PerformanceLog performanceLog)
        {
            var formsEngineService = new FormsEngine();
            APIValidationResultDTO apiValidationResult = formsEngineService.QuickCheckValidateForm(formInput.TemplateId, formInput.IsBeta, formInput.TrackId, formInput.LeadData.BuildCaseInsensitiveDictionary(), ref performanceLog);

            var mapper = new FormValidationResultMapper();
            var formValidationResult = mapper.MapAPIValidationResultDTOToFormValiationResult(apiValidationResult);

            return formValidationResult;
        }
    }
}
