using EDDY.IS.Vendor.Business;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Vendor.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EDDY.IS.Vendor.Web.API.Filters
{
    public class DisabledCampusFilter : CampusFilter
    {
        public Campuses Campuses { get; set; }

        public DisabledCampusFilter()
        {
            Campuses = new Campuses();
        }

        override protected void PerformValidation(string campusId, ref VendorResponseBase responseContent)
        {
            base.PerformValidation(campusId, ref responseContent);
            int campusIdInt;
            if (int.TryParse(campusId, out campusIdInt))
            {
                var campus = Campuses.GetCampus(campusIdInt);
                var campusUnavailable = campus == null || !campus.IsEnabled;
                if (campusUnavailable)
                {
                    responseContent.MessageCodes.Add(InputValidation.MessageCodes.CampusUnavailable);
                }
            }
        }
    }
}