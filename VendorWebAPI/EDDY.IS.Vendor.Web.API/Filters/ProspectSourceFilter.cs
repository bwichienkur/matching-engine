using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Net.Http.Formatting;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Common.Utilities;
using Newtonsoft.Json;
using EDDY.IS.Vendor.Business;
using System.Configuration;
namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class ProspectSourceFilter : ActionFilterBase
    {


        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            HttpResponseMessage invalidHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            VendorResponseBase responseContent = new VendorResponseBase();
            responseContent.ResponseGuid = Guid.NewGuid();
            responseContent.RequestDateTime = DateTime.Now;
            responseContent.IsSuccessful = false;

            responseContent.Messages = new List<VendorResponseMessage>();
            responseContent.MessageCodes = new List<string>();
            NameValueCollection requestValues = null;
            switch (actionContext.Request.Method.Method)
            {
                case "GET":
                    requestValues = actionContext.Request.GetRequestQueryParameters();
                    break;
                case "POST":
                    requestValues = actionContext.Request.GetRequestJsonBodyParameters();
                    break;
            }
            string apikeyString = requestValues["apikey"];
            VendorCampaign vendorCampaign = null;
            if (!String.IsNullOrEmpty(apikeyString))
            {
                Guid apiKey = Guid.Empty;
                if (Guid.TryParse(apikeyString, out apiKey))
                {
                    vendorCampaign = getVendorCampaignByTrackId(apiKey);

                    if (vendorCampaign != null && (vendorCampaign.CampaignTypeId == 7 || vendorCampaign.CampaignTypeId == 8))
                    {
                        if (vendorCampaign.ApplicationId != 27 && vendorCampaign.IsSourceRequired)
                        {
                            string code = GetUrlMessageCode(requestValues["ProspectSourceUrl"], InputValidation.MessageCodes.ProspectSourceIsRequired, InputValidation.MessageCodes.ProspectSourceUrlIsInvalid);

                            if (!String.IsNullOrEmpty(code)) responseContent.MessageCodes.Add(code);

                            code = GetUrlMessageCode(requestValues["ProspectInitiatingUrl"], InputValidation.MessageCodes.ProspectInitiatingUrlIsRequired, InputValidation.MessageCodes.ProspectInitiatingUrlIsInvalid);
                            if (!String.IsNullOrEmpty(code)) responseContent.MessageCodes.Add(code);
                        }
                    }
                    else
                    {
                        responseContent.MessageCodes.Add(InputValidation.MessageCodes.FeatureNotConfiguredAPIDirectory);
                    }

                    responseContent.ResponseDateTime = DateTime.Now;
                    invalidHttpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                    if (responseContent.MessageCodes.Count > 0)
                    {
                        foreach (var messageCode in responseContent.MessageCodes)
                        {
                            VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(messageCode);
                            if (message != null)
                                responseContent.Messages.Add(message);
                        }
                        actionContext.Response = invalidHttpResponseMessage;
                    }

                    if (responseContent.Messages.Count > 0)
                        LogUrlResponse(responseContent, apiKey, actionContext.Request.GetClientIpAddress(),
                                       actionContext.Request.GetRequestQueryParametersAsString(),
                                       actionContext.Request.GetRequestJsonBodyParametersAsString(),
                                       actionContext.Request,
                                       requestValues);
                }
            }
        }

        private void LogUrlResponse(VendorResponseBase responseContent, Guid apiKey, string ipAddress, string qs, string body, HttpRequestMessage requestMessage, NameValueCollection requestValues)
        {
            if (responseContent.Messages.Count > 0)
            {
                //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                //vendorResponseLog.APIKey = apiKey;
                //vendorResponseLog.IPAddress = ipAddress;
                //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(qs).HtmlEncode();
                //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(body).HtmlEncode();
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Validation;
                //logs.LogVendorResponse(vendorResponseLog);

                logResponse(responseContent, requestValues["apikey"], requestMessage, requestValues["email"]);
            }
        }
        private string GetUrlMessageCode(string url, string missingCode, string invalidCode)
        {
            if (string.IsNullOrEmpty(url))
                return missingCode;
            else
            {
                if (url.Length < 5)
                   return InputValidation.MessageCodes.ProspectSourceUrlIsInvalid;
            }

            return "";
        }
    }

}