using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Common.Utilities.NoNull;
using EDDY.IS.EmsLeadEngine.Entities;
using EDDY.IS.EmsLeadEngine.Entities.Common;
using EDDY.IS.Vendor.DataAccess.DataModels;
using EDDY.IS.Vendor.DataAccess.FormsEngineService;
using EDDY.IS.Vendor.DataAccess.MatchingService;
using EDDY.IS.Vendor.DataAccess.ProspectService;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using Microsoft.Practices.EnterpriseLibrary.Logging.ExtraInformation;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using static EDDY.IS.EmsLeadEngine.Entities.Constants;

namespace EDDY.IS.Vendor.DataAccess
{
    public class DataExchangeServiceDAO : VendorBaseDAO
    {
        public VendorResponseBase UpdateEMSLead(LeadUpdateRequest leadUpdateRequest)
        {
            VendorResponseBase vendorResponse = new VendorResponseBase();

            try
            {
                VW_EMSLead existingLead = LookUpLead(leadUpdateRequest);

                if (existingLead == null) 
                    return CreateLeadUpdateFailureResponse("Lead Not Found", InputValidation.MessageCodes.NoLeadFound);
                
                // Existing lead found & lead changed
                ExchangeLeadProcessRequest exchangeLead = MapToExchangeLead(existingLead, leadUpdateRequest);

                if (!LeadChanged(existingLead, exchangeLead.Lead))
                    return CreateLeadUpdateSuccessResponse("No Updates Needed", InputValidation.MessageCodes.NoUpdateNeeded);

                HttpResponseMessage leadServiceResponse = CallLeadService(exchangeLead);

                if (!leadServiceResponse.IsSuccessStatusCode)
                {
                    string serviceMessage = leadServiceResponse.Content?.ReadAsStringAsync().Result;
                    return CreateLeadUpdateFailureResponse("Update Failed", InputValidation.MessageCodes.UpdateFailed);//$"Lead service update failed. StatusCode: {(int)leadServiceResponse.StatusCode}. {serviceMessage}");
                }

                return CreateLeadUpdateSuccessResponse("Lead Updated Successfully", "msg0117");
                

            }
            catch (Exception ex)
            {
                return CreateLeadUpdateFailureResponse($"Exception occurred: {ex.Message}", InputValidation.MessageCodes.ExceptionOccurredWhileUpdating);
            }
        }





        public VW_EMSLead LookUpLead(LeadUpdateRequest leadUpdateRequest)
        {
            VW_EMSLead existingLead = null;
            int instId = leadUpdateRequest.EMSInstitutionId.Value;
            foreach (int lookUpKeyValue in leadUpdateRequest.LookUpKeyList)
            {
                if (!Enum.IsDefined(typeof(ExchangeLeadUniqueKey), lookUpKeyValue))
                    continue;


                ExchangeLeadUniqueKey lookUpKey = (ExchangeLeadUniqueKey)lookUpKeyValue;
                switch (lookUpKey)
                {
                    case ExchangeLeadUniqueKey.ExternalId:
                        existingLead = LookUpEMSLeadByExternalId(instId, leadUpdateRequest.ExternalId);
                        break;

                    case ExchangeLeadUniqueKey.EmailAddress:
                        existingLead = LookUpEMSLeadByEmailAddress(instId, leadUpdateRequest.Email);
                        break;

                    case ExchangeLeadUniqueKey.ISLeadId:
                        existingLead = LookUpEMSLeadByISLeadId(instId, leadUpdateRequest.ISLeadId);
                        break;

                    case ExchangeLeadUniqueKey.Phone1:
                        existingLead = LookUpEMSLeadByPhone1(instId, leadUpdateRequest.Phone);
                        break;

                    case ExchangeLeadUniqueKey.EMSLeadId:
                        existingLead = LookUpEMSLeadByEMSLeadId(instId, leadUpdateRequest.EMSLeadId);
                        break;

                    case ExchangeLeadUniqueKey.FirstLastName:
                        existingLead = LookUpEMSLeadByFirstLastName(instId,
                            leadUpdateRequest.FirstName,
                            leadUpdateRequest.LastName);
                        break;

                    case ExchangeLeadUniqueKey.NameAndEmailorPhone:
                        existingLead = LookUpEMSLeadByNameAndEmailOrPhone(instId,
                            leadUpdateRequest.FirstName,
                            leadUpdateRequest.LastName,
                            leadUpdateRequest.Email,
                            leadUpdateRequest.Phone);
                        break;
                }

                if (existingLead != null)
                    break;

            }
            return existingLead;
        }


        public bool LeadChanged(VW_EMSLead originalLead, EmsLeadEngine.Entities.Common.ExchangeLead updatedLead)
        {
            if (originalLead == null || updatedLead == null)
                return false;

            var updatedType = updatedLead.GetType();
            var originalType = originalLead.GetType();

            var excludedFields = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "EMSInstitutionId",
                "LeadGUID",

                // lookup-only fields
                "ExternalId",
                "Email",
                "EmailAddress",
                "Phone",
                "Phone1",
                "ISLeadId",
                "EMSLeadId",
                "FirstName",
                "LastName"
            };
            foreach (var updatedProp in updatedType.GetProperties())
            {
                if (excludedFields.Contains(updatedProp.Name))
                    continue;

                var originalProp = originalType.GetProperty(updatedProp.Name);

                if (originalProp == null)
                    continue;

                var updatedValue = updatedProp.GetValue(updatedLead, null);

                if (updatedValue == null)
                    continue;

                if (updatedValue is string updatedString &&string.IsNullOrWhiteSpace(updatedString))
                    continue;

                var originalValue = originalProp.GetValue(originalLead, null);

                if (!ValuesAreEqual(originalValue, updatedValue))
                    return true;
            }

            return false;
        }
        private bool ValuesAreEqual(object originalValue, object updatedValue)
        {
            if (originalValue == null && updatedValue == null)
                return true;

            if (originalValue == null || updatedValue == null)
                return false;

            if (originalValue is string originalString && updatedValue is string updatedString)
            {
                return string.Equals( originalString.Trim(), updatedString.Trim(), StringComparison.OrdinalIgnoreCase);
            }

            return Equals(originalValue, updatedValue);
        }


        public ExchangeLeadProcessRequest MapToExchangeLead(VW_EMSLead originalLead, LeadUpdateRequest leadUpdateReq)
        {

            var exchangeLead = new ExchangeLeadProcessRequest
            {
                LeadAction = ExchangeLeadAction.Update,
                LeadUniqueKey = ExchangeLeadUniqueKey.LeadGUID,
                Lead = new EmsLeadEngine.Entities.Common.ExchangeLead
                {
                    EMSInstitutionId = leadUpdateReq.EMSInstitutionId.Value,
                    LeadGUID = originalLead.LeadGUID, // LeadUniqueKey

                    LeadStateId = leadUpdateReq.LeadStateId,
                    ClosedReasonCode = NullIfEmpty(leadUpdateReq.ClosedReasonCode),
                    ClientApplicationDegreeName = NullIfEmpty(leadUpdateReq.ClientApplicationDegreeName),
                    CustomFields = NullIfEmpty(leadUpdateReq.CustomFields),
                    ClientRegistered = NullIfEmpty(leadUpdateReq.ClientRegistered),
                    ClientInitialStartTerm = NullIfEmpty(leadUpdateReq.ClientInitialStartTerm),
                    ClientNotes = NullIfEmpty(leadUpdateReq.ClientNotes),
                    ClientStatus = NullIfEmpty(leadUpdateReq.ClientStatus),
                    ClientApplicationStartTerm = NullIfEmpty(leadUpdateReq.ClientApplicationStartTerm),
                    PendingApplicationChecklistItems = NullIfEmpty(leadUpdateReq.PendingApplicationChecklistItems),
                    CompletedApplicationChecklistItems = NullIfEmpty(leadUpdateReq.CompletedApplicationChecklistItems),

                    // These dates are defaulted if status matches and a value was not passed in
                    ClientApplicationDate = ParseDate(leadUpdateReq.ClientApplicationDate , originalLead.ClientApplicationDate, leadUpdateReq.ClientStatus, "ApplicationsCompleted"),
                    ClientInterviewDate = ParseDate(leadUpdateReq.ClientInterviewDate, originalLead.ClientInterviewDate, leadUpdateReq.ClientStatus, "Interviews"),
                    ClientStartDate = ParseDate(leadUpdateReq.ClientStartDate, originalLead.ClientStartDate, leadUpdateReq.ClientStatus, "Starts"),
                    ClientContactDate = ParseDate(leadUpdateReq.ClientContactDate, originalLead.ClientContactDate, leadUpdateReq.ClientStatus, "Contacts"),
                    ClientEnrollDate = ParseDate(leadUpdateReq.ClientEnrollDate, originalLead.ClientEnrollDate, leadUpdateReq.ClientStatus, "Enrollments"),
                    ClientApplicationStartDate = ParseDate(leadUpdateReq.ClientApplicationStartDate, originalLead.ClientApplicationStartDate, leadUpdateReq.ClientStatus, "ApplicationsStarted"),
                    ClientAdmitDate = ParseDate(leadUpdateReq.ClientAdmitDate, originalLead.ClientAdmitDate, leadUpdateReq.ClientStatus, "Admit"),
                    ClientAppointmentDate = ParseDate(leadUpdateReq.ClientAppointmentDate, originalLead.ClientAppointmentDate, leadUpdateReq.ClientStatus, "Appointment"),
                    ClientQualifiedDate = ParseDate(leadUpdateReq.ClientQualifiedDate, originalLead.ClientQualifiedDate, leadUpdateReq.ClientStatus, "Qualifies"),
                    ClientGraduateDate = ParseDate(leadUpdateReq.ClientGraduateDate, originalLead.ClientGraduateDate, leadUpdateReq.ClientStatus, "Graduated"),
                    ClientFirstTermPersistDate = ParseDate(leadUpdateReq.ClientFirstTermPersistDate, originalLead.ClientFirstTermPersistDate, leadUpdateReq.ClientStatus, "Persists"),
                    ClientApplicationSubmittedDate = ParseDate(leadUpdateReq.ClientApplicationSubmittedDate, originalLead.ClientApplicationSubmittedDate, leadUpdateReq.ClientStatus, "ApplicationSubmitted"),
                    ClientDepositDate = ParseDate(leadUpdateReq.ClientDepositDate, originalLead.ClientDepositDate, leadUpdateReq.ClientStatus, "Deposit"),
                    ClientApplicationDeniedDate = ParseDate(leadUpdateReq.ClientApplicationDeniedDate, originalLead.ClientApplicationDeniedDate, leadUpdateReq.ClientStatus, "ApplicationsDenied"),

                    // Date fields not defaulted by clientStatus
                    ClientStatusUpdatedDate = ParseDate(leadUpdateReq.ClientStatusUpdatedDate),
                    ClientStartReceivedDate = ParseDate(leadUpdateReq.ClientStartReceivedDate),
                    ClientFAFSAReceivedDate = ParseDate(leadUpdateReq.ClientFAFSAReceivedDate)
                }
            };

            return exchangeLead;
        }

        private static string NullIfEmpty(string value)
        {
            return string.IsNullOrWhiteSpace(value) ? null : value;
        }

        private static DateTime? ParseDate(string requestDate, DateTime? originalDate = null, string requestClientStatus = null, string standardizedClientStatus = null)
        {
            if (DateTime.TryParse(requestDate, out var parsedDate)) // Always return updated date if passed in on the request
                return parsedDate;

            // Default current datetime based on status mapping
            if (!originalDate.HasValue && !string.IsNullOrWhiteSpace(standardizedClientStatus) && requestClientStatus == standardizedClientStatus)      
                return DateTime.Now;
            
            return null;
        }



        private HttpResponseMessage CallLeadService(ExchangeLeadProcessRequest exchangeLead)
        {
            var request = new ExchangeMultipleLeadProcessRequest
            {
                AuthenticationToken = Guid.Parse(ConfigurationManager.AppSettings["EmsLeadEngineAuthToken"]),
                TransactionId = Guid.NewGuid(),
                ProcessRequestList = new List<ExchangeLeadProcessRequest> { exchangeLead }
            };

            HttpResponseMessage response = null;
            string baseUrl = ConfigurationManager.AppSettings["EmsLeadEngineBaseUrl"];
            string endpoint = ConfigurationManager.AppSettings["EmsLeadEngineProcessFromDataExchangeEndpoint"];

            if (!int.TryParse(ConfigurationManager.AppSettings["EmsLeadEngineTimeoutMinutes"], out int timeout))
                timeout = 10; // default if unable to parse
            

            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromMinutes(timeout);
                client.BaseAddress = new Uri(baseUrl); 
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                response = client.PostAsync(endpoint, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")).Result;
            }

            return response;
        }






        public VendorResponseBase CreateLeadUpdateFailureResponse(string body, params string[] messageCodes)
        {
            return new VendorResponseBase
            {
                IsSuccessful = false,
                Body = body,
                Messages = messageCodes
                    .Select(code => getVendorResponseMessageByMessageCode(code))
                    .Where(message => message != null)
                    .ToList()
            };
        }
        public VendorResponseBase CreateLeadUpdateSuccessResponse(object body = null, params string[] messageCodes)
        {
            return new VendorResponseBase
            {
                IsSuccessful = true,
                Body = body,
                Messages = messageCodes
                    .Select(code => getVendorResponseMessageByMessageCode(code))
                    .Where(message => message != null)
                    .ToList()
            };
        }


    }
}
