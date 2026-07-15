using EDDY.IS.Core.Logging;
using EDDY.IS.ExternalMatch.Base;
using EDDY.IS.MatchingEngine.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.MatchingEngine
{
    public static class ExternalMatchServiceClient
    {
        public static List<ApiMatchResult> GetMatches(BaseMatchRequest matchRequest, int? matchCount, List<MatchItem> allowedExternalMatchCrList, List<EddyInstitution> eddyInstitutionMatchList, Campaign campaign)
        {
            List<ApiMatchResult> result = null;

            try
            {
                ExternalMatchRequest request = new ExternalMatchRequest();

                request.ChannelId = campaign.ChannelId;
                request.TrackGuid = matchRequest.TrackGuid;
                request.ProspectInput = CreateExternalProspectObject(matchRequest.ProspectInput);
                request.CategoryList = matchRequest.CategoryList;
                request.SubjectList = matchRequest.SubjectList;
                request.SpecialtyList = matchRequest.SpecialtyList;
                request.Application = matchRequest.Application;

                request.EddyInstitutionMatchList = eddyInstitutionMatchList;
                request.MatchRequestCount = matchCount;

                request.AllowedExternalMatchCrProgramList = allowedExternalMatchCrList.Select(m => new ExternalMatchCrProgram() { ClientRelationshipId = m.Match.ClientRelationshipId, EddyProgramId = m.Match.ProgramId, RPL = m.Match.eRPL, ProgramCode = m.Match.ProgramCode }).ToList();

                HttpResponseMessage response = CallService(request, "GetMatches");

                result = TranslateResponse<List<ApiMatchResult>>(response);
            }
            catch (Exception ex)
            {
                try
                {
                    ISException isEx = new ISException(ex, matchRequest, matchCount, allowedExternalMatchCrList, eddyInstitutionMatchList);
                    isEx.Save();
                }
                catch { }
            }

            return result;
        }

        private static T TranslateResponse<T>(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
                return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
            else
                return default(T);
        }

        private static HttpResponseMessage CallService(object request, string apiMethod)
         {
            HttpResponseMessage response = null;

            HttpClient client = new HttpClient();

            string baseServiceURL = ConfigurationManager.AppSettings["ExternalMatchProviderServiceURL"];
            string fullUrl = baseServiceURL;

            client.BaseAddress = new Uri(fullUrl);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            response = client.PostAsync(apiMethod, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")).Result;

            return response;
        }

        private static ExternalMatch.Base.ProspectInput CreateExternalProspectObject(DTO.ProspectInput meProspect)
        {
            ExternalMatch.Base.ProspectInput externalProspect = new ExternalMatch.Base.ProspectInput();
            Prospect prospectInput = new Prospect(meProspect);

            externalProspect.AddressLine2 = prospectInput.AddressLine2;
            externalProspect.Age = prospectInput.Age;
            externalProspect.City = prospectInput.City;
            externalProspect.CountryId = prospectInput.CountryId;
            externalProspect.EducationLevelId = prospectInput.EducationLevelId;
            externalProspect.Email = prospectInput.Email;
            externalProspect.ExternalLeadId = !String.IsNullOrEmpty(prospectInput.ExternalLeadId) ? prospectInput.ExternalLeadId.ToUpper() : "";
            externalProspect.FirstName = prospectInput.FirstName;
            externalProspect.GPAKeyValueId = prospectInput.GPAKeyValueId;
            externalProspect.HSGraduationYear = prospectInput.HSGraduationYear;
            externalProspect.IsCitizen = prospectInput.IsCitizen;
            externalProspect.IsMilitary = prospectInput.IsMilitary;
            externalProspect.KVCodeData = prospectInput.KVCodeData;
            externalProspect.LastName = prospectInput.LastName;
            externalProspect.MilitaryStatusId = prospectInput.MilitaryStatusId;
            externalProspect.Phone1 = prospectInput.Phone1;
            externalProspect.Phone2 = prospectInput.Phone2;
            externalProspect.PostalCode = prospectInput.PostalCode;
            externalProspect.StateId = prospectInput.StateId;
            externalProspect.StreetAddress = prospectInput.StreetAddress;
            externalProspect.YearsTeachingExperienceKeyValueId = prospectInput.YearsTeachingExperienceKeyValueId;
            externalProspect.YearsWorkExperienceKeyValueId = prospectInput.YearsWorkExperienceKeyValueId;
            externalProspect.SiteSourceUrl = prospectInput.FormLeadUrl;

            externalProspect.WebConversionPageUrl = !string.IsNullOrEmpty(prospectInput.LeadInitiatingUrl) ? prospectInput.LeadInitiatingUrl : prospectInput.FormLeadUrl ;
            return externalProspect;
        }
    }
}
