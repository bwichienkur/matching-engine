using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct }
                           , new InputRequired[] { InputRequired.MilitaryStatus }
                           , null)]
    public class MilitaryNonCivilian : Rule, ICRProgramProductRule
    {
        private static readonly List<int> allowedMilitaryStatusList = new List<int>() { 101, 102, 103, 104, 106, 107, 108, 109, 111, 112, 113, 114, 116, 117, 118, 119, 121, 122, 123, 124 };

        public MilitaryNonCivilian(RuleInput ri)
            : base(ri)
        { }
        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
        {
            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();

            Dictionary<BaseRuleDefinitionType,BaseRuleDefinition> ruleDict = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData);
            BaseRuleDefinition ruleDef = null;

            if (ruleDict.TryGetValue(BaseRuleDefinitionType.MilitaryNonCivilian, out ruleDef))
            {
                foreach (ProgramProductRuleInput program in input.Values)
                {
                    List<RuleDefinition> ruleDefinitions = null;

                    if (ruleDef.ProgramProductAssignments.TryGetValue(program.Key, out ruleDefinitions))
                    {
                       if (ruleInput.prospectData.MilitaryStatusId.HasValue)
                        {
                            if (!allowedMilitaryStatusList.Contains(ruleInput.prospectData.MilitaryStatusId.Value))
                            {
                                program.BaseRuleType = ruleDefinitions[0].BaseRuleType;
                                program.RuleId = ruleDefinitions[0].RuleId;
                                program.RuleName = ruleDefinitions[0].RuleName;
                                program.StandardControlCode = ruleDefinitions[0].StandardControlCode;

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
