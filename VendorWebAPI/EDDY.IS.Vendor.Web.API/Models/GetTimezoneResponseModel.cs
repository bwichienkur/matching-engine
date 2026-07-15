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
    public class GetTimezoneResponseModel : VendorResponseBase
    {
        private Timezones timezones = new Timezones();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();
        private TimezoneResponse result = null;
        public GetTimezoneResponseModel(TimezoneRequest timezoneRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.Messages = new List<VendorResponseMessage>();

            string validationMsg = String.Empty;
            if(CheckIfValidRequest(timezoneRequest, out validationMsg))        
                result = timezones.GetTimezoneResponse(timezoneRequest);

            if (result != null)
            {
                this.IsSuccessful = true;
                this.Body = result;

                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ValidationPassed);
                if (message != null)                
                    this.Messages.Add(message);                
            }
            else
            {
                this.IsSuccessful = false;
                this.Body = !String.IsNullOrEmpty(validationMsg) ? new { FailureMessage=validationMsg } : new { FailureMessage = "No Timezone Results Found" };
                VendorResponseMessage message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.NoResults);
                if (message != null)
                    this.Messages.Add(message);
            }
            
            this.ResponseDateTime = DateTime.Now;
            stopwatch.Stop();
            this.TotalResponseTime = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
        }

        private bool CheckIfValidRequest(TimezoneRequest tzr, out string validationMsg)
        {
            validationMsg = string.Empty;

            if((tzr.StateCode == null || tzr.PostalCode == null) && tzr.PhoneNumber == null)
            {
                validationMsg = "Missing Required InputData: PhoneNumber & State/Zip";
                return false;
            }
            if (tzr.PhoneNumber != null && tzr.PhoneNumber.Length < 10)
            {
                validationMsg = "Invalid Phone Number";
                return false;
            }
            return true;
        }
    }
}