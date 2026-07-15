using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine.DataModel.Entity
{
    public class VW_Matching_ProgramProduct
    {
        public int ProgramProductId { get; set; }
        public int InstitutionId { get; set; }
        public int CampusId { get; set; }
        public int ProgramId { get; set; }
        public int ClientRelationshipId { get; set; }
        public int ClientId { get; set; }
        public int ProductId { get; set; }
        public int ClientRelationProductMappingId { get; set; }
        public int ClientCampusRelationshipId { get; set; }
        public int ClientCampusProductMappingId { get; set; }
        public int PsiId { get; set; }
        public int ProgramLevelId { get; set; }
        public int ProgramTypeId { get; set; }
        public int ProgramCampusTypeId { get; set; }
        public int CampusCampusTypeId { get; set; }
        //public int ApplicationId { get; set; }
        public Nullable<int> CategoryId { get; set; }
        public Nullable<int> SubjectId { get; set; }
        public bool IsPrimarySubject { get; set; }
        public Nullable<int> SpecialtyId { get; set; }
        public Nullable<bool> IncludeAllZipCodes { get; set; }
        public Nullable<bool> ProgramProductUseZipCodeRules { get; set; }
        public Nullable<int> ProgramProductAllowableRadius { get; set; }
        public string ProgramProductRadiusZipCode { get; set; }
        public Nullable<bool> ProgramProductZipCodeInclusion { get; set; }
        public Nullable<bool> ProgramProductZipCodeExclusion { get; set; }
        //public int ProgramApplicationId { get; set; }
        public bool CanSmartMatch { get; set; }
        public bool IsHybrid { get; set; }
        public bool InquiryDisabled { get; set; }
        public Nullable<int> ProgramDisplayGroupId { get; set; }
        public string ClickThroughURL { get; set; }
        public bool RequiresSystemTemplateUse { get; set; }
        public bool ShowLeadShare2U { get; set; }
        //public string InquiryDisabledUrl { get; set; }
        //public string AccreditationOrganization { get; set; }
        public Nullable<int> LanguageId { get; set; }
        public Nullable<int> TermId{ get; set; }
        public Nullable<int> DurationId { get; set; }
        public Nullable<int> WorkTypeId { get; set; }
        public Nullable<int> PlacementAudienceId { get; set; }
        public Nullable<int> TeachAbroadTypeId { get; set; }
        public bool LeadRefinementEnabled { get; set; }
        public int? SABSRAPosition_CR { get; set; }
        public int? SABSRAPosition_PSI { get; set; }
        public bool ExcludeMatch1plusForFinAid { get; set; }
        public bool IsNonProfit { get; set; }
        public Nullable<int> TemplateId { get; set; }
        public string DegreeAcronym { get; set; }
        public bool AllowCrossSell { get; set; }
        public bool RequireJournayaLeadId { get; set; }
		public int? CampusOptionGroupId { get; set; }

        public int? AdvertiserId { get; set; }
        public bool? UpsellOutboundTitanium { get; set; }
        public string CustomTCPA { get; set; }
        public string CustomContactCenterTCPA { get; set; }

        public VW_Matching_ProgramProduct(IDataReader dr)
        {
            ProgramProductId = System.Convert.ToInt32(dr["ProgramProductId"]);
            InstitutionId = System.Convert.ToInt32(dr["InstitutionId"]);
            CampusId = System.Convert.ToInt32(dr["CampusId"]);
            ProgramId = System.Convert.ToInt32(dr["ProgramId"]);
            ClientRelationshipId = System.Convert.ToInt32(dr["ClientRelationshipId"]);
            ClientId = System.Convert.ToInt32(dr["ClientId"]);
            ProductId = System.Convert.ToInt32(dr["ProductId"]);
            ClientRelationProductMappingId = System.Convert.ToInt32(dr["ClientRelationProductMappingId"]);
            ClientCampusRelationshipId = System.Convert.ToInt32(dr["ClientCampusRelationshipId"]);
            ClientCampusProductMappingId = System.Convert.ToInt32(dr["ClientCampusProductMappingId"]);
            PsiId = System.Convert.ToInt32(dr["PsiId"]);
            ProgramLevelId = System.Convert.ToInt32(dr["ProgramLevelId"]);
            ProgramTypeId = System.Convert.ToInt32(dr["ProgramTypeId"]);
            ProgramCampusTypeId = System.Convert.ToInt32(dr["ProgramCampusTypeId"]);
            CampusCampusTypeId = System.Convert.ToInt32(dr["CampusCampusTypeId"]);
            //ApplicationId = System.Convert.ToInt32(dr["ApplicationId"]);
            if (!dr.IsDBNull(dr.GetOrdinal("CategoryId")))
                CategoryId = System.Convert.ToInt32(dr["CategoryId"]);
            if (!dr.IsDBNull(dr.GetOrdinal("SubjectId")))
                SubjectId = System.Convert.ToInt32(dr["SubjectId"]);
            if (!dr.IsDBNull(dr.GetOrdinal("SpecialtyId")))
                SpecialtyId = System.Convert.ToInt32(dr["SpecialtyId"]);
            //ProgramApplicationId = System.Convert.ToInt32(dr["ProgramApplicationId"]);
            if (!dr.IsDBNull(dr.GetOrdinal("IsPrimarySubject")))
            IsPrimarySubject = System.Convert.ToBoolean(dr["IsPrimarySubject"]);
            CanSmartMatch = System.Convert.ToBoolean(dr["CanSmartMatch"]);
            IsHybrid = System.Convert.ToBoolean(dr["IsHybrid"]);
            InquiryDisabled = System.Convert.ToBoolean(dr["InquiryDisabled"]);
            ClickThroughURL = System.Convert.ToString(dr["ClickThroughURL"]);
            LeadRefinementEnabled = System.Convert.ToBoolean(dr["LeadRefinementEnabled"]);
            IsNonProfit = System.Convert.ToBoolean(dr["IsNonProfit"]);
            DegreeAcronym = System.Convert.ToString(dr["DegreeAcronym"]);
            //InquiryDisabledUrl = System.Convert.ToString(dr["InquiryDisabledUrl"]);        

            if (!dr.IsDBNull(dr.GetOrdinal("IncludeAllZipCodes")))
                IncludeAllZipCodes = System.Convert.ToBoolean(dr["IncludeAllZipCodes"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProgramProductUseZipCodeRules")))
                ProgramProductUseZipCodeRules = System.Convert.ToBoolean(dr["ProgramProductUseZipCodeRules"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProgramProductAllowableRadius")))
                ProgramProductAllowableRadius = System.Convert.ToInt32(dr["ProgramProductAllowableRadius"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProgramProductRadiusZipCode")))
                ProgramProductRadiusZipCode = System.Convert.ToString(dr["ProgramProductRadiusZipCode"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProgramProductZipCodeInclusion")))
                ProgramProductZipCodeInclusion = System.Convert.ToBoolean(dr["ProgramProductZipCodeInclusion"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProgramProductZipCodeExclusion")))
                ProgramProductZipCodeExclusion = System.Convert.ToBoolean(dr["ProgramProductZipCodeExclusion"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ProgramDisplayGroupId")))
                ProgramDisplayGroupId = System.Convert.ToInt32(dr["ProgramDisplayGroupId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("RequiresSystemTemplateUse")))
                RequiresSystemTemplateUse = System.Convert.ToBoolean(dr["RequiresSystemTemplateUse"]);
            else
                RequiresSystemTemplateUse = false;

            if (!dr.IsDBNull(dr.GetOrdinal("ShowLeadShareTwoU")))
                ShowLeadShare2U = System.Convert.ToBoolean(dr["ShowLeadShareTwoU"]);
            else
                ShowLeadShare2U = false;

            if (!dr.IsDBNull(dr.GetOrdinal("IntensiveProgramLanguageId")))
                LanguageId = System.Convert.ToInt32(dr["IntensiveProgramLanguageId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("TermId")))
                TermId = System.Convert.ToInt32(dr["TermId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("DurationId")))
                DurationId = System.Convert.ToInt32(dr["DurationId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("WorkTypeId")))
                WorkTypeId = System.Convert.ToInt32(dr["WorkTypeId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("PlacementAudienceId")))
                PlacementAudienceId = System.Convert.ToInt32(dr["PlacementAudienceId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("TeachAbroadTypeId")))
                TeachAbroadTypeId = System.Convert.ToInt32(dr["TeachAbroadTypeId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("SABSRAPosition_CR")))
                SABSRAPosition_CR = System.Convert.ToInt32(dr["SABSRAPosition_CR"]);

            if (!dr.IsDBNull(dr.GetOrdinal("SABSRAPosition_PSI")))
                SABSRAPosition_PSI = System.Convert.ToInt32(dr["SABSRAPosition_PSI"]);

            if (!dr.IsDBNull(dr.GetOrdinal("ExcludeMatch1plusForFinAid")))
                ExcludeMatch1plusForFinAid = System.Convert.ToBoolean(dr["ExcludeMatch1plusForFinAid"]);

            if (!dr.IsDBNull(dr.GetOrdinal("TemplateId")))
                TemplateId = System.Convert.ToInt32(dr["TemplateId"]);
            
            if (!dr.IsDBNull(dr.GetOrdinal("AllowCrossSell")))
                AllowCrossSell = System.Convert.ToBoolean(dr["AllowCrossSell"]);
            else
                AllowCrossSell = true;

            if (!dr.IsDBNull(dr.GetOrdinal("RequireJournayaLeadId")))
                RequireJournayaLeadId = System.Convert.ToBoolean(dr["RequireJournayaLeadId"]);
            else
                RequireJournayaLeadId = false;

			if (!dr.IsDBNull(dr.GetOrdinal("CampusOptionGroupId")))
				CampusOptionGroupId = System.Convert.ToInt32(dr["CampusOptionGroupId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("AdvertiserId")))
                AdvertiserId = System.Convert.ToInt32(dr["AdvertiserId"]);

            if (!dr.IsDBNull(dr.GetOrdinal("UpsellOutboundTitanium")))
                UpsellOutboundTitanium = System.Convert.ToBoolean(dr["UpsellOutboundTitanium"]);
            else
                UpsellOutboundTitanium = false;

            if (!dr.IsDBNull(dr.GetOrdinal("CustomTCPA")))
                CustomTCPA = System.Convert.ToString(dr["CustomTCPA"]);

            if (!dr.IsDBNull(dr.GetOrdinal("CustomContactCenterTCPA")))
                CustomContactCenterTCPA = System.Convert.ToString(dr["CustomContactCenterTCPA"]);
        }
    }
}
