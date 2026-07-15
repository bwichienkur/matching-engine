using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_Campaign
    {
        public long CampaignId { get; set; }
        public string CampaignName { get; set; }
        public int CampaignTypeId { get; set; }
        public System.Guid TrackId { get; set; }
        public int ChannelId { get; set; }
        public Nullable<int> SubChannelId { get; set; }
        public Nullable<int> VendorId { get; set; }
        public Nullable<int> CostingModelId { get; set; }
        public bool IsOverrideDefaultValues { get; set; }
        public Nullable<bool> IsCappedOut { get; set; }
        public Nullable<bool> ActiveCampaign { get; set; }
        public Nullable<int> ListingResultCount { get; set; }
        public int SubmissionCount { get; set; }
        public Nullable<int> ApplicationId { get; set; }
        public Nullable<int> MaxCrossSellSchools { get; set; }
        public Nullable<int> MaxFunnelSchools { get; set; }
        public Nullable<int> MaxSmartMatchCount { get; set; }
        public bool HasExitPop { get; set; }
        public int AdditionalQuestionsFlowTypeID { get; set; }
        public int ProgramWizardAdditionalQuestionsFlowTypeID { get; set; }
        public bool LeadScoringProductSelectionEnabled { get; set; }
        public bool LeadScoringProductUpsellEnabled { get; set; }
        public Nullable<int> MarketingUnitId { get; set; }
        public bool HasPreCheck { get; set; }
        public Nullable<int> LeadScoringTierLevel { get; set; }
        public bool Allow150MileCampusFilter { get; set; }
        public Guid? LimboAlternativeTrackId { get; set; }
        public bool IsCrossSellALternateList { get; set; }
        public bool LeadScoringAddAdditionalSmartMatch { get; set; }
        public bool CampusAddAdditionalSmartMatch { get; set; }
        public bool AllowRemonetization { get; set; }
        public string CampaignTCPAMessageName { get; set; }
        public bool UseInternationalTemplate { get; set; }
        public int MasterProfileId { get; set; }

        public bool HasLeaveBehind { get; set; }

        public bool HasXVerify { get; set; }

        public Nullable<int> CampaignAPIMatchBehaviorId { get; set; }

        public Nullable<int> NumberOfOnlineBackfillOnGeoPages { get; set; }

        public string CECLeadScore { get; set; }

        public int? InstitutionAgencyTypeId { get; set; }

        public int? CampaignDuplicateLookback { get; set; }

        public string SourceCode { get; set; }

        public int? MediaPlanTypeId { get; set; }

        public bool? AllowPECDoubleMatch { get; set; }
        public int? OpenMailProfileId { get; set; }

        public bool HasChildren { get; set; }

        public bool IgnoreGEORestrictions { get; set; }
        public bool IgnoreJornayaRule { get; set; }
        
        public bool AllowRevShareRPL { get; set; }
        public bool CalculateRevShareByERPL { get; set; }
        public int? RevenueSharePercentage { get; set; }

    public VW_Matching_Campaign(IDataReader dr)
        {
            CampaignId = System.Convert.ToInt64(dr["CampaignId"]);
            CampaignName = System.Convert.ToString(dr["CampaignName"]);
            CampaignTypeId = System.Convert.ToInt32(dr["CampaignTypeId"]);
            TrackId = Guid.Parse(System.Convert.ToString(dr["TrackId"]));
            ChannelId = System.Convert.ToInt32(dr["ChannelId"]);
            IsOverrideDefaultValues = System.Convert.ToBoolean(dr["IsOverrideDefaultValues"]);
            SubmissionCount = System.Convert.ToInt32(dr["SubmissionCount"]);
            HasExitPop = System.Convert.ToBoolean(dr["HasExitPop"]);
            AdditionalQuestionsFlowTypeID = System.Convert.ToInt32(dr["AdditionalQuestionsFlowTypeID"]);
            ProgramWizardAdditionalQuestionsFlowTypeID = System.Convert.ToInt32(dr["ProgramWizardAdditionalQuestionsFlowTypeID"]);
            LeadScoringProductSelectionEnabled = System.Convert.ToBoolean(dr["LeadScoringProductSelectionEnabled"]);
            HasPreCheck = System.Convert.ToBoolean(dr["HasPreCheck"]);
            CECLeadScore = System.Convert.ToString(dr["CECLeadScore"]);

            if (!dr.IsDBNull(dr.GetOrdinal("Allow150MileCampusFilter")) && dr["Allow150MileCampusFilter"] != null && dr["Allow150MileCampusFilter"] != "")
                Allow150MileCampusFilter = System.Convert.ToBoolean(dr["Allow150MileCampusFilter"]);
            else
                Allow150MileCampusFilter = true; //DEFAULT to true if it doesn't exist.

            if (!dr.IsDBNull(dr.GetOrdinal("LeadScoringProductUpsellEnabled")) && dr["LeadScoringProductUpsellEnabled"] != null && dr["LeadScoringProductUpsellEnabled"] != "")
                LeadScoringProductUpsellEnabled = System.Convert.ToBoolean(dr["LeadScoringProductUpsellEnabled"]);
            else
                LeadScoringProductUpsellEnabled = false;

            if (!dr.IsDBNull(dr.GetOrdinal("MaxCrossSellSchools")))
                MaxCrossSellSchools = System.Convert.ToInt32(dr["MaxCrossSellSchools"]);

            if (!dr.IsDBNull(dr.GetOrdinal("MaxFunnelSchools")))
                MaxFunnelSchools = System.Convert.ToInt32(dr["MaxFunnelSchools"]);

            if (!dr.IsDBNull(dr.GetOrdinal("MaxSmartMatchCount")))
                MaxSmartMatchCount = System.Convert.ToInt32(dr["MaxSmartMatchCount"]);
            
            if (!dr.IsDBNull(dr.GetOrdinal("MarketingUnitId")))
                MarketingUnitId = System.Convert.ToInt32(dr["MarketingUnitId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("SubChannelId")))
                SubChannelId = System.Convert.ToInt32(dr["SubChannelId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("VendorId")))
                VendorId = System.Convert.ToInt32(dr["VendorId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CostingModelId")))
                CostingModelId = System.Convert.ToInt32(dr["CostingModelId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IsCappedOut")))
                IsCappedOut = System.Convert.ToBoolean(dr["IsCappedOut"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ActiveCampaign")))
                ActiveCampaign = System.Convert.ToBoolean(dr["ActiveCampaign"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ListingResultCount")))
                ListingResultCount = System.Convert.ToInt32(dr["ListingResultCount"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ApplicationId")))
                ApplicationId = System.Convert.ToInt32(dr["ApplicationId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("LeadScoringTierLevel")))
                LeadScoringTierLevel = System.Convert.ToInt32(dr["LeadScoringTierLevel"]);

            if (!dr.IsDBNull(dr.GetOrdinal("LimboAlternativeTrackId")))
                LimboAlternativeTrackId = Guid.Parse(System.Convert.ToString(dr["LimboAlternativeTrackId"]));

            IsCrossSellALternateList = System.Convert.ToBoolean(dr["IsCrossSellALternateList"]);
            LeadScoringAddAdditionalSmartMatch = System.Convert.ToBoolean(dr["LeadScoringAddAdditionalSmartMatch"]);
            CampusAddAdditionalSmartMatch = System.Convert.ToBoolean(dr["CampusAddAdditionalSmartMatch"]);
            AllowRemonetization = System.Convert.ToBoolean(dr["AllowRemonetization"]);
            CampaignTCPAMessageName = System.Convert.ToString(dr["CampaignTCPAMessageName"]);

            UseInternationalTemplate = !String.IsNullOrEmpty(dr["UseInternationalTemplate"].ToString()) && Convert.ToBoolean(dr["UseInternationalTemplate"]);
           
            HasLeaveBehind = System.Convert.ToBoolean(dr["HasLeaveBehind"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CampaignAPIMatchBehaviorId")))
                CampaignAPIMatchBehaviorId = System.Convert.ToInt32(dr["CampaignAPIMatchBehaviorId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("NumberOfOnlineBackfillOnGeoPages")))
                NumberOfOnlineBackfillOnGeoPages = System.Convert.ToInt32(dr["NumberOfOnlineBackfillOnGeoPages"]);            

            if (!dr.IsDBNull(dr.GetOrdinal("MasterProfileId")))
                MasterProfileId = System.Convert.ToInt32(dr["MasterProfileId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("HasXVerify")))
                HasXVerify = System.Convert.ToBoolean(dr["HasXVerify"]);

            if (!dr.IsDBNull(dr.GetOrdinal("InstitutionAgencyTypeId")))
                InstitutionAgencyTypeId = System.Convert.ToInt32(dr["InstitutionAgencyTypeId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CampaignDuplicateLookback")))
                CampaignDuplicateLookback = System.Convert.ToInt32(dr["CampaignDuplicateLookback"]);

            SourceCode = System.Convert.ToString(dr["SourceCode"]);

            if (!dr.IsDBNull(dr.GetOrdinal("MediaPlanTypeId")))
                MediaPlanTypeId = System.Convert.ToInt32(dr["MediaPlanTypeId"]);

            if(!dr.IsDBNull(dr.GetOrdinal("AllowPECDoubleMatch")))
                AllowPECDoubleMatch = System.Convert.ToBoolean(dr["AllowPECDoubleMatch"]);

            if (!dr.IsDBNull(dr.GetOrdinal("OpenMailProfileId")))
                OpenMailProfileId = System.Convert.ToInt32(dr["OpenMailProfileId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("HasChildren")))
                HasChildren = System.Convert.ToBoolean(dr["HasChildren"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IgnoreGEORestrictions")))
                IgnoreGEORestrictions = System.Convert.ToBoolean(dr["IgnoreGEORestrictions"]);

            if (!dr.IsDBNull(dr.GetOrdinal("IgnoreJornayaRule")))
                IgnoreJornayaRule = System.Convert.ToBoolean(dr["IgnoreJornayaRule"]); 

            if (!dr.IsDBNull(dr.GetOrdinal("AllowRevShareRPL")))
                AllowRevShareRPL = System.Convert.ToBoolean(dr["AllowRevShareRPL"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CalculateRevShareByERPL")))
                CalculateRevShareByERPL = System.Convert.ToBoolean(dr["CalculateRevShareByERPL"]);

            if (!dr.IsDBNull(dr.GetOrdinal("RevenueSharePercentage")))
                RevenueSharePercentage = System.Convert.ToInt32(dr["RevenueSharePercentage"]);
        }
    }
}
