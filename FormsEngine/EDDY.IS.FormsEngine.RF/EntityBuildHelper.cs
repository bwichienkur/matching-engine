using EDDY.IS.LeadEngine.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.Validation;
using EDDY.IS.Util.StringExtensions;
using EDDY.IS.FormsEngine.MatchingEngine;
using EDDY.IS.FormsEngine.ProspectService;
using EDDY.IS.Core;
using EDDY.IS.Base;
using System.Web;

namespace EDDY.IS.FormsEngine
{
    public class EntityBuildHelper
    {
        #region Lead Create Request for Lead Service

        public static LeadCreateRequest BuildLeadCreateRequestObject(
                 int TemplateId
                , int? ProgramProductId
                , bool IsBeta
                , string TrackId
                , string AlternativeTrackId
                , bool AlternativeTrackIdUtilized
                , string TrackingSessionGUID
                , string MatchResponseGuid
                , string LeadData
                , string LeadAdditionalData
                , string InitialLeadId
                , LeadCreationType? LeadCreationType
                , int? ProspectId)
        {
            LeadCreateRequest LeadRequest = new LeadCreateRequest();
            Guid parseGuid;
            
            LeadRequest.TemplateId = TemplateId;
            LeadRequest.ProgramProductId = ProgramProductId.HasValue && ProgramProductId.Value > 0 ? ProgramProductId.Value : default(int);
            LeadRequest.IsBeta = IsBeta;
            LeadRequest.TrackId = Guid.TryParse(TrackId, out parseGuid) ? parseGuid : default(Guid?);
            LeadRequest.LimboAlternativeCampaignTrackid = Guid.TryParse(AlternativeTrackId, out parseGuid) ? parseGuid : default(Guid?);
            LeadRequest.LimboAlternativeCampaignTrackidUtilized = AlternativeTrackIdUtilized;
            LeadRequest.TrackingSessionGUID = Guid.TryParse(TrackingSessionGUID, out parseGuid) ? parseGuid : default(Guid?);
            LeadRequest.MatchResponseGuid = Guid.TryParse(MatchResponseGuid, out parseGuid) ? parseGuid : default(Guid?);
            LeadRequest.LeadCreationTypeId = LeadCreationType.HasValue ? (int)LeadCreationType.Value : default(int?);
            LeadRequest.InitialLeadId = String.IsNullOrWhiteSpace(InitialLeadId) ? null : InitialLeadId;
            LeadRequest.ProspectId = ProspectId;

            //Process Lead Data
            LeadRequest.LeadData = LeadData.BuildCaseInsensitiveDictionary();
            //Add AdvisorId
            var advisor = LeadRequest.LeadData.ContainsKey("advisorid") ? LeadRequest.LeadData["advisorid"] : null;
            if (!string.IsNullOrWhiteSpace(advisor))
            {
                int intValue = 0;
                LeadRequest.ClientRelationContactId = int.TryParse(advisor, out intValue) ? intValue : (int?)null;
            }

            //Add Additional fields required by delivery
            if (!LeadRequest.LeadData.ContainsKey("templateId"))
            {
                LeadRequest.LeadData.Add("templateId", TemplateId.ToString());
            }

            //Remove fields not used
            if (LeadRequest.LeadData.ContainsKey("Program_Of_Interest"))
            {
                LeadRequest.LeadData.Remove("Program_Of_Interest"); //ProgramId is not useful anymore
            }

            //Lead Additional Data
            string CountryCode = LeadRequest.LeadData.ContainsKey("country") ? LeadRequest.LeadData["country"].ToUpper() : null;
            string StateShortName = LeadRequest.LeadData.ContainsKey("state") ? LeadRequest.LeadData["state"].ToUpper() : null;
            if (String.IsNullOrWhiteSpace(LeadAdditionalData))
            {
                LeadRequest.LeadAdditionalData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrWhiteSpace(CountryCode))
                {
                    var CountryIdStateId = new ValidationEngine().GetCountryIdStateId(CountryCode, StateShortName);
                    if (CountryIdStateId.Item1.HasValue)
                    {
                        LeadRequest.LeadAdditionalData.Add("country-key", CountryIdStateId.Item1.Value.ToString());
                    }
                    if (CountryIdStateId.Item2.HasValue)
                    {
                        LeadRequest.LeadAdditionalData.Add("state-key", CountryIdStateId.Item2.Value.ToString());
                    }
                }
            }
            else
            {
                LeadRequest.LeadAdditionalData = LeadAdditionalData.BuildCaseInsensitiveDictionary();
            }

            //Phone Number cleanup requested by Venkat-Pete
            bool IsUS = CountryCode == "US";
            bool IsCanada = CountryCode == "CA";
            bool IsUSorCanada = IsUS || IsCanada;

            if (LeadRequest.LeadData.ContainsKey("phone"))
            {
                string Phone = LeadRequest.LeadData["phone"];
                if (Phone != null)
                {
                    LeadRequest.LeadData["phone"] = Phone.CleanPhoneNumber(IsUSorCanada);
                }
            }

            if (LeadRequest.LeadData.ContainsKey("alternate_phone"))
            {
                string Phone = LeadRequest.LeadData["alternate_phone"];
                if (Phone != null)
                {
                    LeadRequest.LeadData["alternate_phone"] = Phone.CleanPhoneNumber(IsUSorCanada);
                }
            }

            if (LeadRequest.LeadData.ContainsKey("postal_code"))
            {
                string PostalCode = LeadRequest.LeadData["postal_code"];
                if (PostalCode != null)
                {
                    LeadRequest.LeadData["postal_code"] = PostalCode.CleanZipCode(IsUS, IsCanada);
                }
            }

            if (LeadRequest.LeadData.ContainsKey("ExternalMatchItemGuid"))
            {
                string externalMatchItemGuidString = LeadRequest.LeadData["ExternalMatchItemGuid"];
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["ExternalMatchItemGuid"]))
                {
                    Guid externalMatchItemGuid;
                    if (Guid.TryParse(LeadRequest.LeadData["ExternalMatchItemGuid"], out externalMatchItemGuid))
                    {
                        LeadRequest.ExternalMatchItemGuid = externalMatchItemGuid;
                    }
                }
            }

            if(!LeadRequest.LeadData.ContainsKey("VideoUrl") && LeadRequest.LeadData.ContainsKey("FormLeadUrl"))
            {
                string formLeadUrl = LeadRequest.LeadData["FormLeadUrl"];
                if (!string.IsNullOrEmpty(formLeadUrl))
                    if (formLeadUrl.ToLower().Contains("videourl"))
                    {
                        Uri url = new Uri(HttpUtility.UrlDecode(formLeadUrl));
                        var queryParams = HttpUtility.ParseQueryString(url.Query);
                        string videoUrl = queryParams["videourl"];

                        if(!string.IsNullOrEmpty(videoUrl))
                            LeadRequest.LeadData.Add("VideoUrl", videoUrl);
                        
                    }
            }

            //Remove fields not used
            if (LeadRequest.LeadData.ContainsKey("PreValidatedProgram"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["PreValidatedProgram"]))
                {
                    bool preValidatedProgram;
                    if (bool.TryParse(LeadRequest.LeadData["PreValidatedProgram"], out preValidatedProgram))
                    {
                        LeadRequest.PreValidatedProgram = preValidatedProgram;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("ProspectFlowId"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["ProspectFlowId"]))
                {
                    int prospectFlowId;
                    if (int.TryParse(LeadRequest.LeadData["ProspectFlowId"], out prospectFlowId))
                    {
                        LeadRequest.ProspectFlowId = prospectFlowId;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("ChannelId"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["ChannelId"]))
                {
                    int channelid=0;
                    if (int.TryParse(LeadRequest.LeadData["ChannelId"], out channelid))
                    {
                        LeadRequest.ChannelId = channelid;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("SubChannelId"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["SubChannelId"]))
                {
                    int SubChannelId;
                    if (int.TryParse(LeadRequest.LeadData["SubChannelId"], out SubChannelId))
                    {
                        LeadRequest.SubChannelId = SubChannelId;
                    }
                }
            }


            if (LeadRequest.LeadData.ContainsKey("TrackingSessionGuid"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["TrackingSessionGuid"]))
                {
                    Guid trackingSessionGUID;
                    if (Guid.TryParse(LeadRequest.LeadData["TrackingSessionGuid"], out trackingSessionGUID))
                    {
                        LeadRequest.TrackingSessionGUID = trackingSessionGUID;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("leadid_token"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["leadid_token"]))
                {
                    Guid externalLeadId;
                    if (Guid.TryParse(LeadRequest.LeadData["leadid_token"], out externalLeadId))
                    {
                        LeadRequest.ExternalLeadId = externalLeadId;
                    }
                }
            }
            if (LeadRequest.LeadData.ContainsKey("EstimatedRevShare"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["EstimatedRevShare"]))
                {
                    decimal estimatedRevShare;
                    if (decimal.TryParse(LeadRequest.LeadData["EstimatedRevShare"], out estimatedRevShare))
                    {
                        LeadRequest.EstimatedRevShare = estimatedRevShare;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("ValidateTCPA"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["ValidateTCPA"]))
                {
                    bool validateTCPA;
                    if (bool.TryParse(LeadRequest.LeadData["ValidateTCPA"], out validateTCPA))
                    {
                        LeadRequest.ValidateTCPA = validateTCPA;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("WidgetRequestGuid"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["WidgetRequestGuid"]))
                {
                    Guid widgetRequestGuid;
                    if (Guid.TryParse(LeadRequest.LeadData["WidgetRequestGuid"], out widgetRequestGuid))
                    {
                        LeadRequest.WidgetRequestGuid = widgetRequestGuid;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("WidgetName"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["WidgetName"]))
                {
                    LeadRequest.WidgetName = LeadRequest.LeadData["WidgetName"];
                }
            }

            return LeadRequest;
        }

        public static LeadCreateRequest BuildLeadCreateRequestObjectCustomTCPA(
         int TemplateId
        , int? ProgramProductId
        , bool IsBeta
        , string TrackId
        , string AlternativeTrackId
        , bool AlternativeTrackIdUtilized
        , string TrackingSessionGUID
        , string MatchResponseGuid
        , string LeadData
        , string LeadAdditionalData
        , string InitialLeadId
        , LeadCreationType? LeadCreationType
        , int? ProspectId
        , string CustomTCPA)
        {
            LeadCreateRequest LeadRequest = new LeadCreateRequest();
            Guid parseGuid;

            LeadRequest.TemplateId = TemplateId;
            LeadRequest.ProgramProductId = ProgramProductId.HasValue && ProgramProductId.Value > 0 ? ProgramProductId.Value : default(int);
            LeadRequest.IsBeta = IsBeta;
            LeadRequest.TrackId = Guid.TryParse(TrackId, out parseGuid) ? parseGuid : default(Guid?);
            LeadRequest.LimboAlternativeCampaignTrackid = Guid.TryParse(AlternativeTrackId, out parseGuid) ? parseGuid : default(Guid?);
            LeadRequest.LimboAlternativeCampaignTrackidUtilized = AlternativeTrackIdUtilized;
            LeadRequest.TrackingSessionGUID = Guid.TryParse(TrackingSessionGUID, out parseGuid) ? parseGuid : default(Guid?);
            LeadRequest.MatchResponseGuid = Guid.TryParse(MatchResponseGuid, out parseGuid) ? parseGuid : default(Guid?);
            LeadRequest.LeadCreationTypeId = LeadCreationType.HasValue ? (int)LeadCreationType.Value : default(int?);
            LeadRequest.InitialLeadId = String.IsNullOrWhiteSpace(InitialLeadId) ? null : InitialLeadId;
            LeadRequest.ProspectId = ProspectId;

            //Process Lead Data
            LeadRequest.LeadData = LeadData.BuildCaseInsensitiveDictionary();
            //Add AdvisorId
            var advisor = LeadRequest.LeadData.ContainsKey("advisorid") ? LeadRequest.LeadData["advisorid"] : null;
            if (!string.IsNullOrWhiteSpace(advisor))
            {
                int intValue = 0;
                LeadRequest.ClientRelationContactId = int.TryParse(advisor, out intValue) ? intValue : (int?)null;
            }

            //Add Additional fields required by delivery
            if (!LeadRequest.LeadData.ContainsKey("templateId"))
            {
                LeadRequest.LeadData.Add("templateId", TemplateId.ToString());
            }

            //Remove fields not used
            if (LeadRequest.LeadData.ContainsKey("Program_Of_Interest"))
            {
                LeadRequest.LeadData.Remove("Program_Of_Interest"); //ProgramId is not useful anymore
            }


            if (LeadRequest.LeadData.ContainsKey("UserAgreement") && !String.IsNullOrEmpty(CustomTCPA))
            {
                LeadRequest.LeadData["UserAgreement"] = CustomTCPA;
            }
            else if (!LeadRequest.LeadData.ContainsKey("UserAgreement") && !String.IsNullOrEmpty(CustomTCPA))
            {
                LeadRequest.LeadData.Add("UserAgreement", CustomTCPA);
            }

            //Lead Additional Data
            string CountryCode = LeadRequest.LeadData.ContainsKey("country") ? LeadRequest.LeadData["country"].ToUpper() : null;
            string StateShortName = LeadRequest.LeadData.ContainsKey("state") ? LeadRequest.LeadData["state"].ToUpper() : null;
            if (String.IsNullOrWhiteSpace(LeadAdditionalData))
            {
                LeadRequest.LeadAdditionalData = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                if (!string.IsNullOrWhiteSpace(CountryCode))
                {
                    var CountryIdStateId = new ValidationEngine().GetCountryIdStateId(CountryCode, StateShortName);
                    if (CountryIdStateId.Item1.HasValue)
                    {
                        LeadRequest.LeadAdditionalData.Add("country-key", CountryIdStateId.Item1.Value.ToString());
                    }
                    if (CountryIdStateId.Item2.HasValue)
                    {
                        LeadRequest.LeadAdditionalData.Add("state-key", CountryIdStateId.Item2.Value.ToString());
                    }
                }
            }
            else
            {
                LeadRequest.LeadAdditionalData = LeadAdditionalData.BuildCaseInsensitiveDictionary();
            }

            //Phone Number cleanup requested by Venkat-Pete
            bool IsUS = CountryCode == "US";
            bool IsCanada = CountryCode == "CA";
            bool IsUSorCanada = IsUS || IsCanada;

            if (LeadRequest.LeadData.ContainsKey("phone"))
            {
                string Phone = LeadRequest.LeadData["phone"];
                if (Phone != null)
                {
                    LeadRequest.LeadData["phone"] = Phone.CleanPhoneNumber(IsUSorCanada);
                }
            }

            if (LeadRequest.LeadData.ContainsKey("alternate_phone"))
            {
                string Phone = LeadRequest.LeadData["alternate_phone"];
                if (Phone != null)
                {
                    LeadRequest.LeadData["alternate_phone"] = Phone.CleanPhoneNumber(IsUSorCanada);
                }
            }

            if (LeadRequest.LeadData.ContainsKey("postal_code"))
            {
                string PostalCode = LeadRequest.LeadData["postal_code"];
                if (PostalCode != null)
                {
                    LeadRequest.LeadData["postal_code"] = PostalCode.CleanZipCode(IsUS, IsCanada);
                }
            }

            if (LeadRequest.LeadData.ContainsKey("ExternalMatchItemGuid"))
            {
                string externalMatchItemGuidString = LeadRequest.LeadData["ExternalMatchItemGuid"];
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["ExternalMatchItemGuid"]))
                {
                    Guid externalMatchItemGuid;
                    if (Guid.TryParse(LeadRequest.LeadData["ExternalMatchItemGuid"], out externalMatchItemGuid))
                    {
                        LeadRequest.ExternalMatchItemGuid = externalMatchItemGuid;
                    }
                }
            }

            //Remove fields not used
            if (LeadRequest.LeadData.ContainsKey("PreValidatedProgram"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["PreValidatedProgram"]))
                {
                    bool preValidatedProgram;
                    if (bool.TryParse(LeadRequest.LeadData["PreValidatedProgram"], out preValidatedProgram))
                    {
                        LeadRequest.PreValidatedProgram = preValidatedProgram;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("ProspectFlowId"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["ProspectFlowId"]))
                {
                    int prospectFlowId;
                    if (int.TryParse(LeadRequest.LeadData["ProspectFlowId"], out prospectFlowId))
                    {
                        LeadRequest.ProspectFlowId = prospectFlowId;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("ChannelId"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["ChannelId"]))
                {
                    int channelid = 0;
                    if (int.TryParse(LeadRequest.LeadData["ChannelId"], out channelid))
                    {
                        LeadRequest.ChannelId = channelid;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("SubChannelId"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["SubChannelId"]))
                {
                    int SubChannelId;
                    if (int.TryParse(LeadRequest.LeadData["SubChannelId"], out SubChannelId))
                    {
                        LeadRequest.SubChannelId = SubChannelId;
                    }
                }
            }


            if (LeadRequest.LeadData.ContainsKey("TrackingSessionGuid"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["TrackingSessionGuid"]))
                {
                    Guid trackingSessionGUID;
                    if (Guid.TryParse(LeadRequest.LeadData["TrackingSessionGuid"], out trackingSessionGUID))
                    {
                        LeadRequest.TrackingSessionGUID = trackingSessionGUID;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("leadid_token"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["leadid_token"]))
                {
                    Guid externalLeadId;
                    if (Guid.TryParse(LeadRequest.LeadData["leadid_token"], out externalLeadId))
                    {
                        LeadRequest.ExternalLeadId = externalLeadId;
                    }
                }
            }
            if (LeadRequest.LeadData.ContainsKey("EstimatedRevShare"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["EstimatedRevShare"]))
                {
                    decimal estimatedRevShare;
                    if (decimal.TryParse(LeadRequest.LeadData["EstimatedRevShare"], out estimatedRevShare))
                    {
                        LeadRequest.EstimatedRevShare = estimatedRevShare;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("ValidateTCPA"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["ValidateTCPA"]))
                {
                    bool validateTCPA;
                    if (bool.TryParse(LeadRequest.LeadData["ValidateTCPA"], out validateTCPA))
                    {
                        LeadRequest.ValidateTCPA = validateTCPA;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("WidgetRequestGuid"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["WidgetRequestGuid"]))
                {
                    Guid widgetRequestGuid;
                    if (Guid.TryParse(LeadRequest.LeadData["WidgetRequestGuid"], out widgetRequestGuid))
                    {
                        LeadRequest.WidgetRequestGuid = widgetRequestGuid;
                    }
                }
            }

            if (LeadRequest.LeadData.ContainsKey("WidgetName"))
            {
                if (!string.IsNullOrEmpty(LeadRequest.LeadData["WidgetName"]))
                {
                    LeadRequest.WidgetName = LeadRequest.LeadData["WidgetName"];
                }
            }

            if (!LeadRequest.LeadData.ContainsKey("VideoUrl") && LeadRequest.LeadData.ContainsKey("FormLeadUrl"))
            {
                string formLeadUrl = LeadRequest.LeadData["FormLeadUrl"];
                if (!string.IsNullOrEmpty(formLeadUrl))
                    if (formLeadUrl.ToLower().Contains("videourl"))
                    {
                        Uri url = new Uri(HttpUtility.UrlDecode(formLeadUrl));
                        var queryParams = HttpUtility.ParseQueryString(url.Query);
                        string videoUrl = queryParams["videourl"];

                        if (!string.IsNullOrEmpty(videoUrl)) {
                            LeadRequest.VideoUrl = videoUrl;
                            if (!LeadRequest.LeadData.ContainsKey("VideoUrl"))
                            {
                                LeadRequest.LeadData.Add("VideoUrl", videoUrl);
                            }
                        }
                    }
            }

            return LeadRequest;
        }

        #endregion Lead Create Request for Lead Service


        #region SaveProspectRequest For Prospect Service
        /// <summary>
        /// Prospect Service DTO
        /// </summary>
        /// <param name="LeadRequest"></param>
        /// <returns></returns>
        public static SaveProspectRequest BuildSaveProspectRequest(LeadCreateRequest LeadRequest, Guid TrackId, ProspectFlowTypes pft, string Notes = null)
        {
            SaveProspectRequest prospectRequest = new SaveProspectRequest();
            prospectRequest.Prospect = new ProspectDTO();
            prospectRequest.ProspectFlowDetails = new ProspectFlowDetailsDTO();
            prospectRequest.ProspectFlowDetails.ProspectFlowTypeId = (int)pft;

            if (pft == ProspectFlowTypes.Advising)
            {
                prospectRequest.ProspectFlowDetails.AreaofInterest = StringExtensions.GetFieldValue("category", LeadRequest.LeadData);
            }

            prospectRequest.Prospect.FirstName = StringExtensions.GetFieldValue("first_name", LeadRequest.LeadData);
            prospectRequest.Prospect.LastName = StringExtensions.GetFieldValue("last_name", LeadRequest.LeadData);
            string emailAddress = StringExtensions.GetFieldValue("email", LeadRequest.LeadData);
            prospectRequest.Prospect.Email = string.IsNullOrEmpty(emailAddress) ? null : emailAddress;
            prospectRequest.Prospect.Address1 = StringExtensions.GetFieldValue("address", LeadRequest.LeadData);
            prospectRequest.Prospect.Address2 = StringExtensions.GetFieldValue("address_2", LeadRequest.LeadData);
            prospectRequest.Prospect.Age = StringExtensions.GetFieldValue("age", LeadRequest.LeadData, null);
            prospectRequest.Prospect.Phone = StringExtensions.GetFieldValue("phone", LeadRequest.LeadData);
            prospectRequest.Prospect.OtherPhone = StringExtensions.GetFieldValue("alternate_phone", LeadRequest.LeadData);
            prospectRequest.Prospect.City = StringExtensions.GetFieldValue("city", LeadRequest.LeadData);
            prospectRequest.Prospect.CountryID = StringExtensions.GetFieldValue("country-key", LeadRequest.LeadAdditionalData, null);
            prospectRequest.Prospect.StateID = StringExtensions.GetFieldValue("state-key", LeadRequest.LeadAdditionalData, null);
            prospectRequest.Prospect.EducationLevelID = StringExtensions.GetFieldValue("highest_level_of_education_completed", LeadRequest.LeadData, null);
            if (!string.IsNullOrEmpty(StringExtensions.GetFieldValue("year_of_highest_education_completed", LeadRequest.LeadData))) //high school
            {
                prospectRequest.Prospect.GraduationYear = StringExtensions.GetFieldValue("year_of_highest_education_completed", LeadRequest.LeadData, null);
            }
            else if (!string.IsNullOrEmpty(StringExtensions.GetFieldValue("graduationyear", LeadRequest.LeadData))) //generic for GS
            {
                prospectRequest.Prospect.GraduationYear = StringExtensions.GetFieldValue("graduationyear", LeadRequest.LeadData, null);
            }
            prospectRequest.Prospect.MilitaryStatusID = StringExtensions.GetFieldValue("military_affiliation", LeadRequest.LeadData, null);
            prospectRequest.Prospect.PostalCode = StringExtensions.GetFieldValue("postal_code", LeadRequest.LeadData);
            prospectRequest.Prospect.PreferEmail = StringExtensions.GetFieldValue("preferred_methods_of_contact", LeadRequest.LeadData).ToLower().Contains("email");
            prospectRequest.Prospect.PreferPhone = StringExtensions.GetFieldValue("preferred_methods_of_contact", LeadRequest.LeadData).ToLower().Contains("phone");
            prospectRequest.Prospect.PreferText = StringExtensions.GetFieldValue("preferred_methods_of_contact", LeadRequest.LeadData).ToLower().Contains("text");
            //if statements to not set if question not answered
            if (!string.IsNullOrEmpty(StringExtensions.GetFieldValue("us_citizen", LeadRequest.LeadData)))
            {
                prospectRequest.Prospect.IsUsCitizen = StringExtensions.GetFieldValue("us_citizen", LeadRequest.LeadData) == "Yes";
            }
            if (!string.IsNullOrEmpty(StringExtensions.GetFieldValue("financialaid", LeadRequest.LeadData)))
            {
                prospectRequest.Prospect.NeedsFinancialAid = StringExtensions.GetFieldValue("financialaid", LeadRequest.LeadData) == "Yes" ? false : true;//yes means they have the money so needs aid is false
            }
            if (!string.IsNullOrEmpty(StringExtensions.GetFieldValue("desired_start_date", LeadRequest.LeadData)))
            {
                prospectRequest.Prospect.DesiredStartDate = StringExtensions.GetFieldValue("desired_start_date", LeadRequest.LeadData);
            }
            prospectRequest.ProspectFlowDetails.TrackId = TrackId;
            prospectRequest.ProspectFlowDetails.SessionGuid = LeadRequest.TrackingSessionGUID;

            string CountryCode = LeadRequest.LeadData.ContainsKey("country") ? LeadRequest.LeadData["country"].ToUpper() : null;


            //Phone Number cleanup
            bool IsUS = !prospectRequest.Prospect.CountryID.HasValue ? false : prospectRequest.Prospect.CountryID == 4;
            bool IsCanada = !prospectRequest.Prospect.CountryID.HasValue ? false : prospectRequest.Prospect.CountryID == 5;
            bool IsUSorCanada = IsUS || IsCanada;

            prospectRequest.Prospect.Phone = prospectRequest.Prospect.Phone != null ? prospectRequest.Prospect.Phone.CleanPhoneNumber(IsUSorCanada) : null;
            prospectRequest.Prospect.OtherPhone = prospectRequest.Prospect.OtherPhone != null ? prospectRequest.Prospect.OtherPhone.CleanPhoneNumber(IsUSorCanada) : null;

            //Prospect Advisor Notes
            if (!string.IsNullOrWhiteSpace(Notes))
            {
                prospectRequest.ProspectFlowDetails.Notes = Notes;
            }

            return prospectRequest;
        }
        #endregion SaveProspectRequest For Prospect Service
    }
}
