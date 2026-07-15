using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct }
                           , new InputRequired[] { InputRequired.KVCode }
                           , null)]
    public class Custom_KVLookup : Rule, ICRProgramProductRule
    {
        public Custom_KVLookup(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            //Dictionary<int, ProgramRuleData> ruleData = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, ProgramRuleData>>(MatchingCacheItem.REProgramRuleData);
            BaseRuleDefinition br = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData)[BaseRuleDefinitionType.Custom_KVLookup];

            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();

            foreach (ProgramProductRuleInput program in input.Values)
            {
                if (br.ProgramProductAssignments.ContainsKey(program.Key))
                {
                    List<RuleDefinition> ruleDefinitions = br.ProgramProductAssignments[program.Key];

                    foreach (RuleDefinition rd in ruleDefinitions)
                    {
                        foreach (KeyValuePair<string, int> kvCode in ruleInput.prospectData.KVCodeData)
                        {
                            if (kvCode.Key.ToLower() == rd.StandardControlCode.ToLower() &&
                                !rd.KeyValueCodeIds.Contains(kvCode.Value))
                            {
                                program.BaseRuleType = BaseRuleDefinitionType.Custom_KVLookup;
                                program.RuleId = rd.RuleId;
                                program.RuleName = rd.RuleName;
                                program.StandardControlCode = rd.StandardControlCode;

                                removedPrograms.Add(program);
                                break;

                            }
                        }

                        if (program.RuleId.HasValue)
                            break;
                    }

                }
            }

            output = removedPrograms;
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " ProgramId(s) , out " + output.Count + " ProgramId(s) ) - Custom KVLookup: " + sw.ElapsedMilliseconds.ToString() + "ms");
//            sw.Restart();
//#endif
        }
    }
}
