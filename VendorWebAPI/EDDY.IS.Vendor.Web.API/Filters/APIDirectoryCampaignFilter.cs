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
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.Business;

using Newtonsoft.Json;

namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class APIDirectoryCampaignFilter : ActionFilterBase
    {
        private VendorCampaigns vendorCampaigns = new VendorCampaigns();
        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            HttpResponseMessage invalidHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            VendorResponseBase responseContent = new VendorResponseBase();
            responseContent.ResponseGuid = Guid.NewGuid();
            responseContent.RequestDateTime = DateTime.Now;
            responseContent.IsSuccessful = false;

            responseContent.Messages = new List<VendorResponseMessage>();
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
                var apikeyParam = requestValues["apikey"];


                if (!String.IsNullOrEmpty(apikeyParam))
                {
                    Guid apiKey = Guid.Empty;
                    if (Guid.TryParse(apikeyParam, out apiKey))
                    {
                        VendorCampaign vendorCampaign = getVendorCampaignByTrackId(apiKey);
                        if (vendorCampaign != null)
                        {
                            if (vendorCampaign.ApplicationId != 27 && !vendorCampaign.IsAPIDirectory)
                            {
                                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.FeatureNotConfiguredAPIDirectory);
                                if (message != null)
                                {
                                    responseContent.Messages.Add(message);
                                }
                            }
                        }
                    }
                }
                responseContent.ResponseDateTime = DateTime.Now;

                invalidHttpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());
                if (responseContent.Messages.Count > 0)
                {
                    //VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                    //vendorResponseLog.APIKey = Guid.Parse(requestValues["apikey"]);
                    //vendorResponseLog.IPAddress = actionContext.Request.GetClientIpAddress();
                    //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                    //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(actionContext.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                    //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(actionContext.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                    //logs.LogVendorResponse(vendorResponseLog);
                    logResponse(responseContent, requestValues["apikey"], actionContext.Request, requestValues["email"]);

                    actionContext.Response = invalidHttpResponseMessage;
                }


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
    }
}