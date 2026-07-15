using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Core.Interfaces
{
    public interface ISessionService
    {
        int GetProspectFlowId(string feSessionId);
        List<int> GetProgramIdsFromLeads(string feSessionId);
        void SetProgramIdsFromLeads(string feSessionId, object value);
        List<decimal> GetLeadIds(string feSessionId);
        void SetLeadIds(string feSessionId, object value);
        string GetUserFullName(string feSessionId);
    }
}
