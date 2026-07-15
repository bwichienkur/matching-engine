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
using EDDY.IS.Common.ExceptionHandler;

namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class CategoryFilter : ActionFilterBase
    {
        public Categories Categories { get; set; }

        public CategoryFilter()
        {
            Categories = new Categories();
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

            var categoryIds = requestValues["categoryIds"];
            PerformValidation(categoryIds, actionContext, ref responseContent);
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

        protected virtual void PerformValidation(string categoryIds, HttpActionContext actionContext, ref VendorResponseBase responseContent)
        {
            if (!string.IsNullOrEmpty(categoryIds))
            {
                var categoryIdsArray = JsonConvert.DeserializeObject<string[]>(categoryIds);
                if (categoryIdsArray != null)
                {
                    if (categoryIdsArray.Length > 0)
                    {
                        var actionName = actionContext.ActionDescriptor?.ActionName;
                        var controllerName = actionContext.ControllerContext.ControllerDescriptor?.ControllerName;
                        var isLeadSave = (controllerName?.ToLower() == "lead" && actionName?.ToLower() == "save");

                        if (isArrayInputNotNumeric(categoryIdsArray))
                        {
                            if (!isLeadSave)
                            {
                                responseContent.MessageCodes.Add(InputValidation.MessageCodes.CategoryIdsNotNumeric);
                            }
                            else
                            {
                                ContactRequest contactRequest = (ContactRequest)actionContext.ActionArguments["contactRequest"];
                                contactRequest.CategoryIds = new List<int>();
                            }
                        }
                    }
                }
            }
        }
    }
}