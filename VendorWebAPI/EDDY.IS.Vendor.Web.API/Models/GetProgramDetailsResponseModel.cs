using System;
using System.Collections.Generic;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using System.Diagnostics;
namespace EDDY.IS.Vendor.Web.API.Models
{
    public class GetProgramDetailsResponseModel : VendorResponseBase
    {
        private Programs programs = new Programs();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        public GetProgramDetailsResponseModel(DirectoryRequest programRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.IsSuccessful = true;
            this.Messages = new List<VendorResponseMessage>();
            Program program = programs.GetProgramDetails(programRequest);
            VendorResponseMessage message = new VendorResponseMessage();
            if (program != null)
            {
                this.Body = program;
                this.Messages = new List<VendorResponseMessage>();
                message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ValidationPassed);
                if (message != null)
                {
                    this.Messages.Add(message);
                }
            }
            else {
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