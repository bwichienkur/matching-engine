using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientRelationship
                                                   },
                            null
                            , null)]
    public class Campaign : Rule, ICRProductRule
    {
        public Campaign(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> removed)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            removed = new List<ClientRelationshipProductRuleInput>();
            EDDY.IS.MatchingEngine.Campaign campaign = this.ruleInput.Campaign;

            if (campaign == null || campaign.CRMap == null)
                return;

            foreach (ClientRelationshipProductRuleInput cr in input)
            {
                if(campaign.CRMap.IsInclusion && !campaign.CRMap.ClientRelationshipIds.Contains(cr.ClientRelationshipId))
                {
                    cr.BaseRuleType = BaseRuleDefinitionType.CampaignRestriction;
                    cr.RuleName = "Campaign Inclusion Rule";
                    removed.Add(cr);
                }
                else if(!campaign.CRMap.IsInclusion && campaign.CRMap.ClientRelationshipIds.Contains(cr.ClientRelationshipId))
                {
                    //TFS # 79819
                    if (!cr.ExcludeMatch1plusForFinAid && cr.ProductId == 71 && campaign.MasterProfileId == 7) // Productid = 71 (Match1Plus)  , MasterProfileId = 7 (Financial Aid exclusions)
                        continue;
                    cr.BaseRuleType = BaseRuleDefinitionType.CampaignRestriction;
                    cr.RuleName = "Campaign Exclusion Rule";
                    removed.Add(cr);
                }
            }
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " CrId(s) , out " + removed.Count + " CrId(s) ) - Campaign: " + sw.ElapsedMilliseconds.ToString() + "ms");
//#endif
        }
    }
}
