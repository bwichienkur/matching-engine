using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using System.Web.Http.Description;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Web.API.Filters;
using EDDY.IS.Vendor.Web.API.Models;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Common.ExceptionHandler;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace EDDY.IS.Vendor.Web.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "POST")]
    public class LeadController : BaseAPIController
    {

        //POST api/lead/save
        #region  api/lead/save documentation
        /// <summary>
        /// <para>The Host and Post service is located at https://partners.educationdynamics.com/api/lead/save . The posting service can be called using simple HTTP POST, and will return an JSON-formatted response indicating the success or failure of the partner’s submission, along with details if applicable. The Request must include all the required key-value pairs defined by the API’s https://partners.educationdynamics.com/api/program/form method.</para> 
        /// </summary>
        /// <ParameterDescription>Test</ParameterDescription>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="AgentId">Required field if Call Center Partner.</param>
        /// <param name="AgentName">Required field if Call Center Partner.</param>
        /// <param name="DialerKey">Required field if Call Center Partner.</param>
        /// <param name="CategoryIds">Category(s) Comma-delimited list of CategoryIds.</param>
        /// <param name="SubCategoryIds">Subcategory(s) Comma-delimited list of SubCategoryIds.</param> 
        /// <param name="ProgramLevelId">We only accept the string value i.e. "2". Accepted values:  { "ProgramLevelId": 2,"ProgramLevelName": "Associate"},{ "ProgramLevelId": 3,"ProgramLevelName": "Bachelor"},{ "ProgramLevelId": 22,"ProgramLevelName": "Diploma"},{"ProgramLevelId": 8,"ProgramLevelName": "Master"},{"ProgramLevelId": 18,"ProgramLevelName": "Undergraduate Certificate"}</param> 
        /// <param name="Prefix">Prefix or title, e.g., "Mrs.", "Mr.", "Ms.".</param> 
        /// <param name="FirstName">Required | First name.</param>
        /// <param name="LastName">Required | Last name.</param>
        /// <param name="Address">Address</param>
        /// <param name="City">Required | City.</param> 
        /// <param name="PostalCode">Required | Postal code or Zip code.</param>
        /// <param name="State">Required | Two-character US state abbreviation, e.g. "NJ".</param> 
        /// <param name="Country">Required | Two-character country code, e.g., "US".</param>
        /// <param name="USCitizen">Is a Citizen of the United States. Accepted values: "Yes", "No".</param> 
        /// <param name="Email">Email address.</param>
        /// <param name="Phone">Required | Phone number.</param> 
        /// <param name="AlternatePhone">Optional | Alternate phone number.</param>
        /// <param name="Age">Age.</param> 
        /// <param name="YearHighestEducationCompleted">Year Highest Education was completed.</param>
        /// <param name="HighestLevelofEducationCompleted">Id of the highest level of education completed. We only accept the string value i.e., "2". Accepted Values: [{"Value": "2", "Text": "G.E.D."}, {"Value": "3", "Text": "High School Diploma"}, {"Value": "4", "Text": "Some College ,1-29 Credits"}, {"Value": "5", "Text": "Some College, 30-59 Credits"}, {"Value": "6", "Text": "Some College,60-89 Credits"}, {"Value": "7", "Text": "Some College, 90+ Credits"}, {"Value": "8", "Text": "Associate"}, {"Value": "9", "Text": "Bachelor"}, {"Value": "10", "Text": "Master"}, {"Value": "11", "Text": "Doctorate"}, {"Value": "1", "Text": "Haven't completed High School"}].</param> 
        /// <param name="MilitaryAffiliation">Military Affiliation Id. We only accept the string value i.e., "126". Accepted Values: [{"Value": "126", "Text": "NoMilitaryAffiliation"}, {"Value": "101", "Text": "AF-ActiveDuty(AD)"}, {"Value": "105", "Text": "AF-Civilian"}, {"Value": "102", "Text": "AF-SelectiveReserve(SR)"}, {"Value": "103", "Text": "AF-SpouseofADorSR"}, {"Value": "104", "Text": "AF-Veteran"}, {"Value": "106", "Text": "AR-ActiveDuty(AD)"}, {"Value": "110", "Text": "AR-Civilian"}, {"Value": "107", "Text": "AR-SelectiveReserve(SR)"}, {"Value": "108", "Text": "AR-SpouseofADorSR"}, {"Value": "109", "Text": "AR-Veteran"}, {"Value": "111", "Text": "CG-ActiveDuty(AD)"}, {"Value": "115", "Text": "CG-Civilian"}, {"Value": "112", "Text": "CG-SelectiveReserve(SR)"}, {"Value": "113", "Text": "CG-SpouseofADorSR"}, {"Value": "114", "Text": "CG-Veteran"}, {"Value": "116", "Text": "MC-ActiveDuty(AD)"}, {"Value": "120", "Text": "MC-Civilian"}, {"Value": "117", "Text": "MC-SelectiveReserve(SR)"}, {"Value": "118", "Text": "MC-SpouseofADorSR"}, {"Value": "119", "Text": "MC-Veteran"}, {"Value": "121", "Text": "NV-ActiveDuty(AD)"}, {"Value": "125", "Text": "NV-Civilian"}, {"Value": "122", "Text": "NV-SelectiveReserve(SR)"}, {"Value": "123", "Text": "NV-SpouseofADorSR"}, {"Value": "124", "Text": "NV-Veteran"}].</param>
        /// <param name="DesiredStartDate">Desired start date. We only accept the string value i.e., "Immediately". Accepted Values: [{"Value": "Immediately", "Text": "Immediately"}, {"Value": "1-3 Months", "Text":"1-3 Months"}, {"Value":"4-6 Months", "Text": "4-6 Months"}, {"Value": "7-12 Months", "Text": "7-12 Months"}, {"Value": "More than 1 Year", "Text": "More than 1 Year"}, {"Value": "Not Sure", "Text": "Not Sure"}]</param> 
        /// <param name="USCitizen">Is a Citizen of the United States. Accepted values: "Yes", "No".</param> 
        /// <param name="ProgramId">Program Id.</param> 
        /// <param name="CampusId">Campus Id.</param> 
        /// <param name="AffiliateId">Affiliate Id.</param> 
        /// <param name="AdditionalQuestions">Additional prospect data, if available.</param> 
        /// <param name="LeadIdToken">Required | The ID provided by Jornaya LeadiD.</param>
        /// <param name="LeadSourceUrl">Required | Valid URL where the lead was generated (Landing URL).</param>
        /// <param name="LeadInitiatingUrl">Required | Valid URL where the lead was initiated (Initiating URL).</param>
        /// <param name="SS1">Sub affiliate.</param>
        /// <param name="SS2">BPO Company Name.</param>
        /// <param name="SourceCode">Required when EMS Partner.</param>
        /// <param name="UserAgreement">Required | TCPA information.</param> 
        ///<example>
        /// {
        ///     "apiKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "categoryIds": [25],
        ///     "firstName": "Testfirst",
        ///     "lastname": "Testlast",
        ///     "prefix": "Mr.",
        ///     "address": "801 test Corner Ct",
        ///     "address2": "apt a",
        ///     "city": "Green Bay",
        ///     "postalcode": "55555",
        ///     "state": "WI",
        ///     "country": "US",
        ///     "email": "test@test.com",
        ///     "phone": "5555555555",
        ///     "alternatePhone": "5555555555",
        ///     "age": "21",
        ///     "yearHighestEducationCompleted": "1978",
        ///     "highestLevelofEducationCompleted": "9",
        ///     "militaryAffiliation": "126",
        ///     "desiredStartDate": "Immediately",
        ///     "USCitizen": "Yes",
        ///     "subjectIds": [658],
        ///     "programLevelID": 2,
        ///     "includeAdditionalProgramQuestions": true,
        ///     "programId": 7243,
        ///     "campusId": 4919,
        ///     "affiliateId": "",
        ///     "additionalQuestions": [{"QuestionKey":"RNLicense","QuestionValue":"Yes"}],
        ///     "leadSourceUrl": "https://www.EducationDynamics.com/lead",
        ///     "leadInitiatingUrl": "https://www.EducationDynamics.com/start",
        ///     "SS1": "Sub affiliate",
        ///     "SS2": "BPO Company Name",
        ///     "leadIdToken": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        ///     "userAgreement": "I agree to the user Agreement"
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
        [YearHighestEducationCompletedFilter]
        [TCPAFilter]
        [LeadSourceFilter]
        [PostLeadActionFilter]
        [AgentNameFilter]
        [ActionName("save")]
        public HttpResponseMessage PostLead([FromBody] ContactRequest contactRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                PostLeadResponseModel responseContent = new PostLeadResponseModel(contactRequest);
                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.LeadSave, contactRequest.Email);
                if (!responseContent.IsSuccessful)
                    responseContent.Body = null;

                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = contactRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.LeadSave;

                //if (responseContent.Body != null)
                //{
                //    LeadSubmissionResponse leadSubmissionResponse = responseContent.Body as LeadSubmissionResponse;

                //    if (leadSubmissionResponse != null)
                //    {
                //        vendorResponseLog.OperationValue = leadSubmissionResponse.UID;
                //    }
                //}


                //logs.LogVendorResponse(vendorResponseLog);

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
                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.Exception, contactRequest.Email);
                if (!responseContent.IsSuccessful)
                    responseContent.Body = null;

                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = contactRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Exception;
                //logs.LogVendorResponse(vendorResponseLog);



            }
            return httpResponseMessage;

        }

      
    }
}
