using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct }
                           , new InputRequired[] { InputRequired.IsMilitary }
                           , null)]
    public class IsMilitary : Rule, ICRProgramProductRule
    {
        public IsMilitary(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input, out List<ProgramProductRuleInput> output)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            //Dictionary<int, ProgramRuleData> ruleData = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, ProgramRuleData>>(MatchingCacheItem.REProgramRuleData);
            BaseRuleDefinition br = StaticCacheProxyHost.CacheProxy.Get<Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>>(MatchingCacheItem.RERuleDefinitionData)[BaseRuleDefinitionType.IsMilitary];

            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();

            foreach (ProgramProductRuleInput program in input.Values)
            {
                if (br.ProgramProductAssignments.ContainsKey(program.Key))
                {
                    List<RuleDefinition> ruleDefinitions = br.ProgramProductAssignments[program.Key];

                    if (ruleDefinitions.Count > 0)
                    {
                        RuleDefinition rd = ruleDefinitions[0];

                        if (rd.EntityValue == 1 && !ruleInput.prospectData.IsMilitary.Value ||
                            rd.EntityValue == 0 && ruleInput.prospectData.IsMilitary.Value)
                        {
                            program.BaseRuleType = rd.BaseRuleType;
                            program.RuleId = rd.RuleId;
                            program.RuleName = rd.RuleName;
                            
                            removedPrograms.Add(program);
                        }
                    }
                }
            }

            output = removedPrograms;
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " ProgramId(s) , out " + output.Count + " ProgramId(s) ) - Military Affiliation: " + sw.ElapsedMilliseconds.ToString() + "ms");
//            sw.Restart();
//#endif
        }
    }
}
