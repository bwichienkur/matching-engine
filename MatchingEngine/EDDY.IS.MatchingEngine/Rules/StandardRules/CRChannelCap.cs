using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DTO;
using System.Configuration;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientRelationship
                                                   }, null
                                                   ,
                           new RuleAttribute[] { RuleAttribute.ExecuteCapRule })]
    public class CRChannelCap : Rule, ICRProductRule
    {
        private int? ChannelId { get; set; }

        public CRChannelCap(RuleInput input)
            : base(input)
        {
            if (input.Campaign != null)
                ChannelId = input.Campaign.ChannelId;
        }

        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> output)
        {
            Dictionary<int, Dictionary<int, VW_Matching_ClientRelationshipChannelCaps>> channelCaps = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, Dictionary<int, VW_Matching_ClientRelationshipChannelCaps>>>(MatchingCacheItem.ClientRelationshipChannelCaps);

            List<ClientRelationshipProductRuleInput> removedCRProducts = new List<ClientRelationshipProductRuleInput>();

            if (ChannelId.HasValue && channelCaps != null)
            {
                foreach (ClientRelationshipProductRuleInput crProduct in input)
                {
                    if (channelCaps.ContainsKey(crProduct.ClientRelationshipId))
                    {
                        if (channelCaps[crProduct.ClientRelationshipId].ContainsKey(ChannelId.Value))
                        {
                            VW_Matching_ClientRelationshipChannelCaps c = channelCaps[crProduct.ClientRelationshipId][ChannelId.Value];

                            if (c.LeadCount >= c.CapAmount)
                            {
                                crProduct.BaseRuleType = BaseRuleDefinitionType.LeadCap;
                                crProduct.RuleName = "Daily Channel Cap for CR Reached";
                                removedCRProducts.Add(crProduct);
                            }
                        }
                    }
                }
            }

            output = removedCRProducts;
        }
    }
}
