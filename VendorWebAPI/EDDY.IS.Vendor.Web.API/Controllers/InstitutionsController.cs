using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Vendor.Web.API.Filters;
using EDDY.IS.Vendor.Web.API.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace EDDY.IS.Vendor.Web.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "POST")]
    public class InstitutionsController : BaseAPIController
    {
        //POST api/Institutions/lead/save
        #region  api/lead/save-basic documentation
        /// <summary>
        /// <para>The Host and Post service is located at https://partners.educationdynamics.com/api/Institutions/lead-save . The posting service can be called using simple HTTP POST, and will return an JSON-formatted response indicating the success or failure of the partner’s submission, along with details if applicable. The Request must include all the required key-value pairs defined by the API’s https://partners.educationdynamics.com/api/program/form method.  </para> 
        /// </summary>
        /// <ParameterDescription>Test</ParameterDescription>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="FirstName">Required | First name.</param>
        /// <param name="LastName">Required | Last name.</param>
        /// <param name="Address">Address</param>
        /// <param name="City">Required | City.</param> 
        /// <param name="State">Required | Two-character US state abbreviation, e.g., "NJ".</param>  
        /// <param name="PostalCode">Required | Postal code or Zip code.</param>
        /// <param name="Country">Required | Two-character country code, e.g., "US".</param>
        /// <param name="Email">Required | Email address.</param>
        /// <param name="Phone">Required | Phone number.</param> 
        /// <param name="ProgramId">Required | | Program Id.</param> 
        /// <param name="HighestLevelofEducationCompleted">Id of the highest level of education completed. We only accept the string value i.e., "2". Accepted Values: [{"Value": "2", "Text": "G.E.D."}, {"Value": "3", "Text": "High School Diploma"}, {"Value": "4", "Text": "Some College ,1-29 Credits"}, {"Value": "5", "Text": "Some College, 30-59 Credits"}, {"Value": "6", "Text": "Some College,60-89 Credits"}, {"Value": "7", "Text": "Some College, 90+ Credits"}, {"Value": "8", "Text": "Associate"}, {"Value": "9", "Text": "Bachelor"}, {"Value": "10", "Text": "Master"}, {"Value": "11", "Text": "Doctorate"}, {"Value": "1", "Text": "Haven't completed High School"}].</param> 
        /// <param name="CampusId">Campus Id.</param> 
        /// <param name="SourceCode">Required when EMS Partner.</param>
        /// <param name="AdditionalQuestions">
        /// Optional | Additional prospect data, if available.
        /// &lt;br /&gt;
        /// This field should be provided as a collection of JSON objects using the following format:
        /// &lt;br /&gt;
        /// [{ "QuestionKey":"KeyName", "QuestionValue":"Value" }]
        /// &lt;br /&gt;
        /// Example:
        /// &lt;br /&gt;
        /// [{"QuestionKey":"UTMCampaign","QuestionValue":"SpringCampaign"},{"QuestionKey":"LeadInitiatingUrl","QuestionValue":"https%3A%2F%2Fexample.com%2Flandingpage"}]
        /// &lt;br /&gt;
        /// Any URLs included in QuestionValue should be URL encoded.
        /// &lt;br /&gt;
        /// The following standardized QuestionKey values are commonly supported and should be named exactly as listed:
        /// &lt;ul&gt;
        /// &lt;li&gt;UTMCampaign&lt;/li&gt;
        /// &lt;li&gt;UTMChannel&lt;/li&gt;
        /// &lt;li&gt;UTMVendor&lt;/li&gt;
        /// &lt;li&gt;externalid&lt;/li&gt;
        /// &lt;li&gt;utm_content&lt;/li&gt;
        /// &lt;li&gt;utm_medium&lt;/li&gt;
        /// &lt;li&gt;utm_source&lt;/li&gt;
        /// &lt;li&gt;utm_term&lt;/li&gt;
        /// &lt;li&gt;utm_campaign&lt;/li&gt;
        /// &lt;li&gt;FormName&lt;/li&gt;
        /// &lt;li&gt;LeadInitiatingUrl&lt;/li&gt;
        /// &lt;li&gt;FormLeadUrl&lt;/li&gt;
        /// &lt;li&gt;LeadSourceUrl&lt;/li&gt;
        /// &lt;li&gt;SourceCode&lt;/li&gt;
        /// &lt;li&gt;LeadSourceType&lt;/li&gt;
        /// &lt;li&gt;EddyIPAddress&lt;/li&gt;
        /// &lt;li&gt;gclid&lt;/li&gt;
        /// &lt;li&gt;fbclid&lt;/li&gt;
        /// &lt;li&gt;msclkid&lt;/li&gt;
        /// &lt;li&gt;RN_license&lt;/li&gt;
        /// &lt;li&gt;StartTerm&lt;/li&gt;
        /// &lt;li&gt;HSGradYear&lt;/li&gt;
        /// &lt;/ul&gt;
        /// </param>    
        /// <param name="UserAgreement">Required | TCPA info</param> 
        ///<example>
        /// {
        /// "apiKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        /// "firstName": "Testfirst",
        /// "lastname": "Testlast",
        /// "Address": "801 test Corner Ct",
        /// "city": "Green Bay",
        /// "postalcode": "55555",
        /// "state": "WI",
        /// "country": "US",
        /// "Email": "test@test.com",
        /// "Phone": "5555555555",
        /// "ProgramId": 7243,
        /// "CampusId": 4919,
        /// "HighestLevelofEducationCompleted": "9",
        /// "SourceCode": "STUDY-UG",
        /// "AdditionalQuestions":  [{"QuestionKey":"RNLicense","QuestionValue":"Yes"},{"QuestionKey":"UTMVendor","QuestionValue":"UTMVendorValue"},{"QuestionKey":"UTMCampaign","QuestionValue":"UTMCampaignValue"},{"QuestionKey":"UTMChannel","QuestionValue":"UTMChannelValue"}],
        /// "UserAgreement":"I agree to the user Agreement"
        /// }
        /// </example>
        /// <returns>
        /// Successfully saved leads will return with a "IsSuccessful" flag value as "true".
        /// 
        /// The below is an example of a successfully saved lead.
        ///{
        /// "IsSuccessful": true,
        /// "Body": {
        ///     "UID": "xxx-xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx",
        ///     "LeadTier": 0
        /// },
        /// "ResponseDateTime": "2017-01-30T10:06:18.5930446-05:00",
        /// "RequestDateTime": "2017-01-30T10:05:14.7594556-05:00",
        /// "ResponseGuid": "1b99830e-6bdf-48be-9a51-97a79f5a5003",
        /// "Messages": [
        ///    {
        ///        "MessageCode": "msg0038",
        ///         "Message": "Validation Passed."
        ///    }
        /// ],
        /// "TotalResponseTime": 120
        ///}
        ///
        /// Unsuccessfully saved leads will return with a "IsSuccessful" flag value as "false"
        /// The "Messages" portion of the return body will also include one or more validation messages as to why the lead failed to save.
        /// The below is an example of an unsuccessfully saved lead.
        /// 
        ///{
        /// "IsSuccessful": false,
        /// "Body": null,
        /// "ResponseDateTime": "2019-06-04T13:28:05.8561884-04:00",
        /// "RequestDateTime": "2019-06-04T13:28:03.4897363-04:00",
        /// "ResponseGuid": "a204e540-c53f-45bc-a92e-200f802149dd",
        /// "Messages": [
        ///     {
        ///         "MessageCode": "msg0009",
        ///         "Message": "Exception occurred."
        ///     }
        /// ],
        /// "TotalResponseTime": 2366
        ///}
        /// </returns>     
        #endregion  api/lead/save documentation
        [HttpPost]
        [HostAndPostCampaignFilter]
        [CampusFilter]
        [SubjectFilter]
        [AgeFilter]
        [CategoryFilter]        
        [TCPAFilter]
        [EMSPostLeadActionFilter]
        [ActionName("lead-save")]
        public HttpResponseMessage PostLeadBasic([FromBody] ContactRequest contactRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                PostLeadResponseModel responseContent = new PostLeadResponseModel(contactRequest);
                
                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.LeadSave, contactRequest.Email);

                var queryParams = Request.GetRequestQueryParameters();
                bool returnBody = queryParams != null && !string.IsNullOrEmpty(queryParams.Get("returnLeadId")) && queryParams.Get("returnLeadId").Equals("true");
                if (!responseContent.IsSuccessful && !returnBody)
                {
                    responseContent.Body = null;
                }

                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());


            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.WCF_SERVICE_POLICY);

                VendorResponseBase responseContent = new VendorResponseBase();

                responseContent.IsSuccessful = false;
                responseContent.ResponseGuid = Guid.NewGuid();
                responseContent.RequestDateTime = DateTime.Now;
                responseContent.IsSuccessful = false;
                responseContent.Messages = new List<VendorResponseMessage>();

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.GeneralException);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }

                responseContent.ResponseDateTime = DateTime.Now;
                httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
                

                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.LeadSave, contactRequest.Email);
                if (!responseContent.IsSuccessful)
                {
                    responseContent.Body = null;
                }
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

            }
            return httpResponseMessage;

        }




        //POST api/Institutions/lead/update
        #region lead update documentation
        /// <summary>
        /// <para>
        /// The Lead Update service is located at https://partners.educationdynamics.com/api/Institutions/lead-update.
        /// The posting service can be called using HTTP POST and will return a JSON-formatted response indicating the
        /// success or failure of the partner’s submission, along with response messages when applicable.
        /// This endpoint updates an existing EMS lead. The request must include EMSInstitutionId and LookUpKeyList.
        /// LookUpKeyList determines which identifiers should be used to locate the existing lead. At least one valid
        /// lookup value must be supplied for the provided LookUpKeyList.
        /// If more than one lookup key is provided, the service will attempt to locate the lead using each lookup key
        /// until a matching lead is found.
        /// </para>
        /// </summary>

        /// <param name="APIKey">Required | Partner credentials. &lt;br /&gt;This value will be provided by the EDDY Account Management team.</param>
        /// <param name="EMSInstitutionId">Required | EMS Institution Id. &lt;br /&gt;This value will be provided by the EDDY Account Management team.</param>
        /// <param name="LookUpKeyList">
        /// Required | List of integer lookup keys used to locate the lead to update.
        /// &lt;br /&gt;
        /// Accepted values:
        /// &lt;ul&gt;
        /// &lt;li&gt;1 = ExternalId&lt;/li&gt;
        /// &lt;li&gt;4 = EmailAddress&lt;/li&gt;
        /// &lt;li&gt;6 = ISLeadId&lt;/li&gt;
        /// &lt;li&gt;7 = Phone1&lt;/li&gt;
        /// &lt;li&gt;8 = EMSLeadId&lt;/li&gt;
        /// &lt;li&gt;9 = FirstLastName&lt;/li&gt;
        /// &lt;li&gt;10 = NameAndEmailorPhone&lt;/li&gt;
        /// &lt;/ul&gt;
        /// At least one corresponding lookup value must be supplied.
        /// </param>
        /// <param name="EMSLeadId">EMS Lead Id. &lt;br /&gt;Required when LookUpKeyList includes EMSLeadId.</param>
        /// <param name="ISLeadId">IS Lead Id. &lt;br /&gt;Required when LookUpKeyList includes ISLeadId.</param>
        /// <param name="ExternalId">External lead identifier. &lt;br /&gt;Required when LookUpKeyList includes ExternalId.</param>
        /// <param name="FirstName">First name. &lt;br /&gt;Required when LookUpKeyList includes FirstLastName or NameAndEmailorPhone.</param>
        /// <param name="LastName">Last name. &lt;br /&gt;Required when LookUpKeyList includes FirstLastName or NameAndEmailorPhone.</param>
        /// <param name="Email">Email address. &lt;br /&gt;Required when LookUpKeyList includes EmailAddress, or when LookUpKeyList includes NameAndEmailorPhone and Phone is not provided.</param>
        /// <param name="Phone">Phone number. &lt;br /&gt;Required when LookUpKeyList includes Phone1, or when LookUpKeyList includes NameAndEmailorPhone and Email is not provided.</param>
        /// <param name="LeadStateId">
        /// Lead state id.&lt;br /&gt;
        /// Accepted values:
        /// &lt;ul&gt;
        /// &lt;li&gt;1 = Active&lt;/li&gt;
        /// &lt;li&gt;2 = Closed&lt;/li&gt;
        /// &lt;/ul&gt;
        /// If LeadStateId is set to 2 (Closed), ClosedReasonCode should also be provided.
        /// </param>
        /// <param name="ClosedReasonCode">
        /// Closed reason code.
        /// &lt;br /&gt;
        /// This should only be provided when LeadStateId is set to 2 (Closed).
        /// &lt;br /&gt;
        /// If LeadStateId is set to 2, ClosedReasonCode should also be provided.
        /// </param>
        /// <param name="ClientApplicationDegreeName">Application degree name.</param>
        /// <param name="CustomFields">Additional custom field data. Preferred format is JSON.</param>
        /// <param name="ClientRegistered">Registration status.</param>
        /// <param name="ClientInitialStartTerm">Initial start term.</param>
        /// <param name="ClientNotes">Notes associated with the lead.</param>
        /// <param name="ClientStatus">
        /// Current client status. Partners should map their statuses to the supported standardized client status values.
        /// &lt;br /&gt;
        /// Certain client statuses correspond to client date fields. If a supported ClientStatus is provided and the matching
        /// date field is not provided, the service may default that matching date to the current date when the lead does not
        /// already have a value for that date.
        /// &lt;br /&gt;
        /// Supported standardized status values:
        /// &lt;ul&gt;
        /// &lt;li&gt;ApplicationsCompleted&lt;/li&gt;
        /// &lt;li&gt;Interviews&lt;/li&gt;
        /// &lt;li&gt;Starts&lt;/li&gt;
        /// &lt;li&gt;Contacts&lt;/li&gt;
        /// &lt;li&gt;Enrollments&lt;/li&gt;
        /// &lt;li&gt;ApplicationsStarted&lt;/li&gt;
        /// &lt;li&gt;Admit&lt;/li&gt;
        /// &lt;li&gt;Appointment&lt;/li&gt;
        /// &lt;li&gt;Qualifies&lt;/li&gt;
        /// &lt;li&gt;Graduated&lt;/li&gt;
        /// &lt;li&gt;Persists&lt;/li&gt;
        /// &lt;li&gt;ApplicationSubmitted&lt;/li&gt;
        /// &lt;li&gt;Deposit&lt;/li&gt;
        /// &lt;li&gt;ApplicationsDenied&lt;/li&gt;
        /// &lt;/ul&gt;
        /// </param>        
        /// <param name="ClientApplicationStartTerm">Application start term.</param>
        /// <param name="PendingApplicationChecklistItems">Pending application checklist items.</param>
        /// <param name="CompletedApplicationChecklistItems">Completed application checklist items.</param>
        /// <param name="ClientApplicationDate">Application date.</param>
        /// <param name="ClientStatusUpdatedDate">Status updated date.</param>
        /// <param name="ClientInterviewDate">Interview date.</param>
        /// <param name="ClientStartDate">Start date.</param>
        /// <param name="ClientStartReceivedDate">Start received date.</param>
        /// <param name="ClientContactDate">Contact date.</param>
        /// <param name="ClientEnrollDate">Enrollment date.</param>
        /// <param name="ClientApplicationStartDate">Application start date.</param>
        /// <param name="ClientAdmitDate">Admit date.</param>
        /// <param name="ClientAppointmentDate">Appointment date.</param>
        /// <param name="ClientQualifiedDate">Qualified date.</param>
        /// <param name="ClientGraduateDate">Graduation date.</param>
        /// <param name="ClientFirstTermPersistDate">First term persist date.</param>
        /// <param name="ClientFAFSAReceivedDate">FAFSA received date.</param>
        /// <param name="ClientApplicationSubmittedDate">Application submitted date.</param>
        /// <param name="ClientDepositDate">Deposit date.</param>
        /// <param name="ClientApplicationDeniedDate">Application denied date.</param>
        /// <example>
        /// {
        ///   "apiKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///   "emsInstitutionId": XXX,
        ///   "lookUpKeyList": [1,4],
        ///   "externalId": "ABC123",
        ///   "email": "test@test.com",
        ///   "clientApplicationDegreeName": "Bachelor of Science",
        ///   "customFields": "{\"FieldName\":\"FieldValue\"}",
        ///   "clientRegistered": "Yes",
        ///   "clientNotes": "Updated client notes.",
        ///   "clientStatus": "ApplicationsCompleted",
        ///   "clientApplicationStartTerm": "Fall 2026",
        ///   "pendingApplicationChecklistItems": "Transcript, FAFSA",
        ///   "clientApplicationDate": "2026-05-06",
        ///   "clientStatusUpdatedDate": "2026-05-06"
        /// }
        /// </example>
        /// <returns>
        /// Successfully updated leads will return with an "IsSuccessful" value of true.
        /// The "Body" may include lead update context such as EMSInstitutionId and LeadGUID when available.
        /// The "Messages" collection will include one or more response messages from the API message configuration.
        ///
        /// Example successful response:
        /// {
        ///   "IsSuccessful": true,
        ///   "Body": {
        ///     "EMSInstitutionId": 1234,
        ///     "LeadGUID": "xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx"
        ///   },
        ///   "ResponseDateTime": "2026-05-08T14:39:28.1899614-04:00",
        ///   "RequestDateTime": "2026-05-08T14:39:28.1779627-04:00",
        ///   "ResponseGuid": "b959696d-5bb5-4954-9bd6-d7cc3d981257",
        ///   "Messages": [
        ///     {
        ///       "MessageCode": "msg0117",
        ///       "Message": "Lead update was submitted successfully."
        ///     }
        ///   ],
        ///   "TotalResponseTime": 0
        /// }
        ///
        /// If the lead is found but no update is needed, the response may return IsSuccessful as true with a Body value
        /// indicating "No Updates Needed" and a message explaining that the lead was found but no values changed.
        ///
        /// Validation errors will return a Bad Request response with "IsSuccessful" as false, "Body" as "ValidationFailed",
        /// and one or more messages explaining what needs to be corrected.
        ///
        /// Possible validation messages include:
        /// - EMSInstitutionId is required.
        /// - LeadStateId must be either 1 or 2 when provided.
        /// - LookUpKeyList is required.
        /// - Invalid lookup key.
        /// - At least one valid lookup value is required for the provided LookUpKeyList.
        ///
        /// Example validation failure response:
        /// {
        ///   "IsSuccessful": false,
        ///   "Body": "ValidationFailed",
        ///   "ResponseDateTime": "2026-05-08T14:39:28.1899614-04:00",
        ///   "RequestDateTime": "2026-05-08T14:39:28.1779627-04:00",
        ///   "ResponseGuid": "b959696d-5bb5-4954-9bd6-d7cc3d981257",
        ///   "Messages": [
        ///     {
        ///       "MessageCode": "msg0108",
        ///       "Message": "EMSInstitutionId is required."
        ///     },
        ///     {
        ///       "MessageCode": "msg0109",
        ///       "Message": "LeadStateId must be either 1 or 2 when provided."
        ///     },
        ///     {
        ///       "MessageCode": "msg0110",
        ///       "Message": "LookUpKeyList is required."
        ///     }
        ///   ],
        ///   "TotalResponseTime": 0
        /// }
        ///
        /// If no existing EMS lead can be found using the provided lookup keys, the response will return
        /// "IsSuccessful" as false, a Body value indicating "Lead Not Found", and a configured response message.
        ///
        /// If the lead update cannot be submitted to the lead service, the response will return
        /// "IsSuccessful" as false, a Body value indicating "Update Failed", and a configured response message.
        ///
        /// If an unexpected exception occurs while updating the lead, the response will return
        /// "IsSuccessful" as false and include a configured exception response message.
        /// </returns>
        #endregion lead update documentation


        [HttpPost]
        [HostAndPostCampaignFilter]
        [LeadUpdateValidationFilter]
        [ActionName("lead-update")]
        public HttpResponseMessage PostLeadUpdate([FromBody] LeadUpdateRequest leadUpdateRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                PostLeadUpdateResponseModel responseContent = new PostLeadUpdateResponseModel(leadUpdateRequest);

                logResponse(responseContent, leadUpdateRequest.APIKey, this.Request, VendorResponseBase.OperationType.LeadUpdate, leadUpdateRequest.Email);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

            }
            catch (Exception exc)
            {
                Logging.LogException(exc, Log.GetCurrentMethodName());
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.WCF_SERVICE_POLICY);

                VendorResponseBase responseContent = new VendorResponseBase();

                responseContent.IsSuccessful = false;
                responseContent.ResponseGuid = Guid.NewGuid();
                responseContent.RequestDateTime = DateTime.Now;
                responseContent.Messages = new List<VendorResponseMessage>();

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.GeneralException);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }

                responseContent.ResponseDateTime = DateTime.Now;
                httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;

                logResponse(responseContent, leadUpdateRequest.APIKey, this.Request, VendorResponseBase.OperationType.LeadUpdate, leadUpdateRequest.Email);
                if (!responseContent.IsSuccessful)
                {
                    responseContent.Body = null;
                }
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

            }
            return httpResponseMessage;

        }
    }
}
