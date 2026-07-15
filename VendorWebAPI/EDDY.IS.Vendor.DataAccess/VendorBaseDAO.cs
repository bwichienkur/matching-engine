using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using EDDY.IS.Common.Logging;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Vendor.DataAccess.DataModels;
using EDDY.IS.Vendor.Utilities;
using System.Data.SqlClient;
using AutoMapper;
using System.Runtime.CompilerServices;
using EDDY.IS.Vendor.DataAccess.FormsEngineService;
using System.Xml;
using System.Net;
using System.Text;
using System.Configuration;
using Newtonsoft.Json;
using EDDY.IS.Vendor.DataAccess.MatchingService;
using System.Data.Entity;
using System.Diagnostics;

namespace EDDY.IS.Vendor.DataAccess
{
    public class VendorBaseDAO
    {
        public void LoadSupportingCache()
        {
            this.getAllVendors();
            this.getAllCategories();
            this.getAllStates();
            this.getAllCountries();
            this.getAllSubjects();
            this.getAllSpecialties();
            this.getAllProgramLevels();
            this.getAllEnabledInstitutions();
            this.getAllEnabledPrograms();
            this.getAllAdvisors();
            this.getAllUsers();
            this.getAllProspectStatuses();
            this.getAllEnabledCampuses();
            this.getAllUSPostalCodes();
            this.getAllCanadianPostalCodes();
            this.getAllEnabledMilitaryStatuses();
            this.getAllTCPAMessages();
            this.getAllZipCodes();
            CacheStore.CacheUpdated += CacheStore_CacheUpdated;
        }

        private void CacheStore_CacheUpdated(object sender, CacheItemUpdateEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.CacheKey) && !string.IsNullOrEmpty(e.RemovedReason))
            {
                if (e.RemovedReason.ToLower() == "expired")
                {
                    String[] cacheRefreshArguments = e.CacheKey.Split('_');
                    if (cacheRefreshArguments.Length > 0)
                    {
                        switch (cacheRefreshArguments[0])
                        {
                            case "VendorApi.CampaignLookupByTrackId":
                                Guid trackId;
                                if (Guid.TryParse(cacheRefreshArguments[1], out trackId))
                                {
                                    this.getCampaignByTrackId(trackId);
                                }
                                break;
                            case "VendorApi.CampaignLookupByCampaignId":
                                int campaignId;
                                if (int.TryParse(cacheRefreshArguments[1], out campaignId))
                                {
                                    this.getDirectoryByCampaignId(campaignId);
                                }
                                break;
                            case "VendorApi.VendorsLookup":
                                this.getAllVendors();
                                break;
                            case "VendorApi.CategoriesLookup":
                                this.getAllCategories();
                                break;
                            case "VendorApi.StateLookup":
                                this.getAllStates();
                                break;
                            case "VendorApi.CountryLookup":
                                this.getAllCountries();
                                break;
                            case "VendorApi.SubjectsLookup":
                                this.getAllSubjects();
                                break;
                            case "VendorApi.SpecialtiesLookup":
                                this.getAllSpecialties();
                                break;
                            case "VendorApi.ProgramLevelsLookup":
                                this.getAllProgramLevels();
                                break;
                            case "VendorApi.EnabledInstitutionsLookup":
                                this.getAllEnabledInstitutions();
                                break;
                            case "VendorApi.EnabledProgramLookup":
                                this.getAllEnabledPrograms();
                                break;
                            case "VendorApi.USPostalCodeLookup":
                                this.getAllUSPostalCodes();
                                break;
                            case "VendorApi.CanadianPostalCodeLookup":
                                this.getAllCanadianPostalCodes();
                                break;
                            case "VendorApi.MilitaryStatusList":
                                this.getAllEnabledMilitaryStatuses();
                                break;
                            case "VendorApi.TCPAMessages":
                                this.getAllTCPAMessages();
                                break;
                            case "VendorApi.ZipsLookup":
                                this.getAllZipCodes();
                                break;
                            case "VendorApi.UsersLookup":
                                this.getAllUsers();
                                break;
                        }
                    }
                }
            }
        }

        protected VendorAPIList pageItemList(List<Object> itemList, int startPage, int pageSize)
        {
            VendorAPIList vendorAPIList = null;
            try
            {
                if (itemList != null)
                {
                    vendorAPIList = new VendorAPIList();
                    vendorAPIList.PageSize = pageSize;
                    vendorAPIList.TotalItems = itemList.Count;
                    vendorAPIList.TotalPages = (int)Math.Ceiling(itemList.Count / (Double)pageSize);
                    vendorAPIList.StartPage = startPage > vendorAPIList.TotalPages ? 1 : startPage;
                    vendorAPIList.ItemList = itemList.Skip((vendorAPIList.StartPage - 1) * pageSize).Take(pageSize).ToList();
                }
            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return vendorAPIList;
        }
        protected IEnumerable<Entities.Country> getAllCountries()
        {
            List<Entities.Country> countryList = null;
            try
            {
                const string cacheKey = "VendorApi.CountryLookup";
                countryList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.Country>;
                if (countryList == null)
                {
                    using (var nexusContext = new APINexusEntities())
                    {
                        IEnumerable<DataModels.Country> countries = from Countries in nexusContext.Countries
                                                                    select Countries;

                        if (countries != null)
                        {
                            countryList = new List<Entities.Country>();
                            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<DataModels.Country, Entities.Country>());
                            IMapper mapper = new Mapper(mapperConfiguration);
                            foreach (DataModels.Country country in countries)
                            {
                                Entities.Country mappedCountry = mapper.Map<DataModels.Country, Entities.Country>(country);
                                countryList.Add(mappedCountry);
                            }
                            CacheStore.AddSupportingDataCacheItem(cacheKey, countryList);
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return countryList.AsEnumerable();
        }

        protected IEnumerable<Entities.State> getAllStates()
        {
            List<Entities.State> stateList = null;
            try
            {
                const string cacheKey = "VendorApi.StateLookup";
                stateList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.State>;
                if (stateList == null)
                {
                    using (var nexusContext = new APINexusEntities())
                    {
                        IEnumerable<DataModels.State> states = from State in nexusContext.States
                                                               select State;

                        if (states != null)
                        {
                            stateList = new List<Entities.State>();
                            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<DataModels.Country, Entities.Country>());
                            IMapper mapper = new Mapper(mapperConfiguration);
                            foreach (DataModels.State state in states)
                            {
                                Entities.State mappedState = new Entities.State
                                {
                                    StateId = state.StateID,
                                    Name = state.StateName,
                                    StateCode = state.ShortName,
                                    CountryId = state.CountryID

                                };
                                stateList.Add(mappedState);
                            }
                            CacheStore.AddSupportingDataCacheItem(cacheKey, stateList);
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return stateList.AsEnumerable();
        }

        protected IEnumerable<Entities.Institution> getAllEnabledInstitutions()
        {
            List<Entities.Institution> schoolList = null;
            try
            {
                const string cacheKey = "VendorApi.EnabledInstitutionsLookup";
                schoolList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.Institution>;
                if (schoolList == null)
                {
                    using (var nexusContext = new APINexusEntities())
                    {
                        IEnumerable<DataModels.Institution> schools = from institution in nexusContext.Institutions
                                                                      where institution.IsEnabled
                                                                      select institution;

                        if (schools != null)
                        {
                            schoolList = new List<Entities.Institution>();

                            foreach (DataModels.Institution school in schools)
                            {
                                Entities.Institution mappedSchool = new Entities.Institution
                                {
                                    InstitutionId = school.InstitutionId,
                                    InstitutionName = school.Name,
                                    IsEnabled = school.IsEnabled
                                    //InstitutionCode = school.PartnerCode,
                                    //Address = school.Address1,
                                    //City = school.City,
                                    //Country = country != null ? country : null,
                                    //State = state != null ? state : null,
                                    //PostalCode = school.PostalCode
                                };

                                schoolList.Add(mappedSchool);

                            }
                            CacheStore.AddSupportingDataCacheItem(cacheKey, schoolList);
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return schoolList.AsEnumerable();
        }

        protected IEnumerable<Entities.MilitaryStatus> getAllEnabledMilitaryStatuses()
        {
            List<Entities.MilitaryStatus> statusList = null;
            try
            {
                const string cacheKey = "VendorApi.MilitaryStatusList";
                statusList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.MilitaryStatus>;
                if (statusList == null)
                {
                    using (var nexusContext = new APINexusEntities())
                    {
                        IEnumerable<DataModels.MilitaryStatus> militaryStatuses = from MilitaryStatus in nexusContext.MilitaryStatus
                                                                                  where MilitaryStatus.IsEnabled
                                                                                  select MilitaryStatus;

                        if (militaryStatuses != null)
                        {
                            statusList = new List<Entities.MilitaryStatus>();
                            foreach (DataModels.MilitaryStatus militaryStatus in militaryStatuses)
                            {
                                Entities.MilitaryStatus mappedStatus = new Entities.MilitaryStatus
                                {
                                    MilitaryStatusId = militaryStatus.MilitaryStatusId,
                                    MilitaryStatusName = militaryStatus.MilitaryStatusName,
                                    MilitaryStatusDescription = militaryStatus.MilitaryStatusDescription,
                                    LegacyMilitaryStatusName = militaryStatus.LegacyMilitaryStatusName,
                                    IsEnabled = militaryStatus.IsEnabled
                                };
                                statusList.Add(mappedStatus);
                            }
                            CacheStore.AddSupportingDataCacheItem(cacheKey, statusList);
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return statusList.AsEnumerable();
        }

        private List<PostalCode> getPostalCodes(string cacheKey, Func<IList<Common.Utilities.Entities.ZipCodeEntity>> getZipCodes)
        {
            List<PostalCode> postalCodeList = null;
            postalCodeList = CacheStore.GetCacheItemByKey(cacheKey) as List<PostalCode>;
            if (postalCodeList == null)
            {
                postalCodeList = new List<PostalCode>();
                foreach (Common.Utilities.Entities.ZipCodeEntity postalCode in getZipCodes())
                {
                    PostalCode mappedPostalCode = new PostalCode();
                    mappedPostalCode.PostalCodeString = postalCode.ZipCode.Replace(" ", "");
                    mappedPostalCode.StateCode = postalCode.StateCode;
                    postalCodeList.Add(mappedPostalCode);
                }
                CacheStore.AddSupportingDataCacheItem(cacheKey, postalCodeList);
            }
            return postalCodeList;
        }

        protected List<PostalCode> getAllUSPostalCodes()
        {
            List<PostalCode> postalCodeList = null;
            const string cacheKey = "VendorApi.USPostalCodeLookup";
            postalCodeList = getPostalCodes(cacheKey, new Func<IList<Common.Utilities.Entities.ZipCodeEntity>>(ZipCodeUtilities.GetZipCodes));
            return postalCodeList;
        }

        protected List<PostalCode> getAllCanadianPostalCodes()
        {
            List<PostalCode> postalCodeList = null;
            const string cacheKey = "VendorApi.CanadianPostalCodeLookup";
            postalCodeList = getPostalCodes(cacheKey, new Func<IList<Common.Utilities.Entities.ZipCodeEntity>>(ZipCodeUtilities.GetCanadianZipCodes));
            return postalCodeList;
        }

        protected List<Entities.Category> getAllCategories()
        {
            List<Entities.Category> categoryList = null;
            const string cacheKey = "VendorApi.CategoriesLookup";
            categoryList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.Category>;
            if (categoryList == null)
            {
                using (var nexusContext = new APINexusEntities())
                {
                    IEnumerable<DataModels.Category> categories = from query in nexusContext.Categories
                                                                  select query;

                    if (categories != null)
                    {
                        categoryList = new List<Entities.Category>();
                        foreach (DataModels.Category category in categories)
                        {
                            Entities.Category mappedCategory = new Entities.Category
                            {
                                CategoryId = category.CategoryId,
                                CategoryName = category.CategoryName,
                                IsEnabled = category.IsEnabled
                            };

                            categoryList.Add(mappedCategory);

                        }
                        CacheStore.AddSupportingDataCacheItem(cacheKey, categoryList);
                    }
                }
            }
            return categoryList;
        }

        protected Dictionary<int, List<string>> getAllZipCodes()
        {            
            Dictionary<int,List<string>> zipcodesList = null;            
            const string cacheKey = "VendorApi.ZipsLookup";
            
            zipcodesList = CacheStore.GetCacheItemByKey(cacheKey) as Dictionary<int, List<string>>;
            if (zipcodesList == null)
            {
                using (var nexusContext = new APINexusEntities())
                {
                    var zips = (from query in nexusContext.View_EDDY_GET_ZipCodes
                                orderby query.CampusId, query.ZipCodes descending
                                select query).ToList();

                    if (zips != null)
                    {
                        zipcodesList = new Dictionary<int, List<string>>();
                        foreach (var campus in zips.Select(x => x.CampusId).Distinct())
                        {
                            zipcodesList.Add(campus, zips.Where(x => x.CampusId == campus).Select(x => x.ZipCodes).ToList());
                        }
                        CacheStore.AddSupportingDataCacheItem(cacheKey, zipcodesList);
                    }
                }
            }                            
            return zipcodesList;
        }

        protected List<Entities.Subject> getAllSubjects()
        {
            List<Entities.Subject> subjectList = null;
            const string cacheKey = "VendorApi.SubjectsLookup";
            subjectList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.Subject>;
            if (subjectList == null)
            {
                using (var nexusContext = new APINexusEntities())
                {
                    var subjects = from subject in nexusContext.Subjects
                                   select subject;

                    if (subjects != null)
                    {
                        subjectList = new List<Entities.Subject>();
                        foreach (DataModels.Subject subject in subjects)
                        {
                            Entities.Subject mappedCategory = new Entities.Subject
                            {
                                SubjectId = subject.SubjectId,
                                SubjectName = subject.SubjectName,
                                IsEnabled = subject.IsEnabled
                            };

                            subjectList.Add(mappedCategory);

                        }
                        CacheStore.AddSupportingDataCacheItem(cacheKey, subjectList);
                    }
                }
            }
            return subjectList;
        }

        protected List<Entities.Specialty> getAllSpecialties()
        {
            List<Entities.Specialty> specialtyList = null;
            const string cacheKey = "VendorApi.SpecialtiesLookup";
            specialtyList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.Specialty>;
            if (specialtyList == null)
            {
                using (var nexusContext = new APINexusEntities())
                {
                    var specialties = from specialty in nexusContext.Specialties
                                      select specialty;

                    if (specialties != null)

                        specialtyList = new List<Entities.Specialty>();
                    foreach (DataModels.Specialty specialty in specialties)
                    {
                        Entities.Specialty mappedSpecialty = new Entities.Specialty
                        {
                            SpecialtyId = specialty.SpecialtyId,
                            SpecialtyName = specialty.SpecialtyName,
                            IsEnabled = specialty.IsEnabled
                        };

                        specialtyList.Add(mappedSpecialty);

                    }
                    CacheStore.AddSupportingDataCacheItem(cacheKey, specialtyList);
                }
            }
            return specialtyList;
        }



        protected List<Entities.Level> getAllProgramLevels()
        {
            List<Entities.Level> programLevelList = null;
            const string cacheKey = "VendorApi.ProgramLevelsLookup";
            programLevelList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.Level>;
            if (programLevelList == null)
            {
                using (var nexusContext = new APINexusEntities())
                {
                    var programLevels = from programLevel in nexusContext.ProgramLevels
                                        select programLevel;

                    if (programLevels != null)
                    {
                        programLevelList = new List<Level>();
                        foreach (DataModels.ProgramLevel programLevel in programLevels)
                        {
                            Entities.Level mappedLevel = new Entities.Level
                            {
                                ProgramLevelId = programLevel.ProgramLevelId,
                                ProgramLevelName = programLevel.ProgramLevelName,
                                IsEnabled = programLevel.IsEnabled
                            };
                            programLevelList.Add(mappedLevel);
                        }
                        CacheStore.AddSupportingDataCacheItem(cacheKey, programLevelList);
                    }
                }
            }
            return programLevelList;
        }

        protected Entities.Campus GetCampusByProgramId(int programId)
        {
            Entities.Campus campusResult = null;
            using (var nexusContext = new APINexusEntities())
            {
               // var 
            }
            return campusResult;

        }
        protected Entities.Program GetProgramForCampus()
        {
            Entities.Program programResult = null;
            using (var nexusContext = new APINexusEntities())
            {
                // var 
            }
            return programResult;
        }

        protected List<Entities.Program> getAllEnabledPrograms()
        {
            List<Entities.Program> programList = null;
            const string cacheKey = "VendorApi.EnabledProgramLookup";
            programList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.Program>;
            if (programList == null)
            {
                using (var nexusContext = new APINexusEntities())
                {
                    var programs = from program in nexusContext.Programs
                                   where program.IsEnabled
                                   select program;

                    if (programs != null)
                    {
                        programList = new List<Entities.Program>();
                        foreach (DataModels.Program program in programs)
                        {
                            Entities.Program mappedProgram = new Entities.Program
                            {
                                ProgramId = program.ProgramId,
                                ProgramName = program.ProgramName,
                                IsEnabled = program.IsEnabled
                            };
                            programList.Add(mappedProgram);
                        }
                        CacheStore.AddSupportingDataCacheItem(cacheKey, programList);
                    }
                }
            }
            return programList;
        }

        protected List<Entities.Campus> getAllEnabledCampuses()
        {
            List<Entities.Campus> campusList = null;
            const string cacheKey = "VendorApi.EnabledCampusLookup";
            campusList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.Campus>;
            if (campusList == null)
            {
                using (var nexusContext = new APINexusEntities())
                {
                    var campuses = from campus in nexusContext.Campus
                                   where campus.IsEnabled
                                   select campus;

                    if (campuses != null)
                    {
                        campusList = new List<Entities.Campus>();
                        foreach (DataModels.Campus campus in campuses)
                        {
                            Entities.Campus mappedCampus = new Entities.Campus
                            {
                                CampusId = campus.CampusId,
                                CampusName = campus.Name,
                                IsEnabled = campus.IsEnabled
                            };
                            campusList.Add(mappedCampus);
                        }
                        CacheStore.AddSupportingDataCacheItem(cacheKey, campusList);
                    }
                }
            }
            return campusList;
        }



        protected List<Entities.ContactStandardControlMapping> getAllContactPropertyToStandardControlMappings()
        {
            List<Entities.ContactStandardControlMapping> mapList = null;
            const string cacheKey = "VendorApi.ContactPropertyToStandardControlMap";
            mapList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.ContactStandardControlMapping>;
            if (mapList == null)
            {
                using (var nexusContext = new APINexusEntities())
                {
                    IEnumerable<DataModels.VendorAPIContactToStandardControlMap> standardControlMapping = from query in nexusContext.VendorAPIContactToStandardControlMaps
                                                                                                          select query;

                    if (standardControlMapping != null)
                    {
                        mapList = new List<Entities.ContactStandardControlMapping>();

                        foreach (DataModels.VendorAPIContactToStandardControlMap standardControlMap in standardControlMapping)
                        {
                            Entities.ContactStandardControlMapping map = new Entities.ContactStandardControlMapping
                            {
                                ContactPropertyName = standardControlMap.ContactPropertyName,
                                StandardControlName = standardControlMap.StandardControlName
                            };

                            mapList.Add(map);

                        }
                        CacheStore.AddSupportingDataCacheItem(cacheKey, mapList);
                    }
                }
            }
            return mapList;
        }

        protected Entities.Directory getDirectoryByCampaignId(int campaignId)
        {

            Entities.Directory currentDirectory = null;
            string cacheKey = string.Format("VendorApi.DirectoryLookupByCampaignId_{0}", campaignId);
            currentDirectory = CacheStore.GetCacheItemByKey(cacheKey) as Entities.Directory;
            if (currentDirectory == null)
            {
                Entities.VendorCampaign vendorCampaign = getCampaignByCampaignId(campaignId);
                using (var nexusContext = new APINexusEntities())
                {
                    DataModels.Directory directory = nexusContext.Directories.FirstOrDefault(m => m.DirectoryID == campaignId);

                    if (directory != null)
                    {

                        currentDirectory = new Entities.Directory();
                        currentDirectory.DirectoryId = (int)directory.DirectoryID;
                        currentDirectory.DirectoryName = directory.DirectoryName;
                        currentDirectory.ApplicationId = vendorCampaign.ApplicationId;

                        DataModels.DirectoryApplicationUrl directoryApplicationUrl = nexusContext.DirectoryApplicationUrls.FirstOrDefault(m => m.ApplicationId == vendorCampaign.ApplicationId && m.EntityMetaId == 27);

                        if (directoryApplicationUrl != null)
                        {
                            currentDirectory.BaseURL = directoryApplicationUrl.BaseURL;
                        }

                        CacheStore.AddSupportingDataCacheItem(cacheKey, currentDirectory);
                    }
                }
            }
            return currentDirectory;
        }

        protected Entities.VendorCampaign getCampaignByCampaignId(int campaignId)
        {

            Entities.VendorCampaign vendorCampaign = null;
            string cacheKey = string.Format("VendorApi.CampaignLookupByCampaignId_{0}", campaignId);
            vendorCampaign = CacheStore.GetCacheItemByKey(cacheKey) as Entities.VendorCampaign;
            if (vendorCampaign == null)
            {
                using (var nexusContext = new APINexusEntities())
                {
                    DataModels.Campaign campaign = nexusContext.Campaigns.FirstOrDefault(m => m.CampaignId == campaignId);

                    if (campaign != null)
                    {
                        MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<Campaign, VendorCampaign>());
                        IMapper mapper = new Mapper(mapperConfiguration);
                        vendorCampaign = mapper.Map<Campaign, VendorCampaign>(campaign);
                        CacheStore.AddResponseDataCacheItem(cacheKey, vendorCampaign);
                    }
                }
            }
            return vendorCampaign;
        }

        protected Entities.VendorCampaign getCampaignByTrackId(Guid trackId)
        {

            Entities.VendorCampaign vendorCampaign = null;
            string cacheKey = string.Format("VendorApi.CampaignLookupByTrackId_{0}", trackId);
            vendorCampaign = CacheStore.GetCacheItemByKey(cacheKey) as Entities.VendorCampaign;
            if (vendorCampaign == null)
            {
                using (var nexusContext = new APINexusEntities())
                {



                    DataModels.Campaign campaign = nexusContext.Campaigns.Include("CampaignDetail").FirstOrDefault(m => m.TrackId == trackId);

                    if (campaign != null)
                    {


                        
                        MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<Campaign, VendorCampaign>());
                        IMapper mapper = new Mapper(mapperConfiguration);
                        vendorCampaign = mapper.Map<Campaign, VendorCampaign>(campaign);
                        if (campaign.CampaignDetail.ProspectFlowtypeId != null)
                        {
                            vendorCampaign.ProspectFlowTypeId = (int)campaign.CampaignDetail.ProspectFlowtypeId;
                        }

                        if (campaign.CampaignPeriods != null)
                        {
                            if (campaign.CampaignPeriods.Count > 0)
                            {
                                vendorCampaign.CampaignStatus = campaign.CampaignPeriods.First().CampaignStatu.CampaignStatusName;
                                switch (vendorCampaign.CampaignStatus.ToLower())
                                {
                                    case "active":
                                        vendorCampaign.IsEnabled = true;
                                        break;
                                    case "expired":
                                    case "pending":
                                    case "inactive":
                                    case "terminated":
                                        vendorCampaign.IsEnabled = false;
                                        break;
                                }
                            }
                        }

                        if (campaign.VendorId.HasValue)
                        {
                            Entities.Vendor vendor = this.getAllVendors().Where(m => m.VendorId == campaign.VendorId.Value).FirstOrDefault();
                            if (vendor != null)
                            {
                                vendorCampaign.IsSourceRequired = vendor.IsSourceRequired;
                            }
                            vendorCampaign.VendorId = campaign.VendorId.Value;
                        }

                        vendorCampaign.IsHostAndPost = (bool)campaign.CampaignDetail.HasHostAndPost;


                        vendorCampaign.CampaignTypeId = campaign.CampaignTypeId;
                        if (campaign.CampaignTypeId == 3)
                        {
                            vendorCampaign.IsAPIDirectory = true;
                        }

                        if (campaign.CampaignTypeId == 4)
                        {
                            vendorCampaign.IsCallCenter = true;
                        }

                        if (vendorCampaign.ProspectFlowTypeId > 0)
                        {
                            vendorCampaign.IsProspect = true;
                        }

                        CacheStore.AddResponseDataCacheItem(cacheKey, vendorCampaign);
                    }
                }


            }
            return vendorCampaign;
        }

        protected List<ProgramEducationLevel> getEducationLevelsByProgramIdList(List<int> programIdList)
        {
            List<ProgramEducationLevel> programEducationLevelList = null;

            using (APINexusEntities nexusContext = new APINexusEntities())
            {

                if (programIdList.Count > 0)
                {

                    DataTable programIdDataTable = new DataTable();
                    programIdDataTable.Columns.Add("ProgramId", typeof(int));
                    foreach (var item in programIdList)
                    {
                        programIdDataTable.Rows.Add(item);
                    }
                    SqlParameter programIdTableSqlParameter = new SqlParameter("ProgramIds", SqlDbType.Structured);
                    programIdTableSqlParameter.Value = programIdDataTable;
                    programIdTableSqlParameter.TypeName = "dbo.EDDY_ProgramType";


                    programEducationLevelList = nexusContext.Database.SqlQuery<ProgramEducationLevel>("dbo.EDDY_GET_EducationLevelsByProgram @ProgramIds", programIdTableSqlParameter).ToList();


                }
            }

            return programEducationLevelList;
        }

        protected List<Entities.ProgramState> getAvailableStatesByProgramIdList(List<int> programIdList)
        {
            List<Entities.ProgramState> programStateList = null;

            using (APINexusEntities nexusContext = new APINexusEntities())
            {

                if (programIdList.Count > 0)
                {

                    DataTable programIdDataTable = new DataTable();
                    programIdDataTable.Columns.Add("ProgramProductId", typeof(int));
                    foreach (var item in programIdList)
                    {
                        programIdDataTable.Rows.Add(item);
                    }
                    SqlParameter programIdTableSqlParameter = new SqlParameter("ProgramProductId", SqlDbType.Structured);
                    programIdTableSqlParameter.Value = programIdDataTable;
                    programIdTableSqlParameter.TypeName = "dbo.EDDY_ProgramProductIdType";

                    try
                    {
                        programStateList = nexusContext.Database.SqlQuery<Entities.ProgramState>("dbo.EDDY_GET_IncludedStatesByProgrmaProduct @ProgramProductId", programIdTableSqlParameter).ToList();
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }


                }
            }

            return programStateList;
        }

        protected List<Entities.ProgramCountry> getAvailableCountriesByProgramIdList(List<int> programIdList)
        {
            List<Entities.ProgramCountry> programCountryList = null;

            using (APINexusEntities nexusContext = new APINexusEntities())
            {

                if (programIdList.Count > 0)
                {

                    DataTable programIdDataTable = new DataTable();
                    programIdDataTable.Columns.Add("ProgramProductId", typeof(int));
                    foreach (var item in programIdList)
                    {
                        programIdDataTable.Rows.Add(item);
                    }
                    SqlParameter programIdTableSqlParameter = new SqlParameter("@ProgramProductIds", SqlDbType.Structured);
                    programIdTableSqlParameter.Value = programIdDataTable;
                    programIdTableSqlParameter.TypeName = "dbo.EDDY_ProgramProductIdType";

                    try
                    {
                        programCountryList = nexusContext.Database.SqlQuery<Entities.ProgramCountry>("dbo.EDDY_GET_IncludedCountryByProgrmaProduct @ProgramProductIds", programIdTableSqlParameter).ToList();
                    }
                    catch (Exception exc)
                    {
                        throw exc;
                    }

                }
            }

            return programCountryList;
        }

        protected List<Entities.PostalCodeInclusion> getAvailableZipCodeInclusionsByProgramIdList(List<int> programIdList)
        {
            List<Entities.PostalCodeInclusion> programZipCodeInclusionList = null;

            using (APINexusEntities nexusContext = new APINexusEntities())
            {

                if (programIdList.Count > 0)
                {

                    DataTable programIdDataTable = new DataTable();
                    programIdDataTable.Columns.Add("ProgramId", typeof(int));
                    foreach (var item in programIdList)
                    {
                        programIdDataTable.Rows.Add(item);
                    }
                    SqlParameter programIdTableSqlParameter = new SqlParameter("ProgramIds", SqlDbType.Structured);
                    programIdTableSqlParameter.Value = programIdDataTable;
                    programIdTableSqlParameter.TypeName = "dbo.EDDY_ProgramProductIdType";


                    programZipCodeInclusionList = nexusContext.Database.SqlQuery<Entities.PostalCodeInclusion>("dbo.EDDY_GET_ZipcodeInclusionByProgrmaProduct @ProgramIds", programIdTableSqlParameter).ToList();


                }
            }

            return programZipCodeInclusionList;
        }

        protected int getProductFromLead(decimal leadId)
        {
            int productTier = 0;
            if (leadId > 0)
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {
                    SqlParameter leadIdSqlParameter = new SqlParameter("LeadId", SqlDbType.Decimal);
                    leadIdSqlParameter.Value = leadId;
                    leadIdSqlParameter.ParameterName = "LeadId";

                    productTier = nexusContext.Database.SqlQuery<int>("EDDY_GET_ProductIdFromLead @LeadId", leadIdSqlParameter).FirstOrDefault();
                }
            }

            return productTier;
        }

        public VendorResponseMessage getVendorResponseMessageByMessageCode(string messageCode)
        {
            VendorResponseMessage vendorResponseMessage = null;
            try
            {
                vendorResponseMessage = CacheStore.GetCacheItemByKey(messageCode) as VendorResponseMessage;
                if (vendorResponseMessage == null)
                {
                    using (var nexusContext = new APINexusEntities())
                    {
                        VendorAPIMessage message = nexusContext.VendorAPIMessages.Where(m => m.MessageCode == messageCode).FirstOrDefault();
                        if (message != null)
                        {
                            MapperConfiguration mapperConfiguration = new MapperConfiguration(cfg => cfg.CreateMap<VendorAPIMessage, VendorResponseMessage>());
                            IMapper mapper = new Mapper(mapperConfiguration);
                            vendorResponseMessage = mapper.Map<VendorAPIMessage, VendorResponseMessage>(message);
                            CacheStore.AddSupportingDataCacheItem(messageCode, vendorResponseMessage);
                        }
                    }
                }

            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return vendorResponseMessage;
        }

        protected List<Entities.Advisor> getAllAdvisors()
        {
            List<Entities.Advisor> advisors = null;
            const string cacheKey = "VendorApi.AdvisorsLookup";
            advisors = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.Advisor>;
            if (advisors == null)
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {

                    List<GetAllUsers_Result> users = nexusContext.GetAllUsers().ToList();
                    if (users.Count() > 0)
                    {
                        advisors = new List<Advisor>();
                        foreach (GetAllUsers_Result user in users)
                        {
                            Advisor advisor = new Advisor();
                            advisor.AdvisorId = user.UserId;
                            advisor.LoginId = user.Login;
                            advisor.FirstName = user.FirstName;
                            advisor.LastName = user.LastName;
                            advisor.Email = user.Email;
                            advisors.Add(advisor);
                        }
                        CacheStore.AddSupportingDataCacheItem(cacheKey, advisors);
                    }
                }
            }

            return advisors;
        }

        protected List<Entities.User> getAllUsers()
        {
            List<Entities.User> users = null;
            const string cacheKey = "VendorApi.UsersLookup";
            users = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.User>;

            if (users == null)
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {
                    users = nexusContext.Users
                        .Where(x => x.TSR != null && x.IsEnabled)
                        .Select(x => new Entities.User { 
                            UserId = x.UserID,
                            TSR = x.TSR
                        }).ToList();

                    CacheStore.AddSupportingDataCacheItem(cacheKey, users);
                }
            }

            return users;
        }

        protected List<Entities.ProspectStatus> getAllProspectStatuses()
        {
            List<Entities.ProspectStatus> prospectStatuses = null;
            const string cacheKey = "VendorApi.ProspectStatusLookup";
            prospectStatuses = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.ProspectStatus>;
            if (prospectStatuses == null)
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {

                    List<GetAllProspectStatus_Result> allProspectStatuses = nexusContext.GetAllProspectStatus().ToList();
                    if (allProspectStatuses.Count() > 0)
                    {
                        prospectStatuses = new List<ProspectStatus>();
                        foreach (GetAllProspectStatus_Result currentProspectStatus in allProspectStatuses)
                        {
                            ProspectStatus prospectStatus = new ProspectStatus();
                            prospectStatus.ProspectStatusId = currentProspectStatus.ProspectStatusId;
                            prospectStatus.Code = currentProspectStatus.Code;
                            prospectStatus.Name = currentProspectStatus.Name;
                            prospectStatus.Description = currentProspectStatus.Description;
                            prospectStatuses.Add(prospectStatus);
                        }
                        CacheStore.AddSupportingDataCacheItem(cacheKey, prospectStatuses);
                    }
                }
            }


            return prospectStatuses;
        }

        protected List<Entities.ProgramValidationRule> getProgramValidationRulesByProgramProductId(int programProductId)
        {
            List<Entities.ProgramValidationRule> programValidationRules = null;

            string cacheKey = string.Format("VendorApi.ProgramValidationRulesByProgramProductIdLookup_{0}", programProductId);
            programValidationRules = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.ProgramValidationRule>;
            if (programValidationRules == null)
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {

                    List<GetValidationRulesByProgramProductId_Result> validationRules = nexusContext.GetValidationRulesByProgramProductId(programProductId).ToList();
                    if (validationRules.Count() > 0)
                    {
                        programValidationRules = new List<Entities.ProgramValidationRule>();
                        foreach (GetValidationRulesByProgramProductId_Result currentValidationRule in validationRules)
                        {
                            ProgramValidationRule programValidationRule = new ProgramValidationRule();

                            programValidationRule.RuleName = currentValidationRule.RuleName;

                            programValidationRule.RuleValue = DatabaseUtilities.SetDecimalValue(currentValidationRule.EntityValue);
                            programValidationRule.FieldName = currentValidationRule.Code;
                            programValidationRules.Add(programValidationRule);
                        }

                        CacheStore.AddSupportingDataCacheItem(cacheKey, programValidationRules);
                    }
                }
            }

            return programValidationRules;
        }


        protected List<Entities.Vendor> getAllVendors()
        {
            List<Entities.Vendor> vendorList = null;
            const string cacheKey = "VendorApi.VendorsLookup";
            vendorList = CacheStore.GetCacheItemByKey(cacheKey) as List<Entities.Vendor>;
            if (vendorList == null)
            {
                using (var nexusContext = new APINexusEntities())
                {
                    var vendors = from vendor in nexusContext.Vendors
                                  where vendor.IsEnabled
                                  select vendor;

                    if (vendors != null)
                    {
                        vendorList = new List<Entities.Vendor>();
                        foreach (DataModels.Vendor vendor in vendors)
                        {
                            Entities.Vendor mappedVendor = new Entities.Vendor
                            {
                                VendorId = vendor.VendorId,
                                VendorName = vendor.VendorName,
                                IsSourceRequired = vendor.IsSourceRequired
                            };
                            vendorList.Add(mappedVendor);
                        }
                        CacheStore.AddSupportingDataCacheItem(cacheKey, vendorList);
                    }
                }
            }
            return vendorList;
        }

        protected string getTCPAMessageByKey(string messageKey)
        {
            string tcpaMessage = string.Empty;

            try
            {
                Dictionary<string, string> tcpaMessages = this.getAllTCPAMessages();
                tcpaMessages.TryGetValue(messageKey, out tcpaMessage);
            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            return tcpaMessage;
        }

        private Dictionary<string, string> getAllTCPAMessages()
        {
            Dictionary<string, string> tcpaMessages = null;
            const string cacheKey = "VendorApi.TCPAMessages";

            try
            {
                tcpaMessages = CacheStore.GetCacheItemByKey(cacheKey) as Dictionary<string, string>;
                if (tcpaMessages == null)
                {
                    using (var nexusContext = new APINexusEntities())
                    {
                        Dictionary<string, string> messagesFromDataBase = nexusContext.ResourceMetaData.Where(d => d.IsTCPAMessage).ToDictionary(d => d.ResourceMetaDataKey, d => d.ResourceMetaDataText);
                        if (messagesFromDataBase != null)
                        {
                            CacheStore.AddSupportingDataCacheItem(cacheKey, messagesFromDataBase);
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }

            return tcpaMessages;
        }

        protected decimal  calculateVendorCampaignProgramRPL(decimal rpl, VendorCampaign vendorCampaign)
        {
            decimal calculatedRPL = 0;
            if (vendorCampaign.RevenueSharePercentage.HasValue)
            {
                calculatedRPL = ((rpl * vendorCampaign.RevenueSharePercentage.Value) / 100);
            }
            return calculatedRPL;
        }

        protected CRInvalidProgramOverride getCRInvalidProgramOverride(Guid apiKey)
        {
            CRInvalidProgramOverride crIP = null;

            using (APINexusEntities nexusContext = new APINexusEntities())
            {
                SqlParameter trackIdSqlParameter = new SqlParameter("TrackId", SqlDbType.UniqueIdentifier);
                trackIdSqlParameter.Value = apiKey;
                trackIdSqlParameter.ParameterName = "TrackId";

                crIP = nexusContext.Database.SqlQuery<CRInvalidProgramOverride>("EDDY_GET_CRInvalidProgramOverride @TrackId", trackIdSqlParameter).FirstOrDefault();
            }

            return crIP;
        }


        protected ZipResponse getZipCodes(DirectoryRequest directoryRequest)
        {
            ZipResponse zips = null;
            const string zipcacheKey = "VendorApi.ZipsLookup";            
            MatchingServiceClient matchingServiceClient = new MatchingServiceClient();

            try
            {                
                DirectoryMatchRequest directoryMatchRequest = new DirectoryMatchRequest();

                directoryMatchRequest.Application = ISApplication.VendorAPI;
                directoryMatchRequest.TrackGuid = directoryRequest.APIKey;

                directoryMatchRequest.ProgramTypeList = new List<MatchingService.ProgramType>();
                directoryMatchRequest.ProgramTypeList.Add(MatchingService.ProgramType.FullDegree);
                directoryMatchRequest.RemoveInvalidEntities = true;                
                directoryMatchRequest.CampusId = directoryRequest.CampusId;
                
                directoryMatchRequest.SortMethod = EntitySortMethod.Alphabetical;

                CampusResponse matchedCampusResponse = matchingServiceClient.GetCampuses(directoryMatchRequest);
                if (matchedCampusResponse != null)
                {
                    if(matchedCampusResponse.CampusList != null && matchedCampusResponse.CampusList.Count > 0)
                    {
                        List<int> campuslist = matchedCampusResponse.CampusList.Select(x => x.CampusId).ToList();

                        if(campuslist.Contains(directoryRequest.CampusId))
                        { 
                            Dictionary<int, List<string>> zipscampuslist = CacheStore.GetCacheItemByKey(zipcacheKey) as Dictionary<int, List<string>>;                            
                            ZipResponse zipcodesResponse = new ZipResponse();

                            if (zipscampuslist == null)
                            {
                                Exception exc = new Exception("ZipCode cache failure loading campus zip codes from database");
                                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                                using (APINexusEntities nexusContext = new APINexusEntities())
                                {
                                    var zipslts = (from query in nexusContext.View_EDDY_GET_ZipCodes
                                                    where query.CampusId == directoryRequest.CampusId
                                                    orderby query.ZipCodes descending
                                                    select query.ZipCodes).ToList();

                                    if (zipslts != null)
                                    {
                                        zipcodesResponse.CampusId = directoryRequest.CampusId;
                                        zipcodesResponse.ZipCodes = zipslts;                                        
                                        zips = zipcodesResponse;
                                    }
                                }
                            }
                            else
                            {
                                zipcodesResponse.CampusId = directoryRequest.CampusId;
                                List<string> ziplst = null;
                                zipscampuslist.TryGetValue(directoryRequest.CampusId, out ziplst);
                                zipcodesResponse.ZipCodes = ziplst;                                
                                zips = zipcodesResponse;
                            }
                        }
                    }
                }                
            } 
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return zips;
        }

        protected decimal CreateValidationFailureLead(Lead l)
        {
            decimal result = 0;

            try
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {
                    nexusContext.Leads.Add(l);
                    nexusContext.SaveChanges();
                    result = l.LeadId;
                }
            }
            catch (Exception ex)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, ex.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
            

            return result;
        }

        protected void CreateValidationFailureEMSLead(int ISLeadId)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers["Content-type"] = "application/json";
                client.Encoding = Encoding.UTF8;

                var leadServiceUrl = ConfigurationManager.AppSettings.Get("EmsLeadEngineCreateFromISUrl");
                var authorizationToken = new Guid(ConfigurationManager.AppSettings.Get("EmsLeadEngineAuthToken"));
                var transactionId = Guid.NewGuid();

                var request = new
                {
                    ISLeadIds = new List<int>() { ISLeadId },
                    AuthenticationToken = authorizationToken,
                    TransactionId = transactionId
                };

                string requestData = JsonConvert.SerializeObject(request);

                try
                {
                    var response = client.UploadString(leadServiceUrl, "POST", requestData);
                }
                catch (Exception ex)
                {
                    EDDYLogger.LogMessage(this, LogLevel.Error, ex.Message);
                    ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
                }
            }
        }

        public Lead MapToNexusLead(ContactRequest contact, VendorCampaign campaign, APILead lead, CRInvalidProgramOverride cRInvalidProgramOverride, ProgramValidateResponse validateResponse)
        {
            Lead l = new Lead();
            l.ChannelID = campaign.ChannelId;
            l.VendorId = campaign.VendorId;
            l.ClientRelationshipId = cRInvalidProgramOverride.ClientRelationshipId;
            l.ProgramId = contact.ProgramId;
            l.ProductID = cRInvalidProgramOverride.ProductId;
            l.ApplicationID = campaign.ApplicationId;
            l.FirstName = contact.FirstName;
            l.LastName = contact.LastName;
            l.Address1 = contact.Address;
            l.City = contact.City;
            l.ZipCode = contact.PostalCode;
            l.StateProvince = contact.State;
            l.CountryCode = contact.Country;
            l.EmailAddress = contact.Email;
            l.RealtimeDeliveryStatusId = 432;
            l.TrackId = campaign.TrackId;
            l.PaidStatusTypeId = 3; //paid
            l.CreatedDate = DateTime.Now;
            l.CreatedBy = 0;
            l.CreatedDate_EST = DateTime.Now;
            l.BillingDate = DateTime.Now;
            l.ProgramProductId = cRInvalidProgramOverride.ProgramProductId;
            l.UID = string.Format($"001-{Guid.NewGuid().ToString().ToUpper()}");
            l.UpdatedDate = DateTime.Now;
            l.IsEnabled = true;
            l.Phone1 = lead.Phone1;
            l.TimeToStartInWeeks = 0;
            l.RowGuid = Guid.NewGuid();
            l.Prefix = string.Empty;
            l.Age = lead.Age;
            l.YearHighestEduCompleted = lead.YearHighestEduCompleted;
            l.HighestLevelOfEdu = lead.HighestLevelOfEdu;
            l.Military = lead.Military;
            l.StartDate = lead.StartDate;
            l.CampusId = contact.CampusId;
            l.AllowedViaLeadScoringUpsell = false;
            l.ChatIntegrationSessionId = 0;
            l.ProspectFlowStatusHistoryId = 0;
            l.LeadCreationTypeId = 5; //host and post
            l.IsCallCenter = false;
            l.BusinessModelId = 0;
            l.ProgramTypeID = 1; //full degree
            l.ProgramLevelID = 2;

            Dictionary<string, string> additionalFields = new Dictionary<string, string>();
            foreach (var q in lead.AdditionalFields)
            {
                additionalFields.Add(q.Key, q.Value);
            }
            additionalFields.Add("InitialLeadValidationFailed", "1");
            additionalFields.Add("InitialLeadValidationFailedReason", validateResponse.RuleFailures.First().RuleFailureName);
            l.AdditionalFields = GetXMLFromDictionary(additionalFields);

            return l;
        }

        public static string GetXMLFromDictionary(Dictionary<string, string> dictionary)
        {
            string returnXML = string.Empty;
            if (dictionary != null)
            {

                XmlDocument doc = new XmlDocument();
                XmlElement leadAdditionalFields = doc.CreateElement("LeadAdditionalFields");
                doc.AppendChild(leadAdditionalFields);
                XmlElement fields = doc.CreateElement("Fields");
                doc.DocumentElement.PrependChild(fields);
                XmlNode firstChild = doc.FirstChild;

                foreach (var kvp in dictionary)
                {
                    XmlElement field = doc.CreateElement("Field");
                    field.SetAttribute("FieldName", kvp.Key);
                    field.SetAttribute("Value", kvp.Value);
                    fields.AppendChild(field);
                }

                returnXML = doc.OuterXml;
            }

            return returnXML;
        }
        protected USZipCodeAreaCodeMerge getTimezonesFromDB(TimezoneRequest tzRequest)
        {
            USZipCodeAreaCodeMerge usZipCodeAreaCodeMerge = null;

            using (var nexusContext = new APINexusEntities())
            {
                if(tzRequest.StateCode != null && tzRequest.PostalCode != null)
                {
                    usZipCodeAreaCodeMerge = nexusContext.USZipCodeAreaCodeMerges.Where(m => m.ZipCode == tzRequest.PostalCode && m.StateCode == tzRequest.StateCode).FirstOrDefault();
                    if (usZipCodeAreaCodeMerge != null)
                        return usZipCodeAreaCodeMerge;
                }

                if(tzRequest.PhoneNumber != null)
                {
                    string areaCode = tzRequest.PhoneNumber.Substring(0, 3);
                    string prefix = tzRequest.PhoneNumber.Substring(3, 3);
                    usZipCodeAreaCodeMerge = nexusContext.USZipCodeAreaCodeMerges.Where(m => m.NPA == areaCode && m.NXX == prefix).FirstOrDefault();
                    if (usZipCodeAreaCodeMerge != null)
                        return usZipCodeAreaCodeMerge;
                }

            }


            return usZipCodeAreaCodeMerge;
        }




        public VW_EMSLead LookUpEMSLeadByExternalId(int institutionId, string externalId)
        {
            VW_EMSLead result = null;
            if (!String.IsNullOrEmpty(externalId))
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {
                    result = nexusContext.VW_EMSLead.Where(l => l.InstitutionId == institutionId && l.ExternalId == externalId)
                                            .OrderBy(l => l.LeadStateId).FirstOrDefault();
                }
            }

            return result;
        }
        public VW_EMSLead LookUpEMSLeadByEmailAddress(int institutionId, string email)
        {
            VW_EMSLead result = null;
            if (!String.IsNullOrEmpty(email))
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {
                    result = nexusContext.VW_EMSLead.Where(l => l.InstitutionId == institutionId && l.Email == email
                                                        && l.ClosedReasonCode != "Duplicate"
                                                        && l.ClosedReasonCode != "Marketing Lead Only - Screened Lead")
                                            .OrderBy(l => l.LeadStateId).FirstOrDefault();
                }
            }

            return result;
        }
        public VW_EMSLead LookUpEMSLeadByISLeadId(int institutionId, decimal? isLeadId)
        {
            VW_EMSLead result = null;
            if (isLeadId != null) 
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {
                    result = nexusContext.VW_EMSLead.Where(l => l.InstitutionId == institutionId && l.ISLeadId == isLeadId)
                                                     .FirstOrDefault();
                }
            }

            return result;
        }
        public VW_EMSLead LookUpEMSLeadByPhone1(int institutionId, string phone)
        {
            VW_EMSLead result = null;
            var parsedPhone = ParsePhone(phone);

            if (!String.IsNullOrEmpty(phone))
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {
                    result = nexusContext.VW_EMSLead.Where(l =>  l.InstitutionId == institutionId && l.Phone1 == parsedPhone
                                                         && l.ClosedReasonCode != "Duplicate"
                                                         && l.ClosedReasonCode != "Marketing Lead Only - Screened Lead")
                                           .OrderBy(l => l.LeadStateId).FirstOrDefault();
                }
            }

            return result;
        }

        public VW_EMSLead LookUpEMSLeadByEMSLeadId(int institutionId, decimal? emsLeadId)
        {
            VW_EMSLead result = null;
            if (emsLeadId != null)
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {
                    result = nexusContext.VW_EMSLead.Where(l => l.InstitutionId == institutionId && l.LeadId == emsLeadId)
                                           .FirstOrDefault();
                }
            }

            return result;
        }
        public VW_EMSLead LookUpEMSLeadByFirstLastName(int institutionId, string fName, string lName)
        {
            VW_EMSLead result = null;
            if (!String.IsNullOrEmpty(fName) && !String.IsNullOrEmpty(lName))
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {
                    result = nexusContext.VW_EMSLead.Where(l => l.InstitutionId == institutionId && l.FirstName == fName && l.LastName == lName
                                                        && l.ClosedReasonCode != "Duplicate")
                                            .OrderBy(l => l.LeadStateId).FirstOrDefault();
                }
            }

            return result;
        }
        public VW_EMSLead LookUpEMSLeadByNameAndEmailOrPhone(int institutionId, string fName, string lName, string email, string phone)
        {
            VW_EMSLead result = null;
            var parsedPhone = ParsePhone(phone);

            if (!String.IsNullOrEmpty(fName) && !String.IsNullOrEmpty(lName) && !String.IsNullOrEmpty(email) && !String.IsNullOrEmpty(parsedPhone))
            {
                using (APINexusEntities nexusContext = new APINexusEntities())
                {
                    result = nexusContext.VW_EMSLead.Where(l => l.InstitutionId == institutionId && l.FirstName == fName && l.LastName == lName 
                                                        && (l.Email == email || l.Phone1 == parsedPhone))
                                            .OrderBy(l => l.LeadStateId).FirstOrDefault();
                }
            }

            return result;
        }
        public static string ParsePhone(string phone)
        {
            string phoneReturn = null;

            if (!String.IsNullOrWhiteSpace(phone))
            {
                phoneReturn = new String(phone.Where(Char.IsDigit).ToArray());

                if (phoneReturn.Any() && phoneReturn.Length > 1 && phoneReturn.Substring(0, 1) == "1")
                    phoneReturn = phoneReturn.Substring(1);
            }

            return phoneReturn;
        }

    }



}
