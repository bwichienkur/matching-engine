using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public class Prospect
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone1 { get; set; }
        public string Phone2 { get; set; }
        public string StreetAddress { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public int? Age { get; set; }
        public string PostalCode { get; set; }
        public int? StateId { get; set; }
        public int? CountryId { get; set; }
        public int? EducationLevelId { get; set; }
        public int? HSGraduationYear { get; set; }
        public bool? IsCitizen { get; set; }
        public int? GPAKeyValueId { get; set; }
        public int? YearsWorkExperienceKeyValueId { get; set; }
        public int? YearsTeachingExperienceKeyValueId { get; set; }
        public bool? IsMilitary { get; set; }
        public int? MilitaryStatusId { get; set; }
        public List<KeyValuePair<string, int>> KVCodeData { get; set; }
        public string ExternalLeadId { get; set; }
        public bool? IsMobileNumber { get; set; }
        public Guid? CampaignTrackId { get; set; }
        public string TrackingSessionGUID { get; set; }
        public string CecUniqueId { get; set; }
        public bool IsValid { get; private set; }

        public string FormLeadUrl 
        { 
            get
            {
                return GetDynamicFieldValue(nameof(this.FormLeadUrl));
            }
        }
        public string LeadInitiatingUrl 
        { 
            get 
            {
                return GetDynamicFieldValue(nameof(this.LeadInitiatingUrl));
            } 
        }
        public string LeadSourceUrl 
        { 
            get
            {
                return GetDynamicFieldValue(nameof(this.LeadSourceUrl));
            }
        }
        public string AffiliateId
        {
            get
            {
                return GetDynamicFieldValue(nameof(this.AffiliateId));
            }
        }
        public string SubSource1
        {
            get
            {
                return GetDynamicFieldValue(nameof(this.SubSource1));
            }
        }
        public string SubSource2
        { 
            get
            {
                return GetDynamicFieldValue(nameof(this.SubSource2));
            }
        }

        public string SourceCode
        {
            get
            {
                return GetDynamicFieldValue(nameof(this.SourceCode));
            }
        }

        public string InitiatingURL
        {
            get
            {
                var initiatingUrl = GetDynamicFieldValue(nameof(this.InitiatingURL));
                return initiatingUrl == null ? "" : initiatingUrl;
            }
        }

        public string LandingURL
        {
            get
            {
                var landingUrl = GetDynamicFieldValue(nameof(this.LandingURL));
                return landingUrl == null ? "" : landingUrl;
            }
        }

        public string CallCenterURL
        {
            get
            {
                var callCenterUrl = GetDynamicFieldValue(nameof(this.CallCenterURL));
                return callCenterUrl == null ? "" : callCenterUrl;
            }
        }

        public string VideoUrl
        {
            get
            {
                var videoUrl = GetDynamicFieldValue(nameof(this.VideoUrl));
                return videoUrl == null ? "" : videoUrl;
            }
        }

        public string utm_campaign
        {
            get
            {
                var utmCampaign = GetDynamicFieldValue(nameof(this.utm_campaign));
                return utmCampaign == null ? "" : utmCampaign;
            }
        }

        public string utm_source
        {
            get
            {
                var utmSource = GetDynamicFieldValue(nameof(this.utm_source));
                return utmSource == null ? "" : utmSource;
            }
        }

        public string utm_medium
        {
            get
            {
                var utmMedium = GetDynamicFieldValue(nameof(this.utm_medium));
                return utmMedium == null ? "" : utmMedium;
            }
        }

        private readonly Dictionary<string, string> DynamicFields;

        public Prospect(DTO.ProspectInput prospectInput)
        {
            if (prospectInput == null)
            {
                IsValid = false;
                return;
            }
            else
            {
                IsValid = true;
            }

            this.FirstName = prospectInput.FirstName;
            this.LastName = prospectInput.LastName;
            this.Email = prospectInput.Email;
            this.Phone1 = prospectInput.Phone1;
            this.Phone2 = prospectInput.Phone2;
            this.StreetAddress = prospectInput.StreetAddress;
            this.AddressLine2 = prospectInput.AddressLine2;
            this.City = prospectInput.City;
            this.Age = prospectInput.Age;
            this.PostalCode = prospectInput.PostalCode;
            this.StateId = prospectInput.StateId;
            this.CountryId = prospectInput.CountryId;
            this.EducationLevelId = prospectInput.EducationLevelId;
            this.HSGraduationYear = prospectInput.HSGraduationYear;
            this.IsCitizen = prospectInput.IsCitizen;
            this.GPAKeyValueId = prospectInput.GPAKeyValueId;
            this.YearsWorkExperienceKeyValueId = prospectInput.YearsWorkExperienceKeyValueId;
            this.IsMilitary = prospectInput.IsMilitary;
            this.MilitaryStatusId = prospectInput.MilitaryStatusId;
            this.KVCodeData = prospectInput.KVCodeData;
            this.ExternalLeadId = prospectInput.ExternalLeadId;
            this.IsMobileNumber = prospectInput.IsMobileNumber;
            this.CampaignTrackId = prospectInput.CampaignTrackId;
            this.TrackingSessionGUID = prospectInput.TrackingSessionGUID;
            this.CecUniqueId = prospectInput.CecUniqueId;
            this.DynamicFields = MapDynamicFields(prospectInput.DynamicFields);
        }

        private Dictionary<string, string> MapDynamicFields(List<KeyValuePair<string, string>> dynamicFields)
        {
            var result = new Dictionary<string, string>();

            for (int i = 0; i < dynamicFields?.Count; i++)
            {
                KeyValuePair<string, string> field = dynamicFields[i];
                if (!string.IsNullOrEmpty(field.Key) && !string.IsNullOrEmpty(field.Value) && !result.ContainsKey(field.Key))
                {
                    result.Add(field.Key, field.Value);
                }
            }

            return result;
        }

        private string GetDynamicFieldValue(string key)
        {
            string value = null;

            if (!string.IsNullOrWhiteSpace(key))
            {
                this.DynamicFields?.TryGetValue(key, out value);
            }

            return value;
        }
    }
}
