using System;
using EDDY.IS.Vendor.Entities;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess;
using System.Collections.Generic;
using Newtonsoft.Json;
using EDDY.IS.Vendor.Utilities;

namespace EDDY.IS.Vendor.Business
{
    public class Timezones : VendorBase
    {
        public TimezoneResponse GetTimezoneResponse(TimezoneRequest timezoneRequest)
        {
            TimezoneResponse response = null;
            try
            {
                response = timezoneServiceDAO.GetTimezones(timezoneRequest);
            }
            catch (Exception exc)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(exc, Policies.BUSINESS_POLICY);
            }
            return response;
        }

    }
}
