using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.FormsEngine.Infastructure.Mappers;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.Infastructure.Mappers.Inbound;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class LeadRepository : ILeadRepository
    {
        private LeadSaveManger _leadSaveManager;

        public LeadRepository()
        {
            var log = new PerformanceLog();
            _leadSaveManager = new LeadSaveManger(log);
        }

        public List<Lead> SaveLeads(FormInput formInput)
        {
            RawPostDataDTO rawPostData = GetRawPostData(formInput);

            var apiValidationResultMapper = new APIValidationResultMapper();
            var apiValidationResult = apiValidationResultMapper.MapFormValidationResultToApiValidationResult(formInput.FormValidationResult);

            var leadCreateRequestMapper = new LeadCreateRequestMapper();
            List<LeadCreateRequest> leadCreateRequests = leadCreateRequestMapper.MapFormInputToLeadCreateRequest(formInput);

            List<LeadCreateResponse> leadCreateResponses = _leadSaveManager.Execute(formInput.TemplateId, leadCreateRequests, formInput.SessionId, ISApplication.FormsEngine, rawPostData, formInput.Prospect.ProspectId, null, MatchResponseType.SchoolSelection, formInput.AdvisorId, false, apiValidationResult, null, formInput.ProspectFlowId);

            var mapper = new LeadMapper();
            var leads = mapper.MapLeadCreateResponsesToLeads(leadCreateResponses);

            return leads;
        }

        private RawPostDataDTO GetRawPostData(FormInput formInput)
        {
            var rawPostDataMapper = new RawPostDataMapper();
            return rawPostDataMapper.MapFormInputToRawPostData(formInput);
        }

    }
}
