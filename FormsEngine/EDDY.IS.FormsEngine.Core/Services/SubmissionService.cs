using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Mappers;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Util.StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class SubmissionService : ISubmissionService
    {
        private readonly IMatchValidationService _matchValidationService;
        private readonly IFormValidationService _formValidationService;
        private readonly IProspectService _prospectService;
        private readonly ILeadSubmissionService _leadSubmissionService;
        private readonly ISessionService _sessionService;

        public SubmissionService(IMatchValidationService matchValidationService, IFormValidationService formValidationService, IProspectService prospectService, ILeadSubmissionService leadSubmissionService, ISessionService sessionService)
        {
            _matchValidationService = matchValidationService;
            _formValidationService = formValidationService;
            _prospectService = prospectService;
            _leadSubmissionService = leadSubmissionService;
            _sessionService = sessionService;
        }

        public SubmissionResponse SubmitSchoolPickerWizard(FormInput formInput, ref PerformanceLog performanceLog)
        {
            formInput.LeadCreationType = LeadCreationType.SchoolPickerUserSelection;
            return SubmitForm(formInput, ref performanceLog);
        }

        private SubmissionResponse SubmitForm(FormInput formInput, ref PerformanceLog performanceLog)
        {
            ValidateMatches(formInput);
            ValidateForm(formInput, ref performanceLog);
            SaveProspect(formInput);
            var result = SaveLeads(formInput);

            return result;
        }

        private void ValidateMatches(FormInput formInput)
        {
            _matchValidationService.ValidateMatches(formInput);
        }

        private void ValidateForm(FormInput formInput, ref PerformanceLog performanceLog)
        {
            formInput.FormValidationResult = _formValidationService.ValidateForm(formInput, ref performanceLog);
        }

        private void SaveProspect(FormInput formInput)
        {
            if (formInput?.Prospect != null)
            {
                int? prospectId = formInput.Prospect.ProspectId;
                if (prospectId == null || prospectId < 1)
                {
                    formInput.Prospect.ProspectId = _prospectService.SaveProspect(formInput);
                    // should be setting the prospectflowid into the session here as well
                }
                else
                {
                    _prospectService.SaveProspectAsync(formInput);
                }

                AddProspectFlowIdFromSessionToFormInput(formInput);
            }
        }

        private void AddProspectFlowIdFromSessionToFormInput(FormInput formInput)
        {
            formInput.ProspectFlowId = _sessionService.GetProspectFlowId(formInput.FESessionId);
        }

        private SubmissionResponse SaveLeads(FormInput formInput)
        {
            return _leadSubmissionService.SaveSchoolPickerWizardLeads(formInput);
        }
    }
}
