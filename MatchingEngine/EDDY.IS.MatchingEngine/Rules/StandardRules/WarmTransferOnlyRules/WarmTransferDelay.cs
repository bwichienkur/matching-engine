using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.Constants;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientRelationship
                                                   },
                            null
                            , new RuleAttribute[] { RuleAttribute.ExecuteWarmTransferRules })]
    public class WarmTransferDelay : Rule, ICRProductRule
    {
        public WarmTransferDelay(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> removed)
        {
            removed = new List<ClientRelationshipProductRuleInput>();
            Dictionary<int, VW_Matching_WarmTransferInfo> wtInfo = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_WarmTransferInfo>>(MatchingCacheItem.REWarmTransferInfo);

            if (wtInfo != null)
            {
                foreach (ClientRelationshipProductRuleInput cr in input)
                {
                    if (Product.IsWarmTransferProduct(cr.ProductId) && wtInfo.ContainsKey(cr.ClientRelationshipId))
                    {
                        VW_Matching_WarmTransferInfo wt = wtInfo[cr.ClientRelationshipId];

                        if (wt.WarmTransferTimeDelay.HasValue && wt.LastWarmTransferTime.HasValue)
                        {
                            TimeSpan ts = DateTime.UtcNow - wt.LastWarmTransferTime.Value;

                            if (ts.TotalMinutes < wt.WarmTransferTimeDelay.Value)
                            {
                                cr.BaseRuleType = BaseRuleDefinitionType.WT_TimeDelay;
                                cr.RuleName = "Warm Transfer - Time Delay";
                                removed.Add(cr);
                            }
                        }
                    }
                }
            }
        }
    }
}
