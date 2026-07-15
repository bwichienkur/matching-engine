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
                            new InputRequired[] { InputRequired.Country }
                            , null)]
    public class Country : Rule, ICRProductRule, ICRCampusProductRule, ICRProgramProductRule
    {
        public Country(RuleInput ri) : base(ri)
        { }

        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> removed)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            removed = new List<ClientRelationshipProductRuleInput>();
            CountryGeoCacheItem countryCache = StaticCacheProxyHost.CacheProxy.Get<CountryGeoCacheItem>(MatchingCacheItem.RECountryGeoData);
            
            Dictionary<int, VW_Matching_ClientRelationProductMappingCache> crList =
                StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_ClientRelationProductMappingCache>>(MatchingCacheItem.REClientRelationProductMapping);

            foreach (ClientRelationshipProductRuleInput cr in input)
            {
                VW_Matching_ClientRelationProductMappingCache crpm = null;

                if (crList.ContainsKey(cr.ClientRelationProductMappingId))
                    crpm = crList[cr.ClientRelationProductMappingId];

                if (crpm != null)
                {
                    if (crpm.IncludeAllCountries.HasValue && crpm.IncludeAllCountries.Value)
                        continue;

                    if (countryCache.ClientRelationProductMappingList.ContainsKey(cr.ClientRelationProductMappingId))
                    {
                        List<int> countries = countryCache.ClientRelationProductMappingList[cr.ClientRelationProductMappingId];
                        if (countries.Count > 0 && !countries.Contains(ruleInput.prospectData.CountryId.Value))
                        {
                            cr.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                            cr.RuleName = "Country Inclusion Rule";
                            removed.Add(cr);
                        }
                    }
                }
            }
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " CrId(s) , out " + removed.Count + " CrId(s) ) - Country: " + sw.ElapsedMilliseconds.ToString() + "ms");
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
            CountryGeoCacheItem countryCache = StaticCacheProxyHost.CacheProxy.Get<CountryGeoCacheItem>(MatchingCacheItem.RECountryGeoData);

            Dictionary<int, VW_Matching_ClientCampusProductMappingCache> crCampusList =
                StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_ClientCampusProductMappingCache>>(MatchingCacheItem.REClientCampusProductMapping);

            foreach (ClientRelationshipCampusProductRuleInput crCampus in input)
            {
                VW_Matching_ClientCampusProductMappingCache ccpm = null;

                if (crCampusList.ContainsKey(crCampus.ClientCampusProductMappingId))
                    ccpm = crCampusList[crCampus.ClientCampusProductMappingId];

                if (ccpm != null)
                {
                    if (ccpm.IncludeAllCountries.HasValue && ccpm.IncludeAllCountries.Value)
                        continue;

                    if (countryCache.ClientCampusProductMappingList.ContainsKey(crCampus.ClientCampusProductMappingId))
                    {
                        List<int> countries = countryCache.ClientCampusProductMappingList[crCampus.ClientCampusProductMappingId];
                        if (countries.Count > 0 && !countries.Contains(ruleInput.prospectData.CountryId.Value))
                        {
                            crCampus.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                            crCampus.RuleName = "Country Inclusion Rule";
                            output.Add(crCampus);
                        }
                    }
                }
            }
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " CrCampusId(s) , out " + output.Count + " CrCampusId(s) ) - Country: " + sw.ElapsedMilliseconds.ToString() + "ms");
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
            CountryGeoCacheItem countryCache = StaticCacheProxyHost.CacheProxy.Get<CountryGeoCacheItem>(MatchingCacheItem.RECountryGeoData);

            foreach (ProgramProductRuleInput pp in input.Values)
            {
                if (countryCache.ProgramProductMappingList.ContainsKey(pp.ProgramProductId))
                {
                    List<int> countries = countryCache.ProgramProductMappingList[pp.ProgramProductId];
                    if (countries.Count > 0 && !countries.Contains(ruleInput.prospectData.CountryId.Value))
                    {
                        pp.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                        pp.RuleName = "Country Inclusion Rule";
                        output.Add(pp);
                    }
                }
            }
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " ppId(s) , out " + output.Count + " ppId(s) ) - Country: " + sw.ElapsedMilliseconds.ToString() + "ms");
//#endif
        }
    }
}
