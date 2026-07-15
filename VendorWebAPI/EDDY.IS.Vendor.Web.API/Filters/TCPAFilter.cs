using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http.Controllers;

namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class TCPAFilter : ActionFilterBase
    {

        private const int emsApplicationId = 27;

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            HttpResponseMessage invalidHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.Unauthorized);
            VendorResponseBase responseContent = new VendorResponseBase()
            {
                ResponseGuid = Guid.NewGuid(),
                RequestDateTime = DateTime.Now,
                IsSuccessful = false
            };

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
                var apikeyParam = requestValues["apiKey"];

                if (!string.IsNullOrEmpty(apikeyParam))
                {
                    Guid apiKey = Guid.Empty;
                    if (Guid.TryParse(apikeyParam, out apiKey))
                    {
                        VendorCampaign vendorCampaign = this.getVendorCampaignByTrackId(apiKey);
                        if (vendorCampaign?.ApplicationId != emsApplicationId)
                        {
                            var userAgreement = requestValues["UserAgreement"];
                            if (string.IsNullOrEmpty(userAgreement))
                            {
                                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.UserAgreementRequired);
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
                    //vendorResponseLog.APIKey = Guid.Parse(apikeyParam);
                    //vendorResponseLog.IPAddress = actionContext.Request.GetClientIpAddress();
                    //vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                    //vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(actionContext.Request.GetRequestQueryParametersAsString()).HtmlEncode();
                    //vendorResponseLog.RequestBodyParameters = JsonConvert.SerializeObject(actionContext.Request.GetRequestJsonBodyParametersAsString()).HtmlEncode();
                    //vendorResponseLog.Operation = VendorResponseBase.OperationType.Validation;
                    //logs.LogVendorResponse(vendorResponseLog);
                    logResponse(responseContent, requestValues["apikey"], actionContext.Request, requestValues["email"]);
                    actionContext.Response = invalidHttpResponseMessage;
                }
            }


        }



    }
}