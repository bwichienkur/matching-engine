using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientRelationship }, new InputRequired[] { InputRequired.ExternalLeadId }, null)]
    public class JournayaLeadId : Rule, ICRProductRule
    {
        public JournayaLeadId(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input, out List<ClientRelationshipProductRuleInput> removed)
        {
            removed = new List<ClientRelationshipProductRuleInput>();
            
            foreach (ClientRelationshipProductRuleInput crProduct in input)
            {
                if (crProduct.RequireJournayaLeadId)
                {
                    Guid parseResult;
                    if (String.IsNullOrWhiteSpace(ruleInput.prospectData.ExternalLeadId) || !Guid.TryParse(ruleInput.prospectData.ExternalLeadId, out parseResult))
                    {
                        crProduct.BaseRuleType = BaseRuleDefinitionType.Custom_KVLookup;
                        crProduct.RuleName = "Journaya LeadId Required";
                        removed.Add(crProduct);
                    }
                }
            }
        }
    }
}
