using EDDY.IS.FormsEngine.Core.DTO;
using EDDY.IS.FormsEngine.Core.Interfaces;
using EDDY.IS.FormsEngine.Core.Models;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.Util.StringExtensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EDDY.IS.FormsEngine.Infastructure.Mappers
{
    public class LeadCreateRequestMapper
    {
        public List<LeadCreateRequest> MapFormInputToLeadCreateRequest(FormInput formInput)
        {
            List<LeadCreateRequest> leadCreateRequests = new List<LeadCreateRequest>();

            for (int i = 0; i < formInput?.Matches?.Count; i++)
            {
                var leadCreateRequest = new LeadCreateRequest();
                leadCreateRequest.TemplateId = formInput.TemplateId;
                leadCreateRequest.IsBeta = formInput.IsBeta;
                leadCreateRequest.TrackId = formInput.TrackId;
                leadCreateRequest.ClientRelationContactId = formInput.AdvisorId;
                leadCreateRequest.TrackingSessionGUID = Guid.TryParse(formInput.SessionId, out Guid sessionGuid) ? sessionGuid : Guid.Empty;
                leadCreateRequest.MatchResponseGuid = formInput.MatchResponseGuid;
                leadCreateRequest.LeadCreationTypeId = (int?)formInput.LeadCreationType;
                leadCreateRequest.InitialLeadId = string.IsNullOrWhiteSpace(formInput.InitialLeadId) ? null : formInput.InitialLeadId;
                leadCreateRequest.LimboAlternativeCampaignTrackid = formInput.LimboAlternativeCampaignTrackId;
                leadCreateRequest.LimboAlternativeCampaignTrackidUtilized = formInput.LimboAlternativeCampaignTrackIdUtilized;
                leadCreateRequest.ProspectId = formInput.Prospect?.ProspectId;
                MapLeadDataFields(formInput, leadCreateRequest);

                var match = formInput.Matches[i];
                leadCreateRequest.PassedValidation = match.PassedValidation && formInput.FormValidationResult.Valid;
                leadCreateRequest.ScoreId = match.ScoreId;
                MapFieldsFromMatch(match, leadCreateRequest);


                leadCreateRequests.Add(leadCreateRequest);
            }

            return leadCreateRequests;
        }

        private void MapLeadDataFields(FormInput formInput, LeadCreateRequest leadCreateRequest)
        {
            leadCreateRequest.LeadData = formInput.LeadData.BuildCaseInsensitiveDictionary();
            AddTemplateIdToLeadData(leadCreateRequest.LeadData, formInput.TemplateId);
            RemoveProgramOfInterestFromLeadData(leadCreateRequest.LeadData);
            MapLeadAdditionalData(leadCreateRequest, formInput);
            CleanPhoneNumberInLeadData(leadCreateRequest.LeadData, formInput);
            CleanPostalCodeInLeadData(leadCreateRequest.LeadData, formInput);
        }

        private void MapFieldsFromMatch(Match match, LeadCreateRequest leadCreateRequest)
        {
            leadCreateRequest.ProgramProductId = match.AlternativeProgramProductId.HasValue && match.AlternativeProgramProductId > 0 ? match.AlternativeProgramProductId.Value : match.ProgramProductId;
            leadCreateRequest.PaidStatusType = match.PaidStatusType;
            MapDuplicateFields(match, leadCreateRequest);
            AddInitialLeadValidationFailedReasonToLeadData(match, leadCreateRequest);
            AddInitialLeadValidationFailedToLeadData(leadCreateRequest);
        }

        private void AddTemplateIdToLeadData(Dictionary<string, string> leadData, int templateId)
        {
            if (!leadData.ContainsKey("templateId") && templateId > 0)
            {
                leadData.Add("templateId", templateId.ToString());
            }
        }

        private void RemoveProgramOfInterestFromLeadData(Dictionary<string, string> leadData)
        {
            leadData.Remove("Program_Of_Interest");
        }

        private void MapLeadAdditionalData(LeadCreateRequest leadCreateRequest, FormInput formInput)
        {
            leadCreateRequest.LeadAdditionalData = formInput?.LeadAdditionalData?.BuildCaseInsensitiveDictionary() ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase); ;
            MapLocationIdsToLeadAdditionalData(formInput?.Prospect?.CountryId, formInput?.Prospect?.StateId, leadCreateRequest.LeadAdditionalData);
        }

        private void MapLocationIdsToLeadAdditionalData(int? countryId, int? stateId, Dictionary<string, string> leadAdditionalData)
        {
            if (countryId > 0)
            {
                leadAdditionalData["country-key"] = countryId.ToString();
            }

            if (stateId > 0)
            {
                leadAdditionalData["state-key"] = stateId.ToString();
            }
        }

        private void CleanPhoneNumberInLeadData(Dictionary<string, string> leadData, FormInput formInput)
        {
            string countryCode = formInput.CountryCode?.ToUpper();
            bool isUSOrCanada = countryCode == "US" || countryCode == "CA";

            if (leadData.TryGetValue("phone", out string phoneNumber))
            {
                leadData["phone"] = phoneNumber.CleanPhoneNumber(isUSOrCanada);
            }

            if (leadData.TryGetValue("alternate_phone", out string alternatePhoneNumber))
            {
                leadData["alternate_phone"] = alternatePhoneNumber.CleanPhoneNumber(isUSOrCanada);
            }
        }

        private void CleanPostalCodeInLeadData(Dictionary<string, string> leadData, FormInput formInput)
        {
            string countryCode = formInput.CountryCode?.ToUpper();
            bool isUS = countryCode == "US";
            bool isCanada = countryCode == "CA";

            if (leadData.TryGetValue("postal_code", out string postalCode))
            {
                leadData["postal_code"] = postalCode.CleanZipCode(isUS, isCanada);
            }
        }

        private void MapDuplicateFields(Match match, LeadCreateRequest leadCreateRequest)
        {
            if (!match.PassedValidation)
            {
                if (match.IsInternalDuplicate)
                {
                    leadCreateRequest.IsInternalDuplicate = true;
                }
                else if (match.IsExternalDuplicate)
                {
                    leadCreateRequest.IsExternalDuplicate = true;
                }
            }
        }

        private void AddInitialLeadValidationFailedReasonToLeadData(Match match, LeadCreateRequest leadCreateRequest)
        {
            if (!leadCreateRequest.PassedValidation)
            {
                if (!leadCreateRequest.LeadData.ContainsKey("InitialLeadValidationFailedReason"))
                {
                    KeyValuePair<string, string> ruleFailure = match.RuleFailures.LastOrDefault();
                    if (!string.IsNullOrWhiteSpace(ruleFailure.Key) && !string.IsNullOrWhiteSpace(ruleFailure.Value))
                    {
                        leadCreateRequest.LeadData.Add("InitialLeadValidationFailedReason", $"{ruleFailure.Key}-{ruleFailure.Value}");
                    }
                }
            }
        }

        private void AddInitialLeadValidationFailedToLeadData(LeadCreateRequest leadCreateRequest)
        {
            if (!leadCreateRequest.LeadData.ContainsKey("InitialLeadValidationFailed"))
            {
                leadCreateRequest.LeadData.Add("InitialLeadValidationFailed", (leadCreateRequest.PassedValidation) ? "0" : "1");
            }
        }
    }
}
