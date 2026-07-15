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
        public class DeleteCampusesCacheForMicrositesResponseModel : VendorResponseBase
    {
            private Campuses campuses = new Campuses();
            private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();

            public DeleteCampusesCacheForMicrositesResponseModel(DirectoryRequest campusesRequest)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                this.ResponseGuid = Guid.NewGuid();
                this.RequestDateTime = DateTime.Now;
                this.IsSuccessful = true;
                this.Messages = new List<VendorResponseMessage>();
                campuses.DeleteCampusesCacheForMicrosite(campusesRequest);
                this.Messages = new List<VendorResponseMessage>();
            
                this.ResponseDateTime = DateTime.Now;
                stopwatch.Stop();
                this.TotalResponseTime = Convert.ToInt32(stopwatch.Elapsed.TotalMilliseconds);
            }
        }
    }