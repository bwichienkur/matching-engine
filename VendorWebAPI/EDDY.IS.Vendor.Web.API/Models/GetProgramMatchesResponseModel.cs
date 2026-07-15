using System;
using System.Collections.Generic;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using System.Diagnostics;
namespace EDDY.IS.Vendor.Web.API.Models
{
    public class GetProgramMatchesResponseModel : VendorResponseBase
    {
        private Programs programs = new Programs();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        public GetProgramMatchesResponseModel(ContactRequest contactRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.IsSuccessful = true;
            this.Messages = new List<VendorResponseMessage>();
            VendorResponseBase programMatches = programs.GetProgramMatches(contactRequest);
            if (programMatches != null)
            {
                if (programMatches.IsSuccessful)
                {
                    this.Body = programMatches.Body;

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ValidationPassed);
                    if (message != null)
                    {
                        this.Messages.Add(message);
                    }
                }
                else
                {
                    if (programMatches.Messages != null)
                    {
                        if (programMatches.Messages.Count > 0)
                        {

                            this.Messages = programMatches.Messages;
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