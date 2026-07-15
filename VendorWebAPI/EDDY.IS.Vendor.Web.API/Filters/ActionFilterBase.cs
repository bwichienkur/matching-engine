using System;
using System.Collections.Generic;
using System.Globalization;
using System.Numerics;
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

    public class ActionFilterBase : ActionFilterAttribute
    {
        protected VendorBase vendorBase = new VendorBase();
        protected VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        protected Locations locations = new Locations();
        protected Logs logs = new Logs();


        protected bool isArrayInputNotNumeric(string[] arrayInPut)
        {
            bool isNotNumeric = false;


            foreach (string arrayItem in arrayInPut)
            {

                if (isInputNotNumeric(arrayItem))
                {
                    isNotNumeric = true;
                    break;
                }
            }
            return isNotNumeric;
        }

        protected bool isInputNotNumeric(string inPut)
        {
            bool isNotNumeric = false;

            BigInteger parsedInstitutionId = 0;
            if (!BigInteger.TryParse(inPut, out parsedInstitutionId))
            {
                isNotNumeric = true;
            }
            return isNotNumeric;
        }

        protected bool isInputNotBoolean(string inPut)
        {
            bool isNotBool = false;

            bool parsedBool = false;
            if (!bool.TryParse(inPut, out parsedBool))
            {
                isNotBool = true;
            }
            return isNotBool;
        }
        protected bool isValidUrl(string inPut)
        {
            return Uri.IsWellFormedUriString(inPut, UriKind.Absolute);
        }
        
        protected VendorCampaign getVendorCampaignByTrackId(Guid trackId)
        {
            VendorCampaign vendorCampaign = null;
            try
            {
                VendorCampaigns vendorCampaigns = new VendorCampaigns();
                vendorCampaign = vendorCampaigns.GetVendorCampaignByTrackId(trackId);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.WCF_SERVICE_POLICY);
            }
            return vendorCampaign;
        }

        protected void logResponse(VendorResponseBase responseContent, string apiKey, HttpRequestMessage requestMessage, string email = null)
        {
            VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
            vendorResponseLog.APIKey = Guid.Parse(apiKey);
            vendorResponseLog.IPAddress = requestMessage.GetClientIpAddress();
            vendorResponseLog.EndPoint = requestMessage.RequestUri.AbsolutePath;
            vendorResponseLog.Request = requestMessage.Content.ReadAsStringAsync().Result;
            vendorResponseLog.Response = JsonConvert.SerializeObject(responseContent);
            vendorResponseLog.Email = email;
            vendorResponseLog.Operation = VendorResponseBase.OperationType.Validation;
            logs.LogEddyApiResponse(vendorResponseLog);
        }
    }
}