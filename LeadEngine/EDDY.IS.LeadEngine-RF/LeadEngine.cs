using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.LeadEngine.DTO;
using EDDY.IS.LeadEngine.DataModel;
using EDDY.IS.Util.XML;
using System.Text.RegularExpressions;
using EDDY.IS.Core.Logging;
using EDDY.IS.Util.StringExtensions;
using EDDY.IS.Validation;
using System.Configuration;

namespace EDDY.IS.LeadEngine
{
    public class LeadEngine
    {
        private const string SUCCESS_MESAGE = "Success";
        private static LeadDataService dbLeadsService = new LeadDataService();
        private static BetaLeadDataService dbBetaLeadsService = new BetaLeadDataService();
        private static RawPostDataService dbRawPostDataService = new RawPostDataService();
        private static ValidationEngine validationEngine = new ValidationEngine();

        public enum RealtimeDeliveryStatusValue
        {
            NEW = 100,
            REALTIME = 101,
            DELIVERED = 110,
            BATCHDELIVERY_LEAD = 111,
            RETRYING_TEST = 120,
            NEW_PENDING = 123,
            DELIVERED_ALL_ENDPOINTS = 200,
            DELIVERED_TEST = 220,
            DELIVERY_FAILED_PERMANENTLY = 410,
            FAILED_TEST = 420,
            EDDY_FORM_VALIDATION_FAILED = 430,
            EDDY_API_VALIDATION_FAILED = 432,
            EDDY_FORM_SERVERSIDE_VALIDATION_FAILED_PROFANITY = 434,
            EDDY_FORM_SERVERSIDE_VALIDATION_FAILED = 435,
            EDDY_FORM_INTERNAL_DUPLICATE_LEAD = 440,
            WARM_TRANSFER_TITANIUM_HOLD = 445,
            DUPLICATE_RESPONSE_DELIVERED = 450,
            FAILED_RESPONSE_DELIVERED = 460,
            EDDY_FORM_TEST_LEAD = 470,
            NEXUS_DELIVERY_NOT_REQUIRED = 900,
            LEGACY_DELIVERY = 990,
            EMD_SYNC_NOT_DELIVERED = 998,
            ELEARNERS_SYNC_NOT_DELIVERED = 999,
            SPAM = 1003
        }

        public LeadDTO CreateLeadDTO(LeadCreateRequest Request, bool realtimeDelivery, bool passedSSV = true, bool profanity = false)
        {
            Dictionary<string, string> LeadData = new Dictionary<string, string>(Request.LeadData, StringComparer.OrdinalIgnoreCase);
            LeadDTO Lead = new LeadDTO();

            Lead.ProspectId = Request.ProspectId;
            Lead.ProspectFlowStatusHistoryId = GetFieldValue("prospect_flow_status_history_id", LeadData, 0);
            Lead.ChatIntegrationSessionId = GetFieldValue("chat_integration_session_id", LeadData, 0);
            Lead.ClientRelationContactId = Request.ClientRelationContactId;
            Lead.ProgramProductId = Request.ProgramProductId;
            Lead.FormUniqueId = Request.TemplateId;
            Lead.IsEnabled = true;
            Lead.RawPostDataId = Request.RawPostDataId;
            Lead.MatchResponseGuid = Request.MatchResponseGuid;
            Lead.SubmissionId = Request.SubmissionId;
            Lead.LeadCreationTypeId = Request.LeadCreationTypeId;
            Lead.InitialLeadId = Request.InitialLeadId;
            Lead.PaidStatusTypeId = Request.PaidStatusType;
            Lead.ExternalMatchItemGuid = Request.ExternalMatchItemGuid;

            Lead.CategoryId = GetFieldValue("categoryid", LeadData, 0);
            Lead.FirstName = GetFieldValue("first_name", LeadData);
            Lead.LastName = GetFieldValue("last_name", LeadData);
            Lead.MiddleName = GetFieldValue("middlename", LeadData);
            Lead.Address1 = GetFieldValue("address", LeadData);
            Lead.Address2 = GetFieldValue("address_2", LeadData);
            Lead.EmailAddress = GetFieldValue("email", LeadData);
            Lead.CountryCode = GetFieldValue("country", LeadData, defaultValue: null);
            Lead.StateProvince = GetFieldValue("state", LeadData, defaultValue: null);
            Lead.City = GetFieldValue("city", LeadData, defaultValue: null);

            var isValusValid = Guid.TryParse(GetFieldValue("placementviewguid", LeadData, defaultValue: null), out Guid placementViewId);
            Lead.PlacementViewGuid = isValusValid ? placementViewId : (Guid?)null;

            //Phone Number cleanup requested by Venkat-Pete
            string CountryCode = Lead.CountryCode == null ? "" : Lead.CountryCode.ToUpper();
            bool IsUS = CountryCode == "US";
            bool IsCanada = CountryCode == "CA";
            bool IsUSorCanada = IsUS || IsCanada;
            Lead.Phone1 = GetFieldValue("phone", LeadData);
            Lead.Phone1 = Lead.Phone1 == null ? Lead.Phone1 : Lead.Phone1.CleanPhoneNumber(IsUSorCanada);
            Lead.Phone2 = GetFieldValue("alternate_phone", LeadData);
            Lead.Phone2 = Lead.Phone2 == null ? Lead.Phone2 : Lead.Phone2.CleanPhoneNumber(IsUSorCanada);

            //Postal code clenaup requested by Venkat-Pete
            Lead.ZipCode = GetFieldValue("postal_code", LeadData);
            Lead.ZipCode = Lead.ZipCode == null ? Lead.ZipCode : Lead.ZipCode.CleanZipCode(IsUS, IsCanada);

            // Automatically set missing location values if the zip code is provided;
            SetMissingLeadLocationValues(Lead);

            // Set location values to empty string if values still null
            Lead.City = Lead.City ?? "";
            Lead.CountryCode = Lead.CountryCode ?? "";
            Lead.StateProvince = Lead.StateProvince ?? "";

            Lead.TimeToStartInWeeks = GetFieldValue("time_to_start_in_weeks", LeadData, 0);
            Lead.BusinessModelId = GetFieldValue("businessmodelid", LeadData, 0);
            Lead.VisitorInternalId = Convert.ToDecimal(GetFieldValue("visitorinternalid", LeadData, 0));
            Lead.SessionInternalId = Convert.ToDecimal(GetFieldValue("sessioninternalid", LeadData, 0));
            Lead.Prefix = GetFieldValue("prefix", LeadData);
            Lead.Age = GetFieldValue("age", LeadData, 0);
            Lead.YearHighestEduCompleted = GetFieldValue("year_of_highest_education_completed", LeadData, 0);
            Lead.HighestLevelOfEdu = GetFieldValue("highest_level_of_education_completed", LeadData);
            Lead.MethodOfContact = GetFieldValue("preferred_methods_of_contact", LeadData);
            Lead.StartDate = GetFieldValue("desired_start_date", LeadData);
            Lead.LegacyLeadId = null;
            Lead.RealtimeDeliveryStatusId = (int)GetFieldValue("realtimedeliverystatusid", LeadData, 0);
            Lead.Military = GetFieldValue("military_affiliation", LeadData);
            Lead.ProgramTypeID = GetFieldValue("programtypeid", LeadData, 0);
            Lead.AffiliateId = GetFieldValue("AffiliateId", LeadData);
            Lead.AffiliateId = Lead.AffiliateId == "undefined" || Lead.AffiliateId == "null" ? null : Lead.AffiliateId;
            Lead.IsCallCenter = false;

            if (Request.ProductId.HasValue && Request.ProductId.Value == 52)
            {
                Lead.RealtimeDeliveryStatusId = (int)RealtimeDeliveryStatusValue.WARM_TRANSFER_TITANIUM_HOLD;
            }
            else if (realtimeDelivery)
            {
                Lead.RealtimeDeliveryStatusId = (int)RealtimeDeliveryStatusValue.REALTIME;
            }
            else if (!passedSSV && profanity)
            {
                //failed server side validation
                Lead.RealtimeDeliveryStatusId = (int)RealtimeDeliveryStatusValue.EDDY_FORM_SERVERSIDE_VALIDATION_FAILED_PROFANITY;
            }
            else if (!passedSSV)
            {
                //failed server side validation
                Lead.RealtimeDeliveryStatusId = (int)RealtimeDeliveryStatusValue.EDDY_FORM_SERVERSIDE_VALIDATION_FAILED;
            }
            else if (!Request.PassedValidation)
            {

                if (Request.IsInternalDuplicate || Request.IsExternalDuplicate)
                {
                    Lead.RealtimeDeliveryStatusId = (int)RealtimeDeliveryStatusValue.EDDY_FORM_INTERNAL_DUPLICATE_LEAD;
                }
                else if (Request.IsSpam)
                {
                    bool isSpamDeliveryEnabled = ConfigurationManager.AppSettings.AllKeys.Any(a => a == "IsSpamAllowedForDelivery") ? bool.Parse(ConfigurationManager.AppSettings["IsSpamAllowedForDelivery"].ToString()) : true;
                    Lead.RealtimeDeliveryStatusId = isSpamDeliveryEnabled ? (int)RealtimeDeliveryStatusValue.NEW : (int)RealtimeDeliveryStatusValue.SPAM;
                }
                else if (Request.IsSpamReportingOnly)
                {
                    Lead.RealtimeDeliveryStatusId = (int)RealtimeDeliveryStatusValue.NEW;
                }
                else
                {
                    //if lead failed from the API
                    if (Request.LeadCreationTypeId == 5)
                    {
                        Lead.RealtimeDeliveryStatusId = (int)RealtimeDeliveryStatusValue.EDDY_API_VALIDATION_FAILED;
                    }
                    else
                    {
                        Lead.RealtimeDeliveryStatusId = (int)RealtimeDeliveryStatusValue.EDDY_FORM_VALIDATION_FAILED;
                    }

                }
            }
            else
            {
                Lead.RealtimeDeliveryStatusId = (int)RealtimeDeliveryStatusValue.NEW_PENDING;
            }

            Lead.ExternalLeadId = GetFieldValue("leadid_token", LeadData);
            if (!string.IsNullOrEmpty(Lead.ExternalLeadId))
            {
                Lead.ExternalLeadId = Lead.ExternalLeadId.ToUpper();
            }
            Lead.TrackId = Request.TrackId;
            Lead.TrackingSessionGUID = Request.TrackingSessionGUID;
            Lead.AllowedViaLeadScoringUpsell = Request.AllowedViaLeadScoringUpsell;

            decimal unaudittedRevenue;
            if (decimal.TryParse(GetFieldValue("BuyerRevenue", LeadData), out unaudittedRevenue))
            {
                Lead.UnaudittedRevenue = unaudittedRevenue;
            }
            Guid externalMatchItemGuid;
            if (Guid.TryParse(GetFieldValue("ExternalMatchItemGuid", LeadData), out externalMatchItemGuid))
            {
                Lead.ExternalMatchItemGuid = externalMatchItemGuid;
            }

            DateTime dateEntered;
            if (DateTime.TryParse(GetFieldValue("DateEntered", LeadData), out dateEntered))
            {
                Lead.DateEntered = dateEntered;
            }

            if (LeadData.ContainsKey("SchoolSelectionExpressConsent"))
            {
                GetFieldValue("EDDYUserAgreement", LeadData, true);
                GetFieldValue("IsNewSmartMatchTCPA", LeadData, true);
                GetFieldValue("TCPASelectedSchoolList", LeadData, true);
                GetFieldValue("SmartMatchUserAgreement", LeadData, true);
            }
            else
            {
                //Adjust the TCPA based on the institution
                if (GetFieldValue("IsNewSmartMatchTCPA", LeadData) == "Yes")
                {
                    LeadData["EDDYUserAgreement"] = GetFieldValue("EDDYUserAgreement", LeadData).Replace("{0}", Lead.Phone1);
                    if (!string.IsNullOrEmpty(GetFieldValue("TCPASelectedSchoolList", LeadData)))
                    {
                        LeadData["UserAgreement"] = GetFieldValue("SmartMatchUserAgreement", LeadData, true).Replace("{smschool}{/smschool}", Request.InstitutionName).Replace("{mobile-number}{/mobile-number}", Lead.Phone1);

                    }
                }
            }

            

            if (LeadData.Keys.Count > 0)
            {
                Lead.AdditionalFields = Serialization.GetXMLFromDictionary(LeadData);
            }

            Lead.EstimatedRevShare = Request.EstimatedRevShare;
            Lead.EstimatedLeadRev = Request.EstimatedLeadRev;
            Lead.ScoreId = Request.ScoreId;
            return Lead;
        }

        private int? GetFieldValue(string key, Dictionary<string, string> values, int? DefaultValue, bool RemoveFromDictionary = true)
        {
            int? result = DefaultValue;
            string val = GetFieldValue(key, values, RemoveFromDictionary);
            if (!string.IsNullOrWhiteSpace(val))
            {
                int intValue = 0;
                result = int.TryParse(val, out intValue) ? intValue : DefaultValue;
            }
            return result;
        }

        private string GetFieldValue(string key, Dictionary<string, string> values, bool RemoveFromDictionary = true)
        {
            string result = "";
            if (values.ContainsKey(key))
            {
                result = values[key];
                if (RemoveFromDictionary)
                {
                    values.Remove(key);
                }
            }
            return result;
        }

        private string GetFieldValue(string key, Dictionary<string, string> values, string defaultValue, bool RemoveFromDictionary = true)
        {
            string result = defaultValue;
            if (values.ContainsKey(key))
            {
                result = values[key];
                if (RemoveFromDictionary)
                {
                    values.Remove(key);
                }
            }
            return result;
        }

        private void SetMissingLeadLocationValues(LeadDTO lead)
        {
            var zipCodeIsNotEmpty = !string.IsNullOrWhiteSpace(lead.ZipCode);
            //var locationValuesMissing = (string.IsNullOrEmpty(lead.CountryCode) && (string.IsNullOrEmpty(lead.StateProvince) && string.IsNullOrEmpty(lead.City);
            if (zipCodeIsNotEmpty)
            {
                List<KeyValuePair<string, string>> location = validationEngine.GetCityStateCountry(lead.ZipCode);

                if (string.IsNullOrEmpty(lead.CountryCode))
                {
                    var countryCode = location.Where(c => c.Key == "CountryCode").Select(c => c.Value).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(countryCode))
                    {
                        lead.CountryCode = countryCode;
                    }
                }
                if (string.IsNullOrEmpty(lead.StateProvince))
                {
                    var stateCode = location.Where(c => c.Key == "StateCode").Select(c => c.Value).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(stateCode))
                    {
                        lead.StateProvince = stateCode;
                    }
                }
                if (string.IsNullOrEmpty(lead.City))
                {
                    var city = location.Where(c => c.Key == "City").Select(c => c.Value).FirstOrDefault();
                    if (!string.IsNullOrWhiteSpace(city))
                    {
                        lead.City = city;
                    }
                }
            }
        }

        /// <summary>
        /// Is Test lead based on IS/EC rules:
        /// lastname= test OR
        /// email contains @test.com domain
        /// </summary>
        /// <param name="FirstName"></param>
        /// <param name="LastName"></param>
        /// <param name="Email"></param>
        /// <returns></returns>
        public bool IsTestLead(string FirstName, string LastName, string Email)
        {
            FirstName = FirstName == null ? "" : FirstName;
            LastName = LastName == null ? "" : LastName;
            Email = Email == null ? "" : Email;
            return (LastName.Trim().ToLower() == "test" || Email.ToLower().Contains("@test.com"));
        }

        public LeadCreateResponse UpdateLead(LeadCreateRequest Request, int leadId, PerformanceLog Log)
        {
            Log.StartLogDetail("LeadEngine.CreateLead");
            LeadCreateResponse Response = new LeadCreateResponse();

            Log.StartLogDetail("LeadEngine.CreateLeadDTO");
            LeadDTO Lead = CreateLeadDTO(Request, false);
            Lead.LeadId = leadId;
            Log.EndLogDetail();

            Log.StartLogDetail("LeadEngine.GetLead");
            Response.Lead = dbLeadsService.UpdateLead(Lead);
            Log.EndLogDetail();

            Log.EndLogDetail();

            return Response;
        }

        public LeadCreateResponse CreateLead(LeadCreateRequest Request, int? ProspectId, PerformanceLog Log, bool realtimeDelivery, bool passedSSV = true, bool profanity = false)
        {
            Log.StartLogDetail("LeadEngine.CreateLead");

            Log.StartLogDetail("LeadEngine.CreateLeadDTO");
            LeadDTO Lead = CreateLeadDTO(Request, realtimeDelivery, passedSSV, profanity);
            Lead.ProspectId = ProspectId;
            Log.EndLogDetail();

            Log.StartLogDetail("LeadEngine.CreateLead_LookupAndSave");
            LeadCreateResponse Response = CreateLead(Lead, Request.ProgramProductId, Request.IsBeta, Log);
            Log.EndLogDetail();

            Log.EndLogDetail();

            return Response;
        }

        public LeadDTO BuildLeadDTO(LeadCreateRequest Request, bool realtimeDelivery)
        {
            return CreateLeadDTO(Request, realtimeDelivery);
        }

        public void SaveLeadCreative(List<Tuple<int, int, DateTime?>> leadProgramProductList, bool isBeta, int submissionId)
        {
            foreach (var leadProgramProduct in leadProgramProductList)
            {
                try
                {
                    var leadId = leadProgramProduct.Item1;
                    var programProductId = leadProgramProduct.Item2;

                    dbLeadsService.SaveLeadCreative(leadId, isBeta, submissionId);
                }
                catch (Exception ex)
                {
                    ISException isEx = new ISException(Base.ISApplication.FormsEngine, ex);
                    isEx.Save();
                }
            }
        }

        public List<Tuple<int, int>> UpdateLeads(int? submissionId, RawPostDataDTO RawPostData, Guid campaignTrackId, List<Tuple<int, int, DateTime?>> leadProgramProductList, bool isBeta)
        {
            long rawPostDataId = 0;
            var leadProductList = new List<Tuple<int, int>>();

            try
            {
                rawPostDataId = CreateRawPostData(RawPostData);
            }
            catch (Exception ex)
            {
                ISException isEx = new ISException(Base.ISApplication.FormsEngine, ex);
                isEx.Save();
            }

            foreach (var leadProgramProduct in leadProgramProductList)
            {
                try
                {
                    var leadId = leadProgramProduct.Item1;
                    var programProductId = leadProgramProduct.Item2;
                    var dateEntered = leadProgramProduct.Item3;
                    var productId = dbLeadsService.UpdateLead(leadId, campaignTrackId, submissionId, programProductId, rawPostDataId, dateEntered, isBeta);

                    leadProductList.Add(new Tuple<int, int>(leadId, productId));
                }
                catch (Exception ex)
                {
                    ISException isEx = new ISException(Base.ISApplication.FormsEngine, ex);
                    isEx.Save();
                }
            }

            return leadProductList;
        }

        /// <summary>
        /// Saves Raw Post Data based on client or site capture
        /// </summary>
        /// <param name="Request"></param>
        /// <returns></returns>
        public long CreateRawPostData(RawPostDataDTO Request)
        {
            return dbRawPostDataService.SaveRawPostData(Request).RawPostDataID;
        }

        /// <summary>
        /// Creates a Lead based on LeadDTO
        /// </summary>
        /// <param name="Lead"></param>
        /// <param name="IsBeta"></param>
        /// <returns></returns>
        public LeadCreateResponse CreateLead(LeadDTO Lead, int ProgramProductId, bool IsBeta, PerformanceLog Log)
        {
            LeadCreateResponse Response = new LeadCreateResponse();
            Lead.RowGuid = Guid.NewGuid();

            //Test Lead not to be delivered
            if (IsTestLead(Lead.FirstName, Lead.LastName, Lead.EmailAddress))
            {
                Lead.RealtimeDeliveryStatusId = (int)RealtimeDeliveryStatusValue.EDDY_FORM_TEST_LEAD;
                Response.IsTestLead = true;
            }
            else
            {
                Response.IsTestLead = false;
            }

            if (!IsBeta)
            {
                Log.StartLogDetail("CreateLead.Save");
                Response.Lead = dbLeadsService.SaveLead(Lead);
                //If this has a DateEntered by Edumax pass it along so the lead can be updated. 
                if (Lead.DateEntered.HasValue)
                {
                    Response.Lead.DateEntered = Lead.DateEntered;
                }
                Log.EndLogDetail();
                Response.Success = true;
                Response.ResultMessage = SUCCESS_MESAGE;
            }
            else
            {
                var LeadOut = dbBetaLeadsService.SaveBetaLead(Lead);

                //updated fields when saved.
                Lead.LeadId = LeadOut.LeadId;
                Lead.UID = LeadOut.UID;

                Response.Lead = Lead;
                Response.Success = true;
                Response.ResultMessage = SUCCESS_MESAGE;
            }

            return Response;
        }
    }
}
