using EDDY.IS.Base;
using EDDY.IS.Core;
using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Helpers;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.Util.StringExtensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EDDY.IS.FormsEngine.Core.Mappers
{
    public class FormInputMapper
    {
        private readonly IIPAddressService _ipAddressService;
        private readonly ILocationValidationService _locationValidationService;

        public FormInputMapper(IIPAddressService ipAddressService, ILocationValidationService locationValidationService)
        {
            _ipAddressService = ipAddressService;
            _locationValidationService = locationValidationService;
        }

        public FormInput MapFormRequestToFormInput(FormRequest formRequest, HttpContextBase httpContext)
        {
            Dictionary<string, string> leadData = formRequest.LeadData.BuildCaseInsensitiveDictionary();
            Dictionary<string, string> additionalData = formRequest.AdditionalData.BuildCaseInsensitiveDictionary();

            FormInput formInput = MapFields(formRequest, leadData, additionalData);
            MapFieldsFromHttpContext(formInput, httpContext);

            formInput.MaxSchoolPickerMatches = 10;

            return formInput;
        }

        private FormInput MapFields(FormRequest formRequest, Dictionary<string, string> fields, Dictionary<string, string> additionalFields)
        {
            FormInput formInput = new FormInput();

            formInput.LeadData = formRequest.LeadData;
            formInput.LeadAdditionalData = formRequest.AdditionalData;
            formInput.SessionId = formRequest.SessionId;
            formInput.FESessionId = formRequest.FESessionId;
            formInput.TemplateId = formRequest.TemplateId;
            formInput.IsBeta = formRequest.IsBeta;
            formInput.ApplicationId = formRequest.ApplicationId;
            formInput.FeatureId = formRequest.FeatureId;
            formInput.InstitutionId = formRequest.InstitutionId;
            formInput.InitialLeadId = formRequest.InitialLeadId;
            formInput.CampusId = StringExtensions.GetFieldValue("Campus", fields, null, true);
            formInput.ProgramId = StringExtensions.GetFieldValue("Program_Of_Interest", fields, null, true);
            formInput.AdvisorId = StringExtensions.GetFieldValue("AdvisorId", fields, null, true);
            formInput.CountryCode = GetCountryCode(fields);
            formInput.StateCode = GetStateCode(fields);
            formInput.Categories = ParseFilterIdList("Categories", fields);
            formInput.Subcategories = ParseFilterIdList("SubCategories", fields);
            formInput.Specialties = ParseFilterIdList("Specialties", fields);
            formInput.ProgramLevelIds = GetProgramLevelIds(fields);
            formInput.CampusPreference = GetCampusPreference(fields);
            formInput.Matches = GetMatches(fields);
            formInput.MaxSchoolPickerMatches = GetMaxSchoolPickerMatches(fields);
            formInput.ProspectFlowTypeId = MapProspectFlowTypeId(formRequest);
            formInput.Prospect = MapFormRequestToProspect(formRequest, fields, additionalFields);
            formInput.TrackId = ParseGuid(formRequest.TrackId);
            formInput.MatchResponseGuid = ParseGuid(formRequest.MatchGuid);
            formInput.LimboAlternativeCampaignTrackId = ParseGuid(formRequest.LimboAlternativeCampaignTrackid);
            formInput.LimboAlternativeCampaignTrackIdUtilized = formRequest.LimboAlternativeCampaignTrackidUtilized;

            return formInput;
        }

        private List<int> ParseFilterIdList(string filterName, Dictionary<string, string> filters)
        {
            List<int> ids = new List<int>();

            string idListString = StringExtensions.GetFieldValue(filterName, filters, true);

            if (!string.IsNullOrWhiteSpace(idListString))
            {
                ids = idListString.Split(',').Select(c => Convert.ToInt32(c)).ToList();
            }

            return ids;
        }

        private List<int> GetProgramLevelIds(Dictionary<string, string> filters)
        {
            List<int> programLevelIds = new List<int>();

            int? desiredProgramLevel = StringExtensions.GetFieldValue("EMSDesiredDegreeLevel", filters, null, true);

            if (desiredProgramLevel.HasValue)
            {
                programLevelIds = new List<int> { desiredProgramLevel.Value };
            }

            return programLevelIds;
        }

        private string GetCampusPreference(Dictionary<string, string> filters)
        {

            string emsCampusPreference = StringExtensions.GetFieldValue("EMSCampusPreference", filters, true);
            string emsCampusPreferenceAndLocation = StringExtensions.GetFieldValue("EMSLearningPreferenceAndLocations", filters, true);
            string campusPreference = StringExtensions.GetFieldValue("CampusPreference", filters, true);

            string CampusPreferenceToBeUsed = DetermineCampusPreferenceToBeUsed(emsCampusPreference, emsCampusPreferenceAndLocation, campusPreference);

            return CampusPreferenceToBeUsed;
        }

        private Guid ParseGuid(string guid)
        {
            Guid.TryParse(guid, out Guid parsedGuid);
            return parsedGuid;
        }

        private string DetermineCampusPreferenceToBeUsed(string primaryCampusPreference, string secondaryCampusPreference, string defaultCampusPreference)
        {
            string campusPreferenceToBeUsed = string.Empty;

            if (!string.IsNullOrWhiteSpace(primaryCampusPreference))
            {
                campusPreferenceToBeUsed = primaryCampusPreference;
            }
            else if (!string.IsNullOrWhiteSpace(secondaryCampusPreference))
            {
                campusPreferenceToBeUsed = secondaryCampusPreference;
            }
            else if (!string.IsNullOrWhiteSpace(defaultCampusPreference))
            {
                campusPreferenceToBeUsed = defaultCampusPreference;
            }

            return campusPreferenceToBeUsed;
        }

        private List<Match> GetMatches(Dictionary<string, string> filters)
        {
            List<Match> result = new List<Match>();

            string schoolPickerSelectionsJson = StringExtensions.GetFieldValue("School_Picker", filters);

            if (!string.IsNullOrWhiteSpace(schoolPickerSelectionsJson))
            {
                var matches = JsonConvert.DeserializeObject<Dictionary<int, Match>>(schoolPickerSelectionsJson);

                if (matches != null && matches.Values.Count > 0)
                {
                    result = matches.Values.ToList();
                }
            }

            return result;
        }

        private int GetMaxSchoolPickerMatches(Dictionary<string, string> filters)
        {
            int? maxSchoolPickerMatches = StringExtensions.GetFieldValue("MaxSchoolPickerMatches", filters, null);
            return maxSchoolPickerMatches ?? 0;
        }

        private int MapProspectFlowTypeId(FormRequest formRequest)
        {
            int prospectFlowTypeId = 0;

            if (formRequest?.ApplicationId > 0)
            {
                prospectFlowTypeId = formRequest.ApplicationId == Constants.EMS_APPLICATION_ID ? (int)ProspectFlowTypes.EMS : (int)ProspectFlowTypes.Prospecting;
            }

            return prospectFlowTypeId;
        }

        private Prospect MapFormRequestToProspect(FormRequest formRequest, Dictionary<string, string> fields, Dictionary<string, string> additionalFields)
        {
            Prospect prospect = new Prospect();
            prospect.ProspectId = formRequest.ProspectId;
            prospect.ExternalLeadId = formRequest.LeadIdToken;
            prospect.Age = StringExtensions.GetFieldValue("Age", fields, null);
            prospect.EducationLevelId = StringExtensions.GetFieldValue("Highest_Level_of_Education_Completed", fields, null);
            prospect.GPAId = StringExtensions.GetFieldValue("GPA-key", additionalFields, null, true);
            prospect.HSGraduationYear = StringExtensions.GetFieldValue("Year_of_Highest_Education_Completed", fields, null);
            prospect.GenericGraduationYear = StringExtensions.GetFieldValue("GraduationYear", fields, null);
            prospect.IsUSCitizen = MapIsUSCitizenField(fields);
            prospect.NeedsFinancialAid = MapNeedsFinancialAidField(fields);
            prospect.IsMilitary = MapIsMilitaryField(fields);
            prospect.MilitaryStatusId = MapMilitaryStatusId(fields);
            prospect.YearsTeachingExperienceKeyValueId = StringExtensions.GetFieldValue("Years_of_Teaching_Experience-key", additionalFields, null, true);
            prospect.YearsWorkExperienceKeyValueId = StringExtensions.GetFieldValue("Years_of_Work_Experience-key", additionalFields, null, true);
            prospect.DesiredStartDate = StringExtensions.GetFieldValue("desired_start_date", fields);
            prospect.Email = StringExtensions.GetFieldValue("email", fields);
            prospect.FirstName = StringExtensions.GetFieldValue("first_name", fields);
            prospect.LastName = StringExtensions.GetFieldValue("last_name", fields);
            prospect.Phone1 = MapPhoneNumber("phone", fields, additionalFields);
            prospect.Phone2 = MapPhoneNumber("alternate_phone", fields, additionalFields);
            prospect.PreferEmail = MapContactPreference("email", fields);
            prospect.PreferPhone = MapContactPreference("phone", fields);
            prospect.PreferText = MapContactPreference("text", fields);
            prospect.PostalCode = StringExtensions.GetFieldValue("Postal_Code", fields);
            prospect.StateId = StringExtensions.GetFieldValue("State-key", additionalFields, null, true);
            prospect.City = StringExtensions.GetFieldValue("city", fields);
            prospect.Address1 = StringExtensions.GetFieldValue("address", fields);
            prospect.Address2 = StringExtensions.GetFieldValue("address_2", fields);
            prospect.CountryId = StringExtensions.GetFieldValue("Country-key", additionalFields, null, true);
            prospect.KVCodeData = MapKVCodeData(additionalFields);

            ValidateProspectLocationIds(prospect, fields);

            return prospect;
        }

        private bool? MapIsUSCitizenField(Dictionary<string, string> filters)
        {
            string usCitizen = StringExtensions.GetFieldValue("us_citizen", filters);
            return string.IsNullOrWhiteSpace(usCitizen) ? (bool?)null : usCitizen.ToLower().Contains("yes");
        }

        private bool? MapNeedsFinancialAidField(Dictionary<string, string> filters)
        {
            string hasFinancialAid = StringExtensions.GetFieldValue("financialaid", filters);
            return string.IsNullOrWhiteSpace(hasFinancialAid) ? (bool?)null : hasFinancialAid.ToLower().Contains("no");
        }

        private bool? MapIsMilitaryField(Dictionary<string, string> filters)
        {
            string militaryStatusId = StringExtensions.GetFieldValue("Military_Affiliation", filters);
            return string.IsNullOrWhiteSpace(militaryStatusId) ? (bool?)null : militaryStatusId != Constants.NON_MILITARY_ID;
        }

        private int? MapMilitaryStatusId(Dictionary<string, string> filters)
        {
            string militaryStatusIdString = StringExtensions.GetFieldValue("Military_Affiliation", filters);
            int militaryStatusId = 0;

            if (!string.IsNullOrWhiteSpace(militaryStatusIdString))
            {
                if (!int.TryParse(militaryStatusIdString, out militaryStatusId))
                {
                    militaryStatusId = 0;
                }
            }

            return militaryStatusId == 0 ? (int?)null : militaryStatusId;
        }

        private string MapPhoneNumber(string fieldName, Dictionary<string, string> leadData, Dictionary<string, string> additionalData)
        {
            int? countryId = StringExtensions.GetFieldValue("Country-key", additionalData, null);
            string countryCode = GetCountryCode(leadData);

            bool isUSCountryId = countryId == 4;
            bool isCanadaCountryId = countryId == 5;
            bool isUSCountryCode = countryCode == "US";
            bool isCanadaCountryCode = countryCode == "CA";

            bool isUSorCanada = isUSCountryId || isCanadaCountryId || isUSCountryCode || isCanadaCountryCode;

            string phoneNumber = StringExtensions.GetFieldValue(fieldName, leadData);
            return phoneNumber.CleanPhoneNumber(isUSorCanada);
        }

        private bool MapContactPreference(string preferenceName, Dictionary<string, string> leadData)
        {
            return StringExtensions.GetFieldValue("preferred_methods_of_contact", leadData).ToLower().Contains(preferenceName.ToLower());
        }

        private KeyValuePair<string, int>[] MapKVCodeData(Dictionary<string, string> additionalData)
        {
            bool k12Found = false;
            List<KeyValuePair<string, int>> kvData = new List<KeyValuePair<string, int>>();

            foreach (var item in additionalData)
            {
                if (StringContainsKeySuffix(item.Key) && int.TryParse(item.Value, out int value))
                {
                    var code = ReplaceKeySuffixWithEmptyString(item.Key);
                    kvData.Add(new KeyValuePair<string, int>(code, value));
                    k12Found = code == "K12" ? true : k12Found;
                }
            }

            if (!k12Found)
            {
                kvData.Add(new KeyValuePair<string, int>("K12", 23));
            }

            return kvData.ToArray();
        }

        private bool StringContainsKeySuffix(string key)
        {
            return key.Contains("-key");
        }

        private string ReplaceKeySuffixWithEmptyString(string key)
        {
            return key.Replace("-key", "");
        }

        private void ValidateProspectLocationIds(Prospect prospect, Dictionary<string, string> fields)
        {
            string countryCode = GetCountryCode(fields);
            string stateCode = GetStateCode(fields);

            if (!string.IsNullOrWhiteSpace(countryCode) && !string.IsNullOrWhiteSpace(stateCode))
            {
                Location location = _locationValidationService.GetValidLocation(countryCode, stateCode);

                var countryId = location?.CountryId;
                var stateId = location?.StateId;

                if (countryId > 0 && stateId > 0)
                {
                    prospect.CountryId = countryId;
                    prospect.StateId = stateId;
                }
            }
        }

        private string GetCountryCode(Dictionary<string, string> fields)
        {
            return StringExtensions.GetFieldValue("Country", fields).ToUpper();
        }

        private string GetStateCode(Dictionary<string, string> fields)
        {
            return StringExtensions.GetFieldValue("State", fields).ToUpper();
        }

        private void MapFieldsFromHttpContext(FormInput formInput, HttpContextBase httpContext)
        {
            formInput.HttpReferer = httpContext.Request.ServerVariables["HTTP_REFERER"];
            formInput.UserAgent = httpContext.Request.ServerVariables["HTTP_USER_AGENT"];
            formInput.IpAddress = _ipAddressService.GetIPAddress(httpContext.Request.ServerVariables["HTTP_VIA"], httpContext.Request["HTTP_X_FORWARDED_FOR"], httpContext.Request["REMOTE_ADDR"]);
        }
    }
}
