using EDDY.IS.FormsEngine.DTO.Extended;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces.Repositories
{
    public interface ISessionRepository
    {
        int GetProspectFlowId(string feSessionId);
        object GetProgramIdsFromLeads(string feSessionId);
        void SetProgramIdsFromLeads(string feSessionId, object value);
        object GetLeadIds(string feSessionId);
        void SetLeadIds(string feSessionId, object value);
        FormsEngineWorkflowStatus GetFormsEngineWorkflowStatus(string feSessionId);
    }
}
