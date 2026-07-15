using System;
using System.Collections.Generic;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using System.Diagnostics;
namespace EDDY.IS.Vendor.Web.API.Models
{
    public class GetLeadDetailsResponseModel : VendorResponseBase
    {
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        private Leads leads = new Leads();
        public GetLeadDetailsResponseModel(LeadDetailsRequest leadDetailsRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.IsSuccessful = true;
            this.Messages = new List<VendorResponseMessage>();
            VendorResponseBase leadSaveResponse = leads.GetLeadDetails(leadDetailsRequest);
            if (leadSaveResponse != null)
            {
                if (leadSaveResponse.IsSuccessful)
                {
                    this.Body = leadSaveResponse.Body;

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ValidationPassed);
                    if (message != null)
                    {
                        this.Messages.Add(message);
                    }
                }
                else
                {
                    if (leadSaveResponse.Messages != null)
                    {
                        if (leadSaveResponse.Messages.Count > 0)
                        {

                            this.Messages = leadSaveResponse.Messages;
                        }
                    }
                }
            }

            this.ResponseDateTime = DateTime.Now;
            stopwatch.Stop();
            this.TotalResponseTime = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}