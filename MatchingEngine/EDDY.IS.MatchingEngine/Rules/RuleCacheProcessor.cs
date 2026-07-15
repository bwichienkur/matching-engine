using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.Base.Util;
using System.Configuration;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.Core.Logging;

namespace EDDY.IS.MatchingEngine
{
    public class ZipCodeInclusionExclusion
    {
        public string ZipCode { get; set; }
        public bool IsInclusion { get; set; }

        public override bool Equals(object obj)
        {
            ZipCodeInclusionExclusion zipCode = obj as ZipCodeInclusionExclusion;
            return zipCode != null && zipCode.ZipCode == this.ZipCode;
        }

        public override int GetHashCode()
        {
            return this.ZipCode.GetHashCode();// ^ this.IsInclusion.GetHashCode();
        }
    }

    public class InstitutionGroupCache
    {
        public Dictionary<int, List<Tuple<int, bool>>> InstitutionToGroups { get; set; }
        public Dictionary<int, List<int>> GroupToInstitutions { get; set; }
        public HashSet<int> GroupsThatAllowMultipleSelections { get; set; }

        public InstitutionGroupCache()
        {
            InstitutionToGroups = new Dictionary<int, List<Tuple<int, bool>>>();
            GroupToInstitutions = new Dictionary<int, List<int>>();
            GroupsThatAllowMultipleSelections = new HashSet<int>();
        }
    }

    public class RuleCacheProcessor
    {
        internal static Dictionary<int, List<VW_Matching_ClientRelationshipSchedule>> GetClientRelationshipSchedules()
        {
            return RuleDataService.GetClientRelationshipSchedules();
        }

        internal static Dictionary<int, List<VW_Matching_ClientRelationshipEntitySchedule>> GetClientRelationshipEntitySchedules()
        {
            return RuleDataService.GetClientRelationshipEntitySchedules();
        }

        public static Dictionary<int, List<ZipCodeInclusionExclusion>> GetCampusZipCodeInclusionExclusion()
        {
            //    List<VW_Matching_CampusZipCode> dbZipCodes = RuleDataService.GetAllCampusZipCodes();

            //    var groupedByCampusMapping = dbZipCodes.GroupBy(z => z.ClientCampusProductMappingId);
            //    Dictionary<int, List<ZipCodeInclusionExclusion>> zipCodes = new Dictionary<int, List<ZipCodeInclusionExclusion>>(groupedByCampusMapping.Count());

            //    foreach (var zipGroup in groupedByCampusMapping)
            //    {
            //        List<ZipCodeInclusionExclusion> includeList = new List<ZipCodeInclusionExclusion>(zipGroup.Count());

            //        foreach (var zipItem in zipGroup)
            //        {
            //            ZipCodeInclusionExclusion includeItem = new ZipCodeInclusionExclusion();

            //            includeItem.ZipCode = zipItem.ZipCode;
            //            includeItem.IsInclusion = zipItem.IsZipCodeInclusion.HasValue ? zipItem.IsZipCodeInclusion.Value : false;

            //            includeList.Add(includeItem);
            //        }

            //        zipCodes.Add(zipGroup.First().ClientCampusProductMappingId, includeList);
            //    }

            //    return zipCodes;
            return new Dictionary<int, List<ZipCodeInclusionExclusion>>();
        }
        
        public static Dictionary<int, LeadScoreReservation> GetLeadScoreReservations()
        {
            Dictionary<int, LeadScoreReservation> reservations = new Dictionary<int, LeadScoreReservation>();

            List<VW_Matching_LeadScoreReservationConfiguration> leadScoreReservationsConfigs = LeadScoreReservationDataService.GetLeadScoreReservationConfigurations();
            List<VW_Matching_LeadScoreReservationTierLevel> leadScoreReservationTierLevels = LeadScoreReservationDataService.GetLeadScoreReservationTierLevels();

            foreach (VW_Matching_LeadScoreReservationConfiguration config in leadScoreReservationsConfigs)
            {
                LeadScoreReservation reservation;

                if (reservations.TryGetValue(config.LeadScoreReservationId, out reservation))
                {
                    reservation.MarketingItems.Add(CreateMarketingItemFromConfig(config));
                }
                else
                {
                    reservation = new LeadScoreReservation();
                    reservation.DayBegin = config.DayBegin;
                    reservation.DayEnd = config.DayEnd;
                    reservation.EndDate = config.EndDate;
                    reservation.StartDate = config.StartDate;
                    reservation.ReservationName = config.ReservationName;
                    reservation.MarketingItems.Add(CreateMarketingItemFromConfig(config));

                    reservations.Add(config.LeadScoreReservationId, reservation);
                }
            }

            foreach (VW_Matching_LeadScoreReservationTierLevel config in leadScoreReservationTierLevels)
            {
                LeadScoreReservation reservation;

                if (reservations.TryGetValue(config.LeadScoreReservationId, out reservation))
                {
                    if (reservation.SupplyUnitToAcceptableTierLevel.ContainsKey(config.LeadScoreReservationSupplyUnitId))
                        reservation.SupplyUnitToAcceptableTierLevel[config.LeadScoreReservationSupplyUnitId] = config.LeadScoringTierLevel;
                    else
                        reservation.SupplyUnitToAcceptableTierLevel.Add(config.LeadScoreReservationSupplyUnitId, config.LeadScoringTierLevel);
                }
            }


            return reservations;
        }

        public static Dictionary<int, LeadScoreReservationSupplyUnit> GetLeadScoreReservationSupplyUnits()
        {
            Dictionary<int, LeadScoreReservationSupplyUnit> supplyUnits = new Dictionary<int, LeadScoreReservationSupplyUnit>();

            List<VW_Matching_LeadScoreReservationSupplyUnitDefinition> items = LeadScoreReservationDataService.GetScoreReservationSupplyUnitDefinition();

            foreach (var i in items)
            {
                LeadScoreReservationSupplyUnit su;

                if (supplyUnits.TryGetValue(i.LeadScoreReservationSupplyUnitId, out su))
                {
                    su.SupplyUnitItems.Add(new SupplyUnitItem()
                    {
                        CategoryId = i.CategoryId,
                        ProgramLevelId = i.ProgramLevelId,
                        SpecialtyId = i.SpecialtyId,
                        SubjectId = i.SubjectId
                    });

                    if (i.ExcludedClientRelationshipId.HasValue)
                        su.ExcludedCrs.Add(i.ExcludedClientRelationshipId.Value);
                }
                else
                {
                    su = new LeadScoreReservationSupplyUnit();
                    su.LeadScoreReservationSupplyUnitId = i.LeadScoreReservationSupplyUnitId;
                    su.SupplyUnitName = i.SupplyUnitName;
                    su.SupplyUnitItems.Add(new SupplyUnitItem()
                    {
                        CategoryId = i.CategoryId,
                        ProgramLevelId = i.ProgramLevelId,
                        SpecialtyId = i.SpecialtyId,
                        SubjectId = i.SubjectId
                    });

                    if (i.ExcludedClientRelationshipId.HasValue)
                        su.ExcludedCrs.Add(i.ExcludedClientRelationshipId.Value);

                    supplyUnits.Add(su.LeadScoreReservationSupplyUnitId, su);
                }
            }

            return supplyUnits;
        }
        private static LeadScoreReservationMarketingItem CreateMarketingItemFromConfig(VW_Matching_LeadScoreReservationConfiguration config)
        {
            LeadScoreReservationMarketingItem mi = new LeadScoreReservationMarketingItem();
            mi.CampaignId = config.CampaignId;
            mi.ChannelId = config.ChannelId;
            mi.MarketingUnitId = config.MarketingUnitId;
            mi.VendorId = config.VendorId;
            mi.SubchannelId = config.SubChannelId;
            mi.ApplicationId = config.ApplicationId;

            return mi;
        }

       

        internal static InstitutionGroupCache GetInstitutionGroupCache()
        {
            InstitutionGroupCache instGroupCache = new InstitutionGroupCache();

            List<VW_Matching_InstitutionGroup> instGroupList = RuleDataService.GetAllInstitutionGroupProd();

            foreach (VW_Matching_InstitutionGroup instGroup in instGroupList)
            {
                if (instGroupCache.InstitutionToGroups.ContainsKey(instGroup.InstitutionId))
                    instGroupCache.InstitutionToGroups[instGroup.InstitutionId].Add(new Tuple<int, bool>(instGroup.InstitutionGroupId, instGroup.RemoveInSchoolSelection));
                else
                    instGroupCache.InstitutionToGroups.Add(instGroup.InstitutionId, new List<Tuple<int, bool>>() { new Tuple<int, bool>(instGroup.InstitutionGroupId, instGroup.RemoveInSchoolSelection) });

                if (instGroupCache.GroupToInstitutions.ContainsKey(instGroup.InstitutionGroupId))
                    instGroupCache.GroupToInstitutions[instGroup.InstitutionGroupId].Add(instGroup.InstitutionId);
                else
                    instGroupCache.GroupToInstitutions.Add(instGroup.InstitutionGroupId, new List<int>() { instGroup.InstitutionId });

                if (instGroup.AllowMultiSelectForInstitutionGroup)
                    instGroupCache.GroupsThatAllowMultipleSelections.Add(instGroup.InstitutionGroupId);
            }

            return instGroupCache;
        }


        //internal static TemplateCacheItem GetProgramProductTemplateAssignment()
        //{
        //    TemplateCacheItem templateCache = new TemplateCacheItem();

        //    List<VW_Matching_ProgramProductTemplateAssignment> ppAssignmentList = RuleDataService.GetAllProgramProductTemplateAssignmentProd();

        //    foreach (VW_Matching_ProgramProductTemplateAssignment ppTemplateAssignment in ppAssignmentList)
        //    {
        //        if (templateCache.ProgramProductToTemplateAssignments.ContainsKey(ppTemplateAssignment.programproductid))
        //            templateCache.ProgramProductToTemplateAssignments[ppTemplateAssignment.programproductid] = ppTemplateAssignment.TemplateId;
        //        else
        //        { 
        //            if(!templateCache.ProgramProductToTemplateAssignments.ContainsKey(ppTemplateAssignment.programproductid))
        //                templateCache.ProgramProductToTemplateAssignments.Add(ppTemplateAssignment.programproductid, ppTemplateAssignment.TemplateId);
        //            else
        //            {
        //                try
        //                {
        //                    Exception ex = new Exception(String.Format("GetProgramProductTemplateAssignment duplicate key programproductid = {0}", ppTemplateAssignment.programproductid));
        //                    ISException isEx = new ISException(ex);
        //                    isEx.Save();
        //                }
        //                catch { }
        //            }
        //        }
        //    }

        //    return templateCache;
        //}


        internal static Dictionary<int, List<KeyValuePair<int, VW_Matching_CRCallCenterHours>>> GetCallCenterHoursCache()
        {
            Dictionary<int, List<KeyValuePair<int, VW_Matching_CRCallCenterHours>>> callCenterHours = new Dictionary<int, List<KeyValuePair<int, VW_Matching_CRCallCenterHours>>>();

            List<VW_Matching_CRCallCenterHours> cchList = RuleDataService.GetAllCRCallCenterHours();

            foreach (VW_Matching_CRCallCenterHours ccHour in cchList)
            {
                if (!callCenterHours.ContainsKey(ccHour.ClientRelationshipId))
                    callCenterHours.Add(ccHour.ClientRelationshipId, new List<KeyValuePair<int, VW_Matching_CRCallCenterHours>>());

                KeyValuePair<int, VW_Matching_CRCallCenterHours> tempKeyValue = new KeyValuePair<int, VW_Matching_CRCallCenterHours>(ccHour.Day, ccHour);

                callCenterHours[ccHour.ClientRelationshipId].Add(tempKeyValue);
            }

            return callCenterHours;
        }

        internal static Dictionary<int, List<KeyValuePair<int, VW_Matching_CampusCallCenterHours>>> GetCampusCallCenterHoursCache()
        {

            Dictionary<int, List<KeyValuePair<int, VW_Matching_CampusCallCenterHours>>> callCenterHours = new Dictionary<int, List<KeyValuePair<int, VW_Matching_CampusCallCenterHours>>>();

            List<VW_Matching_CampusCallCenterHours> cchList = RuleDataService.GetAllCampusCallCenterHours();

            foreach (VW_Matching_CampusCallCenterHours ccHour in cchList)
            {
                if (!callCenterHours.ContainsKey(ccHour.ClientCampusRelationshipId))
                    callCenterHours.Add(ccHour.ClientCampusRelationshipId, new List<KeyValuePair<int, VW_Matching_CampusCallCenterHours>>());

                KeyValuePair<int, VW_Matching_CampusCallCenterHours> tempKeyValue = new KeyValuePair<int, VW_Matching_CampusCallCenterHours>(ccHour.Day, ccHour);

                callCenterHours[ccHour.ClientCampusRelationshipId].Add(tempKeyValue);
            }

            return callCenterHours;
        }

        internal static Dictionary<int, VW_Matching_WarmTransferInfo> GetWarmTransferInfoCache()
        {
            Dictionary<int, VW_Matching_WarmTransferInfo> warmTransferInfo = new Dictionary<int, VW_Matching_WarmTransferInfo>();

            List<VW_Matching_WarmTransferInfo> wtList = RuleDataService.GetAllWarmTransferInfo();

            foreach (VW_Matching_WarmTransferInfo wt in wtList)
            {
                if (!warmTransferInfo.ContainsKey(wt.ClientRelationshipId))
                    warmTransferInfo.Add(wt.ClientRelationshipId, wt);
                else
                {
                    try
                    {
                        Exception ex = new Exception(String.Format("GetWarmTransferInfoCache duplicate key ClientRelationshipId = {0}", wt.ClientRelationshipId));
                        ISException isEx = new ISException(ex);
                        isEx.Save();
                    }
                    catch { }
                }
            }

            return warmTransferInfo;
        }

        internal static Dictionary<int, VW_Matching_KVCodeDataCache> GetKVCodeDataCache()
        {
            Dictionary<int, VW_Matching_KVCodeDataCache> kvCodes = new Dictionary<int, VW_Matching_KVCodeDataCache>();

            List<VW_Matching_KVCodeDataCache> kvCodeList = RuleDataService.GetAllKVCodeDataCache();

            foreach (VW_Matching_KVCodeDataCache kv in kvCodeList)
            {
                if (!kvCodes.ContainsKey(kv.KVCodeDataId))
                    kvCodes.Add(kv.KVCodeDataId, kv);
                else
                {
                    try
                    {
                        Exception ex = new Exception(String.Format("GetKVCodeDataCache duplicate key KVCodeDataId = {0}", kv.KVCodeDataId));
                        ISException isEx = new ISException(ex);
                        isEx.Save();
                    }
                    catch { }
                }
            }

            return kvCodes;
        }

        internal static Dictionary<BaseRuleDefinitionType, BaseRuleDefinition> GetRuleDefinitionCache()
        {
            Dictionary<BaseRuleDefinitionType, BaseRuleDefinition> rules = new Dictionary<BaseRuleDefinitionType, BaseRuleDefinition>();

            List<VW_Matching_ProgramValidationRuleCache> ruleList = RuleDataService.GetAllProgramValidationRuleCache();

            var baseRules = ruleList.GroupBy(b => b.pvbrid);

            foreach (var baseRule in baseRules)
            {
                BaseRuleDefinition br = new BaseRuleDefinition();

                br.RuleType = (BaseRuleDefinitionType)baseRule.First().pvbrid;

                var rulesInBaseRule = baseRule.GroupBy(r => r.pvrid);
                foreach (var rule in rulesInBaseRule)
                {
                    RuleDefinition rd = new RuleDefinition();

                    VW_Matching_ProgramValidationRuleCache ruleDefinition = rule.First();
                    rd.BaseRuleType = br.RuleType;
                    rd.EntityValue = ruleDefinition.EntityValue;
                    rd.RuleId = ruleDefinition.pvrid;
                    rd.RuleName = ruleDefinition.RuleName;
                    rd.StandardControlCode = ruleDefinition.Code;
                    rd.IsStatic = ruleDefinition.IsStatic;
                    rd.IsLowerBound = ruleDefinition.IsLowerBound;
                    rd.IsUpperBound = ruleDefinition.IsUpperBound;
                    rd.UtilizeAcademicYear = ruleDefinition.UtilizeAcademicYear;
                    rd.KeyValueCodeIds.AddRange((from r in rule where r.kvcodedataid.HasValue select r.kvcodedataid.Value).Distinct().ToList());

                    br.Rules.Add(rd);
                    foreach (var program in rule)
                    {
                        if (program.programid.HasValue && program.productid.HasValue)
                        {
                            RuleDefinition programRuleDefinition = new RuleDefinition();
                            programRuleDefinition.BaseRuleType = rd.BaseRuleType;
                            programRuleDefinition.RuleId = rd.RuleId;
                            programRuleDefinition.RuleName = rd.RuleName;
                            programRuleDefinition.StandardControlCode = rd.StandardControlCode;
                            programRuleDefinition.IsStatic = rd.IsStatic;
                            programRuleDefinition.IsLowerBound = rd.IsLowerBound;
                            programRuleDefinition.IsUpperBound = rd.IsUpperBound;
                            programRuleDefinition.UtilizeAcademicYear = rd.UtilizeAcademicYear;
                            programRuleDefinition.KeyValueCodeIds.AddRange(rd.KeyValueCodeIds);
                            programRuleDefinition.EntityValue = program.EntityValue;

                            //string key = program.programid.Value + "_" + program.productid.Value;
                            if (br.ProgramProductAssignments.ContainsKey(program.programproductid))
                                br.ProgramProductAssignments[program.programproductid].Add(programRuleDefinition);
                            else
                                br.ProgramProductAssignments.Add(program.programproductid, new List<RuleDefinition>() { programRuleDefinition });

                            //string key = program.programid.Value + "_" + program.productid.Value;
                            //if (br.ProgramProductAssignments.ContainsKey(key))
                            //{
                            //    //if (rd.BaseRuleType == BaseRuleDefinitionType.HSGradYear)
                            //    //    programRuleDefinition.EntityValue = Convert.ToDecimal(evalue);
                            //    //else
                            //    //    programRuleDefinition.EntityValue = rd.EntityValue;
                            //    br.ProgramProductAssignments[key].Add(programRuleDefinition);
                            //}
                            //else
                            //{
                            //    //if (rd.BaseRuleType == BaseRuleDefinitionType.HSGradYear)
                            //    //    programRuleDefinition.EntityValue = Convert.ToDecimal(evalue);
                            //    //else
                            //    //    programRuleDefinition.EntityValue = rd.EntityValue;

                            //    br.ProgramProductAssignments.Add(key, new List<RuleDefinition>() { programRuleDefinition });
                            //}

                            programRuleDefinition = null;
                        }
                    }

                }
                if (!rules.ContainsKey(br.RuleType))
                    rules.Add(br.RuleType, br);
                else
                {
                    try
                    {
                        Exception ex = new Exception(String.Format("GetRuleDefinitionCache duplicate key RuleType = {0}", br.RuleType));
                        ISException isEx = new ISException(ex);
                        isEx.Save();
                    }
                    catch { }
                }
            }

            return rules;
        }

        internal static CountryGeoCacheItem GetCountryGeoCacheItem()
        {
            CountryGeoCacheItem countryGeoCache = new CountryGeoCacheItem();

            List<VW_Matching_CountryCache> countryList = RuleDataService.GetAllCountryCacheProd();

            var entityTypes = countryList.GroupBy(c => c.EntityType);

            foreach (var entityType in entityTypes)
            {
                var entities = entityType.GroupBy(d => d.EntityId);

                foreach (var item in entities)
                {
                    var firstCountry = item.First();

                    AddToCountryGeo(ref countryGeoCache, firstCountry.EntityType, firstCountry.EntityId, firstCountry.CountryId);

                    foreach (var country in item)
                        AddToCountryGeo(ref countryGeoCache, country.EntityType, country.EntityId, country.CountryId);
                }
            }

            return countryGeoCache;
        }

        private static void AddToCountryGeo(ref CountryGeoCacheItem countryGeoCache, string entityType, int entityId, int countryId)
        {
            if (entityType == "PP")
            {
                if (countryGeoCache.ProgramProductMappingList.ContainsKey(entityId))
                    countryGeoCache.ProgramProductMappingList[entityId].Add(countryId);
                else
                    countryGeoCache.ProgramProductMappingList.Add(entityId, new List<int> { countryId });
            }
            else if (entityType == "CM")
            {
                if (countryGeoCache.ClientCampusProductMappingList.ContainsKey(entityId))
                    countryGeoCache.ClientCampusProductMappingList[entityId].Add(countryId);
                else
                    countryGeoCache.ClientCampusProductMappingList.Add(entityId, new List<int> { countryId });
            }
            else
            {
                if (countryGeoCache.ClientRelationProductMappingList.ContainsKey(entityId))
                    countryGeoCache.ClientRelationProductMappingList[entityId].Add(countryId);
                else
                    countryGeoCache.ClientRelationProductMappingList.Add(entityId, new List<int> { countryId });
            }
        }

        internal static StateGeoCacheItem GetStateGeoCacheItem()
        {
            StateGeoCacheItem stateGeoCache = new StateGeoCacheItem();

            List<VW_Matching_StateCache> stateList = RuleDataService.GetAllStateCacheProd();

            var entityTypes = stateList.GroupBy(c => c.EntityType);

            foreach (var entityType in entityTypes)
            {
                var entities = entityType.GroupBy(d => d.EntityId);

                foreach (var item in entities)
                {
                    var firstState = item.First();

                    AddToStateGeo(ref stateGeoCache, firstState.EntityType, firstState.EntityId, firstState.StateId);

                    foreach (var state in item)
                        AddToStateGeo(ref stateGeoCache, state.EntityType, state.EntityId, state.StateId);
                }
            }

            return stateGeoCache;
        }

        private static void AddToStateGeo(ref StateGeoCacheItem stateGeoCache, string entityType, int entityId, int stateId)
        {
            if (entityType == "PP")
            {
                if (stateGeoCache.ProgramProductMappingList.ContainsKey(entityId))
                    stateGeoCache.ProgramProductMappingList[entityId].Add(stateId);
                else
                    stateGeoCache.ProgramProductMappingList.Add(entityId, new List<int> { stateId });
            }
            else if (entityType == "CM")
            {
                if (stateGeoCache.ClientCampusProductMappingList.ContainsKey(entityId))
                    stateGeoCache.ClientCampusProductMappingList[entityId].Add(stateId);
                else
                    stateGeoCache.ClientCampusProductMappingList.Add(entityId, new List<int> { stateId });
            }
            else
            {
                if (stateGeoCache.ClientRelationProductMappingList.ContainsKey(entityId))
                    stateGeoCache.ClientRelationProductMappingList[entityId].Add(stateId);
                else
                    stateGeoCache.ClientRelationProductMappingList.Add(entityId, new List<int> { stateId });
            }
        }

        internal static ZipCodeGeoCacheItem GetZipCodesByZipCodeId(int zipCodeId)
        {
            ZipCodeGeoCacheItem zipInfo = (ZipCodeGeoCacheItem)StaticCacheProxyHost.CacheProxy.Get("ME_REZipCodeGeoData_" + zipCodeId);

            if (zipInfo == null)
            {
                List<VW_Matching_ZipCodeCache> zipCodeList = RuleDataService.GetZipCodesByZipCodeId(zipCodeId);

                if (zipCodeList != null && zipCodeList.Count > 0)
                {
                    zipInfo = new ZipCodeGeoCacheItem();

                    var entityTypes = zipCodeList.GroupBy(c => c.EntityType);

                    foreach (var entityType in entityTypes)
                    {
                        var exclusionType = entityType.GroupBy(e => e.ListType);

                        foreach (var exclusion in exclusionType)
                        {
                            var entities = exclusion.GroupBy(d => d.EntityId);

                            foreach (var item in entities)
                            {
                                //var firstZipCode = item.First();

                                //bool isExclusion = firstZipCode.ListType == "E" ? true : false;
                                //AddToZipCodeGeo(ref zipCodeGeoCache, firstZipCode.EntityType, firstZipCode.EntityId, firstZipCode.ZipCodeId.Value, isExclusion);

                                foreach (var zipCode in item)
                                {
                                    bool isExclusion = zipCode.ListType == "E" ? true : false;

                                    AddToZipCodeGeo(ref zipInfo, zipCode.EntityType, zipCode.EntityId, zipCode.ZipCodeId, isExclusion);
                                }
                            }
                        }
                    }

                    StaticCacheProxyHost.CacheProxy.Set("ME_REZipCodeGeoData_" + zipCodeId, zipInfo, Convert.ToInt32(ConfigurationManager.AppSettings["ME_REZipCodeGeoData_ExpirationMinutes"]));
                }
            }

            return zipInfo;
        }

        #region Private Zip Code Cache Methods
        //private static ZipCodeGeoCacheItem GetZipCodesByZipCodeIdProd(int zipCodeId)
        //{
        //    Service.Service matchingService = new Service.Service();

        //    ZipCodeGeoCacheItem zipInfo = null;

        //    List<VW_Matching_ZipCodeCacheDTO> zipCodeList = matchingService.VW_Matching_ZipCodeCacheService.GetZipCodesByZipCodeId(zipCodeId);

        //    if (zipCodeList != null && zipCodeList.Count > 0)
        //    {
        //        zipInfo = new ZipCodeGeoCacheItem();

        //    var entityTypes = zipCodeList.GroupBy(c => c.EntityType);

        //    foreach (var entityType in entityTypes)
        //    {
        //        var exclusionType = entityType.GroupBy(e => e.ListType);

        //        foreach (var exclusion in exclusionType)
        //        {
        //            var entities = exclusion.GroupBy(d => d.EntityId);

        //            foreach (var item in entities)
        //            {
        //                //var firstZipCode = item.First();

        //                //bool isExclusion = firstZipCode.ListType == "E" ? true : false;
        //                //AddToZipCodeGeo(ref zipCodeGeoCache, firstZipCode.EntityType, firstZipCode.EntityId, firstZipCode.ZipCodeId.Value, isExclusion);

        //                foreach (var zipCode in item)
        //                {
        //                    bool isExclusion = zipCode.ListType == "E" ? true : false;

        //                    AddToZipCodeGeo(ref zipInfo, zipCode.EntityType, zipCode.EntityId, zipCode.ZipCodeId.Value, isExclusion);
        //                }
        //            }
        //        }
        //    }
        //    }

        //    return zipInfo;
        //}

        //private static ZipCodeGeoCacheItem GetZipCodesByZipCodeIdBeta(int zipCodeId)
        //{
        //    Service.Service matchingService = new Service.Service();

        //    ZipCodeGeoCacheItem zipInfo = null;

        //    List<VW_Matching_ZipCodeCacheDTO> zipCodeList = matchingService.VW_Matching_ZipCodeCacheService.GetZipCodesByZipCodeId(zipCodeId);

        //    if (zipCodeList != null && zipCodeList.Count > 0)
        //    {
        //        zipInfo = new ZipCodeGeoCacheItem();

        //    var entityTypes = zipCodeList.GroupBy(c => c.EntityType);

        //    foreach (var entityType in entityTypes)
        //    {
        //        var exclusionType = entityType.GroupBy(e => e.ListType);

        //        foreach (var exclusion in exclusionType)
        //        {
        //            var entities = exclusion.GroupBy(d => d.EntityId);

        //            foreach (var item in entities)
        //            {
        //                //var firstZipCode = item.First();

        //                //bool isExclusion = firstZipCode.ListType == "E" ? true : false;
        //                //AddToZipCodeGeo(ref zipCodeGeoCache, firstZipCode.EntityType, firstZipCode.EntityId, firstZipCode.ZipCodeId.Value, isExclusion);

        //                foreach (var zipCode in item)
        //                {
        //                    bool isExclusion = zipCode.ListType == "E" ? true : false;

        //                    AddToZipCodeGeo(ref zipInfo, zipCode.EntityType, zipCode.EntityId, zipCode.ZipCodeId.Value, isExclusion);
        //                }
        //            }
        //        }
        //    }
        //    }

        //    return zipInfo;
        //}

        private static void AddToZipCodeGeo(ref ZipCodeGeoCacheItem zipCodeGeoCache, string entityType, int entityId, int zipCodeId, bool isExclusion)
        {
            if (entityType == "PP")
            {
                //if (zipCodeGeoCache.ProgramProductMappingList.ContainsKey(entityId))
                //{
                if (isExclusion)
                    zipCodeGeoCache.ProgramProductMappingList.ExclusionList.Add(entityId);
                else
                    zipCodeGeoCache.ProgramProductMappingList.InclusionList.Add(entityId);
                //}
                //else
                //{
                //    ZipCodeCacheItem cacheItem = new ZipCodeCacheItem();

                //    if (isExclusion)
                //        cacheItem.ExclusionList.Add(zipCodeId);
                //    else
                //        cacheItem.InclusionList.Add(zipCodeId);

                //    zipCodeGeoCache.ProgramProductMappingList.Add(entityId, cacheItem);
                //}
            }
            else if (entityType == "CM")
            {
                //if (zipCodeGeoCache.ClientCampusProductMappingList.ContainsKey(entityId))
                //{
                if (isExclusion)
                    zipCodeGeoCache.ClientCampusProductMappingList.ExclusionList.Add(entityId);
                else
                    zipCodeGeoCache.ClientCampusProductMappingList.InclusionList.Add(entityId);
                //}
                //else
                //{
                //    ZipCodeCacheItem cacheItem = new ZipCodeCacheItem();

                //    if (isExclusion)
                //        cacheItem.ExclusionList.Add(zipCodeId);
                //    else
                //        cacheItem.InclusionList.Add(zipCodeId);

                //    zipCodeGeoCache.ClientCampusProductMappingList.Add(entityId, cacheItem);
                //}
            }
            else
            {
                //if (zipCodeGeoCache.ClientRelationProductMappingList.ContainsKey(entityId))
                //{
                if (isExclusion)
                    zipCodeGeoCache.ClientRelationProductMappingList.ExclusionList.Add(entityId);
                else
                    zipCodeGeoCache.ClientRelationProductMappingList.InclusionList.Add(entityId);
                //}
                //else
                //{
                //    ZipCodeCacheItem cacheItem = new ZipCodeCacheItem();

                //    if (isExclusion)
                //        cacheItem.ExclusionList.Add(zipCodeId);
                //    else
                //        cacheItem.InclusionList.Add(zipCodeId);

                //    zipCodeGeoCache.ClientRelationProductMappingList.Add(entityId, cacheItem);
                //}
            }
        }
        #endregion


        internal static Dictionary<int, VW_Matching_ClientRelationProductMappingCache> GetClientRelationProductMappingRecords()
        {
            Dictionary<int, VW_Matching_ClientRelationProductMappingCache> mapping = new Dictionary<int, VW_Matching_ClientRelationProductMappingCache>();

            List<VW_Matching_ClientRelationProductMappingCache> mappingList = RuleDataService.GetAllClientRelationProductMappingCacheProd();

            foreach (VW_Matching_ClientRelationProductMappingCache item in mappingList)
            {
                if (!mapping.ContainsKey(item.ClientRelationProductMappingId))
                    mapping.Add(item.ClientRelationProductMappingId, item);
                else
                {
                    try
                    {
                        Exception ex = new Exception(String.Format("GetClientRelationProductMappingRecords duplicate key ClientRelationProductMappingId = {0}", item.ClientRelationProductMappingId));
                        ISException isEx = new ISException(ex);
                        isEx.Save();
                    }
                    catch { }
                }

            }

            return mapping;
        }

        internal static Dictionary<int, VW_Matching_ClientCampusProductMappingCache> GetClientCampusProductMappingRecords()
        {
            Dictionary<int, VW_Matching_ClientCampusProductMappingCache> mapping = new Dictionary<int, VW_Matching_ClientCampusProductMappingCache>();

            List<VW_Matching_ClientCampusProductMappingCache> mappingList = RuleDataService.GetAllClientCampusProductMappingCacheProd();

            foreach (VW_Matching_ClientCampusProductMappingCache item in mappingList)
            {
                if (!mapping.ContainsKey(item.ClientCampusProductMappingId))
                    mapping.Add(item.ClientCampusProductMappingId, item);
                else
                {
                    try
                    {
                        Exception ex = new Exception(String.Format("GetClientCampusProductMappingRecords duplicate key ClientCampusProductMappingId = {0}", item.ClientCampusProductMappingId));
                        ISException isEx = new ISException(ex);
                        isEx.Save();
                    }
                    catch { }
                }
            }

            return mapping;
        }

        internal static Dictionary<int, HashSet<int>> GetProgramToEdLevelMapping()
        {
            Dictionary<int, HashSet<int>> mapping = new Dictionary<int, HashSet<int>>();

            List<VW_Matching_ProgramToEdLevelMapping> mappingList = RuleDataService.GetAllProgramToEdLevelMappingProd();

            foreach (VW_Matching_ProgramToEdLevelMapping map in mappingList)
            {
                if (mapping.ContainsKey(map.ProgramProductId))
                    mapping[map.ProgramProductId].Add(map.EducationLevelId);
                else
                    mapping.Add(map.ProgramProductId, new HashSet<int> { map.EducationLevelId });

            }

            return mapping;
        }

        internal static Dictionary<int, HashSet<int>> GetProgramLevelToEdLevelMapping()
        {
            Dictionary<int, HashSet<int>> mapping = new Dictionary<int, HashSet<int>>();

            List<VW_Matching_ProgramLevelToEdLevelMapping> mappingList = RuleDataService.GetAllProgramLevelToEdLevelMapping();

            foreach (VW_Matching_ProgramLevelToEdLevelMapping map in mappingList)
            {
                if (mapping.ContainsKey(map.ProgramLevelId))
                    mapping[map.ProgramLevelId].Add(map.EducationLevelId);
                else
                {
                    HashSet<int> edLevels = new HashSet<int>();
                    edLevels.Add(map.EducationLevelId);
                    mapping.Add(map.ProgramLevelId, edLevels);
                }

            }
            return mapping;
        }

        //        internal static Dictionary<int, ProgramRuleData> GetProgramRuleData()
        //        {
        //            Dictionary<int, ProgramRuleData> ruleData = new Dictionary<int, ProgramRuleData>();
        //#if DEBUG
        //            Stopwatch sw = new Stopwatch();
        //            sw.Start();
        //#endif
        //            Service.Service matchingService = new Service.Service();
        //            List<VWActiveProgramRuleDataDTO> ruleDataList = matchingService.VWActiveProgramRuleDataService.GetAll();

        //            foreach (VWActiveProgramRuleDataDTO program in ruleDataList)
        //            {
        //                ruleData.Add(program.ProgramId,
        //                                new ProgramRuleData()
        //                                {
        //                                    MaxHSGradYear = program.MaxHSGradYear,
        //                                    MinimumAge = program.MinimumAge,
        //                                    MinimumGPA = program.MinimumGPA,
        //                                    OnlyMilitary = program.OnlyMilitary,
        //                                    RequiresUSCitizen = program.RequiresUSCitizen,
        //                                    YearsExperience = program.YearsExperience
        //                                });

        //            }

        //#if DEBUG
        //            sw.Stop();
        //            Debug.WriteLine("GetProgramRuleData(): " + sw.ElapsedMilliseconds.ToString() + "ms");
        //#endif

        //            return ruleData;
        //        }



        internal static RulesEngineFactory REFactory
        {
            get
            {
                RulesEngineFactory rf = StaticCacheProxyHost.CacheProxy.Get<RulesEngineFactory>(MatchingCacheItem.RERuleFactory);

                if (rf == null)
                {
                    StaticCacheProxyHost.CacheProxy.RemoveItem(MatchingCacheItem.RERuleFactory);
                    rf = StaticCacheProxyHost.CacheProxy.Get<RulesEngineFactory>(MatchingCacheItem.RERuleFactory);
                }

                return rf;
            }
        }

        internal static RulesEngineFactory LoadRulesEngineFactory()
        {
            RulesEngineFactory rf = new RulesEngineFactory();
            return rf;
        }

        //private Dictionary<string, Cap> _hierarchicalCapList;

        //private Dictionary<string, Cap> HierarchicalCapList
        //{
        //    get
        //    {
        //        if (_hierarchicalCapList == null)
        //            _hierarchicalCapList = StaticCacheProxyHost.CacheProxy.Get<Dictionary<string, Cap>>(MatchingCacheItem.RECaps);

        //        return _hierarchicalCapList;
        //    }
        //}
        internal static Dictionary<int, List<AgentDisallowedLiveTransfer>> GetAgentDisallowedLiveTransferCRs()
        {
            Dictionary<int, List<AgentDisallowedLiveTransfer>> agentDisallowedLiveTransferCRs = new Dictionary<int, List<AgentDisallowedLiveTransfer>>();

            List<VW_Matching_ClientRelationAgentDisallowedLiveTransfer> agentDisallowedList = RuleDataService.GetAgentDisallowedList();

            if (agentDisallowedList != null)
            {
                foreach (var a in agentDisallowedList)
                {
                    if (agentDisallowedLiveTransferCRs.ContainsKey(a.UserId))
                        agentDisallowedLiveTransferCRs[a.UserId].Add(new AgentDisallowedLiveTransfer(a.ClientRelationshipId, a.DisallowLiveTransfer, a.DisallowForm));
                    else
                        agentDisallowedLiveTransferCRs.Add(a.UserId, new List<AgentDisallowedLiveTransfer> {
                            new AgentDisallowedLiveTransfer(a.ClientRelationshipId, a.DisallowLiveTransfer, a.DisallowForm)
                        });
                }
            }
            return agentDisallowedLiveTransferCRs;
        }

        internal static Dictionary<int, HashSet<int>> GetCapNormalizationOverrides()
        {
            Dictionary<int, HashSet<int>> capNormalizationOverrides = new Dictionary<int, HashSet<int>>();

            List<VW_Matching_CapDistribution_NormalizationOverride> dbOverrides = RuleDataService.GetAllCapNormalizationOverrides();

            foreach (var o in dbOverrides)
            {
                if (capNormalizationOverrides.ContainsKey(o.CapDistributionId))
                    capNormalizationOverrides[o.CapDistributionId].Add(o.MarketingUnitId);
                else
                    capNormalizationOverrides.Add(o.CapDistributionId, new HashSet<int> { o.MarketingUnitId });
            }

            return capNormalizationOverrides;
        }

        internal static Dictionary<int, Cap> CreateHierarchicalCapList()
        {
            Dictionary<int, Cap> hierarchicalCapList = new Dictionary<int, Cap>();

            if (StaticSettings.IsBeta)
                hierarchicalCapList = CreateHierarchicalCapListBeta();
            else
                hierarchicalCapList = CreateHierarchicalCapListProd();


            return hierarchicalCapList;
        }

        internal static Dictionary<int, Dictionary<int, VW_Matching_ClientRelationshipChannelCaps>> GetClientRelationshipChannelCaps()
        {
            Dictionary<int, Dictionary<int, VW_Matching_ClientRelationshipChannelCaps>> crChannelCaps = new Dictionary<int, Dictionary<int, VW_Matching_ClientRelationshipChannelCaps>>();

            List<VW_Matching_ClientRelationshipChannelCaps> channelCaps = RuleDataService.GetClientRelationshipChannelCaps();

            if (channelCaps != null)
            {
                foreach (var cap in channelCaps)
                {
                    if (crChannelCaps.ContainsKey(cap.ClientRelationshipId))
                    {
                        if (crChannelCaps[cap.ClientRelationshipId].ContainsKey(cap.ChannelId))
                            crChannelCaps[cap.ClientRelationshipId][cap.ChannelId] = cap;
                        else
                            crChannelCaps[cap.ClientRelationshipId].Add(cap.ChannelId, cap);
                    }
                    else
                    {
                        var d = new Dictionary<int, VW_Matching_ClientRelationshipChannelCaps>();
                        d.Add(cap.ChannelId, cap);

                        crChannelCaps.Add(cap.ClientRelationshipId, d);
                    }
                }
            }

            return crChannelCaps;
        }

        internal static Dictionary<int, Cap> CreateHierarchicalCampaignCapList()
        {
            Dictionary<int, Cap> hierarchicalCapList = new Dictionary<int, Cap>();

            if (StaticSettings.IsBeta)
                hierarchicalCapList = CreateHierarchicalCampaignCapListBeta();
            else
                hierarchicalCapList = CreateHierarchicalCampaignCapListProd();


            return hierarchicalCapList;
        }

        #region Private Cap Cache Methods
        private static Dictionary<int, Cap> CreateHierarchicalCapListProd()
        {
            Dictionary<int, Cap> hierarchicalCapList = new Dictionary<int, Cap>();

            List<VW_Matching_CapHierarchy> databaseCapHierarchy = new List<VW_Matching_CapHierarchy>(RuleDataService.GetAllCapHierarchyProd().OrderBy(cd => cd.CapDurationId).ThenBy(l => l.Level).ThenBy(capd => capd.CapDistributionId));
            int key = 0;
            int capDistributionID = 0;

            foreach (VW_Matching_CapHierarchy capDTO in databaseCapHierarchy)
            {
                if (capDistributionID == capDTO.CapDistributionId)
                    hierarchicalCapList[capDTO.ClientRelationProductMappingId].AddEntityToCap(capDTO.CapDistributionId, capDTO.EntityId);
                else
                {
                    Cap c = new Cap(capDTO.CapTransactionCount, capDTO.TotalCapAmount);

                    c.ParentCapDistributionId = capDTO.ParentCapDistributionId;
                    c.ClientRelationProductMappingId = capDTO.ClientRelationProductMappingId;
                    c.CapDistributionId = capDTO.CapDistributionId;
                    c.Capped = !capDTO.IsOn;
                    c.CapType = (EntityMeta)capDTO.EntityMetaId;
                    //c.EntityId = capDTO.EntityId;
                    c.Level = capDTO.Level;
                    c.LimitType = (CapLimitType)capDTO.CapLimitTypeId;
                    c.CapRoom = Convert.ToInt32(capDTO.CapRoom);
                    c.IsCurrentlyFree = capDTO.IsCurrentlyFree;
                    c.ProductId = capDTO.ProductId;
                    c.Type = (CapType)capDTO.CapTypeId;
                    c.SFProductCode = (SFProductCode?)capDTO.SFProductCodeId;
                    //c.TotalCapAmount = Convert.ToInt32(capDTO.TotalCapAmount);
                    //c.TransactionCount = Convert.ToInt32(capDTO.CapTransactionCount);
                    c.EntityIDSet.Add(capDTO.EntityId);
                    //if ((capDTO.ClientRelationshipId == 22 || capDTO.ClientRelationshipId == 23) && (capDTO.ProductId == 2 || capDTO.ProductId == 9))
                    //    c.IsCecoGoldOrSelect = true;

                    if (capDTO.CapReasonCodeId.HasValue)
                        c.Reason = (CapReasonCode)capDTO.CapReasonCodeId;

                    if (capDTO.ClientRelationProductMappingId != key)
                        hierarchicalCapList.Add(capDTO.ClientRelationProductMappingId, c);
                    else
                        hierarchicalCapList[capDTO.ClientRelationProductMappingId].AddCap(c, capDTO.ParentCapDistributionId);

                    c.TreatAsMatch1 = capDTO.TreatAsMatch1;
                }

                key = capDTO.ClientRelationProductMappingId;
                capDistributionID = capDTO.CapDistributionId;
            }

            return hierarchicalCapList;
        }

        private static Dictionary<int, Cap> CreateHierarchicalCapListBeta()
        {
            Dictionary<int, Cap> hierarchicalCapList = new Dictionary<int, Cap>();

            List<VW_Matching_CapHierarchy> databaseCapHierarchy = new List<VW_Matching_CapHierarchy>(RuleDataService.GetAllCapHierarchyBeta().OrderBy(cd => cd.CapDurationId).ThenBy(l => l.Level).ThenBy(capd => capd.CapDistributionId));
            int key = 0;
            int capDistributionID = 0;

            foreach (VW_Matching_CapHierarchy capDTO in databaseCapHierarchy)
            {
                if (capDistributionID == capDTO.CapDistributionId)
                {
                    hierarchicalCapList[capDTO.ClientRelationProductMappingId].AddEntityToCap(capDTO.CapDistributionId, capDTO.EntityId);
                }
                else
                {
                    Cap c = new Cap(capDTO.CapTransactionCount, capDTO.TotalCapAmount);
                    c.CapDistributionId = capDTO.CapDistributionId;
                    c.Capped = !capDTO.IsOn;
                    c.CapType = (EntityMeta)capDTO.EntityMetaId;
                    //c.EntityId = capDTO.EntityId;
                    c.Level = capDTO.Level;
                    c.LimitType = (CapLimitType)capDTO.CapLimitTypeId;
                    c.CapRoom = Convert.ToInt32(capDTO.CapRoom);
                    c.IsCurrentlyFree = capDTO.IsCurrentlyFree;
                    c.ProductId = capDTO.ProductId;
                    c.Type = (CapType)capDTO.CapTypeId;
                    c.SFProductCode = (SFProductCode?)capDTO.SFProductCodeId;
                    //c.TotalCapAmount = Convert.ToInt32(capDTO.TotalCapAmount);
                    //c.TransactionCount = Convert.ToInt32(capDTO.CapTransactionCount);

                    if (capDTO.CapReasonCodeId.HasValue)
                        c.Reason = (CapReasonCode)capDTO.CapReasonCodeId;

                    if (capDTO.ClientRelationProductMappingId != key)
                        hierarchicalCapList.Add(capDTO.ClientRelationProductMappingId, c);
                    else
                        hierarchicalCapList[capDTO.ClientRelationProductMappingId].AddCap(c, capDTO.ParentCapDistributionId);

                }

                key = capDTO.ClientRelationProductMappingId;
                capDistributionID = capDTO.CapDistributionId;
            }


            return hierarchicalCapList;
        }

        private static Dictionary<int, Cap> CreateHierarchicalCampaignCapListProd()
        {
            Dictionary<int, Cap> hierarchicalCapList = new Dictionary<int, Cap>();

            List<VW_Matching_CampaignCapHierarchy> databaseCapHierarchy = new List<VW_Matching_CampaignCapHierarchy>(RuleDataService.GetAllCampaignCapHierarchyProd());

            List<VW_Matching_CampaignCapHierarchy> parentCampaignCaps = databaseCapHierarchy.Where(c => c.ParentCapDistributionId == 0).ToList();

            foreach (VW_Matching_CampaignCapHierarchy capDTO in parentCampaignCaps)
            {
                //if our list doesnt have this campaign cap lets add it.
                if (!hierarchicalCapList.ContainsKey(capDTO.EntityId))
                {
                    Cap c = new Cap(capDTO.CapTransactionCount, capDTO.TotalCapAmount);

                    c.ParentCapDistributionId = capDTO.ParentCapDistributionId;
                    c.CapDistributionId = capDTO.CapDistributionId;
                    c.Capped = !capDTO.IsOn;
                    c.CapType = (EntityMeta)capDTO.EntityMetaId;
                    c.Level = capDTO.Level;
                    c.LimitType = (CapLimitType)capDTO.CapLimitTypeId;
                    c.CapRoom = Convert.ToInt32(capDTO.CapRoom);
                    c.IsCurrentlyFree = capDTO.IsCurrentlyFree;
                    c.Type = (CapType)capDTO.CapTypeId;
                    c.EntityIDSet.Add(capDTO.EntityId);
                    c.TreatAsMatch1 = capDTO.TreatAsMatch1;

                    c.Children = databaseCapHierarchy.Where(p => p.ParentCapDistributionId == c.CapDistributionId)
                                .Select(p => new Cap(p.CapTransactionCount, p.TotalCapAmount)
                                {
                                    ParentCapDistributionId = p.ParentCapDistributionId,
                                    CapDistributionId = p.CapDistributionId,
                                    Capped = !p.IsOn,
                                    CapType = (EntityMeta)p.EntityMetaId,
                                    Level = p.Level,
                                    LimitType = (CapLimitType)p.CapLimitTypeId,
                                    CapRoom = Convert.ToInt32(p.CapRoom),
                                    IsCurrentlyFree = p.IsCurrentlyFree,
                                    Type = (CapType)p.CapTypeId,
                                    EntityIDSet = new HashSet<int>() { p.EntityId },
                                    TreatAsMatch1 = p.TreatAsMatch1
                                })
                                .ToList();
                    //campaign id, its hierarchy of caps
                    hierarchicalCapList.Add(capDTO.EntityId, c);
                }
            }

            return hierarchicalCapList;
        }

        private static Dictionary<int, Cap> CreateHierarchicalCampaignCapListBeta()
        {
            Dictionary<int, Cap> hierarchicalCapList = new Dictionary<int, Cap>();

            List<VW_Matching_CapHierarchy> databaseCapHierarchy = new List<VW_Matching_CapHierarchy>(RuleDataService.GetAllCapHierarchyBeta().OrderBy(cd => cd.CapDurationId).ThenBy(l => l.Level).ThenBy(capd => capd.CapDistributionId));
            int key = 0;
            int capDistributionID = 0;

            foreach (VW_Matching_CapHierarchy capDTO in databaseCapHierarchy)
            {
                if (capDistributionID == capDTO.CapDistributionId)
                {
                    hierarchicalCapList[capDTO.ClientRelationProductMappingId].AddEntityToCap(capDTO.CapDistributionId, capDTO.EntityId);
                }
                else
                {
                    Cap c = new Cap(capDTO.CapTransactionCount, capDTO.TotalCapAmount);
                    c.CapDistributionId = capDTO.CapDistributionId;
                    c.Capped = !capDTO.IsOn;
                    c.CapType = (EntityMeta)capDTO.EntityMetaId;
                    //c.EntityId = capDTO.EntityId;
                    c.Level = capDTO.Level;
                    c.LimitType = (CapLimitType)capDTO.CapLimitTypeId;
                    c.CapRoom = Convert.ToInt32(capDTO.CapRoom);
                    c.IsCurrentlyFree = capDTO.IsCurrentlyFree;
                    c.ProductId = capDTO.ProductId;
                    c.Type = (CapType)capDTO.CapTypeId;
                    c.SFProductCode = (SFProductCode?)capDTO.SFProductCodeId;
                    //c.TotalCapAmount = Convert.ToInt32(capDTO.TotalCapAmount);
                    //c.TransactionCount = Convert.ToInt32(capDTO.CapTransactionCount);

                    if (capDTO.CapReasonCodeId.HasValue)
                        c.Reason = (CapReasonCode)capDTO.CapReasonCodeId;

                    if (capDTO.ClientRelationProductMappingId != key)
                        hierarchicalCapList.Add(capDTO.ClientRelationProductMappingId, c);
                    else
                        hierarchicalCapList[capDTO.ClientRelationProductMappingId].AddCap(c, capDTO.ParentCapDistributionId);

                }

                key = capDTO.ClientRelationProductMappingId;
                capDistributionID = capDTO.CapDistributionId;
            }


            return hierarchicalCapList;
        }
        #endregion
    }
}
