using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.LeadEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EDDY.IS.FormsEngine.DTO.Extended
{
    public class ProspectResubmitRequestDTO
    {
        public string LeadData { get; set; }
        public LeadCreateRequest LeadRequest { get; set; }
        public string RenderingStrategy { get; set; }
        public ProspectInput ProspectInput { get; set; }
        public CampusPreference? CampusSoftPreference { get; set; }
        public string UserFullName { get; set; }
        public string TrackId { get; set; }
        public string MatchRecordId { get; set; }
        public string FESessionId { get; set; }
        public int? ApplicationId { get; set; }

        public ProspectResubmitRequestDTO(VW_ProspectResubmissionsDTO resubmissionDTO)
        {
            WizardMatchRequest matchRequest = JsonConvert.DeserializeObject<WizardMatchRequest>(resubmissionDTO.RequestInput);
            RenderingStrategy = "WIZARDPLAIN";
            ProspectInput = matchRequest.ProspectInput;
            CampusSoftPreference = matchRequest.CampusPreference;
            UserFullName = matchRequest.ProspectInput.FirstName + " " + matchRequest.ProspectInput.LastName;
            TrackId = resubmissionDTO.CampaignTrackId.ToString();
            MatchRecordId = resubmissionDTO.MatchResponseGUID.ToString();
            FESessionId = resubmissionDTO.TrackingSessionGuid.ToString();
            ApplicationId = matchRequest.ApplicationId;
            string k12 = matchRequest.ProspectInput.KVCodeData.Where(x => x.Key.Equals("K12")).Any() ? matchRequest.ProspectInput.KVCodeData.Where(x => x.Key.Equals("K12")).FirstOrDefault().Value.ToString() : string.Empty;

            LeadData = @"TemplateId=112" + resubmissionDTO.Templateid +
                        "&RenderingStrategy=WIZARDPROFESSIONALBOOTSTRAP&IsBeta=false&TrackId=" + resubmissionDTO.CampaignTrackId +
                        "&SessionId=" + FESessionId +
                        "&MatchGuid=" + matchRequest +
                        "&ProspectId=" + resubmissionDTO.ProspectId +
                        "&Highest_Level_of_Education_Completed=" + matchRequest.ProspectInput.EducationLevelId +
                        "&Year_of_Highest_Education_Completed=" + matchRequest.ProspectInput.HSGraduationYear +
                        "&GPA-key=" + matchRequest.ProspectInput.GPAKeyValueId +
                        "&Age=" + matchRequest.ProspectInput.Age +
                        "&Postal_Code=" + matchRequest.ProspectInput.PostalCode +
                        "&Categories=";
            foreach (int category in matchRequest.CategoryList)
            {
                LeadData = LeadData + category + ",";
            }
            LeadData = LeadData.Remove(LeadData.Length - 1, 1);
            LeadData += "&SubCategories=";
            foreach (int subject in matchRequest.SubjectList)
            {
                LeadData = LeadData + subject + ",";
            }
            LeadData = LeadData.Remove(LeadData.Length - 1, 1);
            LeadData = LeadData + "&First_Name=" + matchRequest.ProspectInput.FirstName +
            "&leadid_token=" + matchRequest.ProspectInput.ExternalLeadId +
            "&Last_Name=" + matchRequest.ProspectInput.LastName +
            "&Email=" + matchRequest.ProspectInput.Email +
            "&Address=" + matchRequest.ProspectInput.StreetAddress +
            "&City=" + matchRequest.ProspectInput.City +
            "&State=" + resubmissionDTO.State +
            "&Country=" + resubmissionDTO.Country +
            "&Postal_Code_Duplicate=" + matchRequest.ProspectInput.PostalCode +
            "&Phone=" + matchRequest.ProspectInput.Phone1 +
            "&Desired_Start_Date=" + (!String.IsNullOrWhiteSpace(resubmissionDTO.DesiredStartDate) ? resubmissionDTO.DesiredStartDate : "Immediately") +
            "&us_citizen=" + ((matchRequest.ProspectInput.IsCitizen == null || (bool)matchRequest.ProspectInput.IsCitizen) ? "Yes" : "No") +
            "&Military_Affiliation=" + matchRequest.ProspectInput.KVCodeData.Where(x => x.Key.Equals("Military_Affiliation")).FirstOrDefault().Value +
            "&K12=" + (k12.Equals("23") ? "Yes" : "No") +
            "&AffiliateId=" +
            "&AdditionalQuestionsShown.Start=false" +
            "&CampusSoftPreferenceShown=false" +
            "&CampusPreferenceShown=false" +
            "&AdditionalData=Highest_Level_of_Education_Completed-key=" + matchRequest.ProspectInput.EducationLevelId +
            "&Year_of_Highest_Education_Completed-key=" + matchRequest.ProspectInput.HSGraduationYear +
            "&State-key=" + matchRequest.ProspectInput.StateId +
            "&Country-key=" + matchRequest.ProspectInput.CountryId +
            "&Desired_Start_Date-key=" + matchRequest.ProspectInput.KVCodeData.Where(x => x.Key.Equals("Desired_Start_Date")).FirstOrDefault().Value +
            "&us_citizen-key=" + matchRequest.ProspectInput.KVCodeData.Where(x => x.Key.Equals("us_citizen")).FirstOrDefault().Value +
            "&DynamicCampusSoftPreference-key=" + matchRequest.CampusPreference +
            "&Military_Affiliation-key=" + matchRequest.ProspectInput.KVCodeData.Where(x => x.Key.Equals("Military_Affiliation")).FirstOrDefault().Value +
            "&Theme=default" +
            "&SMLeadsCreatedCount=0" +
            "&USLeadsCreatedCount=0" +
            "&SplitCampusTypeInResults=undefined" +
            "&FESessionId=" + resubmissionDTO.TrackingSessionGuid +
            "&DeviceId=undefined" +
            "&RenderingExperience=undefined" +
            "&LimboAlternativeCampaignTrackid=" +
            "&LimboAlternativeCampaignTrackidUtilized=false" +
            "&FormTemplateType=2" +
            "&ProgramTemplateId=" +
            "ProgramProductId=" +
            "&ProductId=" +
            "&InstitutionId=undefined" +
            "&ApplicationId=" + matchRequest.ApplicationId +
            "&InstitutionName=undefined" +
            "&ProgramName=undefined";
            //Adding KVCode Data
            foreach (KeyValuePair<string, int> kvCode in matchRequest.ProspectInput.KVCodeData)
            {
                LeadData += ("&" + kvCode.Key + "-key=" + kvCode.Value);
            }

            LeadRequest = EntityBuildHelper.BuildLeadCreateRequestObject(resubmissionDTO.Templateid, null, false, TrackId, null, false, FESessionId, MatchRecordId, LeadData, null, null, null, resubmissionDTO.ProspectId);
        }
    }
}
