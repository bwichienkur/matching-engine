using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess.DataModels;
using System.Configuration;
using Newtonsoft.Json;

namespace EDDY.IS.Vendor.DataAccess
{
    public class LogsDAO : VendorBaseDAO
    {
        public VendorResponseBase LogVendorResponse(VendorResponseLog log)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                bool logResponseBody = false;
                switch (log.Operation) {
                    case VendorResponseBase.OperationType.LeadSave:
                    case VendorResponseBase.OperationType.ProspectSave:
                    case VendorResponseBase.OperationType.GPFive9LeadSave:
                        logResponseBody = true;
                        break;
                    default:
                        if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("VerboseLogging")))
                        {
                            logResponseBody = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("VerboseLogging"));
                        }
                        break;

                }

                if (!logResponseBody)
                {
                    log.Body = null;
                }
                else {
                    if (!log.IsSuccessful) {
                        log.Body = JsonConvert.SerializeObject(log.Messages);
                    }
                }

                APILog apiLog = new APILog
                {
                    ResponseGuid = log.ResponseGuid,
                    RequestTimeStamp = log.RequestDateTime,
                    ResponseTimeStamp = log.ResponseDateTime,
                    TotalResponseTime = log.TotalResponseTime,
                    Response = JsonConvert.SerializeObject(log),
                    MethodName = log.MethodName,
                    IPAddress = log.IPAddress,
                    TrackID = log.APIKey,
                    PostData = log.RequestParameters,
                    OperationType = log.Operation.ToString(),
                    IsSuccessful = log.IsSuccessful,
                    ResponseBody = JsonConvert.SerializeObject(log.Body),
                    OperationValue = log.OperationValue
                };
                using (var eddyLoggingContext = new EddyLoggingEntities())
                {
                    eddyLoggingContext.APILogs.Add(apiLog);
                    eddyLoggingContext.SaveChanges();
                }

            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return vendorResponseBase;
        }

        //new logging
        public VendorResponseBase LogEddyApiResponse(VendorResponseLog log)
        {
            VendorResponseBase vendorResponseBase = null;
            try
            {
                bool logResponseBody = false;
                string lowercaseEndpoint = log.EndPoint.ToLower();
                if (lowercaseEndpoint.Contains("lead") || lowercaseEndpoint.Contains("prospect"))
                {
                    logResponseBody = true;
                }
                else
                {
                    if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings.Get("VerboseLogging")))
                    {
                        logResponseBody = Convert.ToBoolean(ConfigurationManager.AppSettings.Get("VerboseLogging"));
                    }
                }

                if (!logResponseBody)
                {
                    log.Body = null;
                }
                else
                {
                    if (!log.IsSuccessful)
                    {
                        log.Body = JsonConvert.SerializeObject(log.Messages);
                    }
                }

                EddyApiLog eddyApiLog = new EddyApiLog
                {
                    ResponseGuid = log.ResponseGuid,
                    RequestTimeStamp = log.RequestDateTime,
                    TotalResponseTime = log.TotalResponseTime,
                    IPAddress = log.IPAddress,
                    TrackID = log.APIKey,
                    ApiEndpoint = log.EndPoint,
                    Request = log.Request,
                    IsSuccessful = log.IsSuccessful,
                    Response = logResponseBody ? log.Response : null,
                    LeadId = log.LeadId,
                    ProspectFlowId = log.ProspectFlowId,
                    EmailAddress = !string.IsNullOrEmpty(log.Email) ? log.Email: null,
                    FormattedMessage = null,
                    ResponseTimeStamp = log.ResponseDateTime,
                    EstimatedRevShare = log.EstimatedRevShare
                };

                if (logResponseBody)
                {
                    eddyApiLog.FormattedMessage = string.Empty;
                    for (int i = 0; i < log.Messages.Count; i++)
                    {
                        eddyApiLog.FormattedMessage += log.Messages[i].Message;
                        if (i != log.Messages.Count - 1)
                        {
                            eddyApiLog.FormattedMessage += "|";
                        }
                    }
                }

                using (var eddyLoggingContext = new EddyLoggingEntities())
                {
                    eddyLoggingContext.EddyApiLogs.Add(eddyApiLog);
                    eddyLoggingContext.SaveChanges();
                }

            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.DATA_ACCESS_POLICY);
            }
            return vendorResponseBase;
        }
    }
}
