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
                            new InputRequired[] { InputRequired.PostalCode },
                                        null)]
    public class ZipCode : Rule, ICRProductRule, ICRCampusProductRule, ICRProgramProductRule
    {
        private ZipCodeGeoCacheItem zipCache;
        private GeoCodeProcessor geo = new GeoCodeProcessor();
        private const int USA_COUNTRY_ID = 4;
        private const int CA_COUNTRY_ID = 5;

        public ZipCode(RuleInput ri)
            : base(ri)
        {
            if(ri.ZipCoordinate != null)
                zipCache = RuleCacheProcessor.GetZipCodesByZipCodeId(ri.ZipCoordinate.ZipCodeId);
        }

        public static bool HasZipCodeGeoTargeting(VW_Matching_ClientRelationProductMappingCache crpm)
        {
            if (crpm != null && crpm.IsZipCodeInclusionExclusionActive.HasValue && crpm.IsZipCodeInclusionExclusionActive.Value)
            {
                if ((crpm.AllowableRadius.HasValue && GeoCodeProcessor.IsValidZipCode(crpm.RadiusZipCode)) ||
                     (crpm.IsZipCodeInclusion.HasValue && crpm.IsZipCodeInclusion.Value))
                    return true;
            }

            return false;
        }

        public static bool HasZipCodeGeoTargeting(VW_Matching_ClientCampusProductMappingCache ccpm)
        {
            if (ccpm != null && ccpm.IsZipCodeInclusionExclusionActive.HasValue && ccpm.IsZipCodeInclusionExclusionActive.Value)
            {
                if ((ccpm.AllowableRadius.HasValue && GeoCodeProcessor.IsValidZipCode(ccpm.RadiusZipCode)) ||
                     (ccpm.IsZipCodeInclusion.HasValue && ccpm.IsZipCodeInclusion.Value))
                    return true;
            }

            return false;
        }

        public static bool HasZipCodeGeoTargeting(ProgramProductRuleInput pp)
        {
            if (pp != null && pp.IsZipCodeInclusionExclusionActive)
            {
                if (pp.IsZipCodeInclusion || GeoCodeProcessor.IsValidZipCode(pp.RadiusZipCode))
                    return true;
            }

            return false;
        }

        public void ExecuteRule(List<ClientRelationshipProductRuleInput> input,
                                out List<ClientRelationshipProductRuleInput> removed)
        {
//#if DEBUG
//            Stopwatch sw = new Stopwatch();
//            sw.Start();
//#endif
            removed = new List<ClientRelationshipProductRuleInput>();

            if (this.ruleInput.prospectData.CountryId == null 
                || this.ruleInput.prospectData.CountryId == USA_COUNTRY_ID
                || this.ruleInput.prospectData.CountryId == CA_COUNTRY_ID)
            {
                Dictionary<int, VW_Matching_ClientRelationProductMappingCache> crList =
                    StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_ClientRelationProductMappingCache>>(MatchingCacheItem.REClientRelationProductMapping);

                foreach (ClientRelationshipProductRuleInput cr in input)
                {
                    VW_Matching_ClientRelationProductMappingCache crpm = null;

                    if (crList.ContainsKey(cr.ClientRelationProductMappingId))
                        crpm = crList[cr.ClientRelationProductMappingId];

                    if (crpm != null && crpm.IsZipCodeInclusionExclusionActive.HasValue && crpm.IsZipCodeInclusionExclusionActive.Value)
                    {
                        if (this.ruleInput.ZipCoordinate != null && crpm.AllowableRadius.HasValue && GeoCodeProcessor.IsValidZipCode(crpm.RadiusZipCode))
                        {
                            double? distance = geo.GetDistanceBetweenZipCodes(crpm.RadiusZipCode, ruleInput.ZipCoordinate.ZipCode);

                            if (!distance.HasValue || (distance.Value > crpm.AllowableRadius.Value))
                            {
                                cr.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                                cr.RuleName = "Zip Code Radius Rule - " + crpm.AllowableRadius.Value;
                                removed.Add(cr);
                            }
                        }
                        else if (crpm.IsZipCodeInclusion.HasValue && crpm.IsZipCodeInclusion.Value)
                        {
                            if (crpm.IncludeAllZipCodes.HasValue && crpm.IncludeAllZipCodes.Value)
                                continue;

                            //if(zipCache.ClientRelationProductMappingList.ContainsKey(cr.ClientRelationProductMappingId))
                            //{
                            if (zipCache == null || !zipCache.ClientRelationProductMappingList.InclusionList.Contains(cr.ClientRelationProductMappingId))
                            {
                                cr.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                                cr.RuleName = "Zip Code Inclusion Rule";
                                removed.Add(cr);
                            }
                            //}
                        }
                        else if (crpm.IsZipCodeExclusion.HasValue && crpm.IsZipCodeExclusion.Value)
                        {
                            //if(zipCache.ClientRelationProductMappingList.ContainsKey(cr.ClientRelationProductMappingId))
                            //{
                            if (zipCache != null && zipCache.ClientRelationProductMappingList.ExclusionList.Contains(cr.ClientRelationProductMappingId))
                            {
                                cr.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                                cr.RuleName = "Zip Code Exclusion Rule";
                                removed.Add(cr);
                            }
                            //}
                        }
                    }
                }
            }
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " CrId(s) , out " + removed.Count + " CrId(s) ) - ZipCode: " + sw.ElapsedMilliseconds.ToString() + "ms");
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

            if (this.ruleInput.prospectData.CountryId == null
                || this.ruleInput.prospectData.CountryId == USA_COUNTRY_ID
                || this.ruleInput.prospectData.CountryId == CA_COUNTRY_ID)
            {
                Dictionary<int, VW_Matching_ClientCampusProductMappingCache> crCampusList =
                    StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_ClientCampusProductMappingCache>>(MatchingCacheItem.REClientCampusProductMapping);

                foreach (ClientRelationshipCampusProductRuleInput crCampus in input)
                {
                    VW_Matching_ClientCampusProductMappingCache ccpm = null;

                    if (crCampusList.ContainsKey(crCampus.ClientCampusProductMappingId))
                        ccpm = crCampusList[crCampus.ClientCampusProductMappingId];

                    if (ccpm != null && ccpm.IsZipCodeInclusionExclusionActive.HasValue && ccpm.IsZipCodeInclusionExclusionActive.Value)
                    {
                        if (this.ruleInput.ZipCoordinate != null && ccpm.AllowableRadius.HasValue && GeoCodeProcessor.IsValidZipCode(ccpm.RadiusZipCode))
                        {
                            double? distance = geo.GetDistanceBetweenZipCodes(ccpm.RadiusZipCode, ruleInput.ZipCoordinate.ZipCode);

                            if (!distance.HasValue || (distance.Value > ccpm.AllowableRadius.Value))
                            {
                                crCampus.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                                crCampus.RuleName = "Zip Code Radius Rule - " + ccpm.AllowableRadius.Value;
                                output.Add(crCampus);
                            }
                        }
                        else if (ccpm.IsZipCodeInclusion.HasValue && ccpm.IsZipCodeInclusion.Value)
                        {
                            if (ccpm.IncludeAllZipCodes.HasValue && ccpm.IncludeAllZipCodes.Value)
                                continue;

                            //if (zipCache.ClientCampusProductMappingList.ContainsKey(crCampus.ClientCampusProductMappingId))
                            //{
                            if (zipCache == null || !zipCache.ClientCampusProductMappingList.InclusionList.Contains(crCampus.ClientCampusProductMappingId))
                            {
                                crCampus.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                                crCampus.RuleName = "Zip Code Inclusion Rule";
                                output.Add(crCampus);
                            }
                            //}
                        }
                        else if (ccpm.IsZipCodeExclusion.HasValue && ccpm.IsZipCodeExclusion.Value)
                        {
                            //if (zipCache.ClientCampusProductMappingList.ContainsKey(crCampus.ClientCampusProductMappingId))
                            //{
                            if (zipCache != null && zipCache.ClientCampusProductMappingList.ExclusionList.Contains(crCampus.ClientCampusProductMappingId))
                            {
                                crCampus.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                                crCampus.RuleName = "Zip Code Exclusion Rule";
                                output.Add(crCampus);
                            }
                            //}
                        }
                    }
                }
            }
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " CrCampusId(s) , out " + output.Count + " CrCampusId(s) ) - ZipCode: " + sw.ElapsedMilliseconds.ToString() + "ms");
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

            if (this.ruleInput.prospectData.CountryId == null
                || this.ruleInput.prospectData.CountryId == USA_COUNTRY_ID
                || this.ruleInput.prospectData.CountryId == CA_COUNTRY_ID)
            {
                foreach (ProgramProductRuleInput pp in input.Values)
                {
                    if (pp.IsZipCodeInclusionExclusionActive)
                    {
                        if (this.ruleInput.ZipCoordinate != null && GeoCodeProcessor.IsValidZipCode(pp.RadiusZipCode))
                        {
                            double? distance = geo.GetDistanceBetweenZipCodes(pp.RadiusZipCode, ruleInput.ZipCoordinate.ZipCode);

                            if (!distance.HasValue || (distance.Value > pp.AllowableRadius))
                            {
                                pp.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                                pp.RuleName = "Zip Code Radius Rule - " + pp.AllowableRadius;
                                output.Add(pp);
                            }
                        }
                        else if (pp.IsZipCodeInclusion)
                        {
                            if (pp.IncludeAllZipCodes)
                                continue;

                            //if (zipCache.ProgramProductMappingList.ContainsKey(pp.ProgramProductId))
                            //{
                            if (zipCache == null || !zipCache.ProgramProductMappingList.InclusionList.Contains(pp.ProgramProductId))
                            {
                                pp.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                                pp.RuleName = "Zip Code Inclusion Rule";
                                output.Add(pp);
                            }
                            //}
                        }
                        else if (pp.IsZipCodeExclusion)
                        {
                            //if (zipCache.ProgramProductMappingList.ContainsKey(pp.ProgramProductId))
                            //{
                            if (zipCache != null && zipCache.ProgramProductMappingList.ExclusionList.Contains(pp.ProgramProductId))
                            {
                                pp.BaseRuleType = BaseRuleDefinitionType.Geographic_Restriction;
                                pp.RuleName = "Zip Code Exclusion Rule";
                                output.Add(pp);
                            }
                            //}
                        }
                    }
                }
            }
//#if DEBUG
//            sw.Stop();
//            Debug.WriteLine("       ExecuteRule(in " + input.Count + " ppId(s) , out " + output.Count + " ppId(s) ) - ZipCode: " + sw.ElapsedMilliseconds.ToString() + "ms");
//#endif
        }
    }
}
