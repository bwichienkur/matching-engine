using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public class LeadPingRuleResult
    {
        public LeadPingRuleResult() {
            this.RemovedInstitutions = new List<LeadPingRuleOutput>();
            this.ScoreInstitution = new List<LeadPingScoreOutput>();
        }
        public List<LeadPingRuleOutput> RemovedInstitutions { get; set; }
        public List<LeadPingScoreOutput> ScoreInstitution { get; set; }
    }
}
