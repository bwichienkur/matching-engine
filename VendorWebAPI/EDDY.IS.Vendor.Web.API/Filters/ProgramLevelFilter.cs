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
    public class ProgramLevelFilter : ActionFilterBase
    {
        public ProgramLevels ProgramLevels { get; set; }

        public ProgramLevelFilter()
        {
            ProgramLevels = new ProgramLevels();
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

            var programLevelIds = requestValues["ProgramLevelIds"];
            if (!string.IsNullOrEmpty(programLevelIds))
            {
                var programLevelIdsArray = JsonConvert.DeserializeObject<string[]>(programLevelIds);
                if (programLevelIdsArray != null)
                {
                    if (programLevelIdsArray.Length > 0)
                    {
                        if (isArrayInputNotNumeric(programLevelIdsArray))
                        {
                            responseContent.MessageCodes.Add(InputValidation.MessageCodes.ProgramLevelIdIsNotNumeric);
                        }
                        else
                        {
                            foreach (string programLevelIdString in programLevelIdsArray)
                            {
                                int programLevelIdInt;
                                if (int.TryParse(programLevelIdString, out programLevelIdInt))
                                {
                                    var programLevel = ProgramLevels.GetProgramLevel(programLevelIdInt);
                                    var programLevelAvailable = programLevel == null ? false : programLevel.IsEnabled;
                                    if (!programLevelAvailable)
                                    {
                                        responseContent.MessageCodes.Add(InputValidation.MessageCodes.ProgramLevelUnavailable);
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var programLevelId = requestValues["ProgramLevelId"];

            if (!string.IsNullOrEmpty(programLevelId))
            {
                if (isInputNotNumeric(programLevelId))
                {
                    responseContent.MessageCodes.Add(InputValidation.MessageCodes.ProgramLevelIdIsNotNumeric);
                }
            }

            responseContent.ResponseDateTime = DateTime.Now;
            invalidHttpResponseMessage.Content = new ObjectContent(responseContent.GetType(), responseContent, new JsonMediaTypeFormatter());

            if (responseContent.MessageCodes.Count > 0)
            {
                foreach(var messageCode in responseContent.MessageCodes)
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