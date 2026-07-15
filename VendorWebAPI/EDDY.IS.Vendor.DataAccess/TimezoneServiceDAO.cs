using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EDDY.IS.Common.ExceptionHandler;
using EDDY.IS.Vendor.DataAccess.DataModels;
using EDDY.IS.Vendor.Entities;


namespace EDDY.IS.Vendor.DataAccess
{
    public class TimezoneServiceDAO : VendorBaseDAO
    {
        public Entities.TimezoneResponse GetTimezones(TimezoneRequest timezoneRequest)
        {
            try
            {
                USZipCodeAreaCodeMerge timezoneData = this.getTimezonesFromDB(timezoneRequest);
                if (timezoneData != null)
                {
                    TimezoneResponse timezoneResponse = new TimezoneResponse();
                    timezoneResponse.Timezone = timezoneData.TimeZone;
                    timezoneResponse.Offset = timezoneData.UTC;
                    return timezoneResponse;
                }
            }
            catch (Exception ex)
            {
                ExceptionWrapper.ExceptionHandler.HandleException(ex, Policies.DATA_ACCESS_POLICY);
            }
            return null;
        }
    }
}