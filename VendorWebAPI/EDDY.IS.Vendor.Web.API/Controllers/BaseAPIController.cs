using System.Web.Http;
using EDDY.IS.Vendor.Web.API.Filters;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Entities;
using Newtonsoft.Json;
using System.Net.Http;
using System;
using static EDDY.IS.Vendor.Entities.VendorResponseBase;
using EDDY.IS.Vendor.Utilities;

namespace EDDY.IS.Vendor.Web.API.Controllers
{
    [CampaignAuthorizationFilter]
    public class BaseAPIController : ApiController
    {
        protected VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        protected Logs logs = new Logs();

        protected void logResponse(VendorResponseBase responseContent, Guid apiKey, HttpRequestMessage requestMessage, OperationType operationType, string email = null)
        {
            VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
            vendorResponseLog.APIKey = apiKey;
            vendorResponseLog.IPAddress = requestMessage.GetClientIpAddress();
            vendorResponseLog.EndPoint = requestMessage.RequestUri.AbsolutePath;
            vendorResponseLog.Request = requestMessage.Content.ReadAsStringAsync().Result;
            vendorResponseLog.Response = JsonConvert.SerializeObject(responseContent);
            vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
            vendorResponseLog.Messages = responseContent.Messages;
            vendorResponseLog.Email = email;
            vendorResponseLog.Operation = operationType;

            if (responseContent.Body != null)
            {
                switch (operationType)
                {
                    case OperationType.ProspectSave:
                        ProspectSubmissionResponse prospectSubmissionResponse = responseContent.Body as ProspectSubmissionResponse;
                        if (prospectSubmissionResponse != null)
                        {
                            vendorResponseLog.ProspectFlowId = prospectSubmissionResponse.ProspectFlowId;
                        }
                        break;
                    case OperationType.LeadSave:
                        LeadSubmissionResponse leadSubmissionResponse = responseContent.Body as LeadSubmissionResponse;
                        if (leadSubmissionResponse != null)
                        {
                            vendorResponseLog.LeadId = leadSubmissionResponse.LeadId;
                            vendorResponseLog.EstimatedRevShare = leadSubmissionResponse.EstimatedRevShare;
                        }
                        break;
                    default:
                        break;
                }
            }
            logs.LogEddyApiResponse(vendorResponseLog);
        }

        protected void logEdumaxResponse(VendorResponseBase responseContent, Guid apiKey, HttpRequestMessage requestMessage, string email = null)
        {
            VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
            vendorResponseLog.APIKey = apiKey;
            vendorResponseLog.IPAddress = requestMessage.GetClientIpAddress();
            vendorResponseLog.EndPoint = requestMessage.RequestUri.AbsolutePath;
            vendorResponseLog.Request = requestMessage.Content.ReadAsStringAsync().Result;
            vendorResponseLog.Response = JsonConvert.SerializeObject(responseContent);
            vendorResponseLog.IsSuccessful = responseContent.IsSuccessful;
            vendorResponseLog.Messages = responseContent.Messages;
            vendorResponseLog.Email = email;
            vendorResponseLog.Operation = OperationType.LeadSave;

            if (responseContent.Body != null)
            {
                EduMaxLeadSubmissionResponse leadSubmissionResponse = responseContent.Body as EduMaxLeadSubmissionResponse;
                if (leadSubmissionResponse != null)
                {
                    vendorResponseLog.LeadId = leadSubmissionResponse.LeadId;
                }
            }
            logs.LogEddyApiResponse(vendorResponseLog);
        }
    }
}
