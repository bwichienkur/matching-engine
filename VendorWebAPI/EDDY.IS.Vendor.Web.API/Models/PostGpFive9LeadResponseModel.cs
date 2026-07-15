using System;
using System.Collections.Generic;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using System.Diagnostics;
namespace EDDY.IS.Vendor.Web.API.Models
{
    public class PostGpFive9LeadResponseModel : VendorResponseBase
    {

        private GpFive9 gpFive9 = new GpFive9();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        public PostGpFive9LeadResponseModel(ContactRequest contactRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.IsSuccessful = true;
            this.Messages = new List<VendorResponseMessage>();
            VendorResponseBase routeLeadResponse = gpFive9.RouteLead(contactRequest);
            if (routeLeadResponse != null)
            {
                if (routeLeadResponse.IsSuccessful)
                {
                    this.Body = routeLeadResponse.Body;

                    VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ValidationPassed);
                    if (message != null)
                    {
                        this.Messages.Add(message);
                    }
                    message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.RoutedSuccessfully);
                    if (message != null)
                    {
                        this.Messages.Add(message);
                    }

                }
                else
                {
                    if (routeLeadResponse.Messages != null)
                    {
                        if (routeLeadResponse.Messages.Count > 0)
                        {

                            this.Messages = routeLeadResponse.Messages;
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