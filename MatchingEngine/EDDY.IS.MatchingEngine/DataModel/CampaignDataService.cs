using EDDY.IS.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using System.Data.SqlClient;
using EDDY.IS.Core.Logging;
using System.Collections.Concurrent;
using EDDY.IS.Base;
using EDDY.IS.MatchingEngine.Enums;

namespace EDDY.IS.MatchingEngine.DataModel
{
    internal class CampaignDataService
    {
        public static Dictionary<Guid, Campaign> GetCampaigns()
        {
            Dictionary<Guid, Campaign> campaigns = new Dictionary<Guid, Campaign>();
            Dictionary<long, Guid> campaignIdsToGuids = new Dictionary<long, Guid>();

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_Campaign");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        Campaign c = Convert(new Entity.VW_Matching_Campaign(dr));

                        if (!campaigns.ContainsKey(c.TrackId))
                        {
                            campaigns.Add(c.TrackId, c);
                            campaignIdsToGuids.Add(c.CampaignId, c.TrackId);
                        }
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    ISException isEx = new ISException(ex);
            //    isEx.Save();
            //}
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            GetCampaignsAddDetailedLists(campaigns, campaignIdsToGuids);
            GetCampaignsAddAdjustments(campaigns, campaignIdsToGuids);

            return campaigns;
        }

        public static Dictionary<int, HashSet<int>> GetChannelGroups()
        {
            Dictionary<int, HashSet<int>> channelGroups = new Dictionary<int, HashSet<int>>();
            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_ChannelGroup");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        VW_Matching_ChannelGroup listItem = new VW_Matching_ChannelGroup(dr);

                        if (channelGroups.ContainsKey(listItem.ChannelId))
                            channelGroups[listItem.ChannelId].Add(listItem.ChannelGroupId);
                        else
                            channelGroups.Add(listItem.ChannelId, new HashSet<int>() { listItem.ChannelGroupId });
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return channelGroups;
        }

        private static void GetCampaignsAddDetailedLists(Dictionary<Guid, Campaign> campaigns, Dictionary<long, Guid> campaignIdsToGuids)
        {
            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CampaignDetailedLists");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        VW_Matching_CampaignDetailedLists listItem = new Entity.VW_Matching_CampaignDetailedLists(dr);

                        if (campaignIdsToGuids.ContainsKey(listItem.CampaignId))
                        {
                            Campaign campaign = campaigns[campaignIdsToGuids[listItem.CampaignId]];
                            AddDetailedListToCampaign(campaign, listItem);
                        }
                    }
                }
            }
            //catch (Exception ex)
            //{
            //    ISException isEx = new ISException(ex);
            //    isEx.Save();
            //}
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
        }

        private static void GetCampaignsAddAdjustments(Dictionary<Guid, Campaign> campaigns, Dictionary<long, Guid> campaignIdsToGuids)
        {
            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CampaignCRAdjustments");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        VW_Matching_CampaignCRAdjustments listItem = new Entity.VW_Matching_CampaignCRAdjustments(dr);

                        if (campaignIdsToGuids.ContainsKey(listItem.CampaignId))
                        {
                            Campaign campaign = campaigns[campaignIdsToGuids[listItem.CampaignId]];

                            if (listItem.Include)
                                campaign.CRProductInclusions.Add(new Tuple<int, int>(listItem.ProductId, listItem.ClientRelationshipId));
                            else
                                campaign.CRProductExclusions.Add(new Tuple<int, int>(listItem.ProductId, listItem.ClientRelationshipId));
                        }
                    }
                }

                dr = DataHelper.GetDataReader("VW_Matching_CampaignCRPSIAdjustments");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        VW_Matching_CampaignCRPSIAdjustments listItem = new Entity.VW_Matching_CampaignCRPSIAdjustments(dr);

                        if (campaignIdsToGuids.ContainsKey(listItem.CampaignId))
                        {
                            Campaign campaign = campaigns[campaignIdsToGuids[listItem.CampaignId]];

                            campaign.CRPSIExclusions.Add(new Tuple<int, int>(listItem.PSIId, listItem.ClientRelationshipId));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
        }
        public static Campaign GetCampaignByTrackId(Guid trackId)
        {
            VW_Matching_Campaign campaignData = null;
            Campaign finalCampaign = null;

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_Campaign", "trackId = '" + trackId.ToString() + "'");

                if (dr.HasRows)
                {
                    dr.Read();
                    campaignData = new Entity.VW_Matching_Campaign(dr);
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }
            if (campaignData != default(VW_Matching_Campaign))
            {
                finalCampaign = Convert(campaignData);

                GetCampaignDetailedListsByCampaignId(finalCampaign);
                GetCampaignCRAdjustmentsByCampaignId(finalCampaign);
            }

            return finalCampaign;
        }

        private static Campaign Convert(VW_Matching_Campaign c)
        {
            if (c != null)
                return new Campaign()
                {
                    ActiveCampaign = c.ActiveCampaign.Value,
                    ApplicationId = c.ApplicationId,
                    CampaignId = c.CampaignId,
                    ChannelId = c.ChannelId,
                    IsCapped = c.IsCappedOut.Value,
                    HasChildren = c.HasChildren,
                    ListingResultCount = c.ListingResultCount.Value,
                    SubmissionCount = c.SubmissionCount,
                    TrackId = c.TrackId,
                    VendorId = c.VendorId,
                    MaxSubmissionCount = c.SubmissionCount,
                    MaxItemsDisplayed = c.MaxFunnelSchools,
                    MaxSmartMatchCount = c.MaxSmartMatchCount,
                    AllowsExitPop = c.HasExitPop,
                    AdditionQuestionsFlowType = (Base.AdditionalQuestionsFlowType)c.AdditionalQuestionsFlowTypeID,
                    ProgramWizardAdditionQuestionsFlowType = (Base.AdditionalQuestionsFlowType)c.ProgramWizardAdditionalQuestionsFlowTypeID,
                    HasPreCheck = c.HasPreCheck,
                    MarketingUnitId = c.MarketingUnitId,
                    LeadScoringMinimumTierLevel = c.LeadScoringTierLevel,
                    SubChannelId = c.SubChannelId,
                    Allow150MileCampusFilter = c.Allow150MileCampusFilter,
                    CampaignTypeId = c.CampaignTypeId,
                    LimboAlternativeTrackId = c.LimboAlternativeTrackId,
                    IsCrossSellALternateList = c.IsCrossSellALternateList,
                    LeadScoringAddAdditionalSmartMatch = c.LeadScoringAddAdditionalSmartMatch,
                    CampusAddAdditionalSmartMatch = c.CampusAddAdditionalSmartMatch,
                    AllowRemonetization = c.AllowRemonetization,
                    CampaignTCPAMessageName = c.CampaignTCPAMessageName,
                    UseInternationalTemplate = c.UseInternationalTemplate,
                    MasterProfileId = c.MasterProfileId,
                    AllowsLeaveBehind = c.HasLeaveBehind,
                    CampaignAPIMatchBehavior = (CampaignAPIMatchBehavior?)c.CampaignAPIMatchBehaviorId,
                    NumberOfOnlineBackfillOnGeoPages = c.NumberOfOnlineBackfillOnGeoPages,
                    HasXVerify = c.HasXVerify,
                    CECLeadScore = c.CECLeadScore,
                    InstitutionAgencyType = (InstitutionAgencyType?)c.InstitutionAgencyTypeId,
                    CampaignDuplicateLookback = c.CampaignDuplicateLookback,
                    SourceCode = c.SourceCode,
                    MediaPlanType = (MediaPlanType?)c.MediaPlanTypeId,
                    AllowPECDoubleMatch = c.AllowPECDoubleMatch.HasValue ? c.AllowPECDoubleMatch.Value : false,
                    OpenMailProfileId = c.OpenMailProfileId,
                    IgnoreGEORestrictions = c.IgnoreGEORestrictions,
                    IgnoreJornayaRule = c.IgnoreJornayaRule,
                    AllowRevShareRPL = c.AllowRevShareRPL,
                    CalculateRevShareByERPL = c.CalculateRevShareByERPL,
                    RevenueSharePercentage = c.RevenueSharePercentage
                };
            else
                return new Campaign();
        }

        private static void GetCampaignCRAdjustmentsByCampaignId(Campaign campaign)
        {
            List<VW_Matching_CampaignCRAdjustments> crAdjustmentList = new List<Entity.VW_Matching_CampaignCRAdjustments>();
            List<VW_Matching_CampaignCRPSIAdjustments> crPSIAdjustmentList = new List<Entity.VW_Matching_CampaignCRPSIAdjustments>();

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CampaignCRAdjustments", "campaignId = " + campaign.CampaignId);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        crAdjustmentList.Add(new Entity.VW_Matching_CampaignCRAdjustments(dr));
                    }
                }

                dr = DataHelper.GetDataReader("VW_Matching_CampaignCRPSIAdjustments", "campaignId = " + campaign.CampaignId);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        crPSIAdjustmentList.Add(new Entity.VW_Matching_CampaignCRPSIAdjustments(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            foreach (VW_Matching_CampaignCRAdjustments crProduct in crAdjustmentList)
            {
                if (crProduct.Include)
                    campaign.CRProductInclusions.Add(new Tuple<int, int>(crProduct.ProductId, crProduct.ClientRelationshipId));
                else
                    campaign.CRProductExclusions.Add(new Tuple<int, int>(crProduct.ProductId, crProduct.ClientRelationshipId));
            }

            foreach (VW_Matching_CampaignCRPSIAdjustments crProduct in crPSIAdjustmentList)
            {
                campaign.CRPSIExclusions.Add(new Tuple<int, int>(crProduct.PSIId, crProduct.ClientRelationshipId));
            }
        }

        private static void GetCampaignDetailedListsByCampaignId(Campaign campaign)
        {
            List<VW_Matching_CampaignDetailedLists> detailedLists = new List<Entity.VW_Matching_CampaignDetailedLists>();

            SqlDataReader dr = null;

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CampaignDetailedLists", "campaignId = " + campaign.CampaignId);

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        detailedLists.Add(new Entity.VW_Matching_CampaignDetailedLists(dr));
                    }
                }
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(ex);
                isEx.Save();
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            foreach (VW_Matching_CampaignDetailedLists listItem in detailedLists)
            {
                AddDetailedListToCampaign(campaign, listItem);
            }
        }

        private static void AddDetailedListToCampaign(Campaign campaign, VW_Matching_CampaignDetailedLists listItem)
        {
            if (listItem.EntityType == "PL")
            {
                if (campaign.ProgramLevelMap == null)
                {
                    campaign.ProgramLevelMap = new CampaignProgramLevelMap();
                    campaign.ProgramLevelMap.IsInclusion = listItem.Include > 0 ? true : false;
                }

                campaign.ProgramLevelMap.ProgramLevelIds.Add(listItem.EntityId);
            }
            else if (listItem.EntityType == "SB")
            {
                if (campaign.SubjectMap == null)
                {
                    campaign.SubjectMap = new CampaignSubjectMap();
                    campaign.SubjectMap.IsInclusion = listItem.Include > 0 ? true : false;
                }

                campaign.SubjectMap.SubjectIds.Add(listItem.EntityId);
            }
            else if (listItem.EntityType == "CT")
            {
                if (campaign.CategoryMap == null)
                {
                    campaign.CategoryMap = new CampaignCategoryMap();
                    campaign.CategoryMap.IsInclusion = listItem.Include > 0 ? true : false;
                }

                campaign.CategoryMap.CategoryIds.Add(listItem.EntityId);
            }
            else if (listItem.EntityType == "CR")
            {
                if (campaign.CRMap == null)
                {
                    campaign.CRMap = new CampaignClientRelationshipMap();
                    campaign.CRMap.IsInclusion = listItem.Include > 0 ? true : false;
                }

                campaign.CRMap.ClientRelationshipIds.Add(listItem.EntityId);
            }
            else if (listItem.EntityType == "PR")
            {
                if (campaign.Products == null)
                    campaign.Products = new List<int>();

                campaign.Products.Add(listItem.EntityId);
            }
        }




        #region CampaignTemplateRestriction    
        public static ConcurrentDictionary<Guid, HashSet<int>> GetCampaignRestrictedTemplateList()
        {
            SqlDataReader dr = null;
            ConcurrentDictionary<Guid, HashSet<int>> campaignRestrictedTemplateToCampaign = new ConcurrentDictionary<Guid, HashSet<int>>();

            try
            {
                dr = DataHelper.GetDataReader("VW_Matching_CampaignRestrictedTemplateLists");

                if (dr.HasRows)
                {
                    while (dr.Read())
                    {
                        VW_Matching_CampaignRestrictedTemplate campaignRestrictedTemplate = new VW_Matching_CampaignRestrictedTemplate(dr);
                        campaignRestrictedTemplateToCampaign.AddOrUpdate(campaignRestrictedTemplate.TrackId
                                , (key) =>
                                {
                                    HashSet<int> newTemplate = new HashSet<int>() { campaignRestrictedTemplate.TemplateId };
                                    return newTemplate;

                                }
                                , (key, existingTemplate) =>
                                {
                                    existingTemplate.Add(campaignRestrictedTemplate.TemplateId);
                                    return existingTemplate;
                                });
                    }
                }
            }
            finally
            {
                if (dr != null)
                {
                    dr.Close();
                    dr.Dispose();
                }
            }

            return campaignRestrictedTemplateToCampaign;
        }

        #endregion
    }
}
