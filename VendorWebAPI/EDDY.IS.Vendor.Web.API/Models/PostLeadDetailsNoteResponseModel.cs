using System;
using System.Collections.Generic;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using System.Diagnostics;
namespace EDDY.IS.Vendor.Web.API.Models
{
    public class PostLeadDetailsNoteResponseModel : VendorResponseBase
    {

        private Leads leads = new Leads();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        public PostLeadDetailsNoteResponseModel(LeadDetailsNoteRequest leadDetailsNoteRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.IsSuccessful = true;
            this.Messages = new List<VendorResponseMessage>();
            VendorResponseBase leadSaveNoteResponse = leads.SaveLeadNote(leadDetailsNoteRequest);
            if (leadSaveNoteResponse != null)
            {
                if (leadSaveNoteResponse.IsSuccessful)
                {
                    this.Body = leadSaveNoteResponse.Body;

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ValidationPassed);
                    if (message != null)
                    {
                        this.Messages.Add(message);
                    }
                }
                else
                {
                    if (leadSaveNoteResponse.Messages != null)
                    {
                        if (leadSaveNoteResponse.Messages.Count > 0)
                        {

                            this.Messages = leadSaveNoteResponse.Messages;
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