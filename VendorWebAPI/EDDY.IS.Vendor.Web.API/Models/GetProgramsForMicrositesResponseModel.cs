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
    public class GetProgramForMicrositesResponseModel : VendorResponseBase
    {
        private Programs programs = new Programs();
        private VendorResponseMessages vendorResponseMessages = new VendorResponseMessages();


        //Response body returns ProgramList where each Program has a list of Campuses
        //Swapped fields on Microsite (ProgramIds will not match - check when saving lead for correct ProgramId)
        public GetProgramForMicrositesResponseModel(DirectoryRequest campusesRequest)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            this.ResponseGuid = Guid.NewGuid();
            this.RequestDateTime = DateTime.Now;
            this.IsSuccessful = true;
            this.Messages = new List<VendorResponseMessage>();

            List<ProgramMicrosite> ProgramMicrositeList = new List<ProgramMicrosite>();
            VendorAPIList vendorAPIList = programs.GetProgramsForMicrosite(campusesRequest, out ProgramMicrositeList);

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