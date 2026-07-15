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
   
    public class ProgramFilter : ActionFilterBase
    {
        public Programs Programs { get; set; }

        public ProgramFilter()
        {
            Programs = new Programs();
        }

        public override void OnActionExecuting(HttpActionContext actionContext)
        {
            HttpResponseMessage invalidHttpResponseMessage = new HttpResponseMessage(HttpStatusCode.BadRequest);
            VendorResponseBase responseContent = new VendorResponseBase();
            responseContent.ResponseGuid = Guid.NewGuid();
            responseContent.RequestDateTime = DateTime.Now;
            responseContent.IsSuccessful = false;

            responseContent.Messages = new List<VendorResponseMessage>();
            responseContent.MessageCodes = new List<String>();
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
            var programId = requestValues["ProgramId"];
            if (!string.IsNullOrEmpty(programId))
            {
                if (this.isInputNotNumeric(programId))
                {
                    responseContent.MessageCodes.Add(InputValidation.MessageCodes.ProgramIdIsNotNumeric);
                }
                else
                {
                    var program = Programs.GetProgram(int.Parse(programId));
                    var programAvailable = program == null ? false : program.IsEnabled;
                    if (!programAvailable)
                    {
                        responseContent.MessageCodes.Add(InputValidation.MessageCodes.ProgramUnavailable);
                    }
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