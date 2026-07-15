using System;
using System.Collections.Generic;
using System.Linq;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Common.Logging;
using EDDY.IS.Vendor.DataAccess.FormsEngineService;
using EDDY.IS.Vendor.DataAccess.MatchingService;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Common.Utilities;
using ProgramTag = EDDY.IS.Vendor.Entities.ProgramTag;

namespace EDDY.IS.Vendor.DataAccess
{
    public class MatchingServiceDAO : VendorBaseDAO
    {
        private FormsEngineClient formsEngineClient = new FormsEngineClient();
        private MatchingServiceClient matchingServiceClient = new MatchingServiceClient();

        public VendorAPIList GetPrograms(DirectoryRequest directoryRequest)
        {
            VendorAPIList vendorAPIList = null;
            List<Object> itemList = null;
            try
            {

                string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}{8}{9}_GetPrograms", directoryRequest.APIKey, (directoryRequest.CategoryIds != null ? string.Join(",", directoryRequest.CategoryIds.ToArray()) : "noCategoryIds"), (directoryRequest.ProgramLevelIds != null ? string.Join(",", directoryRequest.ProgramLevelIds.ToArray()) : "noLevelIds"), (directoryRequest.SubjectIds != null ? string.Join(",", directoryRequest.SubjectIds.ToArray()) : "noSubjectIds"), directoryRequest.CampusType, directoryRequest.InstitutionId, directoryRequest.CampusId, directoryRequest.PostalCode, directoryRequest.ProspectFilters, (directoryRequest.SpecialtyIds != null ? string.Join(",", directoryRequest.SpecialtyIds.ToArray()) : "noSpecialtyIds"));

                itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                if (itemList == null)
                {

                    Entities.VendorCampaign vendorCampaign = this.getCampaignByTrackId(directoryRequest.APIKey);

                    DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

                    directoryMatchRequest.Application = ISApplication.VendorAPI;
                    directoryMatchRequest.TrackGuid = directoryRequest.APIKey;
                    directoryMatchRequest.InstitutionIdList = new List<int>();

                    if (directoryRequest.InstitutionId != 0)
                    {
                        directoryMatchRequest.InstitutionIdList = new List<int>();
                        directoryMatchRequest.InstitutionIdList.Add(directoryRequest.InstitutionId);
                    }

                    if (directoryRequest.CampusId != 0)
                    {
                        directoryMatchRequest.CampusList = new List<int>();
                        directoryMatchRequest.CampusList.Add(directoryRequest.CampusId);
                    }
                    if (directoryRequest.CategoryIds != null)
                    {
                        if (directoryRequest.CategoryIds.Count > 0)
                        {
                            directoryMatchRequest.CategoryList = directoryRequest.CategoryIds;
                        }
                    }
                    if (directoryRequest.SubjectIds != null)
                    {
                        if (directoryRequest.SubjectIds.Count > 0)
                        {
                            directoryMatchRequest.SubjectList = directoryRequest.SubjectIds;
                        }
                    }
                    if (directoryRequest.SpecialtyIds != null)
                    {
                        if (directoryRequest.SpecialtyIds.Count > 0)
                        {
                            directoryMatchRequest.SpecialtyList = directoryRequest.SpecialtyIds;
                        }
                    }
                    if (directoryRequest.ProgramLevelIds != null)
                    {
                        if (directoryRequest.ProgramLevelIds.Count > 0)
                        {
                            directoryMatchRequest.ProgramLevelList = directoryRequest.ProgramLevelIds;
                        }
                    }



                    directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
                    directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);

                    if (!String.IsNullOrEmpty(directoryRequest.CampusType))
                    {
                        switch (directoryRequest.CampusType.ToLower())
                        {
                            case "online":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Online;
                                break;

                            case "ground":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Ground;
                                break;
                            case "all":
                                break;

                            default:
                                break;

                        }
                    }
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    directoryMatchRequest.RemoveInvalidEntities = true;

                    MatchingService.ProspectInput prospectInput = null;
                    prospectInput = new MatchingService.ProspectInput();
                    if (!String.IsNullOrEmpty(directoryRequest.PostalCode))
                    {
                        prospectInput.PostalCode = directoryRequest.PostalCode;
                    }
                    if (directoryRequest.ProspectFilters != null)
                    {
                        if (directoryRequest.ProspectFilters.Count > 0)
                        {
                            List<AdditionalField> questionAnswers = new List<AdditionalField>();
                            if (directoryRequest.ProspectFilters.Count > 0)
                            {
                                List<ContactStandardControlMapping> mappings = this.getAllContactPropertyToStandardControlMappings();
                                foreach (QuestionAnswer questionAnswer in directoryRequest.ProspectFilters)
                                {
                                    ContactStandardControlMapping map = mappings.Where(m => m.ContactPropertyName.ToLower() == questionAnswer.QuestionKey.ToLower()).FirstOrDefault();
                                    if (map != null)
                                    {
                                        AdditionalField currentQuestionAnswer = new AdditionalField();
                                        currentQuestionAnswer.Key = map.StandardControlName;
                                        currentQuestionAnswer.Value = questionAnswer.QuestionValue;
                                        questionAnswers.Add(currentQuestionAnswer);
                                    }
                                }

                            }



                            FormsEngineClient formsEngineClient = new FormsEngineClient();
                            var apliLead = new APILead
                            {
                                Prefix = "",
                                FirstName = "",
                                LastName = "",
                                Address1 = "",
                                Address2 = "",
                                City = "",
                                ZipCode = "",
                                StateProvince = "",
                                CountryCode = "",
                                USCitizen = "",
                                EmailAddress = "",
                                Phone1 = "",
                                Phone2 = "",
                                Age = null,
                                YearHighestEduCompleted = null,
                                HighestLevelOfEdu = "",
                                Military = "",
                                StartDate = "",
                                Categories = directoryRequest.CategoryIds,
                                SubCategories = directoryRequest.SubjectIds,
                                DesiredDegreeLevel = null,
                                AdditionalFields = questionAnswers.Count > 0 ? questionAnswers : null
                            };

                            FormsEngineService.ProspectInput prospect = formsEngineClient.BuildProspect(vendorCampaign.ApplicationId, false, directoryRequest.APIKey.ToString(), apliLead);
                            if (prospect != null)
                            {
                                prospectInput.KVCodeData = prospect.KVCodeData;
                            }
                        }
                    }
                    directoryMatchRequest.ProspectInput = prospectInput;
                    ProgramResponse matchedProgramResponse = matchingServiceClient.GetPrograms(directoryMatchRequest, true);
                    if (matchedProgramResponse != null)
                    {
                        if (matchedProgramResponse.ProgramList != null)
                        {

                            itemList = new List<Object>();
                            List<MatchingService.Program> matchedProgramList = matchedProgramResponse.ProgramList;
                            if (matchedProgramList.Count > 0)
                            {


                                List<int> programProductIdList = matchedProgramList.GroupBy(p => p.ProgramProductId).Select(grp => grp.FirstOrDefault().ProgramProductId).ToList();
                                List<int> programIdList = matchedProgramList.GroupBy(p => p.ProgramId).Select(grp => grp.FirstOrDefault().ProgramId).ToList();

                                List<ProgramEducationLevel> programEducationLevels = this.getEducationLevelsByProgramIdList(programIdList);
                                List<PostalCodeInclusion> postalCodeInclusions = this.getAvailableZipCodeInclusionsByProgramIdList(programProductIdList);


                                Entities.Directory directory = this.getDirectoryByCampaignId((int)vendorCampaign.CampaignId);


                                foreach (MatchingService.ProgramWithInstitutionCampus matchedProgram in matchedProgramList)
                                {
                                    Entities.Program currentVendorProgram = new Entities.Program();
                                    currentVendorProgram = this.serializeProgram(matchedProgram, programIdList, directoryRequest, programEducationLevels, postalCodeInclusions, vendorCampaign, directory);
                                    if (currentVendorProgram != null)
                                    {
                                        itemList.Add(currentVendorProgram);
                                        if (matchedProgram.ExternalMatchItemGuid.HasValue)
                                            RedisHelper.SetInCache(directoryRequest.APIKey + "_" + matchedProgram.ProgramId, matchedProgram.ExternalMatchItemGuid.Value.ToString() + "|" + matchedProgram.ProgramProductId + "|" + matchedProgram.RevenuePerLead);
                                    }
                                }
                                CacheStore.AddResponseDataCacheItem(cacheKey, itemList);
                            }
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);

            }

            if (itemList != null)
            {
                vendorAPIList = pageItemList(itemList, directoryRequest.StartPage, directoryRequest.PageSize);
            }

            return vendorAPIList;
        }

        public Entities.Program GetProgramDetails(DirectoryRequest directoryRequest)
        {
            Entities.Program currentVendorProgram = null;
            try
            {

                DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

                directoryMatchRequest.Application = ISApplication.VendorAPI;
                directoryMatchRequest.TrackGuid = directoryRequest.APIKey;
                directoryMatchRequest.ProgramIdList = new List<int>();
                directoryMatchRequest.ProgramIdList.Add(directoryRequest.ProgramId);
                directoryMatchRequest.CampusId = directoryRequest.CampusId;

                directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
                directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);

                if (!String.IsNullOrEmpty(directoryRequest.CampusType))
                {
                    switch (directoryRequest.CampusType.ToLower())
                    {
                        case "online":
                            directoryMatchRequest.CampusType = MatchingService.CampusType.Online;
                            break;

                        case "ground":
                            directoryMatchRequest.CampusType = MatchingService.CampusType.Ground;
                            break;
                        case "all":
                            break;

                        default:
                            break;

                    }
                }
                directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                directoryMatchRequest.RemoveInvalidEntities = true;


                ProgramResponse matchedProgramResponse = matchingServiceClient.GetPrograms(directoryMatchRequest, true);

                if (matchedProgramResponse != null)
                {
                    if (matchedProgramResponse.ProgramList != null)
                    {
                        List<MatchingService.Program> matchedProgramList = matchedProgramResponse.ProgramList;
                        if (matchedProgramList.Count > 0)
                        {
                            MatchingService.ProgramWithInstitutionCampus matchedProgram = matchedProgramList.FirstOrDefault() as MatchingService.ProgramWithInstitutionCampus;
                            List<int> programIdList = matchedProgramList.GroupBy(p => p.ProgramId).Select(grp => grp.FirstOrDefault().ProgramId).ToList();
                            Entities.VendorCampaign vendorCampaign = this.getCampaignByTrackId(directoryRequest.APIKey);
                            Entities.Directory directory = this.getDirectoryByCampaignId((int)vendorCampaign.CampaignId);
                            List<ProgramEducationLevel> programEducationLevels = this.getEducationLevelsByProgramIdList(programIdList);
                            List<PostalCodeInclusion> postalCodeInclusions = this.getAvailableZipCodeInclusionsByProgramIdList(programIdList);

                            currentVendorProgram = this.serializeProgram(matchedProgram, programIdList, directoryRequest, programEducationLevels, postalCodeInclusions, vendorCampaign, directory);
                            if (matchedProgram.ExternalMatchItemGuid.HasValue)
                                RedisHelper.SetInCache(directoryRequest.APIKey + "_" + matchedProgram.ProgramId, matchedProgram.ExternalMatchItemGuid.Value.ToString() + "|" + matchedProgram.ProgramProductId + "|" + matchedProgram.RevenuePerLead);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);

            }



            return currentVendorProgram;
        }

        private Entities.Program serializeProgram(MatchingService.ProgramWithInstitutionCampus matchedProgram, List<int> programIdList, DirectoryRequest directoryRequest, List<ProgramEducationLevel> programEducationLevels, List<PostalCodeInclusion> postalCodeInclusions, Entities.VendorCampaign vendorCampaign, Entities.Directory directory)
        {
            Entities.Program currentVendorProgram = null;
            try
            {
                string cacheKey = String.Format("{0}{1}{2}_GetProgramDetails", directoryRequest.APIKey, matchedProgram.ProgramId, matchedProgram.CampusId);

                currentVendorProgram = CacheStore.GetCacheItemByKey(cacheKey) as Entities.Program;

                if (currentVendorProgram == null)
                {
                    if (matchedProgram != null)
                    {
                        string categoryDirectoryUrl = Utilities.EddyURLs.GetDirectoryURL(vendorCampaign.ApplicationId, (int)Utilities.CampaignType.APIorDirectory, vendorCampaign.CampaignId, "CATEGORY", directory?.BaseURL, directory?.DirectoryName);
                        string subjectDirectoryUrl = Utilities.EddyURLs.GetDirectoryURL(vendorCampaign.ApplicationId, (int)Utilities.CampaignType.APIorDirectory, vendorCampaign.CampaignId, "SUBJECT", directory?.BaseURL, directory?.DirectoryName);
                        string directoryUrl = Utilities.EddyURLs.GetDirectoryURL(vendorCampaign.ApplicationId, (int)Utilities.CampaignType.APIorDirectory, vendorCampaign.CampaignId, "PROGRAM", directory?.BaseURL, directory?.DirectoryName);

                        currentVendorProgram = new Entities.Program();

                        /* Commented out till we go over GEO rules with Lea
                        /ApiProgramResponse apiProgramResponse = matchingServiceClient.GetApiProgram(directoryRequest.ProgramId, directoryRequest.APIKey);
                        if (apiProgramResponse != null)
                        {
                            //if (apiProgramResponse.States != null)
                            //{
                            //    if (apiProgramResponse.States.Count > 0)
                            //    {
                            //        List<State> programStates = this.getAllStates().Where(s => apiProgramResponse.States.Contains(s.StateId)).ToList();
                            //        currentVendorProgram.AvailableStates = programStates;
                            //    }
                            //}
                            //if (apiProgramResponse.Countries != null)
                            //{
                            //    if (apiProgramResponse.Countries.Count > 0)
                            //    {
                            //        List<Country> programCountries = this.getAllCountries().Where(c => apiProgramResponse.Countries.Contains(c.CountryId)).ToList();
                            //        currentVendorProgram.AvailableCountries = programCountries;
                            //    }
                            //}
                        }
                        */


                        Entities.ProgramInstitution currentVendorProgramInstitutionBase = new ProgramInstitution();
                        currentVendorProgramInstitutionBase.InstitutionId = matchedProgram.InstitutionId;
                        currentVendorProgramInstitutionBase.InstitutionName = matchedProgram.InstitutionName;
                        currentVendorProgramInstitutionBase.InstitutionDescription = matchedProgram.InstitutionDescription;
                        currentVendorProgramInstitutionBase.Campuses = new List<CampusBase>();
                        Entities.CampusBase currentVendorProgramCampus = new Entities.CampusBase();
                        currentVendorProgramCampus.CampusId = matchedProgram.CampusId;
                        currentVendorProgramCampus.CampusName = matchedProgram.CampusName;
                        currentVendorProgramCampus.CampusType = matchedProgram.CampusType.ToString();
                        currentVendorProgramCampus.Address = matchedProgram.CampusAddress1?.ToString();
                        currentVendorProgramCampus.City = matchedProgram.CampusCity;

                        currentVendorProgramCampus.PostalCode = matchedProgram.CampusPostalCode;

                        Entities.Country country = null;
                        Entities.State state = null;

                        country = this.getAllCountries().FirstOrDefault(c => c.CountryCode == matchedProgram.CampusCountryCode);
                        if (country != null)
                        {

                            switch (country.CountryCode.ToLower())
                            {
                                case "us":
                                case "ca":
                                    state = this.getAllStates().FirstOrDefault(s => s.StateCode == matchedProgram.CampusState && s.CountryId == country.CountryId);
                                    break;
                                default:
                                    state = this.getAllStates().FirstOrDefault(s => s.Name.Contains(matchedProgram.CampusState));
                                    break;
                            }

                        }
                        currentVendorProgramCampus.Country = country != null ? country : null;
                        currentVendorProgramCampus.State = state != null ? state : null;


                        currentVendorProgramInstitutionBase.Campuses.Add(currentVendorProgramCampus);
                        currentVendorProgramInstitutionBase.LogoURL = EddyURLs.GetLogoURL(matchedProgram.CampusLogoURL, matchedProgram.InstitutionLogoURL);

                        currentVendorProgram.Institution = currentVendorProgramInstitutionBase;

                        currentVendorProgram.ProgramId = matchedProgram.ProgramId;
                        currentVendorProgram.ProgramName = matchedProgram.ProgramName;
                        currentVendorProgram.ProgramDescription = matchedProgram.ProgramDescription;

                        Entities.Category category = getAllCategories().Where(m => m.CategoryId == (int)matchedProgram.CategoryId).FirstOrDefault();
                        if (category != null)
                        {
                            ProgramCategory programCategory = new ProgramCategory();
                            programCategory.CategoryId = category.CategoryId;
                            programCategory.CategoryName = category.CategoryName;
                            currentVendorProgram.Category = programCategory;

                            currentVendorProgram.Category.DirectoryURL = string.Format("{0}{1}", directoryUrl, matchedProgram.CategoryId);



                            Entities.Subject subCategory = getAllSubjects().FirstOrDefault(m => m.SubjectId == matchedProgram.SubjectId);
                            if (subCategory != null)
                            {
                                currentVendorProgram.Category.Subjects = new List<Entities.Subject>();



                                subCategory.DirectoryURL = string.Format("{0}{1}", directoryUrl, matchedProgram.SubjectId);
                                currentVendorProgram.Category.Subjects.Add(subCategory);


                            }
                        }




                        currentVendorProgram.LogoURL = EddyURLs.GetLogoURL(matchedProgram.CampusLogoURL, matchedProgram.InstitutionLogoURL);

                        if (!string.IsNullOrEmpty(directoryUrl))
                        {
                            currentVendorProgram.DirectoryURL = string.Format("{0}/{1}/{2}", directoryUrl, matchedProgram.InstitutionId, matchedProgram.ProgramId);
                            currentVendorProgram.FormURL = string.Format("{0}/{1}/{2}", directoryUrl, matchedProgram.InstitutionId, matchedProgram.ProgramId);
                        }

                        if (programEducationLevels != null)
                        {
                            if (programEducationLevels.Count > 0)
                            {
                                currentVendorProgram.EducationLevels = programEducationLevels.Where(e => e.ProgramId == matchedProgram.ProgramId).ToList();

                                Entities.LevelBase mappedLevel = new Entities.LevelBase();
                                mappedLevel.ProgramLevelId = matchedProgram.ProgramLevelId;
                                mappedLevel.ProgramLevelName = matchedProgram.ProgramLevelName;

                                currentVendorProgram.ProgramLevel = mappedLevel;

                            }
                        }
                        if (postalCodeInclusions != null)
                        {
                            if (postalCodeInclusions.Count > 0)
                            {
                                PostalCodeInclusion postalCodeInclusion = postalCodeInclusions.FirstOrDefault(z => z.ProgramProductId == matchedProgram.ProgramProductId);
                                if (postalCodeInclusion != null)
                                {
                                    currentVendorProgram.IsZipcodeIncluded = postalCodeInclusion.IsZipCodeInclusion;
                                }
                            }
                        }
                        if (vendorCampaign.AllowRevShareRPL)
                        {

                            if (vendorCampaign.CalculateRevShareByERPL)
                            {
                                if (matchedProgram.EffectiveRevenuePerLead.HasValue)
                                {
                                    currentVendorProgram.EstimatedRevShare = this.calculateVendorCampaignProgramRPL(matchedProgram.EffectiveRevenuePerLead.Value, vendorCampaign);
                                }
                            }
                            else
                            {
                                if (matchedProgram.RevenuePerLead.HasValue)
                                {
                                    currentVendorProgram.EstimatedRevShare = this.calculateVendorCampaignProgramRPL(matchedProgram.RevenuePerLead.Value, vendorCampaign);
                                }
                            }

                        }

                        CacheStore.AddResponseDataCacheItem(cacheKey, currentVendorProgram);
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }



            return currentVendorProgram;
        }

        public AllocationStatus CheckAllocation(DirectoryRequest directoryRequest)
        {
            AllocationStatus allocationStatus = null;
            try
            {
                string cacheKey = String.Format("{0}{1}{2}{3}_CheckAllocation", directoryRequest.APIKey, (directoryRequest.CategoryIds != null ? string.Join(",", directoryRequest.CategoryIds.ToArray()) : "noCategoryIds"), (directoryRequest.ProgramLevelIds != null ? string.Join(",", directoryRequest.ProgramLevelIds.ToArray()) : "noLevelIds"), (directoryRequest.SubjectIds != null ? string.Join(",", directoryRequest.SubjectIds.ToArray()) : "noSubjectIds"), directoryRequest.CampusType);

                allocationStatus = CacheStore.GetCacheItemByKey(cacheKey) as AllocationStatus;

                if (allocationStatus == null)
                {
                    allocationStatus = new AllocationStatus { CheckedCategoryIds = directoryRequest.CategoryIds, CheckedLevelIds = directoryRequest.ProgramLevelIds, CheckedSubjectIds = directoryRequest.SubjectIds, HasAllocation = false };

                    DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

                    directoryMatchRequest.Application = ISApplication.VendorAPI;
                    directoryMatchRequest.TrackGuid = directoryRequest.APIKey;
                    directoryMatchRequest.InstitutionIdList = new List<int>();


                    if (directoryRequest.CategoryIds != null)
                    {
                        if (directoryRequest.CategoryIds.Count > 0)
                        {
                            directoryMatchRequest.CategoryList = directoryRequest.CategoryIds;
                        }
                    }
                    if (directoryRequest.SubjectIds != null)
                    {
                        if (directoryRequest.SubjectIds.Count > 0)
                        {
                            directoryMatchRequest.SubjectList = directoryRequest.SubjectIds;
                        }
                    }
                    if (directoryRequest.ProgramLevelIds != null)
                    {
                        if (directoryRequest.ProgramLevelIds.Count > 0)
                        {
                            directoryMatchRequest.ProgramLevelList = directoryRequest.ProgramLevelIds;
                        }
                    }

                    directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
                    directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);

                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    ProgramResponse matchedProgramResponse = matchingServiceClient.GetPrograms(directoryMatchRequest, true);
                    if (matchedProgramResponse != null)
                    {
                        if (matchedProgramResponse.ProgramList != null)
                        {
                            if (matchedProgramResponse.ProgramList.Count > 0)
                            {
                                if (matchedProgramResponse.ProgramList.Any(p => p.FailedValidation == false))
                                {
                                    allocationStatus.HasAllocation = true;
                                }
                            }
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }



            return allocationStatus;
        }

        public VendorAPIList GetInstitutions(DirectoryRequest directoryRequest)
        {
            VendorAPIList vendorAPIList = null;
            List<Object> itemList = null;
            try
            {
                string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}{6}_GetInstitutions", directoryRequest.InstitutionId, directoryRequest.APIKey, (directoryRequest.CategoryIds != null ? string.Join(",", directoryRequest.CategoryIds.ToArray()) : "noCategoryIds"), (directoryRequest.ProgramLevelIds != null ? string.Join(",", directoryRequest.ProgramLevelIds.ToArray()) : "noLevelIds"), (directoryRequest.SubjectIds != null ? string.Join(",", directoryRequest.SubjectIds.ToArray()) : "noSubjectIds"), directoryRequest.CampusType, directoryRequest.PostalCode);

                itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                if (itemList == null)
                {
                    DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

                    directoryMatchRequest.Application = ISApplication.VendorAPI;
                    directoryMatchRequest.TrackGuid = directoryRequest.APIKey;

                    directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
                    directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);

                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    directoryMatchRequest.RemoveInvalidEntities = true;

                    if (directoryRequest.InstitutionId != 0)
                    {
                        directoryMatchRequest.InstitutionIdList = new List<int>();
                        directoryMatchRequest.InstitutionIdList.Add(directoryRequest.InstitutionId);
                    }
                    if (directoryRequest.CategoryIds != null)
                    {
                        if (directoryRequest.CategoryIds.Count > 0)
                        {
                            directoryMatchRequest.CategoryList = directoryRequest.CategoryIds;
                        }
                    }
                    if (directoryRequest.SubjectIds != null)
                    {
                        if (directoryRequest.SubjectIds.Count > 0)
                        {
                            directoryMatchRequest.SubjectList = directoryRequest.SubjectIds;
                        }
                    }
                    if (directoryRequest.ProgramLevelIds != null)
                    {
                        if (directoryRequest.ProgramLevelIds.Count > 0)
                        {
                            directoryMatchRequest.ProgramLevelList = directoryRequest.ProgramLevelIds;
                        }
                    }

                    if (!String.IsNullOrEmpty(directoryRequest.CampusType))
                    {
                        switch (directoryRequest.CampusType.ToLower())
                        {
                            case "online":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Online;
                                break;

                            case "ground":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Ground;
                                break;
                            case "all":
                                break;

                            default:
                                break;

                        }
                    }
                    MatchingService.ProspectInput prospectInput = null;
                    if (!String.IsNullOrEmpty(directoryRequest.PostalCode))
                    {
                        prospectInput = new MatchingService.ProspectInput();
                        prospectInput.PostalCode = directoryRequest.PostalCode;

                    }
                    directoryMatchRequest.ProspectInput = prospectInput;
                    InstitutionResponse matchedInstitutionResponse = matchingServiceClient.GetInstitutions(directoryMatchRequest, GetInstitutionCampusOption.CampusOn2ndLevel);
                    if (matchedInstitutionResponse != null)
                    {
                        if (matchedInstitutionResponse.InstitutionList != null)
                        {

                            itemList = new List<Object>();
                            List<MatchingService.Institution> matchedInstitutionList = matchedInstitutionResponse.InstitutionList;
                            if (matchedInstitutionList.Count > 0)
                            {
                                Entities.VendorCampaign vendorCampaign = this.getCampaignByTrackId(directoryRequest.APIKey);
                                Entities.Directory directory = this.getDirectoryByCampaignId((int)vendorCampaign.CampaignId);
                                string directoryUrl = Utilities.EddyURLs.GetDirectoryURL(vendorCampaign.ApplicationId, (int)Utilities.CampaignType.APIorDirectory, vendorCampaign.CampaignId, "INSTITUTION", directory?.BaseURL, directory?.DirectoryName);


                                foreach (MatchingService.Institution matchedInstitution in matchedInstitutionList)
                                {
                                    MatchingService.InstitutionWithCampus matchedInstitutionWithCampus = matchedInstitution as MatchingService.InstitutionWithCampus;

                                    Entities.Institution currentVendorInstitution = new Entities.Institution();
                                    currentVendorInstitution.InstitutionName = matchedInstitution.InstitutionName;
                                    currentVendorInstitution.InstitutionId = matchedInstitution.InstitutionId;
                                    //Entities.Institution baseVendorInstitution = getAllInstitutions().Where(s => s.InstitutionId == matchedInstitutionWithCampus.InstitutionId).FirstOrDefault();
                                    //if (baseVendorInstitution != null)
                                    //{
                                    //    //currentVendorInstitution.City = baseVendorInstitution.City;
                                    //    //currentVendorInstitution.State = baseVendorInstitution.State;
                                    //    //currentVendorInstitution.PostalCode = baseVendorInstitution.PostalCode;
                                    //    //currentVendorInstitution.Country = baseVendorInstitution.Country;
                                    //}

                                    currentVendorInstitution.InstitutionDescription = matchedInstitutionWithCampus.InstitutionDescription;

                                    currentVendorInstitution.LogoURL = EddyURLs.GetLogoURL("", matchedInstitutionWithCampus.InstitutionLogoURL);
                                    if (!string.IsNullOrEmpty(directoryUrl))
                                    {
                                        currentVendorInstitution.InstitutionFormURL = string.Format("{0}/{1}", directoryUrl, matchedInstitutionWithCampus.InstitutionId);
                                        //currentVendorInstitution.InstitutionURL = string.Format("{0}/{1}", directoryUrl, matchedInstitutionWithCampus.InstitutionId);

                                    }
                                    if (matchedInstitutionWithCampus.CampusList != null)
                                    {
                                        if (matchedInstitutionWithCampus.CampusList.Count > 0)
                                        {
                                            currentVendorInstitution.Campuses = new List<Entities.CampusBase>();
                                            foreach (MatchingService.Campus matchedCampus in matchedInstitutionWithCampus.CampusList)
                                            {
                                                Entities.CampusBase currentVendorInstitutionCampus = new Entities.CampusBase();
                                                currentVendorInstitutionCampus.Address = matchedCampus.Address1;
                                                currentVendorInstitutionCampus.CampusId = matchedCampus.CampusId;
                                                currentVendorInstitutionCampus.CampusName = matchedCampus.CampusName;
                                                currentVendorInstitutionCampus.CampusType = matchedCampus.CampusType.Value.ToString();
                                                currentVendorInstitutionCampus.City = matchedCampus.City;
                                                currentVendorInstitutionCampus.PostalCode = matchedCampus.PostalCode;
                                                //currentVendorInstitutionCampus.Institution = currentVendorInstitution;
                                                Entities.Country country = null;
                                                Entities.State state = null;
                                                country = this.getAllCountries().FirstOrDefault(c => c.CountryCode == matchedCampus.CountryCode);
                                                if (country != null)
                                                {
                                                    switch (country.CountryCode.ToLower())
                                                    {
                                                        case "us":
                                                        case "ca":
                                                            state = this.getAllStates().FirstOrDefault(s => s.StateCode == matchedCampus.State && s.CountryId == country.CountryId);
                                                            break;
                                                        default:
                                                            state = this.getAllStates().FirstOrDefault(s => s.Name.Contains(matchedCampus.State) && s.CountryId == country.CountryId);
                                                            //state = this.getAllStates().FirstOrDefault(s => s.Name == matchedCampus.State.Trim() && s.CountryId == country.CountryId);
                                                            break;
                                                    }

                                                }
                                                currentVendorInstitutionCampus.Country = country != null ? country : null;
                                                currentVendorInstitutionCampus.State = state != null ? state : null;
                                                //currentVendorInstitutionCampus.LogoURL = EddyURLs.GetLogoURL(matchedInstitutionWithCampus.InstitutionId, matchedCampus.CampusId, matchedCampus.HasCampusLogo);

                                                //MatchingService.CampusZipCodeRuleResponse zipCodeRules = this.getCampusZipCodeRuleByCampusId(currentVendorInstitutionCampus.CampusId, directoryRequest.APIKey);
                                                //if (zipCodeRules != null)
                                                //{
                                                //    if (zipCodeRules.ZipCodeList != null)
                                                //    {
                                                //        if (zipCodeRules.ZipCodeList.Count > 0)
                                                //        {
                                                //            if (zipCodeRules.ZipCodeRuleType == ZipCodeRuleType.Inclusion)
                                                //            {
                                                //                currentVendorInstitutionCampus.IncludedZips = new List<string>();
                                                //                zipCodeRules.ZipCodeList.ToList().ForEach(z => currentVendorInstitutionCampus.IncludedZips.Add(z));
                                                //            }
                                                //            else if (zipCodeRules.ZipCodeRuleType == ZipCodeRuleType.Exclusion)
                                                //            {
                                                //                currentVendorInstitutionCampus.ExcludedZips = new List<string>();
                                                //                zipCodeRules.ZipCodeList.ToList().ForEach(z => currentVendorInstitutionCampus.ExcludedZips.Add(z));
                                                //            }
                                                //        }
                                                //    }
                                                //}

                                                currentVendorInstitution.Campuses.Add(currentVendorInstitutionCampus);


                                            }
                                        }
                                    }
                                    itemList.Add(currentVendorInstitution);
                                }
                            }
                            CacheStore.AddResponseDataCacheItem(cacheKey, itemList);
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            if (itemList != null)
            {
                vendorAPIList = pageItemList(itemList, directoryRequest.StartPage, directoryRequest.PageSize);
            }

            return vendorAPIList;
        }

        //DeleteCampusesCacheForMicrosites



            public void DeleteProgramsCacheForMicrosites(DirectoryRequest directoryRequest)
            {
                List<Object> itemList = null;

                try
                {
                    string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}_GetProgramsForMicrosites", directoryRequest.APIKey, directoryRequest.InstitutionId, directoryRequest.CampusId, directoryRequest.FeatureId, directoryRequest.EducationLevelId, directoryRequest.CampusIds != null ? string.Join("|", directoryRequest.CampusIds) : "");
                    itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                    CacheStore.RemoveCacheItemByKey(cacheKey);


                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, Log.GetCurrentMethodName());
                    EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                    ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
                }

            }

            public void DeleteCampusesCacheForMicrosites(DirectoryRequest directoryRequest)
            {
                List<Object> itemList = null;

                try
                {
                    string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}_GetCampusesForMicrosites", directoryRequest.APIKey, directoryRequest.InstitutionId, directoryRequest.CampusId, directoryRequest.FeatureId, directoryRequest.EducationLevelId, directoryRequest.CampusIds != null ? string.Join("|", directoryRequest.CampusIds) : "");

                    CacheStore.RemoveCacheItemByKey(cacheKey);


                }
                catch (Exception exc)
                {
                    Logging.LogException(exc, Log.GetCurrentMethodName());
                    EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                    ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
                }

            }

        
        public VendorAPIList GetCampusesForMicrosites(DirectoryRequest directoryRequest, out List<CampusMicrosite> CampusList, bool campusListRequired)
        {
            VendorAPIList vendorAPIList = null;
            List<Object> itemList = null;
            CampusList = new List<CampusMicrosite>();

            try
            {
                string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}{6}_GetCampusesForMicrosites", directoryRequest.APIKey, directoryRequest.InstitutionId, directoryRequest.CampusId, directoryRequest.FeatureId,directoryRequest.EducationLevelId, directoryRequest.CampusIds != null ? string.Join("|", directoryRequest.CampusIds) : "", directoryRequest.ProgramDelimiter);
                itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                //Get ItemList if its not cached & set the CampusList
                if (itemList == null)
                {
                    //Creating object needed to get campus data from ME
                    DirectoryMatchRequest directoryMatchRequest = SetDirectoryMatchRequestData(directoryRequest);
                    CampusResponse matchedCampusResponse = matchingServiceClient.GetCampuses(directoryMatchRequest);

                    if (matchedCampusResponse != null)
                    {
                        if (matchedCampusResponse.CampusList != null)
                        {
                            itemList = new List<Object>();
                            List<MatchingService.CampusWithInstitution> matchedCampusList = matchedCampusResponse.CampusList;
                            if (matchedCampusList.Count > 0)
                            {
                                //Looping through each campus to create the CampusMicrosite objects each with its list of programs under each campus
                                foreach (MatchingService.CampusWithInstitution matchedCampus in matchedCampusList)
                                {

                                    Entities.CampusMicrosite currentVendorCampus = new Entities.CampusMicrosite();

                                    currentVendorCampus.CampusId = matchedCampus.CampusId;
                                    currentVendorCampus.CampusName = matchedCampus.CampusName;

                                    if (matchedCampus.ProgramList.Count > 0)
                                    {
                                        currentVendorCampus.Programs = new List<ProgramMicrosite>();

                                        foreach (var p in matchedCampus.ProgramList)
                                        {
                                            ProgramMicrosite pm = new ProgramMicrosite();

                                            pm.CategoryName = p.CategoryName;
                                            pm.OptionGroupDisplayOrder = p.CampusOptionGroupPosition;
                                            pm.OptionGroupDisplayValue = p.CampusOptionGroup;
                                            pm.ProgramDisplayValue = GetProgramValue(p.ProgramName, directoryRequest.ProgramDelimiter);
                                            pm.ProgramId = p.ProgramId;
                                            pm.SubjectName = p.SubjectName;
                                            pm.ProgramLevelId = p.ProgramLevelId;
                                            pm.ProgramLevelName = p.ProgramLevelName;
                                            pm.ProgramDelimitedValue = GetProgramDelimitedValue(p.ProgramName, directoryRequest.ProgramDelimiter);
                                            pm.startDateList = p.StartDateList;
                                            pm.tagList = p.TagList?.Select(t => new ProgramTag { TagType = t.TagType, Tag = t.Tag }).ToList();

                                            currentVendorCampus.Programs.Add(pm);
                                        }
                                    }
                                    CampusList.Add(currentVendorCampus);
                                    itemList.Add(currentVendorCampus);
                                }
                                CacheStore.AddResponseDataCacheItem(cacheKey, itemList);
                            }
                        }

                    }
                }
                //Convert if we need to set the actual CampusList and we already have the ItemList cached
                else if (campusListRequired && !CampusList.Any())
                {
                    foreach (Object obj in itemList)
                    {
                        CampusList.Add((CampusMicrosite)obj);
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            if (itemList != null)
            {
                vendorAPIList = new VendorAPIList();
                vendorAPIList.TotalItems = itemList.Count;
                vendorAPIList.ItemList = itemList;
            }

            vendorAPIList.ItemList = itemList;

            return vendorAPIList;
        }


        public VendorAPIList GetProgramsForMicrosites(DirectoryRequest directoryRequest, out List<ProgramMicrosite> ProgramList, bool programListRequired)
        {
            VendorAPIList vendorAPIList = null;
            List<Object> itemList = null;
            ProgramList = new List<ProgramMicrosite>();
            try
            {
                string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}{6}_GetProgramsForMicrosites", directoryRequest.APIKey, directoryRequest.InstitutionId, directoryRequest.CampusId, directoryRequest.FeatureId, directoryRequest.EducationLevelId, directoryRequest.CampusIds != null ? string.Join("|", directoryRequest.CampusIds) : "", directoryRequest.ProgramDelimiter);
                itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                //Get ItemList if its not cached & set the ProgramList
                if (itemList == null)              
                {
                    //Creating object needed to get campus data from ME
                    DirectoryMatchRequest directoryMatchRequest = SetDirectoryMatchRequestData(directoryRequest);
                    CampusResponse matchedCampusResponse = matchingServiceClient.GetCampuses(directoryMatchRequest);

                    if (matchedCampusResponse != null)
                    {
                        if (matchedCampusResponse.CampusList != null)
                        {
                            itemList = new List<Object>();
                            
                            //Looping through each campus to generate ProgramMicrosite objects with the campus list under each program
                            foreach (MatchingService.CampusWithInstitution matchedCampus in matchedCampusResponse.CampusList)
                            {
                                foreach (var p in matchedCampus.ProgramList)
                                {
                                    string programDisplayName = GetProgramValue(p.ProgramName, directoryRequest.ProgramDelimiter);
                                    List<ProgramMicrosite> filteredPrograms = ProgramList.Where(program => program.ProgramDisplayValue == programDisplayName).ToList();
                                    if (!filteredPrograms.Any())
                                    {
                                        ProgramMicrosite pm = new ProgramMicrosite();
                                        pm.CategoryName = p.CategoryName;
                                        pm.OptionGroupDisplayOrder = p.CampusOptionGroupPosition;
                                        pm.OptionGroupDisplayValue = p.CampusOptionGroup;
                                        pm.ProgramDisplayValue = programDisplayName;
                                        pm.ProgramId = p.ProgramId;
                                        pm.SubjectName = p.SubjectName;
                                        pm.ProgramLevelId = p.ProgramLevelId;
                                        pm.ProgramLevelName = p.ProgramLevelName;
                                        pm.ProgramDelimitedValue = GetProgramDelimitedValue(p.ProgramName, directoryRequest.ProgramDelimiter);
                                        pm.CampusList = new List<CampusMicrosite>();
                                        pm.startDateList = p.StartDateList;
                                        pm.tagList = p.TagList?.Select(t => new ProgramTag { TagType = t.TagType, Tag = t.Tag }).ToList();

                                        pm.CampusList.Add(new CampusMicrosite
                                        {
                                            CampusId = matchedCampus.CampusId,
                                            CampusName = matchedCampus.CampusName
                                        });
                                        ProgramList.Add(pm);
                                    }
                                    else
                                    {
                                        ProgramList.Where(program => program.ProgramDisplayValue == programDisplayName).FirstOrDefault().CampusList.Add(new CampusMicrosite
                                        {
                                            CampusId = matchedCampus.CampusId,
                                            CampusName = matchedCampus.CampusName
                                        });
                                    }
                                }
                            }
                            foreach (ProgramMicrosite pm in ProgramList)
                            {
                                itemList.Add(pm);
                            }

                            CacheStore.AddResponseDataCacheItem(cacheKey, itemList);                      
                        }
                    }   
                }
                //Convert if we need to set the actual ProgramList and we already have the ItemList cached
                else if(programListRequired && !ProgramList.Any())
                {
                    foreach (Object obj in itemList)
                    {
                        ProgramList.Add((ProgramMicrosite)obj);
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            if (itemList != null)
            {
                vendorAPIList = new VendorAPIList();
                vendorAPIList.TotalItems = itemList.Count;
                vendorAPIList.ItemList = itemList;
            }

            vendorAPIList.ItemList = itemList;

            return vendorAPIList;
        }

        public DirectoryMatchRequest SetDirectoryMatchRequestData(DirectoryRequest directoryRequest)
        {
            DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

            directoryMatchRequest.Application = ISApplication.VendorAPI;
            directoryMatchRequest.TrackGuid = directoryRequest.APIKey;

            directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
            directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);
            directoryMatchRequest.RemoveInvalidEntities = true;
            //directoryMatchRequest.MaxNestedProgramCount = 100;
            if (directoryRequest.EducationLevelId != 0)
                directoryMatchRequest.ProspectInput = new MatchingService.ProspectInput() { EducationLevelId = directoryRequest.EducationLevelId };

            if (directoryRequest.InstitutionId != 0)
                directoryMatchRequest.InstitutionIdList = new List<int> { directoryRequest.InstitutionId };

            if (directoryRequest.CampusId != 0)
                directoryMatchRequest.CampusId = directoryRequest.CampusId;

            if (directoryRequest.FeatureId != 0)
                directoryMatchRequest.FeatureId = directoryRequest.FeatureId;

            if (directoryRequest.CampusIds != null && directoryRequest.CampusIds.Count > 0)
                directoryMatchRequest.CampusList = new List<int>(directoryRequest.CampusIds);

            directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;
            return directoryMatchRequest;
        }

        public VendorAPIList GetCampuses(DirectoryRequest directoryRequest)
        {
            VendorAPIList vendorAPIList = null;
            List<Object> itemList = null;
            try
            {
                string cacheKey = String.Format("{0}{1}_GetCampuses", directoryRequest.APIKey, directoryRequest.CampusId);

                itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                if (itemList == null)
                {
                    DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

                    directoryMatchRequest.Application = ISApplication.VendorAPI;
                    directoryMatchRequest.TrackGuid = directoryRequest.APIKey;

                    directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
                    directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);
                    directoryMatchRequest.RemoveInvalidEntities = true;
                    if (directoryRequest.CampusId != 0)
                    {
                        directoryMatchRequest.CampusId = directoryRequest.CampusId;
                    }
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    CampusResponse matchedCampusResponse = matchingServiceClient.GetCampuses(directoryMatchRequest);
                    if (matchedCampusResponse != null)
                    {
                        if (matchedCampusResponse.CampusList != null)
                        {

                            itemList = new List<Object>();
                            List<MatchingService.CampusWithInstitution> matchedCampusList = matchedCampusResponse.CampusList;
                            if (matchedCampusList.Count > 0)
                            {
                                foreach (MatchingService.CampusWithInstitution matchedCampus in matchedCampusList)
                                {

                                    Entities.Campus currentVendorCampus = new Entities.Campus();

                                    Entities.State currentVendorCampusState = new Entities.State();

                                    Entities.Country country = null;
                                    Entities.State state = null;
                                    currentVendorCampus.Address = matchedCampus.Address1;
                                    currentVendorCampus.City = matchedCampus.City;
                                    currentVendorCampus.PostalCode = matchedCampus.PostalCode;
                                    country = this.getAllCountries().FirstOrDefault(c => c.CountryCode == matchedCampus.CountryCode);
                                    if (country != null)
                                    {
                                        state = this.getAllStates().FirstOrDefault(s => s.StateCode == matchedCampus.State && s.CountryId == country.CountryId);
                                    }

                                    InstitutionBase institutionBase = new InstitutionBase();
                                    institutionBase.InstitutionId = (int)matchedCampus.InstitutionId;
                                    institutionBase.InstitutionName = matchedCampus.InstitutionName;
                                    institutionBase.LogoURL = EddyURLs.GetLogoURL(matchedCampus.CampusLogoURL, matchedCampus.InstitutionLogoURL);
                                    institutionBase.InstitutionDescription = matchedCampus.InstitutionDescription;
                                    currentVendorCampus.Institution = institutionBase;

                                    currentVendorCampus.Country = country != null ? country : null;
                                    currentVendorCampus.State = state != null ? state : null;

                                    currentVendorCampus.CampusId = matchedCampus.CampusId;
                                    currentVendorCampus.CampusName = matchedCampus.CampusName;

                                    //currentVendorCampus.LogoURL = EddyURLs.GetLogoURL(currentVendorCampus.Institution.InstitutionId, currentVendorCampus.CampusId, matchedCampus.HasCampusLogo);
                                    currentVendorCampus.PostalCode = matchedCampus.PostalCode;
                                    currentVendorCampus.CampusType = matchedCampus.CampusType.Value.ToString();

                                    MatchingService.CampusZipCodeRuleResponse zipCodeRules = this.getCampusZipCodeRuleByCampusId(matchedCampus.CampusId, directoryRequest.APIKey);
                                    if (zipCodeRules != null)
                                    {
                                        if (zipCodeRules.ZipCodeList != null)
                                        {
                                            if (zipCodeRules.ZipCodeList.Count > 0)
                                            {
                                                if (zipCodeRules.ZipCodeRuleType == ZipCodeRuleType.Inclusion)
                                                {
                                                    currentVendorCampus.IncludedZips = new List<string>();
                                                    zipCodeRules.ZipCodeList.ToList().ForEach(z => currentVendorCampus.IncludedZips.Add(z));
                                                }
                                                else if (zipCodeRules.ZipCodeRuleType == ZipCodeRuleType.Exclusion)
                                                {
                                                    currentVendorCampus.ExcludedZips = new List<string>();
                                                    zipCodeRules.ZipCodeList.ToList().ForEach(z => currentVendorCampus.ExcludedZips.Add(z));
                                                }
                                            }
                                        }
                                    }

                                    itemList.Add(currentVendorCampus);
                                }
                                CacheStore.AddResponseDataCacheItem(cacheKey, itemList);
                            }
                        }

                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            if (itemList != null)
            {
                vendorAPIList = pageItemList(itemList, directoryRequest.StartPage, directoryRequest.PageSize);
            }

            return vendorAPIList;
        }


        private MatchingService.CampusZipCodeRuleResponse getCampusZipCodeRuleByCampusId(int campusId, Guid apiKey)
        {
            MatchingService.CampusZipCodeRuleResponse zipCodeRules = null;
            string cacheKey = String.Format("{0}{1}_ getCampusZipCodeRuleByCampusId", apiKey, campusId);

            zipCodeRules = CacheStore.GetCacheItemByKey(cacheKey) as MatchingService.CampusZipCodeRuleResponse;

            if (zipCodeRules == null)
            {
                zipCodeRules = matchingServiceClient.GetCampusZipCodeRules(campusId, apiKey);

                CacheStore.AddResponseDataCacheItem(cacheKey, zipCodeRules);
            }
            return zipCodeRules;
        }

        public VendorAPIList GetCategories(DirectoryRequest directoryRequest)
        {
            VendorAPIList vendorAPIList = null;
            List<Object> itemList = null;
            try
            {
                string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}{6}{7}_GetCategories", directoryRequest.APIKey, (directoryRequest.CategoryIds != null ? string.Join(",", directoryRequest.CategoryIds.ToArray()) : "noCategoryIds"), (directoryRequest.ProgramLevelIds != null ? string.Join(",", directoryRequest.ProgramLevelIds.ToArray()) : "noLevelIds"), (directoryRequest.SubjectIds != null ? string.Join(",", directoryRequest.SubjectIds.ToArray()) : "noSubjectIds"), directoryRequest.CampusType, directoryRequest.CampusId, directoryRequest.InstitutionId, (directoryRequest.SpecialtyIds != null ? string.Join(",", directoryRequest.SpecialtyIds.ToArray()) : "noSpecialtyIds"));

                itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                if (itemList == null)
                {
                    DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

                    directoryMatchRequest.Application = ISApplication.VendorAPI;
                    directoryMatchRequest.TrackGuid = directoryRequest.APIKey;
                    if (directoryRequest.InstitutionId != 0)
                    {
                        directoryMatchRequest.InstitutionIdList = new List<int>();
                        directoryMatchRequest.InstitutionIdList.Add(directoryRequest.InstitutionId);
                    }
                    directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
                    directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);
                    if (directoryRequest.CampusId != 0)
                    {
                        directoryMatchRequest.CampusId = directoryRequest.CampusId;
                    }
                    if (directoryRequest.ProgramLevelIds != null)
                    {
                        if (directoryRequest.ProgramLevelIds.Count > 0)
                        {
                            directoryMatchRequest.ProgramLevelList = directoryRequest.ProgramLevelIds;
                        }
                    }

                    if (!String.IsNullOrEmpty(directoryRequest.CampusType))
                    {
                        switch (directoryRequest.CampusType.ToLower())
                        {
                            case "online":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Online;
                                break;

                            case "ground":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Ground;
                                break;
                            case "all":
                                break;

                            default:
                                break;

                        }
                    }
                    MatchingService.ProspectInput prospectInput = null;
                    prospectInput = new MatchingService.ProspectInput();
                    if (!String.IsNullOrEmpty(directoryRequest.PostalCode))
                    {
                        prospectInput.PostalCode = directoryRequest.PostalCode;
                    }
                    directoryMatchRequest.ProspectInput = prospectInput;
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;
                    directoryMatchRequest.RemoveInvalidEntities = true;
                    CategoryResponse matchedCategoryResponse = matchingServiceClient.GetCategories(directoryMatchRequest);

                    if (matchedCategoryResponse != null)
                    {
                        if (matchedCategoryResponse.CategoryList != null)
                        {



                            if (matchedCategoryResponse.CategoryList.Count > 0)
                            {
                                itemList = new List<Object>();

                                Entities.VendorCampaign vendorCampaign = this.getCampaignByTrackId(directoryRequest.APIKey);

                                Entities.Directory directory = this.getDirectoryByCampaignId((int)vendorCampaign.CampaignId);

                                string directoryUrl = Utilities.EddyURLs.GetDirectoryURL(vendorCampaign.ApplicationId, (int)Utilities.CampaignType.APIorDirectory, vendorCampaign.CampaignId, "CATEGORY", directory?.BaseURL, directory?.DirectoryName);

                                foreach (MatchingService.Category matchedCategory in matchedCategoryResponse.CategoryList)
                                {

                                    Entities.Category mappedCategory = new Entities.Category();

                                    mappedCategory.CategoryId = matchedCategory.CategoryId;
                                    mappedCategory.CategoryName = matchedCategory.CategoryName;
                                    if (!string.IsNullOrEmpty(directoryUrl))
                                    {
                                        mappedCategory.DirectoryURL = string.Format("{0}{1}", directoryUrl, matchedCategory.CategoryId);

                                    }
                                    itemList.Add(mappedCategory);
                                }
                                CacheStore.AddResponseDataCacheItem(cacheKey, itemList);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            if (itemList != null)
            {
                vendorAPIList = pageItemList(itemList, directoryRequest.StartPage, directoryRequest.PageSize);
            }

            return vendorAPIList;
        }


        public VendorAPIList GetSubCategories(DirectoryRequest directoryRequest)
        {
            VendorAPIList vendorAPIList = null;
            List<Object> itemList = null;
            try
            {
                string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}_GetSubCategories", directoryRequest.APIKey, (directoryRequest.CategoryIds != null ? string.Join(",", directoryRequest.CategoryIds.ToArray()) : "noCategoryIds"), (directoryRequest.ProgramLevelIds != null ? string.Join(",", directoryRequest.ProgramLevelIds.ToArray()) : "noLevelIds"), (directoryRequest.SubjectIds != null ? string.Join(",", directoryRequest.SubjectIds.ToArray()) : "noSubjectIds"), directoryRequest.CampusType, directoryRequest.InstitutionId);

                itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                if (itemList == null)
                {
                    DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

                    directoryMatchRequest.Application = ISApplication.VendorAPI;
                    directoryMatchRequest.TrackGuid = directoryRequest.APIKey;
                    if (directoryRequest.InstitutionId != 0)
                    {
                        directoryMatchRequest.InstitutionIdList = new List<int>();
                        directoryMatchRequest.InstitutionIdList.Add(directoryRequest.InstitutionId);
                    }
                    if (directoryRequest.CategoryIds != null)
                    {
                        if (directoryRequest.CategoryIds.Count > 0)
                        {
                            directoryMatchRequest.CategoryList = directoryRequest.CategoryIds;
                        }
                    }

                    if (directoryRequest.ProgramLevelIds != null)
                    {
                        if (directoryRequest.ProgramLevelIds.Count > 0)
                        {
                            directoryMatchRequest.ProgramLevelList = directoryRequest.ProgramLevelIds;
                        }
                    }
                    if (!String.IsNullOrEmpty(directoryRequest.CampusType))
                    {
                        switch (directoryRequest.CampusType.ToLower())
                        {
                            case "online":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Online;
                                break;

                            case "ground":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Ground;
                                break;
                            case "all":
                                break;

                            default:
                                break;

                        }
                    }
                    MatchingService.ProspectInput prospectInput = null;
                    prospectInput = new MatchingService.ProspectInput();
                    if (!String.IsNullOrEmpty(directoryRequest.PostalCode))
                    {
                        prospectInput.PostalCode = directoryRequest.PostalCode;
                    }
                    directoryMatchRequest.ProspectInput = prospectInput;
                    directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
                    directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);
                    directoryMatchRequest.RemoveInvalidEntities = true;
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    SubjectResponse matchedSubjectResponse = matchingServiceClient.GetSubjects(directoryMatchRequest);
                    if (matchedSubjectResponse != null)
                    {
                        if (matchedSubjectResponse.SubjectList != null)
                        {



                            if (matchedSubjectResponse.SubjectList.Count > 0)
                            {
                                itemList = new List<Object>();
                                Entities.VendorCampaign vendorCampaign = this.getCampaignByTrackId(directoryRequest.APIKey);
                                Entities.Directory directory = this.getDirectoryByCampaignId((int)vendorCampaign.CampaignId);

                                string directoryUrl = Utilities.EddyURLs.GetDirectoryURL(vendorCampaign.ApplicationId, (int)Utilities.CampaignType.APIorDirectory, vendorCampaign.CampaignId, "SUBJECT", directory?.BaseURL, directory?.DirectoryName);

                                foreach (MatchingService.Subject matchedSubject in matchedSubjectResponse.SubjectList)
                                {
                                    Entities.Subject subject = getAllSubjects().FirstOrDefault(m => m.SubjectId == matchedSubject.SubjectId);

                                    if (!string.IsNullOrEmpty(directoryUrl))
                                    {
                                        subject.DirectoryURL = string.Format("{0}{1}", directoryUrl, matchedSubject.SubjectId);
                                        //Are there Forms for SubCategories?
                                        //mappedCategory.FormURL = string.Format("{0}{1}", directoryUrl, matchedSubject.SubjectId);
                                    }
                                    itemList.Add(subject);
                                }
                                CacheStore.AddResponseDataCacheItem(cacheKey, itemList);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            if (itemList != null)
            {
                vendorAPIList = pageItemList(itemList, directoryRequest.StartPage, directoryRequest.PageSize);
            }

            return vendorAPIList;
        }

        public VendorAPIList GetSpecialties(DirectoryRequest directoryRequest)
        {
            VendorAPIList vendorAPIList = null;
            List<Object> itemList = null;
            try
            {
                string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}_GetSpecialties", directoryRequest.APIKey, (directoryRequest.CategoryIds != null ? string.Join(",", directoryRequest.CategoryIds.ToArray()) : "noCategoryIds"), (directoryRequest.ProgramLevelIds != null ? string.Join(",", directoryRequest.ProgramLevelIds.ToArray()) : "noLevelIds"), (directoryRequest.SubjectIds != null ? string.Join(",", directoryRequest.SubjectIds.ToArray()) : "noSubjectIds"), directoryRequest.CampusType, directoryRequest.InstitutionId);

                itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                if (itemList == null)
                {
                    DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

                    directoryMatchRequest.Application = ISApplication.VendorAPI;
                    directoryMatchRequest.TrackGuid = directoryRequest.APIKey;
                    if (directoryRequest.InstitutionId != 0)
                    {
                        directoryMatchRequest.InstitutionIdList = new List<int>();
                        directoryMatchRequest.InstitutionIdList.Add(directoryRequest.InstitutionId);
                    }
                    if (directoryRequest.SubjectIds != null)
                    {
                        if (directoryRequest.SubjectIds.Count > 0)
                        {
                            directoryMatchRequest.SubjectList = directoryRequest.SubjectIds;
                        }
                    }

                    if (directoryRequest.ProgramLevelIds != null)
                    {
                        if (directoryRequest.ProgramLevelIds.Count > 0)
                        {
                            directoryMatchRequest.ProgramLevelList = directoryRequest.ProgramLevelIds;
                        }
                    }
                    if (!String.IsNullOrEmpty(directoryRequest.CampusType))
                    {
                        switch (directoryRequest.CampusType.ToLower())
                        {
                            case "online":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Online;
                                break;

                            case "ground":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Ground;
                                break;
                            case "all":
                                break;

                            default:
                                break;

                        }
                    }
                    MatchingService.ProspectInput prospectInput = null;
                    prospectInput = new MatchingService.ProspectInput();
                    if (!String.IsNullOrEmpty(directoryRequest.PostalCode))
                    {
                        prospectInput.PostalCode = directoryRequest.PostalCode;
                    }
                    directoryMatchRequest.ProspectInput = prospectInput;
                    directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
                    directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);
                    directoryMatchRequest.RemoveInvalidEntities = true;
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                    SpecialtyResponse matchedSpecialtyResponse = matchingServiceClient.GetSpecialties(directoryMatchRequest);
                    if (matchedSpecialtyResponse != null)
                    {
                        if (matchedSpecialtyResponse.SpecialtyList != null)
                        {



                            if (matchedSpecialtyResponse.SpecialtyList.Count > 0)
                            {
                                itemList = new List<Object>();
                                Entities.VendorCampaign vendorCampaign = this.getCampaignByTrackId(directoryRequest.APIKey);
                                Entities.Directory directory = this.getDirectoryByCampaignId((int)vendorCampaign.CampaignId);

                                string directoryUrl = Utilities.EddyURLs.GetDirectoryURL(vendorCampaign.ApplicationId, (int)Utilities.CampaignType.APIorDirectory, vendorCampaign.CampaignId, "SPECIALTY", directory?.BaseURL, directory?.DirectoryName);

                                foreach (MatchingService.Specialty matchedSpecialty in matchedSpecialtyResponse.SpecialtyList)
                                {
                                    Entities.Specialty specialty = getAllSpecialties().FirstOrDefault(m => m.SubjectId == matchedSpecialty.SubjectId);

                                    if (!string.IsNullOrEmpty(directoryUrl))
                                    {
                                        specialty.DirectoryURL = string.Format("{0}{1}", directoryUrl, matchedSpecialty.SpecialtyId);
                                        //Are there Forms for SubCategories?
                                        //mappedCategory.FormURL = string.Format("{0}{1}", directoryUrl, matchedSubject.SubjectId);
                                    }
                                    itemList.Add(specialty);
                                }
                                CacheStore.AddResponseDataCacheItem(cacheKey, itemList);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            if (itemList != null)
            {
                vendorAPIList = pageItemList(itemList, directoryRequest.StartPage, directoryRequest.PageSize);
            }

            return vendorAPIList;
        }

        public VendorAPIList GetLevels(DirectoryRequest directoryRequest)
        {
            VendorAPIList vendorAPIList = null;
            List<Object> itemList = null;
            try
            {
                string cacheKey = String.Format("{0}{1}{2}{3}{4}{5}_GetLevels", directoryRequest.APIKey, (directoryRequest.CategoryIds != null ? string.Join(",", directoryRequest.CategoryIds.ToArray()) : "noCategoryIds"), (directoryRequest.ProgramLevelIds != null ? string.Join(",", directoryRequest.ProgramLevelIds.ToArray()) : "noLevelIds"), (directoryRequest.SubjectIds != null ? string.Join(",", directoryRequest.SubjectIds.ToArray()) : "noSubjectIds"), directoryRequest.CampusType, directoryRequest.InstitutionId);

                itemList = CacheStore.GetCacheItemByKey(cacheKey) as List<Object>;

                if (itemList == null)
                {
                    DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

                    directoryMatchRequest.Application = ISApplication.VendorAPI;
                    directoryMatchRequest.TrackGuid = directoryRequest.APIKey;
                    if (directoryRequest.InstitutionId != 0)
                    {
                        directoryMatchRequest.InstitutionIdList = new List<int>();
                        directoryMatchRequest.InstitutionIdList.Add(directoryRequest.InstitutionId);
                    }
                    directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
                    directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);
                    if (directoryRequest.CampusId != 0)
                    {
                        directoryMatchRequest.CampusId = directoryRequest.CampusId;
                    }
                    if (directoryRequest.CategoryIds != null)
                    {
                        if (directoryRequest.CategoryIds.Count > 0)
                        {
                            directoryMatchRequest.CategoryList = directoryRequest.CategoryIds;
                        }
                    }
                    if (directoryRequest.SubjectIds != null)
                    {
                        if (directoryRequest.SubjectIds.Count > 0)
                        {
                            directoryMatchRequest.SubjectList = directoryRequest.SubjectIds;
                        }
                    }
                    if (!String.IsNullOrEmpty(directoryRequest.CampusType))
                    {
                        switch (directoryRequest.CampusType.ToLower())
                        {
                            case "online":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Online;
                                break;

                            case "ground":
                                directoryMatchRequest.CampusType = MatchingService.CampusType.Ground;
                                break;
                            case "all":
                                break;

                            default:
                                break;

                        }
                    }
                    MatchingService.ProspectInput prospectInput = null;
                    prospectInput = new MatchingService.ProspectInput();
                    if (!String.IsNullOrEmpty(directoryRequest.PostalCode))
                    {
                        prospectInput.PostalCode = directoryRequest.PostalCode;
                    }
                    directoryMatchRequest.ProspectInput = prospectInput;
                    directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;
                    directoryMatchRequest.RemoveInvalidEntities = true;
                    ProgramLevelResponse matchedProgramLevelResponse = matchingServiceClient.GetProgramLevels(directoryMatchRequest);
                    if (matchedProgramLevelResponse != null)
                    {
                        if (matchedProgramLevelResponse.ProgramLevelList != null)
                        {
                            if (matchedProgramLevelResponse.ProgramLevelList.Count > 0)
                            {
                                itemList = new List<Object>();
                                foreach (MatchingService.ProgramLevel matchedProgramLevel in matchedProgramLevelResponse.ProgramLevelList)
                                {
                                    Entities.Level mappedLevel = new Entities.Level();
                                    mappedLevel.ProgramLevelId = matchedProgramLevel.ProgramLevelId;
                                    mappedLevel.ProgramLevelName = matchedProgramLevel.ProgramLevelName;


                                    itemList.Add(mappedLevel);
                                }
                                CacheStore.AddResponseDataCacheItem(cacheKey, itemList);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            if (itemList != null)
            {
                vendorAPIList = pageItemList(itemList, directoryRequest.StartPage, directoryRequest.PageSize);
            }

            return vendorAPIList;
        }

        public string GetProgramDelimitedValue(string programName, string delimiter)
        {
            if (string.IsNullOrEmpty(delimiter))
                return string.Empty;

            string result = string.Empty;

            int dIndex = programName.IndexOf(delimiter);

            if (dIndex > -1)
                result = programName.Substring(dIndex, programName.Length - dIndex).Replace(delimiter, string.Empty).Trim();

            return result;
        }

        public string GetProgramValue(string programName, string delimiter)
        {
            if (string.IsNullOrEmpty(delimiter))
                return programName;

            string result = string.Empty;

            int dIndex = programName.IndexOf(delimiter);

            if (dIndex > -1)
                result = programName.Substring(0, dIndex).Trim();

            return result;
        }

    }
}
