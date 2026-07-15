using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Utilities;
using System.Diagnostics;

namespace EDDY.IS.Vendor.Web.API.Models
{
    public class GetZipsResponseModel : VendorResponseBase
    {
        private Zips zips = new Zips();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();

        public GetZipsResponseModel(DirectoryRequest zipRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.IsSuccessful = true;
            this.Messages = new List<VendorResponseMessage>();
            ZipResponse zip = zips.GetZipCodes(zipRequest);
            VendorResponseMessage message = new VendorResponseMessage();
            if (zip != null)
            {
                this.Body = zip;
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