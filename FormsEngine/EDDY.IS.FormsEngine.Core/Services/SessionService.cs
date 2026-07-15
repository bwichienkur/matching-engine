using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.DTO.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Services
{
    public class SessionService : ISessionService
    {
        private readonly ISessionRepository _sessionRepository;

        public SessionService(ISessionRepository sessionRepository)
        {
            _sessionRepository = sessionRepository;
        }

        public int GetProspectFlowId(string feSessionId)
        {
            return _sessionRepository.GetProspectFlowId(feSessionId);
        }

        public List<int> GetProgramIdsFromLeads(string feSessionId)
        {
            var programIds = _sessionRepository.GetProgramIdsFromLeads(feSessionId) as IEnumerable<int>;
            return programIds?.ToList() ?? new List<int>();
        }

        public void SetProgramIdsFromLeads(string feSessionId, object value)
        {
            _sessionRepository.SetProgramIdsFromLeads(feSessionId, value);
        }

        public List<decimal> GetLeadIds(string feSessionId)
        {
            var leadIds = _sessionRepository.GetLeadIds(feSessionId) as IEnumerable<decimal>;
            return leadIds?.ToList() ?? new List<decimal>();
        }

        public void SetLeadIds(string feSessionId, object value)
        {
            _sessionRepository.SetLeadIds(feSessionId, value);
        }

        public string GetUserFullName(string feSessionId)
        {
            string userFullName = null;

            if (!string.IsNullOrWhiteSpace(feSessionId))
            {
                FormsEngineWorkflowStatus workflowStatus = _sessionRepository.GetFormsEngineWorkflowStatus(feSessionId);
                userFullName = workflowStatus?.UserFullName;
            }

            return !string.IsNullOrWhiteSpace(userFullName) ? userFullName : string.Empty;
        }
    }
}
