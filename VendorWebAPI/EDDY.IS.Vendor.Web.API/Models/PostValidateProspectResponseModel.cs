using System;
using System.Collections.Generic;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using System.Diagnostics;

namespace EDDY.IS.Vendor.Web.API.Models
{
    public class PostValidateProspectResponseModel : VendorResponseBase
    {

        private Prospects prospects = new Prospects();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        public PostValidateProspectResponseModel()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.IsSuccessful = true;
            this.Messages = new List<VendorResponseMessage>();

            this.Body = null;

            VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ProspectIsValid);
            if (message != null)
            {
                this.Messages.Add(message);
            }



            this.ResponseDateTime = DateTime.Now;


            stopwatch.Stop();
            this.TotalResponseTime = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
        }

    }
}