using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public abstract class Rule : IRule
    {
        public RuleInput ruleInput { get; private set; }

        public Rule(RuleInput input)
        {
            ruleInput = input;
        }
    }
}
