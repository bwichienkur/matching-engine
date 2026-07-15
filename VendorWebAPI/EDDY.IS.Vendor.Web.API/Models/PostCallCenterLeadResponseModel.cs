using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace EDDY.IS.Vendor.Web.API.Models
{
    public class PostCallCenterLeadResponseModel : VendorResponseBase
    {
        private Leads leads = new Leads();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        public PostCallCenterLeadResponseModel(ContactRequest contactRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;

            this.Messages = new List<VendorResponseMessage>();
            VendorResponseBase leadSaveResponse = leads.SaveCallCenterLead(contactRequest);
            if (leadSaveResponse != null)
            {
                this.IsSuccessful = leadSaveResponse.IsSuccessful;
                if (this.IsSuccessful)
                {
                    this.Body = leadSaveResponse.Body;
                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ValidationPassed);
                    if (message != null)
                    {
                        this.Messages.Add(message);
                    }
                }
                else if (leadSaveResponse.Messages?.Count > 0)
                {
                    this.Messages = leadSaveResponse.Messages;
                }
            }

            this.ResponseDateTime = DateTime.Now;
            stopwatch.Stop();
            this.TotalResponseTime = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}