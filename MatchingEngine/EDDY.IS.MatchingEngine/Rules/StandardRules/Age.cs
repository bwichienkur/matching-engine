using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct }
                           ,new InputRequired[] { InputRequired.Age}
                           ,null)]
    public class Age : Rule, ICRProgramProductRule
    {
        public Age(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            //Dictionary<int, ProgramRuleData> ruleData = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, ProgramRuleData>>(MatchingCacheItem.REProgramRuleData);
            BaseRuleDefinition brMin = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData)[BaseRuleDefinitionType.MinimumAge];
            BaseRuleDefinition brMax = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData)[BaseRuleDefinitionType.MaximumAge];

            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();

            foreach (ProgramProductRuleInput program in input.Values)
            {
                bool removed = false;

                if (brMin.ProgramProductAssignments.ContainsKey(program.Key))
                {
                    List<RuleDefinition> ruleDefinitions = brMin.ProgramProductAssignments[program.Key];

                    if (ruleDefinitions.Count > 0)
                    {
                        RuleDefinition rd = ruleDefinitions[0];

                        if (ruleInput.prospectData.Age.Value < rd.EntityValue)
                        {
                            program.BaseRuleType = rd.BaseRuleType;
                            program.RuleId = rd.RuleId;
                            program.RuleName = rd.RuleName;

                            removed = true;

                            removedPrograms.Add(program);
                        }
                    }
                }
                else if (ruleInput.prospectData.Age.Value < 17)
                {
                    removed = true;

                    program.BaseRuleType = BaseRuleDefinitionType.MinimumAge;
                    program.RuleName = "Default Under 17";
                    removedPrograms.Add(program);
                }

                if (!removed && brMax.ProgramProductAssignments.ContainsKey(program.Key))
                {
                    List<RuleDefinition> ruleDefinitions = brMax.ProgramProductAssignments[program.Key];

                    if (ruleDefinitions.Count > 0)
                    {
                        RuleDefinition rd = ruleDefinitions[0];

                        if (ruleInput.prospectData.Age.Value > rd.EntityValue)
                        {
                            program.BaseRuleType = rd.BaseRuleType;
                            program.RuleId = rd.RuleId;
                            program.RuleName = rd.RuleName;

                            removedPrograms.Add(program);
                        }
                    }
                }

                removed = false;
            }

            output = removedPrograms;
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " ProgramId(s) , out " + output.Count + " ProgramId(s) ) - Minimum Age: " + sw.ElapsedMilliseconds.ToString() + "ms");
//            sw.Restart();
//#endif
        }
    }
}
