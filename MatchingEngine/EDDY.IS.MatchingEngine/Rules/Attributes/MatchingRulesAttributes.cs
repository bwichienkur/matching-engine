using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [AttributeUsage(AttributeTargets.Class)]
    public class MatchingRuleAttributes : System.Attribute
    {
        public EntityProcessing[] EntityProcessingTypes { get; private set; }
        public InputRequired[] InputsRequired { get; private set; }
        public RuleAttribute[] RequiredAttributes { get; private set; }

        public MatchingRuleAttributes()
        {
        }

        public MatchingRuleAttributes(EntityProcessing[] ept, InputRequired[] ir, RuleAttribute[] attributes)
        {
            EntityProcessingTypes = ept;
            InputsRequired = ir;
            RequiredAttributes = attributes;
        }
    }

    public enum RuleAttribute
    {
        ExecuteWarmTransferRules,
        ExecuteCapRule,
        ExecuteSmartMatchRules,
        ExecuteEMSRules,
        ExecuteLeadSubmitRules
    }
}
