using EDDY.IS.FormsEngine.Business.Services.Base;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Entities.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Business.Services
{
    public class WizardSubmissionService : SubmissionService 
    {

        public ManagedChoiceResultDTO ProcessSubmission(GetManagedChoiceRequest request)
        {
            var result = new ManagedChoiceResultDTO();
            result.SMLeadsCreatedCount = request.SMLeadsCreatedCount;
            result.USLeadsCreatedCount = request.USLeadsCreatedCount;

            // redirect to Start if LeadData is not properly completed
            if (LeadDataIsNotProperlyDefined(request.LeadData))
            {
                result.MoveToStart = true;
                return result;
            }

            return result;
        }

        private bool LeadDataIsNotProperlyDefined(string leadData)
        {
            return (leadData == null || leadData == "null" || leadData == "undefined" || leadData.Length < 1);
        }

    }
}
