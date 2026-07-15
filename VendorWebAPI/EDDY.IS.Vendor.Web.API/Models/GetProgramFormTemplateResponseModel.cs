using System;
using System.Collections.Generic;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using System.Diagnostics;
namespace EDDY.IS.Vendor.Web.API.Models
{
    public class GetProgramFormTemplateResponseModel : VendorResponseBase
    {
        private Programs programs = new Programs();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        public GetProgramFormTemplateResponseModel(ContactRequest contactRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.IsSuccessful = true;
            this.Messages = new List<VendorResponseMessage>();
           

            FormTemplate formTemplate= programs.GetProgramForm(contactRequest);
            VendorResponseMessage message = new VendorResponseMessage();
            if (formTemplate != null)
            {
                this.Body = formTemplate;
                this.Messages = new List<VendorResponseMessage>();
                message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ValidationPassed);
                if (message != null)
                {
                    this.Messages.Add(message);
                }
            }
            else
            {
                message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.NoResults);
                if (message != null)
                {
                    this.Messages.Add(message);
                }
            }
            this.ResponseDateTime = DateTime.Now;
            stopwatch.Stop();
            this.TotalResponseTime = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
        }
    }
}