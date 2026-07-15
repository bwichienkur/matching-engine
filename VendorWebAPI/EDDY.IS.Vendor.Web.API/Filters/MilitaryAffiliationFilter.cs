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

namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class MilitaryAffiliationFilter : ActionFilterBase
    {
        public MilitaryStatuses MilitaryStatuses { get; set; }

        public MilitaryAffiliationFilter()
        {
            MilitaryStatuses = new MilitaryStatuses();
        }

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

            string militaryAffiliation = requestValues["MilitaryAffiliation"];
            PerformValidation(militaryAffiliation, ref responseContent);
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

        private void PerformValidation(string militaryAffiliation, ref VendorResponseBase responseContent)
        {
            if (!String.IsNullOrEmpty(militaryAffiliation))
            {
                int parsedMilitaryAffiliation = 0;
                if (!int.TryParse(militaryAffiliation, out parsedMilitaryAffiliation))
                {
                    responseContent.MessageCodes.Add(InputValidation.MessageCodes.MilitaryNotNumeric);
                }
                else
                {
                    var militaryStatus = MilitaryStatuses.GetMilitaryStatus(parsedMilitaryAffiliation);
                    var isAvailable = militaryStatus == null ? false : militaryStatus.IsEnabled;
                    if (!isAvailable)
                    {
                        responseContent.MessageCodes.Add(InputValidation.MessageCodes.MilitaryAffiliationUnavailable);
                    }
                }
            }
        }
    }
}