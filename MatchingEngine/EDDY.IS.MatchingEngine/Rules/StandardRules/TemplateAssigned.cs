using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;

namespace EDDY.IS.MatchingEngine.Rules
{
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ProgramProduct }
                           , null
                           , null)]
    public class TemplateAssigned : Rule, ICRProgramProductRule
    {
        public TemplateAssigned(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input,
                             out List<ProgramProductRuleInput> output)
        {
            //#if DEBUG
            //            Stopwatch sw = new Stopwatch();
            //            sw.Start();
            //#endif
            MatchDatabase mdCached = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

            List<ProgramProductRuleInput> removedPrograms = new List<ProgramProductRuleInput>();

            HashSet<int> inputProgramProductIds = new HashSet<int>(input.Keys);
            inputProgramProductIds.IntersectWith(mdCached.TemplateUnassignedProgramProducts);

            foreach (int ppid in inputProgramProductIds)
            {
                ProgramProductRuleInput program = input[ppid];
                program.BaseRuleType = BaseRuleDefinitionType.TemplateAssigned;
                program.RuleName = "Template not assigned";
                removedPrograms.Add(program);
            }

            output = removedPrograms;
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " ppId(s) , out " + output.Count + " ppId(s)) - TemplateAssigned: " + sw.ElapsedMilliseconds.ToString() + "ms");
//#endif
        }
    }
}
