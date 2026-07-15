using System;
using System.Collections.Generic;
using System.Web.Mvc;

using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Net.Http.Formatting;
using EDDY.IS.Vendor.Utilities;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Web.API.Models;
using EDDY.IS.Common.Logging;
using EDDY.IS.Common.Utilities;
using EDDY.IS.Common.ExceptionHandler;
using Newtonsoft.Json;
using System.Web.Http.Cors;

namespace EDDY.IS.Vendor.Web.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "POST")]
    public class LeadDetailsController : Controller
    {
        protected VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        protected Logs logs = new Logs();


        [System.Web.Mvc.HttpGet]
        // GET: LeadDetails
        public ActionResult Index([FromUri] LeadDetailsRequest leadDetailsRequest)
        {
            LeadDetailsViewModel leadDetailsViewModel = null;
            try
            {

                leadDetailsViewModel = new LeadDetailsViewModel(leadDetailsRequest);
             

                VendorResponseLog vendorResponseLog = new VendorResponseLog();
                vendorResponseLog.APIKey = leadDetailsRequest.APIKey;
                vendorResponseLog.IPAddress = this.HttpContext.Request.UserHostAddress;
                vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.HttpContext.Request.QueryString).HtmlEncode();
               
                logs.LogVendorResponse(vendorResponseLog);
            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.WCF_SERVICE_POLICY);

                VendorResponseBase responseContent = new VendorResponseBase();

                responseContent.IsSuccessful = false;
                responseContent.ResponseGuid = Guid.NewGuid();
                responseContent.RequestDateTime = DateTime.Now;
                responseContent.IsSuccessful = false;
                responseContent.Messages = new List<VendorResponseMessage>();

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.GeneralException);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }

                responseContent.ResponseDateTime = DateTime.Now;
               

                VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                vendorResponseLog.APIKey = leadDetailsRequest.APIKey;
                vendorResponseLog.IPAddress = this.HttpContext.Request.UserHostAddress;
                vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.HttpContext.Request.QueryString).HtmlEncode();
                logs.LogVendorResponse(vendorResponseLog);


            }
            return View("LeadDetailsView", leadDetailsViewModel);
        }
        [System.Web.Mvc.HttpPost]
        public ActionResult SaveNote([FromBody] LeadDetailsNoteRequest leadDetailsNoteRequest)
        {
          
            try
            {
                Leads leads = new Leads();
                VendorResponseBase leadSaveNoteResponse = leads.SaveLeadNote(leadDetailsNoteRequest);


                VendorResponseLog vendorResponseLog = new VendorResponseLog();
                vendorResponseLog.APIKey = leadDetailsNoteRequest.APIKey;
                vendorResponseLog.IPAddress = this.HttpContext.Request.UserHostAddress;
                vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.HttpContext.Request.QueryString).HtmlEncode();

                logs.LogVendorResponse(vendorResponseLog);

            }
            catch (Exception exc)
            {
                EDDYLogger.LogMessage(this, LogLevel.Error, exc.Message);
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.WCF_SERVICE_POLICY);

                VendorResponseBase responseContent = new VendorResponseBase();

                responseContent.IsSuccessful = false;
                responseContent.ResponseGuid = Guid.NewGuid();
                responseContent.RequestDateTime = DateTime.Now;
                responseContent.IsSuccessful = false;
                responseContent.Messages = new List<VendorResponseMessage>();

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.GeneralException);
                if (message != null)
                {
                    responseContent.Messages.Add(message);
                }

                responseContent.ResponseDateTime = DateTime.Now;


                VendorResponseLog vendorResponseLog = logs.InitializeVendorResponseLogObject(responseContent);
                vendorResponseLog.APIKey = leadDetailsNoteRequest.APIKey;
                vendorResponseLog.IPAddress = this.HttpContext.Request.UserHostAddress;
                vendorResponseLog.MethodName = Log.GetCurrentMethodName();
                vendorResponseLog.RequestUrlParameters = JsonConvert.SerializeObject(this.HttpContext.Request.QueryString).HtmlEncode();
                logs.LogVendorResponse(vendorResponseLog);


            }
            return RedirectToAction("Index", "LeadDetails", new { APIKey = leadDetailsNoteRequest.APIKey, ProspectFlowID = leadDetailsNoteRequest.ProspectFlowId, AdvisorEmail = leadDetailsNoteRequest.AdvisorEmail });

        }
    }
}