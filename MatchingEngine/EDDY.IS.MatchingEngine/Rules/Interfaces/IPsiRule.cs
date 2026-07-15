using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public interface IPsiRule
    {
        void ExecuteRule(List<PSIRuleInput> input,
                         out List<PSIRuleInput> output);
    }
}
