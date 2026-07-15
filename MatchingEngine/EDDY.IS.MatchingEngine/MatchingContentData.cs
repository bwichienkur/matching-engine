using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.MatchingEngine.DTO;
using EDDY.IS.Base.Util;
using EDDY.IS.Core.Logging;
using EDDY.IS.MatchingEngine.DataModel;
using EDDY.IS.MatchingEngine.DataModel.Entity;
using EDDY.IS.MatchingEngine.DTO.Response;

namespace EDDY.IS.MatchingEngine
{
    public class MatchingContentData
    {
        public static Dictionary<int, ProgramDisplayGroup> GetProgramDisplayGroups()
        {
            Dictionary<int, ProgramDisplayGroup> programGroups = new Dictionary<int, ProgramDisplayGroup>();

            List<VW_Matching_ProgramDisplayGroup> dbGroups = ContentDataService.GetAllProgramDisplayGroup();

            foreach (var group in dbGroups)
            {
                programGroups.Add(group.ProgramDisplayGroupId, new ProgramDisplayGroup() { ProgramDisplayGroupId = group.ProgramDisplayGroupId, GroupName = group.ProgramDisplayGroupName, GroupDescription = group.ProgramDisplayGroupDescription, GroupShortDescription = group.ProgramDisplayGroupDescriptionShort });
            }

            return programGroups;
        }


        public static Dictionary<int, string> GetCampusPhoneNumbers()
        {
            Dictionary<int, string> campusNumbers = new Dictionary<int, string>();

            List<VW_Matching_ClientCampusRelationshipInfo> dbCampusNumbers = ContentDataService.GetAllClientCampusRelationshipInfo();

            foreach (var campusNumber in dbCampusNumbers)
            {
                if (campusNumbers.ContainsKey(campusNumber.ClientCampusRelationshipId))
                    campusNumbers[campusNumber.ClientCampusRelationshipId] = campusNumber.CallCenterPhone;
                else
                    campusNumbers.Add(campusNumber.ClientCampusRelationshipId, campusNumber.CallCenterPhone);
            }

            return campusNumbers;
        }

        //public static Dictionary<int, List<Address>> GetProgramAddresses()
        //{
        //    List<VW_Matching_ProgramAddress> dbProgramAddress = ContentDataService.GetAllProgramAddresses();

        //    Dictionary<int, List<Address>> programAddressList = new Dictionary<int, List<Address>>();

        //    if (dbProgramAddress != null && dbProgramAddress.Any())
        //    {
        //        foreach (var progAddr in dbProgramAddress)
        //        {
        //            if (!programAddressList.ContainsKey(progAddr.ProgramId))
        //                programAddressList.Add(progAddr.ProgramId, new List<Address>() { new Address { Address1 = progAddr.Address1, Address2 = progAddr.Address2, City = progAddr.City, CityId = progAddr.CityId, CountryCode = progAddr.CountryCode, CountryId = progAddr.CountryId, CountryName = progAddr.CountryName, Fax = progAddr.Fax, Phone = progAddr.Phone, PostalCode = progAddr.PostalCode, StateProvinceCode = progAddr.StateProvinceCode, StateId = progAddr.StateId } });
        //            else
        //                programAddressList[progAddr.ProgramId].Add(new Address { Address1 = progAddr.Address1, Address2 = progAddr.Address2, City = progAddr.City, CityId = progAddr.CityId, CountryCode = progAddr.CountryCode, CountryId = progAddr.CountryId, CountryName = progAddr.CountryName, Fax = progAddr.Fax, Phone = progAddr.Phone, PostalCode = progAddr.PostalCode, StateProvinceCode = progAddr.StateProvinceCode, StateId = progAddr.StateId });
        //        }
        //    }

        //    return programAddressList;
        //}

        #region Private Get Program Content
        public static List<ProgramContent> GetProgramContent()
        {
            List<ProgramContent> pc = null;

            //if (StaticSettings.IsBeta)
            //    pc = GetProgramContent(ContentDataService.GetAllProgramContentBeta);
            //else
            pc = GetProgramContentData();

            return pc;
        }

        private static List<ProgramContent> GetProgramContentData()
        {
            Task<Dictionary<int, VW_Matching_ProgramContent>> dbProgramContent = ContentDataService.GetAllProgramContentProd();
            Task<Dictionary<int, VW_Matching_ProgramShortDescription>> dbProgramShortDescriptions = ContentDataService.GetAllProgramShortDescription();
            Task<Dictionary<int, VW_Matching_ProgramLongDescription>> dbProgramLongDescriptions = ContentDataService.GetAllProgramLongDescription();
            Task<Dictionary<int, List<Address>>> dbProgramAddress = ContentDataService.GetAllProgramAddresses();
            Task<Dictionary<int, VW_Matching_SchoolLogo>> dbSchoolLogos = ContentDataService.GetAllSchoolLogos();
            Task<Dictionary<int, VW_Matching_ProgramLogo>> dbProgramlLogos = ContentDataService.GetAllProgramLogos();

            Task.WhenAll(dbProgramContent, dbProgramAddress, dbProgramShortDescriptions, dbProgramLongDescriptions, dbSchoolLogos);
            List<ProgramContent> programContentList = new List<ProgramContent>(dbProgramContent.Result.Count);

            foreach (var firstProgram in dbProgramContent.Result.Values)
            {
                ProgramContent programContent = new ProgramContent();

                programContent.ProgramId = firstProgram.ProgramId;
                programContent.ProgramName = firstProgram.ProgramName;
                programContent.ProgramDisplayName = firstProgram.ProgramDisplayName;
                programContent.DisclaimerType = ConvertDisclaimerTypeTextToEnum(firstProgram.ProgramDisclaimerType);
                programContent.DisclaimerText = firstProgram.ProgramDisclaimer;
                programContent.GradSchoolsDegreeName = firstProgram.GradSchoolsDegreeName;

                if (firstProgram.SchoolId.HasValue && dbSchoolLogos.Result.ContainsKey(firstProgram.SchoolId.Value))
                {
                    //programContent.HasSchoolLogo = dbSchoolLogos.Result[firstProgram.SchoolId.Value].HasLogo;
                    programContent.SchoolLogoURL = dbSchoolLogos.Result[firstProgram.SchoolId.Value].LogoURL;
                }
                else
                {
                    programContent.SchoolLogoURL = "";
                }
                programContent.SchoolId = firstProgram.SchoolId;

                if (dbProgramlLogos.Result.ContainsKey(firstProgram.ProgramId))
                {
                    //programContent.HasProgramLogo = true;
                    programContent.ProgramLogoURL = dbProgramlLogos.Result[firstProgram.ProgramId].LogoURL;
                }
                else
                {
                    //programContent.HasProgramLogo = false;
                    programContent.ProgramLogoURL = "";
                }




                if (dbProgramShortDescriptions.Result.ContainsKey(firstProgram.ProgramId))
                {
                    VW_Matching_ProgramShortDescription shortDesc = dbProgramShortDescriptions.Result[firstProgram.ProgramId];

                    if (shortDesc.CopyFromProgramId.HasValue)
                        programContent.ShortDescription = dbProgramShortDescriptions.Result[shortDesc.CopyFromProgramId.Value].ShortDescription;
                    else
                        programContent.ShortDescription = shortDesc.ShortDescription;
                }

                if (dbProgramLongDescriptions.Result.ContainsKey(firstProgram.ProgramId))
                {
                    VW_Matching_ProgramLongDescription longDesc = dbProgramLongDescriptions.Result[firstProgram.ProgramId];

                    if (longDesc.CopyFromProgramId.HasValue)
                        programContent.ProgramDescription = dbProgramLongDescriptions.Result[longDesc.CopyFromProgramId.Value].ProgramDescription;
                    else
                        programContent.ProgramDescription = longDesc.ProgramDescription;
                }

                if(dbProgramAddress.Result.ContainsKey(firstProgram.ProgramId))
                {
                    programContent.ProgramAddressList = new List<Address>(dbProgramAddress.Result[firstProgram.ProgramId]);
                }
                //var programAddressList = dbProgramAddress.Result.Where(pa => pa.ProgramId == firstProgram.ProgramId);
                //programContent.ProgramAddressList = new List<Address>();

                //if (programAddressList.Count() > 0)
                //{
                //    programContent.ProgramAddressList.Capacity = programAddressList.Count();
                //    foreach (var progAddr in programAddressList)
                //    {
                //        programContent.ProgramAddressList.Add(new Address() { Address1 = progAddr.Address1, Address2 = progAddr.Address2, City = progAddr.City, CityId = progAddr.CityId, CountryCode = progAddr.CountryCode, CountryId = progAddr.CountryId, CountryName = progAddr.CountryName, Fax = progAddr.Fax, Phone = progAddr.Phone, PostalCode = progAddr.PostalCode, StateProvinceCode = progAddr.StateProvinceCode, StateId = progAddr.StateId });
                //    }
                //}

                programContentList.Add(programContent);
            }

            return programContentList;
        }
        //public static List<ProgramContent> GetProgramContent()
        //{
        //    List<ProgramContent> programContentList = new List<ProgramContent>();

        //    List<VW_Matching_ProgramContent> dbProgramContent = ContentDataService.GetAllProgramContentProd();            

        //    foreach (var firstProgram in dbProgramContent)
        //    {
        //        ProgramContent programContent = new ProgramContent();

        //        programContent.ProgramId = firstProgram.ProgramId;
        //        programContent.ProgramName = firstProgram.ProgramName;
        //        programContent.DisclaimerType = ConvertDisclaimerTypeTextToEnum(firstProgram.ProgramDisclaimerType);
        //        programContent.DisclaimerText = firstProgram.ProgramDisclaimer;
        //        programContent.GradSchoolsDegreeName = firstProgram.GradSchoolsDegreeName;
        //        programContent.HasSchoolLogo = firstProgram.HasSchoolLogo;
        //        programContent.SchoolId = firstProgram.SchoolId;
        //        programContent.ShortDescription = firstProgram.ShortDescription;
        //        programContent.HasProgramLogo = firstProgram.HasProgramLogo;            

        //        if (firstProgram.ProgramDescriptionCompressed != null && firstProgram.ProgramDescriptionCompressed.Length > 0)
        //        {
        //            programContent.ProgramDescription = StringCompression.DecompressUTF8(firstProgram.ProgramDescriptionCompressed);

        //            if (!String.IsNullOrEmpty(programContent.ProgramDescription))
        //                programContent.ProgramDescription = programContent.ProgramDescription.Trim();
        //        }

        //        programContentList.Add(programContent);
        //    }

        //    return programContentList;
        //}
        #endregion

        #region Public Campus Content
        public static List<CampusContent> GetCampusContent()
        {
            List<CampusContent> campusContentList = new List<CampusContent>();

            List<VW_Matching_CampusContent> dbCampusContent = ContentDataService.GetAllCampusContentProd();

            foreach (var campus in dbCampusContent)
            {
                CampusContent campusContent = new CampusContent();

                campusContent.CampusId = campus.CampusId;
                campusContent.CampusName = campus.CampusName;
                //campusContent.HasLogo = campus.HasLogo.HasValue ? campus.HasLogo.Value : false;
                campusContent.Address1 = campus.Address1;
                campusContent.Address2 = campus.Address2;
                campusContent.City = campus.City;
                campusContent.StateProvinceCode = campus.StateProvinceCode;
                campusContent.PostalCode = campus.PostalCode != null ? campus.PostalCode.Trim() : null;
                campusContent.CountryCode = campus.CountryCode;
                campusContent.CountryName = campus.CountryName;
                campusContent.CountryId = campus.CountryId;
                campusContent.StateId = campus.StateId;
                campusContent.CityId = campus.CityId;
                campusContent.CampusFax = campus.Fax;
                campusContent.CampusPhone = campus.Phone;

                campusContent.LogoURL = campus.LogoURL;

                campusContentList.Add(campusContent);
            }

            return campusContentList;
        }
        #endregion

        #region Public Institution Content
        public static Dictionary<int, InstitutionContent> GetInstitutionContent()
        {
            Dictionary<int, InstitutionContent> institutionContentDict = new Dictionary<int, InstitutionContent>();

            List<VW_Matching_InstitutionContentDetail> dbInstitutuionContent = ContentDataService.GetAllInstitutionContentDetailProd();

            var groupedByInstitutionId = dbInstitutuionContent.GroupBy(i => i.InstitutionId);

            foreach (var institutionIdGroup in groupedByInstitutionId)
            {
                var firstInstitution = institutionIdGroup.First();
                InstitutionContent instContent = new InstitutionContent();

                instContent.InstitutionId = firstInstitution.InstitutionId;
                instContent.InstitutionName = firstInstitution.InstitutionName;
                instContent.DisclaimerType = ConvertDisclaimerTypeTextToEnum(firstInstitution.InstitutionDisclaimerType);
                instContent.DisclaimerText = firstInstitution.InstitutionDisclaimer;
                instContent.Address1 = firstInstitution.Address1;
                instContent.Address2 = firstInstitution.Address2;
                instContent.City = firstInstitution.City;
                instContent.State = firstInstitution.StateProvinceCode;
                instContent.Country = firstInstitution.Country;
                instContent.Zip = firstInstitution.PostalCode;
                instContent.CarnegieClassification = firstInstitution.CarnegieClassificationName;
                instContent.CalendarTypeOnline = firstInstitution.CalendarTypeOnline;
                instContent.LearningFormatOnline = firstInstitution.LearningFormatOnline;
                instContent.GeographicComments = firstInstitution.GeographicComments;
                //instContent.InstitutionType = firstInstitution.InstitutionType;
                instContent.InstitutionalAccreditationType = firstInstitution.InstitutionalAccreditationType;
                instContent.LoansOffered = firstInstitution.LoansOffered;
                instContent.OnCampusRequirements = firstInstitution.OnCampusRequirements;
                instContent.ScholarshipOffered = firstInstitution.ScholarshipOffered;
                instContent.RegistrationDetails = firstInstitution.RegistrationDetails;
                instContent.ComputerRequirement = firstInstitution.ComputerRequirement;
                instContent.TotalEnrollment = firstInstitution.TotalEnrollment;
                instContent.ThirdPartyContactAllowed = firstInstitution.ThirdPartyContactAllowed;
                instContent.Locale = firstInstitution.Locale;
                instContent.AcademicYearBeginDay = firstInstitution.AcademicYearBeginDay;
                instContent.AcademicYearBeginMonth = firstInstitution.AcademicYearBeginMonth;
                instContent.AccreditationOrganization = firstInstitution.AccreditationOrganization;
                instContent.IsFaithBased = firstInstitution.IsFaithBased;
                instContent.HasAdditionalScript = firstInstitution.HasAdditionalScript;

                instContent.InstitutionDescription = firstInstitution.Description;
                instContent.InstitutionDescriptionInternational = firstInstitution.InternationalDescription;

                instContent.LogoURL = firstInstitution.LogoURL;
                

                if (!institutionContentDict.ContainsKey(instContent.InstitutionId))
                {
                    institutionContentDict.Add(instContent.InstitutionId, instContent);
                }
            }

            return institutionContentDict;
        }

        //private static Dictionary<int, InstitutionContent> GetInstitutionContentBeta()
        //{
        //    Dictionary<int, InstitutionContent> institutionContentDict = new Dictionary<int, InstitutionContent>();

        //    List<VW_Matching_InstitutionContentDetail> dbInstitutuionContent = ContentDataService.GetAllInstitutionContentDetailBeta();

        //    var groupedByInstitutionId = dbInstitutuionContent.GroupBy(i => i.InstitutionId);

        //    foreach (var institutionIdGroup in groupedByInstitutionId)
        //    {
        //        var firstInstitution = institutionIdGroup.First();
        //        InstitutionContent instContent = new InstitutionContent();

        //        instContent.InstitutionId = firstInstitution.InstitutionId;
        //        instContent.InstitutionName = firstInstitution.InstitutionName;
        //        instContent.DisclaimerType = ConvertDisclaimerTypeTextToEnum(firstInstitution.InstitutionDisclaimerType);
        //        instContent.DisclaimerText = firstInstitution.InstitutionDisclaimer;
        //        instContent.Address1 = firstInstitution.Address1;
        //        instContent.Address2 = firstInstitution.Address2;
        //        instContent.City = firstInstitution.City;
        //        instContent.State = firstInstitution.StateProvinceCode;
        //        instContent.Country = firstInstitution.Country;
        //        instContent.Zip = firstInstitution.PostalCode;
        //        instContent.CarnegieClassification = firstInstitution.CarnegieClassificationName;
        //        instContent.CalendarTypeOnline = firstInstitution.CalendarTypeOnline;
        //        instContent.LearningFormatOnline = firstInstitution.LearningFormatOnline;
        //        instContent.GeographicComments = firstInstitution.GeographicComments;
        //        instContent.InstitutionType = firstInstitution.InstitutionType;
        //        instContent.InstitutionalAccreditationType = firstInstitution.InstitutionalAccreditationType;
        //        instContent.LoansOffered = firstInstitution.LoansOffered;
        //        instContent.OnCampusRequirements = firstInstitution.OnCampusRequirements;
        //        instContent.ScholarshipOffered = firstInstitution.ScholarshipOffered;
        //        instContent.RegistrationDetails = firstInstitution.RegistrationDetails;
        //        instContent.ComputerRequirement = firstInstitution.ComputerRequirement;
        //        instContent.TotalEnrollment = firstInstitution.TotalEnrollment;
        //        instContent.ThirdPartyContactAllowed = firstInstitution.ThirdPartyContactAllowed;
        //        instContent.Locale = firstInstitution.Locale;
        //        instContent.AcademicYearBeginMonth = firstInstitution.AcademicYearBeginMonth;
        //        instContent.AcademicYearBeginDay = firstInstitution.AcademicYearBeginDay;

        //        instContent.InstitutionDescription = firstInstitution.Description;
        //        instContent.InstitutionDescriptionInternational = firstInstitution.InternationalDescription;

        //        if (!institutionContentDict.ContainsKey(instContent.InstitutionId))
        //        {
        //            institutionContentDict.Add(instContent.InstitutionId, instContent);
        //        }
        //    }

        //    return institutionContentDict;
        //}

        #endregion

        public static List<ProgramLevelContent> GetProgramLevelContent()
        {
            List<ProgramLevelContent> programLevelList = new List<ProgramLevelContent>();

            List<VW_Matching_ProgramLevelContent> dbProgramLevelContent = ContentDataService.GetAllProgramLevelContent();

            foreach (var programLevelItem in dbProgramLevelContent)
            {
                programLevelList.Add(new ProgramLevelContent() { ProgramLevelId = programLevelItem.ProgramLevelId, ProgramLevelName = programLevelItem.ProgramLevelName });
            }

            return programLevelList;
        }

        public static List<SpecialtyContent> GetSpecialtyContent()
        {
            List<SpecialtyContent> specialtyList = new List<SpecialtyContent>();

            List<VW_Matching_SpecialtyContent> dbSpecialtyContent = ContentDataService.GetAllSpecialtyContent();

            foreach (var specialtyItem in dbSpecialtyContent)
            {
                specialtyList.Add(new SpecialtyContent() { SpecialtyId = specialtyItem.SpecialtyId, SpecialtyName = specialtyItem.SpecialtyName });
            }

            return specialtyList;
        }

        public static List<SubjectContent> GetSubjectContent()
        {
            List<SubjectContent> subjectList = new List<SubjectContent>();

            List<VW_Matching_SubjectContent> dbSubjectContent = ContentDataService.GetAllSubjectContent();

            foreach (var subjectItem in dbSubjectContent)
            {
                subjectList.Add(new SubjectContent() { SubjectId = subjectItem.SubjectId, SubjectName = subjectItem.SubjectName });
            }

            return subjectList;
        }

        public static List<CategoryContent> GetCategoryContent()
        {
            List<CategoryContent> categoryList = new List<CategoryContent>();

            List<VW_Matching_CategoryContent> dbCategoryContent = ContentDataService.GetAllCategoryContent();

            foreach (var categoryItem in dbCategoryContent)
            {
                categoryList.Add(new CategoryContent() { CategoryId = categoryItem.CategoryId, CategoryName = categoryItem.CategoryName });
            }

            return categoryList;
        }

		public static Dictionary<int, VW_Matching_CampusOptionGroupContent> GetCampusOptionGroupContent()
		{
			Dictionary<int, VW_Matching_CampusOptionGroupContent> campusOptionGroups = new Dictionary<int, VW_Matching_CampusOptionGroupContent>();

			List<VW_Matching_CampusOptionGroupContent> dbCampupsOptionGroupContent = ContentDataService.GetAllCampusOptionGroupContent();

			foreach (var campusOptionGroupItem in dbCampupsOptionGroupContent)
			{
				campusOptionGroups.Add(campusOptionGroupItem.CampusOptionGroupId, campusOptionGroupItem);
			}

			return campusOptionGroups;
		}
        public static Dictionary<EntityMeta, Dictionary<int, List<Image>>> GetImageContent()
        {
            Dictionary<EntityMeta, Dictionary<int, List<Image>>> imageDict = new Dictionary<EntityMeta, Dictionary<int, List<Image>>>();

            List<VW_Matching_Image> dbImageContent = ContentDataService.GetAllImageContent();

            var groupedByMeta = dbImageContent.GroupBy(i => i.EntityMetaId);

            foreach (var metaGroup in groupedByMeta)
            {
                Dictionary<int, List<Image>> imageMetaList = new Dictionary<int, List<Image>>();

                foreach(var imageItem in metaGroup)
                {
                    if (imageMetaList.ContainsKey(imageItem.EntityId))
                        imageMetaList[imageItem.EntityId].Add(new Image() { ImageType = (ImageType)imageItem.ImagesVideoLogoTypeId, FileUrl = imageItem.FileURL, Length = imageItem.Length, Breadth = imageItem.Breadth });
                    else
                        imageMetaList.Add(imageItem.EntityId, new List<Image> { new Image() { ImageType = (ImageType)imageItem.ImagesVideoLogoTypeId, FileUrl = imageItem.FileURL, Length = imageItem.Length, Breadth = imageItem.Breadth } });
                }

                imageDict.Add((EntityMeta)metaGroup.First().EntityMetaId, imageMetaList);
            }

            return imageDict;
        }
        
        public static DisclaimerType? ConvertDisclaimerTypeTextToEnum(string disclaimerType)
        {
            DisclaimerType? finalDisclaimerType = null;

            if (!String.IsNullOrWhiteSpace(disclaimerType))
            {
                switch (disclaimerType.Trim().ToLower())
                {
                    case "link":
                        finalDisclaimerType = DisclaimerType.Link;
                        break;
                    case "text":
                        finalDisclaimerType = DisclaimerType.Text;
                        break;
                    default:
                        break;
                }
            }

            return finalDisclaimerType;
        }

        public static List<MatchItemInternal> JoinContentData(List<MatchItemInternal> matchItemList)
        {
            List<CampusContent> campusContent = StaticCacheProxyHost.CacheProxy.Get<List<CampusContent>>(MatchingCacheItem.CampusContent);
            Dictionary<int, InstitutionContent> institutionContent = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, InstitutionContent>>(MatchingCacheItem.InstitutionContent);
            List<ProgramContent> programContent = StaticCacheProxyHost.CacheProxy.Get<List<ProgramContent>>(MatchingCacheItem.ProgramContent);
            List<ProgramLevelContent> programLevelContent = StaticCacheProxyHost.CacheProxy.Get<List<ProgramLevelContent>>(MatchingCacheItem.ProgramLevelContent);
            List<SubjectContent> subjectContent = StaticCacheProxyHost.CacheProxy.Get<List<SubjectContent>>(MatchingCacheItem.SubjectContent);
            List<CategoryContent> categoryContent = StaticCacheProxyHost.CacheProxy.Get<List<CategoryContent>>(MatchingCacheItem.CategoryContent);
            List<SpecialtyContent> specialtyContent = StaticCacheProxyHost.CacheProxy.Get<List<SpecialtyContent>>(MatchingCacheItem.SpecialtyContent);
            Dictionary<int, string> campusNumbers = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, string>>(MatchingCacheItem.CampusNumbersContent);
            Dictionary<EntityMeta, Dictionary<int, List<Image>>> images = StaticCacheProxyHost.CacheProxy.Get<Dictionary<EntityMeta, Dictionary<int, List<Image>>>>(MatchingCacheItem.Images);
            Dictionary<int, List<DateTime>> iStartDates = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<DateTime>>>(MatchingCacheItem.InstitutionStartDate);
            Dictionary<int, List<DateTime>> pStartDates = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<DateTime>>>(MatchingCacheItem.ProgramStartDate);
            Dictionary<int, List<ProgramTag>> programTags = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<ProgramTag>>>(MatchingCacheItem.ProgramTag);
            //Dictionary<int, string> campusOptionGroups = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, string>>(MatchingCacheItem.CampusOptionGroups);
            //Dictionary<int, List<Address>> programAddressContent = StaticCacheProxyHost.CacheProxy.Get<Dictionary<int, List<Address>>>(MatchingCacheItem.ProgramAddresses);

            //if (images != null)
            //{
            //    if(!images.TryGetValue(EntityMeta.Program, out programImages))
            //        programImages = new List<KeyValuePair<int,Image>>();

            //    if(!images.TryGetValue(EntityMeta.Campus, out campusImages))
            //        campusImages = new List<KeyValuePair<int, Image>>();

            //    if(!images.TryGetValue(EntityMeta.Institution, out instImages))
            //        instImages = new List<KeyValuePair<int, Image>>();
            //}

            bool cacheLoadFailed = false;

            if (campusNumbers != null)
            {
                var pairedCampuses = (from mi in matchItemList
                                      join cn in campusNumbers
                                      on mi.ClientCampusRelationshipId equals cn.Key
                                      select new { mi, cn.Value }).ToList();

                pairedCampuses.ForEach(pc =>
                {
                    pc.mi.PhoneNumber = pc.Value;
                });
            }

            if (campusContent != null)
            {
                var pairedCampuses = (from mi in matchItemList
                                      join cc in campusContent
                                      on mi.CampusId equals cc.CampusId
                                      select new { mi, cc }).ToList();

                pairedCampuses.ForEach(pc =>
                {
                    pc.mi.CampusName = pc.cc.CampusName;
                    pc.mi.CampusAddress1 = pc.cc.Address1;
                    pc.mi.CampusAddress2 = pc.cc.Address2;
                    pc.mi.CampusCity = pc.cc.City;
                    pc.mi.CampusStateCode = pc.cc.StateProvinceCode;
                    pc.mi.CampusPostalCode = pc.cc.PostalCode;
                    pc.mi.CampusCountry = pc.cc.CountryCode;
                    pc.mi.CampusCountryName = pc.cc.CountryName;

                    //pc.mi.HasCampusLogo = pc.cc.HasLogo;
                    pc.mi.CampusLogoURL = pc.cc.LogoURL;

                    pc.mi.CampusCountryId = pc.cc.CountryId;
                    pc.mi.CampusStateId = pc.cc.StateId;
                    pc.mi.CampusCityId = pc.cc.CityId;
                    pc.mi.CampusFax = pc.cc.CampusFax;
                    pc.mi.CampusPhone = pc.cc.CampusPhone;

                    if (images.ContainsKey(EntityMeta.Campus) && images[EntityMeta.Campus].ContainsKey(pc.mi.CampusId))
                        //pc.mi.CampusImageList = new List<Image>(campusSpecificImages.Select(ci => ci.Value));
                        pc.mi.CampusImageList = new List<Image>(images[EntityMeta.Campus][pc.mi.CampusId]);

                });
            }
            else
            {
                cacheLoadFailed = true;
                Exception ex = new Exception("JoinContentData - CampusContent is null");
                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            if (institutionContent != null)
            {
                var pairedInstitutions = (from mi in matchItemList
                                          join ic in institutionContent
                                          on mi.InstitutionId equals ic.Value.InstitutionId
                                          select new { mi, ic }).ToList();

                pairedInstitutions.ForEach(pi =>
                {
                pi.mi.InstitutionName = pi.ic.Value.InstitutionName;
                pi.mi.InstitutionDisclaimerType = pi.ic.Value.DisclaimerType;
                pi.mi.InstitutionDisclaimer = pi.ic.Value.DisclaimerText;
                pi.mi.InstitutionDescription = pi.ic.Value.InstitutionDescription;
                pi.mi.InstitutionDescriptionInternational = pi.ic.Value.InstitutionDescriptionInternational;
                pi.mi.IsFaithBased = pi.ic.Value.IsFaithBased;
                pi.mi.HasAdditionalScript = pi.ic.Value.HasAdditionalScript;
                pi.mi.AccreditationOrganization = pi.ic.Value.AccreditationOrganization;
                pi.mi.InstitutionLogoURL = pi.ic.Value.LogoURL; // Is this needed?

                if(images.ContainsKey(EntityMeta.Institution) && images[EntityMeta.Institution].ContainsKey(pi.mi.InstitutionId))
                    pi.mi.InstitutionImageList = new List<Image>(images[EntityMeta.Institution][pi.mi.InstitutionId]);

                if(iStartDates.ContainsKey(pi.mi.InstitutionId))
                    pi.mi.InstitutionStartDateList = iStartDates[pi.mi.InstitutionId].OrderBy(st=>st).Select(st=>st.ToString("MM/dd/yyyy")).ToList();

                });
            }
            else
            {
                cacheLoadFailed = true;
                Exception ex = new Exception("JoinContentData - InstitutionContent is null");
                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            if (programContent != null)
            {
                var pairedPrograms = (from mi in matchItemList
                                      join pc in programContent
                                      on mi.ProgramId equals pc.ProgramId
                                      select new { mi, pc }).ToList();

                pairedPrograms.ForEach(pi =>
                {
                    pi.mi.ProgramName = pi.pc.ProgramName;
                    pi.mi.ProgramDisplayName = pi.pc.ProgramDisplayName;
                    pi.mi.ProgramDisclaimerType = pi.pc.DisclaimerType;
                    pi.mi.ProgramDisclaimer = pi.pc.DisclaimerText;
                    pi.mi.ProgramDescription = pi.pc.ProgramDescription;
                    pi.mi.ProgramShortDescription = pi.pc.ShortDescription;
                    pi.mi.GradSchoolsDegreeName = pi.pc.GradSchoolsDegreeName;
                    //pi.mi.HasSchoolLogo = pi.pc.HasSchoolLogo;
                    pi.mi.SchoolLogoURL = pi.pc.SchoolLogoURL;
                    pi.mi.SchoolId = pi.pc.SchoolId;
                    //pi.mi.HasProgramLogo = pi.pc.HasProgramLogo;
                    pi.mi.ProgramLogoURL = pi.pc.ProgramLogoURL;
                    //pi.mi.ProgramAddressList = pi.pc.ProgramAddressList;

                    //var progSpecificImages = programImages.Where(c => c.Key == pi.mi.ProgramId);

                    //if (progSpecificImages != null && progSpecificImages.Any())
                    //    pi.mi.ProgramImageList = new List<Image>(progSpecificImages.Select(ci => ci.Value));
                    if (images.ContainsKey(EntityMeta.Program) && images[EntityMeta.Program].ContainsKey(pi.mi.ProgramId))
                        //pc.mi.CampusImageList = new List<Image>(campusSpecificImages.Select(ci => ci.Value));
                        pi.mi.ProgramImageList = new List<Image>(images[EntityMeta.Program][pi.mi.ProgramId]);

                    if (pStartDates.ContainsKey(pi.mi.ProgramId))
                        pi.mi.ProgramStartDateList = pStartDates[pi.mi.ProgramId].OrderBy(st => st).Select(st => st.ToString("MM/dd/yyyy")).ToList();

                    if (programTags.ContainsKey(pi.mi.ProgramId))
                        pi.mi.ProgramTagList = new List<ProgramTag>(programTags[pi.mi.ProgramId]);

                    //var programAddress = programAddressContent.Where(a => a.Key == pi.mi.ProgramId);
                    //List<Address> programAddress = null;
                    //if (programAddressContent.TryGetValue(pi.mi.ProgramId, out programAddress))
                    //{
                    //    if (programAddress != null && programAddress.Any())
                    //        pi.mi.ProgramAddressList = new List<Address>(programAddress);
                    //else
                    //    pi.mi.ProgramAddressList = new List<Address>();
                    //}\
                    if (pi.pc.ProgramAddressList != null && pi.pc.ProgramAddressList.Any())
                        pi.mi.ProgramAddressList = new List<Address>(pi.pc.ProgramAddressList);
                });
            }
            else
            {
                cacheLoadFailed = true;
                Exception ex = new Exception("JoinContentData - ProgramContent is null");
                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            if (programLevelContent != null)
            {
                var pairedProgramLevels = (from mi in matchItemList
                                           join plc in programLevelContent
                                           on mi.ProgramLevelId equals plc.ProgramLevelId
                                           select new { mi, plc }).ToList();

                pairedProgramLevels.ForEach(pi => pi.mi.ProgramLevelName = pi.plc.ProgramLevelName);
            }
            else
            {
                cacheLoadFailed = true;
                Exception ex = new Exception("JoinContentData - ProgramLevelContent is null");
                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            if (subjectContent != null)
            {
                var pairedSubjects = (from mi in matchItemList
                                      join sc in subjectContent
                                      on mi.PrimarySubjectId equals sc.SubjectId
                                      select new { mi, sc }).ToList();

                pairedSubjects.ForEach(pi => pi.mi.SubjectName = pi.sc.SubjectName);
            }
            else
            {
                cacheLoadFailed = true;
                Exception ex = new Exception("JoinContentData - SubjectContent is null");
                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            if (categoryContent != null)
            {
                var pairedCategories = (from mi in matchItemList
                                        join cc in categoryContent
                                        on mi.PrimaryCategoryId equals cc.CategoryId
                                        select new { mi, cc }).ToList();

                pairedCategories.ForEach(pi => pi.mi.CategoryName = pi.cc.CategoryName);
            }
            else
            {
                cacheLoadFailed = true;
                Exception ex = new Exception("JoinContentData - CategoryContent is null");
                ISException isEx = new ISException(ex);
                isEx.Save();
            }

            ///*******************************************************
            // * Hack to make sure there is specialty content
            // *******************************************************/
            //if (specialtyContent == null)
            //{
            //    //Refresh specialty content
            //    StaticCacheProxyHost.CacheProxy.RemoveItem(MatchingCacheItem.SpecialtyContent);

            //    //Retrieve specialty content after refresh
            //    specialtyContent = StaticCacheProxyHost.CacheProxy.Get<List<SpecialtyContent>>(MatchingCacheItem.SpecialtyContent);

            //    //Log an exception that we had to refresh specialty content
            //    Exception ex = new Exception("JoinContentData - Refreshing Specialty Content");
            //    ISException isEx = new ISException(ex);
            //    isEx.Save();
            //}

            //if (specialtyContent != null)
            //{
            //    var pairedSpecialties = (from mi in matchItemList
            //                             join sc in specialtyContent
            //                             on mi.SpecialtyId equals sc.SpecialtyId
            //                             select new { mi, sc }).ToList();

            //    pairedSpecialties.ForEach(pi => pi.mi.SpecialtyName = pi.sc.SpecialtyName);
            //}
            //else
            //{
            //    cacheLoadFailed = true;
            //    Exception ex = new Exception("JoinContentData - SpecialtyContent is null");
            //    ISException isEx = new ISException(ex);
            //    isEx.Save();
            //}

            if (cacheLoadFailed)
                throw new Exception("MatchingContentData.JoinContentData cache load failed");

            return matchItemList;
        }
    }

    public class ProgramContent
    {
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string ProgramDisplayName { get; set; }
        public DisclaimerType? DisclaimerType { get; set; }
        public string DisclaimerText { get; set; }
        //public Dictionary<int, string> ApplicationProgramDescription { get; set; }
        public string ProgramDescription { get; set; }
        public string ShortDescription { get; set; }
        public string GradSchoolsDegreeName { get; set; }
        //public bool HasSchoolLogo { get; set; }
        public string SchoolLogoURL { get; set; }
        public int? SchoolId { get; set; }
        //public bool HasProgramLogo { get; set; }
        public string ProgramLogoURL { get; set; }
        //public List<Address> ProgramAddressList { get; set; }
        public List<Address> ProgramAddressList { get; set; }
        //public List<string> StartDateList { get; set; }
    }

    public class InstitutionContent
    {
        public int InstitutionId { get; set; }
        public string InstitutionName { get; set; }
        public string InstitutionDescription { get; set; }
        public string InstitutionDescriptionInternational { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Zip { get; set; }
        public string CarnegieClassification { get; set; }
        public string CalendarTypeOnline { get; set; }
        public string LearningFormatOnline { get; set; }
        public string GeographicComments { get; set; }
        //public string InstitutionType { get; set; }
        public string InstitutionalAccreditationType { get; set; }
        public string LoansOffered { get; set; }
        public string OnCampusRequirements { get; set; }
        public string ScholarshipOffered { get; set; }
        public string RegistrationDetails { get; set; }
        public string ComputerRequirement { get; set; }
        public string TotalEnrollment { get; set; }
        public DateTime? DateTotalEnrollmentAsOf { get; set; }
        public DisclaimerType? DisclaimerType { get; set; }
        public string DisclaimerText { get; set; }
        //public Dictionary<int, InstitutionDescription> ApplicationInstitutionDescription { get; set; }
        //public InstitutionDescription InstitutionDescription { get; set; }
        public bool ThirdPartyContactAllowed { get; set; }
        public string Locale { get; set; }
        public string AcademicYearBeginMonth { get; set; }
        public string AcademicYearBeginDay { get; set; }
        public string AccreditationOrganization { get; set; }
        public bool IsFaithBased { get; set; }
        public bool HasAdditionalScript { get; set; }
        public string AdHeader { get; set; }
        public string AdDescription { get; set; }
        public string LogoURL { get; set; }
        //public List<string> InstitutionStartDateList { get; set; }

        public InstitutionDetail CreateDetail()
        {
            InstitutionDetail id = new InstitutionDetail();
            id.Address1 = this.Address1;
            id.Address2 = this.Address2;
            id.CalendarTypeOnline = this.CalendarTypeOnline;
            id.CarnegieClassification = this.CarnegieClassification;
            id.City = this.City;
            id.ComputerRequirement = this.ComputerRequirement;
            id.Country = this.Country;
            id.GeographicComments = this.GeographicComments;
            id.InstitutionalAccreditationType = this.InstitutionalAccreditationType;
            id.InstitutionDescription = this.InstitutionDescription;
            id.InstitutionDescriptionInternational = this.InstitutionDescriptionInternational;
            id.InstitutionDisclaimer = this.DisclaimerText;
            id.InstitutionDisclaimerType = this.DisclaimerType;
            id.InstitutionId = this.InstitutionId;
            id.InstitutionName = this.InstitutionName;
            //id.InstitutionType = this.InstitutionType;
            id.LearningFormatOnline = this.LearningFormatOnline;
            id.LoansOffered = this.LoansOffered;
            id.OnCampusRequirements = this.OnCampusRequirements;
            id.RegistrationDetails = this.RegistrationDetails;
            id.ScholarshipOffered = this.ScholarshipOffered;
            id.State = this.State;
            id.TotalEnrollment = this.TotalEnrollment;
            id.Zip = this.Zip;
            id.Locale = this.Locale;
            id.AcademicYearBeginDay = this.AcademicYearBeginDay;
            id.AcademicYearBeginMonth = this.AcademicYearBeginMonth;
            id.AccreditationOrganization = this.AccreditationOrganization;
            id.AdHeader = this.AdHeader;
            id.AdDescription = this.AdDescription;
            id.InstitutionLogoURL = this.LogoURL;
            //id.StartDateList = this.InstitutionStartDateList;

            return id;
        }
    }

    //public class InstitutionDescription
    //{
    //    public string Online { get; set; }
    //    public string Ground { get; set; }
    //}

    public class CampusContent
    {
        public int CampusId { get; set; }
        public string CampusName { get; set; }
        //public bool HasLogo { get; set; }
        public string LogoURL { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public int? CountryId { get; set; }
        public int? StateId { get; set; }
        public int? CityId { get; set; }
        public string CampusPhone { get; set; }
        public string CampusFax { get; set; }
    }

    public class ProgramAddress
    {
        public int ProgramId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string StateProvinceCode { get; set; }
        public string PostalCode { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Phone { get; set; }
        public string Fax { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
    }

    public class ProgramLevelContent
    {
        public int ProgramLevelId { get; set; }
        public string ProgramLevelName { get; set; }
    }

    public class SubjectContent
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
    }

    public class CategoryContent
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
    }

    public class SpecialtyContent
    {
        public int SpecialtyId { get; set; }
        public string SpecialtyName { get; set; }
    }

    public class ProgramDisplayGroup
    {
        public int ProgramDisplayGroupId { get; set; }
        public string GroupName { get; set; }
        public string GroupDescription { get; set; }
        public string GroupShortDescription { get; set; }
    }
}
