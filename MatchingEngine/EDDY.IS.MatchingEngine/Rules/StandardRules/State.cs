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
    [MatchingRuleAttributes(new EntityProcessing[] { EntityProcessing.ClientRelationship, 
                                                     EntityProcessing.ClientCampusRelationship,
                                                     EntityProcessing.ProgramProduct
                                                   },
                            new InputRequired[] { InputRequired.State }
                            , null)]
    public class State : Rule, ICRProductRule, ICRCampusProductRule, ICRProgramProductRule
    {
        public State(RuleInput ri)
            : base(ri)
        { }

        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> removed)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            removed = new List<ClientRelationshipProductRuleInput>();
            StateGeoCacheItem stateCache = StaticCacheProxyHost.CacheProxy.Get<StateGeoCacheItem>(MatchingCacheItem.REStateGeoData);

            Dictionary<int, VW_Matching_ClientRelationProductMappingCache> crList =
                StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_ClientRelationProductMappingCache>>(MatchingCacheItem.REClientRelationProductMapping);

            foreach (ClientRelationshipProductRuleInput cr in input)
            {
                VW_Matching_ClientRelationProductMappingCache crpm = null;

                if (crList.ContainsKey(cr.ClientRelationProductMappingId))
                    crpm = crList[cr.ClientRelationProductMappingId];

                if (crpm != null)
                {
                    if (crpm.IncludeAllStates.HasValue && crpm.IncludeAllStates.Value)
                        continue;

                    if (stateCache.ClientRelationProductMappingList.ContainsKey(cr.ClientRelationProductMappingId))
                    {
                        List<int> states = stateCache.ClientRelationProductMappingList[cr.ClientRelationProductMappingId];
                        if (states.Count > 0 && !states.Contains(ruleInput.prospectData.StateId.Value))
                        {
                            cr.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                            cr.RuleName = "State Inclusion Rule";
                            removed.Add(cr);
                        }
                    }
                }
            }
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " CrId(s) , out " + removed.Count + " CrId(s) ) - State: " + sw.ElapsedMilliseconds.ToString() + "ms");
//#endif
        }

        public void ExecuteRule(List<ClientRelationshipCampusProductRuleInput> input,
                             out List<ClientRelationshipCampusProductRuleInput> output)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            output = new List<ClientRelationshipCampusProductRuleInput>();
            StateGeoCacheItem stateCache = StaticCacheProxyHost.CacheProxy.Get<StateGeoCacheItem>(MatchingCacheItem.REStateGeoData);

            Dictionary<int, VW_Matching_ClientCampusProductMappingCache> crCampusList =
                StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_ClientCampusProductMappingCache>>(MatchingCacheItem.REClientCampusProductMapping);

            foreach (ClientRelationshipCampusProductRuleInput crCampus in input)
            {
                VW_Matching_ClientCampusProductMappingCache ccpm = null;

                if (crCampusList.ContainsKey(crCampus.ClientCampusProductMappingId))
                    ccpm = crCampusList[crCampus.ClientCampusProductMappingId];

                if (ccpm != null)
                {
                    if (ccpm.IncludeAllStates.HasValue && ccpm.IncludeAllStates.Value)
                        continue;

                    if (stateCache.ClientCampusProductMappingList.ContainsKey(crCampus.ClientCampusProductMappingId))
                    {
                        List<int> states = stateCache.ClientCampusProductMappingList[crCampus.ClientCampusProductMappingId];
                        if (states.Count > 0 && !states.Contains(ruleInput.prospectData.StateId.Value))
                        {
                            crCampus.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                            crCampus.RuleName = "State Inclusion Rule";
                            output.Add(crCampus);
                        }
                    }
                }
            }
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " CrCampusId(s) , out " + output.Count + " CrCampusId(s) ) - State: " + sw.ElapsedMilliseconds.ToString() + "ms");
//#endif
        }

        public void ExecuteRule(Dictionary<int, ProgramProductRuleInput> input,
                             out List<ProgramProductRuleInput> output)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            output = new List<ProgramProductRuleInput>();
            StateGeoCacheItem stateCache = StaticCacheProxyHost.CacheProxy.Get<StateGeoCacheItem>(MatchingCacheItem.REStateGeoData);

            foreach (ProgramProductRuleInput pp in input.Values)
            {
                if (stateCache.ProgramProductMappingList.ContainsKey(pp.ProgramProductId))
                {
                    List<int> states = stateCache.ProgramProductMappingList[pp.ProgramProductId];
                    if (states.Count > 0 && !states.Contains(ruleInput.prospectData.StateId.Value))
                    {
                        pp.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                        pp.RuleName = "State Inclusion Rule";
                        output.Add(pp);
                    }
                }
            }
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " ppId(s) , out " + output.Count + " ppId(s) ) - State: " + sw.ElapsedMilliseconds.ToString() + "ms");
//#endif
        }
    }
}
