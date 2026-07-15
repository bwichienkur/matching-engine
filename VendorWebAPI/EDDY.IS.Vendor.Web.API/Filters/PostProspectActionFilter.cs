using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Net.Http.Formatting;

using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Common.Utilities;
using Newtonsoft.Json;

namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class PostProspectActionFilter : ActionFilterBase
    {

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
            ContactRequest request = actionContext.Request.GetRequestJsonBodyContactRequest();

            string firstName = requestValues["FirstName"];
            if (string.IsNullOrEmpty(firstName))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.FirstNameRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }

            string phone = requestValues["Phone"];
            if (string.IsNullOrEmpty(phone))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PhoneRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {
                if (phone.Length > 20)
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PhoneMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }

                if (this.isInputNotNumeric(phone))
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PhoneNotNumeric);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
            }

            string alternatePhone = requestValues["AlternatePhone"];
            if (!string.IsNullOrEmpty(alternatePhone))
            {
                if (alternatePhone.Length > 20)
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.AlternatePhoneMaxLength);
                    if (message != null)
                        responseContent.Messages.Add(message);
                }

                if (this.isInputNotNumeric(alternatePhone))
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.AlternatePhoneNotNumeric);
                    if (message != null)
                        responseContent.Messages.Add(message);
                }
            }

            string email = requestValues["Email"];
            if (string.IsNullOrEmpty(email))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.EmailRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {
                if (email.Length > 50)
                {

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.EmailMaxLength);
                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
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
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Validation;

                //logs.LogVendorResponse(vendorResponseLog);
                logResponse(responseContent, requestValues["apikey"], actionContext.Request, requestValues["email"]);

                actionContext.Response = invalidHttpResponseMessage;
            }
        }
    }
}