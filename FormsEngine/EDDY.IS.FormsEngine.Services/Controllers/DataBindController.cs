using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Mappers;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.FormsEngine.DTO;
using EDDY.IS.FormsEngine.DTO.Extended;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.Services.Controllers.Base;
using EDDY.IS.FormsEngine.Services.Controllers.Common;
using EDDY.IS.FormsEngine.Services.Models;
using EDDY.IS.Util.HTMLExtensions;
using EDDY.IS.Util.StringExtensions;

namespace EDDY.IS.FormsEngine.Services.Controllers
{
    public class DataBindController : DataBindControllerBase
    {
        private readonly IInstitutionService _institutionService;
        private readonly IFailedMatchReplacementService _failedMatchReplacementService;
        private readonly IComponentRenderingService _componentRenderingService; 

        public DataBindController(IInstitutionService institutionService, IFailedMatchReplacementService failedMatchReplacementService, IComponentRenderingService componentRenderingService)
        {
            _institutionService = institutionService;
            _failedMatchReplacementService = failedMatchReplacementService;
            _componentRenderingService = componentRenderingService;
        }

        [HttpGet]
        public ActionResult GetStates(DataBindFilter Filters)
        {
            List<DataBindResultItem> Result = new List<DataBindResultItem>();
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                string ZipCode = StringExtensions.GetFieldValue("Postal_Code", Dictionary);
                string Country = StringExtensions.GetFieldValue("Country", Dictionary);
                var StateList = Validation.GetStatesResult(Country, ZipCode);
                Result = (from sl in StateList
                          select new DataBindResultItem() { Text = sl.Text, Key = sl.Key, Value = sl.Value, Selected = sl.Selected }).ToList();
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetCountries(DataBindFilter Filters)
        {
            List<DataBindResultItem> Result = new List<DataBindResultItem>();
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                string ZipCode = StringExtensions.GetFieldValue("Postal_Code", Dictionary);
                string State = StringExtensions.GetFieldValue("State", Dictionary);
                var CountryList = Validation.GetCountriesResult(State, ZipCode);
                Result = (from c in CountryList
                          select new DataBindResultItem() { Text = c.Text, Key = c.Key, Value = c.Value, Selected = c.Selected }).ToList();
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult GetDesiredCountries(DataBindFilter Filters)
        {
            List<DataBindResultItem> Result = new List<DataBindResultItem>();
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                bool IsBeta = false;
                bool.TryParse(StringExtensions.GetFieldValue("IsBeta", Dictionary), out IsBeta);
                int? ApplicationId = 20; //Desired country and city require applciationid to be SAB

                DirectoryMatchRequest request = BuildDirectoryMatchRequest(Dictionary, true, EntitySortMethod.Alphabetical, ApplicationId, null);
                request.ProgramTypeList = new ProgramType[] { ProgramType.StudyAbroad };
                request.SortMethod = EntitySortMethod.Alphabetical;
                var facetedNav = FormsEngineService.RelatedServices.GetFacetedNavigation(request, IsBeta);


                Result = (from c in facetedNav.Countries
                          select new DataBindResultItem() { Key = c.CountryId.ToString(), Value = c.CountryId.ToString(), Text = c.CountryName }).ToList();
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetDesiredCities(DataBindFilter Filters)
        {
            List<DataBindResultItem> Result = new List<DataBindResultItem>();
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                bool IsBeta = false;
                bool.TryParse(StringExtensions.GetFieldValue("IsBeta", Dictionary), out IsBeta);
                int? ApplicationId = 20; //Desired country and city require applciationid to be SAB
                int? DesiredCountryId = StringExtensions.GetFieldValue("Desired_Country", Dictionary,null);

                DirectoryMatchRequest request = BuildDirectoryMatchRequest(Dictionary, true, EntitySortMethod.Alphabetical, ApplicationId, null);
                request.ProgramTypeList = new ProgramType[] { ProgramType.StudyAbroad };
                request.SortMethod = EntitySortMethod.Alphabetical;
                if (DesiredCountryId.HasValue)
                {
                    request.GeoTarget = new GeoTarget();
                    request.GeoTarget.CountryList = new int[] { DesiredCountryId.Value };
                }
                var facetedNav = FormsEngineService.RelatedServices.GetFacetedNavigation(request, IsBeta);


                Result = (from c in facetedNav.Cities
                          select new DataBindResultItem() { Key = c.CityId.ToString(), Value = c.CityId.ToString(), Text = c.CityName }).ToList();
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetCity(DataBindFilter Filters)
        {
            DataBindResultItem Result = new DataBindResultItem();
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                string ZipCode = StringExtensions.GetFieldValue("Postal_Code", Dictionary);
                Result.Text = Validation.GetCity(ZipCode);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult GetProgramLevels(DataBindFilter Filters)
        {
            List<DataBindResultItem> Result = new List<DataBindResultItem>();
            
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                bool IsBeta = false;
                bool.TryParse(StringExtensions.GetFieldValue("IsBeta", Dictionary), out IsBeta);
                int? ApplicationId = StringExtensions.GetFieldValue("ApplicationId", Dictionary, null);


                string ZipCode = StringExtensions.GetFieldValue("Postal_Code", Dictionary);
                var ProgramLevelResult = FormsEngineService.RelatedServices.GetProgramLevels(BuildDirectoryMatchRequest(Dictionary, true, EntitySortMethod.Alphabetical, ApplicationId, null), IsBeta);

                var programLevels = (from pll in ProgramLevelResult.ProgramLevelList
                                     join pl in FormsEngineService.GetProgramLevels()
                                     on pll.ProgramLevelId equals pl.ProgramLevelId
                                     select new { pll.ProgramLevelId, pll.ProgramLevelName, pl.DisplayOrder }).ToList();

                Result = (from pl in programLevels
                          orderby pl.DisplayOrder ascending
                          select new DataBindResultItem() { Key = pl.ProgramLevelId.ToString(), Value = pl.ProgramLevelId.ToString(), Text = pl.ProgramLevelName }).ToList();
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetPrograms(DataBindFilter Filters)
        {
            string Result = null;
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                bool IsBeta = false;
                bool.TryParse(StringExtensions.GetFieldValue("IsBeta", Dictionary), out IsBeta);

                int? InstitutionId = StringExtensions.GetFieldValue("InstitutionId", Dictionary, 0);
                int? ProgramId = StringExtensions.GetFieldValue("ProgramId", Dictionary, null);
                string TrackId = StringExtensions.GetFieldValue("TrackId", Dictionary);
                int? ApplicationId = StringExtensions.GetFieldValue("ApplicationId", Dictionary, null);
                string DeviceId = StringExtensions.GetFieldValue("DeviceId", Dictionary);
                bool IsProgramWizard= StringExtensions.GetFieldValue("FormTemplateType", Dictionary)=="3";
                bool IncludeProgramLevelGroups = StringExtensions.GetFieldValue("IncludeProgramLevelGroups", Dictionary) == "true";
                int? TemplateId = StringExtensions.GetFieldValue("TemplateId", Dictionary, null);
                ProgramOptGroup pog = ProgramOptGroup.Default;

                if (TemplateId.HasValue )
                {
                    var ProgramControl = FormsEngineService.GetTemplateControlByCode(TemplateId.Value, "Program_Of_Interest");

                    if (ProgramControl != null)
                    {
                        string OptGroupSetting = ProgramControl.GetExtendedPropertyValue<string>("ProgramOptGroup");
                        OptGroupSetting = OptGroupSetting ?? string.Empty;
                        switch (OptGroupSetting.ToLowerInvariant())
                        {
                            case "custom" : pog = ProgramOptGroup.Custom; break;
                            case "none": pog = ProgramOptGroup.None; break;
                        }
                    }
                }

                //Custom FE flag will precede over any setting
                if(!IncludeProgramLevelGroups)
                {
                    pog = ProgramOptGroup.None;
                }

                Result = GetProgramTemplateProgramsDDL(InstitutionId.Value, ProgramId, IsBeta, TrackId, Filters.FilterString, ApplicationId, DeviceId, string.Empty, IsProgramWizard, pog);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult GetCampusTypes(DataBindFilter Filters)
        {
            List<DataBindResultItem> Result = new List<DataBindResultItem>();
            DataBindResultItem ResultItemBoth = new DataBindResultItem() { Text = "Both", Key = "Both", Value = "Both" };
            DataBindResultItem ResultItemCampus = new DataBindResultItem() { Text = "Campus", Key = "Campus", Value = "Campus" };
            DataBindResultItem ResultItemOnline = new DataBindResultItem() { Text = "Online", Key = "Online", Value = "Online" };

            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                bool IsBeta = false;
                bool.TryParse(StringExtensions.GetFieldValue("IsBeta", Dictionary), out IsBeta);
                int? ApplicationId = StringExtensions.GetFieldValue("ApplicationId", Dictionary, null);
                string DefaultCampusPreference = StringExtensions.GetFieldValue("DefaultCampusPreference", Dictionary);
                var CampusTypeResult = FormsEngineService.RelatedServices.GetCampusTypes(BuildDirectoryMatchRequest(Dictionary, true, EntitySortMethod.Alphabetical, ApplicationId, null), IsBeta);

                if (CampusTypeResult == null || CampusTypeResult.CampusTypeList == null || CampusTypeResult.CampusTypeList.Count() == 0)
                {
                    ResultItemBoth.Selected = true;
                    Result.Add(ResultItemBoth);
                }
                else if (CampusTypeResult.CampusTypeList.Count() == 1)
                {
                    if (CampusTypeResult.CampusTypeList[0] == CampusType.Online)
                    {
                        ResultItemOnline.Selected = true;
                        Result.Add(ResultItemOnline);
                    }
                    else
                    {
                        ResultItemCampus.Selected = true;
                        Result.Add(ResultItemCampus);
                    }
                }
                else 
                {
                    if (DefaultCampusPreference.ToLower() == "online")
                    {
                        ResultItemOnline.Selected = true;
                    }
                    else if (DefaultCampusPreference.ToLower() == "campus")
                    {
                        ResultItemCampus.Selected = true;
                    }
                    else
                    {
                        ResultItemBoth.Selected = true;
                    }

                    Result.Add(ResultItemOnline);
                    Result.Add(ResultItemCampus);
                    Result.Add(ResultItemBoth);
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
                Result.Clear();
                ResultItemBoth.Selected = true;
                Result.Add(ResultItemBoth); //default both
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetCategories(DataBindFilter Filters)
        {
            CategoryResponse Result = null;
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                bool IsBeta = false;
                bool.TryParse(StringExtensions.GetFieldValue("IsBeta", Dictionary), out IsBeta);
                int? ApplicationId = StringExtensions.GetFieldValue("ApplicationId", Dictionary, null);

                Result = FormsEngineService.RelatedServices.GetCategories(BuildDirectoryMatchRequest(Dictionary, true, EntitySortMethod.Alphabetical, ApplicationId, null), IsBeta);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }
        
        [HttpGet]
        public ActionResult GetSubCategories(DataBindFilter Filters)
        {
            SubjectResponse Result = null;
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                bool IsBeta = false;
                bool.TryParse(StringExtensions.GetFieldValue("IsBeta", Dictionary), out IsBeta);
                int? ApplicationId = StringExtensions.GetFieldValue("ApplicationId", Dictionary, null);
                Result = FormsEngineService.RelatedServices.GetSubCategories(BuildDirectoryMatchRequest(Dictionary, true, EntitySortMethod.Alphabetical, ApplicationId, null), IsBeta);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetSpecialties(DataBindFilter Filters)
        {
            SpecialtyResponse Result = null;
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                bool IsBeta = false;
                bool.TryParse(StringExtensions.GetFieldValue("IsBeta", Dictionary), out IsBeta);
                int? ApplicationId = StringExtensions.GetFieldValue("ApplicationId", Dictionary, null);

                Result = FormsEngineService.RelatedServices.GetSpecialties(BuildDirectoryMatchRequest(Dictionary, true, EntitySortMethod.Alphabetical, ApplicationId, null), IsBeta);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult GetResourceMetaDataTextForKey(string Keys)
        {
            string[] keyArray;
            Dictionary<string, string> Result = null;
            Keys = Keys ?? string.Empty;

            try
            {
                keyArray = Keys.Trim().Split(',');

                if (keyArray.Length > 0)
                {
                    Result = new Dictionary<string, string>();

                    foreach (string key in keyArray)
                    {
                        Result.Add(key, FormsEngine.GetResourceMetaDataTextForKey(key));
                    }
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }


        [HttpGet]
        public ActionResult GetResourceMetaDataTextForTCPA()
        {
            Dictionary<string, string> Result = null;

            try
            {
                Result = FormsEngine.GetResourceMetaDataForTCPA();

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetLandingPageSettings(string formLeadUrl)
        {
            VW_LandingPageSettingsDTO Result = null;

            try
            {
                Result = FormsEngine.GetLandingPageSettings(formLeadUrl);

            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        public ActionResult GetEMSInstitutionTCPAText(int InstitutionId)
        {
            string Result = null;

            try
            {
                Result = FormsEngine.GetInstitutionTCPAText(InstitutionId);
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }
            return new JsonpResult(Result);
        }

        [HttpGet]
        [AllowCrossSiteJsonAttribute]
        public JsonResult SearchPreDefinedValueList(string term, string standardControlCode)
        {
            JsonResult result = null;
            try
            {


                if (!string.IsNullOrEmpty(term) && !string.IsNullOrEmpty(standardControlCode))
                {
                    List<DataModel.ValueList> valueLists = null;
                    FormsEngineService.GetStandardControlPreDefinedValueList().TryGetValue(standardControlCode, out valueLists);
                    if (valueLists != null)
                    {
                        IEnumerable<DataModel.ValueList> searchedValueLists = valueLists.Where(vl => vl.Text.ToLower().Contains(term.ToLower())).Take(10).ToList();
                        if (searchedValueLists.Count() > 0)
                        {
                      
                            result = Json(searchedValueLists.Select(vl => new { id = vl.Value, text = vl.Text, key = vl.Key }).ToList(), JsonRequestBehavior.AllowGet);
                        }
                        else
                        {

                            List<DataModel.ValueList> defaultValueList = valueLists.Where(m => m.IsDefault == true).ToList();

                            if (defaultValueList.Count() > 0)
                            {

                                result = Json(defaultValueList.Select(vl => new { id = vl.Value, text = vl.Text, key = vl.Key }).ToList(), JsonRequestBehavior.AllowGet);
                            }
                            else
                            {
                                result = Json(valueLists.Where(m => m.Text.ToLower() == "other").Select(vl => new { id = vl.Value, text = vl.Text, key = vl.Key }).ToList(), JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();

            }
            return result;
        }

        [HttpGet]
        public ActionResult GetKVCodeData(DataBindFilter Filters)
        {
            List<DataBindResultItem> result = new List<DataBindResultItem>();
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();
                string kvCodeDataName = StringExtensions.GetFieldValue("Name", Dictionary);
                int? institutionId = StringExtensions.GetFieldValue("InstitutionId", Dictionary, 0);

                if (institutionId != null && institutionId != 0)
                {
                    List<KeyValuePair<string, string>> referrals = FormsEngineService.GetKVCodeData(kvCodeDataName, institutionId.Value);
                    result = referrals.Select(e => new DataBindResultItem { Key = e.Key, Value = e.Value, Text = e.Value }).ToList();
                }
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return new JsonpResult(result);
        }

        [HttpGet]
        public ActionResult GetCampuses(DataBindFilter Filters)
        {
             List<CampusWithInstitutionModel> result = new List<CampusWithInstitutionModel>();
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();

                bool IsBeta = false;
                bool.TryParse(StringExtensions.GetFieldValue("IsBeta", Dictionary), out IsBeta);
                string TrackId = StringExtensions.GetFieldValue("TrackId", Dictionary);
                int? InstitutionId = StringExtensions.GetFieldValue("InstitutionId", Dictionary, 0);
                int? ApplicationId = StringExtensions.GetFieldValue("ApplicationId", Dictionary, 0);

                DirectoryMatchRequest matchRequest = BuildDirectoryMatchRequest(IsBeta, TrackId, Filters.FilterString, true, EntitySortMethod.Alphabetical, InstitutionId, ApplicationId, null);
                CampusResponse campusResponse = FormsEngineService.RelatedServices.GetCampuses(matchRequest, IsBeta);

                var campuses = campusResponse.CampusList.Where(c => !c.FailedValidation).ToList();
                result = campuses.Select(c => new CampusWithInstitutionModel { CampusId = c.CampusId, CampusName = c.CampusName, CampusType = c.CampusType??CampusType.Ground}).ToList();
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return new JsonpResult(result);
        }

    

        [HttpGet]
        public ActionResult GetCampusLocations(DataBindFilter Filters)
        {
            List<CampusWithInstitutionModel> result = new List<CampusWithInstitutionModel>();
            try
            {
                var Dictionary = Filters.FilterString.BuildCaseInsensitiveDictionary();

                bool IsBeta = false;
                bool.TryParse(StringExtensions.GetFieldValue("IsBeta", Dictionary), out IsBeta);
                string TrackId = StringExtensions.GetFieldValue("TrackId", Dictionary);
                int? InstitutionId = StringExtensions.GetFieldValue("InstitutionId", Dictionary, 0);
                int? ApplicationId = StringExtensions.GetFieldValue("ApplicationId", Dictionary, 0);
                
                DirectoryMatchRequest matchRequest = BuildDirectoryMatchRequest(IsBeta, TrackId, Filters.FilterString, true, EntitySortMethod.Alphabetical, InstitutionId, ApplicationId, null);
                CampusResponse campusResponse = FormsEngineService.RelatedServices.GetCampuses(matchRequest, IsBeta);

                var campuses = campusResponse.CampusList.Where(c => !c.FailedValidation && c.CampusType == CampusType.Ground).ToList();
                result = campuses.Select(c => new CampusWithInstitutionModel { CampusId = c.CampusId, CampusName = c.CampusName, CampusType = CampusType.Ground }).ToList();
            }
            catch (Exception ex)
            {
                new ISException(EDDY.IS.Base.ISApplication.FormsEngine, ex).Save();
            }

            return new JsonpResult(result);
        }

    }
  

}
