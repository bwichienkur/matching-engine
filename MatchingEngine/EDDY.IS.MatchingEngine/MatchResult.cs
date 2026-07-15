using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public class MatchResult
    {
        public List<MatchItem> MatchItemList { get; set; }
        public BusinessModel ChosenBusinessModel { get; set; }
        public RulesResult RulesResult { get; set; }
        public Campaign ChosenCampaign { get; set; }
    }

    public class ValidateProgramMatchResult : MatchResult
    {
        public ValidateProgramMatchResultType ResultType { get; set; }
    }

    public enum ValidateProgramMatchResultType
    {
        MatchExists,
        ProgramNotAvailable,
        CampaignCappedOut,
        CampaignInactive,
        CampaignLeadScore,
        InquiryDisabled,
        RuleFailure,
        NoPaidPrograms,
        CampaignRestriction
    }
}
