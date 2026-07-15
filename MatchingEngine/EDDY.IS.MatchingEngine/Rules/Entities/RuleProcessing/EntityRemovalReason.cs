using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public class EntityRemovalReason
    {
        public BaseRuleDefinitionType BaseRuleType { get; set; }
        public string RuleName { get; set; }
        public int? RuleId { get; set; }
        public string StandardControlCode { get; set; }
        public MatchItem matchItem { get; set; }
        public int? ScoreId { get; set; }
    }
}
