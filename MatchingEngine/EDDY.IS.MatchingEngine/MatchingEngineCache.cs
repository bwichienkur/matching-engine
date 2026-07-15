using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Caching;
using EDDY.IS.Core.Logging;
using EDDY.IS.LocalCache;
using EDDY.IS.MatchingEngine.DataModel;

namespace EDDY.IS.MatchingEngine
{
    public enum MatchingCacheItem
    {
        SRADefinition = 1,
        SRAeRPL = 2,
        SRAStrategic = 3,
        ZipCodeCoordinates = 4,
        RECaps = 5,
        RERuleFactory = 6,
        REProgramToEdLevelMapping = 7,
        REProgramLevelToEdLevelMapping = 8,
        ProgramContent = 9,
        InstitutionContent = 10,
        CampusContent = 11,
        REClientRelationProductMapping = 12,
        REClientCampusProductMapping = 13,
        RECountryGeoData = 14,
        REStateGeoData = 15,
        RERuleDefinitionData = 16,
        ProgramLevelContent = 17,
        SubjectContent = 18,
        CategoryContent = 19,
        REKVCodeData = 20,
        //REProgramProductTemplateAssignment = 21,
        CrossSellMapping = 22,
        Features = 23,
        MatchDatabase = 24,
        InstitutionGroup = 25,
        CampusZipCode = 26,
        RECallCenterHours = 27,
        REWarmTransferInfo = 28,
        SpecialtyContent = 29,
        RECampusCallCenterHours = 30,
        CampusNumbersContent = 31,
        LeadScoreReservationConfigurations = 32,
        LeadScoreReservationSupplyUnits = 33,
        LeadScoringProducts = 34,
        //ClickProgramProducts = 35,
        ProgramDisplayGroups = 36,
        Campaigns = 37,
        Products = 38,
        Applications = 39,
        ChannelGroups = 40,
        Images = 41,
        ProgramAddresses = 42,
        CapMUOverrides = 43,
        SABPSIeRPC = 44,
        CampaignRestrictedTemplates = 45,
        ClientRelationshipSchedules = 46,
        ClientRelationshipEntitySchedules = 47,
        InstitutionStartDate = 48,
        ProgramStartDate = 49,
        StatePricing = 50,
        AgentDisallowedList = 51,
        ClientRelationContacts = 52,
        ClientCampusContacts = 53,
        CampusOptionGroups = 54,
        MatchResponseSearchVendorAllowed = 55,
        CampaignCaps = 56,
        LeadPingLeadScores = 57,
        LeadPingLeadScoreCPLs = 58,
        ClientRelationshipChannelCaps = 59,
        ProgramTag = 60
    }

    public class CachePopulation
    {
        public Func<object> PopulationMethod { get; set; }
        public bool HoldPrePopulation { get; set; }

        public CachePopulation(Func<object> populationMethod, bool holdPrePopulation)
        {
            PopulationMethod = populationMethod;
            HoldPrePopulation = holdPrePopulation;
        }
    }

    public class MatchingEngineCache : LocalCacheBase
    {
        private const string itemKeyPrefix = "ME_";

        private Dictionary<MatchingCacheItem, CachePopulation> populationMethods;

        public MatchingEngineCache()
        {
            populationMethods = new Dictionary<MatchingCacheItem, CachePopulation>();

            populationMethods.Add(MatchingCacheItem.Campaigns, new CachePopulation(Campaign.GetCampaignCache, true));
            populationMethods.Add(MatchingCacheItem.ProgramContent, new CachePopulation(MatchingContentData.GetProgramContent, true));
            //populationMethods.Add(MatchingCacheItem.ClickProgramProducts, new CachePopulation(RuleCacheProcessor.GetClickProgramProduct, true));
            populationMethods.Add(MatchingCacheItem.SRADefinition, new CachePopulation(SchoolRankingEngine.GetActiveBusinessModels, true));
            populationMethods.Add(MatchingCacheItem.SRAeRPL, new CachePopulation(SchoolRankingEngine.GetLeadCurrentRPL, true));
            populationMethods.Add(MatchingCacheItem.SRAStrategic, new CachePopulation(SchoolRankingEngine.GetStrategicValues, true));
            populationMethods.Add(MatchingCacheItem.ZipCodeCoordinates, new CachePopulation(GeoCodeProcessor.GetAllZipCodeCoordinates, false));
            populationMethods.Add(MatchingCacheItem.RERuleFactory, new CachePopulation(RuleCacheProcessor.LoadRulesEngineFactory, true));
            populationMethods.Add(MatchingCacheItem.REProgramLevelToEdLevelMapping, new CachePopulation(RuleCacheProcessor.GetProgramLevelToEdLevelMapping, true));
            populationMethods.Add(MatchingCacheItem.REProgramToEdLevelMapping, new CachePopulation(RuleCacheProcessor.GetProgramToEdLevelMapping, true));
            populationMethods.Add(MatchingCacheItem.InstitutionContent, new CachePopulation(MatchingContentData.GetInstitutionContent, true));
            populationMethods.Add(MatchingCacheItem.MatchDatabase, new CachePopulation(MatchDatabase.LoadMatchDatabase, true));
            populationMethods.Add(MatchingCacheItem.CampusContent, new CachePopulation(MatchingContentData.GetCampusContent, true));
            populationMethods.Add(MatchingCacheItem.ProgramLevelContent, new CachePopulation(MatchingContentData.GetProgramLevelContent, true));
            populationMethods.Add(MatchingCacheItem.SubjectContent, new CachePopulation(MatchingContentData.GetSubjectContent, true));
            populationMethods.Add(MatchingCacheItem.CategoryContent, new CachePopulation(MatchingContentData.GetCategoryContent, true));
            populationMethods.Add(MatchingCacheItem.RECaps, new CachePopulation(RuleCacheProcessor.CreateHierarchicalCapList, true));
            populationMethods.Add(MatchingCacheItem.CampaignCaps, new CachePopulation(RuleCacheProcessor.CreateHierarchicalCampaignCapList, true));
            populationMethods.Add(MatchingCacheItem.RERuleDefinitionData, new CachePopulation(RuleCacheProcessor.GetRuleDefinitionCache, true));
            populationMethods.Add(MatchingCacheItem.REKVCodeData, new CachePopulation(RuleCacheProcessor.GetKVCodeDataCache, true));
            populationMethods.Add(MatchingCacheItem.REClientRelationProductMapping, new CachePopulation(RuleCacheProcessor.GetClientRelationProductMappingRecords, true));
            //populationMethods.Add(MatchingCacheItem.REProgramProductTemplateAssignment, new CachePopulation(RuleCacheProcessor.GetProgramProductTemplateAssignment, true));
            populationMethods.Add(MatchingCacheItem.CrossSellMapping, new CachePopulation(CrossSellProcessor.GetAllCrossSellMappings, true));
            populationMethods.Add(MatchingCacheItem.Features, new CachePopulation(FeatureProcessor.GetFeatures, true));
            populationMethods.Add(MatchingCacheItem.InstitutionGroup, new CachePopulation(RuleCacheProcessor.GetInstitutionGroupCache, true));
            populationMethods.Add(MatchingCacheItem.RECallCenterHours, new CachePopulation(RuleCacheProcessor.GetCallCenterHoursCache, true));
            populationMethods.Add(MatchingCacheItem.REStateGeoData, new CachePopulation(RuleCacheProcessor.GetStateGeoCacheItem, true));
            populationMethods.Add(MatchingCacheItem.LeadScoringProducts, new CachePopulation(LeadScoringProcessor.GetLeadScoreProducts, true));
            populationMethods.Add(MatchingCacheItem.ProgramDisplayGroups, new CachePopulation(MatchingContentData.GetProgramDisplayGroups, true));
            populationMethods.Add(MatchingCacheItem.CampusZipCode, new CachePopulation(RuleCacheProcessor.GetCampusZipCodeInclusionExclusion, true));
            populationMethods.Add(MatchingCacheItem.LeadScoreReservationSupplyUnits, new CachePopulation(RuleCacheProcessor.GetLeadScoreReservationSupplyUnits, true));
            populationMethods.Add(MatchingCacheItem.LeadScoreReservationConfigurations, new CachePopulation(RuleCacheProcessor.GetLeadScoreReservations, true));
            populationMethods.Add(MatchingCacheItem.REClientCampusProductMapping, new CachePopulation(RuleCacheProcessor.GetClientCampusProductMappingRecords, true));
            populationMethods.Add(MatchingCacheItem.RECountryGeoData, new CachePopulation(RuleCacheProcessor.GetCountryGeoCacheItem, true));
            populationMethods.Add(MatchingCacheItem.REWarmTransferInfo, new CachePopulation(RuleCacheProcessor.GetWarmTransferInfoCache, true));
            populationMethods.Add(MatchingCacheItem.SpecialtyContent, new CachePopulation(MatchingContentData.GetSpecialtyContent, true));
            populationMethods.Add(MatchingCacheItem.RECampusCallCenterHours, new CachePopulation(RuleCacheProcessor.GetCampusCallCenterHoursCache, true));
            populationMethods.Add(MatchingCacheItem.CampusNumbersContent, new CachePopulation(MatchingContentData.GetCampusPhoneNumbers, true));
            populationMethods.Add(MatchingCacheItem.Products, new CachePopulation(LeadScoringProcessor.GetProducts, true));
            populationMethods.Add(MatchingCacheItem.Applications, new CachePopulation(LeadScoringProcessor.GetApplications, true));
            populationMethods.Add(MatchingCacheItem.ChannelGroups, new CachePopulation(CampaignDataService.GetChannelGroups, true));
            populationMethods.Add(MatchingCacheItem.Images, new CachePopulation(MatchingContentData.GetImageContent, true));            
            populationMethods.Add(MatchingCacheItem.InstitutionStartDate, new CachePopulation(ContentDataService.GetAllInstitutionStartDates, true));            
            populationMethods.Add(MatchingCacheItem.ProgramStartDate, new CachePopulation(ContentDataService.GetAllProgramStartDates, true));
            populationMethods.Add(MatchingCacheItem.ProgramTag, new CachePopulation(ContentDataService.GetAllProgramTags, true));
            //populationMethods.Add(MatchingCacheItem.ProgramAddresses, new CachePopulation(MatchingContentData.GetProgramAddresses, true));
            populationMethods.Add(MatchingCacheItem.CapMUOverrides, new CachePopulation(RuleCacheProcessor.GetCapNormalizationOverrides, true));
            populationMethods.Add(MatchingCacheItem.SABPSIeRPC, new CachePopulation(SchoolRankingEngine.GetSABPSIeRPL, true));
            populationMethods.Add(MatchingCacheItem.CampaignRestrictedTemplates, new CachePopulation(CampaignRestrictedTemplateProcessor.GetCampaignRestrictedTemplateList, true));
            populationMethods.Add(MatchingCacheItem.ClientRelationshipSchedules, new CachePopulation(RuleCacheProcessor.GetClientRelationshipSchedules, true));
            populationMethods.Add(MatchingCacheItem.ClientRelationshipEntitySchedules, new CachePopulation(RuleCacheProcessor.GetClientRelationshipEntitySchedules, true));
            populationMethods.Add(MatchingCacheItem.StatePricing, new CachePopulation(SchoolRankingEngine.GetStatePricing, true));
            populationMethods.Add(MatchingCacheItem.AgentDisallowedList, new CachePopulation(RuleCacheProcessor.GetAgentDisallowedLiveTransferCRs, true));
            populationMethods.Add(MatchingCacheItem.ClientCampusContacts, new CachePopulation(ContentDataService.GetClientCampusContacts, true));
            populationMethods.Add(MatchingCacheItem.ClientRelationContacts, new CachePopulation(ContentDataService.GetClientRelationContacts, true));
            populationMethods.Add(MatchingCacheItem.MatchResponseSearchVendorAllowed, new CachePopulation(MatchDatabaseDataService.GetAllMatchResponseSearchVendorAllowedLogged, true));
            populationMethods.Add(MatchingCacheItem.CampusOptionGroups, new CachePopulation(MatchingContentData.GetCampusOptionGroupContent, true));
            populationMethods.Add(MatchingCacheItem.LeadPingLeadScores, new CachePopulation(LeadScoringProcessor.GetLeadPingLeadScores, true));
            populationMethods.Add(MatchingCacheItem.LeadPingLeadScoreCPLs, new CachePopulation(LeadScoringProcessor.GetLeadPingLeadScoreCPLs, true));
            populationMethods.Add(MatchingCacheItem.ClientRelationshipChannelCaps, new CachePopulation(RuleCacheProcessor.GetClientRelationshipChannelCaps, true));
        }

        public T Get<T>(MatchingCacheItem key)
        {
            return (T)Get(itemKeyPrefix + key.ToString());
        }

        public override void PreloadEntireCache()
        {
            EDDY.IS.Core.Logging.PerformanceLog pLog = new PerformanceLog(Base.ISApplication.MatchingEngine, "PreloadEntireCache", null, null);

            if (populationMethods.Any(f => f.Value.HoldPrePopulation == false))
            {
                ThreadPool.QueueUserWorkItem(o => LoadDoNotWaitForCacheItems());
            }

            Parallel.ForEach(populationMethods, LoadHoldPrepopulationCacheItems);
            //foreach (var cacheItem in populationMethods)
                //LoadHoldPrepopulationCacheItems(cacheItem);

            MatchDatabase md = StaticCacheProxyHost.CacheProxy.Get<MatchDatabase>(MatchingCacheItem.MatchDatabase);

            if (md != null)
                md.LoadFinalObjects();

            //pLog.StartLogDetail("TemplateCacheItem.LoadUnassignedProgramProducts");
            //TemplateCacheItem tci = StaticCacheProxyHost.CacheProxy.Get<TemplateCacheItem>(MatchingCacheItem.REProgramProductTemplateAssignment);

            //if (tci != null)
            //    tci.LoadUnassignedProgramProducts();
            //pLog.EndLogDetail();

            //StaticCacheProxyHost.CacheProxy.Set("ME_CachePreloaded", true);
            pLog.EndLog(null);
        }

        private void LoadDoNotWaitForCacheItems()
        {
            foreach (KeyValuePair<MatchingCacheItem, CachePopulation> cacheItem in populationMethods)
            {
                if (!cacheItem.Value.HoldPrePopulation)
                    LoadCacheItem(cacheItem.Key, cacheItem.Value.PopulationMethod);
            }
        }

        private void LoadHoldPrepopulationCacheItems(KeyValuePair<MatchingCacheItem, CachePopulation> cacheItem)
        {
            if (cacheItem.Value.HoldPrePopulation)
                LoadCacheItem(cacheItem.Key, cacheItem.Value.PopulationMethod);
        }

        public void LoadCacheItem(MatchingCacheItem key, Func<object> cacheItem)
        {
            EDDY.IS.Core.Logging.PerformanceLog pLog = new PerformanceLog(Base.ISApplication.MatchingEngine, "Loading - " + key, null, null);
            try
            {
                string itemName = key.ToString();

                int? expirationMinutes = GetCacheItemExpiration(itemKeyPrefix + itemName);

                object cacheValue = cacheItem.Invoke();

                if (expirationMinutes.HasValue)
                    appCache.Insert(itemKeyPrefix + itemName, cacheValue, null, DateTime.Now.AddMinutes(expirationMinutes.Value), Cache.NoSlidingExpiration, new CacheItemUpdateCallback(MatchingCacheItemUpdateCallback));
                else
                    appCache.Insert(itemKeyPrefix + itemName, cacheValue);
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            pLog.EndLog(null);
        }

        public override void RemoveItem(string key)
        {
            base.RemoveItem(key);
        }

        public void RemoveItem(MatchingCacheItem item)
        {
            //MatchingCacheItem cacheItemType;
            string itemName = item.ToString();

            int? expirationMinutes = GetCacheItemExpiration(itemKeyPrefix + itemName);

            //if (Enum.TryParse(key, out cacheItemType))
            //{
                var populationFunc = populationMethods[item];

                object cacheValue = populationFunc.PopulationMethod.Invoke();

                base.RemoveItem(itemKeyPrefix + itemName);

                if (expirationMinutes.HasValue)
                    appCache.Insert(itemKeyPrefix + itemName, cacheValue, null, DateTime.Now.AddMinutes(expirationMinutes.Value), Cache.NoSlidingExpiration, new CacheItemUpdateCallback(MatchingCacheItemUpdateCallback));
                else
                    appCache.Insert(itemKeyPrefix + itemName, cacheValue);
            //}
        }

        private void MatchingCacheItemUpdateCallback(string key, CacheItemUpdateReason reason, out object expensiveObject, out CacheDependency dependency, out DateTime absoluteExpiration, out TimeSpan slidingExpiration)
        {
            EDDY.IS.Core.Logging.PerformanceLog p = new PerformanceLog(Base.ISApplication.MatchingEngine, "Refreshing - " + key, null, null);
            
            object newObject = null;

            dependency = null;
            absoluteExpiration = DateTime.Now.AddMinutes(GetCacheItemExpiration(key).Value);
            slidingExpiration = Cache.NoSlidingExpiration;

            try
            {
                newObject = populationMethods[(MatchingCacheItem)Enum.Parse(typeof(MatchingCacheItem), key.Replace(itemKeyPrefix, ""))].PopulationMethod.Invoke();
            }
            catch (Exception ex)
            {
                try
                {
                    newObject = this.Get(key);
                }
                catch { }
                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            expensiveObject = newObject;

            p.EndLog(null);
        }
    }
}
