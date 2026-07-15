using System;
using System.Collections.Generic;
using System.Net;
using System.Collections.Specialized;
using System.Web.Http.Filters;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Net.Http.Formatting;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Common.ExceptionHandler;
using Newtonsoft.Json;
namespace EDDY.IS.Vendor.Web.API.Filters
{

    public class CampaignAuthorizationFilter : AuthorizationFilterAttribute
    {
        private VendorCampaigns vendorCampaigns = new VendorCampaigns();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        private Logs logs = new Logs();


        public override void OnAuthorization(HttpActionContext actionContext)
        {
            string email = null;
            string apikeyParam = null;
            try
            {
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


                if (requestValues != null)
                {
                    apikeyParam = requestValues["apikey"];
                    email = requestValues["email"];

                    if (!String.IsNullOrEmpty(apikeyParam))
                    {
                        Guid apiKey = Guid.Empty;
                        if (Guid.TryParse(apikeyParam, out apiKey))
                        {
                            VendorCampaign vendorCampaign = getVendorCampaignByTrackId(apiKey);
                            if (vendorCampaign != null)
                            {

                                if (!vendorCampaign.IsEnabled)
                                {
                                    switch (vendorCampaign.CampaignStatus.ToLower())
                                    {

                                        case "expired":
                                            actionContext.Response = getUnAuthorizedCampaignHttpResponseMessage(InputValidation.MessageCodes.CampaignExpired, actionContext, apikeyParam, email);
                                            break;
                                        case "pending":
                                            actionContext.Response = getUnAuthorizedCampaignHttpResponseMessage(InputValidation.MessageCodes.CampaignPending, actionContext, apikeyParam, email);
                                            break;
                                        case "inactive":
                                            actionContext.Response = getUnAuthorizedCampaignHttpResponseMessage(InputValidation.MessageCodes.CampaignInactive, actionContext, apikeyParam, email);
                                            break;
                                        case "terminated":
                                            actionContext.Response = getUnAuthorizedCampaignHttpResponseMessage(InputValidation.MessageCodes.CampaignTerminated, actionContext, apikeyParam, email);
                                            break;
                                        default:
                                            actionContext.Response = getUnAuthorizedCampaignHttpResponseMessage(InputValidation.MessageCodes.CampaignDisabled, actionContext, apikeyParam, email);
                                            break;
                                    }

                                }
                                else
                                {
                                    if (vendorCampaign.APIRateExceeded)
                                    {
                                        actionContext.Response = getUnAuthorizedCampaignHttpResponseMessage(InputValidation.MessageCodes.APICallRateExceeded, actionContext, apikeyParam, email);
                                    }
                                }
                            }
                            else
                            {
                                actionContext.Response = getUnAuthorizedCampaignHttpResponseMessage(InputValidation.MessageCodes.InvalidAPIKey, actionContext, apikeyParam, email);
                            }
                        }
                        else
                        {
                            actionContext.Response = getUnAuthorizedCampaignHttpResponseMessage(InputValidation.MessageCodes.APIKeyNotGuid, actionContext, apikeyParam, email);
                        }
                    }
                    else
                    {
                        actionContext.Response = getUnAuthorizedCampaignHttpResponseMessage(InputValidation.MessageCodes.MissingAPIKey, actionContext, apikeyParam, email);
                    }
                }
                else
                {
                    actionContext.Response = getUnAuthorizedCampaignHttpResponseMessage(InputValidation.MessageCodes.MissingAPIKey, actionContext, apikeyParam, email);
                }
            }
            catch (JsonReaderException exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                actionContext.Response = getBadFormatHttpResponseMessage(InputValidation.MessageCodes.InvalidRequestBody, actionContext, exc.Message, apikeyParam, email);
            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
        }
        private VendorCampaign getVendorCampaignByTrackId(Guid trackId)
        {
            VendorCampaign vendorCampaign = null;
            try
            {
                vendorCampaign = vendorCampaigns.GetVendorCampaignByTrackId(trackId);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.WCF_SERVICE_POLICY);
            }
            return vendorCampaign;
        }

        private HttpResponseMessage setHttpResponseMessage(HttpResponseMessage httpResponseMessage, VendorResponseMessage message, string reason, HttpActionContext actionContext, string apiKey, string email)
        {
            VendorResponseBase responseContent = new VendorResponseBase();
            responseContent.ResponseGuid = Guid.NewGuid();
            responseContent.RequestDateTime = DateTime.Now;
            responseContent.IsSuccessful = false;
            responseContent.Messages = new List<VendorResponseMessage>();
            if (message != null)
            {
                responseContent.Messages.Add(message);
            }
            responseContent.ResponseDateTime = DateTime.Now;

            httpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

            Guid apiKeyGuid = Guid.Empty;
            Guid.TryParse(apiKey, out apiKeyGuid);

            VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
            vendorResponseLog.IPAddress = actionContext.Request.GetClientIpAddress();
            vendorResponseLog.MethodName = Log.GetCurrentMethodName();
            vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(actionContext.Request.GetRequestQueryParametersAsString()).HtmlEncode();
            vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(actionContext.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
            vendorResponseLog.Operation = VendorResponseBase.OperationType.Validation;
            vendorResponseLog.APIKey = apiKeyGuid;
            vendorResponseLog.EndPoint = actionContext.Request.RequestUri.AbsolutePath;
            vendorResponseLog.Request = actionContext.Request.Content.ReadAsStringAsync().Result;
            vendorResponseLog.Response = JsonConvert.SerializeObject(responseContent);
            vendorResponseLog.Email = email;
            logs.LogEddyApiResponse(vendorResponseLog);

            return httpResponseMessage;
        }

        private HttpResponseMessage getUnAuthorizedCampaignHttpResponseMessage(string reason, HttpActionContext actionContext, string apiKey, string email)
        {
            HttpResponseMessage unauthorizedHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(reason);
            return setHttpResponseMessage(unauthorizedHttpResponseMessage, message, reason, actionContext, apiKey, email);
        }

        private HttpResponseMessage getBadFormatHttpResponseMessage(string reason, HttpActionContext actionContext, string message, string apiKey, string email)
        {
            HttpResponseMessage badFormatHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            VendorResponseMessage responseMessage = vendorResponseMessages.GetVendorResponseMessageByMessageCode(reason);
            if (responseMessage != null && !string.IsNullOrWhiteSpace(message))
            {
                responseMessage.Message = message;
            }
            return setHttpResponseMessage(badFormatHttpResponseMessage, responseMessage, reason, actionContext, apiKey, email);
        }
    }
}