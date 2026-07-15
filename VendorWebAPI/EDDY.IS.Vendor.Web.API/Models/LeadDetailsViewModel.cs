using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Business;

namespace EDDY.IS.Vendor.Web.API.Models
{
    public class LeadDetailsViewModel 
    {
        private Leads leads = new Leads();
        private LeadDetails leadDetails;
        private LeadNote leadNote;

        public LeadNote LeadNote
        {
            get
            {
                return leadNote;
            }

            set
            {
                leadNote = value;
            }
        }

        public LeadDetails LeadDetails
        {
            get
            {
                return leadDetails;
            }

            set
            {
                leadDetails = value;
            }
        }

        public LeadDetailsViewModel(LeadDetailsRequest leadDetailsRequest) {
            VendorResponseBase leadSaveResponse = leads.GetLeadDetails(leadDetailsRequest);
            this.leadDetails = new LeadDetails();
            this.leadNote = new LeadNote();
            if (leadSaveResponse != null) {
                if (leadSaveResponse.Body != null)
                {
                    this.leadDetails = leadSaveResponse.Body as LeadDetails;

                }
            }
         
        }
    }
}