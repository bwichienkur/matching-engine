using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct }
                           , new InputRequired[] { InputRequired.EducationLevel }
                           , null)]
    public class EducationLevel : Rule, ICRProgramProductRule
    {
        public EducationLevel(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(Dictionary<int,ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            Dictionary<int, HashSet<int>> programToEdLevelMapping = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, HashSet<int>>>(MatchingCacheItem.REProgramToEdLevelMapping);
            Dictionary<int, HashSet<int>> programLevelToEdLevelMapping = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, HashSet<int>>>(MatchingCacheItem.REProgramLevelToEdLevelMapping);

            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();
            int edLevel = ruleInput.prospectData.EducationLevelId.Value;

            foreach (ProgramProductRuleInput pp in input.Values)
            {
                if (programToEdLevelMapping.ContainsKey(pp.ProgramProductId))
                {
                    if (!programToEdLevelMapping[pp.ProgramProductId].Contains(edLevel))
                    {
                        pp.BaseRuleType = BaseRuleDefinitionType.Minimum_Education_Level;
                        pp.RuleName = "Program Minimum Education Level Restriction";

                        removedPrograms.Add(pp);
                    }
                }
                else if (programLevelToEdLevelMapping.ContainsKey(pp.ProgramLevelId))
                {
                    if (!programLevelToEdLevelMapping[pp.ProgramLevelId].Contains(edLevel))
                    {                        
                        pp.BaseRuleType = BaseRuleDefinitionType.Minimum_Education_Level;
                        pp.RuleName = "General Mapping Minimum Education Level Restriction";

                        removedPrograms.Add(pp);
                    }
                }
            }

            output = removedPrograms;
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " ProgramId(s) , out " + output.Count + " ProgramId(s) ) - Education Level: " + sw.ElapsedMilliseconds.ToString() + "ms");
//            sw.Restart();
//#endif
        }
    }
}
