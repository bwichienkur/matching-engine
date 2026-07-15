using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.MatchingEngine.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientRelationship }
                                                    , new InputRequired[] { InputRequired.Email }
                                                    , new RuleAttribute[] { RuleAttribute.ExecuteEMSRules }
                           )]
    public class LenexaAgencyDuplicateByEmail : Rule, ICRProductRule
    {
        //private static readonly HashSet<int> emsDupeSchools = new HashSet<int> { 9070, 9071, 9073, 9122, 9121, 9120, 9123, 9168, 9167, 9166 };
        public LenexaAgencyDuplicateByEmail(RuleInput ri)
            : base(ri)
        { 
            
        }

        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> removed)
        {
            removed = new List<ClientRelationshipProductRuleInput>();

            Dictionary<int, List<VW_Matching_EMSDuplicateInfo>> dupes = RuleDataService.GetDuplicateLeadsByClientRelationship(ruleInput.prospectData.Email);

            foreach (ClientRelationshipProductRuleInput cr in input)
            {
                //if (ruleInput.Campaign.InstitutionAgencyType.HasValue && ruleInput.Campaign.InstitutionAgencyType.Value == InstitutionAgencyType.Lenexa)
                //{
                    if (dupes.ContainsKey(cr.ClientRelationshipId))
                    {
                        foreach (var dupe in dupes[cr.ClientRelationshipId])
                        {
                            int lookbackPeriod = this.ruleInput.Campaign.CampaignDuplicateLookback.HasValue ? ruleInput.Campaign.CampaignDuplicateLookback.Value : dupe.CrDuplicateLookback;

                            if (lookbackPeriod > 0 && dupe.DaysElapsed <= lookbackPeriod)
                            {
                                cr.BaseRuleType = BaseRuleDefinitionType.InternalDuplicate;
                                cr.RuleId = (int)BaseRuleDefinitionType.InternalDuplicate;
                                cr.RuleName = "Internal Duplicate";

                                removed.Add(cr);

                                break;
                            }
                        }
                    }
                //}
            }
        }
    }
}
