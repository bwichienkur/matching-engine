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
    public class LeadSourceFilter : ActionFilterBase
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
                }
            }
            if (vendorCampaign != null)
            {
                if (vendorCampaign.ApplicationId != 27)
                {
                    if (vendorCampaign.IsSourceRequired)
                    {

                        bool isRequired = true;
                        if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("RequireContactSource")))
                        {
                            isRequired = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("RequireContactSource"));
                        }

                        var leadSourceUrl = requestValues["LeadSourceUrl"];

                        if (string.IsNullOrEmpty(leadSourceUrl) && isRequired)
                        {

                            responseContent.MessageCodes.Add(InputValidation.MessageCodes.LeadSourceUrlIsRequired);

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(leadSourceUrl) && leadSourceUrl.Length < 5)
                            {
                                responseContent.MessageCodes.Add(InputValidation.MessageCodes.LeadSourceUrlIsInvalid);
                            }
                        }

                        var leadInitiatingUrl = requestValues["LeadInitiatingUrl"];

                        if (string.IsNullOrEmpty(leadInitiatingUrl) && isRequired)
                        {

                            responseContent.MessageCodes.Add(InputValidation.MessageCodes.LeadInitiatingUrlIsRequired);

                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(leadInitiatingUrl) && leadInitiatingUrl.Length < 5)
                            {
                                responseContent.MessageCodes.Add(InputValidation.MessageCodes.LeadInitiatingUrlIsInvalid);
                            }
                        }

                        responseContent.ResponseDateTime = DateTime.Now;
                        invalidHttpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                        if (responseContent.MessageCodes.Count > 0)
                        {
                            foreach (var messageCode in responseContent.MessageCodes)
                            {
                                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(messageCode);
                                if (message != null)
                                {
                                    responseContent.Messages.Add(message);
                                }
                            }
                            actionContext.Response = invalidHttpResponseMessage;
                        }

                        if (responseContent.Messages.Count > 0)
                        {
                            //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                            //vendorResponseLog.APIKey = Guid.Parse(requestValues["apikey"]);
                            //vendorResponseLog.IPAddress = actionContext.Request.GetClientIpAddress();
                            //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                            //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(actionContext.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                            //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(actionContext.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                            //vendorResponseLog.Operation = VendorResponseBase.OperationType.Validation;
                            //logs.LogVendorResponse(vendorResponseLog);

                            logResponse(responseContent, requestValues["apikey"], actionContext.Request, requestValues["email"]);
                        }
                    }
                }
            }

        }
    }

}