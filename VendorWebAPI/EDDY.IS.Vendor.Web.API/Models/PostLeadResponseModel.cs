using System;
using System.Collections.Generic;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using System.Diagnostics;
namespace EDDY.IS.Vendor.Web.API.Models
{
    public class PostLeadResponseModel : VendorResponseBase
    {

        private Leads leads = new Leads();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        public PostLeadResponseModel(ContactRequest contactRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;   
            this.Messages = new List<VendorResponseMessage>();

            VendorResponseBase leadSaveResponse = leads.SaveLead(contactRequest);
            if (leadSaveResponse != null)
            {
                if (leadSaveResponse.IsSuccessful)
                {
                    this.IsSuccessful = true;
                    this.Body = leadSaveResponse.Body;

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ValidationPassed); // need to add messages / message codes? is this only in the code or is it in the db too?
                    if (message != null)
                    {
                        this.Messages.Add(message);
                    }
                }
                else
                {
                    this.IsSuccessful = false;
                    this.Body = leadSaveResponse.Body;
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