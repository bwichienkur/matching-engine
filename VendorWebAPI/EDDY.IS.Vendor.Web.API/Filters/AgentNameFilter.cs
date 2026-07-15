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
using System.Configuration;

namespace EDDY.IS.Vendor.Web.API.Filters
{
	public class AgentNameFilter : ActionFilterBase
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

            var apikeyParam = requestValues["apikey"];
            var agentName = requestValues["AgentName"];
			bool agentNameRequired = false;

			if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("RequireAgentName")))
				agentNameRequired = System.Convert.ToBoolean(ConfigurationManager.AppSettings.Get("RequireAgentName"));

			if (!String.IsNullOrEmpty(apikeyParam) && agentNameRequired)
            {
                Guid apiKey = Guid.Empty;
                if (Guid.TryParse(apikeyParam, out apiKey))
                {
                    VendorCampaign vendorCampaign = getVendorCampaignByTrackId(apiKey);

                    //For Call Center Partners require AgentName
                    if (vendorCampaign.SubChannelId == 8 && String.IsNullOrEmpty(agentName))
                    {
                        responseContent.ResponseDateTime = DateTime.Now;
                        responseContent.MessageCodes.Add(InputValidation.MessageCodes.AgentNameIsRequired);

                        invalidHttpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

                        if (responseContent.MessageCodes.Count > 0)
                        {
                            foreach (string messageCode in responseContent.MessageCodes)
                            {
                                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(messageCode);
                                if (message != null)
                                {
                                    responseContent.Messages.Add(message);
                                }
                            }
                            actionContext.Response = invalidHttpResponseMessage;

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