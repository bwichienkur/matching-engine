using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.MatchingEngine.DTO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    internal class MatchItemMapper
    {
        private static Dictionary<int, SubjectContent> GetSubjectDictionary()
        {
            List<SubjectContent> subjectContent = StaticCacheProxyHost.CacheProxy.Get<List<SubjectContent>>(MatchingCacheItem.SubjectContent);

            return subjectContent.ToDictionary(s => s.SubjectId);
        }

        private static Dictionary<int, CategoryContent> GetCategoryDictionary()
        {
            List<CategoryContent> categoryContent = StaticCacheProxyHost.CacheProxy.Get<List<CategoryContent>>(MatchingCacheItem.CategoryContent);

            return categoryContent.ToDictionary(c => c.CategoryId);
        }

        public static Program MapToProgram(MatchItem programItem, Dictionary<int, List<string>> programGroupList, Dictionary<int, MatchItem> clickPrograms, bool includeImages, Base.InstitutionLeadTypes leadType, List <CampusSlim> campusList = null)
        {
            MatchItem clickMatchItem = null;
            Dictionary<int, VW_Matching_CampusOptionGroupContent> campusOptionGroups = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_CampusOptionGroupContent>>(MatchingCacheItem.CampusOptionGroups);

            int? clickMatchItemProgramProductId = null;
            string clickThroughUrl = null;

            if (clickPrograms != null && clickPrograms.TryGetValue(programItem.Match.ProgramId, out clickMatchItem))
            {
                clickMatchItemProgramProductId = clickMatchItem.Match.ProgramProductId;
                clickThroughUrl = clickMatchItem.Match.ClickThroughUrl;
            }

            bool inquiryDisabled = programItem.Match.InquiryDisabled;
            decimal programRankScore = programItem.ProgramRankScore;
            bool failedValidation = programItem.FailedValidation;

            if(programItem.LeadTypeLeadCapped.HasValue && programItem.LeadTypeClickCapped.HasValue)
            {
                if (programItem.LeadTypeLeadCapped.Value && programItem.Match.HasClick && (!programItem.LeadTypeClickCapped.Value && programItem.FailedValidation))
                    inquiryDisabled = true;
            }

            if(programItem.LeadTypeLeadCapped.HasValue)
            {
                if (programItem.LeadTypeLeadCapped.Value && !programItem.Match.HasClick)
                    programRankScore = 0;

                if (programItem.LeadTypeLeadCapped.Value && !programItem.Match.HasClick)
                    failedValidation = true;
            }

            Program program = new Program()
            {
                ProgramProductId = programItem.Match.ProgramProductId,
                ProgramProductIdClick = !clickMatchItemProgramProductId.HasValue ? null : (int?)clickMatchItemProgramProductId,
                ProgramId = programItem.Match.ProgramId,
                ProgramName = programItem.Match.ProgramName,
                DegreeAcronym = programItem.Match.DegreeAcronym,
                ProgramDescription = programItem.Match.ProgramDescription,
                ProgramShortDescription = programItem.Match.ProgramShortDescription,
                ProgramType = (ProgramType)programItem.Match.ProgramTypeId,
                ProgramLevelId = programItem.Match.ProgramLevelId,
                ProductId = programItem.Match.ProductId,
                SubjectId = programItem.Match.PrimarySubjectId,
                CategoryId = programItem.Match.PrimaryCategoryId,
                ProgramCampusType = (CampusType)programItem.Match.ProgramCampusTypeId,
                ProgramDisclaimerType = programItem.Match.ProgramDisclaimerType,
                ProgramDisclaimer = programItem.Match.ProgramDisclaimer,
                ProgramLevelName = programItem.Match.ProgramLevelName,
                PaidStatusTypeId = programItem.Match.PaidStatusTypeId,
                ClickThroughUrl = !clickMatchItemProgramProductId.HasValue ? programItem.Match.ClickThroughUrl : clickThroughUrl,

                //InquiryDisabled = programItem.LeadTypeLeadCapped.HasValue && programItem.LeadTypeLeadCapped.Value && programItem.Match.HasClick && (programItem.LeadTypeClickCapped.HasValue && !programItem.LeadTypeClickCapped.Value && programItem.FailedValidation) ? true : programItem.Match.InquiryDisabled, //Hack alert - will fix when I redo clicks capping
                InquiryDisabled = inquiryDisabled,

                IsHybrid = programItem.Match.IsHybrid,
                //HasSchoolLogo = programItem.Match.HasSchoolLogo,
                //HasProgramLogo = programItem.Match.HasProgramLogo,
                SchoolLogoURL = programItem.Match.SchoolLogoURL,
                ProgramLogoURL = programItem.Match.ProgramLogoURL,
                CampusLogoURL = programItem.Match.CampusLogoURL,
                InstitutionLogoURL = programItem.Match.InstitutionLogoURL,
                SchoolId = programItem.Match.SchoolId,
                TemplateId = programItem.Match.TemplateId,
                AllowedViaLeadScoringUpsell = programItem.AllowedViaLeadScoringUpsell,
                ShowTwoULeadShareControl = ConfigurationManager.AppSettings["Enable2ULeadShare"] == "true" ? programItem.Match.ShowLeadShare2U : false,

                ProgramSFProductCode = programItem.Match.SFProductCode_PSI,
                SABSRAPosition_PSI = programItem.Match.SABSRAPosition_PSI,

                InstitutionType = leadType,
                ExternalMatchItemGuid = programItem.ExternalMatchItemGuid,

                //ProgramRankScore = programItem.LeadTypeLeadCapped.HasValue & (programItem.LeadTypeLeadCapped.Value && !programItem.Match.HasClick) ? 0 : programItem.ProgramRankScore,
                //FailedValidation = programItem.LeadTypeLeadCapped.HasValue & (programItem.LeadTypeLeadCapped.Value && !programItem.Match.HasClick) ? true : programItem.FailedValidation,

                ProgramRankScore = programRankScore,
                FailedValidation = failedValidation,

                StartDateList = programItem.Match.ProgramStartDateList,
                TagList = programItem.Match.ProgramTagList,
                InstitutionStartDateList = programItem.Match.InstitutionStartDateList,
                CampusOptionGroup = programItem.Match.CampusOptionGroupId.HasValue && campusOptionGroups.ContainsKey(programItem.Match.CampusOptionGroupId.Value)
                                                    ? campusOptionGroups[programItem.Match.CampusOptionGroupId.Value].CampusOptionGroupName : "",
                CampusOptionGroupPosition = programItem.Match.CampusOptionGroupId.HasValue && campusOptionGroups.ContainsKey(programItem.Match.CampusOptionGroupId.Value)
                                                    ? campusOptionGroups[programItem.Match.CampusOptionGroupId.Value].Position : new Nullable<int>(),
                SubjectName = programItem.Match.SubjectName,
                CategoryName = programItem.Match.CategoryName,
                RevenuePerLead = programItem.Match.RPL,
                EffectiveRevenuePerLead = programItem.Match.eRPL
            };

            if (program.ProgramType != ProgramType.FullDegree)
                program.AddressList = programItem.Match.ProgramAddressList;

            if (programGroupList != null && programGroupList.ContainsKey(programItem.Match.ProgramId))
            {
                program.ProgramDisplayGroupProgramLevelNameList = programGroupList[programItem.Match.ProgramId];
                program.ProgramDisplayGroupId = programItem.Match.ProgramDisplayGroupId;
                program.ProgramDisplayGroupName = programItem.Match.ProgramDisplayGroupName;
                program.ProgramDisplayGroupDescription = programItem.Match.ProgramDisplayGroupDescription;
                program.ProgramDisplayGroupShortDescription = programItem.Match.ProgramDisplayGroupShortDescription;
            }

            if (includeImages)
                program.ImageList = programItem.Match.ProgramImageList;

            if(campusList != null)
                program.CampusSlimList = campusList;

            return program;
        }

        public static ProgramWithInstitutionCampus MapToProgramWithInstitutionCampus(MatchItem programItem, Dictionary<int, List<string>> programGroupList, Dictionary<int, MatchItem> clickPrograms, bool includeImages, Base.InstitutionLeadTypes leadType = Base.InstitutionLeadTypes.School)
        {
            ProgramWithInstitutionCampus program = new ProgramWithInstitutionCampus(MapToProgram(programItem, programGroupList, clickPrograms, includeImages, leadType, null));
            Dictionary<int, VW_Matching_CampusOptionGroupContent> campusOptionGroups = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, VW_Matching_CampusOptionGroupContent>>(MatchingCacheItem.CampusOptionGroups);

            program.Is2USchool = ConfigurationManager.AppSettings["Enable2UTCPAChange"] == "true" && programItem.Match.ClientId == 349 ? true : false;
            program.InstitutionId = programItem.Match.InstitutionId;
            program.InstitutionName = programItem.Match.InstitutionName;
            program.InstitutionDisclaimerType = programItem.Match.InstitutionDisclaimerType;
            program.InstitutionDisclaimer = programItem.Match.InstitutionDisclaimer;
            program.InstitutionDescription = programItem.Match.InstitutionDescription;
            program.InstitutionDescriptionInternational = programItem.Match.InstitutionDescriptionInternational;
            program.InstitutionLogoURL = programItem.Match.InstitutionLogoURL;

            program.CampusId = programItem.Match.CampusId;
            program.CampusType = (CampusType)programItem.Match.CampusCampusTypeId;
            program.CampusName = programItem.Match.CampusName;
            program.CampusAddress1 = programItem.Match.CampusAddress1;
            program.CampusAddress2 = programItem.Match.CampusAddress2;
            program.CampusCity = programItem.Match.CampusCity;
            program.CampusState = programItem.Match.CampusStateCode;
            program.CampusPostalCode = programItem.Match.CampusPostalCode;
            program.CampusPhone = programItem.Match.CampusPhone;
            program.CampusFax = programItem.Match.CampusFax;
            //program.HasCampusLogo = programItem.Match.HasCampusLogo;
            program.CampusLogoURL = programItem.Match.CampusLogoURL;
            

            program.RequiresSystemTemplateUse = programItem.Match.RequiresSystemTemplateUse;
            program.RemonetizationRestriction = !programItem.Match.LeadRefinementEnabled;
            program.RequiresSystemTemplateUse = programItem.Match.RequiresSystemTemplateUse;

            program.CampusCountryId = programItem.Match.CampusCountryId.HasValue ? programItem.Match.CampusCountryId.Value : 0;
            program.CampusCountryCode = programItem.Match.CampusCountry;
            program.CampusCountryName = programItem.Match.CampusCountryName;

            program.TreatAsMatch1 = programItem.Match.TreatAsMatch1;

            program.ExternalMatchItemGuid = programItem.ExternalMatchItemGuid;
            program.RevenuePerLead = programItem.Match.RPL;
            program.EffectiveRevenuePerLead = programItem.Match.eRPL;
            program.CustomTCPA = programItem.Match.CustomTCPA;

            return program;
        }

        public static Program MapToProgramNeo(MatchItem programItem, bool isWT, DTO.ProspectInput pi)
        {
            return new Program()
            {
                ProgramProductId = programItem.Match.ProgramProductId,
                ProgramId = programItem.Match.ProgramId,
                ProgramName = programItem.Match.ProgramName,
                DegreeAcronym = programItem.Match.DegreeAcronym,
                ProgramDescription = String.IsNullOrEmpty(programItem.Match.ProgramDescription) ? "" : "true",
                ProgramLevelName = programItem.Match.ProgramLevelName,
                ProgramType = (ProgramType)programItem.Match.ProgramTypeId,
                ProgramLevelId = programItem.Match.ProgramLevelId,
                SubjectId = programItem.Match.PrimarySubjectId,
                CategoryId = programItem.Match.PrimaryCategoryId,
                ProgramCampusType = (CampusType)programItem.Match.ProgramCampusTypeId,
                ProgramRankScore = programItem.ProgramRankScore,
                FailedValidation = programItem.FailedValidation,
                ProductId = programItem.Match.ProductId,
                PaidStatusTypeId = programItem.Match.PaidStatusTypeId,
                TemplateId = programItem.Match.TemplateId,
                SchoolLogoURL = programItem.Match.SchoolLogoURL,
                ProgramLogoURL = programItem.Match.ProgramLogoURL,
                CampusLogoURL = programItem.Match.CampusLogoURL,
                InstitutionLogoURL = programItem.Match.InstitutionLogoURL,
                ExternalMatchItemGuid = programItem.ExternalMatchItemGuid,
                InstitutionType = programItem.InstitutionLeadType,
                TransferNumber = isWT ? LiveTransferNumberProcessor.GetLiveTransferNumber(programItem, pi) : ""
            };
        }

        public static NeoCampus MapToNeoCampus(MatchItem mi)
        {
            return new NeoCampus()
            {
                CampusId = mi.Match.CampusId,
                CampusType = (CampusType)mi.Match.CampusCampusTypeId,
                CampusName = mi.Match.CampusName,
                Address1 = mi.Match.CampusAddress1,
                Address2 = mi.Match.CampusAddress2,
                City = mi.Match.CampusCity,
                State = mi.Match.CampusStateCode,
                PostalCode = mi.Match.CampusPostalCode,
                CountryCode = mi.Match.CampusCountry,
                Phone = mi.Match.CampusPhone,
                Fax = mi.Match.CampusFax,
                InstitutionId = mi.Match.InstitutionId,
                InstitutionName = mi.Match.InstitutionId == 21 ? "American InterContinental University, a member of the AIU System" : mi.Match.InstitutionName,
                CampusLogoURL = mi.Match.CampusLogoURL,
                IsFaithBased = mi.Match.IsFaithBased,
                InstitutionLogoURL = mi.Match.InstitutionLogoURL
            };
        }
    }
}
