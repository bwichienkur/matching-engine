using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.Core.Interfaces.Repositories;
using EDDY.IS.FormsEngine.DTO.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Repositories
{
    public class SessionRepository : ISessionRepository
    {
        public int GetProspectFlowId(string feSessionId)
        {
            return FESession.Get(feSessionId, Constants.PROSPECT_WORKFLOWID) == null ? 0 : (int)FESession.Get(feSessionId, Constants.PROSPECT_WORKFLOWID);
        }

        public object GetProgramIdsFromLeads(string feSessionId)
        {
            return FESession.Get(feSessionId, Constants.WIZARD_USERSELECTPROGRAMIDS_KEY);
        }

        public void SetProgramIdsFromLeads(string feSessionId, object value)
        {
            FESession.Set(feSessionId, Constants.WIZARD_USERSELECTPROGRAMIDS_KEY, value);
        }

        public object GetLeadIds(string feSessionId)
        {
            return FESession.Get(feSessionId, Constants.WIZARD_USERSELECTLEADIDS_KEY);
        }

        public void SetLeadIds(string feSessionId, object value)
        {
            FESession.Set(feSessionId, Constants.WIZARD_USERSELECTLEADIDS_KEY, value);
        }

        public FormsEngineWorkflowStatus GetFormsEngineWorkflowStatus(string feSessionId)
        {
            return FESession.Get<FormsEngineWorkflowStatus>(feSessionId, Constants.WORKFLOW_SESSIONKEY);
        }
    }
}
