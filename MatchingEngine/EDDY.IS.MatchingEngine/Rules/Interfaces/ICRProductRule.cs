using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    public interface ICRProductRule
    {
        void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                         out List<ClientRelationshipProductRuleInput> output);
    }
}
