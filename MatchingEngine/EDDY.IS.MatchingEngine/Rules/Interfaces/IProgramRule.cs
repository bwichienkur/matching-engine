using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public interface IProgramRule
    {
        void ExecuteRule(List<ProgramRuleInput> input,
                         out List<ProgramRuleInput> output);
    }
}
