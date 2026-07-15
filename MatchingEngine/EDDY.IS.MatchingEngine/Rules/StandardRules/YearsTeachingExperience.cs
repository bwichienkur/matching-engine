using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DataModel.Entity;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct }
                           , new InputRequired[] { InputRequired.YearsTeachingExperience }
                           , null)]
    public class YearsTeachingExperience : Rule, ICRProgramProductRule
    {
        public YearsTeachingExperience(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            BaseRuleDefinition br = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData)[BaseRuleDefinitionType.YearsTeachingExperience];
            VW_Matching_KVCodeDataCache kvCodeData = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_KVCodeDataCache>>(MatchingCacheItem.REKVCodeData)[ruleInput.prospectData.YearsTeachingExperienceKeyValueId.Value];

            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();

            foreach (ProgramProductRuleInput program in input.Values)
            {
                if (br.ProgramProductAssignments.ContainsKey(program.Key))
                {
                    List<RuleDefinition> ruleDefinitions = br.ProgramProductAssignments[program.Key];

                    if (ruleDefinitions.Count > 0)
                    {
                        RuleDefinition rd = ruleDefinitions[0];

                        if (!kvCodeData.StartRange.HasValue || kvCodeData.StartRange < rd.EntityValue)
                        {
                            program.BaseRuleType = rd.BaseRuleType;
                            program.RuleId = rd.RuleId;
                            program.RuleName = rd.RuleName;
                            program.StandardControlCode = rd.StandardControlCode;

                            removedPrograms.Add(program);
                        }
                    }
                }
            }

            output = removedPrograms;
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " ProgramId(s) , out " + output.Count + " ProgramId(s) ) - Years Teaching Experience: " + sw.ElapsedMilliseconds.ToString() + "ms");
//            sw.Restart();
//#endif
        }
    }
}
