using EDDY.IS.Core;
using EDDY.IS.MatchingEngine.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.Constants;
using EDDY.IS.MatchingEngine.DTO;
using System.Configuration;
using EDDY.IS.Base.Util;
using EDDY.IS.MatchingEngine.Enums;

namespace EDDY.IS.MatchingEngine
{
    public class Campaign
    {
        public long CampaignId { get; set; }
        public int? VendorId { get; set; }
        public int ChannelId { get; set; }
        public int? SubChannelId { get; set; }
        public int? MarketingUnitId { get; set; }
        public Guid TrackId { get; set; }
        public bool IsCapped { get; set; }
        public bool HasChildren { get; set; }
        public bool ActiveCampaign { get; set; }
        public int ListingResultCount { get; set; }
        public int SubmissionCount { get; set; }
        public int? ApplicationId { get; set; }
        public int? MaxSubmissionCount { get; set; }
        public int? MaxItemsDisplayed { get; set; }
        public virtual int? MaxSmartMatchCount { get; set; }
        public bool AllowsExitPop { get; set; }
        public Base.AdditionalQuestionsFlowType AdditionQuestionsFlowType { get; set; }
        public Base.AdditionalQuestionsFlowType ProgramWizardAdditionQuestionsFlowType { get; set; }

        public bool HasPreCheck { get; set; }
        public int? LeadScoringMinimumTierLevel { get; set; }
        public bool Allow150MileCampusFilter { get; set; }
        public int CampaignTypeId { get; set; }
        public Guid? LimboAlternativeTrackId { get; set; }
        public bool IsCrossSellALternateList { get; set; }
        public bool LeadScoringAddAdditionalSmartMatch { get; set; }
        public bool CampusAddAdditionalSmartMatch { get; set; }
        public bool AllowRemonetization { get; set; }
        public string CampaignTCPAMessageName { get; set; }
        public bool UseInternationalTemplate { get; set; }
        public int MasterProfileId { get; set; }
        public bool AllowsLeaveBehind { get; set; }
        public bool HasXVerify { get; set; }
        public string CECLeadScore { get; set; }
        public bool IgnoreGEORestrictions { get; set; }
        public bool IgnoreJornayaRule { get; set; }

        public CampaignProgramLevelMap ProgramLevelMap { get; set; }
        public CampaignCategoryMap CategoryMap { get; set; }
        public CampaignClientRelationshipMap CRMap { get; set; }
        public CampaignSubjectMap SubjectMap { get; set; }
        public List<int> Products { get; set; }
        public HashSet<Tuple<int, int>> CRProductInclusions { get; set; }
        public HashSet<Tuple<int, int>> CRProductExclusions { get; set; }
        public HashSet<Tuple<int, int>> CRPSIExclusions { get; set; }

        public CampaignAPIMatchBehavior? CampaignAPIMatchBehavior { get; set; }

        public InstitutionAgencyType? InstitutionAgencyType { get; set; }
        public int? CampaignDuplicateLookback { get; set; }
        public int? NumberOfOnlineBackfillOnGeoPages { get; set; }

        public string SourceCode { get; set; }

        public MediaPlanType? MediaPlanType { get; set; }
        public bool AllowPECDoubleMatch { get; set; }
        public int? OpenMailProfileId { get; set; }
        public bool AllowRevShareRPL { get; set; }
        public bool CalculateRevShareByERPL { get; set; }
        public int? RevenueSharePercentage { get; set; }

        public Campaign()
        {
            Products = new List<int>();
            CRProductExclusions = new HashSet<Tuple<int, int>>();
            CRProductInclusions = new HashSet<Tuple<int, int>>();
            CRPSIExclusions = new HashSet<Tuple<int, int>>();
        }

        public static Campaign Get(Guid trackId)
        {
            Dictionary<Guid, Campaign> campaigns = StaticCacheProxyHost.CacheProxy.Get<Dictionary<Guid, Campaign>>(MatchingCacheItem.Campaigns);

            if (campaigns != null && campaigns.ContainsKey(trackId))
                return campaigns[trackId];
            else
                return null;
        }

        public static Dictionary<Guid, Campaign> GetCampaignCache()
        {
            return CampaignDataService.GetCampaigns();
        }

        public bool PassesLeadScoring(int? leadScoringTierLevel, out BaseRuleType? ruleTypeFailure)
        {
            ruleTypeFailure = null;

            if (leadScoringTierLevel.HasValue && leadScoringTierLevel > this.LeadScoringMinimumTierLevel)
            {
                ruleTypeFailure = BaseRuleType.LeadScoringMinimumTierLevel;
                return false;
            }

            return true;
        }

        public bool IsValid(out BaseRuleType? ruleTypeFailure)
        {
            ruleTypeFailure = null;

            if (!this.ActiveCampaign)
            {
                ruleTypeFailure = BaseRuleType.CampaignInactive;
                return false;
            }
            if (this.IsCampaignCapped())
            {
                ruleTypeFailure = BaseRuleType.CampaignCapReached;
                return false;
            }

            //else campaign is valid
            return true;
        }

        public bool IsValid(int? leadScoringTierLevel, out BaseRuleType? ruleTypeFailure)
        {
            ruleTypeFailure = null;

            if (!this.PassesLeadScoring(leadScoringTierLevel, out ruleTypeFailure))
                return false;

            if (!this.ActiveCampaign)
            {
                ruleTypeFailure = BaseRuleType.CampaignInactive;
                return false;
            }
            if (this.IsCampaignCapped())
            {
                ruleTypeFailure = BaseRuleType.CampaignCapReached;
                return false;
            }

            //else campaign is valid
            return true;
        }

        public Cap GetCapWithChildren()
        {
            Dictionary<int, Cap> caps = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, Cap>>(MatchingCacheItem.CampaignCaps);
            return caps[int.Parse(this.CampaignId.ToString())];
        }

        public bool IsCampaignCapped()
        {
            bool res = false;
            if (this.IsCapped) //if the campaign is capped return
                res = true;
            if(!res && this.HasChildren) //if not capped and has children
            {
                Cap theCap = GetCapWithChildren();
                if (theCap != null && theCap.Children != null)
                {
                    res = theCap.Children.Where(c => !c.Capped).Count() == 0; //if all children are capped out then this campaign is capped
                }
            }
            return res;
        }

        public HashSet<int> GetAllAllowedProducts(IS.Base.ISApplication? application)
        {
            HashSet<int> products = GetCampaignAllowProducts(application);

            foreach (Tuple<int, int> crProduct in this.CRProductInclusions)
            {
                products.Add(crProduct.Item1);
            }

            return products;
        }


        private HashSet<int> GetCampaignAllowProducts(IS.Base.ISApplication? application)
        {
            HashSet<int> products = new HashSet<int>();
            //List<int> PremierTierList = ConfigurationManager.AppSettings["LeadScoringPremierTiers"].Split(',').Select(n => int.Parse(n)).ToList();

            //get the product quality scores from the matching engine cache
            foreach (int productId in this.Products)
            {
                if (application.HasValue && Product.IsWarmTransferProduct(productId) && application.Value != Base.ISApplication.Apollo && application.Value != Base.ISApplication.VendorAPI)
                    continue;
                else
                    products.Add(productId);
            }

            return products;
        }
        
        public List<MatchItem> ApplyPSIFilter(List<MatchItem> result, IS.Base.ISApplication? application)
        {

            Dictionary<int, HashSet<int>> crPSIInclusions = new Dictionary<int, HashSet<int>>();
            Dictionary<int, HashSet<int>> crPSIExclusions = new Dictionary<int, HashSet<int>>();

            
            foreach (Tuple<int, int> crExclusion in this.CRPSIExclusions)
            {
                if (crPSIExclusions.ContainsKey(crExclusion.Item1))
                    crPSIExclusions[crExclusion.Item1].Add(crExclusion.Item2);
                else
                    crPSIExclusions.Add(crExclusion.Item1, new HashSet<int> { crExclusion.Item2 });
            }

            var updateCount = result.Where(m => 
                                        (crPSIExclusions.ContainsKey(m.Match.PsiId)
                                            && crPSIExclusions[m.Match.PsiId].Contains(m.Match.ClientRelationshipId)
                                        )
                                ).Update(u => { u.FailedValidation = true; u.RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "CRPSI" }; });
            
            return result;
        }

        public List<MatchItem> ApplyProductFilter(List<MatchItem> result, IS.Base.ISApplication? application)
        {
            HashSet<int> campaignAllowedProducts = GetCampaignAllowProducts(application);
            
            Dictionary<int, HashSet<int>> crProductInclusions = new Dictionary<int, HashSet<int>>();
            Dictionary<int, HashSet<int>> crProductExclusions = new Dictionary<int, HashSet<int>>();

            foreach (Tuple<int, int> crInclusion in this.CRProductInclusions)
            {
                if (!campaignAllowedProducts.Contains(crInclusion.Item1))
                {
                    if (crProductInclusions.ContainsKey(crInclusion.Item1))
                        crProductInclusions[crInclusion.Item1].Add(crInclusion.Item2);
                    else
                        crProductInclusions.Add(crInclusion.Item1, new HashSet<int> { crInclusion.Item2 });
                }
            }
            foreach (Tuple<int, int> crExclusion in this.CRProductExclusions)
            {
                if (crProductExclusions.ContainsKey(crExclusion.Item1))
                    crProductExclusions[crExclusion.Item1].Add(crExclusion.Item2);
                else
                    crProductExclusions.Add(crExclusion.Item1, new HashSet<int> { crExclusion.Item2 });
            }

            var updateCount = result.Where(m => (crProductInclusions.ContainsKey(m.Match.ProductId) 
                                        && !crProductInclusions[m.Match.ProductId].Contains(m.Match.ClientRelationshipId)
                                        ) ||
                                        (crProductExclusions.ContainsKey(m.Match.ProductId) 
                                            && crProductExclusions[m.Match.ProductId].Contains(m.Match.ClientRelationshipId)
                                        )
                                ).Update(u => { u.FailedValidation = true; u.RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "CRProduct" }; });

            return result;
        }

        public List<MatchItem> ApplyProductFilter(ProductFilterType filterType, int? leadScoringTierLevel, List<MatchItem> result, IS.Base.ISApplication? application)
        {
            //If called, but no filter then return all match items passed in.
            if (filterType == ProductFilterType.None)
                return result;

            HashSet<int> campaignAllowedProducts = GetCampaignAllowProducts(application);
            Dictionary<int, int> productQualityRankings = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, int>>(MatchingCacheItem.LeadScoringProducts);

            if (filterType == ProductFilterType.Full)
            {
                HashSet<int> allProducts = GetAllAllowedProducts(application);
                //result.RemoveAll(m => !allProducts.Contains(m.Match.ProductId));
                result.Where(m => !allProducts.Contains(m.Match.ProductId)).Update(u => { u.FailedValidation = true; u.RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "Product" }; });
            }

            Dictionary<int, HashSet<int>> crProductInclusions = new Dictionary<int, HashSet<int>>();
            Dictionary<int, HashSet<int>> crProductExclusions = new Dictionary<int, HashSet<int>>();

            foreach (Tuple<int, int> crInclusion in this.CRProductInclusions)
            {
                if (!campaignAllowedProducts.Contains(crInclusion.Item1))
                {
                    if (crProductInclusions.ContainsKey(crInclusion.Item1))
                        crProductInclusions[crInclusion.Item1].Add(crInclusion.Item2);
                    else
                        crProductInclusions.Add(crInclusion.Item1, new HashSet<int> { crInclusion.Item2 });
                }
            }
            foreach (Tuple<int, int> crExclusion in this.CRProductExclusions)
            {
                if (crProductExclusions.ContainsKey(crExclusion.Item1))
                    crProductExclusions[crExclusion.Item1].Add(crExclusion.Item2);
                else
                    crProductExclusions.Add(crExclusion.Item1, new HashSet<int> { crExclusion.Item2 });
            }

            var updateCount = result.Where(m => (crProductInclusions.ContainsKey(m.Match.ProductId)
                                        && !crProductInclusions[m.Match.ProductId].Contains(m.Match.ClientRelationshipId)
                                        ) ||
                                        (crProductExclusions.ContainsKey(m.Match.ProductId)
                                            && crProductExclusions[m.Match.ProductId].Contains(m.Match.ClientRelationshipId)
                                        )
                                ).Update(u => { u.FailedValidation = true; u.RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "CRProduct" }; }); 

            return result;
        }

        private void ApplyCampaignFiltersInternal(List<MatchItem> result, IS.Base.ISApplication? application)
        {
            if ((this.ProgramLevelMap != null && this.ProgramLevelMap.ProgramLevelIds.Count > 0) ||
                 (this.CategoryMap != null && this.CategoryMap.CategoryIds.Count > 0) ||
                 (this.SubjectMap != null && this.SubjectMap.SubjectIds.Count > 0) ||
                 (application.HasValue && application.Value == Base.ISApplication.VendorAPI &&
                    this.CRMap != null && this.CRMap.ClientRelationshipIds.Count > 0) ||
                    //Remove Free/Fraid if not SEO Channel
                    this.ChannelId != 21)
            {
                /*
                Do not change to LINQ implementation. That will cause many extra loops which we're trying to avoid for performance reasons. - ERICK
                */
                for (int i = result.Count - 1; i >= 0; i--)
                {
                    //Remove Free/Fraid for non-SEO channel
                    if (this.ChannelId != 21 && result[i].Match.PaidStatusTypeId != PaidStatusType.Paid)
                    {
                        //result.RemoveAt(i);
                        result[i].FailedValidation = true;
                        result[i].RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "Remove Free/Fraid for non-SEO channel" };
                        continue;
                    }

                    if (this.ProgramLevelMap != null && this.ProgramLevelMap.ProgramLevelIds.Count > 0)
                    {
                        if (this.ProgramLevelMap.IsInclusion)
                        {
                            if (!this.ProgramLevelMap.ProgramLevelIds.Contains(result[i].Match.ProgramLevelId))
                            {
                                //result.RemoveAt(i);
                                result[i].FailedValidation = true;
                                result[i].RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "ProgramLevel" };
                                continue;
                            }
                        }
                        else
                        {
                            if (this.ProgramLevelMap.ProgramLevelIds.Contains(result[i].Match.ProgramLevelId))
                            {
                                //result.RemoveAt(i);
                                result[i].FailedValidation = true;
                                result[i].RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "ProgramLevel" };
                                continue;
                            }
                        }
                    }

                    if (this.CategoryMap != null && this.CategoryMap.CategoryIds.Count > 0)
                    {
                        if (this.CategoryMap.IsInclusion)
                        {
                            if (result[i].Match.PrimaryCategoryId.HasValue && !this.CategoryMap.CategoryIds.Contains(result[i].Match.PrimaryCategoryId.Value))
                            {
                                //result.RemoveAt(i);
                                result[i].FailedValidation = true;
                                result[i].RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "Category" };
                                continue;
                            }
                        }
                        else
                        {
                            if (result[i].Match.PrimaryCategoryId.HasValue && this.CategoryMap.CategoryIds.Contains(result[i].Match.PrimaryCategoryId.Value))
                            {
                                //result.RemoveAt(i);
                                result[i].FailedValidation = true;
                                result[i].RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "Category" };
                                continue;
                            }
                        }
                    }

                    if (this.SubjectMap != null && this.SubjectMap.SubjectIds.Count > 0)
                    {
                        if (this.SubjectMap.IsInclusion)
                        {
                            if (result[i].Match.PrimarySubjectId.HasValue && !this.SubjectMap.SubjectIds.Contains(result[i].Match.PrimarySubjectId.Value))
                            {
                                //result.RemoveAt(i);
                                result[i].FailedValidation = true;
                                result[i].RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "Subject" };
                                continue;
                            }
                        }
                        else
                        {
                            if (result[i].Match.PrimarySubjectId.HasValue && this.SubjectMap.SubjectIds.Contains(result[i].Match.PrimarySubjectId.Value))
                            {
                                //result.RemoveAt(i);
                                result[i].FailedValidation = true;
                                result[i].RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "Subject" };
                                continue;
                            }
                        }
                    }

                    if (application.HasValue && application.Value == Base.ISApplication.VendorAPI &&
                        this.CRMap != null && this.CRMap.ClientRelationshipIds.Count > 0)
                    {
                        if (this.CRMap.IsInclusion)
                        {
                            if (!this.CRMap.ClientRelationshipIds.Contains(result[i].Match.ClientRelationshipId))
                            {
                                //result.RemoveAt(i);
                                result[i].FailedValidation = true;
                                result[i].RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "ClientRelationship" };
                                continue;
                            }
                        }
                        else
                        {
                            if (this.CRMap.ClientRelationshipIds.Contains(result[i].Match.ClientRelationshipId))
                            {
                                //result.RemoveAt(i);
                                result[i].FailedValidation = true;
                                result[i].RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, RuleEntityEntityId = (int)this.CampaignId, RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "ClientRelationship" };
                                continue;
                            }
                        }
                    }
                }
            }
        }

        public List<MatchItem> ApplyCampaignFilters(List<MatchItem> matchItemList, IS.Base.ISApplication? application, EDDY.IS.Core.Logging.PerformanceLog plog)
        {
            plog.StartLogDetail("ApplyCampaignFilters");

            List<MatchItem> result = matchItemList;

            plog.StartLogDetail("Campaign.ApplyProductFilter");
            result = ApplyProductFilter(result, application);
            plog.EndLogDetail();

            plog.StartLogDetail("Campaign.ApplyPSIFilter");
            result = ApplyPSIFilter(result, application);
            plog.EndLogDetail();

            plog.StartLogDetail("Campaign.ApplyNonProductFilters");
            ApplyCampaignFiltersInternal(result, application);
            plog.EndLogDetail();

            if(this.HasChildren)
            {
                plog.StartLogDetail("Campaign.ApplyCampaignChildCapFilters");
                ApplyCampaignChildCapFilters(result, application);
                plog.EndLogDetail();
            }

            return result;
        }

        public List<MatchItem> ApplyCampaignFilters(List<MatchItem> matchItemList, IS.Base.ISApplication? application, ProductFilterType filterType, EDDY.IS.Core.Logging.PerformanceLog plog)
        {
            plog.StartLogDetail("ApplyCampaignFilters");

            List<MatchItem> result = matchItemList;

            if (filterType != ProductFilterType.None && this.Products != null && this.Products.Count > 0)
            {
                plog.StartLogDetail("Campaign.ApplyProductFilter");
                result = ApplyProductFilter(filterType, null, result, application);
                plog.EndLogDetail();
            }

            plog.StartLogDetail("Campaign.ApplyPSIFilter");
            result = ApplyPSIFilter(result, application);
            plog.EndLogDetail();

            plog.StartLogDetail("Campaign.ApplyNonProductFilters");
            ApplyCampaignFiltersInternal(result, application);
            plog.EndLogDetail();

            if (this.HasChildren)
            {
                plog.StartLogDetail("Campaign.ApplyCampaignChildCapFilters");
                ApplyCampaignChildCapFilters(result, application);
                plog.EndLogDetail();
            }

            return result;
        }
        public List<MatchItem> ApplyCampaignChildCapFilters(List<MatchItem> result, IS.Base.ISApplication? application)
        {
            Cap c = this.GetCapWithChildren();
            List<int> cappedCCR = new List<int>();
            List<Cap> cappedCampusCap = c.Children.Where(ca => ca.Capped).ToList(); //get the capped children
            foreach(Cap cap in cappedCampusCap)
            {
                cappedCCR.AddRange(cap.EntityIDSet); //add the ccr of the capped child to a collection
            }
            //update matches so any match for that ccr is failed for campus cap under the campaign
            var updateCount = result.Where(m => cappedCCR.Contains(m.Match.ClientCampusRelationshipId))
                                    .Update(u => { u.FailedValidation = true; 
                                        u.RemovalReason = new RemovalReason() { RuleEntity = EntityMeta.Campaign, 
                                            RuleEntityEntityId = (int)this.CampaignId, 
                                            RuleType = BaseRuleType.CampaignRestriction, RuleDetail = "CampaignCampusCap" }; });

            return result;
        }
    }
    public enum CampaignAPIMatchBehavior
    {
        SmartMatchOnlyIfHasEddySmartMatchOrSchoolSelection = 1,
        SmatchMatchAnyScenario = 2,
        SchoolSelectionOnly = 3,
        SmartMatchAndSchoolSelection = 4
    }
    public enum ProductFilterType
    {
        None,
        CRProductOnly,
        Full
    }
    public class CampaignProgramLevelMap
    {
        public bool IsInclusion { get; set; }
        public List<int> ProgramLevelIds { get; private set; }

        public CampaignProgramLevelMap()
        {
            ProgramLevelIds = new List<int>();
        }
    }

    public class CampaignCategoryMap
    {
        public bool IsInclusion { get; set; }
        public List<int> CategoryIds { get; private set; }

        public CampaignCategoryMap()
        {
            CategoryIds = new List<int>();
        }
    }

    public class CampaignSubjectMap
    {
        public bool IsInclusion { get; set; }
        public List<int> SubjectIds { get; private set; }

        public CampaignSubjectMap()
        {
            SubjectIds = new List<int>();
        }
    }

    public class CampaignClientRelationshipMap
    {
        public bool IsInclusion { get; set; }
        public List<int> ClientRelationshipIds { get; private set; }

        public CampaignClientRelationshipMap()
        {
            ClientRelationshipIds = new List<int>();
        }
    }

}
