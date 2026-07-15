using EDDY.IS.MatchingEngine.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientRelationship }
                            , new InputRequired[] { InputRequired.User } 
                            , null)]
    public class AgentDisallowedList : Rule, ICRProductRule
    {
        public AgentDisallowedList(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> removed)
        {
            removed = new List<ClientRelationshipProductRuleInput>();
            Dictionary<int, List<AgentDisallowedLiveTransfer>> agentDisallowedList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<AgentDisallowedLiveTransfer>>>(MatchingCacheItem.AgentDisallowedList);

            if (agentDisallowedList != null)
            {
                if (agentDisallowedList.ContainsKey(ruleInput.UserID.Value))
                {
                    foreach (ClientRelationshipProductRuleInput cr in input)
                    {
                        var agent = agentDisallowedList[ruleInput.UserID.Value].FirstOrDefault(x => x.ClientRelationShipId == cr.ClientRelationshipId);
                        if (agent == null)
                            continue;

                        var isWarmTransferProduct = Product.IsWarmTransferProduct(cr.ProductId);

                        if (isWarmTransferProduct && agent.DisallowLiveTransfer)
                        {
                            cr.BaseRuleType = BaseRuleDefinitionType.AgentDisallowedWT;
                            cr.RuleName = "Agent Disallowed for WT";
                            removed.Add(cr);
                        }
                        else if (!isWarmTransferProduct && agent.DisallowForm)
                        {
                            cr.BaseRuleType = BaseRuleDefinitionType.AgentDisallowedWT;
                            cr.RuleName = "Agent Disallowed for Form";
                            removed.Add(cr);
                        }
                    }
                }
            }
        }
    }
}
