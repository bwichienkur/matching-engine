using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public interface ICRProgramProductRule
    {
        void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input,
                         out List<ProgramProductRuleInput> output);
    }
}
