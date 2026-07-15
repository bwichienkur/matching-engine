using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Vendor.Web.API.Filters;
using EDDY.IS.Vendor.Web.API.Models;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web.Http;
using System.Web.Http.Cors;
using EDDY.IS.Vendor;
namespace EDDY.IS.Vendor.Web.API.Controllers
{

    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class UtilController : BaseAPIController
    {
        [HttpGet]
        [DisabledCampusFilter]
        [DisabledCategoryFilter]
        [DisabledSubjectFilter]
        [ActionName("checkapikey")]
        public HttpResponseMessage CheckApiKey()
        {
            return Request.CreateResponse(HttpStatusCode.OK, true.ToString());
        }



        [HttpGet]
        [ActionName("gettimezone")]
        public HttpResponseMessage GetTimezone([FromUri] TimezoneRequest timezoneRequest)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage(HttpStatusCode.NoContent);
            try
            {
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                VendorResponseBase responseContent = new GetTimezoneResponseModel(timezoneRequest);
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                this.Request.Content = new ObjectContent(timezoneRequest.GetType(), timezoneRequest, new JsonMediaTypeFormatter());
                logResponse(responseContent, timezoneRequest.APIKey, this.Request, VendorResponseBase.OperationType.Search);
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
                responseContent.ResponseDateTime = DateTime.Now;
                httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
                httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                responseContent.Messages = new List<VendorResponseMessage>();
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.GeneralException);
                if (message != null)              
                    responseContent.Messages.Add(message);

                this.Request.Content = new ObjectContent(timezoneRequest.GetType(), timezoneRequest, new JsonMediaTypeFormatter());
                logResponse(responseContent, timezoneRequest.APIKey, this.Request, VendorResponseBase.OperationType.Exception);
            }
            return httpResponseMessage;

        }
    }
}