using EDDY.IS.Core.Logging;
using EDDY.IS.FormsEngine.Caching;
using EDDY.IS.FormsEngine.DataModel;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Util.StringExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace EDDY.IS.FormsEngine
{
    public class ProspectInputBuilder
    {
        //Constants
        private const string NON_MILITARY_ID = "126";

        private static string[] DynamicFieldKeys
        {
            get
            {
                return new string[]
                {
                    "LeadInitiatingUrl",
                    "FormLeadUrl",
                    "LeadSourceUrl",
                    "AffiliateId",
                    "SubSource1",
                    "SubSource2",
                    "SourceCode",
                    "InitiatingURL",
                    "LandingURL",
                    "CallCenterURL",
                    "VideoUrl",
                    "utm_source",
                    "utm_medium",
                    "utm_campaign"
                };
            }
        }    

        // Prospect Input for Matching Engine Service
        public static ProspectInput BuildProspectInput(Dictionary<string, string> LeadData, Dictionary<string, string> LeadAdditionalData, bool? IsMobileNumber, string TrackingSessionGUID)
        {
            Dictionary<string, string> AdditionalData = new Dictionary<string, string>(LeadAdditionalData, StringComparer.OrdinalIgnoreCase);

            //Prospect Input
            ProspectInput Prospect = new ProspectInput();
            Prospect.Age = StringExtensions.GetFieldValue("Age", LeadData, null);
            Prospect.CountryId = StringExtensions.GetFieldValue("Country-key", AdditionalData, null, true);
            Prospect.EducationLevelId = StringExtensions.GetFieldValue("Highest_Level_of_Education_Completed", LeadData, null);
            Prospect.GPAKeyValueId = StringExtensions.GetFieldValue("GPA-key", AdditionalData, null, true);
            Prospect.HSGraduationYear = StringExtensions.GetFieldValue("Year_of_Highest_Education_Completed", LeadData, null);
            Prospect.ExternalLeadId = StringExtensions.GetFieldValue("leadid_token", LeadData);
            string UsCitizen = StringExtensions.GetFieldValue("us_citizen", LeadData);
            Prospect.IsCitizen = string.IsNullOrWhiteSpace(UsCitizen) ? (bool?)null : UsCitizen.Contains("Yes");
            string Military = StringExtensions.GetFieldValue("Military_Affiliation", LeadData);
            int MilitaryStatusId = 0;
            if (!string.IsNullOrWhiteSpace(Military))
            {
                if (!int.TryParse(Military, out MilitaryStatusId))
                {
                    MilitaryStatusId = 0;
                }
            }
            Prospect.IsMilitary = string.IsNullOrWhiteSpace(Military) ? (bool?)null : Military != NON_MILITARY_ID;
            Prospect.MilitaryStatusId = MilitaryStatusId == 0 ? (int?)null : MilitaryStatusId;
            Prospect.PostalCode = StringExtensions.GetFieldValue("Postal_Code", LeadData);
            Prospect.StateId = StringExtensions.GetFieldValue("State-key", AdditionalData, null, true);
            Prospect.YearsTeachingExperienceKeyValueId = StringExtensions.GetFieldValue("Years_of_Teaching_Experience-key", AdditionalData, null, true);
            Prospect.YearsWorkExperienceKeyValueId = StringExtensions.GetFieldValue("Years_of_Work_Experience-key", AdditionalData, null, true);

            //Extended prospect input fields
            Prospect.City = StringExtensions.GetFieldValue("city", LeadData);
            Prospect.AddressLine2 = StringExtensions.GetFieldValue("address_2", LeadData);
            Prospect.Email = StringExtensions.GetFieldValue("email", LeadData);
            Prospect.FirstName = StringExtensions.GetFieldValue("first_name", LeadData);
            Prospect.LastName = StringExtensions.GetFieldValue("last_name", LeadData);
            Prospect.Phone1 = StringExtensions.GetFieldValue("phone", LeadData);
            Prospect.Phone2 = StringExtensions.GetFieldValue("alternate_phone", LeadData);
            Prospect.StreetAddress = StringExtensions.GetFieldValue("address", LeadData);
            Prospect.IsMobileNumber = IsMobileNumber;

            Prospect.CecUniqueId = StringExtensions.GetFieldValue("CecId", LeadData);

            //Phone Number cleanup
            bool IsUS = !Prospect.CountryId.HasValue ? false : Prospect.CountryId == 4;
            bool IsCanada = !Prospect.CountryId.HasValue ? false : Prospect.CountryId == 5;
            bool IsUSorCanada = IsUS || IsCanada;


            Prospect.Phone1 = string.IsNullOrEmpty(Prospect.Phone1) ? "" : Prospect.Phone1.CleanPhoneNumber(IsUSorCanada);
            Prospect.Phone2 = string.IsNullOrEmpty(Prospect.Phone2) ? "" : Prospect.Phone2.CleanPhoneNumber(IsUSorCanada);


            //Additional fields
            bool K12Found = false;
            List<KeyValuePair<string, int>> kvdata = new List<KeyValuePair<string, int>>();
            foreach (var item in AdditionalData)
            {
                if (item.Key.Contains("-key"))
                {
                    int value = 0;
                    if (int.TryParse(item.Value, out value))
                    {
                        var code = item.Key.Replace("-key", "");
                        kvdata.Add(new KeyValuePair<string, int>(code, value));
                        K12Found = code == "K12" ? true : K12Found;
                    }
                }
            }
            if (!K12Found)
            {
                kvdata.Add(new KeyValuePair<string, int>("K12", 23));
            }
            if (kvdata.Count > 0)
            {
                Prospect.KVCodeData = kvdata.ToArray();
            }

            var dynamicFields = new List<KeyValuePair<string, string>>();
            foreach (string fieldKey in DynamicFieldKeys)
            {
                string fieldValue = GetFieldValueFromLeadCollections(fieldKey, LeadData, AdditionalData);
                var field = new KeyValuePair<string, string>(fieldKey, fieldValue);
                dynamicFields.Add(field);
            }
            Prospect.DynamicFields = dynamicFields.ToArray();

            Prospect.TrackingSessionGUID = TrackingSessionGUID;
            return Prospect;
        }

        private static string GetFieldValueFromLeadCollections(string key, Dictionary<string, string> leadData, Dictionary<string, string> additionalData)
        {
            string value = StringExtensions.GetFieldValue(key, leadData);

            if (string.IsNullOrEmpty(value))
            {
                value = StringExtensions.GetFieldValue(key, additionalData);
            }

            return value;
        }

        public static void AggregateCreativeURLs(LeadCreateRequest leadRequest)
        {
            try
            {
                Dictionary<string, string> LeadData = new Dictionary<string, string>(leadRequest.LeadData, StringComparer.OrdinalIgnoreCase);
                string firstName = LeadData.ContainsKey("first_name") ? LeadData["first_name"] : "";
                string phone = LeadData.ContainsKey("phone") ? LeadData["phone"] : "";
                string email = LeadData.ContainsKey("email") ? LeadData["email"] : "";
                string leadSourceUrl = LeadData.ContainsKey("LeadSourceUrl") ? HttpUtility.UrlDecode(LeadData["LeadSourceUrl"]) : "";
                string leadInitiatingUrl = LeadData.ContainsKey("LeadInitiatingUrl") ? HttpUtility.UrlDecode(LeadData["LeadInitiatingUrl"]) : "";
                string formLeadUrl = LeadData.ContainsKey("FormLeadUrl") ? HttpUtility.UrlDecode(LeadData["FormLeadUrl"]) : "";
                string startUrl = ConfigurationManager.AppSettings.Get("CreativePortalUrl");
                string videoUrl = LeadData.ContainsKey("VideoUrl") ? HttpUtility.UrlDecode(LeadData["VideoUrl"]) : "";
                bool bypassCreativeUrls = LeadData.ContainsKey("UrlsFromQueryString") ? LeadData["UrlsFromQueryString"] == "UrlsFromQueryString" : false;

                var urls = new EDDY_FE_GetCreativeURLs_Result();
                if (!bypassCreativeUrls) { 
                    var dbSubmissionService = new SubmissionDataService();
                    urls = dbSubmissionService.GetCreativeUrls(startUrl, leadRequest.TrackId, leadRequest.LeadCreationTypeId, formLeadUrl, leadSourceUrl, leadInitiatingUrl, firstName, phone, email, videoUrl);
                }

                leadRequest.InitiatingURL = bypassCreativeUrls ? leadInitiatingUrl : urls.InitiatingURL;
                leadRequest.LandingURL = bypassCreativeUrls ? formLeadUrl.Split('?')[0] : urls.LandingURL;
                leadRequest.CallCenterURL = bypassCreativeUrls ? "" : urls.CallCenterURL;
                leadRequest.VideoUrl = bypassCreativeUrls ? videoUrl : urls.VideoURL;

                if (!leadRequest.LeadData.ContainsKey("InitiatingURL"))
                    leadRequest.LeadData.Add("InitiatingURL", leadRequest.InitiatingURL);
                else
                    leadRequest.LeadData["InitiatingURL"] = leadRequest.InitiatingURL;

                if (!leadRequest.LeadData.ContainsKey("LandingURL"))
                    leadRequest.LeadData.Add("LandingURL", leadRequest.LandingURL);
                else
                    leadRequest.LeadData["LandingURL"] = leadRequest.LandingURL;

                if (!leadRequest.LeadData.ContainsKey("CallCenterURL"))
                    leadRequest.LeadData.Add("CallCenterURL", leadRequest.CallCenterURL);
                else
                    leadRequest.LeadData["CallCenterURL"] = leadRequest.CallCenterURL;

                if (!string.IsNullOrEmpty(urls.VideoURL))
                if (!leadRequest.LeadData.ContainsKey("VideoUrl"))
                    leadRequest.LeadData.Add("VideoUrl", urls.VideoURL);
                else
                    leadRequest.LeadData["VideoUrl"] = urls.VideoURL;

                leadRequest.IsUrlDerived = true;
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(Base.ISApplication.FormsEngine, ex, "Get creative URLs");
                isEx.Save();
            }
        }
    }
}
