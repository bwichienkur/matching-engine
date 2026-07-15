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
using System.Numerics;

namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class PagingActionFilter : ActionFilterBase
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
            var pageSize = requestValues["PageSize"];
            if (string.IsNullOrEmpty(pageSize))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PageSizeRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {
                int pageSizeResult;
                if (!int.TryParse(pageSize, out pageSizeResult))
                {
                    VendorResponseMessage message;
                    BigInteger pageSizeBigInt;
                    if (BigInteger.TryParse(pageSize, out pageSizeBigInt))
                    {
                        message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PageSizeExceedsIntegerSize);
                    }
                    else
                    {
                        message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PageSizeNotNumeric);
                    }

                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
                else
                {
                    if (pageSizeResult < 1)
                    {
                        VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.PageSizeLessThanZero);
                        if (message != null)
                        {
                            responseContent.Messages.Add(message);
                        }
                    }
                }
            }
            var startPage = requestValues["StartPage"];
            if (string.IsNullOrEmpty(startPage))
            {
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.StartIndexRequired);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }
            }
            else
            {
                int startPageResult;
                if (!int.TryParse(startPage, out startPageResult))
                {
                    VendorResponseMessage message;
                    BigInteger startPageBigInteger;
                    if (BigInteger.TryParse(startPage, out startPageBigInteger))
                    {
                        message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.StartIndexExceedsIntegerSize);
                    }
                    else
                    {
                        message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.StartIndexNotNumeric);
                    }

                    if (message != null)
                    {
                        responseContent.Messages.Add(message);
                    }
                }
                else
                {
                    if (startPageResult < 1)
                    {
                        VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.StartIndexLessThanZero);
                        if (message != null)
                        {
                            responseContent.Messages.Add(message);
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
                //vendorResponseLog.Operation = VendorResponseBase.OperationType.Validation;
                //logs.LogVendorResponse(vendorResponseLog);

                logResponse(responseContent, requestValues["apikey"], actionContext.Request, requestValues["email"]);
                actionContext.Response = invalidHttpResponseMessage;
            }

        }

    }
}