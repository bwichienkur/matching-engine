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
    //Response body returns CampusList where each Campus has a list of Programs
    public class GetCampusForMicrositesResponseModel : VendorResponseBase
    {
        private Campuses campuses = new Campuses();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();

        public GetCampusForMicrositesResponseModel(DirectoryRequest campusesRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.IsSuccessful = true;
            this.Messages = new List<VendorResponseMessage>();
            List<CampusMicrosite> CampusMicrositeList = new List<CampusMicrosite>();
            VendorAPIList vendorAPIList = campuses.GetCampusesForMicrosite(campusesRequest, out CampusMicrositeList);
            this.Messages = new List<VendorResponseMessage>();
            VendorResponseMessage message = new VendorResponseMessage();
            if (vendorAPIList != null)
            {

                message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.ValidationPassed);
                if (message != null)
                {
                    this.Messages.Add(message);
                }
                if (vendorAPIList.ItemList.Count > 0)
                {
                    this.Body = vendorAPIList;
                }
                else
                {

                    message = vendorResponseMessages.GetVendorResponseMessageByMessageCode(InputValidation.MessageCodes.NoResults);
                    if (message != null)
                    {
                        this.Messages.Add(message);
                    }
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