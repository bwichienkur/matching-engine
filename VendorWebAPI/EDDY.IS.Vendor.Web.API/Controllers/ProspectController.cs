using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Web.API.Filters;
using EDDY.IS.Vendor.Web.API.Models;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Common.ExceptionHandler;
using Newtonsoft.Json;
using System.Web.Http.Description;
using System.Web.Http.Cors;

namespace EDDY.IS.Vendor.Web.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "POST")]
    [ProspectCampaignFilter]
    public class ProspectController : BaseAPIController
    {

        //POST api/prospect/save
        #region  api/prospect/save documentation
        /// <summary>
        /// <para>Once a campaign has been created, the APIKey and Posting instructions can be provided to the Call Center partner. The partner will post prospect data into ISHP using the APIKey provided.</para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="FirstName">Required | First name.</param>
        /// <param name="LastName">Required | Last name.</param>
        /// <param name="City">Required | City.</param> 
        /// <param name="PostalCode">Required | Postal code or Zip code.</param>
        /// <param name="State">Required | Two-character US state abbreviation, e.g., "NJ".</param> 
        /// <param name="Country">Required | Two-character country code, e.g., "US".</param>
        /// <param name="Email">Required | Email address.</param>
        /// <param name="Phone">Required | Phone number.</param> 
        /// <param name="AlternatePhone">Optional | Alternate phone number.</param>
        /// <param name="AreaOfInterest">Optional | Area of interest.</param> 
        /// <param name="YearHighestEducationCompleted">Year Highest Education was completed.</param>
        /// <param name="ExternalSystemId">External system Id.</param>
        /// <param name="ProspectSourceUrl">Required | Valid URL where the prospect was generated (CallCenter URL).</param>
        /// <param name="ProspectInitiatingUrl">Required | Valid URL where the prospect was initiated.</param>
        /// <example>
        /// 
        ///{
        /// "apiKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        /// "firstName": "Testfirst",
        /// "lastname": "Testlast",
        /// "address": "801 test Corner Ct",
        /// "address2": "apt a",
        /// "city": "Green Bay",
        /// "postalcode": "55555",
        /// "state": "WI",
        /// "country": "US",
        /// "email": "test@test.com",
        /// "phone": "5555555555",
        /// "alternatePhone": "5555555555",
        /// "yearHighestEducationCompleted": "1978",
        /// "externalSystemId": "123"
        /// "prospectSourceUrl": "https://www.EducationDynamics.com/lead"
        /// "prospectInitiatingUrl": "https://www.EducationDynamics.com/start"
        ///}
        /// </example>
        /// <returns>
        /// {
        /// "IsSuccessful": true,
        /// "Body": {
        ///     "ProspectId": 8787805,
        ///     "ProspectFlowId": 10959192
        ///     },
        /// "ResponseDateTime": "2017-01-31T16:02:33.9357698-05:00",
        /// "RequestDateTime": "2017-01-31T16:02:07.5655017-05:00",
        /// "ResponseGuid": "6fc82d18-1f2e-4a1b-8fa6-36573a56f695",
        /// "Messages": [
        ///         {
        ///         "MessageCode": "msg0038",
        ///         "Message": "Validation Passed."
        ///         }
        ///     ],
        /// "TotalResponseTime": 13
        /// }
        /// </returns>
        #endregion  api/prospect/save documentation
        [HttpPost]
        [ProspectSourceFilter]
        [PostProspectActionFilter]
        [ActionName("save")]
        public HttpResponseMessage PostProspect([FromBody] ContactRequest contactRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                PostProspectResponseModel responseContent = new PostProspectResponseModel(contactRequest);
                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.ProspectSave, contactRequest.Email);
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
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.ProspectSave;
                //if (responseContent.Body != null)
                //{
                //    ProspectSubmissionResponse prospectSubmissionResponse = responseContent.Body as ProspectSubmissionResponse;

                //    if (prospectSubmissionResponse != null)
                //    {
                //        vendorResponseLog.OperationValue = prospectSubmissionResponse.ProspectFlowId.ToString();
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

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = contactRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Exception;
                //logs.LogVendorResponse(vendorResponseLog);
                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.ProspectSave, contactRequest.Email);
                if (!responseContent.IsSuccessful)
                {
                    responseContent.Body = null;
                }
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());
            }
            return httpResponseMessage;

        }


        //POST api/prospect/validate
        #region  api/prospect/validate documentation
        /// <summary>
        /// <para>Once a campaign has been created, the APIKey and Posting instructions can be provided to the Call Center partner. The partner will post a potential prospect data to ensure the it is valid before posting to save it using the APIKey provided.</para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="FirstName">Required | First name.</param>
        /// <param name="LastName">Required | Last name.</param>
        /// <param name="City">Required | City.</param> 
        /// <param name="PostalCode">Required | Postal code or Zip code. </param>
        /// <param name="State">Required | Two-character US state abbreviation, e.g., "NJ".</param> 
        /// <param name="Country">Required | Two-character country code, e.g., "US".</param>
        /// <param name="Email">Optional | Email address.</param>
        /// <param name="Phone">Required | Phone number.</param> 
        /// <param name="AlternatePhone">Optional | Alternate phone number.</param>
        /// <param name="AreaOfInterest">Optional | Area of interest.</param> 
        /// <param name="YearHighestEducationCompleted">Year Highest Education was completed.</param>
        /// <param name="ExternalSystemId">External system Id.</param>
        /// <example>
        /// 
        ///{
        /// "apiKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        /// "firstName": "Testfirst",
        /// "lastname": "Testlast",
        /// "address": "801 test Corner Ct",
        /// "address2": "apt a",
        /// "city": "Green Bay",
        /// "postalcode": "55555",
        /// "state": "WI",
        /// "country": "US",
        /// "email": "test@test.com",
        /// "phone": "5555555555",
        /// "alternatePhone": "5555555555",
        /// "yearHighestEducationCompleted": "1978",
        /// "externalSystemId": "123"
        ///}
        /// </example>
        /// <returns>
        /// {
        /// "IsSuccessful": true,
        /// "Body": null,
        /// "ResponseDateTime": "2017-01-31T16:02:33.9357698-05:00",
        /// "RequestDateTime": "2017-01-31T16:02:07.5655017-05:00",
        /// "ResponseGuid": "6fc82d18-1f2e-4a1b-8fa6-36573a56f695",
        /// "Messages": [
        ///         {
        ///         "MessageCode": "msg0069",
        ///         "Message": "Prospect is Valid"
        ///         }
        ///     ],
        /// "TotalResponseTime": 13
        /// }
        /// </returns>
        #endregion  api/prospect/validate documentation
        [HttpPost]
        [PostValidateProspectActionFilter]
        [ActionName("validate")]
        public HttpResponseMessage ValidateProspect([FromBody] ContactRequest contactRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                PostValidateProspectResponseModel responseContent = new PostValidateProspectResponseModel();
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = contactRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Validation;
                //logs.LogVendorResponse(vendorResponseLog);

                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.Validation, contactRequest.Email);
                
            }
            catch (Exception exc)
            {
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

                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.Exception, contactRequest.Email);

            }
            return httpResponseMessage;

        }


        //POST api/prospect/udpate
        #region  api/prospect/udpate documentation
        /// <summary>
        /// <para>Once a campaign has been created, the APIKey and Posting instructions can be provided to the Call Center partner. The partner will post prospect data into ISHP using the APIKey provided.</para> 
        /// </summary>
        /// <param name="APIKey">Required | Partner Credentials. This value will be provided by the EDDY Account Management team.</param>
        /// <param name="FirstName">Required | First name.</param>
        /// <param name="LastName">Required | Last name.</param>
        /// <param name="City">Required | City.</param> 
        /// <param name="PostalCode">Required | Postal code or Zip code. </param>
        /// <param name="State">Required | Two-character US state abbreviation, e.g., "NJ".</param> 
        /// <param name="Country">Required | Two-character country code, e.g., "US".</param>
        /// <param name="Email">Required | Email address.</param>
        /// <param name="Phone">Required | Phone number.</param> 
        /// <param name="AlternatePhone">Optional | Alternate phone number.</param>
        /// <param name="AreaOfInterest">Optional | Area of interest.</param> 
        /// <param name="YearHighestEducationCompleted">Year Highest Education was completed.</param>
        /// <param name="ExternalSystemId">External system Id.</param>
        /// <param name="ProspectSourceUrl">Required | Valid URL where the prospect was generated (Call Center URL).</param>
        /// <param name="ProspectInitiatingUrl">Required | Valid URL where the prospect was initiated.</param>
        /// <example>
        /// 
        ///{
        /// "apiKey": "XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX",
        /// "firstName": "Testfirst",
        /// "lastname": "Testlast",
        /// "address": "801 test Corner Ct",
        /// "address2": "apt a",
        /// "city": "Green Bay",
        /// "postalcode": "55555",
        /// "state": "WI",
        /// "country": "US",
        /// "email": "test@test.com",
        /// "Phone": "5555555555",
        /// "alternatePhone": "5555555555",
        /// "yearHighestEducationCompleted": "1978",
        /// "externalSystemId": "123"
        /// "prospectSourceUrl": "https://www.EducationDynamics.com/lead"
        /// "prospectInitiatingUrl": "https://www.EducationDynamics.com/start"
        ///}
        /// </example>
        /// <returns>
        /// {
        /// "IsSuccessful": true,
        /// "Body": {
        ///     "ProspectId": 8787805,
        ///     "ProspectFlowId": 10959192
        ///     },
        /// "ResponseDateTime": "2017-01-31T16:02:33.9357698-05:00",
        /// "RequestDateTime": "2017-01-31T16:02:07.5655017-05:00",
        /// "ResponseGuid": "6fc82d18-1f2e-4a1b-8fa6-36573a56f695",
        /// "Messages": [
        ///         {
        ///         "MessageCode": "msg0038",
        ///         "Message": "Validation Passed."
        ///         }
        ///     ],
        /// "TotalResponseTime": 13
        /// }
        /// </returns>
        #endregion  api/prospect/update documentation
        [HttpPost]
        [ProspectSourceFilter]
        [PostProspectActionFilter]
        [ActionName("savebyProspectid")]
        [ApiExplorerSettings(IgnoreApi = true)]
        public HttpResponseMessage PostProspectSaveByProspectId([FromBody] ContactRequest contactRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                PostProspectResponseModel responseContent = new PostProspectResponseModel(contactRequest);
                

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = contactRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.ProspectSave;
                //if (responseContent.Body != null)
                //{
                //    ProspectSubmissionResponse prospectSubmissionResponse = responseContent.Body as ProspectSubmissionResponse;

                //    if (prospectSubmissionResponse != null)
                //    {
                //        vendorResponseLog.OperationValue = prospectSubmissionResponse.ProspectFlowId.ToString();
                //    }
                //}
                //logs.LogVendorResponse(vendorResponseLog);

                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.ProspectSave, contactRequest.Email);
                if (!responseContent.IsSuccessful)
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

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = contactRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Exception;
                //logs.LogVendorResponse(vendorResponseLog);

                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.Exception, contactRequest.Email);
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
