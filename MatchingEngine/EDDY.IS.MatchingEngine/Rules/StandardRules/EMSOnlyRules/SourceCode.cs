using EDDY.IS.MatchingEngine;
using EDDY.IS.MatchingEngine.Rules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.Program }
                                                    , null
                                                    , new RuleAttribute[] { RuleAttribute.ExecuteEMSRules, RuleAttribute.ExecuteLeadSubmitRules }
                           )]
    public class SourceCode : Rule, IProgramRule
    {
        public SourceCode(RuleInput ri) : base(ri)
        {
        }


        public void ExecuteRule(List<ProgramRuleInput> input, out List<ProgramRuleInput> output)
        {
            var campaign = this.ruleInput.Campaign;
            var removedPrograms = new List<ProgramRuleInput>();
            int? channelId = campaign?.ChannelId;

            if (campaign.MediaPlanType != null && campaign.MediaPlanType == Enums.MediaPlanType.LeadManagement)
            {
                if (IsMissingSourceCode(this.ruleInput.prospectData))
                {
                    foreach (ProgramRuleInput program in input)
                    {
                        program.BaseRuleType = BaseRuleDefinitionType.SourceCodeMissing;
                        program.RuleId = (int)BaseRuleDefinitionType.SourceCodeMissing;
                        program.RuleName = "SourceCode Missing";

                        removedPrograms.Add(program);
                    }
                }
                else if (ruleInput.prospectData != null && ruleInput.prospectData.IsValid && campaign.SourceCode.ToLower() != ruleInput.prospectData.SourceCode.ToLower())
                {
                    foreach (ProgramRuleInput program in input)
                    {
                        program.BaseRuleType = BaseRuleDefinitionType.SourceCodeMissing;
                        program.RuleId = (int)BaseRuleDefinitionType.SourceCodeMissing;
                        program.RuleName = "SourceCode is Invalid";

                        removedPrograms.Add(program);
                    }
                }
            }


            output = removedPrograms;
        }

        private bool IsMissingSourceCode(Prospect prospectInput)
        {
            bool result = false;

            if (prospectInput != null && prospectInput.IsValid)
            {
                result = string.IsNullOrEmpty(prospectInput.SourceCode);
            }

            return result;
        }
    }
}
