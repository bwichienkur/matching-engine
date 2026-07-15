using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct }
                          , new InputRequired[] { InputRequired.USCitizen }
                          , null)]
    public class USCitizen : Rule, ICRProgramProductRule
    {
        public USCitizen(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
        {
            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();

            Dictionary<BaseRuleDefinitionType, BaseRuleDefinition> ruleDefs = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData);

            BaseRuleDefinition br;

            if (ruleDefs.TryGetValue(BaseRuleDefinitionType.USCitizen, out br))
            {
                foreach (ProgramProductRuleInput program in input.Values)
                {
                    if (br.ProgramProductAssignments.ContainsKey(program.Key))
                    {
                        List<RuleDefinition> ruleDefinitions = br.ProgramProductAssignments[program.Key];

                        if (ruleDefinitions.Count > 0)
                        {
                            RuleDefinition rd = ruleDefinitions[0];

                            if (rd.EntityValue == 1 && !ruleInput.prospectData.IsCitizen.Value ||
                                rd.EntityValue == 0 && ruleInput.prospectData.IsCitizen.Value)
                            {
                                program.BaseRuleType = rd.BaseRuleType;
                                program.RuleId = rd.RuleId;
                                program.RuleName = rd.RuleName;

                                removedPrograms.Add(program);
                            }
                        }
                    }
                }
            }

            output = removedPrograms;
        }
    }
}
