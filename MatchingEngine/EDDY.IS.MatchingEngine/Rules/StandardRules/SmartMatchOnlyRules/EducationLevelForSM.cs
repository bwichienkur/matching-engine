using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.Constants;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using System.Configuration;
using EDDY.IS.MatchingEngine;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.Program }
                            , new InputRequired[] { InputRequired.EducationLevel }
                            , new RuleAttribute[] { RuleAttribute.ExecuteSmartMatchRules })]
    public class ProgramLevelBasedOnEducationLevelForSM : Rule, IProgramRule
    {
        public ProgramLevelBasedOnEducationLevelForSM(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(List<ProgramRuleInput> input,
                                out List<ProgramRuleInput> removed)
        {
            removed = new List<ProgramRuleInput>();

            if (ruleInput.DesiredProgramLevelList != null && ruleInput.DesiredProgramLevelList.Any())
                return;
            else
            {
                foreach (ProgramRuleInput p in input)
                {
                    if (ruleInput.prospectData.EducationLevelId.Value == (int)Enums.EducationLevel.Bachelor &&
                         (p.ProgramLevelId == (int)Enums.ProgramLevel.Associate || p.ProgramLevelId == (int)Enums.ProgramLevel.Bachelor))
                    {
                        p.BaseRuleType = BaseRuleDefinitionType.InvalidProgramLevelForSM;
                        p.RuleName = "Invalid Program Level for SM";
                        removed.Add(p);
                    }
                    else if (ruleInput.prospectData.EducationLevelId.Value == (int)Enums.EducationLevel.Associate &&
                         p.ProgramLevelId == (int)Enums.ProgramLevel.Associate)
                    {
                        p.BaseRuleType = BaseRuleDefinitionType.InvalidProgramLevelForSM;
                        p.RuleName = "Invalid Program Level for SM";
                        removed.Add(p);
                    }
                    else if ((ruleInput.prospectData.EducationLevelId.Value == (int)Enums.EducationLevel.Master || ruleInput.prospectData.EducationLevelId.Value == (int)Enums.EducationLevel.Doctorate) &&
                         (p.ProgramLevelId == (int)EDDY.IS.MatchingEngine.Enums.ProgramLevel.Associate || p.ProgramLevelId == (int)EDDY.IS.MatchingEngine.Enums.ProgramLevel.Bachelor))
                    {
                        p.BaseRuleType = BaseRuleDefinitionType.InvalidProgramLevelForSM;
                        p.RuleName = "Invalid Program Level for SM";
                        removed.Add(p);
                    }
                }
            }
        }
    }
}
