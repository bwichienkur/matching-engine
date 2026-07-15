using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Vendor.Web.API.Models;
using EDDY.IS.Vendor.Web.API.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using System.Web.Http.Description;

namespace EDDY.IS.Vendor.Web.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "POST")]
    [ApiExplorerSettings(IgnoreApi = true)]
    public class CallCenterLeadController : BaseAPIController
    {
        [HttpPost]
        [ActionName("save")]
        public HttpResponseMessage PostLead([FromBody] ContactRequest contactRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                var responseContent = new PostCallCenterLeadResponseModel(contactRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                logResponse(responseContent, contactRequest.APIKey, this.Request, VendorResponseBase.OperationType.LeadSave, contactRequest.Email);

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

        // GET: EDUMAX
        [HttpPost]
        [HostAndPostCampaignFilter]
        [ActionName("edumaxsave")]
        public HttpResponseMessage PostEduMaxLead([FromBody] ContactRequest contactRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                PostEduMaxLeadResponseModel responseContent = new PostEduMaxLeadResponseModel(contactRequest);
                logEdumaxResponse(responseContent, contactRequest.APIKey, this.Request, contactRequest.Email);
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

                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = contactRequest.APIKey;
                //vendorResponseLog.IPAddress = this.Request.GetClientIpAddress();
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(this.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                //vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Exception;
                //logs.LogVendorResponse(vendorResponseLog);

                logEdumaxResponse(responseContent, contactRequest.APIKey, this.Request, contactRequest.Email); 
                if (!responseContent.IsSuccessful)
                    responseContent.Body = null;
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());
            }
            return httpResponseMessage;

        }

    }
}