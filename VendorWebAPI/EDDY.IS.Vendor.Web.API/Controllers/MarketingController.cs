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
    public class MarketingController : BaseAPIController
    {

        //POST api/Institutions/lead/save
        #region  api/Marketing/lead/ documentation
        /// <summary>
        /// <para>The Host and Post service is located at https://partners.educationdynamics.com/api/Marketing/lead-save . The posting service can be called using simple HTTP POST, and will return an JSON-formatted response indicating the success or failure of the partner’s submission, along with details if applicable. The Request must include all the required key-value pairs defined by the API’s https://partners.educationdynamics.com/api/program/form method.  </para> 
        /// </summary>
        /// <ParameterDescription>Test</ParameterDescription>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="FirstName">Required | First name.</param>
        /// <param name="LastName">Required | Last name.</param>
        /// <param name="Address">Address.</param>
        /// <param name="City">Required | City.</param> 
        /// <param name="State">Two-character US state abbreviation, e.g., "NJ".</param>  
        /// <param name="PostalCode">Required | Postal code or Zip code.</param>
        /// <param name="Country">Required | Two-character country code, e.g., "US".</param>
        /// <param name="Email">Required | Email address.</param>
        /// <param name="Phone">Required | Phone number.</param> 
        /// <param name="ProgramId">Required | Program Id.</param> 
        /// <param name="HighestLevelofEducationCompleted">Id of the highest level of education completed. We only accept the string value i.e., "2". Accepted Values: [{"Value": "2", "Text": "G.E.D."}, {"Value": "3", "Text": "High School Diploma"}, {"Value": "4", "Text": "Some College ,1-29 Credits"}, {"Value": "5", "Text": "Some College, 30-59 Credits"}, {"Value": "6", "Text": "Some College,60-89 Credits"}, {"Value": "7", "Text": "Some College, 90+ Credits"}, {"Value": "8", "Text": "Associate"}, {"Value": "9", "Text": "Bachelor"}, {"Value": "10", "Text": "Master"}, {"Value": "11", "Text": "Doctorate"}, {"Value": "1", "Text": "Haven't completed High School"}].</param> 
        /// <param name="CampusId">Campus Id.</param> 
        /// <param name="AdditionalQuestions">Additional prospect data, if available.</param> 
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
        #endregion  api/Marketing/lead/ documentation
        [HttpPost]
        [HostAndPostCampaignFilter]
        [CampusFilter]
        [SubjectFilter]
        [AgeFilter]
        [CategoryFilter]        
        [TCPAFilter]
        [PostLeadActionFilter]
        [ActionName("lead-save")]
        public HttpResponseMessage PostLeadBasic([FromBody] ContactRequest contactRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                PostLeadResponseModel responseContent = new PostLeadResponseModel(contactRequest);
                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.LeadSave, contactRequest.Email);
                if (!responseContent.IsSuccessful)
                {
                    responseContent.Body = null;
                }
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
                {
                    responseContent.Body = null;
                }
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
