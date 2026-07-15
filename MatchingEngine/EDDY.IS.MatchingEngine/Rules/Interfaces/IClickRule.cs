using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public interface IClickRule
    {
        void ExecuteRule(List<ClickRuleInput> input,
                         out List<ClickRuleInput> output);
    }
}
